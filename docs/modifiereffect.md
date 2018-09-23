ModifierEffect
===
___
A ModifierEffect signifies the effect of a modifier on a player
It should house the implementation, and delegation of such an effect
Methods on effects can be delegated from ModPlayer

## How to get a ModifierEffect
You can get an effect as follows:
```csharp
ModifierPlayer.Player(player).GetEffect<MyModifierEffect>()
```

## UsesEffectAttribute
____
The UsesEffect attribute is linking a Modifier to a ModifierEffect.
This will make the ModifierEffect become activated if we have an item with this modifier.

Usage:

```csharp
[UsesEffect(typeof(MyEffect))]
public class MyModifier : Modifier
```

```csharp
[UsesEffect(typeof(FirstEffect), typeof(SecondEffect), typeof(ThirdEffect)]
public class MyModifier : Modifier
```
____

## DelegationPrioritizationAttribute
____
This attribute is used to set a custom prioritization for a delegation
It allows you to customize at which point your delegation is called in the chain
The end result is a prioritization list as follows:

* First part: all delegations prioritized as DelegationPrioritization.Early, order by their level
* Second part: all delegations with no custom prioritization
* Third part: all delegations prioritized as DelegationPrioritization.Late, order by their level

To increase the force put into your prioritization, increase the delegation level

##### Example use-case
____
Consider a modifier adding a flat +50 damage, and another modifier giving +100% damage to the item.
Without the prioritization options, it sometimes happens that the latter modifier is delegated before the former one. This means the multiplication happens before addition. Example:

`Base 10 damage. * 100% = 20 damage, + 50 damage = 70 damage`

`Base 10 damage. + 50 = 60 damage * 100% = 120 damage`

You can see how big of a difference this can make. So this is what this functionality is used for: **to prioritize when your delegation is executed in the chain**

##### How to use
____
You have two options (technically three): early in the chain or later in the chain.

To be in the middle, simply don't use this attribute.

Examples:
```csharp
// a multiplicative effect wants to be last in the chain, hence 999
[AutoDelegation("OnModifyHitNPC")]
[DelegationPrioritization(DelegationPrioritization.Late, 999)]
```

```csharp
// this particular example wants to happen early, for example mitigating damage entirely
[AutoDelegation("OnPreHurt")]
[DelegationPrioritization(DelegationPrioritization.Early, 100)]
```