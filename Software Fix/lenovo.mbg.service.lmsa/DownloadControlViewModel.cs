using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Download;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.ViewModels;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa;

public class DownloadControlViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private bool m_isDownloadedClick = false;

	private Visibility m_downloadPannelVisibility = Visibility.Collapsed;

	private Visibility mDownloadingTaskPanelVisibility = Visibility.Visible;

	private Visibility mDownloadedTaskPanelVisibility = Visibility.Collapsed;

	private static DownloadControlViewModel _singleInstance;

	private DownloadButtonViewModel _downloadButtonViewModel = null;

	private Brush mDownloadingTabForegroundBrush = null;

	private Brush mDownloadingTabBackgroundBrush = null;

	private Brush mDownloadCompletedTabForegroundBrush = null;

	private Brush mDownloadCompletedTabBackgroundBrush = null;

	public ObservableCollection<DownloadInfo> _DownloadingTasks = new ObservableCollection<DownloadInfo>();

	public ObservableCollection<DownloadedInfo> _DownloadedTasks = new ObservableCollection<DownloadedInfo>();

	private object downloadInfoLock = new object();

	private string _DownloadSavePath = Configurations.DownloadPath;

	public bool isDownloadedClick
	{
		get
		{
			return m_isDownloadedClick;
		}
		set
		{
			m_isDownloadedClick = value;
			OnPropertyChanged("isDownloadedClick");
		}
	}

	public Visibility downloadPannelVisibility
	{
		get
		{
			return m_downloadPannelVisibility;
		}
		set
		{
			m_downloadPannelVisibility = value;
			OnPropertyChanged("downloadPannelVisibility");
		}
	}

	public Visibility DownloadingTaskPanelVisibility
	{
		get
		{
			return mDownloadingTaskPanelVisibility;
		}
		set
		{
			if (mDownloadingTaskPanelVisibility != value)
			{
				mDownloadingTaskPanelVisibility = value;
				OnPropertyChanged("DownloadingTaskPanelVisibility");
			}
		}
	}

	public Visibility DownloadedTaskPanelVisibility
	{
		get
		{
			return mDownloadedTaskPanelVisibility;
		}
		set
		{
			if (mDownloadedTaskPanelVisibility != value)
			{
				mDownloadedTaskPanelVisibility = value;
				OnPropertyChanged("DownloadedTaskPanelVisibility");
			}
		}
	}

	public ReplayCommand DownloadedItemClick { get; private set; }

	public ReplayCommand ViewButtonClickCommand { get; set; }

	public static DownloadControlViewModel SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new DownloadControlViewModel();
			}
			return _singleInstance;
		}
	}

	public DownloadButtonViewModel DownloadButtonViewModel
	{
		get
		{
			return _downloadButtonViewModel;
		}
		set
		{
			if (_downloadButtonViewModel != value)
			{
				_downloadButtonViewModel = value;
				OnPropertyChanged("DownloadButtonViewModel");
			}
		}
	}

	public ButtonViewModel CloseButtonViewModel { get; set; }

	public string DownloadLoationDirectory { get; set; }

	public ReplayCommand DownloadLoationClick { get; set; }

	public Brush DownloadingTabForegroundBrush
	{
		get
		{
			return mDownloadingTabForegroundBrush;
		}
		set
		{
			if (mDownloadingTabForegroundBrush != value)
			{
				mDownloadingTabForegroundBrush = value;
				OnPropertyChanged("DownloadingTabForegroundBrush");
			}
		}
	}

	public Brush DownloadingTabBackgroundBrush
	{
		get
		{
			return mDownloadingTabBackgroundBrush;
		}
		set
		{
			if (mDownloadingTabBackgroundBrush != value)
			{
				mDownloadingTabBackgroundBrush = value;
				OnPropertyChanged("DownloadingTabBackgroundBrush");
			}
		}
	}

	public Brush DownloadCompletedTabForegroundBrush
	{
		get
		{
			return mDownloadCompletedTabForegroundBrush;
		}
		set
		{
			if (mDownloadCompletedTabForegroundBrush != value)
			{
				mDownloadCompletedTabForegroundBrush = value;
				OnPropertyChanged("DownloadCompletedTabForegroundBrush");
			}
		}
	}

	public Brush DownloadCompletedTabBackgroundBrush
	{
		get
		{
			return mDownloadCompletedTabBackgroundBrush;
		}
		set
		{
			if (mDownloadCompletedTabBackgroundBrush != value)
			{
				mDownloadCompletedTabBackgroundBrush = value;
				OnPropertyChanged("DownloadCompletedTabBackgroundBrush");
			}
		}
	}

	public DownloadedTaskModel SelectedDownloadedItem { get; set; }

	public ReplayCommand DownloadClick { get; set; }

	public ReplayCommand DeleteDownloadingTask { get; set; }

	public ButtonViewModel ModifyStoragePathButtonViewModel { get; set; }

	public ButtonViewModel OpenStoragePathViewModel { get; set; }

	public ObservableCollection<DownloadInfo> DownloadingTasks
	{
		get
		{
			return _DownloadingTasks;
		}
		set
		{
			_DownloadingTasks = value;
			OnPropertyChanged("DownloadingTasks");
		}
	}

	public ObservableCollection<DownloadedInfo> DownloadedTasks
	{
		get
		{
			return _DownloadedTasks;
		}
		set
		{
			_DownloadedTasks = value;
			OnPropertyChanged("DownloadedTasks");
		}
	}

	public string DownloadSavePath
	{
		get
		{
			return Configurations.DownloadPath;
		}
		set
		{
			Configurations.DownloadPath = value;
			OnPropertyChanged("DownloadSavePath");
		}
	}

	private DownloadControlViewModel()
	{
		DownloadedItemClick = new ReplayCommand(DownloadedItemClickHandler);
		ViewButtonClickCommand = new ReplayCommand(ViewButtonClickCommandHandler);
		DownloadClick = new ReplayCommand(DownloadClickHandler);
		DeleteDownloadingTask = new ReplayCommand(DeleteDownloadingClickHandler);
		GlobalCmdHelper.Instance.OnDelRomAfterRescue = delegate(string fileName)
		{
			GlobalDeletedCallback(new List<string> { fileName });
		};
		GlobalCmdHelper.Instance.OnDelRomAfterRescueRetry = delegate(List<string> fileArr)
		{
			GlobalDeletedCallback(fileArr);
		};
	}

	public void Show()
	{
		downloadPannelVisibility = Visibility.Visible;
		isDownloadedClick = false;
	}

	private void DownloadedItemClickHandler(object aparams)
	{
		if (MainWindowControl.Instance.IsExecuteWork())
		{
			return;
		}
		DownloadedInfo data = aparams as DownloadedInfo;
		List<Dictionary<string, string>> list = JsonHelper.DeserializeJson2ListFromFile<Dictionary<string, string>>(Configurations.DownloadedMatchPath);
		if (list != null && list.Count > 0)
		{
			Dictionary<string, string> dictionary = list.FirstOrDefault((Dictionary<string, string> n) => n["FileUrl"] == data.FileUrl);
			if (dictionary != null)
			{
				ApplcationClass.ApplcationStartWindow.ShowDownloadCenter(isShow: false);
				MainWindowViewModel.SingleInstance.GotoPluginById("8ab04aa975e34f1ca4f9dc3a81374e2c", dictionary);
			}
		}
	}

	private void CloseButtonClickCommandHandler(object parameter)
	{
		if (UserService.Single.CurrentLoggedInUser == null)
		{
			ApplcationClass.NonTopmostPopup.IsOpen = true;
		}
		else if (UserService.Single.CurrentLoggedInUser != null && !string.IsNullOrEmpty(UserService.Single.CurrentLoggedInUser.UserId))
		{
			ApplcationClass.NonTopmostPopup.IsOpen = true;
		}
	}

	private void HeadLineInitialize()
	{
		DownloadLoationClick = new ReplayCommand(DownloadLoationClickHandler);
	}

	private void DownloadLoationClickHandler(object parameter)
	{
		GlobalFun.OpenFileExplorer(Configurations.DownloadPath);
	}

	public void DeleteInvalidDownloaded()
	{
		Task.Run(delegate
		{
			if (DownloadedTasks != null && DownloadedTasks.Count > 0)
			{
				List<DownloadedInfo> list = new List<DownloadedInfo>();
				foreach (DownloadedInfo downloadedTask in DownloadedTasks)
				{
					if (!GlobalFun.Exists(Path.Combine(downloadedTask.downloadInfo.LocalPath, Path.GetFileNameWithoutExtension(downloadedTask.downloadInfo.FileName))) && !GlobalFun.Exists(Path.Combine(downloadedTask.downloadInfo.LocalPath, downloadedTask.downloadInfo.FileName)))
					{
						list.Add(downloadedTask);
					}
				}
				if (list.Count > 0)
				{
					FireDeleteDownloaded(list, needDelete: false);
				}
			}
		});
	}

	public void Load(DownloadInfo downloadInfo)
	{
		if (downloadInfo.Status == DownloadStatus.WAITTING || downloadInfo.Status == DownloadStatus.MANUAL_PAUSE || downloadInfo.Status == DownloadStatus.DOWNLOADING)
		{
			AddDownloadingTask(downloadInfo);
		}
		else if (downloadInfo.Status == DownloadStatus.SUCCESS || downloadInfo.Status == DownloadStatus.UNZIPSUCCESS || downloadInfo.Status == DownloadStatus.ALREADYEXISTS)
		{
			RemoveDownloadingTask(downloadInfo);
			AddDownloadedTask(downloadInfo, insertFirst: true);
		}
	}

	public void AddDownloadingTask(DownloadInfo downloadInfo)
	{
		System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			lock (downloadInfoLock)
			{
				DownloadInfo downloadInfo2 = DownloadingTasks.FirstOrDefault((DownloadInfo n) => n.FileUrl == downloadInfo.FileUrl);
				if (downloadInfo2 == null)
				{
					DownloadingTasks.Insert(0, downloadInfo);
				}
			}
		});
	}

	public void RemoveDownloadingTask(DownloadInfo downloadInfo)
	{
		System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			lock (downloadInfoLock)
			{
				DownloadInfo downloadInfo2 = DownloadingTasks.FirstOrDefault((DownloadInfo n) => n.FileUrl == downloadInfo.FileUrl);
				if (downloadInfo2 != null)
				{
					DownloadingTasks.RemoveAt(DownloadingTasks.IndexOf(downloadInfo2));
				}
			}
		});
	}

	public void AddDownloadedTask(DownloadInfo downloadInfo, bool insertFirst)
	{
		System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			lock (downloadInfoLock)
			{
				DownloadedInfo downloadedInfo = DownloadedTasks.FirstOrDefault((DownloadedInfo n) => n.FileUrl == downloadInfo.FileUrl);
				if (downloadedInfo == null)
				{
					if (insertFirst)
					{
						DownloadedTasks.Add(Convert(downloadInfo));
					}
					else
					{
						DownloadedTasks.Insert(0, Convert(downloadInfo));
					}
				}
			}
		});
	}

	public void RemoveDownloadedTask(DownloadedInfo data)
	{
		System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			lock (downloadInfoLock)
			{
				DownloadedInfo downloadedInfo = DownloadedTasks.FirstOrDefault((DownloadedInfo n) => n.FileUrl == data.FileUrl);
				if (downloadedInfo != null)
				{
					DownloadedTasks.Remove(downloadedInfo);
				}
			}
		});
	}

	private DownloadedInfo Convert(DownloadInfo data)
	{
		DownloadedInfo downloadedInfo = new DownloadedInfo(data)
		{
			FileUrl = data.FileUrl
		};
		List<DownloadedInfo> list = JsonHelper.DeserializeJson2ListFromFile<DownloadedInfo>(Configurations.DownloadedMatchPath);
		if (list != null && list.Count > 0)
		{
			DownloadedInfo downloadedInfo2 = list.FirstOrDefault((DownloadedInfo n) => n.FileUrl == data.FileUrl);
			if (downloadedInfo2 != null)
			{
				downloadedInfo.marketName = downloadedInfo2.marketName;
				downloadedInfo.modelName = downloadedInfo2.modelName;
				downloadedInfo.hwCode = downloadedInfo2.hwCode;
				downloadedInfo.simCount = downloadedInfo2.simCount;
				downloadedInfo.country = downloadedInfo2.country;
				downloadedInfo.downloadInfo.IsManualMatch = !HostProxy.User.user.IsB2BSupportMultDev;
			}
			else
			{
				downloadedInfo.downloadInfo.IsManualMatch = false;
			}
		}
		else
		{
			downloadedInfo.downloadInfo.IsManualMatch = false;
		}
		return downloadedInfo;
	}

	private void DownloadClickHandler(object parameter)
	{
		DownloadInfo downloadInfo = parameter as DownloadInfo;
		if (downloadInfo.Status == DownloadStatus.DOWNLOADING)
		{
			LogHelper.LogInstance.Info("====>>Click try to stop downloading!");
			DownloadInfo downloadInfo2 = parameter as DownloadInfo;
			global::Smart.FileDownloadV6.Stop(downloadInfo2.FileUrl);
			return;
		}
		LogHelper.LogInstance.Info("====>>Click try to restart downloading!");
		if (global::Smart.FileDownloadV6.DownloadingCount >= 5)
		{
			ApplcationClass.ApplcationStartWindow.ShowMessage("K0071", "K0321", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
		}
		else
		{
			global::Smart.FileDownloadV6.ReStart(downloadInfo.FileUrl);
		}
	}

	private void DeleteDownloadingClickHandler(object parameter)
	{
		if (true != ApplcationClass.ApplcationStartWindow.ShowMessage("K0319", MessageBoxButton.YesNo))
		{
			return;
		}
		Task.Run(delegate
		{
			if (parameter is DownloadInfo downloadInfo)
			{
				RemoveDownloadingTask(downloadInfo);
				global::Smart.FileDownloadV6.Delete(downloadInfo.FileUrl);
			}
			else
			{
				DownloadedInfo item = parameter as DownloadedInfo;
				FireDeleteDownloaded(new List<DownloadedInfo> { item });
			}
		});
	}

	private void GlobalDeletedCallback(List<string> files)
	{
		List<DownloadedInfo> list = DownloadedTasks.Where((DownloadedInfo p) => files.Contains(p.downloadInfo.OriginalFileName)).ToList();
		FireDeleteDownloaded(list);
	}

	private void FireDeleteDownloaded(List<DownloadedInfo> list, bool needDelete = true)
	{
		List<string> list2 = new List<string>();
		foreach (DownloadedInfo item in list)
		{
			RemoveDownloadedTask(item);
			if (needDelete)
			{
				global::Smart.FileDownloadV6.Delete(item.downloadInfo.FileUrl);
			}
			list2.Add(item.downloadInfo.FileUrl);
		}
		if (list2.Count > 0)
		{
			UpdateDownloadedMatch(list2);
		}
	}

	private void UpdateDownloadedMatch(List<string> urls)
	{
		List<Dictionary<string, string>> list = JsonHelper.DeserializeJson2ListFromFile<Dictionary<string, string>>(Configurations.DownloadedMatchPath);
		if (list == null || list.Count <= 0)
		{
			return;
		}
		foreach (string url in urls)
		{
			Dictionary<string, string> dictionary = list.FirstOrDefault((Dictionary<string, string> n) => n["FileUrl"] == url);
			if (dictionary != null)
			{
				list.Remove(dictionary);
			}
		}
		JsonHelper.SerializeObject2File(Configurations.DownloadedMatchPath, list);
	}

	public void ModifyButtonDownloadPath()
	{
		WindowWrapper owner = new WindowWrapper(System.Windows.Application.Current.MainWindow);
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		while (true)
		{
			folderBrowserDialog.SelectedPath = DownloadSavePath;
			folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
			if (folderBrowserDialog.ShowDialog(owner) == DialogResult.OK)
			{
				if (folderBrowserDialog.SelectedPath.Length > 50)
				{
					ApplcationClass.ApplcationStartWindow.ShowMessage("K1856");
					continue;
				}
				DownloadSavePath = folderBrowserDialog.SelectedPath;
				break;
			}
			break;
		}
	}

	private void ViewButtonClickCommandHandler(object parameter)
	{
		GlobalFun.OpenFileExplorer(DownloadSavePath);
	}
}
