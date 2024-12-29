using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.UserControls.Tips;

public partial class ReconnectTips : UserControl, IComponentConnector
{
	public ReconnectTips()
	{
		InitializeComponent();
	}

	private void btnOk_Click(object sender, RoutedEventArgs e)
	{
		CloseWindow(sender as Button);
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		CloseWindow(sender as Button);
	}

	private void CloseWindow(DependencyObject obj)
	{
		Window.GetWindow(obj)?.Close();
	}
}
