using System;
using System.Diagnostics;
using System.Reflection;
using Loot;
using Moq;
using NUnit.Framework;
using Terraria;
using Terraria.ModLoader;

namespace LootTests
{
	public interface IFakeMod
	{
		string Name { get; }
		Assembly Code { get; }
	}

	public class FakeMod : Mod, IFakeMod
	{
		// @todo if we inherit from Mod, we somehow don't stub Code correctly
		public new virtual Assembly Code { get; }
	}

	[TestFixture]
	internal class EMMLoaderTests
	{
		[SetUp]
		public void Setup()
		{
			Main.instance = new Main();
			Main.dedServ = true;
			EMMLoader.Initialize();
			EMMLoader.Load();
		}

		[Test]
		public void TestRegisterMod()
		{
			var fakeMod = new Mock<FakeMod>();

			// since load has to be non public we can't stub it
			fakeMod.Object.GetType()
				.GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)
				?.SetValue(fakeMod.Object, false);
			
			Assert.Throws<Exception>(() => EMMLoader.RegisterMod(fakeMod.Object));

			fakeMod.Object.GetType()
				.GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)
				?.SetValue(fakeMod.Object, true);

			fakeMod.SetupGet(x => x.Name).Returns("TestMod");
			// @todo if we inherit from Mod, we somehow don't stub Code correctly
			fakeMod.SetupGet(x => x.Code).Returns(Assembly.GetExecutingAssembly());

			Assert.AreEqual(fakeMod.Object.Name, "TestMod");
			Assert.AreEqual(fakeMod.Object.Code, Assembly.GetExecutingAssembly());

			EMMLoader.RegisterMod(fakeMod.Object);

			fakeMod.VerifyGet(x => x.Name, Times.Exactly(7));
			fakeMod.VerifyGet(x => x.Code, Times.Once);
			TestData(1, testReserves:false, testDictionaries:false);
		}

		[Test]
		public void TestSetupMod()
		{
			EMMLoader.Initialize();
			EMMLoader.Load();

			var fakeMod = new Mock<FakeMod>();

			fakeMod.Object.GetType()
				.GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)
				?.SetValue(fakeMod.Object, true);

			fakeMod.SetupGet(x => x.Name).Returns("TestMod");
			// @todo figure out how to stub Code assembly
			//fakeMod.SetupGet(x => x.Code).Returns(Assembly.LoadFrom(TestContext.CurrentContext.TestDirectory + "\\LootTests.dll"));

			Assert.AreEqual(fakeMod.Object.Name, "TestMod");
			//Assert.AreEqual(fakeMod.Object.Code, Assembly.LoadFrom(TestContext.CurrentContext.TestDirectory + "\\LootTests.dll"));

			EMMLoader.RegisterMod(fakeMod.Object);
			FakeMod obj = fakeMod.Object;
			obj.Load();
			EMMLoader.SetupContent(obj);

			//Assert.IsTrue(EMMLoader.Modifiers.Any(x => x.Value.GetType() == typeof(TestApplyMod)));
			//Assert.IsTrue(EMMLoader.Rarities.Any(x => x.Value.GetType() == typeof(TestRarity)));
			//Assert.IsTrue(EMMLoader.Pools.Any(x => x.Value.GetType() == typeof(TestPool)));
		}

		//[Test]
		//public void TestModifierPool()
		//{
		//	TestSetupMod();

		//	var testPool = EMMLoader.Pools.FirstOrDefault(x => x.Value.GetType() == typeof(TestPool));
		//	Assert.AreNotEqual(testPool, null);
		//}

		private void TestData(int num, bool testReserves = true, bool testMaps = true, bool testDictionaries = true, bool testMods = true)
		{
			if (testReserves)
			{
				Assert.IsTrue(EMMLoader.ReserveRarityID() == num);
				Assert.IsTrue(EMMLoader.ReserveModifierID() == num);
				Assert.IsTrue(EMMLoader.ReservePoolID() == num);
				Assert.IsTrue(EMMLoader.ReserveEffectID() == num);
			}

			if (testMaps)
			{
				Assert.IsTrue(EMMLoader.RaritiesMap.Count == num);
				Assert.IsTrue(EMMLoader.ModifiersMap.Count == num);
				Assert.IsTrue(EMMLoader.PoolsMap.Count == num);
				Assert.IsTrue(EMMLoader.EffectsMap.Count == num);
			}

			if (testDictionaries)
			{
				Assert.IsTrue(EMMLoader.Rarities.Count == num);
				Assert.IsTrue(EMMLoader.Modifiers.Count == num);
				Assert.IsTrue(EMMLoader.Pools.Count == num);
				Assert.IsTrue(EMMLoader.Effects.Count == num);
			}

			if (testMods)
			{
				Assert.IsTrue(EMMLoader.Mods.Count == num);
			}
		}

		[Test]
		public void TestInitialize()
		{
			EMMLoader.Initialize();
			EMMLoader.Load();

			TestData(0);
		}
	}
}
