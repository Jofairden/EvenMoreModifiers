using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Loot.Api.Loaders;
using Terraria.ModLoader;

namespace Loot.Api.Content
{
	/// <summary>
	/// This class is used for Content holders in the Content namespace
	/// As a modder you should not use this class
	/// Even though it is public, this is only because
	/// the fields on <see cref="ContentLoader"/> must be accessible to modders
	/// </summary>
	public abstract class LoadableContentBase<T> where T : ILoadableContent, ICloneable
	{
		internal Dictionary<string, List<KeyValuePair<string, T>>> Map;
		internal Dictionary<uint, T> Content;

		internal bool SkipModChecks;

		public uint IdCount { get; private set; }

		private uint GetNextId()
		{
			uint @return = IdCount;
			IdCount++;
			return @return;
		}

		internal void _Initialize()
		{
			Map = new Dictionary<string, List<KeyValuePair<string, T>>>();
			Content = new Dictionary<uint, T>();

			Initialize();
		}

		internal virtual void Initialize()
		{
		}

		internal void _Load()
		{
			Load();
		}

		internal virtual void Load()
		{
		}

		internal void _Unload()
		{
			Map?.Clear();
			Content?.Clear();
			IdCount = 0;

			Unload();
		}

		internal virtual void Unload()
		{
		}

		internal void AddMod(Mod mod)
		{
			Map.Add(mod.Name, new List<KeyValuePair<string, T>>());
		}

		internal virtual bool CheckContentPiece(T contentPiece) => true;

		public void AddContent(Type type, Mod mod)
		{
			T contentPiece = (T)Activator.CreateInstance(type);
			AddContent(contentPiece, mod);
		}

		public void AddContent(T contentPiece, Mod mod)
		{
			if (!typeof(ILoadableContentSetter).IsAssignableFrom(typeof(T)))
			{
				throw new Exception("Invalid type passed to AddContent");
			}

			string s = $"[{nameof(T)}]{nameof(contentPiece)}";
			if (!SkipModChecks)
			{
				RegistryLoader.CheckModLoading(mod, s);
				RegistryLoader.CheckModRegistered(mod);
			}

			if (!Map.TryGetValue(mod.Name, out var lrm))
			{
				throw new Exception($"Map for {mod.Name} not found (trying to add {s}");
			}

			if (!CheckContentPiece(contentPiece))
			{
				throw new Exception($"A problem occurred trying to add {s}");
			}

			if (lrm.Exists(x => x.Key.Equals(contentPiece.Name)))
			{
				throw new Exception($"You have already added {s}");
			}

			((ILoadableContentSetter)contentPiece).Mod = mod;
			((ILoadableContentSetter)contentPiece).Type = GetNextId();
			Content[contentPiece.Type] = contentPiece;
			Map[mod.Name].Add(new KeyValuePair<string, T>(contentPiece.Name, contentPiece));
		}

		public T GetContent(Type type)
		{
			T contentPiece = Content.Values.FirstOrDefault(x => x.GetType().FullName == type.FullName);
			return (T)contentPiece?.Clone();
		}

		public T GetContent(string modName, string contentKey)
		{
			T contentPiece = _GetContent(modName, contentKey);
			return (T)contentPiece?.Clone();
		}

		private T _GetContent(string modName, string key)
		{
			return Map[modName].FirstOrDefault(x => x.Key.Equals(key)).Value;
		}

		public T GetContent(uint type)
		{
			return type < IdCount ? (T)Content[type].Clone() : default;
		}

		public ReadOnlyCollection<T> GetContent()
			=> Content.Select(e => (T)e.Value?.Clone()).ToList().AsReadOnly();
	}
}
