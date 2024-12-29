using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.backuprestore.View;

public partial class BackUpHelpGuideView : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public BackUpHelpGuideView()
	{
		InitializeComponent();
		GenerateQrCode();
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void BtnCloseClick(object sender, RoutedEventArgs e)
	{
		Result = false;
		Close();
		CloseAction?.Invoke(false);
	}

	private void GenerateQrCode()
	{
		MemoryStream memoryStream = QrCodeUtility.GenerateQrCodeImageStream(HostProxy.LanguageService.IsChinaRegionAndLanguage() ? $"https://download.lenovo.com/lsa/Resource/MA/ma{Configurations.AppVersionCode}.apk" : $"https://rasa.page.link/ma{Configurations.AppVersionCode}");
		BitmapImage bitmapImage = new BitmapImage();
		bitmapImage.BeginInit();
		bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
		bitmapImage.EndInit();
		try
		{
			imgDownQR.Source = bitmapImage;
		}
		catch (Exception)
		{
		}
	}
}
