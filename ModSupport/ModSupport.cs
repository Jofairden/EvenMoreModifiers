using Terraria.ModLoader;

namespace Loot.ModSupport
{
	/// <summary>
	/// The ModSupport class defines a class that adds cross-mod compatibility for another mod
	/// </summary>
	internal abstract class ModSupport
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
