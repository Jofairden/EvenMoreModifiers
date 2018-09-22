using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace Loot.Core.ModContent
{
	public class ContentManager
	{
		private IDictionary<string, ModContent> _contents;
		private IEnumerable<ModContent> _modContents => _contents.Select(x => x.Value);

		public T GetContent<T>() where T : ModContent
		{
			T content = (T)_modContents.FirstOrDefault(x => x.GetType() == typeof(T));
			return content;
		}

		public ModContent GetContent(Type type)
		{
			ModContent content = (ModContent)_modContents.FirstOrDefault(x => x.GetType() == type);
			return content;
		}

		public ModContent GetContent(string key)
		{
			ModContent content;
			return _contents.TryGetValue(key, out content) ? content : null;
		}

		public void AddContent(string key, ModContent modContent)
		{
			if (_contents.ContainsKey(key))
			{
				throw new Exception($"Key '{key}' already present in ContentManager");
			}

			if (_contents.Values.Contains(modContent))
			{
				// TODO warn
			}

			modContent._Initialize();
			_contents.Add(key, modContent);
			if (Loot.Loaded)
			{
				modContent._Load();
			}
		}

		internal void Initialize(Mod mod)
		{
			_contents = new Dictionary<string, ModContent>();
			var assembly = mod.Code;
			var modContents = assembly.GetTypes().Where(x => x.BaseType == typeof(ModContent) && !x.IsAbstract);
			foreach (var modContent in modContents)
			{
				ModContent obj = (ModContent)Activator.CreateInstance(modContent);
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
