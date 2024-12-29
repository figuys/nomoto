using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.themes.generic;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class RescueSuccessSubmitView : UserControl, IMessageViewV6, IComponentConnector
{
	private readonly ResuceSuccessSubmitViewModel _VM;

	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public RescueSuccessSubmitView(string modelName)
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
		_VM = new ResuceSuccessSubmitViewModel(modelName);
		base.DataContext = _VM;
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		GlobalFun.OpenUrlByBrowser("www.lenovo.com/privacy/");
	}

	private void OnBtnSubmit(object sender, RoutedEventArgs e)
	{
		_VM.Submit().ContinueWith(delegate(Task<bool?> task)
		{
			if (task.Result.HasValue)
			{
				FireClose(true == task.Result);
			}
		});
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		_VM.ReleaseLocker();
		FireClose(null);
	}

	private void FireClose(bool? result)
	{
		Result = result;
		WaitHandler.Set();
		CloseAction?.Invoke(result);
	}
}
