using Loot.Api.Loaders;

namespace Loot.ILEditing
{
	/// <summary>
	/// A base class for ILEdits that is loaded automatically by <see cref="LoadingFunneler"/>
	/// </summary>
	internal abstract class ILEdit
	{
		public abstract void Apply(bool dedServ);
	}
}
