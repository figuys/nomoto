using System;
using System.Collections.Generic;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class MainViewModel : ViewModelBase, ILoading
{
	private Visibility _loadingViewVisibility = Visibility.Collapsed;

	private LeftNavigationViewModel leftNavigationViewModel;

	private readonly object loadingLock = new object();

	private List<object> _LoadintCounter;

	public Visibility LoadingViewVisibility
	{
		get
		{
			return _loadingViewVisibility;
		}
		set
		{
			if (_loadingViewVisibility != value)
			{
				_loadingViewVisibility = value;
				OnPropertyChanged("LoadingViewVisibility");
			}
		}
	}

	public LeftNavigationViewModel LeftNavigationViewModel
	{
		get
		{
			return leftNavigationViewModel;
		}
		set
		{
			if (leftNavigationViewModel != value)
			{
				leftNavigationViewModel = value;
				OnPropertyChanged("LeftNavigationViewModel");
			}
		}
	}

	public MainViewModel()
	{
		_LoadintCounter = new List<object>();
		LeftNavigationViewModel = LeftNavigationViewModel.SingleInstance;
		HostProxy.deviceManager.MasterDeviceChanged += delegate(object s, MasterDeviceChangedEventArgs e)
		{
			if (e.Current != null)
			{
				e.Current.SoftStatusChanged += Current_SoftStatusChanged;
			}
			if (e.Previous != null)
			{
				e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
			}
			LeftNavigationViewModel.SingleInstance.ResetNavigationItemViewModel();
		};
	}

	public override void LoadData()
	{
		base.LoadData();
	}

	private void MasterDeviceChangedHandler(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		switch (e)
		{
		case DeviceSoftStateEx.Offline:
		case DeviceSoftStateEx.ManualDisconnect:
			DisConnectedHandler(this);
			break;
		case DeviceSoftStateEx.Connecting:
		case DeviceSoftStateEx.Online:
			ConnectedHandler(this);
			break;
		case DeviceSoftStateEx.Reconncting:
			break;
		}
	}

	private void ConnectedHandler(MainViewModel mainView)
	{
		LeftNavigationViewModel.SingleInstance.ResetViewByConnectionStatus(DeviceSoftStateEx.Online);
	}

	private void DisConnectedHandler(MainViewModel mainView)
	{
		LeftNavigationViewModel.SingleInstance.ResetViewByConnectionStatus(DeviceSoftStateEx.Offline);
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			try
			{
				if (MusicPlayerViewModel.SingleInstance != null && MusicPlayerViewModel.SingleInstance.CurrentPlayId != 0)
				{
					MusicPlayerViewModel.SingleInstance.Stop();
					MusicPlayerViewModel.SingleInstance.ResetSongText();
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Stop music player throw exception:" + ex.ToString());
			}
		});
		try
		{
			((ILoading)mainView).Abort();
		}
		catch (Exception ex2)
		{
			LogHelper.LogInstance.Error("Abort loading view throw exception:" + ex2.ToString());
		}
	}

	public void Show(object handler)
	{
		lock (loadingLock)
		{
			if (!_LoadintCounter.Contains(handler))
			{
				_LoadintCounter.Add(handler);
				LoadingViewVisibility = Visibility.Visible;
			}
		}
	}

	public void Hiden(object handler)
	{
		lock (loadingLock)
		{
			if (_LoadintCounter.Contains(handler))
			{
				_LoadintCounter.Remove(handler);
				LoadingViewVisibility = ((_LoadintCounter.Count == 0) ? Visibility.Collapsed : Visibility.Visible);
			}
		}
	}

	public void Abort()
	{
		lock (loadingLock)
		{
			_LoadintCounter.Clear();
			LoadingViewVisibility = Visibility.Collapsed;
		}
	}
}
