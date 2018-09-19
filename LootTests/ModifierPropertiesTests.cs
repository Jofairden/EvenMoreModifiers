using System;
using Loot.Core;
using Moq;
using NUnit.Framework;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace LootTests
{
	[TestFixture()]
	public class ModifierPropertiesTests
	{
		public interface IFakeProperties
		{
			float MinMagnitude { get; set; }
			float MaxMagnitude { get; set; }
			float MagnitudeStrength { get; set; }
			float BasePower { get; set; }
			float Magnitude { get; set; }
			float Power { get; set; }
		}

		public class FakeProperties : IFakeProperties
		{
			public virtual float MinMagnitude { get; set; }
			public virtual float MaxMagnitude { get; set; }
			public virtual float MagnitudeStrength { get; set; }
			public virtual float BasePower { get; set; }
			public virtual float Magnitude { get; set; }
			public virtual float Power { get; set; }

			public FakeProperties RollMagnitudeAndPower(float magnitudePower = 1f, float lukStat = 0f)
			{
				Magnitude = RollMagnitude(magnitudePower, lukStat);

				int iterations = (int)Math.Ceiling(lukStat) / 2;

				// makes you more lucky rolling better magnitudes
				for (int i = 0; i < iterations; i++)
				{
					float rolledMagnitude = RollMagnitude(magnitudePower, lukStat);
					if (rolledMagnitude > Magnitude)
					{
						Magnitude = rolledMagnitude;
					}
				}

				Power = RollPower();

				return this;
			}

			internal float RollMagnitude(float magnitudePower, float lukStat)
			{
				float useMin = MinMagnitude * (0.01f * lukStat);
				float useMax = MaxMagnitude * magnitudePower;
				float randomMag = (useMin + Main.rand.NextFloat() * (useMax - useMin));
				return randomMag * MagnitudeStrength;
			}

			internal float RollPower()
			{
				return BasePower * Magnitude;
			}
		}

		[SetUp]
		public void Setup()
		{
			Main.instance = new Main();
			Main.dedServ = true;
			Main.rand = new UnifiedRandom();
		}

		[Test]
		public void TestMinMaxMagnitudes()
		{
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
		}

		[Test]
		public void TestSaveAndLoad()
		{
			//@todo why did this fail?
			//Assert.AreEqual(new ModifierProperties(), new ModifierProperties());
			var p = new ModifierProperties
			{
				Magnitude = 10f,
				Power = 100f
			};
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
		
		//@todo figure this one out
		[Test]
		public void TestRoll()
		{
			var mock = new Mock<FakeProperties>();
			mock.SetupAllProperties();
			mock.SetupProperty(x => x.Magnitude);
			mock.SetupProperty(x => x.Power);
			mock.SetupSet(x => x.Magnitude = 1f);
			mock.SetupSet(x => x.Power = 2f);

			mock.Object.RollMagnitudeAndPower(10f, 10);

			mock.VerifySet(x => x.Magnitude = It.IsAny<float>(), Times.AtLeastOnce);
			mock.VerifySet(x => x.Power = It.IsAny<float>(), Times.Once);
			mock.Verify(x => x.RollMagnitude(It.IsAny<float>(), It.IsAny<float>()), Times.AtLeastOnce);
			mock.Verify(x => x.RollPower(), Times.Once);

			Assert.AreEqual(mock.Object.Magnitude, 1f);
			Assert.AreEqual(mock.Object.Power, 2f);
		}
	}
}
