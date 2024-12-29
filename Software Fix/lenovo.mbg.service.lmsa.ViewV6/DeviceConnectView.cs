using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.ViewModels;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class DeviceConnectView : UserControl, IComponentConnector, IStyleConnector
{
	public DeviceConnectView()
	{
		InitializeComponent();
		base.DataContext = DeviceConnectViewModel.Instance;
	}

	private void OnMouseLeave(object sender, MouseEventArgs e)
	{
		Popup popup = base.Template.FindName("popup", this) as Popup;
		popup.IsOpen = false;
	}

	private void StackPanel_LostFocus(object sender, RoutedEventArgs e)
	{
		Popup popup = base.Template.FindName("popup", this) as Popup;
		popup.IsOpen = false;
	}
}
