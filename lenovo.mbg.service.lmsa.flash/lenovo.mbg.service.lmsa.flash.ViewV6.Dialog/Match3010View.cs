using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class Match3010View : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public Match3010View(DevCategory category, string modelName, string number, object wModel)
	{
		InitializeComponent();
		WarrantyInfoBaseModel warrantyInfoBaseModel = wModel as WarrantyInfoBaseModel;
		warranty.DataContext = new WarrantyInfoViewModelV6(warrantyInfoBaseModel);
		switch (category)
		{
		case DevCategory.Tablet:
			image.Source = Application.Current.Resources["v6_warranty_tabletnew"] as ImageSource;
			break;
		case DevCategory.Smart:
			image.Source = Application.Current.Resources["v6_warranty_smartnew"] as ImageSource;
			break;
		default:
			image.Source = Application.Current.Resources["v6_warranty_phonenew"] as ImageSource;
			break;
		}
		string soreceLanguage = ((category == DevCategory.Phone) ? "K0079" : "K0082");
		txtLeft.Text = HostProxy.LanguageService.Translate("K0087") + ": " + modelName;
		txtRight.Text = HostProxy.LanguageService.Translate(soreceLanguage) + ": " + number;
		if (!string.IsNullOrEmpty(warrantyInfoBaseModel?.ShipLocation))
		{
			TextBlock textBlock = txtLeft;
			textBlock.Text = textBlock.Text + "\n" + HostProxy.LanguageService.Translate("K0270") + ": " + warrantyInfoBaseModel.ShipLocation;
		}
		btnok.IsEnabled = true;
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnBtnClick(object sender, RoutedEventArgs e)
	{
		Close();
		CloseAction?.Invoke(true);
	}
}
