using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.messenger;

public partial class MainWindow : Window, IComponentConnector
{
	public MainWindow()
	{
		InitializeComponent();
		Task.Run(delegate
		{
			DotNetBrowserHelper.Instance.LoadUrl(browserView, "https://www.messenger.com/t/motome2017", null);
		});
	}
}
