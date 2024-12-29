using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.support;

public partial class MainFrame : UserControl, IComponentConnector, IStyleConnector
{
	public MainFrame()
	{
		InitializeComponent();
	}

	private void btn_back_Click(object sender, RoutedEventArgs e)
	{
		(sender as Button).Visibility = Visibility.Collapsed;
	}
}
