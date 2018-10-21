//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Terraria;
//using Terraria.ModLoader;
//using Terraria.ModLoader.IO;
//
//namespace Loot.Core
//{
//	// TODO event based?
//	public abstract class GlobalModifier
//	{
//		internal static class Handler
//		{
//			private static IEnumerable<GlobalModifier> GlobalModifiers => EMMLoader.GlobalModifiers.Select(x => x.Value);
//
//			internal static ModifierProperties GetModifierProperties(Modifier modifier, Item item)
//			{
//				ModifierProperties p = null;
//				foreach (var gm in GlobalModifiers)
//				{
//					var get = gm.GetModifierProperties(item);
//					if (get != null)
//					{
//						if (p == null)
//						{
//							p = get;
//						}
//						else
//						{
//							// TODO
//							p.Set(get.MinMagnitude, get.MaxMagnitude, get.MagnitudeStrength, get.BasePower, get.RarityLevel, get.RollChance, get.RoundPrecision);
//						}
//					}
//				}
//				return p;
//			}
//
//			internal static bool CanRoll(Modifier modifier, ModifierContext ctx)
//			{
//				return GlobalModifiers.All(x => x.CanRoll(modifier, ctx));
//			}
//
//			internal static bool CanRollCraft(Modifier modifier, ModifierContext ctx)
//			{
//				return GlobalModifiers.All(x => x.CanRollCraft(modifier, ctx));
//			}
//
//			internal static bool CanRollPickup(Modifier modifier, ModifierContext ctx)
//			{
//				return GlobalModifiers.All(x => x.CanRollPickup(modifier, ctx));
//			}
//
//			internal static bool CanRollReforge(Modifier modifier, ModifierContext ctx)
//			{
//				return GlobalModifiers.All(x => x.CanRollReforge(modifier, ctx));
//			}
//
//			internal static bool UniqueRoll(Modifier modifier, ModifierContext ctx)
//			{
//				return GlobalModifiers.All(x => x.UniqueRoll(modifier, ctx));
//			}
//
//			internal static void Roll(Modifier modifier, Item item)
//			{
//				foreach (var gm in GlobalModifiers)
//					gm.Roll(modifier, item);
//			}
//
//			internal static void Apply(Modifier modifier, Item item)
//			{
//				foreach (var gm in GlobalModifiers)
//					gm.Apply(modifier, item);
//			}
//
//			internal static void Clone(Modifier modifier, ref Modifier clone)
//			{
//				foreach (var gm in GlobalModifiers)
//					gm.Clone(modifier, ref clone);
//			}
//
//			internal static void Load(Modifier modifier, TagCompound tag)
//			{
//				foreach (var gm in GlobalModifiers)
//					gm.Load(modifier, tag);
//			}
//
//			internal static void Save(Modifier modifier, TagCompound tag)
//			{
//				foreach (var gm in GlobalModifiers)
//					gm.Save(modifier, tag);
//			}
//		}
//
//		/// <summary>
//		/// The modifier in question
//		/// You can use this property inside any method of GlobalModifier
//		/// </summary>
//		protected internal Modifier Modifier { get; internal set; }
//		public Mod Mod { get; internal set; }
//		public uint Type { get; internal set; }
//		public virtual string Name => GetType().Name;
//
//		// TODO
//		public GlobalModifier AsNewInstance()
//		{
//			var i = (GlobalModifier)Activator.CreateInstance(GetType());
//			i.Modifier = Modifier;
//			i.Mod = Mod;
//			i.Type = Type;
//			return i;
//		}
//
//		public virtual ModifierProperties GetModifierProperties(Item item)
//		{
//			return null;
//		}
//
//		public virtual bool CanRoll(Modifier modifier, ModifierContext ctx)
//		{
//			return true;
//		}
//
//		public virtual bool CanRollCraft(Modifier modifier, ModifierContext ctx)
//		{
//			return true;
//		}
//
//		public virtual bool CanRollPickup(Modifier modifier, ModifierContext ctx)
//		{
//			return true;
//		}
//
//		public virtual bool CanRollReforge(Modifier modifier, ModifierContext ctx)
//		{
//			return true;
//		}
//
//		public virtual bool UniqueRoll(Modifier modifier, ModifierContext ctx)
//		{
//			return true;
//		}
//
//		public virtual void Roll(Modifier modifier, Item item)
//		{
//		}
//
//		public virtual void Apply(Modifier modifier, Item item)
//		{
//		}
//
//		public virtual void Clone(Modifier modifier, ref Modifier clone)
//		{
//		}
//
//		public virtual void Load(Modifier modifier, TagCompound tag)
//		{
//		}
//
//		public virtual void Save(Modifier modifier, TagCompound tag)
//		{
//		}
//
//	}
//}
