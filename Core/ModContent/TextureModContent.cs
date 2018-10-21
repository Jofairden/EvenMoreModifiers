using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Core.ModContent
{
	public abstract class TextureModContent
	{
		protected IDictionary<string, Texture2D> _textures;

		protected Texture2D GetFrom(IDictionary<string, Texture2D> dictionary, string key)
		{
			Texture2D texture;
			return dictionary.TryGetValue(key, out texture) ? texture : null;
		}

		public Texture2D GetTexture(string key)
		{
			return GetFrom(_textures, key);
		}

		protected void AddTo(IDictionary<string, Texture2D> dictionary, string key, Texture2D texture)
		{
			if (dictionary.ContainsKey(key))
			{
				throw new Exception($"Key '{key}' already present in ModContent");
			}
			ProcessTexture2D(ref texture);
			dictionary.Add(key, texture);
		}

		public void AddTexture(string key, Texture2D texture)
		{
			AddTo(_textures, key, texture);
		}

		public void _Initialize()
		{
			_textures = new Dictionary<string, Texture2D>();
			Initialize();
		}

		public void _Load()
		{
			Load();
		}

		public void _Unload()
		{
			_textures = null;
			Unload();
		}

		protected virtual void ProcessTexture2D(ref Texture2D texture) { }
		protected virtual void Initialize() { }
		protected virtual void Load() { }
		protected virtual void Unload() { }

		public abstract string GetRegistryKey();
	}
}
