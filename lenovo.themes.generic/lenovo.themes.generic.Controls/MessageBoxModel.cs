using System;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Controls;

internal class MessageBoxModel : NotifyBase
{
	private string _Message;

	private string _okBtn = string.Empty;

	public string Title { get; set; }

	public string Message
	{
		get
		{
			return _Message;
		}
		set
		{
			_Message = value;
			OnPropertyChanged("Message");
		}
	}

	public string CancelBtn { get; set; }

	public int CancelBtnWidth { get; set; } = 100;

	public Action OnOkAction { get; set; }

	public Action OnReportAction { get; set; }

	public Action OnCancelAction { get; set; }

	public string OkBtn
	{
		get
		{
			return _okBtn;
		}
		set
		{
			_okBtn = value;
			OnPropertyChanged("OkBtn");
		}
	}

	public int OkBtnWidth { get; set; } = 100;

	public Visibility CancelBtnVisible { get; set; }

	public Visibility ReportBtnVisible { get; set; }

	public MessageBoxModel()
	{
		CancelBtnVisible = Visibility.Collapsed;
		ReportBtnVisible = Visibility.Collapsed;
	}
}
