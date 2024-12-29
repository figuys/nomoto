using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa;

public partial class WindowUpdateTip : Window, IComponentConnector
{
	public WindowUpdateTip()
	{
		InitializeComponent();
		base.Loaded += WindowUpdateTip_Loaded;
	}

	private void WindowUpdateTip_Loaded(object sender, RoutedEventArgs e)
	{
		InitialResource();
	}

	private void InitialResource()
	{
		if (GetTemplateChild("MainGrid") is Grid grid)
		{
			grid.MouseDown += gridMain_MouseDown;
		}
	}

	private void gridMain_MouseDown(object sender, MouseButtonEventArgs e)
	{
	}
}
