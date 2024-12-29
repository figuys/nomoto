using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.Feedback.View;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.ViewModels;
using lenovo.themes.generic.Component;

namespace lenovo.mbg.service.lmsa;

public class HostOperationService : IHostOperationService
{
	private ConcurrentDictionary<string, Window> _Cache = new ConcurrentDictionary<string, Window>();

	public void CloseMaskLayer(string uid)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			Window value = null;
			_Cache.TryRemove(uid, out value);
			if (value != null)
			{
				value.Hide();
				value.Close();
				value = null;
			}
		});
	}

	public IntPtr ShowMaskLayer(string uid, WindowState state = WindowState.Normal)
	{
		IntPtr handler = IntPtr.Zero;
		Application.Current.Dispatcher.Invoke(delegate
		{
			Window mainWindow = Application.Current.MainWindow;
			Rectangle position = HardwareHelper.GetPosition(mainWindow);
			Window mask = new Window
			{
				Uid = Guid.NewGuid().ToString("N"),
				ResizeMode = ResizeMode.CanResize,
				Owner = Application.Current.MainWindow,
				WindowStyle = WindowStyle.None,
				ShowInTaskbar = false,
				AllowsTransparency = true,
				Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FC000000")),
				Opacity = 0.3,
				Focusable = false,
				IsEnabled = false,
				WindowState = state,
				BorderThickness = new Thickness(0.0),
				Left = position.Left,
				Top = position.Top,
				Width = position.Width,
				Height = position.Height
			};
			mask.Focusable = false;
			mask.IsHitTestVisible = false;
			mask.IsEnabled = false;
			if (_Cache.Count == 0)
			{
				mask.Closing += delegate
				{
					mask.Owner?.Activate();
				};
			}
			_Cache.TryAdd(uid, mask);
			mask.Show();
			handler = new WindowInteropHelper(mask).Handle;
		});
		return handler;
	}

	public void CloseAll()
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			foreach (Window value in _Cache.Values)
			{
				if (value != null)
				{
					value.Hide();
					value.Close();
				}
			}
			_Cache.Clear();
		});
	}

	public string GetAppVersion()
	{
		return LMSAContext.MainProcessVersion;
	}

	public void ShowGuideTips()
	{
		if (UserService.Single.IsOnline && UserService.Single.CurrentLoggedInUser != null && PermissionService.Single.CheckPermission(UserService.Single.CurrentLoggedInUser.UserId, "8", "1"))
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				MainWindowViewModel.SingleInstance.DeletePersonalDataViewModel.IsShowDeletePersonalDataGuide = true;
				MainWindowViewModel.SingleInstance.DeletePersonalDataViewModel.IsShowDeletePersonalDataGuidePopup = true;
			});
		}
	}

	public void ShowFeedBack()
	{
		HostProxy.HostMaskLayerWrapper.Show(new FeedbackMainView(), HostMaskLayerWrapper.CloseMaskOperation.NotCloseWhenDeviceDisconnect);
	}

	public void ShowBannerAsync(object data)
	{
		MainWindowViewModel.SingleInstance.ShowBanner(data);
	}
}
