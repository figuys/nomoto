using System;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;

namespace lenovo.mbg.service.lmsa.OrderView;

public partial class OrderRefundView : Window, IComponentConnector
{
	private string _Id;

	public OrderRefundView(string orderId)
	{
		_Id = orderId;
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void OnBtnRefund(object sender, RoutedEventArgs e)
	{
		string id = AppContext.WebApi.RequestBase(WebApiUrl.CALL_B2B_GET_ORDERID_URL, new
		{
			macAddressRsa = RsaHelper.RSAEncrypt(WebApiContext.RSA_PUBLIC_KEY, GlobalFun.GetMacAddr()),
			orderId = _Id
		}).content?.ToString();
		Close();
		if (!string.IsNullOrEmpty(id))
		{
			Application.Current.Dispatcher.BeginInvoke((Action)delegate
			{
				OrderRefundIDView orderRefundIDView = new OrderRefundIDView(id);
				orderRefundIDView.ShowDialog();
			});
		}
	}
}
