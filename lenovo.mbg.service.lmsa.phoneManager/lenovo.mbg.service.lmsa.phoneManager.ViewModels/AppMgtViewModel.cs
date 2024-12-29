using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.common;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Business.Apps;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.Component.Progress;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class AppMgtViewModel : ViewModelBase
{
	private static AppMgtViewModel _singleInstance;

	private int _systemAppCount;

	private int _myAppCount;

	private bool _isAllSelected;

	private int _AppTotalCount;

	private string _AppTotalSize;

	private int _AppChooseCount;

	private ObservableCollection<AppInfoModel> _myAppList;

	private ObservableCollection<AppInfoModel> _sysAppList;

	public DeviceAppManager DeviceAppManager;

	private AppType _AppType;

	private Visibility _UninstallVisbility;

	private string _ExportAppCacheDirectory = Configurations.AppCacheDir;

	private bool _isEnabledAppRefreshBtn = true;

	private int _AppCount;

	public static AppMgtViewModel SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new AppMgtViewModel();
			}
			return _singleInstance;
		}
	}

	public int SystemAppCount
	{
		get
		{
			return _systemAppCount;
		}
		set
		{
			_systemAppCount = value;
			OnPropertyChanged("SystemAppCount");
		}
	}

	public string SysAppCountLabel => "K0482";

	public int MyAppCount
	{
		get
		{
			return _myAppCount;
		}
		set
		{
			_myAppCount = value;
			OnPropertyChanged("MyAppCount");
		}
	}

	public string MyAppCountLabel => "K0483";

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

	public int AppTotalCount
	{
		get
		{
			return _AppTotalCount;
		}
		set
		{
			_AppTotalCount = value;
			OnPropertyChanged("AppTotalCount");
		}
	}

	public string AppTotalSize
	{
		get
		{
			return _AppTotalSize;
		}
		set
		{
			_AppTotalSize = value;
			OnPropertyChanged("AppTotalSize");
		}
	}

	public int AppChooseCount
	{
		get
		{
			return _AppChooseCount;
		}
		set
		{
			_AppChooseCount = value;
			OnPropertyChanged("AppChooseCount");
		}
	}

	public ObservableCollection<AppInfoModel> MyAppList
	{
		get
		{
			return _myAppList;
		}
		set
		{
			if (_myAppList != value)
			{
				_myAppList = value;
				OnPropertyChanged("MyAppList");
			}
		}
	}

	public ObservableCollection<AppInfoModel> SysAppList
	{
		get
		{
			return _sysAppList;
		}
		set
		{
			if (_sysAppList != value)
			{
				_sysAppList = value;
				OnPropertyChanged("SysAppList");
			}
		}
	}

	public AppType AppType
	{
		get
		{
			return _AppType;
		}
		set
		{
			_AppType = value;
			if (_AppType == AppType.SystemApp)
			{
				UninstallVisbility = Visibility.Collapsed;
			}
			else
			{
				UninstallVisbility = Visibility.Visible;
			}
		}
	}

	public Visibility UninstallVisbility
	{
		get
		{
			return _UninstallVisbility;
		}
		set
		{
			_UninstallVisbility = value;
			OnPropertyChanged("UninstallVisbility");
		}
	}

	public ReplayCommand AppSortCommand { get; set; }

	public ReplayCommand InstallApp { get; set; }

	public ReplayCommand ExportApp { get; set; }

	public ReplayCommand UninstallApp { get; set; }

	public ReplayCommand Refresh { get; set; }

	public bool IsEnabledAppRefreshBtn
	{
		get
		{
			return _isEnabledAppRefreshBtn;
		}
		set
		{
			if (_isEnabledAppRefreshBtn != value)
			{
				_isEnabledAppRefreshBtn = value;
				OnPropertyChanged("IsEnabledAppRefreshBtn");
			}
		}
	}

	public ReplayCommand SearchCommand { get; set; }

	public event EventHandler RefreshFinishHandler;

	public AppMgtViewModel()
	{
		ExportApp = new ReplayCommand(ExportAppCommandHandler);
		UninstallApp = new ReplayCommand(UninstallAppCommandHandler);
		Refresh = new ReplayCommand(RefreshCommandHandler);
		SearchCommand = new ReplayCommand(SearchCommandHandler);
		AppSortCommand = new ReplayCommand(AppSortCommandHandler);
		IsAllSelected = false;
		MyAppList = new ObservableCollection<AppInfoModel>();
		SysAppList = new ObservableCollection<AppInfoModel>();
		DeviceAppManager = new DeviceAppManager();
	}

	public override void LoadData()
	{
		base.LoadData();
		IsAllSelected = false;
		RefreshHandler(refresh: false);
	}

	private void AppSortCommandHandler(object prameter)
	{
	}

	private void ExportAppCommandHandler(object prameter)
	{
		if (!(prameter is ObservableCollection<AppInfoModel> source))
		{
			return;
		}
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		folderBrowserDialog.Description = LangTranslation.Translate("K0651");
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
		{
			if (!string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
			{
				_ExportAppCacheDirectory = folderBrowserDialog.SelectedPath;
			}
			List<string> idList = (from a in source
				where a.IsSelected
				select a into n
				select n.PackageName).ToList();
			new ImportAndExportWrapper().ExportFile(BusinessType.APP_EXPORT, 18, idList, ResourcesHelper.StringResources.SingleInstance.APP_EXPORT_MESSAGE, "{958781C8-0788-4F87-A4C3-CBD793AAB1A0}", ResourcesHelper.StringResources.SingleInstance.APP_CONTENT, _ExportAppCacheDirectory, (string id, Header header) => Path.Combine(_ExportAppCacheDirectory, id + ".apk"));
		}
	}

	private bool PrevCheck(string title, string content)
	{
		DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
		if (masterDevice != null && masterDevice.ConnectType == ConnectType.Wifi)
		{
			LenovoPopupWindow okwin = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, title, content, "K0327", null);
			HostProxy.HostMaskLayerWrapper.New(okwin, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				okwin.ShowDialog();
			});
			return false;
		}
		return true;
	}

	public void UninstallAppSingle(AppInfoModel appinfo)
	{
		if (PrevCheck(ResourcesHelper.StringResources.SingleInstance.APP_UNINSTALL_TITLE, ResourcesHelper.StringResources.SingleInstance.APP_UNINSTALL_CONTENT) && MessageBoxHelper.DeleteConfirmMessagebox(ResourcesHelper.StringResources.SingleInstance.UNINSTALL_CONFIRM_TITLE, ResourcesHelper.StringResources.SingleInstance.UNINSTALL_SINGAL_APP_CONTENT, "K0208", "K0485", new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Assets/Images/PicPopup/delete.png"))))
		{
			List<string> list = new List<string>();
			list.Add(appinfo.PackageName);
			Uninstall(list);
		}
	}

	private void UninstallAppCommandHandler(object prameter)
	{
		if (PrevCheck(ResourcesHelper.StringResources.SingleInstance.APP_UNINSTALL_TITLE, ResourcesHelper.StringResources.SingleInstance.APP_UNINSTALL_CONTENT) && prameter is ObservableCollection<AppInfoModel> source && MessageBoxHelper.DeleteConfirmMessagebox(ResourcesHelper.StringResources.SingleInstance.UNINSTALL_CONFIRM_TITLE, ResourcesHelper.StringResources.SingleInstance.UNINSTALL_APP_CONTENT, "K0208", "K0485", new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Assets/Images/PicPopup/delete.png"))))
		{
			List<string> packagenames = (from a in source
				where a.IsSelected
				select a into n
				select n.PackageName).ToList();
			Uninstall(packagenames);
		}
	}

	private void Uninstall(List<string> packagenames)
	{
		HostProxy.AsyncCommonProgressLoader.Progress(Context.MessageBox, delegate(IAsyncTaskContext context, CommonProgressWindowViewModel viewModel)
		{
			Action<NotifyTypes, object> action = (Action<NotifyTypes, object>)context.ObjectState;
			action(NotifyTypes.INITILIZE, new List<object>
			{
				new List<object>
				{
					ResourcesHelper.StringResources.SingleInstance.APP_CONTENT,
					packagenames.Count
				},
				new List<ProgressPramater>
				{
					new ProgressPramater
					{
						Message = ResourcesHelper.StringResources.SingleInstance.APP_UNINSTALL_MESSAGE
					}
				}
			});
			Stopwatch stopwatch = new Stopwatch();
			ResourceExecuteResult resourceExecuteResult = new ResourceExecuteResult();
			_ = Context.CurrentDevice?.Property?.ModelName;
			stopwatch.Start();
			try
			{
				foreach (string packagename in packagenames)
				{
					if (context.IsCancelCommandRequested)
					{
						break;
					}
					bool flag = AppsDeviceAppManager.Instance.Uninstall(context, packagename);
					resourceExecuteResult.Update(flag);
					if (flag)
					{
						_AppCount--;
						action(NotifyTypes.PERCENT, 1);
					}
				}
			}
			catch
			{
			}
			stopwatch.Stop();
			if (!context.IsCancelCommandRequested)
			{
				action(NotifyTypes.SUCCESS, new List<object>
				{
					new List<ProgressPramater>
					{
						new ProgressPramater
						{
							Message = ResourcesHelper.StringResources.SingleInstance.UNINSTALL_SUCCESS_MESSAGE
						}
					},
					true
				});
			}
		});
	}

	private void RefreshCommandHandler(object prameter)
	{
		lock (this)
		{
			if (!IsEnabledAppRefreshBtn)
			{
				return;
			}
			IsEnabledAppRefreshBtn = false;
		}
		RefreshHandler(refresh: true);
	}

	private void RefreshHandler(bool refresh)
	{
		HostProxy.PermissionService.BeginConfirmAppIsReady(HostProxy.deviceManager.MasterDevice, "Apps", null, delegate(bool? isReady)
		{
			if (isReady.HasValue && isReady.Value)
			{
				LogHelper.LogInstance.Info("BeginConfirmAppIsReady action called");
				SortedList<AppType, List<AppInfoModel>> appInfos = DeviceAppManager.GetAppInfo(refresh);
				if (appInfos != null && appInfos.Count != 0)
				{
					_AppCount = (from n in appInfos.SelectMany((KeyValuePair<AppType, List<AppInfoModel>> n) => n.Value)
						select n.PackageName).Distinct().Count();
					string totalsize = GlobalFun.ConvertLong2String(appInfos.SelectMany((KeyValuePair<AppType, List<AppInfoModel>> n) => n.Value).Sum((AppInfoModel n) => n.LSize), "F2");
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						SysAppList.Clear();
						MyAppList.Clear();
						MyAppCount = 0;
						IsAllSelected = false;
						SetAppTips(_AppCount, totalsize, 0);
						if (appInfos[AppType.SystemApp] != null && appInfos[AppType.SystemApp].Count > 0)
						{
							appInfos[AppType.SystemApp].ForEach(delegate(AppInfoModel n)
							{
								SysAppList.Add(n);
							});
							SystemAppCount = SysAppList.Count;
						}
						if (appInfos[AppType.MyApp] != null && appInfos[AppType.MyApp].Count > 0)
						{
							appInfos[AppType.MyApp].ForEach(delegate(AppInfoModel n)
							{
								MyAppList.Add(n);
							});
							MyAppCount = MyAppList.Count;
						}
						IsEnabledAppRefreshBtn = true;
					});
					AppsDeviceAppManager.Instance.UpdateIcon(UpdateAppIcon);
					this.RefreshFinishHandler?.Invoke(null, null);
				}
			}
			else
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					IsEnabledAppRefreshBtn = true;
				});
			}
		});
	}

	private void UpdateAppIcon(string packagename, string iconpath)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			foreach (AppInfoModel item in SysAppList.Where((AppInfoModel n) => n.PackageName == packagename))
			{
				item.AppImage = ImageHandleHelper.LoadBitmap(iconpath);
			}
			foreach (AppInfoModel item2 in MyAppList.Where((AppInfoModel n) => n.PackageName == packagename))
			{
				item2.AppImage = ImageHandleHelper.LoadBitmap(iconpath);
			}
		});
	}

	private void SearchCommandHandler(object prameter)
	{
		if (prameter == null)
		{
			return;
		}
		string name = prameter as string;
		List<AppInfoModel> list = DeviceAppManager.Select(name, AppType);
		switch (AppType)
		{
		case AppType.MyApp:
			MyAppList.Clear();
			list.ForEach(delegate(AppInfoModel n)
			{
				MyAppList.Add(n);
			});
			break;
		case AppType.SystemApp:
			SysAppList.Clear();
			list.ForEach(delegate(AppInfoModel n)
			{
				SysAppList.Add(n);
			});
			break;
		}
	}

	public void SetAppTips(int totalcount, string totalsize, int choosecount)
	{
		AppTotalCount = totalcount;
		AppTotalSize = totalsize;
		AppChooseCount = choosecount;
	}

	public override void Reset()
	{
		if (DataIsLoaded)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				base.Reset();
				SysAppList.Clear();
				MyAppList.Clear();
				SystemAppCount = 0;
				MyAppCount = 0;
				AppChooseCount = 0;
				AppTotalSize = null;
				AppTotalCount = 0;
				IsAllSelected = false;
				AppsDeviceAppManager.Instance.Clear();
			});
		}
	}
}
