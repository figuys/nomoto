using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.Model;

namespace lenovo.mbg.service.lmsa.toolbox.UserControlsV6;

public partial class GifProgressViewV6 : Window, IUserMsgControl, IComponentConnector
{
	public EventHandler CancelHandler;

	public ProgressModel Model { get; set; }

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public GifProgressViewV6()
	{
		InitializeComponent();
		base.ShowInTaskbar = false;
		base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		Model = new ProgressModel
		{
			Percentage = 0.0,
			CloseCmd = new RoutedCommand(),
			Information = "K0226"
		};
		base.DataContext = Model;
		base.Closed += delegate
		{
			Result = false;
			CloseAction?.Invoke(false);
		};
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		CancelHandler(null, null);
		Result = false;
		CloseAction?.Invoke(false);
		Close();
	}
}
