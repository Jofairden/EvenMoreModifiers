using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers
{
	public struct ModifierEffectTooltipLine
	{
		public string Text;
		public Color? Color;
	}

	/// <summary>
	/// Defines a modifier effect
	/// </summary>
	public abstract class ModifierEffect : ICloneable
	{
		public Mod Mod { get; internal set; }
		public uint Type { get; internal set; }
		public float Magnitude { get; internal set; } = 1f;
		public float Power { get; internal set; } = 1f;

		public virtual string Name => GetType().Name;
		public virtual float BasePower => 1f;
		public virtual float MinMagnitude => 1f;
		public virtual float MaxMagnitude => 1f;
		public virtual float RarityLevel => 1f;
		public virtual float RollChance => 1f;

		public virtual ModifierEffectTooltipLine[] Description { get; }

		internal float RollAndApplyMagnitude()
		{
			Magnitude = MinMagnitude + Main.rand.NextFloat() * (MaxMagnitude - MinMagnitude);
			Power = BasePower * Magnitude;
			return Power;
		}

		public virtual bool CanRoll(ModifierContext ctx) => true;

		public virtual void UpdateItem(ModifierContext ctx, bool isItemEquipped = false)
		{

		}

		public virtual void ApplyItem(ModifierContext ctx)
		{

		}

		public virtual void ApplyNPC(ModifierContext ctx)
		{

		}

		public virtual void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{

		}

		public virtual void Clone(ref ModifierEffect clone)
		{

		}

		public object Clone()
		{
			ModifierEffect clone = (ModifierEffect)MemberwiseClone();
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

		protected internal static ModifierEffect _Load(TagCompound tag)
		{
			string modname = tag.GetString("ModName");
			Assembly assembly;
			if (EMMLoader.Mods.TryGetValue(modname, out assembly))
			{
				ModifierEffect e = (ModifierEffect)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));
				e.Type = tag.Get<uint>("EffectType");
				e.Mod = ModLoader.GetMod(modname);
				e.Magnitude = tag.GetFloat("Magnitude");
				e.Power = tag.GetFloat("Power");
				e.Load(tag);
				return e;
			}
			throw new Exception($"ModifierEffect load error for {modname}");
		}

		protected internal static TagCompound Save(ModifierEffect effect)
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
	}
}
