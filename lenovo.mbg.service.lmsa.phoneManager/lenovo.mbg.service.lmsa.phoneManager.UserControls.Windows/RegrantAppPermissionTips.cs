using System.Windows;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;

public partial class RegrantAppPermissionTips : Window, IComponentConnector
{
	public RegrantAppPermissionTips()
	{
		InitializeComponent();
	}

	private void btnIsReady_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void btnCancel_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}
}
