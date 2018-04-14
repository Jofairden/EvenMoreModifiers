using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Loot.System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terraria.ModLoader.IO;

namespace Loot
{
	[TestClass]
	public class UnitTest
	{
		[TestMethod]
		public void TestModifierProperties()
		{
			//Assert.AreEqual(new ModifierProperties(), new ModifierProperties());

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
			var tc = p.Save();
			Assert.IsInstanceOfType(tc, typeof(TagCompound));
			Assert.IsTrue(tc.ContainsKey("Magnitude"));
			Assert.IsTrue(tc.ContainsKey("Power"));
			Assert.AreEqual(tc.Get<float>("Magnitude"), 10f);
			Assert.AreEqual(tc.Get<float>("Power"), 100f);

			p = ModifierProperties.Load(tc);
			Assert.IsInstanceOfType(p, typeof(ModifierProperties));
			Assert.AreEqual(p.Magnitude, 10f);
			Assert.AreEqual(p.Magnitude, tc.Get<float>("Magnitude"));
			Assert.AreEqual(p.Power, 100f);
			Assert.AreEqual(p.Power, tc.Get<float>("Power"));
		}

		[TestMethod]
		public void TestEMMLoader()
		{
			EMMLoader.Initialize();
			EMMLoader.Load();

			Assert.IsTrue(EMMLoader.ReserveRarityID() == 0);
			Assert.IsTrue(EMMLoader.ReserveModifierID() == 0);
			Assert.IsTrue(EMMLoader.ReservePoolID() == 0);
		}

		[TestMethod]
		public void LogModifiers()
		{
			var mod = new Loot();

			var f = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic);
			f.SetValue(mod, true);

			mod.Load();

			f.SetValue(mod, false);

			Console.Write(string.Join("\n", EMMLoader.RequestModifierRarities().Select(x => x.Name)));
			Console.Write(string.Join("\n", EMMLoader.RequestModifiers().Select(x => x.Description)));
		}
	}
}
