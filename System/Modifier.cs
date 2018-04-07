using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.System
{
	public struct ModifierTooltipLine
	{
		public string Text;
		public Color? Color;
	}

	public struct ModifierProperties
	{
		public float MinMagnitude { get; private set; }
		public float MaxMagnitude { get; private set; }
		public float MagnitudeStrength { get; private set; }
		public float BasePower { get; private set; }
		public float RarityLevel { get; private set; }
		public float RollChance { get; private set; }
		public int RoundPrecision { get; private set; }
		public float Magnitude { get; private set; }
		private float _power;
		public float Power
		{
			get { return _power; }
			private set
			{
				_power = value;
				RoundedPower = (float)Math.Round(value, RoundPrecision);
			}
		}
		public float RoundedPower
		{
			get;
			private set;
		}

		public ModifierProperties(float minMagnitude = 1f, float maxMagnitude = 1f, float magnitudeStrength = 1f, float basePower = 1f, float rarityLevel = 1f, float rollChance = 1f, int roundPrecision = 1, ModifierProperties? source = null) : this()
		{
			if (source.HasValue)
				this = source.Value;

			Set(minMagnitude, maxMagnitude, magnitudeStrength, basePower, rarityLevel, rollChance, roundPrecision);
		}

		public ModifierProperties Set(float minMagnitude = 1f, float maxMagnitude = 1f, float magnitudeStrength = 1f, float basePower = 1f, float rarityLevel = 1f, float rollChance = 1f, int roundPrecision = 0)
		{
			if (minMagnitude != MinMagnitude)
				MinMagnitude = minMagnitude;

			if (maxMagnitude != MaxMagnitude)
				MaxMagnitude = maxMagnitude;

			if (magnitudeStrength != MagnitudeStrength)
				MagnitudeStrength = magnitudeStrength;

			if (basePower != BasePower)
				BasePower = basePower;

			if (rarityLevel != RarityLevel)
				RarityLevel = rarityLevel;

			if (rollChance != RollChance)
				RollChance = rollChance;

			if (roundPrecision != RoundPrecision)
				RoundPrecision = roundPrecision;

			return this;
		}

		public ModifierProperties RollMagnitudeAndPower(float? magnitude = null, float? power = null)
		{
			/* Roll power TODO support /luck/ stat */
			Magnitude = magnitude ?? (MinMagnitude + Main.rand.NextFloat() * (MaxMagnitude - MinMagnitude)) * MagnitudeStrength;
			Power = power ?? BasePower * Magnitude;
			return this;
		}

		public TagCompound Save()
		{
			return new TagCompound
			{
				{"Magnitude", Magnitude},
				{"Power", Power}
			};
		}

		public static ModifierProperties Load(TagCompound tag)
		{
			try
			{
				return new ModifierProperties().RollMagnitudeAndPower(tag.GetFloat("Magnitude"), tag.GetFloat("Power"));
			}
			catch (Exception)
			{
				return new ModifierProperties().RollMagnitudeAndPower();
			}
		}
	}


	/// <summary>
	/// Defines a modifier, which is an unloaded GlobalItem
	/// Making it a GlobalItem gives easy access to all hooks
	/// The various hooks are called by our own GlobalItem
	/// In your Modifier, it is safe to assume when one of the hooks is called, that item currently is modified by this modifier
	/// </summary>
	public abstract class Modifier : GlobalItem, ICloneable, IRollable
	{
		public Mod Mod { get; internal set; }
		public uint Type { get; internal set; }
		public new virtual string Name => GetType().Name;

		public ModifierProperties Properties { get; internal set; }

		// Must be getter due to various fields that can change interactively
		public virtual ModifierTooltipLine[] Description { get; }

		/// <summary>
		/// Returns the Modifier specified by type, null if not present
		/// </summary>
		public static Modifier GetModifier(ushort type)
			=> EMMLoader.GetModifier(type);

		public Modifier AsNewInstance()
			=> (Modifier)Activator.CreateInstance(GetType());

		public virtual ModifierProperties GetModifierProperties(Item item)
		{
			return new ModifierProperties();
		}

		/// <summary>
		/// If this Modifier can roll/apply 
		/// </summary>
		public virtual bool CanRoll(ModifierContext ctx)
			=> true;

		public virtual bool CanApplyCraft(ModifierContext ctx)
			=> true;

		public virtual bool CanApplyPickup(ModifierContext ctx)
			=> true;

		public virtual bool CanApplyReforge(ModifierContext ctx)
			=> true;

		/// <summary>
		/// Allows modders to do something when this modifier is applied
		/// If a modder needs ModPlayer hooks, they should make their own ModPlayer and apply fields using this hook
		/// </summary>
		public virtual void Apply(Item item)
		{

		}

		/// <summary>
		/// Allows modders to do custom cloning here
		/// Happens after default cloning, which clones various info (mod, type, magnitude and power)
		/// </summary>
		public virtual void Clone(ref Modifier clone)
		{

		}

		public new object Clone()
		{
			Modifier clone = (Modifier)MemberwiseClone();
			clone.Mod = Mod;
			clone.Type = Type;
			clone.Properties = Properties;
			Clone(ref clone);
			return clone;
		}

		/// <summary>
		/// Allows modder to do custom loading here
		/// Use the given TC to pull data you saved using <see cref="Save(TagCompound)"/>
		/// </summary>
		/// <param name="tag"></param>
		public virtual void Load(TagCompound tag)
		{

		}

		/// <summary>
		/// Allows modder to do custom saving here
		/// Use the given TC to put data you want to save, which can be loaded using <see cref="Load(TagCompound)"/>
		/// </summary>
		/// <param name="tag"></param>
		public virtual void Save(TagCompound tag)
		{

		}

		protected internal static Modifier _Load(Item item, TagCompound tag)
		{
			string modname = tag.GetString("ModName");
			Assembly assembly;
			if (EMMLoader.Mods.TryGetValue(modname, out assembly))
			{
				// If we load a null here, it means a modifier is unloaded
				Modifier e;
				try
				{
					e = (Modifier)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));
				}
				catch (Exception)
				{
					return null;
				}

				e.Type = tag.Get<uint>("ModifierType");
				e.Mod = ModLoader.GetMod(modname);
				var p = ModifierProperties.Load(tag.GetCompound("ModifierProperties"));
				e.Properties = e.GetModifierProperties(item).RollMagnitudeAndPower(p.Magnitude, p.Power);
				e.Load(tag);
				return e;
			}
			throw new Exception($"Modifier load error for {modname}");
		}

		protected internal static TagCompound Save(Modifier modifier)
		{
			var tag = new TagCompound
			{
				{ "Type", modifier.GetType().FullName },
				{ "ModifierType", modifier.Type },
				{ "ModName", modifier.Mod.Name },
				{ "ModifierProperties", modifier.Properties.Save() }
			};
			modifier.Save(tag);
			return tag;
		}

		// Never autoload us
		public sealed override bool Autoload(ref string name) => false;
		public sealed override bool InstancePerEntity => true;
		public sealed override bool CloneNewInstances => true;

		// The following hooks aren't applicable in instanced context, so we seal them here so they can't be used	
		public sealed override GlobalItem Clone(Item item, Item itemClone) => base.Clone(item, itemClone);
		public sealed override void ExtractinatorUse(int extractType, ref int resultType, ref int resultStack)
		{
		}
		public sealed override void CaughtFishStack(int type, ref int stack)
		{
		}
		public sealed override void AnglerChat(int type, ref string chat, ref string catchLocation)
		{
		}
		public sealed override void ArmorSetShadows(Player player, string set)
		{
		}
		public sealed override void ArmorArmGlowMask(int slot, Player drawPlayer, float shadow, ref int glowMask, ref Color color)
		{
		}
		public sealed override void DrawArmorColor(EquipType type, int slot, Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
		{
		}
		public sealed override bool DrawBody(int body) => base.DrawBody(body);
		public sealed override void DrawHair(int head, ref bool drawHair, ref bool drawAltHair)
		{
		}
		public sealed override void DrawHands(int body, ref bool drawHands, ref bool drawArms)
		{
		}
		public sealed override bool DrawHead(int head) => base.DrawHead(head);
		public sealed override bool DrawLegs(int legs, int shoes) => base.DrawLegs(legs, shoes);
		public sealed override Vector2? HoldoutOffset(int type) => base.HoldoutOffset(type);
		public sealed override Vector2? HoldoutOrigin(int type) => base.HoldoutOrigin(type);
		public sealed override bool IsAnglerQuestAvailable(int type) => base.IsAnglerQuestAvailable(type);
		public sealed override string IsArmorSet(Item head, Item body, Item legs) => base.IsArmorSet(head, body, legs);
		public sealed override string IsVanitySet(int head, int body, int legs) => base.IsVanitySet(head, body, legs);
		// If modders wish to save/load data, they should use our custom save and load hooks
		public sealed override void Load(Item item, TagCompound tag)
		{
		}
		public sealed override void LoadLegacy(Item item, BinaryReader reader)
		{
		}
		public sealed override bool NeedsSaving(Item item) => base.NeedsSaving(item);
		public sealed override GlobalItem NewInstance(Item item) => base.NewInstance(item);
		public sealed override void OpenVanillaBag(string context, Player player, int arg)
		{
		}
		public sealed override bool PreOpenVanillaBag(string context, Player player, int arg) => base.PreOpenVanillaBag(context, player, arg);
		public sealed override void PreUpdateVanitySet(Player player, string set)
		{
		}
		// If modders wish to save/load data, they should use our custom save and load hooks
		public sealed override TagCompound Save(Item item) => base.Save(item);
		public sealed override void SetMatch(int armorSlot, int type, bool male, ref int equipSlot, ref bool robes)
		{
		}
		public sealed override void UpdateArmorSet(Player player, string set)
		{
		}
		public sealed override void UpdateVanitySet(Player player, string set)
		{
		}
		public sealed override bool WingUpdate(int wings, Player player, bool inUse) => base.WingUpdate(wings, player, inUse);
	}
}
