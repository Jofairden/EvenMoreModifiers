using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Rarities;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Loot.Modifiers
{
	public enum ModifierContextMethod
	{
		OnReforge,
		OnCraft,
		OnPickup,
		UpdateItem,
		UpdateNPC
	}

	public struct ModifierContext
	{
		public ModifierContextMethod Method;
		public Player Player;
		public NPC NPC;
		public Item Item;
		public IDictionary<string, object> CustomData;
	}

	/// <summary>
	/// Defines a modifier.
	/// </summary>
	public abstract class Modifier : ICloneable
	{
		public uint Type { get; internal set; }
		public Mod Mod { get; internal set; }
		public ModifierRarity Rarity { get; internal set; }
		protected ModifierEffect[] Effects;
		public ModifierEffect[] ActiveEffects { get; internal set; }

		public float TotalRarityLevel =>
			ActiveEffects.Select(effect => effect.RarityLevel).DefaultIfEmpty(0).Sum();

		public IEnumerable<ModifierEffectTooltipLine[]> Description =>
			ActiveEffects.Select(effect => effect.TooltipLines);

		public virtual string Name => this.GetType().Name;
		public virtual float RollChance => 1f;

		internal ModifierRarity UpdateRarity()
		{
			Rarity = EMMLoader.GetModifierRarity(this);
			return Rarity;
		}

		internal ModifierEffect[] RollEffects(ModifierContext ctx)
		{
			WeightedRandom<ModifierEffect> wr = new WeightedRandom<ModifierEffect>();
			List<ModifierEffect> list = new List<ModifierEffect>();
			foreach (var e in Effects.Where(x => x.CanRoll(ctx)))
				wr.Add(e, e.RollChance);

			if (wr.elements.Count <= 0)
				return ActiveEffects;

			for (int i = 0; i < 4; ++i)
			{
				if (wr.elements.Count <= 0 || Main.rand.NextFloat() > (EffectRollChance(i)))
					break;

				ModifierEffect e = wr.Get();
				list.Add(e);
				wr.elements.Remove(new Tuple<ModifierEffect, double>(e, e.RollChance));
				wr.needsRefresh = true;
			}

			ActiveEffects = list.ToArray();
			return ActiveEffects;
		}

		internal float EffectRollChance(int len)
			=> 0.5f / (float)Math.Pow(2, len);


		internal bool _CanApply(ModifierContext ctx)
		{

			if (Effects.Length <= 0)
				return false;

			switch (ctx.Method)
			{
				case ModifierContextMethod.OnCraft:
					return CanApplyCraft(ctx);
				case ModifierContextMethod.OnPickup:
					return CanApplyPickup(ctx);
				case ModifierContextMethod.OnReforge:
					return CanApplyReforge(ctx);
				case ModifierContextMethod.UpdateItem:
					return CanUpdateItem(ctx);
				case ModifierContextMethod.UpdateNPC:
					return CanUpdateItem(ctx);
				default:
					return true;
			}
		}

		public virtual bool CanRoll(ModifierContext ctx) => Effects.Length > 0 && Effects.Any(x => x.CanRoll(ctx));
		public virtual bool CanApplyCraft(ModifierContext ctx) => true;
		public virtual bool CanApplyPickup(ModifierContext ctx) => true;
		public virtual bool CanApplyReforge(ModifierContext ctx) => true;
		public virtual bool CanApplyItem(ModifierContext ctx) => true;
		public virtual bool CanUpdateItem(ModifierContext ctx) => true;
		public virtual bool CanUpdateNPC(ModifierContext ctx) => true;

		internal void ApplyItem(ModifierContext ctx)
		{
			if (_CanApply(ctx))
			{
				foreach (var effect in ActiveEffects)
				{
					effect.ApplyItem(ctx);
				}
			}
		}

		internal void UpdateItem(ModifierContext ctx, bool equipped = false)
		{
			if (_CanApply(ctx))
			{
				foreach (var effect in ActiveEffects)
				{
					effect.UpdateItem(ctx, equipped);
				}
			}
		}

		public override string ToString()
			=> EMMUtils.JSLog(typeof(Modifier), this);

		public virtual void OnClone(Modifier clone)
		{

		}

		public object Clone()
		{
			Modifier clone = (Modifier)this.MemberwiseClone();
			clone.Type = Type;
			clone.Mod = Mod;
			clone.Rarity = (ModifierRarity)Rarity?.Clone();
			clone.Effects =
				Effects?
				.Select(x => x?.Clone())
				.Cast<ModifierEffect>()
				.ToArray();
			clone.ActiveEffects =
				ActiveEffects?
				.Select(x => x?.Clone())
				.Cast<ModifierEffect>()
				.ToArray();
			OnClone(clone);
			return clone;
		}
	}
}
