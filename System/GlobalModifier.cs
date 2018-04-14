using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.System
{
	public abstract class GlobalModifier : IModifier
	{
		internal static class Handler
		{
			private static IEnumerable<GlobalModifier> GlobalModifiers(Modifier modifier)
			{
				var gms = EMMLoader.GlobalModifiers.Select(x => x.Value);
				foreach (var gm in gms)
					gm.Modifier = modifier;
				return gms;
			}

			internal static ModifierProperties GetModifierProperties(Modifier modifier, Item item)
			{
				ModifierProperties p = null;
				foreach (var gm in GlobalModifiers(modifier))
				{
					var get = gm.GetModifierProperties(item);
					if (get != null)
					{
						if (p == null)
						{
							p = get;
						}
						else
						{
							// TODO
							p.Set(get.MinMagnitude, get.MaxMagnitude, get.MagnitudeStrength, get.BasePower, get.RarityLevel, get.RollChance, get.RoundPrecision);
						}
					}
				}
				return p;
			}

			internal static bool CanRoll(Modifier modifier, ModifierContext ctx)
			{
				return GlobalModifiers(modifier).All(x => x.CanRoll(ctx));
			}

			internal static bool CanRollCraft(Modifier modifier, ModifierContext ctx)
			{
				return GlobalModifiers(modifier).All(x => x.CanRollCraft(ctx));
			}

			internal static bool CanRollPickup(Modifier modifier, ModifierContext ctx)
			{
				return GlobalModifiers(modifier).All(x => x.CanRollPickup(ctx));
			}

			internal static bool CanRollReforge(Modifier modifier, ModifierContext ctx)
			{
				return GlobalModifiers(modifier).All(x => x.CanRollReforge(ctx));
			}

			internal static bool UniqueRoll(Modifier modifier, ModifierContext ctx)
			{
				return GlobalModifiers(modifier).All(x => x.UniqueRoll(ctx));
			}

			internal static void Roll(Modifier modifier, Item item)
			{
				foreach (var gm in GlobalModifiers(modifier))
					gm.Roll(item);
			}

			internal static void Apply(Modifier modifier, Item item)
			{
				foreach (var gm in GlobalModifiers(modifier))
					gm.Apply(item);
			}

			internal static void Clone(Modifier modifier, ref Modifier clone)
			{
				foreach (var gm in GlobalModifiers(modifier))
					gm.Clone(ref clone);
			}

			internal static void Load(Modifier modifier, TagCompound tag)
			{
				foreach (var gm in GlobalModifiers(modifier))
					gm.Load(tag);
			}

			internal static void Save(Modifier modifier, TagCompound tag)
			{
				foreach (var gm in GlobalModifiers(modifier))
					gm.Save(tag);
			}
		}

		/// <summary>
		/// The modifier in question
		/// You can use this property inside any method of GlobalModifier
		/// </summary>
		protected internal Modifier Modifier { get; internal set; }
		public Mod Mod { get; internal set; }
		public uint Type { get; internal set; }
		public virtual string Name => GetType().Name;

		// TODO
		public GlobalModifier AsNewInstance()
		{
			var i = (GlobalModifier)Activator.CreateInstance(GetType());
			i.Modifier = Modifier;
			i.Mod = Mod;
			i.Type = Type;
			return i;
		}

		public virtual ModifierProperties GetModifierProperties(Item item)
		{
			return null;
		}

		public virtual bool CanRoll(ModifierContext ctx)
		{
			return true;
		}

		public virtual bool CanRollCraft(ModifierContext ctx)
		{
			return true;
		}

		public virtual bool CanRollPickup(ModifierContext ctx)
		{
			return true;
		}

		public virtual bool CanRollReforge(ModifierContext ctx)
		{
			return true;
		}

		public virtual bool UniqueRoll(ModifierContext ctx)
		{
			return true;
		}

		public virtual void Roll(Item item)
		{
		}

		public virtual void Apply(Item item)
		{
		}

		public virtual void Clone(ref Modifier clone)
		{
		}

		public virtual void Load(TagCompound tag)
		{
		}

		public virtual void Save(TagCompound tag)
		{
		}

	}
}
