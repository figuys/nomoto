using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic;

public class MessageViewHelper
{
	protected readonly ViewModelBase Vm;

	protected Action<UserControl> MessageViewChange { get; }

	public MessageViewHelper(ViewModelBase vm)
	{
		Vm = vm;
	}

	public MessageViewHelper(ViewModelBase vm, Action<UserControl> viewChangeAction)
		: this(vm)
	{
		MessageViewChange = viewChangeAction;
	}

	public Task<bool?> Show(string title, string message, string ok, string cancel, bool showClose, MessageBoxImage icon, string notifyText = null, bool isPrivacy = false, Action<bool?> closeCallback = null)
	{
		return Task.Run(delegate
		{
			MessageCommViewV6 vv = null;
			Application.Current.Dispatcher.Invoke(delegate
			{
				vv = new MessageCommViewV6();
				vv.Init(title, message, ok, cancel, showClose, icon, notifyText, isPrivacy);
				vv.CloseAction = delegate(bool? r)
				{
					Close(r, closeCallback);
				};
				ChangeView(vv);
			});
			vv.WaitHandler.WaitOne();
			return vv.Result;
		});
	}

	public Task<bool?> ShowCustom(IMessageViewV6 view)
	{
		return Task.Run(delegate
		{
			MessageContentViewV6 vv = null;
			Application.Current.Dispatcher.Invoke(delegate
			{
				vv = new MessageContentViewV6();
				vv.Init(view);
				ChangeView(vv);
			});
			vv.WaitHandler.WaitOne();
			return vv.Result;
		});
	}

	public Task<bool?> ShowTopPic(string title, string message, ImageSource image, string ok, string cancel, bool showClose, bool isNotifyText, Action<bool?> closeCallback = null)
	{
		return Task.Run(delegate
		{
			MessageTopPicViewV6 vv = null;
			Application.Current.Dispatcher.Invoke(delegate
			{
				vv = new MessageTopPicViewV6();
				vv.Init(title, message, image, ok, cancel, showClose, isNotifyText);
				vv.CloseAction = delegate(bool? r)
				{
					Close(r, closeCallback);
				};
				ChangeView(vv);
			});
			vv.WaitHandler.WaitOne();
			return vv.Result;
		});
	}

	public Task<bool?> ShowRightPic(string title, string message, ImageSource image, string ok, string cancel, string tips = null, bool showClose = false, bool popupWhenClose = false, Action<bool?> closeCallback = null)
	{
		return Task.Run(delegate
		{
			MessageRightPicViewV6 vv = null;
			Application.Current.Dispatcher.Invoke(delegate
			{
				vv = new MessageRightPicViewV6();
				vv.Init(image, title, message, ok, cancel, tips, showClose, popupWhenClose);
				vv.CloseAction = delegate(bool? r)
				{
					Close(r, closeCallback);
				};
				ChangeView(vv);
			});
			vv.WaitHandler.WaitOne();
			return vv.Result;
		});
	}

	public void Close(bool? result, Action<bool?> closeCallback = null)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			if (Vm.MessageView is IMessageViewV6 messageViewV && !messageViewV.WaitHandler.SafeWaitHandle.IsClosed)
			{
				messageViewV.Result = result;
				messageViewV.WaitHandler.Set();
			}
			ChangeView(null);
			closeCallback?.Invoke(result);
		});
	}

	private void ChangeView(UserControl view)
	{
		Vm.MessageView = view;
		MessageViewChange?.Invoke(view);
	}
}
