using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using lenovo.themes.generic.Component;

namespace lenovo.themes.generic.Controls;

public partial class MessageBoxEx : Window, IComponentConnector
{
	public MessageBoxEx CurrentDialog { get; private set; }

	public MessageBoxEx(Window owner = null)
	{
		InitializeComponent();
		base.Width = 500.0;
		base.MinHeight = 212.0;
	}

	private void InitText(MessageBoxModel model)
	{
		if (!string.IsNullOrEmpty(model.Title) && Regex.IsMatch(model.Title, "^K\\d{4}"))
		{
			tbkTitle.LangKey = model.Title;
		}
		else
		{
			tbkTitle.Text = model.Title;
		}
		if (!string.IsNullOrEmpty(model.Message) && Regex.IsMatch(model.Message, "^K\\d{4}"))
		{
			txtInfo.LangKey = model.Message;
		}
		else
		{
			txtInfo.Text = model.Message;
		}
		if (!string.IsNullOrEmpty(model.OkBtn) && Regex.IsMatch(model.OkBtn, "^K\\d{4}"))
		{
			btnOk.LangKey = model.OkBtn;
		}
		else
		{
			btnOk.Content = model.OkBtn;
		}
		if (!string.IsNullOrEmpty(model.CancelBtn) && Regex.IsMatch(model.CancelBtn, "^K\\d{4}"))
		{
			btnCancel.LangKey = model.CancelBtn;
		}
		else
		{
			btnCancel.Content = model.CancelBtn;
		}
		base.DataContext = model;
	}

