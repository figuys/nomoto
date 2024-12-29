using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class DeviceInfoView : UserControl, IComponentConnector
{
	public DeviceInfoView()
	{
		InitializeComponent();
		base.DataContext = new DeviceInfoViewModel();
	}
}
