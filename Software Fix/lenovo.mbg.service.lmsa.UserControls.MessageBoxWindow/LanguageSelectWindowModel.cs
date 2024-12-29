using System.Windows.Controls;
using System.Windows.Media;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.lmsa.ViewModels;
using lenovo.themes.generic;
using lenovo.themes.generic.Controls;

namespace lenovo.mbg.service.lmsa.UserControls.MessageBoxWindow;

public class LanguageSelectWindowModel : OkCancelWindowModel
{
	public LanguageSelectWindowModel(int button)
	{
		base.ViewModel = LanguageSelectViewModel.DefaultValues();
		base.ContentControlTemplate = button switch
		{
			1 => ComponentResources.SingleInstance.GetResource("restartNowOrLaterWindowTemplate") as ControlTemplate, 
			2 => ComponentResources.SingleInstance.GetResource("defaultOKCancelWindowTemplate") as ControlTemplate, 
			_ => ComponentResources.SingleInstance.GetResource("languageSelectWindowTemplate") as ControlTemplate, 
		};
	}

	public LenovoPopupWindow CreateWindow(string title, string content, ImageSource image)
	{
		LanguageSelectViewModel languageSelectViewModel = base.ViewModel as LanguageSelectViewModel;
		languageSelectViewModel.Title = LangTranslation.Translate(title);
		languageSelectViewModel.Content = LangTranslation.Translate(content);
		languageSelectViewModel.CancelButtonText = "K0208";
		languageSelectViewModel.OKButtonText = "K0327";
		languageSelectViewModel.RestartNowButtonText = "K0301";
		languageSelectViewModel.RestartLaterButtonText = "K0300";
		languageSelectViewModel.Language1Checked = languageSelectViewModel.LanguageSelected == "en-US";
		languageSelectViewModel.Language8Checked = languageSelectViewModel.LanguageSelected == "zh-CN";
		languageSelectViewModel.Language2Checked = languageSelectViewModel.LanguageSelected == "pt-BR";
		languageSelectViewModel.Language3Checked = languageSelectViewModel.LanguageSelected == "es-ES";
		languageSelectViewModel.Language4Checked = languageSelectViewModel.LanguageSelected == "pl-PL";
		languageSelectViewModel.Language5Checked = languageSelectViewModel.LanguageSelected == "ru-RU";
		languageSelectViewModel.Language6Checked = languageSelectViewModel.LanguageSelected == "ja-JP";
		languageSelectViewModel.Language7Checked = languageSelectViewModel.LanguageSelected == "it-IT";
		languageSelectViewModel.Language9Checked = languageSelectViewModel.LanguageSelected == "de-DE";
		languageSelectViewModel.Language10Checked = languageSelectViewModel.LanguageSelected == "sk-SK";
		languageSelectViewModel.Language11Checked = languageSelectViewModel.LanguageSelected == "sr-RS";
		languageSelectViewModel.Language12Checked = languageSelectViewModel.LanguageSelected == "ro-RO";
		languageSelectViewModel.Language13Checked = languageSelectViewModel.LanguageSelected == "bg-BG";
		languageSelectViewModel.Language14Checked = languageSelectViewModel.LanguageSelected == "cs-CZ";
		languageSelectViewModel.Language15Checked = languageSelectViewModel.LanguageSelected == "fr-FR";
		languageSelectViewModel.ContentIconImageSource = image;
		return CreateWindow();
	}
}
