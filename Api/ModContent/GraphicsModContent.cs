using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Loot.Api.ModContent
{
	public class GraphicsModContent : TextureModContent
	{
		private IDictionary<string, Texture2D> _glowmaskTextures;
		private IDictionary<string, Texture2D> _shaderTextures;

		private IDictionary<string, string> _keyStore;

		private IDictionary<string, Texture2D> _lookupTable;
		private IDictionary<string, Texture2D> _lookupGlowmaskTable;
		private IDictionary<string, Texture2D> _lookupShaderTable;

		protected override void Initialize()
		{
			_glowmaskTextures = new Dictionary<string, Texture2D>();
			_shaderTextures = new Dictionary<string, Texture2D>();

			_keyStore = new Dictionary<string, string>();

			_lookupTable = new Dictionary<string, Texture2D>();
			_lookupGlowmaskTable = new Dictionary<string, Texture2D>();
			_lookupShaderTable = new Dictionary<string, Texture2D>();
		}

		protected override void Unload()
		{
			_glowmaskTextures = null;
			_shaderTextures = null;

			_keyStore = null;

			_lookupTable = null;
			_lookupGlowmaskTable = null;
			_lookupShaderTable = null;
		}

		public override string GetRegistryKey()
		{
			return "ModGraphics";
		}

		internal void AddKeyPass(string key, string keyPass)
		{
			if (!_keyStore.ContainsKey(key))
			{
				_keyStore.Add(key, keyPass);
			}
		}

		private IDictionary<string, int> _vanillaNamesCache;

		private void FillVanillaNamesCache()
		{
			_vanillaNamesCache = new Dictionary<string, int>();
			for (int i = 1; i < ItemID.Count; i++)
			{
				Item item = new Item();
				item.SetDefaults(i);
				if (_vanillaNamesCache.ContainsKey(item.Name))
				{
					Loot.Logger.Error("There was a problem during initialization of vanilla names cache");
				}

				_vanillaNamesCache.Add($"{item.Name.Replace(" ", "")}_{i}", i);
			}
		}

		internal string GetAssetKey(string key, Mod mod)
		{
			if (_vanillaNamesCache == null)
			{
				FillVanillaNamesCache();
			}

			string kvpName = key;
			if (key.Contains('/'))
			{
				key = key.Substring(key.LastIndexOf('/') + 1);
			}

			// is an item name
			if (char.IsLetter(key.First()))
			{
				int index = key.LastIndexOf('_');
				key = new string(key.TakeWhile((x, i) => i < index).ToArray());
				var itemName = key;
				Item item = mod.GetItem(key)?.item;
				if (item == null)
				{
					if (!_vanillaNamesCache.Keys.Any(x => x.StartsWith(key.Replace(" ", ""))))
					{
						return null;
					}

					item = new Item();
					item.SetDefaults(_vanillaNamesCache[_vanillaNamesCache.Keys.First(x => x.StartsWith(key.Replace(" ", "")))]);
				}

				return kvpName.Replace(itemName, item.type.ToString());
			}

			// is an item id
			if (char.IsNumber(key.First()))
			{
				int index = key.LastIndexOf('_');
				string itemId = new string(key.TakeWhile((x, i) => i < index).ToArray());
				if (int.Parse(itemId) >= ItemLoader.ItemCount) return null;
				return kvpName;
			}

			return null;
		}

		internal bool AnyGlowmaskAssetExists(string key, Mod mod)
		{
			GetItemKey(key);
			return _glowmaskTextures.Any(x => x.Key.StartsWith(mod.Name) && x.Key.Contains($"{key}_Glowmask") || x.Key.Contains($"{key}_Glow"));
		}

		internal bool AnyShaderAssetExists(string key, Mod mod)
		{
			key = GetItemKey(key);
			return _shaderTextures.Any(x => x.Key.StartsWith(mod.Name) && x.Key.Contains($"{key}_Shader") || x.Key.Contains($"{key}_Shad"));
		}

		private string GetItemKey(string key)
		{
			if (key.Contains('/'))
			{
				key = key.Substring(key.LastIndexOf('/') + 1);
			}

			int index = key.LastIndexOf('_');
			return new string(key.TakeWhile((x, i) => i < index).ToArray());
		}

		// @todo expose a global GraphicsContent instance and prepare automagically in context ?
		public void Prepare(Mod mod)
		{
			if (_keyStore.TryGetValue(mod.Name, out var keyPass))
			{
				var glowmasks = _glowmaskTextures.Where(x => x.Key.StartsWith(keyPass)).ToArray();
				var shaders = _glowmaskTextures.Where(x => x.Key.StartsWith(keyPass)).ToArray();

				_lookupTable =
					_textures.Where(x => x.Key.StartsWith(keyPass))
						.Concat(glowmasks)
						.Concat(shaders)
						.ToDictionary(x => SubstringLastIndex('/', x.Key), x => x.Value);

				_lookupGlowmaskTable = glowmasks.ToDictionary(x => SubstringLastIndex('/', x.Key), x => x.Value);
				_lookupShaderTable = shaders.ToDictionary(x => SubstringLastIndex('/', x.Key), x => x.Value);
			}
			else
			{
				throw new Exception($"Tried to get keyPass from keyStore for mod {mod.Name} but key not present");
			}
		}

		public void Prepare(Item item)
		{
			string itemId = item.type.ToString();
			var glowmasks = _glowmaskTextures.Where(x => GetItemKey(x.Key).Equals(itemId)).ToArray();
			var shaders = _shaderTextures.Where(x => GetItemKey(x.Key).Equals(itemId)).ToArray();

			_lookupTable =
				_textures.Where(x => GetItemKey(x.Key).Equals(itemId))
					.Concat(glowmasks)
					.Concat(shaders)
					.ToDictionary(x => SubstringLastIndex('/', x.Key), x => x.Value);

			_lookupGlowmaskTable = glowmasks.ToDictionary(x => SubstringLastIndex('/', x.Key), x => x.Value);
			_lookupShaderTable = shaders.ToDictionary(x => SubstringLastIndex('/', x.Key), x => x.Value);
		}

		public void ClearPreparation()
		{
			_lookupTable.Clear();
			_lookupGlowmaskTable.Clear();
			_lookupShaderTable.Clear();
		}

		public Texture2D GetPreparedTexture(string key, bool clearPreparation = true)
		{
			return GetPreparedAsset(_lookupTable, key, clearPreparation);
		}

		public Texture2D GetPreparedGlowmask(string key, bool clearPreparation = true)
		{
			Texture2D glowmask = GetPreparedAsset(_lookupGlowmaskTable, $"{key}_Glowmask", clearPreparation);
			if (glowmask != null) return glowmask;
			return GetPreparedAsset(_lookupGlowmaskTable, $"{key}_Glow", clearPreparation);
		}

		public Texture2D GetPreparedShader(string key, bool clearPreparation = true)
		{
			Texture2D shader = GetPreparedAsset(_lookupShaderTable, $"{key}_Shader", clearPreparation);
			if (shader != null) return shader;
			return GetPreparedAsset(_lookupShaderTable, $"{key}_Shad", clearPreparation);
		}

		private Texture2D GetPreparedAsset(IDictionary<string, Texture2D> dict, string key, bool clearPreparation = true)
		{
			Texture2D texture = GetFrom(dict, key);
			if (clearPreparation && texture != null)
			{
				ClearPreparation();
			}

			return texture;
		}

		public Texture2D GetGlowmaskTexture(string key)
		{
			return GetFrom(_glowmaskTextures, key);
		}

		public void AddGlowmaskTexture(string key, Texture2D texture)
		{
			AddTo(_glowmaskTextures, key, texture);
		}

		public Texture2D GetShaderTexture(string key)
		{
			return GetFrom(_shaderTextures, key);
		}

		public void AddShaderTexture(string key, Texture2D texture)
		{
			AddTo(_shaderTextures, key, texture);
		}

		protected override void ProcessTexture2D(ref Texture2D texture)
		{
			texture = MultiplyColorsByAlpha(texture);
		}

		//to clarify, this code simply 'fixes' out bad saving of the file in photoshop or whatever
		//yeah, it works for every pixel individually
		//some art programs save the file with every pixel having an RGBA where RGB is bigger than 0 but A may be 0 , where A would usually be transparency there, in XNA's texture loading the alpha channel is independent , and with terraria's draw mode it actually can do awesome things
		//in terraria, A channel signals additive<---> opaque
		//a pixel with 255 alpha draws fully opaque, a pixel with 0 alpha draws fully additive
		//so if you do say, 255,0,0,0 color, it would draw a fully additive red
		//which can be awesome...except when your file saves the alpha channel of supposedly transparent pixels wrong
		public static Texture2D MultiplyColorsByAlpha(Texture2D texture)
		{
			Color[] data = new Color[texture.Width * texture.Height];
			texture.GetData(data);
			for (int i = 0; i < data.Length; i++)
			{
				Vector4 we = data[i].ToVector4();
				data[i] = new Color(we.X * we.W, we.Y * we.W, we.Z * we.W, we.W);
			}

			texture.SetData(data);
			return texture;
		}

		private string SubstringLastIndex(char chr, string str)
		{
			int index = str.LastIndexOf(chr);
			if (index != -1)
			{
				return str.Substring(index + 1);
			}

			return null;
		}

		protected override void Load()
		{
			//var assembly = Assembly.GetExecutingAssembly();
			//var types = assembly.GetTypes().Where(x => !x.IsAbstract && x.IsClass).ToList();

			//IList<string> exceptKeys = new List<string>();

			// @todo
			// @todo load entity/shader based on ItemID in Assets/Shader or Assets/Glowmask folders
			// @todo Item_{Id} -> The usecase here is on GlobalItem, not ModItem

			// Loads regular types by having a ShaderEntity or GlowmaskEntity
			//foreach (var type in types)
			//{
			//	try
			//	{
			//var shaderEntity = type.GetProperty("ShaderEntity");
			//var getShaderEntity = type.GetMethod("GetShaderEntity");
			//string key;

			//if (shaderEntity != null && getShaderEntity != null)
			//{
			//	// Get the shader entity
			//	object obj = FormatterServices.GetUninitializedObject(type);
			//	ShaderEntity entity = (ShaderEntity)getShaderEntity.Invoke(obj, null);

			//	// Load the base texture
			//	key = type.FullName;
			//	var texture = ModLoader.GetTexture(key.Replace('.', '/'));
			//	AddTexture(key, texture);
			//	exceptKeys.Add(key);

			//	// Load the shader texture
			//	key = entity.GetEntityKey(type);
			//	texture = ModLoader.GetTexture(key.Replace('.', '/'));
			//	AddShaderTexture(key, texture);
			//	exceptKeys.Add(key);
			//}

			//var glowmaskEntity = type.GetProperty("GlowmaskEntity");
			//var getGlowmaskEntity = type.GetMethod("GetGlowmaskEntity");

			//if (glowmaskEntity != null && getGlowmaskEntity != null)
			//{
			//	// Get the glowmask entity
			//	object obj = FormatterServices.GetUninitializedObject(type);
			//	GlowmaskEntity entity = (GlowmaskEntity)getGlowmaskEntity.Invoke(obj, null);
			//	Texture2D texture;

			//	// Try loading the base texture
			//	key = type.FullName;
			//	if (!exceptKeys.Contains(key))
			//	{
			//		texture = ModLoader.GetTexture(key.Replace('.', '/'));
			//		AddTexture(key, texture);
			//		exceptKeys.Add(key);
			//	}

			//	// Try loading the glowmask
			//	key = entity.GetEntityKey(type);
			//	texture = ModLoader.GetTexture(key.Replace('.', '/'));
			//	AddGlowmaskTexture(key, texture);
			//	exceptKeys.Add(key);
			//}
			//	}
			//	catch (Exception e)
			//	{
			//		// fluff
			//		ErrorLogger.Log(e.ToString());
			//	}
			//}

			// @todo support above usecase as well
			// load all types that are marked for autoloading
			//var autoloadTypes =
			//	from t in assembly.GetTypes()
			//	let attribute = t.GetCustomAttribute<AutoloadModContent>(true)
			//	where attribute != null
			//	select new { Type = t, Attribute = attribute };

			//// hijack the textures dictionary
			//FieldInfo texturesField = typeof(Mod).GetField("textures", BindingFlags.Instance | BindingFlags.NonPublic);
			//var textures = (Dictionary<string, Texture2D>)texturesField.GetValue(Loot.Instance);

			//foreach (var autoload in autoloadTypes)
			//{
			//	//var fullName = autoload.Type.FullName.Replace('.', '/');
			//	var name = autoload.Type.Name;
			//	//var path = autoload.Type.Namespace;
			//	var compatibleTextures = textures
			//		.Where(x =>
			//		{
			//			var sub = SubstringLastIndex('/', x.Value.Name);
			//			return sub != null && sub.Contains(name);
			//		})
			//		.Select(x => new KeyValuePair<string, Texture2D>(x.Value.Name.Replace('/', '.'), x.Value));

			//	var attr = autoload.Attribute;

			//	if (attr == null)
			//		throw new NullReferenceException("Attribute was null during ModGraphicsContent Load");

			//	// try adding all textures
			//	foreach (var compatibleTexture in compatibleTextures.Where(x => !exceptKeys.Contains(x.Key)))
			//	{
			//		AddTexture(compatibleTexture.Key, compatibleTexture.Value);
			//	}
			//}
		}
	}
}
