using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Tabs.Soulforging
{
	internal class GuiSoulgauge : UIElement
	{
		// TODO generify to animated element?
		private class GaugeDrawing
		{
			public static bool IsLoaded { get; private set; }

			public static void Load()
			{
				if (!IsLoaded)
				{
					STATIC.Load();
					ANIM.Load();
					IsLoaded = true;
				}
			}

			public static void Unload()
			{
				STATIC.Unload();
				ANIM.Unload();
				IsLoaded = false;
			}

			public static class STATIC
			{
				public static Texture2D TEX_EMPTY;
				public static Texture2D TEX_FULL;

				public static void Load()
				{
					TEX_EMPTY = Loot.Instance.GetTexture("UI/Tabs/Soulforging/Soulgauge_0");
					TEX_FULL = Loot.Instance.GetTexture("UI/Tabs/Soulforging/Soulgauge_100");
				}

				public static void Unload()
				{
					TEX_EMPTY = null;
					TEX_FULL = null;
				}
			}

			public static class ANIM
			{
				public const int MAX_FRAME = 4;
				public const int MAX_TICK = 6;
				public static Vector2 FRAME = new Vector2(124, 106);
				public static Texture2D TEX_TWENTY;
				public static Texture2D TEX_FOURTY;
				public static Texture2D TEX_SIXTY;
				public static Texture2D TEX_EIGHTY;

				public static void Load()
				{
					TEX_TWENTY = Loot.Instance.GetTexture("UI/Tabs/Soulforging/Soulgauge_anim_20");
					TEX_FOURTY = Loot.Instance.GetTexture("UI/Tabs/Soulforging/Soulgauge_anim_40");
					TEX_SIXTY = Loot.Instance.GetTexture("UI/Tabs/Soulforging/Soulgauge_anim_60");
					TEX_EIGHTY = Loot.Instance.GetTexture("UI/Tabs/Soulforging/Soulgauge_anim_80");
				}

				public static void Unload()
				{
					TEX_TWENTY = null;
					TEX_FOURTY = null;
					TEX_SIXTY = null;
					TEX_EIGHTY = null;
				}
			}

			public static bool IsAnimation(GaugeLevel level) 
				=> level != GaugeLevel.ZERO && level != GaugeLevel.HUNDRED;

			public static Texture2D GetTextureByLevel(GaugeLevel level)
			{
				switch (level)
				{
					default:
					case GaugeLevel.ZERO:
						return STATIC.TEX_EMPTY;
					case GaugeLevel.HUNDRED:
						return STATIC.TEX_FULL;
					case GaugeLevel.TWENTY:
						return ANIM.TEX_TWENTY;
					case GaugeLevel.FOURTY:
						return ANIM.TEX_FOURTY;
					case GaugeLevel.SIXTY:
						return ANIM.TEX_SIXTY;
					case GaugeLevel.EIGHTY:
						return ANIM.TEX_EIGHTY;
				}
			}

			private static short _tick;
			private static short _frame;
			private static Texture2D _activeTexture;

			public static void Update()
			{
				_tick++;
				if (_tick >= ANIM.MAX_TICK)
				{
					_tick = 0;
					_frame++;
					if (_frame >= ANIM.MAX_FRAME)
					{
						_frame = 0;
					}
				}
			}

			public static Rectangle GetDrawFrame()
				=> new Rectangle(0, _frame * (int)ANIM.FRAME.Y, (int)ANIM.FRAME.X, (int)ANIM.FRAME.Y);
		}

		// TODO trivial asf
		public string GaugeText(GaugeLevel level)
		{
			switch (level)
			{
				default:
				case GaugeLevel.ZERO:
					return "0";
				case GaugeLevel.HUNDRED:
					return "100";
				case GaugeLevel.TWENTY:
					return "20";
				case GaugeLevel.FOURTY:
					return "40";
				case GaugeLevel.SIXTY:
					return "60";
				case GaugeLevel.EIGHTY:
					return "80";
			}
		}

		// TODO this wouldn't be stored here, test for now
		public GaugeLevel GaugeLevel;
		public Texture2D DrawTexture => GaugeDrawing.GetTextureByLevel(GaugeLevel);

		public GuiSoulgauge()
		{
			GaugeDrawing.Load();
			GaugeLevel = GaugeLevel.HUNDRED;
		}

		~GuiSoulgauge()
		{
			GaugeDrawing.Unload();
		}

		public override void OnInitialize()
		{
			base.OnInitialize();
			Width.Set(GaugeDrawing.ANIM.FRAME.X, 0);
			Height.Set(GaugeDrawing.ANIM.FRAME.Y, 0);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (GaugeDrawing.IsLoaded)
			{
				if (IsMouseHovering)
				{
					Main.hoverItemName =
						$"Soul level: {GaugeText(GaugeLevel)}%";
				}

				GaugeDrawing.Update();
				CalculatedStyle dimensions = GetDimensions();

				if (GaugeDrawing.IsAnimation(GaugeLevel))
				{
					spriteBatch.Draw(DrawTexture, dimensions.Position(), GaugeDrawing.GetDrawFrame(), Color.White);
				}
				else
				{
					spriteBatch.Draw(DrawTexture, dimensions.Position(), Color.White);
				}
			}
		}
	}
}
