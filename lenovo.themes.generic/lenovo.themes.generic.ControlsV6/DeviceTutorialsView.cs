using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ControlsV6;

public partial class DeviceTutorialsView : UserControl, IComponentConnector
{
	public DeviceTutorialsView()
	{
		InitializeComponent();
		base.Loaded += DeviceTutorialsView_Loaded;
	}

	private void DeviceTutorialsView_Loaded(object sender, RoutedEventArgs e)
	{
		DeviceTutorialsViewModel deviceTutorialsViewModel2 = (DeviceTutorialsViewModel)(base.DataContext = new DeviceTutorialsViewModel());
		deviceTutorialsViewModel2.LoadData(null);
	}
}
