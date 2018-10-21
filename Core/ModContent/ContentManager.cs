using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace Loot.Core.ModContent
{
	public class ContentManager
	{
		private IDictionary<string, TextureContent> _contents;
		private IEnumerable<TextureContent> _modContents => _contents.Select(x => x.Value);

		public T GetContent<T>() where T : TextureContent
		{
			T content = (T)_modContents.FirstOrDefault(x => x.GetType() == typeof(T));
			return content;
		}

		public TextureContent GetContent(Type type)
		{
			TextureContent content = _modContents.FirstOrDefault(x => x.GetType() == type);
			return content;
		}

		public TextureContent GetContent(string key)
		{
			TextureContent content;
			return _contents.TryGetValue(key, out content) ? content : null;
		}

		public void AddContent(string key, TextureContent textureContent)
		{
			if (_contents.ContainsKey(key))
			{
				throw new Exception($"Key '{key}' already present in ContentManager");
			}

			if (_contents.Values.Contains(textureContent))
			{
				// TODO warn
				ErrorLogger.Log($"ModContent with registry key {textureContent.GetRegistryKey()} was already present");
			}

			textureContent._Initialize();
			_contents.Add(key, textureContent);
		}

		internal void Initialize(Mod mod)
		{
			_contents = new Dictionary<string, TextureContent>();
			var assembly = mod.Code;
			var modContents = assembly.GetTypes().Where(x => x.BaseType == typeof(TextureContent) && !x.IsAbstract);
			foreach (var modContent in modContents)
			{
				TextureContent obj = (TextureContent)Activator.CreateInstance(modContent);
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
