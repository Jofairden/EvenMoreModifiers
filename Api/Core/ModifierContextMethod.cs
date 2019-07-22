namespace Loot.Api.Core
{
	/// <summary>
	/// Defines a method for a context in which a Modifier might be rolled
	/// Used in <see cref="ModifierContext"/>
	/// </summary>
	public enum ModifierContextMethod
	{
		OnReforge,
		OnCraft,
		SetupStartInventory,
		WorldGeneration,
		FirstLoad,
		OnPickup,
		Custom,
		OnCubeReroll
	}
}
