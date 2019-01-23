using System.Collections.Generic;
using Terraria;

namespace Loot.Core.System.Modifier
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

	/// <summary>
	/// Defines a context in which a Modifier might be rolled
	/// Which fields are available (not null) depends on the method
	/// <list type="table">
	/// 
	/// <listheader>
	///	<term>Method</term>
	/// <term>Available fields</term>
	/// </listheader>
	/// 
	/// <item>
	/// <term><see cref="ModifierContextMethod.OnReforge"/> and <see cref="ModifierContextMethod.SetupStartInventory"/></term>
	/// <term>
	/// <list type="number">
	/// <term><see cref="Method"/></term>
	/// <term><see cref="Item"/></term>
	/// <term><see cref="Player"/></term>
	/// </list>
	/// </term>
	/// </item>
	/// 
	/// <item>
	/// <term><see cref="ModifierContextMethod.OnCraft"/></term>
	/// <term>
	/// <list type="number">
	/// <term><see cref="Method"/></term>
	/// <term><see cref="Item"/></term>
	/// <term><see cref="Player"/></term>
	/// <term><see cref="Recipe"/></term>
	/// </list>
	/// </term>
	/// </item>
	/// 
	/// <item>
	/// <term><see cref="ModifierContextMethod.WorldGeneration"/>, <see cref="ModifierContextMethod.FirstLoad"/></term>
	/// <term>
	/// <list type="number">
	/// <term><see cref="Method"/></term>
	/// <term><see cref="Item"/></term>
	/// <term><see cref="CustomData"/> - The data available is "chestData" which holds a tuple of chest.x and chest.y position</term>
	/// </list>
	/// </term>
	/// </item>
	/// </list>
	/// </summary>
	public struct ModifierContext
	{
		public IDictionary<string, object> CustomData;
		public ModifierContextMethod Method;
		public Player Player;
		public NPC NPC;
		public Item Item;
		public Recipe Recipe;
	}
}