	public static bool? Show(HostMaskLayerWrapper wrapper, string title, string Message, string btnOkTitle = null, string btnCancelTitle = null)
	{
		MessageBoxEx dlg = new MessageBoxEx();
		MessageBoxModel messageBoxModel = new MessageBoxModel();
		messageBoxModel.Message = Message;
		messageBoxModel.Title = title;
		if (string.IsNullOrEmpty(btnOkTitle))
		{
			btnOkTitle = "K0327";
		}
		messageBoxModel.OkBtn = btnOkTitle;
		if (!string.IsNullOrEmpty(btnCancelTitle))
		{
			messageBoxModel.CancelBtn = btnCancelTitle;
			messageBoxModel.CancelBtnVisible = Visibility.Visible;
		}
		else
		{
			messageBoxModel.CancelBtnVisible = Visibility.Collapsed;
		}
		dlg.InitText(messageBoxModel);
		return wrapper.New(dlg, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			dlg.ShowDialog();
			return (bool?)dlg.Tag;
		});
	}

	public static bool? Show(HostMaskLayerWrapper wrapper, string Message, MessageBoxButton btn = MessageBoxButton.OKCancel, MessageBoxImage icon = MessageBoxImage.Asterisk)
	{
		MessageBoxEx dlg = new MessageBoxEx();
		MessageBoxModel messageBoxModel = new MessageBoxModel();
		messageBoxModel.Message = Message;
		switch (btn)
		{
		case MessageBoxButton.OKCancel:
			messageBoxModel.OkBtn = "K0327";
			messageBoxModel.CancelBtn = "K0208";
			messageBoxModel.CancelBtnVisible = Visibility.Visible;
			break;
		case MessageBoxButton.YesNo:
			messageBoxModel.OkBtn = "K0571";
			messageBoxModel.CancelBtn = "K0570";
			messageBoxModel.CancelBtnVisible = Visibility.Visible;
			break;
		default:
			messageBoxModel.OkBtn = "K0327";
			messageBoxModel.CancelBtnVisible = Visibility.Collapsed;
			break;
		}
		switch (icon)
		{
		case MessageBoxImage.Hand:
			messageBoxModel.Title = "Error";
			break;
		case MessageBoxImage.Exclamation:
			messageBoxModel.Title = "K0071";
			break;
		default:
			messageBoxModel.Title = "K0711";
			break;
		}
		dlg.InitText(messageBoxModel);
		return wrapper.New(dlg, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			dlg.ShowDialog();
			return (bool?)dlg.Tag;
		});
	}

	public static bool? Show(HostMaskLayerWrapper wrapper, string Message, string btnOkTitle, Action OnOkAction = null, MessageBoxImage icon = MessageBoxImage.Asterisk, string btnCancelTitle = null, Action OnCancelAction = null)
	{
		MessageBoxEx dlg = new MessageBoxEx();
		MessageBoxModel messageBoxModel = new MessageBoxModel();
		messageBoxModel.Message = Message;
		messageBoxModel.OnOkAction = OnOkAction;
		messageBoxModel.OnCancelAction = OnCancelAction;
		if (string.IsNullOrEmpty(btnOkTitle))
		{
			btnOkTitle = "K0327";
		}
		messageBoxModel.OkBtn = btnOkTitle;
		if (!string.IsNullOrEmpty(btnCancelTitle))
		{
			messageBoxModel.CancelBtn = btnCancelTitle;
			messageBoxModel.CancelBtnVisible = Visibility.Visible;
		}
		else
		{
			messageBoxModel.CancelBtnVisible = Visibility.Collapsed;
		}
		switch (icon)
		{
		case MessageBoxImage.Hand:
			messageBoxModel.Title = "Error";
			break;
		case MessageBoxImage.Exclamation:
			messageBoxModel.Title = "K0071";
			break;
		default:
			messageBoxModel.Title = "K0711";
			break;
		}
		dlg.InitText(messageBoxModel);
		return wrapper.New(dlg, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			dlg.ShowDialog();
			return (bool?)dlg.Tag;
		});
	}

	public static bool? Show(HostMaskLayerWrapper wrapper, string Message, Action OnReportAction, MessageBoxButton btn = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Asterisk)
	{
		MessageBoxEx dlg = new MessageBoxEx();
		MessageBoxModel messageBoxModel = new MessageBoxModel();
		messageBoxModel.Message = Message;
		switch (btn)
		{
		case MessageBoxButton.OKCancel:
			messageBoxModel.OkBtn = "K0327";
			messageBoxModel.CancelBtn = "K0208";
			messageBoxModel.CancelBtnVisible = Visibility.Visible;
			break;
		case MessageBoxButton.YesNo:
			messageBoxModel.OkBtn = "K0571";
			messageBoxModel.CancelBtn = "K0570";
			messageBoxModel.CancelBtnVisible = Visibility.Visible;
			break;
		default:
			messageBoxModel.OkBtn = "K0327";
			messageBoxModel.CancelBtnVisible = Visibility.Collapsed;
			break;
		}
		switch (icon)
		{
		case MessageBoxImage.Hand:
			messageBoxModel.Title = "Error";
			break;
		case MessageBoxImage.Exclamation:
			messageBoxModel.Title = "K0071";
			break;
		default:
			messageBoxModel.Title = "K0711";
			break;
		}
		messageBoxModel.OnReportAction = OnReportAction;
		messageBoxModel.ReportBtnVisible = Visibility.Visible;
		dlg.InitText(messageBoxModel);
		return wrapper.New(dlg, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			dlg.ShowDialog();
			return (bool?)dlg.Tag;
		});
	}

	public static bool? Show(HostMaskLayerWrapper wrapper, string msg, string title, string okText = "K0327", string cancelText = "", int okbtnWidth = 100, int cancelbtnWidth = 100, Action OnReportAction = null)
	{
		MessageBoxEx dlg = new MessageBoxEx();
		MessageBoxModel messageBoxModel = new MessageBoxModel
		{
			Title = title,
			Message = msg,
			OkBtn = okText,
			OkBtnWidth = okbtnWidth,
			CancelBtn = cancelText,
			CancelBtnWidth = cancelbtnWidth
		};
		if (string.IsNullOrEmpty(cancelText))
		{
			messageBoxModel.CancelBtnVisible = Visibility.Collapsed;
		}
		else
		{
			messageBoxModel.CancelBtnVisible = Visibility.Visible;
		}
		if (OnReportAction != null)
		{
			messageBoxModel.OnReportAction = OnReportAction;
			messageBoxModel.ReportBtnVisible = Visibility.Visible;
		}
		dlg.InitText(messageBoxModel);
		return wrapper.New(dlg, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			dlg.ShowDialog();
			return (bool?)dlg.Tag;
		});
	}

	public void ShowAutoClose(HostMaskLayerWrapper wrapper, string title, string msg, int showSeconds, string okText = "K0327", string cancelText = "", Action CallBack = null)
	{
		CurrentDialog = new MessageBoxEx();
		MessageBoxModel model = new MessageBoxModel
		{
			Title = title,
			Message = msg,
			CancelBtn = cancelText,
			OkBtn = $"{okText} ( {showSeconds.ToString()} )",
			CancelBtnVisible = (string.IsNullOrEmpty(cancelText) ? Visibility.Collapsed : Visibility.Visible)
		};
		CurrentDialog.InitText(model);
		DispatcherTimer dispatcherTimer = new DispatcherTimer();
		dispatcherTimer.Interval = TimeSpan.FromSeconds(1.0);
		dispatcherTimer.Tick += delegate(object sender, EventArgs e)
		{
			showSeconds--;
			if (showSeconds < 0)
			{
				((DispatcherTimer)sender).Stop();
				CurrentDialog.Close();
			}
			CurrentDialog.Dispatcher.Invoke(() => CurrentDialog.btnOk.Content = $"{okText} ( {showSeconds} )");
		};
		dispatcherTimer.Start();
		CurrentDialog.Closed += delegate
		{
			CallBack?.Invoke();
		};
		wrapper.New(CurrentDialog, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			CurrentDialog.ShowDialog();
		});
	}

	public static MessageBoxEx CreateMsgTip(string title, string msg)
	{
		MessageBoxEx messageBoxEx = new MessageBoxEx();
		MessageBoxModel model = new MessageBoxModel
		{
			Title = title,
			Message = msg,
			OkBtn = null,
			CancelBtn = null
		};
		messageBoxEx.btnOk.Visibility = Visibility.Collapsed;
		messageBoxEx.InitText(model);
		return messageBoxEx;
	}

	public void CloseDialog()
	{
		CurrentDialog.Close();
	}

	private void OnBtnCancel(object sender, RoutedEventArgs e)
	{
		if ((sender as Button).Tag != null)
		{
			base.Tag = false;
			(base.DataContext as MessageBoxModel).OnCancelAction?.Invoke();
		}
		else
		{
			base.Tag = null;
		}
		Close();
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		(base.DataContext as MessageBoxModel).OnOkAction?.Invoke();
		base.Tag = true;
		base.DialogResult = true;
	}

	private void OnBtnReport(object sender, RoutedEventArgs e)
	{
		MessageBoxModel obj = base.DataContext as MessageBoxModel;
		base.DialogResult = true;
		obj.OnReportAction?.Invoke();
	}

	public void UpdateMessageInfo(string message)
	{
		(base.DataContext as MessageBoxModel).Message = message;
	}
}
