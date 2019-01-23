using Terraria.ModLoader;

namespace Loot.Ext.ModSupport
{
	internal abstract class ModSupporter
	{
		public abstract string ModName { get; }
		public bool ModIsLoaded { internal set; get; }

		public Mod GetSupportingMod() => ModLoader.GetMod(ModName);

		public abstract bool CheckValidity(Mod mod);

		public virtual void AddClientSupport(Mod mod)
		{
		}

		public virtual void AddServerSupport(Mod mod)
		{
		}
	}
}
