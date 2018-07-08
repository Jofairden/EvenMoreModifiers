using System;
using System.Linq;
using System.Reflection;
using Loot;
using Loot.Core;
using Loot.UI;
using LootTests.TestModifiers;
using LootTests.TestPools;
using LootTests.TestRarities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
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
		public static readonly EmptyModLoadShell _modShell = new EmptyModLoadShell();

		[Test]
		public void TestModifierProperties()
		{
			Assert.AreEqual(new ModifierProperties(), new ModifierProperties());

			var p = new ModifierProperties();
			Assert.AreEqual(p.Magnitude, 0f);
			Assert.AreEqual(p.Power, 0f);
			Assert.AreEqual(p.RoundedPower, 0f);

			float n = 10f, m = n * 2f;
			p = new ModifierProperties(minMagnitude: n, maxMagnitude: m);
			Assert.AreEqual(p.MinMagnitude, n);
			Assert.AreEqual(p.MaxMagnitude, m);
			Assert.AreEqual(p.MinMagnitude, p.MaxMagnitude - n);
			Assert.AreEqual(p.MaxMagnitude, p.MinMagnitude + n);
			Assert.IsTrue(p.MinMagnitude <= p.MaxMagnitude);
			Assert.IsTrue(p.MaxMagnitude >= p.MinMagnitude);

			p = p.RollMagnitudeAndPower(10f, 100f);
			var tc = ModifierProperties._Save(null, p);
			Assert.IsInstanceOf<TagCompound>(tc);
			Assert.IsTrue(tc.ContainsKey("Magnitude"));
			Assert.IsTrue(tc.ContainsKey("Power"));
			Assert.IsTrue(tc.ContainsKey("ModifierPropertiesSaveVersion"));
			Assert.AreEqual(tc.Get<float>("Magnitude"), 10f);
			Assert.AreEqual(tc.Get<float>("Power"), 100f);
			Assert.IsTrue(tc.Get<int>("ModifierPropertiesSaveVersion") > 0);

			p = ModifierProperties._Load(null, tc);
			Assert.IsInstanceOf<ModifierProperties>(p);
			Assert.AreEqual(p.Magnitude, 10f);
			Assert.AreEqual(p.Magnitude, tc.Get<float>("Magnitude"));
			Assert.AreEqual(p.Power, 100f);
			Assert.AreEqual(p.Power, tc.Get<float>("Power"));
		}

		[Test]
		public void TestEMMLoader()
		{
			EMMLoader.Initialize();
			EMMLoader.Load();

			Assert.IsTrue(EMMLoader.ReserveRarityID() == 0);
			Assert.IsTrue(EMMLoader.ReserveModifierID() == 0);
			Assert.IsTrue(EMMLoader.ReservePoolID() == 0);

			Assert.IsTrue(EMMLoader.RaritiesMap.Count == 0);
			Assert.IsTrue(EMMLoader.ModifiersMap.Count == 0);
			Assert.IsTrue(EMMLoader.PoolsMap.Count == 0);
			Assert.IsTrue(EMMLoader.GlobalModifiersMap.Count == 0);
			Assert.IsTrue(EMMLoader.Rarities.Count == 0);
			Assert.IsTrue(EMMLoader.Modifiers.Count == 0);
			Assert.IsTrue(EMMLoader.Pools.Count == 0);
			Assert.IsTrue(EMMLoader.GlobalModifiers.Count == 0);
			Assert.IsTrue(EMMLoader.Mods.Count == 0);

			Assert.Throws<Exception>(() => EMMLoader.RegisterMod(_modShell));

			var f = _modShell.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic);
			f.SetValue(_modShell, true);

			EMMLoader.RegisterMod(_modShell);

			Assert.Throws<Exception>(() => EMMLoader.RegisterMod(_modShell));

			Assert.IsTrue(EMMLoader.Mods.Count == 1);
			Assert.IsTrue(EMMLoader.RaritiesMap.Count == 1);
			Assert.IsTrue(EMMLoader.ModifiersMap.Count == 1);
			Assert.IsTrue(EMMLoader.PoolsMap.Count == 1);
			Assert.IsTrue(EMMLoader.GlobalModifiersMap.Count == 1);
			
			EMMLoader.SetupContent(_modShell);
			
			Assert.IsTrue(EMMLoader.Modifiers.Any(x=> x.Value.GetType() == typeof(TestApplyMod)));
			Assert.IsTrue(EMMLoader.Rarities.Any(x=> x.Value.GetType() == typeof(TestRarity)));
			Assert.IsTrue(EMMLoader.Pools.Any(x=> x.Value.GetType() == typeof(TestPool)));
		}

		[Test]
		public void TestModifierPool()
		{
			TestEMMLoader();

			var testPool = EMMLoader.Pools.FirstOrDefault(x => x.Value.GetType() == typeof(TestPool));
			Assert.AreNotEqual(testPool, null);
			
		}
		
		[Test]
		public void LogEMMLoaderEntries()
		{
			var mod = new Loot.Loot();

			var f = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic);
			f.SetValue(mod, true);

			Terraria.Main.instance = new Main();
			Terraria.Main.dedServ = true;
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