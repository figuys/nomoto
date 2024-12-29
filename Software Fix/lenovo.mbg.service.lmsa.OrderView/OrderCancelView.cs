using System.Windows;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.OrderView;

public partial class OrderCancelView : Window, IComponentConnector
{
	public OrderCancelView()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
	}
}
