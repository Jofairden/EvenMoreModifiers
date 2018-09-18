using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Loot.Core.ModContent
{
	public class ModGraphicsContent : ModContent
	{
		protected IDictionary<string, Texture2D> _glowmaskTextures;
		protected IDictionary<string, Texture2D> _shaderTextures;
		private IDictionary<string, Texture2D> _lookupTable;

		protected override void Initialize()
		{
			_glowmaskTextures = new Dictionary<string, Texture2D>();
			_shaderTextures = new Dictionary<string, Texture2D>();
			_lookupTable = new Dictionary<string, Texture2D>();
		}

		protected override void Unload()
		{
			_glowmaskTextures = null;
			_shaderTextures = null;
			_lookupTable = null;
		}

		public override string GetRegistryKey()
		{
			return "ModGraphics";
		}

		// @todo expose a global GraphicsContent instance and prepare automagically in context ?
		public void Prepare(object obj)
		{
			string nm = obj.GetType().Namespace;
			_lookupTable =
				_textures.Where(x => x.Key.StartsWith(nm))
					.Concat(_glowmaskTextures.Where(x => x.Key.StartsWith(nm)))
					.Concat(_shaderTextures.Where(x => x.Key.StartsWith(nm)))
					.ToDictionary(x => SubstringLastIndex('.', x.Key), x => x.Value);
		}

		public void ClearPreparation()
		{
			_lookupTable.Clear();
		}

		public Texture2D GetPreparedTexture(string key, bool clearPreperation = true)
		{
			Texture2D texture = GetFrom(_lookupTable, key);
			if (clearPreperation && texture != null)
				ClearPreparation();
			return texture;
		}

		public Texture2D GetGlowmaskTexture(string key)
		{
			return GetFrom(_glowmaskTextures, key);
		}

		protected void AddGlowmaskTexture(string key, Texture2D texture)
		{
			AddTo(_glowmaskTextures, key, texture);
		}

		public Texture2D GetShaderTexture(string key)
		{
			return GetFrom(_shaderTextures, key);
		}

		protected void AddShaderTexture(string key, Texture2D texture)
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
			var assembly = Assembly.GetExecutingAssembly();
			var types = assembly.GetTypes().Where(x => !x.IsAbstract && x.IsClass).ToList();

			IList<string> exceptKeys = new List<string>();

			// @todo
			// @todo load entity/shader based on ItemID in Assets/Shader or Assets/Glowmask folders
			// @todo Item_{Id} -> The usecase here is on GlobalItem, not ModItem

			// Loads regular types by having a ShaderEntity or GlowmaskEntity
			foreach (var type in types)
			{
				try
				{
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
				}
				catch (Exception e)
				{
					// fluff
					ErrorLogger.Log(e.ToString());
				}
			}

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
