using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.UserControls.Tips;

public partial class DoNotEnableDebugTips : UserControl, IComponentConnector
{
	private ReplayCommand closeCommand;

	private bool canClose = false;

	public ReplayCommand CloseCommand
	{
		get
		{
			return closeCommand;
		}
		set
		{
			if (closeCommand != value)
			{
				closeCommand = value;
			}
		}
	}

	public bool CanClose
	{
		get
		{
			return canClose;
		}
		set
		{
			canClose = value;
		}
	}

	public DoNotEnableDebugTips()
	{
		InitializeComponent();
		CloseCommand = new ReplayCommand(CloseCommandHandler);
		base.DataContext = this;
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		CanClose = true;
		Window.GetWindow(sender as Button)?.Close();
	}

	private void CloseCommandHandler(object args)
	{
		CanClose = true;
		if (args is Window window)
		{
			window.Close();
		}
	}
}
