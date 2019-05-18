using Loot.Api.Ext;
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

			var btn0 = new GuiButton(GuiButton.ButtonType.Parchment);
			btn0.OnClick += delegate { _soulgauge.GaugeLevel = GaugeLevel.ZERO; };
			TabFrame.Append(btn0.Below(_soulgauge));

			var btn1 = new GuiButton(GuiButton.ButtonType.Parchment);
			btn1.OnClick += delegate { _soulgauge.GaugeLevel = GaugeLevel.TWENTY; };
			TabFrame.Append(btn1.Below(btn0));

			var btn2 = new GuiButton(GuiButton.ButtonType.Parchment);
			btn2.OnClick += delegate { _soulgauge.GaugeLevel = GaugeLevel.FOURTY; };
			TabFrame.Append(btn2.Below(btn1));

			var btn3 = new GuiButton(GuiButton.ButtonType.Parchment);
			btn3.OnClick += delegate { _soulgauge.GaugeLevel = GaugeLevel.SIXTY; };
			TabFrame.Append(btn3.Below(btn2));

			var btn4 = new GuiButton(GuiButton.ButtonType.Parchment);
			btn4.OnClick += delegate { _soulgauge.GaugeLevel = GaugeLevel.EIGHTY; };
			TabFrame.Append(btn4.Below(btn3));

			var btn5 = new GuiButton(GuiButton.ButtonType.Parchment);
			btn5.OnClick += delegate { _soulgauge.GaugeLevel = GaugeLevel.HUNDRED; };
			TabFrame.Append(btn5.Below(btn4));
		}
	}
}
