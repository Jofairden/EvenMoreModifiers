using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers
{
	internal class ModifierEffectTooltipLineTagSerializer : TagSerializer<ModifierEffectTooltipLine, TagCompound>
	{
		public override ModifierEffectTooltipLine Deserialize(TagCompound tag)
		{
			return new ModifierEffectTooltipLine
			{
				Text = tag.GetString("Text"),
				Color = tag.Get<Color>("Color")
			};
		}

		public override TagCompound Serialize(ModifierEffectTooltipLine value)
		{
			return new TagCompound
			{
				{"Text", value.Text },
				{"Color", value.Color }
			};
		}
	}

	internal class ModifierEffectTagSerializer : TagSerializer<ModifierEffect, TagCompound>
	{
		public override ModifierEffect Deserialize(TagCompound tag)
		{
			ModifierEffect e = (ModifierEffect)Activator.CreateInstance(tag.Get<Type>("Type"));
			e.Type = tag.Get<uint>("EffectType");
			e.Mod = ModLoader.GetMod(tag.GetString("ModName"));
			e.Magnitude = tag.GetFloat("Magnitude");
			e.Power = tag.GetFloat("Power");
			//int lines = tag.GetAsInt("Lines");
			//if (lines > 0)
			//{
			//	var list = new List<ModifierEffectTooltipLine>();
			//	for (int i = 0; i < lines; ++i)
			//	{
			//		list.Add(tag.Get<ModifierEffectTooltipLine>($"Line{i}"));
			//	}
			//	e.Description = list.ToArray();
			//}
			return e;
		}

		public override TagCompound Serialize(ModifierEffect value)
		{
			var tc = new TagCompound {
					{ "Type", value.GetType() },
					{ "EffectType", value.Type },
					{ "ModName", value.Mod.Name },
					{ "Magnitude", value.Magnitude },
					{ "Power", value.Power  },
				};
			//tc.Add("Lines", value.Description.Length);
			//if (value.Description.Length  > 0)
			//{
			//	for (int i = 0; i < value.Description.Length; ++i)
			//	{
			//		tc.Add($"Line{i}", value.Description[i]);
			//	}
			//}
			return tc;
		}
	}

	internal class ModifierRarityTagSerializer : TagSerializer<ModifierRarity, TagCompound>
	{
		public override ModifierRarity Deserialize(TagCompound tag)
		{
			ModifierRarity r = (ModifierRarity)Activator.CreateInstance(tag.Get<Type>("Type"));
			r.Type = tag.Get<uint>("RarityType");
			r.Mod = ModLoader.GetMod(tag.GetString("ModName"));
			return r;
		}

		public override TagCompound Serialize(ModifierRarity value)
		{
			return new TagCompound
			{
				{"Type", value.GetType() },
				{"RarityType", value.Type },
				{"ModName", value.Mod.Name },
			};
		}
	}

	internal class ModifierTagSerializer : TagSerializer<Modifier, TagCompound>
	{
		public override Modifier Deserialize(TagCompound tag)
		{
			Modifier m = (Modifier)Activator.CreateInstance(tag.Get<Type>("Type"));
			m.Type = tag.Get<uint>("ModifierType");
			m.Mod = ModLoader.GetMod(tag.GetString("ModName"));
			m.Rarity = tag.Get<ModifierRarity>("Rarity");
			int effects = tag.GetAsInt("Effects");
			if (effects > 0)
			{
				var list = new List<ModifierEffect>();
				for (int i = 0; i < effects; ++i)
				{
					list.Add(tag.Get<ModifierEffect>($"Effect{i}"));
				}
				m.Effects = list.ToArray();
			}
			int activeeffects = tag.GetAsInt("ActiveEffects");
			if (activeeffects > 0)
			{
				var list = new List<ModifierEffect>();
				for (int i = 0; i < activeeffects; ++i)
				{
					list.Add(tag.Get<ModifierEffect>($"ActiveEffect{i}"));
				}
				m.ActiveEffects = list.ToArray();
			}
			return m;
		}

		public override TagCompound Serialize(Modifier value)
		{
			var tc = new TagCompound
			{
				{"Type", value.GetType() },
				{"ModifierType", value.Type },
				{"ModName", value.Mod.Name },
				{"Rarity", value.Rarity },
			};
			tc.Add("Effects", value.Effects.Length);
			if (value.Effects.Length > 0)
			{
				for (int i = 0; i < value.Effects.Length; ++i)
				{
					tc.Add($"Effect{i}", value.Effects[i]);
				}
			}
			tc.Add("ActiveEffects", value.ActiveEffects.Length);
			for (int i = 0; i < value.ActiveEffects.Length; ++i)
			{
				tc.Add($"ActiveEffect{i}", value.ActiveEffects[i]);
			}
			return tc;
		}
	}
}
