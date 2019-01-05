using Loot.Core.Graphics;
using Loot.Core.System.Core;
using Loot.Core.System.Loaders;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Core.System
{
	/// <summary>
	/// Defines a modifier, which is an unloaded GlobalItem
	/// Making it a GlobalItem gives easy access to all hooks
	/// The various hooks are called by our own GlobalItem
	/// In your Modifier, it is safe to assume when one of the hooks is called, that item currently is modified by this modifier
	/// </summary>
	public abstract class Modifier : GlobalItem, ILoadableContent, ILoadableContentSetter, ICloneable
	{
		public Mod Mod { get; internal set; }

		Mod ILoadableContentSetter.Mod
		{
			set { Mod = value; }
		}

		public uint Type { get; internal set; }

		uint ILoadableContentSetter.Type
		{
			set { Type = value; }
		}

		public new string Name => GetType().Name;

		public virtual GlowmaskEntity GetGlowmaskEntity(Item item) => null;
		public virtual ShaderEntity GetShaderEntity(Item item) => null;

		public ModifierProperties Properties { get; internal set; }

		// Must be getter due to various fields that can change interactively
		public virtual ModifierTooltipLine[] TooltipLines => new ModifierTooltipLine[0];

		public virtual ModifierPropertiesBuilder GetModifierProperties(Item item)
			=> ModifierProperties.Builder;

		/* Global
			For now:
			We cannot roll on items that can stack (stacking is undefined behavior)
		*/
		protected internal bool _CanRoll(ModifierContext ctx)
		{
			// Properties are pre-loaded before CanRoll as they may be needed for checks
			Properties = GetModifierProperties(ctx.Item).Build();
			return ctx.Item.maxStack <= 1 && CanRoll(ctx);
		}

		/// <summary>
		/// If this Modifier can roll at all in the given context
		/// Properties are available here, apart from magnitude and power
		/// </summary>
		public virtual bool CanRoll(ModifierContext ctx) => true;

		/// <summary>
		/// Allows modders to do something when the modifier is rolled in the given context
		/// The passed rolledModifiers are already rolled modifiers on the item
		/// </summary>
		public virtual void Roll(ModifierContext ctx, IEnumerable<Modifier> rolledModifiers)
		{
		}

		/// <summary>
		/// Returns if the modifier will actually be added after it is rolled.
		/// This is called after <see cref="Roll"/> is called
		/// This is used to be able to stop a modifier from being added after it has rolled
		/// If the roll is deemed illicit, the next roll is forced to succeed
		/// </summary>
		public virtual bool PostRoll(ModifierContext ctx, IEnumerable<Modifier> rolledModifiers) => true;

		/// <summary>
		/// Allows modders to do something when this modifier is applied
		/// If a modder needs ModPlayer hooks, they should make their own ModPlayer and apply fields using this hook
		/// This is also called for <see cref="SetDefaults"/>
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

		protected internal static Modifier _NetReceive(Item item, BinaryReader reader)
		{
			string type = reader.ReadString();
			string modName = reader.ReadString();
			ModifierProperties properties = ModifierProperties._NetReceive(item, reader);

			Modifier m = ContentLoader.Modifier.GetContent(modName, type);
			if (m != null)
			{
				m.Properties = m.GetModifierProperties(item).Build();
				m.Properties.Magnitude = properties.Magnitude;
				m.Properties.Power = properties.Power;
				m.NetReceive(item, reader);
				return m;
			}

			throw new Exception($"Modifier _NetReceive error for {modName}");
		}

		protected internal static void _NetSend(Modifier modifier, Item item, BinaryWriter writer)
		{
			writer.Write(modifier.GetType().Name);
			writer.Write(modifier.Mod.Name);
			ModifierProperties._NetSend(item, modifier.Properties, writer);
			modifier.NetSend(item, writer);
		}

		/// <summary>
		/// Allows modder to do custom loading here
		/// Use the given TC to pull data you saved using <see cref="Save(Item,TagCompound)"/>
		/// </summary>
		/// <param name="tag"></param>
		public new virtual void Load(Item item, TagCompound tag)
		{
		}

		protected internal static Modifier _Load(Item item, TagCompound tag)
		{
			string modName = tag.GetString("ModName");
			Assembly assembly;
			if (modName != null
				&& MainLoader.Mods.TryGetValue(modName, out assembly))
			{
				// If we load a null here, it means a modifier is unloaded
				Modifier m = null;

				var saveVersion = tag.ContainsKey("ModifierSaveVersion") ? tag.GetInt("ModifierSaveVersion") : 1;

				string modifierTypeName = tag.GetString("Type");

				// adapt by save version
				if (saveVersion == 1)
				{
					// in first save version, modifiers were saved by full assembly namespace
					//m = (ModifierPool)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));// we modified saving
					modifierTypeName = modifierTypeName.Substring(modifierTypeName.LastIndexOf('.') + 1);
					m = ContentLoader.Modifier.GetContent(modName, modifierTypeName);
				}
				else if (saveVersion == 2)
				{
					// from saveVersion 2 and onwards, they are saved by assembly (mod) and type name
					m = ContentLoader.Modifier.GetContent(modName, modifierTypeName);
				}

				if (m != null)
				{
					// saveVersion 1, no longer needed. Type and Mod is already created by new instance
					//m.Type = tag.Get<uint>("ModifierType");
					//m.Mod = ModLoader.GetMod(modname);
					var p = ModifierProperties._Load(item, tag.GetCompound("ModifierProperties"));
					m.Properties = m.GetModifierProperties(item).Build();
					m.Properties.Magnitude = p.Magnitude;
					m.Properties.Power = p.Power;
					m.Load(item, tag);
					return m;
				}

				return null;
			}

			throw new Exception($"Modifier load error for {modName}");
		}

		/// <summary>
		/// Allows modder to do custom saving here
		/// Use the given TC to put data you want to save, which can be loaded using <see cref="Load(Item,TagCompound)"/>
		/// </summary>
		public virtual void Save(Item item, TagCompound tag)
		{
		}

		protected internal static TagCompound Save(Item item, Modifier modifier)
		{
			var tag = new TagCompound
			{
				{"Type", modifier.GetType().Name},
				//{ "ModifierType", modifier.Type }, //Used to be saved in saveVersion 1
				{"ModName", modifier.Mod.Name},
				{"ModifierProperties", ModifierProperties._Save(item, modifier.Properties)},
				{"ModifierSaveVersion", 2}
			};
			modifier.Save(item, tag);
			return tag;
		}

		// Never autoload us
		public sealed override bool Autoload(ref string name) => false;
		public sealed override bool InstancePerEntity => true;
		public sealed override bool CloneNewInstances => true;

		public sealed override void SetDefaults(Item item)
		{
			base.SetDefaults(item);
			Apply(item);
		}

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
		//public sealed override void Load(Item item, TagCompound tag)
		//{
		//}
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
