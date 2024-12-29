using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.lang;

namespace lenovo.themes.generic.ControlsV6;

public partial class PicVideoAuthorizeViewV6 : Window, IComponentConnector
{
	public PicVideoAuthorizeViewV6()
	{
		InitializeComponent();
		string text = LangTranslation.Translate("K0746");
		string text2 = string.Format(LangTranslation.Translate("K1110"), text);
		txtMsgTitile.Text = text2;
		txtMsgTitile.TextEffects.Add(new TextEffect
		{
			Foreground = new SolidColorBrush(Color.FromRgb(67, 181, 226)),
			PositionStart = text2.IndexOf(text),
			PositionCount = text.ToString().Length
		});
	}

	private void OnBtnClick(object sender, RoutedEventArgs e)
	{
		Button button = sender as Button;
		base.DialogResult = Convert.ToBoolean(button.Tag);
	}
}
