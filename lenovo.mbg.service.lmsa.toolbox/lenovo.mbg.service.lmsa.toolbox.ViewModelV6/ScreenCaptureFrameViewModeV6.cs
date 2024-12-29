using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.ScreenCapture.BLL;
using lenovo.mbg.service.lmsa.toolbox.ScreenCapture.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.ViewModelV6;

public class ScreenCaptureFrameViewModeV6 : ViewModelBase
{
	private VideoBLL videoBLL = new VideoBLL();

	private bool _exportBtnEnable;

	private bool _deleteBtnEnable;

	private bool _refreshBtnEnable = true;

	private ReplayCommand startCommand;

	private bool startBtnEnable;

	private bool _isAllSelected;

	private Visibility connectDeviceTipsVisibility;

	private Visibility screenCupturePanelVisibility = Visibility.Collapsed;

	private Visibility _txtTipsVisibility = Visibility.Collapsed;

	public ObservableCollection<CaptureVideoItemDetailViewModelV6> VideoDataList { get; private set; }

	public ReplayCommand ExportBtnClick { get; private set; }

	public ReplayCommand DeleteBtnClick { get; private set; }

	public ReplayCommand RefreshBtnClick { get; private set; }

	public bool ExportBtnEnable
	{
		get
		{
			return _exportBtnEnable;
		}
		set
		{
			_exportBtnEnable = value;
			OnPropertyChanged("ExportBtnEnable");
		}
	}

	public bool DeleteBtnEnable
	{
		get
		{
			return _deleteBtnEnable;
		}
		set
		{
			_deleteBtnEnable = value;
			OnPropertyChanged("DeleteBtnEnable");
		}
	}

