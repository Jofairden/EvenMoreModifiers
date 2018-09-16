Getting started
===
____
Learn how to setup your mod to be able to add your content to EMM.

#### Registering your mod
____
First, you need to register your mod into EMM. Autoloading your creations is the easiest way. To register your mod, call this in your Load() override:

```csharp
Loot.EMMLoader.RegisterMod(this)
```

This will let EMM know you want your mod to contribute.

#### Autoloading your content
____
Next up, you will let EMM know to setup content for you, i.e. it actually adds everything you made to the system:

```csharp
Loot.EMMLoader.SetupContent(this)
```
   
This also needs to be called in your Load() override.