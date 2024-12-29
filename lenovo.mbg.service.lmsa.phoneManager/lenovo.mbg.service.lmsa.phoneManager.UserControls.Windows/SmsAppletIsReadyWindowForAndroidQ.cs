using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;

public partial class SmsAppletIsReadyWindowForAndroidQ : Window, IComponentConnector
{
	public TcpAndroidDevice Device { get; set; }

	public SmsAppletIsReadyWindowForAndroidQ(TcpAndroidDevice device)
	{
		Device = device;
		InitializeComponent();
	}

	private void btnIsReady_Click(object sender, RoutedEventArgs e)
	{
		if (new DeviceSmsManagement().SmsAppletIsReady(Device))
		{
			Close();
		}
	}

	private void btnCancel_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}
}
