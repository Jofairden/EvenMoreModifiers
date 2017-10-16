using System;
using Loot.Modifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LootTests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void LogModifiers()
		{
			var mod = new Loot.Loot();
			mod.Load();

			foreach (Modifier modifier in ModifierLoader.RequestModifiers())
			{
				Console.Write(modifier);
			}
			foreach (ModifierRarity rarity in ModifierLoader.RequestRarities())
			{
				Console.Write(rarity);
			}
		}
	}
}
