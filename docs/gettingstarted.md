Getting started
===
___
It is completely possible to use EMM in your own mod and start adding your own modifiers!
On this page, we will cover how te set yourself up in order to do this.

## Extracting the mod
Firstly, [download](https://zond.tech/emm/downloads/) EMM's .tmod file and place it in your mods folder. Now go in game, navigate to the mods menu and press the extra info button for EMM. From there, press the extract button.

## Getting the .dll file
Now you will have extracted EMM, with that comes the .dll file you'll need for your project. The extraction process unpacks the mod and places its files in the Mod Reader folder. This is usually located in `%userprofile%\Documents\My Games\Terraria\ModLoader\Mod Reader`. In there, look for a folder named `Loot`. Inside that folder, grab the Windows.dll file and take it to some place you can easily navigate to. It is recommended to place it inside a `lib/` folder inside your mod root. Make sure to rename the file to `Loot.dll`. 

**FAQ**: Why call it 'Loot'?
**A**: EMM is internally named as 'Loot' by its previous owner, which cannot be changed.

## Referencing the file
Now inside Visual Studio you can add the .dll as a reference which will give you access to everything EMM has to offer you. To do this, navigate to the references subject in your solution explorer, right click it and press 'Add Reference'

## Usage requirements


#### Setting the dependency
**Note**: it is very important to consider if you want your mod to be an optional reference or forced. If you want EMM to be required for your mod, add EMM to your build.txt as dependency: `modReferences = Loot`. If you want EMM to be optional, add it to the weakReferences in build.txt: `weakReferences = Loot`. Additionally, if you make EMM a weak reference, **you must make 100% sure to not call any code referencing the EMM assembly if EMM is not loaded**. If you don't, the game will crash for people using your mod.

#### Registering your mod
In order to have content in your mod added to the EMM system, you need to actually register your mod and then your content. You do this inside your mod's Load() hook. If you made EMM a weakReference, make sure to only call this if EMM is loaded.

```cs
public override void Load()
{
	EMMLoader.RegisterMod(this);
	EMMLoader.SetupContent(this);
}
```

Or as a weakReference:

```cs
public override void Load()
{
	Mod mod = ModLoader.GetMod("Loot");
	if (mod != null)
	{
		EMMLoader.RegisterMod(this);
		EMMLoader.SetupContent(this);
	}
}
```

**Note: if you want your mod to work on Mono (mac)**, you must put code that references the EMM assembly in a separate class. It could look something like this:

```cs
public static class EMMHandler
{
	public static void RegisterMod(Mod mod)
	{
		EMMLoader.RegisterMod(mod);
	}
	
	public static void SetupContent(Mod mod)
	{
		EMMLoader.SetupContent(mod);
	}
}
```
Which would change your Load() to:

```cs
public override void Load()
{
	EMMHandler.RegisterMod(this);
	EMMHandler.SetupContent(this);
}
```

Or if as a weakReference:

```cs
public override void Load()
{
	Mod mod = ModLoader.GetMod("Loot");
	if (mod != null)
	{
		EMMHandler.RegisterMod(this);
		EMMHandler.SetupContent(this);
	}
}
```

Note that, it is best practice to do this for any weakReference. You only have to do this for this part though _if you can make sure to not reference anything in your assembly yourself that references EMM's assembly, such as a modifier_.

After this, you're set! You can now start adding modifiers.
To learn how to add modifiers, see the particular section that describes it.

#### Autoloading glowmask and shader assets
If you want to autoload your own assets, you'll need to add the following to your Load():

```cs
if (!Main.dedServ)
{
	EMMLoader.RegisterAssets(this, "MyFolder");
}
```
Note that again, it is best practice to add a new method to EMMHandler and call that if you want this to work on Mono.

Make sure to edit "MyFolder" to the folder you want it to register.
More details can be found in the specific section for this subject.