using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.ScreenCapture.BLL;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenCapture.ViewModel;

public class ScreenCaptureFrameViewMode : ViewModelBase
{
	private VideoDataListViewModel videoDataListViewModel;

	private ReplayCommand startCommand;

	private long running;

	private bool startBtnEnable;

	private Visibility connectDeviceTipsVisibility;

	private Visibility screenCupturePanelVisibility = Visibility.Collapsed;

	private Visibility _TxtTipsVisibility = Visibility.Collapsed;

	private ReplayCommand cancelButtonClickCommand;

	public VideoDataListViewModel VideoDataListViewModel
	{
		get
		{
			return videoDataListViewModel;
		}
		set
		{
			if (videoDataListViewModel != value)
			{
				videoDataListViewModel = value;
				OnPropertyChanged("VideoDataListViewModel");
			}
		}
	}

	public ReplayCommand StartCommand
	{
		get
		{
			return startCommand;
		}
		set
		{
			if (startCommand != value)
			{
				startCommand = value;
				OnPropertyChanged("StartCommand");
			}
		}
	}

	public bool StartBtnEnable
	{
		get
		{
			return startBtnEnable;
		}
		set
		{
			if (startBtnEnable != value)
			{
				startBtnEnable = value;
				OnPropertyChanged("StartBtnEnable");
			}
		}
	}

	public Visibility ConnectDeviceTipsVisibility
	{
		get
		{
			return connectDeviceTipsVisibility;
		}
		set
		{
			if (connectDeviceTipsVisibility != value)
			{
				connectDeviceTipsVisibility = value;
				OnPropertyChanged("ConnectDeviceTipsVisibility");
			}
		}
	}

	public Visibility ScreenCupturePanelVisibility
	{
		get
		{
			return screenCupturePanelVisibility;
		}
		set
		{
			if (screenCupturePanelVisibility != value)
			{
				screenCupturePanelVisibility = value;
				OnPropertyChanged("ScreenCupturePanelVisibility");
			}
		}
	}

	public Visibility TxtTipsVisibility
	{
		get
		{
			return _TxtTipsVisibility;
		}
		set
		{
			if (_TxtTipsVisibility != value)
			{
				_TxtTipsVisibility = value;
				OnPropertyChanged("TxtTipsVisibility");
			}
		}
	}

	public ReplayCommand CancelButtonClickCommand
	{
		get
		{
			return cancelButtonClickCommand;
		}
		set
		{
			if (cancelButtonClickCommand != value)
			{
				cancelButtonClickCommand = value;
				OnPropertyChanged("CancelButtonClickCommand");
			}
		}
	}

	public ScreenCaptureFrameViewMode()
	{
		VideoDataListViewModel = new VideoDataListViewModel();
		CancelButtonClickCommand = new ReplayCommand(CancelButtonClickCommandHandler);
		StartCommand = new ReplayCommand(StartCommandHandler);
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
		};
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		TcpAndroidDevice tcpAndroidDevice = (TcpAndroidDevice)sender;
		SetStartBtnEnableStatus(e);
		SetPanelVisibility(e, tcpAndroidDevice.Property?.Category == "tablet");
		LoadData(tcpAndroidDevice);
		if (e == DeviceSoftStateEx.Offline)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				VideoDataListViewModel.Reset();
			});
		}
		else
		{
			VideoDataListViewModel.LoadData();
		}
	}

	private bool SetStartBtnEnableStatus(DeviceSoftStateEx deviceStatus)
	{
		StartBtnEnable = deviceStatus == DeviceSoftStateEx.Online;
		return StartBtnEnable;
	}

	private void StartCommandHandler(object e)
	{
		if (Interlocked.Read(ref running) != 0L)
		{
			return;
		}
		try
		{
			Interlocked.Exchange(ref running, 1L);
			TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
			if (currentDevice == null)
			{
				return;
			}
			if (currentDevice.ConnectType == ConnectType.Wifi)
			{
				LenovoPopupWindow win = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, null, "K0257", "K0327", null);
				HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
				{
					win.ShowDialog();
				});
				return;
			}
			HostProxy.BehaviorService.Collect(BusinessType.SCREEN_RECORDER_USED, null);
			HostProxy.PermissionService.BeginConfirmAppIsReady(HostProxy.deviceManager.MasterDevice, "ScreenRecord", null, delegate(bool? isReady)
			{
				if (isReady.HasValue && isReady.Value)
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						DeviceSoftStateEx softStatus = currentDevice.SoftStatus;
						if (SetStartBtnEnableStatus(softStatus))
						{
							StartBtnEnable = false;
							Task.Factory.StartNew(delegate
							{
								new VideoBLL().StartScreenCapture(delegate
								{
									HostProxy.CurrentDispatcher?.Invoke(delegate
									{
										StartBtnEnable = true;
									});
								});
							});
						}
					});
				}
			});
		}
		finally
		{
			Interlocked.Exchange(ref running, 0L);
		}
	}

	private void SetPanelVisibility(DeviceSoftStateEx deviceStatus, bool _isTablet)
	{
		bool flag = deviceStatus == DeviceSoftStateEx.Online;
		ConnectDeviceTipsVisibility = (flag ? Visibility.Hidden : Visibility.Visible);
		ScreenCupturePanelVisibility = ((!flag) ? Visibility.Hidden : Visibility.Visible);
		_TxtTipsVisibility = ((!_isTablet) ? Visibility.Hidden : Visibility.Visible);
	}

	private void CancelButtonClickCommandHandler(object args)
	{
		if (args is Window window)
		{
			window.Close();
		}
	}
}
