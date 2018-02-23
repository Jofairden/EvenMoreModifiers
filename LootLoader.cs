using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Loot.Modifiers
{
    internal struct RarityMap
    {
        public string Name;
        public ModifierRarity Rarity;
    }

    internal struct EffectMap
    {
        public string Name;
        public ModifierEffect Effect;
    }

    internal struct ModifierMap
    {
        public string Name;
        public RarityMap RarityMap;
        public IEnumerable<EffectMap> EffectMaps;
    }

    /// <summary>
    /// The loader, handles all loading
    /// </summary>
    public static class LootLoader
    {
        //public static class OurRarities
        //{
        //	public static ModifierRarity Common = new ModifierRarity("Common", 0f, Color.White, RequirementPass.MatchStrengthPass);
        //	public static ModifierRarity Uncummon = new ModifierRarity("Uncummon", 1f, Color.Orange, RequirementPass.MatchStrengthPass);
        //	public static ModifierRarity Rare = new ModifierRarity("Rare", 2f, Color.Yellow, RequirementPass.MatchStrengthPass);
        //	public static ModifierRarity Legendary = new ModifierRarity("Legendary", 4f, Color.Red, RequirementPass.MatchStrengthPass);
        //	public static ModifierRarity Transcendent = new ModifierRarity("Transcendent", 8f, Color.Purple, RequirementPass.MatchStrengthPass);
        //}

        private static uint rarityNextID = 0;
        private static uint effectNextID = 0;
        private static uint modifierNextID = 0;
        internal static IDictionary<string, List<RarityMap?>> RaritiesMap = new Dictionary<string, List<RarityMap?>>();
        internal static IDictionary<string, List<EffectMap?>> EffectsMap = new Dictionary<string, List<EffectMap?>>();
        internal static IDictionary<string, List<ModifierMap?>> ModifiersMap = new Dictionary<string, List<ModifierMap?>>();
        internal static IDictionary<uint, ModifierRarity> Rarities = new Dictionary<uint, ModifierRarity>();
        internal static IDictionary<uint, ModifierEffect> Effects = new Dictionary<uint, ModifierEffect>();
        internal static IDictionary<uint, Modifier> Modifiers = new Dictionary<uint, Modifier>();

        internal static IDictionary<string, Assembly> Mods = new ConcurrentDictionary<string, Assembly>();

        internal static void Load()
        {
            RegisterMod(Loot.Instance);
        }

        internal static void Unload()
        {
            RaritiesMap = null;
            EffectsMap = null;
            ModifiersMap = null;
            Rarities = null;
            Effects = null;
            Modifiers = null;
            Mods = null;
        }

        /// <summary>
        /// Registers specified mod, enabling autoloading for that mod
        /// </summary>
        /// <param name="mod"></param>
		public static void RegisterMod(Mod mod)
        {
            bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
            if (b != null && !b.Value)
            {
                throw new Exception("RegisterMod can only be called from Mod.Load or Mod.Autoload");
            }

            if (Mods.ContainsKey(mod.Name))
            {
                throw new Exception($"Mod {mod.Name} is already registered");
            }

            Mods.Add(new KeyValuePair<string, Assembly>(mod.Name, mod.Code));
            RaritiesMap.Add(new KeyValuePair<string, List<RarityMap?>>(mod.Name, new List<RarityMap?>()));
            EffectsMap.Add(new KeyValuePair<string, List<EffectMap?>>(mod.Name, new List<EffectMap?>()));
            ModifiersMap.Add(new KeyValuePair<string, List<ModifierMap?>>(mod.Name, new List<ModifierMap?>()));
        }

        internal static uint ReserveRarityID()
        {
            uint reserved = rarityNextID;
            rarityNextID++;
            return reserved;
        }

        internal static uint ReserveEffectID()
        {
            uint reserved = effectNextID;
            effectNextID++;
            return reserved;
        }

        internal static uint ReserveModifierID()
        {
            uint reserved = modifierNextID;
            modifierNextID++;
            return reserved;
        }

        internal static Modifier GetWeightedModifier(ModifierContext ctx)
		{
			var wr = new WeightedRandom<Modifier>();
            foreach (var m in Modifiers.Where(x => x.Value._CanApply(ctx)))
				wr.Add(m.Value, m.Value.Weight);
			return wr.Get();
		}

        /// <summary>
        /// Returns the ModifierRarity specified by type, null if not present
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ModifierRarity GetRarity(uint type)
        {
            return type < rarityNextID ? Rarities[type] : null;
        }

        /// <summary>
        /// Returns the ModifierEffect specified by type, null if not present
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ModifierEffect GetEffect(uint type)
        {
            return type < effectNextID ? Effects[type] : null;
        }

        /// <summary>
        /// Returns the Modifier specified by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Modifier GetModifier(uint type)
        {
            return type < modifierNextID ? Modifiers[type] : null;
        }

        /// <summary>
        /// Will iterate registered mods, and attempt to autoload their rarities and effects
        /// </summary>
		internal static void SetupContent()
        {
            //public static ModifierRarity Common = new ModifierRarity("Common", 0f, Color.White);
            //public static ModifierRarity Uncommon = new ModifierRarity("Uncommon", 1f, Color.Orange);
            //public static ModifierRarity Rare = new ModifierRarity("Rare", 2f, Color.Yellow);
            //public static ModifierRarity Legendary = new ModifierRarity("Legendary", 4f, Color.Red);
            //public static ModifierRarity Transcendent = new ModifierRarity("Transcendent", 8f, Color.Purple);

            foreach (var kvp in Mods)
            {
				var ordered = kvp.Value
                    .GetTypes()
                    .OrderBy(x => x.FullName, StringComparer.InvariantCulture)
                    .Where(t => t.IsClass && !t.IsAbstract); /* || type.GetConstructor(new Type[0]) == null*/
				var rarities = ordered.Where(x => x.IsSubclassOf(typeof(ModifierRarity)));
				var effects = ordered.Where(x => x.IsSubclassOf(typeof(ModifierEffect)));
				var modifiers = ordered.Where(x => x.IsSubclassOf(typeof(Modifier)));

                // important: load things in order. (modifiers relies on all.. etc.)
				foreach (Type type in rarities)
				{
					AutoloadRarity(type, ModLoader.GetMod(kvp.Key));
				}

				foreach (Type type in effects)
				{
					AutoloadEffect(type, ModLoader.GetMod(kvp.Key));
				}

				foreach (Type type in modifiers)
				{
					AutoloadModifier(type, ModLoader.GetMod(kvp.Key));
				}
            }

            //ErrorLogger.ClearLog();
            //ErrorLogger.Log(string.Join("\n", Rarities.Select(r => r.Value.Name)));
            //ErrorLogger.Log(string.Join("\n", Effects.Select(e => e.Value.Description)));
        }

        private static void AutoloadModifier(Type type, Mod mod)
        {
            Modifier modifier = (Modifier)Activator.CreateInstance(type);
            AddModifier(modifier, mod);
        }

        public static void AddModifier(Modifier modifier, Mod mod)
        {
            bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
            if (b != null && !b.Value)
            {
                throw new Exception("AddModifier can only be called from Mod.Load or Mod.Autoload");
            }
            if (!Mods.ContainsKey(mod.Name))
            {
                throw new Exception($"Mod {mod.Name} is not registered, please register before adding");
            }

            List<ModifierMap?> lmm;
            if (!ModifiersMap.TryGetValue(mod.Name, out lmm))
            {
                throw new Exception($"ModifierMaps for {mod.Name} not found");
            }
            else if (lmm.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(modifier.Name)) != null)
            {
                throw new Exception($"You have already added a modifier with the name {modifier.Name}");
            }

            modifier.Mod = mod;
            modifier.Type = ReserveModifierID();
            Modifiers[modifier.Type] = modifier;
            var lem = modifier.Effects.Select(x => new EffectMap { Name = x.Name, Effect = x });
            ModifiersMap[mod.Name].Add(new ModifierMap { Name = modifier.Name, EffectMaps = lem, RarityMap = { Name = modifier.Rarity.Name, Rarity = modifier.Rarity } });
        }

        /// <summary>
        /// Autoloads a rarity
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mod"></param>
		private static void AutoloadRarity(Type type, Mod mod)
        {
            ModifierRarity rarity = (ModifierRarity)Activator.CreateInstance(type);
            AddRarity(rarity, mod);
        }

        /// <summary>
        /// Adds a rarity
        /// </summary>
        /// <param name="rarity"></param>
        /// <param name="mod"></param>
		public static void AddRarity(ModifierRarity rarity, Mod mod)
        {
            bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
            if (b != null && !b.Value)
            {
                throw new Exception("AddRarity can only be called from Mod.Load or Mod.Autoload");
            }
            if (!Mods.ContainsKey(mod.Name))
            {
                throw new Exception($"Mod {mod.Name} is not registered, please register before adding");
            }

            List<RarityMap?> lrm;
            if (!RaritiesMap.TryGetValue(mod.Name, out lrm))
            {
                throw new Exception($"RarityMaps for {mod.Name} not found");
            }
            else if (lrm.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(rarity.Name)) != null)
            {
                throw new Exception($"You have already added a rarity with the name {rarity.Name}");
            }

            rarity.Mod = mod;
            rarity.Type = ReserveRarityID();
            Rarities[rarity.Type] = rarity;
            RaritiesMap[mod.Name].Add(new RarityMap { Name = rarity.Name, Rarity = rarity });
        }

        /// <summary>
        /// Autoloads an effect
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mod"></param>
		private static void AutoloadEffect(Type type, Mod mod)
        {
            ModifierEffect effect = (ModifierEffect)Activator.CreateInstance(type);
            AddEffect(effect, mod);
        }

        /// <summary>
        /// Adds an effect
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="mod"></param>
		public static void AddEffect(ModifierEffect effect, Mod mod)
        {
            bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
            if (b != null && !b.Value)
            {
                throw new Exception("AddEffect can only be called from Mod.Load or Mod.Autoload");
            }

            List<EffectMap?> lem;
            if (!EffectsMap.TryGetValue(mod.Name, out lem))
            {
                throw new Exception($"EffectMaps for {mod.Name} not found");
            }
            else if (lem.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(effect.Name)) != null)
            {
                throw new Exception($"You have already added a effect with the name {effect.Name}");
            }

            effect.Mod = mod;
            effect.Type = ReserveEffectID();
            Effects[effect.Type] = effect;
            EffectsMap[mod.Name].Add(new EffectMap { Name = effect.Name, Effect = effect });
        }

        internal static Exception ThrowException(string message)
            => new Exception($"{Loot.Instance.DisplayName ?? "EvenMoreModifiers"}: {message}");

        /// <summary>
        /// Requests all effects, and returns them as a readonly collection
        /// </summary>
        /// <returns></returns>
		public static IReadOnlyCollection<ModifierEffect> RequestEffects()
        {
            return Effects.Select(e => e.Value).ToList().AsReadOnly();
        }

        /// <summary>
        /// Requests all rarities, and returns them as a readonly collection
        /// </summary>
        /// <returns></returns>
		public static IReadOnlyCollection<ModifierRarity> RequestRarities()
        {
            return Rarities.Select(r => r.Value).ToList().AsReadOnly();
        }

        /// <summary>
        /// Requests all modifiers, and returns them as a readonly collection
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Modifier> RequestModifiers()
        {
            return Modifiers.Select(m => m.Value).ToList().AsReadOnly();
        }

    }
}
