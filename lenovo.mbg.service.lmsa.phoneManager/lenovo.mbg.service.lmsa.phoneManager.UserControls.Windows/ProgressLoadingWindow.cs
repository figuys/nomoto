using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls.Windows;

public partial class ProgressLoadingWindow : Window, IComponentConnector
{
	[DefaultValue(false)]
	public bool IsDisposed { get; private set; }

	public ProgressLoadingWindow()
	{
		InitializeComponent();
		base.Closing += delegate
		{
			IsDisposed = true;
		};
	}
}
