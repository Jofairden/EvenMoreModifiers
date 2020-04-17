using Loot.Ext;
using Loot.UI.Common.Controls.Button;

namespace Loot.UI.Tabs.Soulforging
{
	internal class GuiSoulforgeTab : GuiTab
	{
		public override string Header => "Soulforging";
		public override int GetPageHeight()
		{
			return 400;
		}

		private GuiSoulgauge _soulgauge;

		public override void OnInitialize()
		{
			base.OnInitialize();

			_soulgauge = new GuiSoulgauge();
			_soulgauge.OnInitialize();
			_soulgauge.Left.Set(TabFrame.Width.Pixels * 0.5f - _soulgauge.Width.Pixels * 0.5f, 0);
			TabFrame.Append(_soulgauge);
		}
	}
}
