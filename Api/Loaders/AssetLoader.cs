using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loot.Api.ModContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Loot.Api.Loaders
{
	/// <summary>
	/// This Loader is responsible for loading graphics assets for a mod
	/// These include shader and glowmask textures
	/// </summary>
	public static class AssetLoader
	{
		public static void RegisterAssets(Mod mod, string folder, bool clearOwnTextures = true)
		{
			RegistryLoader.CheckModLoading(mod, "RegisterAssets");
			RegistryLoader.CheckModRegistered(mod);

			if (Loot.ModContentManager == null)
			{
				throw new NullReferenceException("Loot.ContentManager is null in RegisterAssets");
			}

			var graphicsContent = Loot.ModContentManager.GetContent<GraphicsModContent>();
			if (graphicsContent == null)
			{
				throw new NullReferenceException("ModGraphicsContent is null in RegisterAssets");
			}

			if (folder.StartsWith($"{mod.Name}/"))
			{
				folder = folder.Replace($"{mod.Name}/", "");
			}

			string keyPass = $"{mod.Name}/{folder}";
			graphicsContent.AddKeyPass(mod.Name, keyPass);

			FieldInfo texturesField = typeof(Mod).GetField("textures", BindingFlags.Instance | BindingFlags.NonPublic);
			Dictionary<string, Texture2D> dictionary = ((Dictionary<string, Texture2D>) texturesField?.GetValue(mod));
			if (dictionary == null)
			{
				throw new NullReferenceException($"textures dictionary for mod {mod.Name} was null");
			}

			var textures = dictionary.Where(x => x.Key.StartsWith(folder)).ToList();
			var glowmasks = textures.Where(x => x.Key.EndsWith("_Glowmask") || x.Key.EndsWith("_Glow")).ToList();
			var shaders = textures.Where(x => x.Key.EndsWith("_Shader") || x.Key.EndsWith("_Shad")).ToList();

			foreach (var kvp in glowmasks)
			{
				string assetKey = graphicsContent.GetAssetKey(kvp.Value.Name, mod);
				if (assetKey == null)
				{
					continue;
				}

				if (graphicsContent.AnyGlowmaskAssetExists(assetKey, mod))
				{
					throw new Exception($"{mod.Name} attempted to add a glowmask asset already present: {assetKey}");
				}

				graphicsContent.AddGlowmaskTexture(assetKey, kvp.Value);
				if (clearOwnTextures)
				{
					dictionary.Remove(kvp.Key);
				}
			}

			foreach (var kvp in shaders)
			{
				string assetKey = graphicsContent.GetAssetKey(kvp.Value.Name, mod);
				if (assetKey == null)
				{
					continue;
				}

				if (graphicsContent.AnyShaderAssetExists(assetKey, mod))
				{
					throw new Exception($"{mod.Name} attempted to add a shader asset already present: {assetKey}");
				}

				graphicsContent.AddShaderTexture(assetKey, kvp.Value);
				if (clearOwnTextures)
				{
					dictionary.Remove(kvp.Key);
				}
			}

			// sanity check
			// prepare throws an exception if keyPass not registered in keyStore correctly
			graphicsContent.Prepare(mod);
			graphicsContent.ClearPreparation();
		}
	}
}
