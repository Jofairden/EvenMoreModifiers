ModifierPool
===
___
To make a new ModifierPool, simply make a new class that inherits the class `ModifierPool` in the `Loot.System` namespace.

#### Concept
___
The idea of a pool is that it consists of a 'pool' of modifiers of which up to 4 can be rolled on an item. EMM itself uses a [single pool that consists of all modifiers in the system](https://github.com/Jofairden/EvenMoreModifiers/blob/a5c1629c4a382eceb7f490ae1487e0dc6c94385f/System/AllModifiersPool.cs#L11).

Also note that, the modifiers on an item are actually saved as a ModifierPool: the Modifiers are inside this ModifierPool.

#### Primary use
___
The primary use of a pool should be to roll a 'themed' item. You could also make pools that are rare that consist only of modifiers that work well together, making it more likely the item becomes very potent with the rolled modifiers.

You'll want to populate the `Modifiers` array in your contructor. An example can be seen [here](https://github.com/Jofairden/EvenMoreModifiers/blob/a5c1629c4a382eceb7f490ae1487e0dc6c94385f/Pools/WeaponModifierPool.cs#L10-L26)