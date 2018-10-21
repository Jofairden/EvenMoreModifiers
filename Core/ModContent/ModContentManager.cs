using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace Loot.Core.ModContent
{
	public class ModContentManager
	{
		private IDictionary<string, TextureModContent> _contents;
		private IEnumerable<TextureModContent> _modContents => _contents.Select(x => x.Value);

		public T GetContent<T>() where T : TextureModContent
		{
			T content = (T)_modContents.FirstOrDefault(x => x.GetType() == typeof(T));
			return content;
		}

		public TextureModContent GetContent(Type type)
		{
			TextureModContent modContent = _modContents.FirstOrDefault(x => x.GetType() == type);
			return modContent;
		}

		public TextureModContent GetContent(string key)
		{
			TextureModContent modContent;
			return _contents.TryGetValue(key, out modContent) ? modContent : null;
		}

		public void AddContent(string key, TextureModContent textureModContent)
		{
			if (_contents.ContainsKey(key))
			{
				throw new Exception($"Key '{key}' already present in ContentManager");
			}

			if (_contents.Values.Contains(textureModContent))
			{
				// TODO warn
				ErrorLogger.Log($"ModContent with registry key {textureModContent.GetRegistryKey()} was already present");
			}

			textureModContent._Initialize();
			_contents.Add(key, textureModContent);
		}

		internal void Initialize(Mod mod)
		{
			_contents = new Dictionary<string, TextureModContent>();
			var assembly = mod.Code;
			var modContents = assembly.GetTypes().Where(x => x.BaseType == typeof(TextureModContent) && !x.IsAbstract);
			foreach (var modContent in modContents)
			{
				TextureModContent obj = (TextureModContent)Activator.CreateInstance(modContent);
				AddContent(obj.GetRegistryKey(), obj);
			}
		}

		internal void Load()
		{
			foreach (var content in _modContents)
			{
				content._Load();
			}
		}

		internal void Unload()
		{
			foreach (var content in _modContents)
			{
				content._Unload();
			}
			_contents = null;
		}
	}
}
