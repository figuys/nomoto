using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.OrderView;

public partial class OrderRefundIDView : Window, IComponentConnector
{
	public OrderRefundIDView(string id)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		txtID.Text = id ?? string.Empty;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void OnLeftClickUrl(object sender, MouseButtonEventArgs e)
	{
		string text = (sender as TextBlock).Text;
		GlobalFun.OpenUrlByBrowser(text);
	}
}
