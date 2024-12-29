using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class RescueFailedView : UserControl, IComponentConnector
{
	public RescueFailedView(string message, bool isNormalNote, string image = null)
	{
		InitializeComponent();
		txtContent.LangKey = message;
		if (string.IsNullOrEmpty(image))
		{
			bd.Visibility = Visibility.Collapsed;
		}
		else
		{
			picture.Source = Application.Current.Resources[image] as ImageSource;
		}
		if (isNormalNote)
		{
			SetNormalText();
		}
		else
		{
			SetEffectsText();
		}
	}

	public void SetEffectsText()
	{
		string text = LangTranslation.Translate("K1200");
		string text2 = LangTranslation.Translate("K1201");
		string text3 = string.Format(LangTranslation.Translate("K1199"), text, text2);
		txtNote.Text = text3;
		txtNote.TextEffects.Add(new TextEffect
		{
			Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2")),
			PositionStart = text3.IndexOf(text),
			PositionCount = text.ToString().Length
		});
		txtNote.TextEffects.Add(new TextEffect
		{
			Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2")),
			PositionStart = text3.IndexOf(text2),
			PositionCount = text2.ToString().Length
		});
	}

	public void SetNormalText()
	{
		txtNote.Text = HostProxy.LanguageService.Translate("K1241");
	}
}
