using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.hardwaretest.ViewModel;

namespace lenovo.mbg.service.lmsa.hardwaretest.View;

public partial class MainFrame : UserControl, IComponentConnector
{
	public MainFrame()
	{
		InitializeComponent();
		base.Unloaded += MainFrame_Unloaded;
		base.Loaded += MainFrame_Loaded;
	}

	private void MainFrame_Loaded(object sender, RoutedEventArgs e)
	{
		if (Context.CurrentViewType == ViewType.MAIN)
		{
			MainViewModel mainViewModel = Context.FindViewModel<MainViewModel>(typeof(MainView));
			if (mainViewModel.Started)
			{
				mainViewModel.StartLoop();
			}
		}
	}

	private void MainFrame_Unloaded(object sender, RoutedEventArgs e)
	{
		if (Context.CurrentViewType == ViewType.MAIN)
		{
			MainViewModel mainViewModel = Context.FindViewModel<MainViewModel>(typeof(MainView));
			if (mainViewModel.Started)
			{
				mainViewModel.StopLoop();
			}
		}
	}
}
