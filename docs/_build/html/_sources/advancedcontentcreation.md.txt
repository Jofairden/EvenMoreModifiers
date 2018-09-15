Advanced content creation
===
In the following section we will go over advanced content creation, which includes an explanation of the more complex systems that are in place that make this possible.

## Automated event-based delegation
____
This mod features what I call 'automated event-based delegation'. The purpose of this system is simple: it allows you to run ModPlayer code, through a Modifier. What does this mean? Just imagine that you need to run ModPlayer code for some Modifier, this system makes that possible **in an easy-to-use yet efficient manner**.

To use this, you do not need to understand how the system works internally: you just need to know how to call the necessary features to have it work. This system is used with ModifierEffects. This is a class not yet covered, so we will do so now.

## ModifierEffect & delegation
____
ModifierEffects live on the player, specifically a ModPlayer, part of EMM called the '[ModifierPlayer](https://github.com/Jofairden/EvenMoreModifiers/blob/rework/ModifierPlayer.cs)'. Do not make these more complicated as they are: they give you access to virtually all ModPlayer hooks similarly to how Modifier exposes most GlobalItem hooks to you. Instead, it does it in a different way. It does not inherit from ModPlayer, it is it's own class instead. This is where the event-based delegation comes into play, as it allows those hooks to actually be 'attached' to an existing ModPlayer hook. Here is what that looks like:

```csharp
public class CursedEffect : ModifierEffect
{
	public int CurseCount;

	public override void ResetEffects(ModifierPlayer player)
	{
		CurseCount = 0;
	}

	[AutoDelegation("OnUpdateBadLifeRegen")]
	private void Curse(ModifierPlayer player)
	{
		if (CurseCount > 0 && !player.player.buffImmune[BuffID.Cursed])
		{
			if (player.player.lifeRegen > 0)
			{
				player.player.lifeRegen = 0;
			}
			player.player.lifeRegen -= 2 * CurseCount;
			player.player.lifeRegenTime = 0;
		}
	}
}
```

The above example showcases a 'CursedEffect', that inherits from ModifierEffect. Our Curse method is being delegated to the `ModPlayer.OnUpdateBadLifeRegen`, notice how the AutoDelegation attribute makes that possible. All you need to make sure is that your method signature matches the one of the hook you want to attach to, where the first parameter is _always_ of type ModifierPlayer. Even if the hook normally has no parameters.

You are now correct if you say that the Curse method is being ran from within the OnUpdateBadLifeRegen hook.

The effect is 'attached' to a Modifier in the following way:

```csharp
[UsesEffect(typeof(CursedEffect))]
public class CursedDamage : Modifier
```

Notice the UsesEffect attribute, in which you specify which effect should become active when an item has this modifier. Note that you can specify multiple effects, if you want:

```csharp
[UsesEffect(typeof(EffectOne), typeof(EffectTwo), typeof(EffectThree))]
public class MyModifier : Modifier
```