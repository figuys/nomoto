using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using GoogleAnalytics;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.ViewModels.SystemOperation;

public class AboutOperationItemViewModel : MouseOverMenuItemViewModel
{
	private Dictionary<string, string> LanguageURLMap = new Dictionary<string, string>
	{
		{ "en-US", "r-en" },
		{ "es-ES", "es" },
		{ "ja-JP", "ja" },
		{ "pl-PL", "pl" },
		{ "pt-BR", "pt-br" },
		{ "ru-RU", "ru" },
		{ "it-IT", "it" },
		{ "zh-CN", "zh-cn" },
		{ "de-DE", "de" },
		{ "sk-SK", "sk" },
		{ "sr-RS", "sr" },
		{ "ro-RO", "ro" },
		{ "bg-BG", "bg" },
		{ "cs-CZ", "cz" },
		{ "fr-FR", "fr" },
		{ "hi-IN", "r-en" },
		{ "id-ID", "r-en" }
	};

	public AboutOperationItemViewModel()
	{
		base.Icon = Application.Current.FindResource("aboutDrawingImage") as ImageSource;
		base.MouseOverIcon = Application.Current.FindResource("about_clickDrawingImage") as ImageSource;
		base.Header = "K0283";
	}

	public override void ClickCommandHandler(object args)
	{
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateCustomEvent(App.Category, "MenuAboutButtonClick", "menu-about button click", 0L).Build());
		string format = "https://download.lenovo.com/lenovo/lla/l505-0009-06-{0}.pdf";
		string currentLanguage = HostProxy.LanguageService.GetCurrentLanguage();
		GlobalFun.OpenUrlByBrowser(string.Format(format, LanguageURLMap[currentLanguage]));
	}
}
