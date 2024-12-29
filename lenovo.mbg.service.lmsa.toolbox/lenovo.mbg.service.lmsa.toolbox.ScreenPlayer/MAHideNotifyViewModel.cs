using System;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;

public class MAHideNotifyViewModel : NotifyBase
{
	public bool IsOk { get; private set; }

	public ReplayCommand NextCommand { get; set; }

	public ReplayCommand CloseCommand { get; set; }

	public MAHideNotifyViewModel(Window wnd)
	{
		MAHideNotifyViewModel mAHideNotifyViewModel = this;
		CloseCommand = new ReplayCommand(delegate(object param)
		{
			mAHideNotifyViewModel.IsOk = Convert.ToBoolean(param as string);
			wnd.Close();
		});
	}
}
