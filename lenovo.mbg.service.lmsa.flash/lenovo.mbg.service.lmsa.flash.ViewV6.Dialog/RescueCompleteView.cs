using System;
using System.Collections.Generic;
using System.Web;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class RescueCompleteView : Window, IUserMsgControl, IComponentConnector
{
	private string brand;

	private DevCategory category;

	private string modelName;

	private long flashId;

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public RescueCompleteView(string modelName, string brand, DevCategory category, long flashId, bool isEraseFailed)
	{
		InitializeComponent();
		this.brand = char.ToUpper(brand[0]) + brand.Substring(1);
		this.category = category;
		this.modelName = modelName;
		this.flashId = flashId;
		base.Owner = Application.Current.MainWindow;
		panelNotify.Visibility = ((!isEraseFailed) ? Visibility.Collapsed : Visibility.Visible);
	}

	private void OnBtnFacebook(object sender, RoutedEventArgs e)
	{
		string text = "https://motorola-global-portal.custhelp.com/app/answers/detail/a_id/158726";
		string text2 = "https://lenovomobilesupport.lenovo.com/us/en/downloads/ds101291";
		string text3 = HttpUtility.UrlEncode("RescueByRSA");
		string text4 = HttpUtility.UrlEncode("https://www.facebook.com/");
		string text5 = HttpUtility.UrlEncode((category == DevCategory.Phone) ? text : text2);
		GlobalFun.OpenUrlByBrowser(string.Format("https://www.facebook.com/dialog/share?app_id=184484190795&display=page&e2e=&hashtag=%23{0}&href={1}&locale={2}&quote={3}&sdk=joey&redirect_uri={4}", text3, text5, LMSAContext.CurrentLanguage, "", text4));
		UploadData(1, isOk: true);
	}

	private void OnBtnTwitter(object sender, RoutedEventArgs e)
	{
		string text = "phone";
		if (category == DevCategory.Tablet)
		{
			text = "tablet";
		}
		else if (category == DevCategory.Smart)
		{
			text = "smart-device";
		}
		string arg = brand + " " + text;
		string arg2 = HttpUtility.UrlEncode(string.Format(HostProxy.LanguageService.Translate("K1177"), arg));
		string text2 = "https://www.motorola.com/us/rescue-and-smart-assistant/p";
		string text3 = "https://lenovomobilesupport.lenovo.com/us/en/downloads/ds101291";
		string arg3 = HttpUtility.UrlEncode((category == DevCategory.Phone) ? text2 : text3);
		GlobalFun.OpenUrlByBrowser($"https://twitter.com/intent/tweet?text={arg2}&url={arg3}");
		UploadData(2, isOk: true);
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
		Result = true;
		CloseAction?.Invoke(true);
	}

	public static void ShowDialogEx(string modelName, string information, DevCategory category, long flashId, bool isEraseFailed = false)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			RescueCompleteView userUi = new RescueCompleteView(modelName, information, category, flashId, isEraseFailed);
			MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
		});
	}

	private void UploadData(int platform, bool isOk)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("modelName", modelName);
		dictionary.Add("flashId", flashId);
		dictionary.Add("socialPlatform", platform);
		dictionary.Add("shareResult", isOk ? 1 : 0);
		FlashContext.SingleInstance.service.RequestContent(WebServicesContext.COLLECTION_SHARE_RESCUE_RESULT, dictionary);
	}

	private void OnBtnSubmit(object sender, RoutedEventArgs e)
	{
	}

	public Window GetMsgUi()
	{
		return this;
	}
}
