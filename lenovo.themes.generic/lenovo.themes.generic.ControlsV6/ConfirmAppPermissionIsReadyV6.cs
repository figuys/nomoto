using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.themes.generic.ControlsV6;

public partial class ConfirmAppPermissionIsReadyV6 : UserControl, IComponentConnector
{
	public ConfirmAppPermissionIsReadyV6()
	{
		InitializeComponent();
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		Window.GetWindow(sender as Button)?.Close();
	}
}
