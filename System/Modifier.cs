using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.System
{
	public struct ModifierEffectTooltipLine
	{
		public string Text;
		public Color? Color;
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
		public float Magnitude { get; internal set; } = 1f;
		public float Power { get; internal set; } = 1f;

		public new virtual string Name => GetType().Name;
		public virtual float BasePower => 1f;
		public virtual float MinMagnitude => 1f;
		public virtual float MaxMagnitude => 1f;
		public virtual float RarityLevel => 1f;
		public virtual float RollChance => 1f;

		public virtual ModifierEffectTooltipLine[] Description { get; }

		/// <summary>
		/// Returns the Modifier specified by type, null if not present
		/// </summary>
		public static Modifier GetModifier(ushort type)
			=> EMMLoader.GetModifier(type);

		public Modifier AsNewInstance() 
			=> (Modifier)Activator.CreateInstance(GetType());

		/// <summary>
		/// Rolls a magnitude between min and max
		/// </summary>
		internal float RollPower() // TODO support /luck/ stat
		{
			Magnitude = MinMagnitude + Main.rand.NextFloat() * (MaxMagnitude - MinMagnitude);
			Power = BasePower * Magnitude;
			return Power;
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
		/// <param name="clone"></param>
		public virtual void Clone(ref Modifier clone)
		{

		}

		public new object Clone()
		{
			Modifier clone = (Modifier)MemberwiseClone();
			clone.Mod = Mod;
			clone.Type = Type;
			clone.Magnitude = Magnitude;
			clone.Power = Power;
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

		protected internal static Modifier _Load(TagCompound tag)
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

				e.Type = tag.Get<uint>("EffectType");
				e.Mod = ModLoader.GetMod(modname);
				e.Magnitude = tag.GetFloat("Magnitude");
				e.Power = tag.GetFloat("Power");
				e.Load(tag);
				return e;
			}
			throw new Exception($"ModifierEffect load error for {modname}");
		}

		protected internal static TagCompound Save(Modifier effect)
		{
			var tag = new TagCompound {
					{ "Type", effect.GetType().FullName },
					{ "EffectType", effect.Type },
					{ "ModName", effect.Mod.Name },
					{ "Magnitude", effect.Magnitude },
					{ "Power", effect.Power },
				};
			effect.Save(tag);
			return tag;
		}

		// Never autoload us
		public sealed override bool Autoload(ref string name) => false;

		// The following hooks aren't applicable in instanced context, so we seal them here so they can't be used
		public sealed override bool InstancePerEntity => base.InstancePerEntity;
		public sealed override bool CloneNewInstances => base.CloneNewInstances;
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
		public override void DrawHair(int head, ref bool drawHair, ref bool drawAltHair)
		{
		}
		public override void DrawHands(int body, ref bool drawHands, ref bool drawArms)
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
