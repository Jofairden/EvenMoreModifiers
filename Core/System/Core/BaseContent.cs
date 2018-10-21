using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Core.System.Loaders;
using Terraria.ModLoader;

namespace Loot.Core.System.Core
{
	public abstract class BaseContent<T> where T : ILoadableContent, ICloneable
	{
		internal IDictionary<string, List<KeyValuePair<string, T>>> Map;
		internal IDictionary<uint, T> Content;

		internal bool SkipModChecks;

		public uint IdCount { get; private set; }

		public uint GetNextId()
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
			Map.Clear();
			Content.Clear();
			IdCount = 0;

			Unload();
		}

		internal virtual void Unload()
		{
		}

		internal void AddMod(Mod mod)
		{
			Map.Add(new KeyValuePair<string, List<KeyValuePair<string, T>>>(mod.Name, new List<KeyValuePair<string, T>>()));
		}

		internal virtual bool CheckContentPiece(T contentPiece) => true;

		public void AddContent(Type type, Mod mod)
		{
			T contentPiece = (T)Activator.CreateInstance(type);
			string s = $"[{nameof(T)}]{nameof(contentPiece)}";
			if (!SkipModChecks)
			{
				MainLoader.CheckModLoading(mod, s);
				MainLoader.CheckModRegistered(mod);
			}

			List<KeyValuePair<string, T>> lrm;
			if (!Map.TryGetValue(mod.Name, out lrm))
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
			return (T)contentPiece.Clone();
		}

		private T _GetContent(string modName, string key)
		{
			return Map[modName].FirstOrDefault(x => x.Key.Equals(key)).Value;
		}

		public T GetContent(uint type)
		{
			return type < IdCount ? (T)Content[type].Clone() : default(T);
		}

		// I wish we could use dynamic :(

		//private T BuildNewContentInstance(T contentPiece)
		//{
		//	if (contentPiece == null)
		//	{
		//		throw new ArgumentNullException(nameof(contentPiece), $"contentPiece was null when trying to construct a new instance of it.");
		//	}

		//	Exception exc;
		//	T instance = (T)Activator.CreateInstance(contentPiece.GetType());
		//	try
		//	{
		//		dynamic dynPiece = (dynamic)contentPiece;
		//		dynamic dynInstance = (dynamic)instance;
		//		dynInstance.Type = dynPiece.Type;
		//		dynInstance.Mod = dynPiece.Mod;
		//		return (T)dynInstance;
		//	}
		//	catch (Exception e)
		//	{
		//		exc = e;
		//	}
		//	throw new Exception($"There was an unknown constructing a new instance of [{nameof(T)}]{nameof(contentPiece)}" +
		//						$"\n{exc}");
		//}

		public IReadOnlyCollection<T> RequestContent()
			=> Content.Select(e => (T)e.Value?.Clone()).ToList().AsReadOnly();
	}
}
