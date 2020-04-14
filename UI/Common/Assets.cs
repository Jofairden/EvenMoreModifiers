using Loot.Api.Attributes;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.UI.Common
{
	internal static class Assets
	{
		internal static class Textures
		{
			[StaticAsset("Placeholder")]
			public static Texture2D PlaceholderTexture;

			internal static class GUI
			{
				[StaticAsset("UI/GuiCloseButton")]
				public static Texture2D CloseButtonTexture;
				[StaticAsset("UI/GuiCloseButtonChains")]
				public static Texture2D CloseButtonChainsTexture;

				[StaticAsset("UI/GuiHeader")]
				public static Texture2D HeaderTexture;
				[StaticAsset("UI/HeaderSkull")]
				public static Texture2D SkullDecorationTexture;

				[StaticAsset("UI/GuiPanelTop")]
				public static Texture2D PanelTopTexture;
				[StaticAsset("UI/GuiPanelTile")]
				public static Texture2D PanelTileTexture;
				[StaticAsset("UI/GuiPanelBottom")]
				public static Texture2D PanelBottomTexture;

				[StaticAsset("UI/Common/Controls/Button/GuiArrowButton")]
				public static Texture2D ArrowButtonTexture;

				[StaticAsset("UI/Common/Controls/Button/GuiButton")]
				public static Texture2D ButtonTexture;

				[StaticAsset("UI/Common/Controls/Panel/GuiPanel")]
				public static Texture2D PanelTexture;

				[StaticAsset("Tiles/LunarCube")]
				public static Texture2D LunarCubeTexture;


				internal static class Tabs
				{
					[StaticAsset("UI/GuiTabCubing")]
					public static Texture2D CubingTabTexture;
					[StaticAsset("UI/GuiTabEssencecraft")]
					public static Texture2D EssenceCraftTexture;
					[StaticAsset("UI/GuiTabSoulforge")]
					public static Texture2D SoulforgeTexture;
				}
			}

			internal static class Soulgauge
			{
				[StaticAsset("UI/Tabs/Soulforging/Soulgauge_0")]
				public static Texture2D EmptyTexture;
				[StaticAsset("UI/Tabs/Soulforging/Soulgauge_100")]
				public static Texture2D FullTexture;

				[StaticAsset("UI/Tabs/Soulforging/Soulgauge_anim_20")]
				public static Texture2D Anim20;
				[StaticAsset("UI/Tabs/Soulforging/Soulgauge_anim_40")]
				public static Texture2D Anim40;
				[StaticAsset("UI/Tabs/Soulforging/Soulgauge_anim_60")]
				public static Texture2D Anim60;
				[StaticAsset("UI/Tabs/Soulforging/Soulgauge_anim_80")]
				public static Texture2D Anim80;
			}
		}
	}
}
