using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.phoneManager.Business.Apps;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;

public partial class CheckPermissionToGetAppInfoWindow : Window, IComponentConnector
{
	public bool PermissionApply { get; private set; }

	public CheckPermissionToGetAppInfoWindow()
	{
		InitializeComponent();
	}

	private void btnIsReady_Click(object sender, RoutedEventArgs e)
	{
		PermissionApply = AppsDeviceAppManager.Instance.CheckPermissionToGetAppInfo();
		if (PermissionApply)
		{
			Close();
		}
	}

	private void btnCancel_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}
}
