using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.lmsa.Login.Business;

namespace lenovo.mbg.service.lmsa.OrderView;

public partial class B2BPricePanel : UserControl, IComponentConnector
{
	private float SINGLE_DEVICE_PRICE = 1.99f;

	public Action OnBuyAction { get; set; }

	public int SingleDeviceCount { get; private set; }

	public B2BPricePanel()
	{
		InitializeComponent();
		SingleDeviceCount = 1;
		txtUnit.Text = UserService.Single.CurrentLoggedInUser.PriceUnit;
		txtPrice1.Text = $"{UserService.Single.CurrentLoggedInUser.PriceUnit}{UserService.Single.CurrentLoggedInUser.Price1}";
		txtPrice2.Text = $"{UserService.Single.CurrentLoggedInUser.PriceUnit}{UserService.Single.CurrentLoggedInUser.Price2}";
		txtPrice3.Text = $"{UserService.Single.CurrentLoggedInUser.PriceUnit}{UserService.Single.CurrentLoggedInUser.Price3}";
		txtSave.Text = $"{UserService.Single.CurrentLoggedInUser.PriceUnit} {10f * UserService.Single.CurrentLoggedInUser.Price1 - UserService.Single.CurrentLoggedInUser.Price2:F2}";
		SINGLE_DEVICE_PRICE = UserService.Single.CurrentLoggedInUser.Price1;
		txtSinglePriceTotal2.Text = $"{UserService.Single.CurrentLoggedInUser.Price1}";
		btnSingleDevice.Tag = UserService.Single.CurrentLoggedInUser.SkuName1;
		btnTenDevices.Tag = UserService.Single.CurrentLoggedInUser.SkuName2;
		btnMonthlyFee.Tag = UserService.Single.CurrentLoggedInUser.SkuName3;
	}

	private void ImageSubstract_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		SingleDeviceCount--;
		CalculateTotalPrice();
		if (SingleDeviceCount == 1)
		{
			imgSubstract.IsEnabled = false;
		}
	}

	private void ImageAdd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		SingleDeviceCount++;
		CalculateTotalPrice();
		imgSubstract.IsEnabled = true;
	}

	private void CalculateTotalPrice()
	{
		txtDeviceCount.Text = SingleDeviceCount.ToString();
		float num = SINGLE_DEVICE_PRICE * (float)SingleDeviceCount;
		txtSinglePriceTotal2.Text = num.ToString();
	}

	private void OnBtnBuy(object sender, RoutedEventArgs e)
	{
		Button button = sender as Button;
		object tag = button.Tag;
		int quantity = ((!(button.Name == btnSingleDevice.Name)) ? 1 : SingleDeviceCount);
		object obj = AppContext.WebApi.RequestContent(WebApiUrl.CALL_B2B_ORDER_BUY_URL, new
		{
			macAddressRsa = RsaHelper.RSAEncrypt(WebApiContext.RSA_PUBLIC_KEY, GlobalFun.GetMacAddr()),
			sku = tag,
			quantity = quantity
		});
		if (obj != null)
		{
			GlobalFun.OpenUrlByBrowser(obj.ToString());
			OnBuyAction?.Invoke();
		}
	}
}
