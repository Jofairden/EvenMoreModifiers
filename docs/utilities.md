Utilities
===
___
EMM provides necessary utility functions for modifiers, rarities and pools. These can be called on a mod instance. The utils can be found in [EMMUtils](https://github.com/Jofairden/EvenMoreModifiers/blob/a5c1629c4a382eceb7f490ae1487e0dc6c94385f/EMMUtils.cs#L18-L61).
The utils work primarily the same as the ones in tML.
An example call to get one of your modifiers:

```c#
myMod.GetModifier<MyModifier>();
```