	public bool RefreshBtnEnable
	{
		get
		{
			return _refreshBtnEnable;
		}
		set
		{
			_refreshBtnEnable = value;
			OnPropertyChanged("RefreshBtnEnable");
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

	public bool IsAllSelected
	{
		get
		{
			return _isAllSelected;
		}
		set
		{
			_isAllSelected = value;
			OnPropertyChanged("IsAllSelected");
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
			return _txtTipsVisibility;
		}
		set
		{
			if (_txtTipsVisibility != value)
			{
				_txtTipsVisibility = value;
				OnPropertyChanged("TxtTipsVisibility");
			}
		}
	}

	public ScreenCaptureFrameViewModeV6()
	{
		VideoDataList = new ObservableCollection<CaptureVideoItemDetailViewModelV6>();
		StartCommand = new ReplayCommand(StartCommandHandler);
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
		InitButton();
		RefreshCommandHandler(null);
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
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

	public override void Dispose()
	{
		base.Dispose();
		HostProxy.deviceManager.MasterDeviceChanged -= DeviceManager_MasterDeviceChanged;
	}

	private void InitButton()
	{
		ExportBtnClick = new ReplayCommand(ExportCommandHandler);
		DeleteBtnClick = new ReplayCommand(DeleteCommandHandler);
		RefreshBtnClick = new ReplayCommand(RefreshCommandHandler);
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		TcpAndroidDevice tcpAndroidDevice = (TcpAndroidDevice)sender;
		SetStartBtnEnableStatus(e);
		SetPanelVisibility(e, tcpAndroidDevice.Property?.Category == "tablet");
		LoadData(tcpAndroidDevice);
		switch (e)
		{
		case DeviceSoftStateEx.Online:
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				RefreshCommandHandler(null);
			});
			break;
		case DeviceSoftStateEx.Offline:
			ExportBtnEnable = false;
			DeleteBtnEnable = false;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				VideoDataList.Clear();
			});
			break;
		}
	}

	private bool SetStartBtnEnableStatus(DeviceSoftStateEx deviceStatus)
	{
		StartBtnEnable = deviceStatus == DeviceSoftStateEx.Online;
		return StartBtnEnable;
	}

	private void StartCommandHandler(object e)
	{
		TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (currentDevice == null)
		{
			return;
		}
		if (currentDevice.ConnectType == ConnectType.Wifi)
		{
			ToolboxViewContext.SingleInstance.MessageBox.ShowMessage("K0257");
			return;
		}
		HostProxy.BehaviorService.Collect(BusinessType.SCREEN_RECORDER_USED, new BusinessData(BusinessType.SCREEN_RECORDER_USED, currentDevice));
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

	private void SetPanelVisibility(DeviceSoftStateEx deviceStatus, bool _isTablet)
	{
		bool flag = deviceStatus == DeviceSoftStateEx.Online;
		ConnectDeviceTipsVisibility = (flag ? Visibility.Hidden : Visibility.Visible);
		ScreenCupturePanelVisibility = ((!flag) ? Visibility.Hidden : Visibility.Visible);
		TxtTipsVisibility = ((!_isTablet) ? Visibility.Hidden : Visibility.Visible);
	}

	private void ExportCommandHandler(object e)
	{
		FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
		if (folderBrowser.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			_ = folderBrowser.SelectedPath;
			List<string> idList = (from m in VideoDataList
				where m.IsChecked
				select m.Id).ToList();
			string saveDir = folderBrowser.SelectedPath.Trim();
			new ImportAndExportWrapper().ExportFile(BusinessType.SCREEN_RECORDER_EXPORT, 20, idList, "K0567", "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", "Videos", saveDir, null);
		});
	}

	private void RefreshCommandHandler(object e)
	{
		IsAllSelected = false;
		ExportBtnEnable = false;
		DeleteBtnEnable = false;
		RefreshBtnEnable = false;
		List<string> idList = videoBLL.GetIdList("RecordScreen", "id", isSortDesc: false);
		List<VideoDetailModel> videoInfoList = videoBLL.GetVideoInfoList(idList);
		VideoDataList.Clear();
		if (videoInfoList != null)
		{
			foreach (VideoDetailModel item in videoInfoList)
			{
				VideoDataList.Add(new CaptureVideoItemDetailViewModelV6
				{
					Id = item.Id,
					VideoName = item.Name,
					LongDuration = item.Duration,
					Size = item.Size,
					ModifiedDate = item.ModifiedDateDisplayString
				});
			}
			Task.Factory.StartNew(delegate
			{
				try
				{
					UpdateVideoImages(VideoDataList);
				}
				catch
				{
				}
			});
		}
		RefreshBtnEnable = true;
	}

	private void UpdateVideoImages(IEnumerable<CaptureVideoItemDetailViewModelV6> target)
	{
		List<string> idList = target.Select((CaptureVideoItemDetailViewModelV6 m) => m.Id).ToList();
		string exportFolder = Path.Combine(Configurations.PicCacheDir, "ScreenCAP");
		videoBLL.ExportVideoThumbnailList(idList, exportFolder, delegate(string id, bool success, string path)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				foreach (CaptureVideoItemDetailViewModelV6 item in target)
				{
					if (item.Id.Equals(id))
					{
						try
						{
							BitmapImage bitmapImage = new BitmapImage();
							bitmapImage.BeginInit();
							bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
							bitmapImage.UriSource = new Uri(path, UriKind.Absolute);
							bitmapImage.EndInit();
							item.IconSource = bitmapImage;
							break;
						}
						catch
						{
							break;
						}
					}
				}
			});
		});
	}

	private void DeleteCommandHandler(object e)
	{
		if (ToolboxViewContext.SingleInstance.MessageBox.ShowMessage("K0569", MessageBoxButton.OKCancel) == true && HostProxy.deviceManager.MasterDevice is TcpAndroidDevice device)
		{
			BusinessData businessData = new BusinessData(BusinessType.SCREEN_RECORDER_DELETE, device);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			List<string> idList = (from m in VideoDataList
				where m.IsChecked
				select m.Id).ToList();
			bool flag = videoBLL.DeleteVideo(idList);
			stopwatch.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.SCREEN_RECORDER_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, flag ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, null));
			if (flag)
			{
				RefreshCommandHandler(null);
			}
		}
	}
}
