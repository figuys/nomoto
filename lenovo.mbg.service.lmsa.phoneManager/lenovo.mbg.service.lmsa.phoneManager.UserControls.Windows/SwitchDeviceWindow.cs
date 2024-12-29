using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;

public partial class SwitchDeviceWindow : Window, IComponentConnector
{
	[DefaultValue(false)]
	public bool IsDisposed { get; private set; }

	public SwitchDeviceWindow()
	{
		InitializeComponent();
		base.DataContext = SwitchDeviceWindowViewModel.Instance;
		base.Closing += delegate
		{
			IsDisposed = true;
		};
	}
}
