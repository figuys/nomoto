using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.framework.devicemgt;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;

public partial class ResetToDefaultSmsAppletTipsForAndroidQ : Window, IComponentConnector
{
	private volatile bool isTimeout;

	public TcpAndroidDevice Device { get; set; }

	public ResetToDefaultSmsAppletTipsForAndroidQ(TcpAndroidDevice device)
	{
		Device = device;
		InitializeComponent();
		Task.Run(delegate
		{
			int num = 0;
			do
			{
				Thread.Sleep(1000);
			}
			while (++num < 15);
			isTimeout = true;
		});
	}

	private void btnIsReady_Click(object sender, RoutedEventArgs e)
	{
		if (isTimeout || !Device.SoftStatusIsLocked)
		{
			Close();
		}
	}

	private void btnCancel_Click(object sender, RoutedEventArgs e)
	{
		if (isTimeout || !Device.SoftStatusIsLocked)
		{
			Close();
		}
	}
}
