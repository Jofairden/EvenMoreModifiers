using Loot.Api.Attributes;
using Loot.Api.Core;
using Loot.Api.Delegators;
using Loot.Modifiers.Base;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Utility
{
	// Light effect, spawns a light on the player
	// This is one of the most basic modifiers with effect examples
	public class LightEffect : ModifierEffect
	{
		// Public variables that are adjustable
		public float LightStrength;
		public Color LightColor;

		// OnInitialize can initialize an effect's values
		// It is called when a player initializes its effects
		public override void OnInitialize()
		{
			// Setting Strength is not really needed, as the default
			// value for floats is already 0f. Just for clarity's sake.
			LightStrength = 0f;
			LightColor = Color.White;
		}

		// ResetEffects is called automatically if the effect is being delegated
		// This is used to reset variables like you are used to in ModPlayer
		public override void ResetEffects()
		{
			LightStrength = 0f;
			LightColor = Color.White;
		}

		public override void Clone(ref ModifierEffect clone)
		{
			// For clarity, additional cloning is possible like this
			((LightEffect) clone).LightStrength = LightStrength;
			((LightEffect) clone).LightColor = LightColor;
		}

		// This method will be delegated to PostUpdate
		// Meaning that this code is automatically called
		// on ModifierPlayer's PostUpdate hook if this effect
		// is being delegated
		[AutoDelegation("OnPostUpdate")]
		private void Light()
		{
			Lighting.AddLight(player.Center, LightColor.ToVector3() * .15f * LightStrength);
		}
	}

	// The UsesEffect attribute is linking this Modifier
	// to the LightEffect effect.
	// This will make the LightEffect become activated
	// if we have an item with this modifier.
	[UsesEffect(typeof(LightEffect))]
	public class LightPlus : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower} light");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(2f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierDelegatorPlayer.GetPlayer(player).GetEffect<LightEffect>().LightStrength += (int) Properties.RoundedPower;
		}
	}
}
