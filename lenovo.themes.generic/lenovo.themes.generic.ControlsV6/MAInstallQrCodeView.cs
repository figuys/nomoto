using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;

namespace lenovo.themes.generic.ControlsV6;

public partial class MAInstallQrCodeView : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public MAInstallQrCodeView()
	{
		InitializeComponent();
		GenerateQrCode();
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
		Result = false;
		CloseAction?.Invoke(false);
	}

	private void GenerateQrCode()
	{
		try
		{
			MemoryStream memoryStream = QrCodeUtility.GenerateQrCodeImageStream(global::Smart.LanguageService.IsChinaRegionAndLanguage() ? $"https://download.lenovo.com/lsa/Resource/MA/ma{Configurations.AppVersionCode}.apk" : $"https://rasa.page.link/ma{Configurations.AppVersionCode}");
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
			bitmapImage.EndInit();
			qrCode.Source = bitmapImage;
		}
		catch (Exception)
		{
		}
	}
}
