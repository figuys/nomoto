using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.UserControls.Tips;

public partial class GrantDebugPermissionTips : UserControl, IComponentConnector
{
	public bool IsManualClose { get; private set; }

	public GrantDebugPermissionTips()
	{
		InitializeComponent();
		IsManualClose = false;
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		IsManualClose = true;
		HostProxy.HostMaskLayerWrapper.Close(this);
	}
}
