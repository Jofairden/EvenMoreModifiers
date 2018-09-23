The glowmask- and shader-entities system
===
___
EMM features a system called entities for glowmasks and shaders. This allows you to apply glowmasks and shaders to items based on its modifiers. (You quite literally attach one to the Modifier itself!)

## How does it work?
In the background, EMM handles everything for you. An 'entity' lives on the modifier of an item, which can alter its appearance. Examples:

<https://streamable.com/6kj8o>

<https://streamable.com/ezkm5>

## How to use it?

### Shader entity
Inside your modifier class, simply override `GetShaderEntity()` and return an entity:
```cs
public override ShaderEntity GetShaderEntity(Item item)
{
	return new ShaderEntity(item,
		GameShaders.Armor.GetShaderIdFromItemId(ItemID.MirageDye),
		drawLayer: ShaderDrawLayer.Front,
		drawOffsetStyle: ShaderDrawOffsetStyle.Alternate,
		shaderDrawColor: Color.IndianRed);
}
```

Note that custom shaders are not supported at this time.

Read below to learn how to use a custom sprite.

#### Glowmask entity
Inside your modifier class, simply override `GetGlowmaskEntity()` and return an entity:
```cs
public override GlowmaskEntity GetGlowmaskEntity(Item item)
{
	return new GlowmaskEntity(item);
}
```
Read below to learn how to use a custom sprite.

## Using custom assets
EMM has a custom system built in that can automagically load assets that entities can use. This means you can control which parts of the item is being applied a glowmask or shader. An example where only a specific part of the item is being applied a shader: <https://streamable.com/ezkm5>

#### How to load my own assets?
All you have to do is specify to register your assets in Load():
```cs
if (!Main.dedServ)
{
	EMMLoader.RegisterAssets(this, "MyFolder");
}
```

#### How does it work?
The above code will make it so that assets from the folder `<YourMod>/GraphicAssets` are added to the system. This path is relative to your mod, so if you enter `MyFolder/SubFolder/DeeperFolder` it will look in there. Currently only a single folder is supported, and you should put all assets in there.

**Keynote:** the assets will be removed from your mod’s internal texture array. You can disable this by passing: `clearOwnTextures: false`

#### Naming rules
All assets can be named by either the item name or its ID. The ID (or type) is recommended for vanilla items. Currently you can only add assets for your own mod or vanilla. Example asset names:
* MyModItem_Glowmask.png –or– MyModItem_Glow.png
* MyModItem_Shader.png –or– MyModItem_Shad.png

As shown, names can be suffixed with either the full variant or shorthand of the asset type. This suffix is important or else the asset will not be added.

For vanilla items, it is recommended to name them by their type, it is however still possible to name them by their name. (EMM will try to resolve its type manually)
