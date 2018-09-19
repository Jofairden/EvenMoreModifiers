using System;
using System.Linq;
using System.Reflection;
using Loot;
using Loot.UI;
using NUnit.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using Assert = NUnit.Framework.Assert;

namespace LootTests
{
	public sealed class EmptyModLoadShell : Mod
	{
		public override string Name => "LootTestsModShell";
		public static EmptyModLoadShell Instance;
		
		public EmptyModLoadShell()
		{
			Instance = this;
		}
	}


	[TestFixture]
	public class UnitTests
	{
		[SetUp]
		public void Setup()
		{
			Main.instance = new Main();
			Main.dedServ = true;
			Main.rand = new UnifiedRandom();
		}

		public static readonly EmptyModLoadShell _modShell = new EmptyModLoadShell();
		
		[Test]
		public void LogEMMLoaderEntries()
		{
			var mod = new Loot.Loot();

			var f = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic);
			f.SetValue(mod, true);

			mod.Load();
			
			f.SetValue(mod, false);
			
			f = _modShell.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic);
			f.SetValue(_modShell, true);

			EMMLoader.RegisterMod(_modShell);
			EMMLoader.SetupContent(_modShell);

			Console.WriteLine("Modifier rarities:");
			Console.WriteLine();
			Console.WriteLine(string.Join("\n", EMMLoader.RequestModifierRarities().Select(x => x.Name)));
			Console.WriteLine();
			Console.WriteLine("Modifiers:");
			Console.WriteLine();
			Console.WriteLine(string.Join("\n", EMMLoader.RequestModifiers().Select(x => x.Name)));
			Console.WriteLine();
			Console.WriteLine("Modifier pools:");
			Console.WriteLine();
			Console.WriteLine(string.Join("\n", EMMLoader.RequestModifierPools().Select(x => x.Name)));
			Console.WriteLine();
		}

		[Test]
		public void TestCubeUI()
		{
			TestCubeRerollUI();
			TestCubeSealUI();
			
			void TestCubeRerollUI()
			{
				var ui = new CubeRerollUI();
				var backPanel = ui.GetType().GetField("_backPanel", BindingFlags.Instance | BindingFlags.NonPublic);
				if (backPanel != null) Assert.AreEqual(backPanel.GetValue(ui), null);
				Assert.AreEqual(ui._cubePanel, null);
				Assert.AreEqual(ui._rerollItemPanel, null);
//				ui.Activate();
//				if (backPanel != null) Assert.AreNotEqual(backPanel.GetValue(ui), null);
//				Assert.AreNotEqual(ui._cubePanel, null);
//				Assert.AreNotEqual(ui._rerollItemPanel, null);
//				
//				var dragPanel = ui.GetType().GetField("_dragPanel", BindingFlags.Instance | BindingFlags.NonPublic);
//				if (dragPanel != null && backPanel != null)
//				{
//					Assert.AreEqual(dragPanel.GetValue(ui), backPanel.GetValue(ui));
//				}
			}

			void TestCubeSealUI()
			{
				var ui = new CubeSealUI();
				var backPanel = ui.GetType().GetField("_backPanel", BindingFlags.Instance | BindingFlags.NonPublic);
				if (backPanel != null) Assert.AreEqual(backPanel.GetValue(ui), null);
				Assert.AreEqual(ui._itemPanel, null);
			}
		}
		

	}
}
