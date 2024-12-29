using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class RescuingFailViewV6 : UserControl, IComponentConnector
{
	protected int operatorType;

	public Action<int> ClickAction { get; set; }

	public RescuingFailViewV6()
	{
		InitializeComponent();
	}

	public void Init(FlashStatusV6 status, string modelName, string imei, string message, bool? isNormalNotice, bool isRetry = false, DevCategory category = DevCategory.Phone)
	{
		btnCancel.Visibility = Visibility.Collapsed;
		if (Plugin.SupportMulti)
		{
			modelnameimeistp.Visibility = Visibility.Visible;
			modelnamectrl.Text = modelName;
			if (!string.IsNullOrEmpty(imei))
			{
				imeistap.Visibility = Visibility.Visible;
				imeictrl.Text = imei;
			}
		}
		if (category == DevCategory.Phone)
		{
			if (isRetry)
			{
				btnOk.Content = "K0636";
				rightPanel.Visibility = Visibility.Visible;
			}
			else
			{
				btnOk.Content = "K0327";
				rightPanel.Visibility = Visibility.Collapsed;
			}
		}
		else
		{
			btnOk.Content = (isRetry ? "K0636" : "K0327");
			rightPanel.Visibility = Visibility.Collapsed;
		}
		operatorType = (isRetry ? 1 : 0);
		txtMessage.LangKey = message;
		string soreceLanguage = "K1241";
		if (status == FlashStatusV6.FAIL)
		{
			titleMessage.LangKey = "K0391";
			img.Source = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/failed.png"));
		}
		else
		{
			isNormalNotice = true;
			warn.Visibility = (string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible);
			titleMessage.LangKey = "K1652";
			soreceLanguage = "K1657";
			img.Source = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/quit.png"));
		}
		if (!isNormalNotice.HasValue)
		{
			txtNote.Visibility = Visibility.Collapsed;
		}
		else if (isNormalNotice == true)
		{
			txtNote.Text = HostProxy.LanguageService.Translate(soreceLanguage);
		}
	}

	private void OkClick(object sender, RoutedEventArgs e)
	{
		ClickAction(operatorType);
	}

	private void CancelClick(object sender, RoutedEventArgs e)
	{
		ClickAction(2);
	}
}
