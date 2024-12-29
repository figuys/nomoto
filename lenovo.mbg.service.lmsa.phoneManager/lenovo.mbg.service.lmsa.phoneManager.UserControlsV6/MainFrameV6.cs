using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class MainFrameV6 : UserControl, IComponentConnector
{
	public MainFrameV6()
	{
		InitializeComponent();
		base.Loaded += delegate
		{
			Context.IsPluginActived = true;
		};
		base.Unloaded += delegate
		{
			Context.IsPluginActived = false;
		};
	}
}
