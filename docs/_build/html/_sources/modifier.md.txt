Making a modifier
===
___
To make a new Modifier, simply make a new class that inherits the class `Modifier` in the `Loot.System` namespace.

#### Properties
___

##### Brief properties explanation
Modifiers have the following features:

**Power rolls**: modifiers can roll its power by 'magnitudes'. This happens when the modifier is rolled on the item, and these values are saved.

**Weighted roll chances**: modifiers can have different chances of being rolling (weighted rolling). The default weight is 1f. If you increase this number, your modifier will show up more often. Putting it to 2f will make it show up twice as often. Something like 10f is a really high roll chance. For very powerful modifiers, you want the roll chance to be low.

**Rarity levels**: the total rarity is decided by the sum of the rarity level of all modifiers on the item. The default is 1f. If you have more powerful modifiers, you will want to increase this number to signify the power of it.

**Modifiers function like GlobalItems**. This means you have access to pretty much all hooks available in GlobalItem (a few [exceptions](https://github.com/Jofairden/EvenMoreModifiers/blob/rework/System/Modifier.cs#L297)). You can assume the item in the context of a hook (the item parameter) is an item being influenced by that modifier. This is handled by EMM and out of your way, mostly consisting of [boilerplate code](https://github.com/Jofairden/EvenMoreModifiers/blob/rework/ModifierItem.cs).

Below I will detail all the properties of [ModifierProperties](https://github.com/Jofairden/EvenMoreModifiers/blob/rework/System/Modifier.cs#L22-L112)

```csharp
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
```

##### Properties explanation in context
**MinMagnitude** and **MaxMagnitude** define the range that **Magnitude** will roll in. For example 10f - 15f will make **Magnitude** roll between 10f and 15f. **MagnitudeStrength** influences the final result of that roll as a multiplier. For example setting this to 2 and rolling the upper bound of the previous example (15f) would result in a Magnitude of 30. **Power** is then calculated by **BasePower**\***Magnitude**, so a BasePower of 10 in the previous context would result in a Power of 300. When Power is set, it automatically sets **RoundedPower** as well. This property is primarily useful for tooltips, where you'll likely want to display 230% instead of something like 230.568893%. The precision of the rounding can be adjusted with **RoundPrecision**, which defaults to 0. So by default it will prune all decimals. If you set it to 1, it will leave 1 decimal, at 2 it will leave 2, and so forth...

These Properties (and all other hooks) support inheritance.  A good example of that can be found [here](https://github.com/Jofairden/EvenMoreModifiers/blob/rework/Modifiers/EquipModifiers/KnockbackImmunity.cs#L20-L31) and [here](https://github.com/Jofairden/EvenMoreModifiers/blob/rework/Modifiers/WeaponDebuffModifier.cs#L57-L60).


##### Modifying properties
To modify the Properties, you override the [GetModifierProperties](https://github.com/Jofairden/EvenMoreModifiers/blob/rework/System/Modifier.cs#L134-L137) method and return a new object. All properties have a default value, to set specific ones you can specify the parameter names like so:
```csharp
public virtual ModifierProperties GetModifierProperties(Item item)
{
	return new ModifierProperties(roundPrecision: 1, minMagnitude: 15f, maxMagnitude: 30f);
}
```

Please note that you need to be careful when using inheritance! Always call base first and then modify whatever properties it returns:

A base class:
```csharp
public abstract class EquipModifier : Modifier
{
	public override ModifierProperties GetModifierProperties(Item item)
	{
		return new ModifierProperties(magnitudeStrength: item.IsAccessory() ? .6f : 1f);
	}
}
```

If a class inherits from this base class, the properties should be handled like so:
```csharp
public override ModifierProperties GetModifierProperties(Item item)
{
	return base.GetModifierProperties(item).Set(maxMagnitude: 15f);
}
```

#### Saving and loading data per modifier
___
You can save and load your own data per modifier, an example can be seen [here](https://github.com/Jofairden/EvenMoreModifiers/blob/rework/Modifiers/WeaponModifiers/RandomDebuff.cs#L43-L53).
Essentially, saving and loading works exactly the same as you are used to. The handling of it is managed by EMM itself and out of your way.

##### Saving data
Please note that, inside the Save method the TagCompound already contains necessary save data for the modifier (which is automatically handled by EMM).
Make sure that you don't clear the TC or make a new instance!
```csharp
public override void Save(Item item, TagCompound tag)
{
	// If you use inheritance, always call base first
	base.Save(item, tag);
	
	// Any saving here!
}
```

##### Loading data
```csharp
public override void Load(Item item, TagCompound tag)
{
	// If you use inheritance, always call base first
	base.Load(item, tag);
	
	// Any loading here!
}
```

#### Tooltips
___
Tooltips by modifiers are automatically managed by EMM, and it does the following:
* Item name automatically modified if rarity overrides color and/or adds prefix/suffix
* Adds a rarity tag by name [Modifier:Name]
* Adds a modifier line (description) per line by name [Modifier:Description:i]

However, you can still modify the tooltips per modifier if you want to. The ModifyTooltips method is called after this automatic behaviour. 