using Loot.Attributes;
using Loot.UI.Common;
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
			public const int MAX_FRAME = 4;
			public const int MAX_TICK = 6;
			public static Vector2 FRAME = new Vector2(124, 106);

			public static bool IsAnimation(GaugeLevel level)
				=> level != GaugeLevel.ZERO && level != GaugeLevel.HUNDRED;

			public static Texture2D GetTextureByLevel(GaugeLevel level)
			{
				switch (level)
				{
					default:
					case GaugeLevel.ZERO:
						return Assets.Textures.Soulgauge.EmptyTexture;
					case GaugeLevel.HUNDRED:
						return Assets.Textures.Soulgauge.FullTexture;
					case GaugeLevel.TWENTY:
						return Assets.Textures.Soulgauge.Anim20;
					case GaugeLevel.FOURTY:
						return Assets.Textures.Soulgauge.Anim40;
					case GaugeLevel.SIXTY:
						return Assets.Textures.Soulgauge.Anim60;
					case GaugeLevel.EIGHTY:
						return Assets.Textures.Soulgauge.Anim80;
				}
			}

			private static short _tick;
			private static short _frame;
			private static Texture2D _activeTexture;

			public static void Update()
			{
				_tick++;
				if (_tick >= MAX_TICK)
				{
					_tick = 0;
					_frame++;
					if (_frame >= MAX_FRAME)
					{
						_frame = 0;
					}
				}
			}

			public static Rectangle GetDrawFrame()
				=> new Rectangle(0, _frame * (int)FRAME.Y, (int)FRAME.X, (int)FRAME.Y);
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
		public GaugeLevel GaugeLevel = GaugeLevel.HUNDRED;
		public Texture2D DrawTexture => GaugeDrawing.GetTextureByLevel(GaugeLevel);

		public override void OnInitialize()
		{
			base.OnInitialize();
			Width.Set(GaugeDrawing.FRAME.X, 0);
			Height.Set(GaugeDrawing.FRAME.Y, 0);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
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
