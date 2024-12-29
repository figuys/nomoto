using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GoogleAnalytics;
using GoogleAnalytics.WPF.Managed;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.hostcontroller;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Language;
using lenovo.mbg.service.lmsa.LenovoId;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.Login.ViewModel;
using lenovo.mbg.service.lmsa.Login.ViewModel.UserOperation;
using lenovo.mbg.service.lmsa.ModelV6;
using lenovo.mbg.service.lmsa.OperationGuide.DeletePersionData;
using lenovo.mbg.service.lmsa.Properties;
using lenovo.mbg.service.lmsa.ResourcesCleanUp.View;
using lenovo.mbg.service.lmsa.ResourcesCleanUp.ViewModel;
using lenovo.mbg.service.lmsa.Services;
using lenovo.mbg.service.lmsa.ViewModel.SystemOperation;
using lenovo.mbg.service.lmsa.ViewModels.SystemOperation;
using lenovo.mbg.service.lmsa.ViewV6;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.Controls.Windows;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.Dialog.Permissions;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels;

internal class MainWindowViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private enum DeviceCap
	{
		VERTRES = 10,
		PHYSICALWIDTH = 110,
		SCALINGFACTORX = 114,
		DESKTOPVERTRES = 117
	}

	private class PermissionsCheckTipViewManager
	{
		private Dictionary<TcpAndroidDevice, PermissionsCheckConfirmEventArgs> tasks = new Dictionary<TcpAndroidDevice, PermissionsCheckConfirmEventArgs>();

		protected object locker = new object();

		private ConfirmAppPermissionWindow ConfirmPermissionWin = null;

		private MessageRightGifStepsViewV7 checkFilePermissionView = null;

		private KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs> currentDevice { get; set; }

		private void Add(TcpAndroidDevice d, PermissionsCheckConfirmEventArgs e)
		{
			lock (locker)
			{
				tasks[d] = e;
			}
		}

		private void Remove(TcpAndroidDevice d)
		{
			lock (locker)
			{
				tasks.Remove(d);
				if (currentDevice.Key == d)
				{
					currentDevice = new KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs>(null, null);
				}
			}
		}

		private KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs> Pop()
		{
			lock (locker)
			{
				int num = tasks.Count - 1;
				while (num >= 0)
				{
					KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs> result = tasks.ElementAt(num);
					if (result.Key.SoftStatus != DeviceSoftStateEx.Connecting)
					{
						tasks.Remove(result.Key);
						num--;
						continue;
					}
					return result;
				}
			}
			return new KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs>(null, null);
		}

		private void ShowTipView(TcpAndroidDevice device, int _checkPermissionsFailedType)
		{
			lock (locker)
			{
				if (ConfirmPermissionWin != null || checkFilePermissionView != null)
				{
					return;
				}
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					if (ConfirmPermissionWin == null && checkFilePermissionView == null)
					{
						string value = currentDevice.Key?.ConnectedAppType;
						string subText = string.Empty;
						if (!string.IsNullOrEmpty(value))
						{
							if ("Ma".Equals(value))
							{
								subText = "K0750";
							}
							else if ("Moto".Equals(value))
							{
								subText = "K0797";
							}
						}
						checkFilePermissionView = new MessageRightGifStepsViewV7(LangTranslation.Translate("K1886"), LangTranslation.Translate("K1887"), LangTranslation.Translate("K1888"), "app_permissions_all_files.gif");
						checkFilePermissionView.ConfirmCallback = delegate
						{
							checkFilePermissionView.SetConfirmBtnStatus(_enabled: false);
							Task.Run(delegate
							{
								KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs> keyValuePair = currentDevice;
								bool? haveGranted = device?.CheckPermissions(new List<string> { "ACCESS_ALL_FILES" });
								HostProxy.CurrentDispatcher?.Invoke(delegate
								{
									if (haveGranted.HasValue && haveGranted.Value)
									{
										checkFilePermissionView.Result = true;
										checkFilePermissionView.Close();
									}
									checkFilePermissionView.SetConfirmBtnStatus(_enabled: true);
								});
							});
						};
						checkFilePermissionView.CancelCallback = delegate
						{
							Task.Run(delegate
							{
								CancelAllTask();
							});
						};
						ConfirmAppPermissionViewModel checkBackupPermissionViewModel = new ConfirmAppPermissionViewModel();
						checkBackupPermissionViewModel.CurrentView = new NormalPermissionView
						{
							DataContext = new NormalPermissionViewModel("K0834", subText)
						};
						ConfirmPermissionWin = new ConfirmAppPermissionWindow
						{
							DataContext = checkBackupPermissionViewModel
						};
						checkBackupPermissionViewModel.CloseCallback = delegate
						{
							Task.Run(delegate
							{
								ConfirmPermissionWin.Result = false;
								CancelAllTask();
								CloseTipView();
							});
						};
						checkBackupPermissionViewModel.ConfirmCallback = delegate
						{
							checkBackupPermissionViewModel.ConfirmButtonEnabled = false;
							Task.Run(delegate
							{
								KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs> keyValuePair2 = currentDevice;
								bool? haveGranted2 = device?.CheckPermissions(new List<string> { "Backup" });
								HostProxy.CurrentDispatcher?.Invoke(delegate
								{
									if (haveGranted2.HasValue && haveGranted2.Value)
									{
										ConfirmPermissionWin.Result = true;
										ConfirmPermissionWin.Close();
									}
									checkBackupPermissionViewModel.ConfirmButtonEnabled = true;
								});
							});
						};
						Task.Run(delegate
						{
							bool? flag = true;
							bool? flag2 = null;
							if (_checkPermissionsFailedType == 1)
							{
								int? num = device?.Property?.ApiLevel;
								if (num.HasValue && num > 29)
								{
									flag = ApplcationClass.ApplcationStartWindow.ShowMessage(checkFilePermissionView);
								}
								if (flag == true)
								{
									flag2 = device?.CheckPermissions(new List<string> { "Backup" });
									if (flag2 != true)
									{
										flag2 = ApplcationClass.ApplcationStartWindow.ShowMessage(ConfirmPermissionWin);
									}
								}
							}
							if (_checkPermissionsFailedType == 2)
							{
								flag2 = ApplcationClass.ApplcationStartWindow.ShowMessage(ConfirmPermissionWin);
							}
							if (flag2 == true && currentDevice.Value.RequestPermissionsAction(arg: true))
							{
								PrivateAutoProcess(currentDevice.Key, currentDevice.Value, forceRemoveSelf: true);
							}
							checkFilePermissionView = null;
							ConfirmPermissionWin = null;
						});
					}
				});
			}
		}

		private void CloseTipView()
		{
			lock (locker)
			{
				if (ConfirmPermissionWin != null)
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						ConfirmPermissionWin?.GetMsgUi().Close();
						ConfirmPermissionWin = null;
					});
				}
			}
		}

		public void AutoProcess(TcpAndroidDevice device, PermissionsCheckConfirmEventArgs e)
		{
			PrivateAutoProcess(device, e, forceRemoveSelf: false);
		}

		private void PrivateAutoProcess(TcpAndroidDevice device, PermissionsCheckConfirmEventArgs e, bool forceRemoveSelf)
		{
			LogHelper.LogInstance.Info($"PrivateAutoProcess entered, device:[{device}],forceRemoveSelf:[{forceRemoveSelf}]");
			lock (locker)
			{
				if (forceRemoveSelf)
				{
					Remove(device);
					currentDevice = new KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs>(null, null);
					KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs> keyValuePair = Pop();
					device = keyValuePair.Key;
					e = keyValuePair.Value;
				}
				if (device == null)
				{
					KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs> keyValuePair2 = Pop();
					device = keyValuePair2.Key;
					e = keyValuePair2.Value;
				}
				if (device == null)
				{
					LogHelper.LogInstance.Info($"PrivateAutoProcess exited, currentDeviceKV[{currentDevice.Key}], device:[device is null]");
					CloseTipView();
					return;
				}
				if (device.SoftStatus == DeviceSoftStateEx.Connecting)
				{
					if (currentDevice.Key == null)
					{
						currentDevice = new KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs>(device, e);
						ShowTipView(device, e.CheckPermissionsFailedResult);
					}
					else
					{
						Add(device, e);
					}
				}
				else if (currentDevice.Key == device)
				{
					KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs> keyValuePair3 = Pop();
					if (keyValuePair3.Key == null)
					{
						currentDevice = new KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs>(null, null);
						CloseTipView();
					}
					else
					{
						currentDevice = keyValuePair3;
					}
				}
				else
				{
					Remove(device);
				}
				if (tasks.Count == 0 && currentDevice.Key == null)
				{
					CloseTipView();
				}
				LogHelper.LogInstance.Info($"PrivateAutoProcess exited, task count[{tasks?.Count}] currentDeviceKV[{currentDevice.Key}], device:[{device}],forceRemoveSelf:[{forceRemoveSelf}]");
			}
		}

		private void CancelAllTask()
		{
			lock (locker)
			{
				foreach (KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs> task in tasks)
				{
					task.Value.RequestPermissionsAction(arg: false);
				}
				if (currentDevice.Key != null && currentDevice.Value != null)
				{
					currentDevice.Value.RequestPermissionsAction(arg: false);
				}
				currentDevice = new KeyValuePair<TcpAndroidDevice, PermissionsCheckConfirmEventArgs>(null, null);
			}
		}
	}

	private ObservableCollection<PluginModel> _PluginArr = new ObservableCollection<PluginModel>();

	private volatile bool isDisposing = false;

	private long lockFaltalErrorMsg = 0L;

	private DeviceDataCollection _DeviceDataCollection = new DeviceDataCollection();

	private static MainWindowViewModel singleInstance;

	private ConcurrentDictionary<int, CouponInfo> CacheCouponDic = new ConcurrentDictionary<int, CouponInfo>();

	protected AutoResetEvent couponlocker = new AutoResetEvent(initialState: false);

	private PermissionsCheckTipViewManager permissionsCheckTipViewManager = new PermissionsCheckTipViewManager();

	private long runninglocker = 0L;

	private Visibility m_deletePersonalButtonVisibility = Visibility.Collapsed;

	private MainWindowOperationPanelStyleViewModel mainWindowOperationPanelStyle = null;

	private Visibility maskLayerVisibility = Visibility.Collapsed;

	private UserOperationMenuViewModel mUserOperation;

	private SystemOperationViewModel mSystemOperation;

	private bool mDeletePersonalDataEnabled = true;

	private ReplayCommand mDeletePersonalDataCommand;

	private string mAssemblyVersion;

	private string _downloadcentertip = "K0322";

	private string _downloadcenterbutton = "K0698";

	private DeletePersonalDataOperationGuideViewModel m_deletePersonalDataViewModel;

	private ReplayCommand goMyDeviceCommandClick;

	private ReplayCommand goRescueCommandClick;

	private ReplayCommand goBackupRestoreCommandClick;

	private ReplayCommand goToolboxCommandClick;

	private FeedbackOperationItemViewModel feedback = new FeedbackOperationItemViewModel();

	private ImageSource _BannerImage = null;

	private Visibility _SmallBannerVisibility = Visibility.Collapsed;

	private Visibility _BannerVisibility = Visibility.Collapsed;

	private Visibility _MiddleBannerVisibility = Visibility.Collapsed;

	private bool _IsUiWorkEnabe = true;

	private string _Discount;

	private string _MotoCare;

	private string _SubText;

	private int _PositionStart;

	private int _PositionCount;

	private Visibility _iconDriverVisibility = Visibility.Collapsed;

	private Stopwatch sw = new Stopwatch();

	public ObservableCollection<PluginModel> PluginArr
	{
		get
		{
			return _PluginArr;
		}
		set
		{
			_PluginArr = value;
			OnPropertyChanged("PluginArr");
		}
	}

	public ReplayCommand BannerClickCommand { get; }

	protected static MotoCareInfo motoCareInfo { get; set; }

	protected static string thumbnailImg => motoCareInfo?.thumbnailImg;

	protected static string picture => motoCareInfo?.picture;

	protected static CouponInfo coupon { get; set; }

	protected static bool MiddleBannerManualClose { get; set; }

	public static MainWindowViewModel SingleInstance => singleInstance ?? (singleInstance = new MainWindowViewModel());

	public Visibility DeletePersonalButtonVisibility
	{
		get
		{
			return m_deletePersonalButtonVisibility;
		}
		set
		{
			if (m_deletePersonalButtonVisibility != value)
			{
				m_deletePersonalButtonVisibility = value;
				OnPropertyChanged("DeletePersonalButtonVisibility");
			}
		}
	}

	public MainWindowOperationPanelStyleViewModel MainWindowOperationPanelStyle
	{
		get
		{
			return mainWindowOperationPanelStyle;
		}
		set
		{
			if (mainWindowOperationPanelStyle != value)
			{
				mainWindowOperationPanelStyle = value;
				OnPropertyChanged("MainWindowOperationPanelStyle");
			}
		}
	}

	public Visibility MaskLayerVisibility
	{
		get
		{
			return maskLayerVisibility;
		}
		set
		{
			maskLayerVisibility = value;
			OnPropertyChanged("MaskLayerVisibility");
		}
	}

	public UserOperationMenuViewModel UserOperation
	{
		get
		{
			return mUserOperation;
		}
		set
		{
			if (mUserOperation != value)
			{
				mUserOperation = value;
				OnPropertyChanged("UserOperation");
			}
		}
	}

	public SystemOperationViewModel SystemOperation
	{
		get
		{
			return mSystemOperation;
		}
		set
		{
			if (mSystemOperation != value)
			{
				mSystemOperation = value;
				OnPropertyChanged("SystemOperation");
			}
		}
	}

	public HelpOperationViewModel HelpOperation { get; set; }

	public bool DeletePersonalDataEnabled
	{
		get
		{
			return mDeletePersonalDataEnabled;
		}
		set
		{
			if (mDeletePersonalDataEnabled != value)
			{
				mDeletePersonalDataEnabled = value;
				OnPropertyChanged("DeletePersonalDataEnabled");
			}
		}
	}

	public ReplayCommand DeletePersonalDataCommand
	{
		get
		{
			return mDeletePersonalDataCommand;
		}
		set
		{
			if (mDeletePersonalDataCommand != value)
			{
				mDeletePersonalDataCommand = value;
				OnPropertyChanged("DeletePersonalDataCommand");
			}
		}
	}

	public string AssemblyVersion
	{
		get
		{
			return mAssemblyVersion;
		}
		set
		{
			if (!(mAssemblyVersion == value))
			{
				mAssemblyVersion = value;
				OnPropertyChanged("AssemblyVersion");
			}
		}
	}

	public string downloadcentertip
	{
		get
		{
			return _downloadcentertip;
		}
		set
		{
			_downloadcentertip = value;
			OnPropertyChanged("downloadcentertip");
		}
	}

	public string downloadcenterbutton
	{
		get
		{
			return _downloadcenterbutton;
		}
		set
		{
			_downloadcenterbutton = value;
			OnPropertyChanged("downloadcenterbutton");
		}
	}

	public DeletePersonalDataOperationGuideViewModel DeletePersonalDataViewModel
	{
		get
		{
			return m_deletePersonalDataViewModel;
		}
		set
		{
			if (m_deletePersonalDataViewModel != value)
			{
				m_deletePersonalDataViewModel = value;
				OnPropertyChanged("DeletePersonalDataViewModel");
			}
		}
	}

	public ReplayCommand GoMyDeviceCommandClick
	{
		get
		{
			return goMyDeviceCommandClick;
		}
		set
		{
			if (goMyDeviceCommandClick != value)
			{
				goMyDeviceCommandClick = value;
				OnPropertyChanged("GoMyDeviceCommandClick");
			}
		}
	}

	public ReplayCommand GoRescueCommandClick
	{
		get
		{
			return goRescueCommandClick;
		}
		set
		{
			if (goRescueCommandClick != value)
			{
				goRescueCommandClick = value;
				OnPropertyChanged("GoRescueCommandClick");
			}
		}
	}

	public ReplayCommand GoBackupRestoreCommandClick
	{
		get
		{
			return goBackupRestoreCommandClick;
		}
		set
		{
			if (goBackupRestoreCommandClick != value)
			{
				goBackupRestoreCommandClick = value;
				OnPropertyChanged("GoBackupRestoreCommandClick");
			}
		}
	}

	public ReplayCommand GoToolboxCommandClick
	{
		get
		{
			return goToolboxCommandClick;
		}
		set
		{
			if (goToolboxCommandClick != value)
			{
				goToolboxCommandClick = value;
				OnPropertyChanged("GoToolboxCommandClick");
			}
		}
	}

	public FeedbackOperationItemViewModel FeedBack
	{
		get
		{
			return feedback;
		}
		set
		{
			if (feedback != value)
			{
				feedback = value;
				OnPropertyChanged("FeedBack");
			}
		}
	}

	public ImageSource BannerImage
	{
		get
		{
			return _BannerImage;
		}
		set
		{
			_BannerImage = value;
			OnPropertyChanged("BannerImage");
		}
	}

	public Visibility SmallBannerVisibility
	{
		get
		{
			return _SmallBannerVisibility;
		}
		set
		{
			_SmallBannerVisibility = value;
			OnPropertyChanged("SmallBannerVisibility");
		}
	}

	public Visibility BannerVisibility
	{
		get
		{
			return _BannerVisibility;
		}
		set
		{
			_BannerVisibility = value;
			OnPropertyChanged("BannerVisibility");
		}
	}

	public Visibility MiddleBannerVisibility
	{
		get
		{
			return _MiddleBannerVisibility;
		}
		set
		{
			if (HostProxy.HostNavigation.CurrentPluginID == "8ab04aa975e34f1ca4f9dc3a81374e2c" && !HostProxy.User.user.IsB2BSupportMultDev)
			{
				MiddleBannerManualClose = value != Visibility.Visible;
				_MiddleBannerVisibility = value;
			}
			else
			{
				_MiddleBannerVisibility = Visibility.Collapsed;
			}
			OnPropertyChanged("MiddleBannerVisibility");
		}
	}

	public bool IsUiWorkEnabe
	{
		get
		{
			return _IsUiWorkEnabe;
		}
		set
		{
			_IsUiWorkEnabe = value;
			OnPropertyChanged("IsUiWorkEnabe");
		}
	}

	public string Discount
	{
		get
		{
			return _Discount;
		}
		set
		{
			_Discount = value;
			OnPropertyChanged("Discount");
		}
	}

	public string MotoCare
	{
		get
		{
			return _MotoCare;
		}
		set
		{
			_MotoCare = value;
			OnPropertyChanged("MotoCare");
		}
	}

	public string SubText
	{
		get
		{
			return _SubText;
		}
		set
		{
			_SubText = value;
			OnPropertyChanged("SubText");
		}
	}

	public int PositionStart
	{
		get
		{
			return _PositionStart;
		}
		set
		{
			_PositionStart = value;
			OnPropertyChanged("PositionStart");
		}
	}

	public int PositionCount
	{
		get
		{
			return _PositionCount;
		}
		set
		{
			_PositionCount = value;
			OnPropertyChanged("PositionCount");
		}
	}

	public Visibility IconDriverVisibility
	{
		get
		{
			return _iconDriverVisibility;
		}
		set
		{
			_iconDriverVisibility = value;
			OnPropertyChanged("IconDriverVisibility");
		}
	}

	public void LoadPlugin()
	{
		if (ApplcationClass.AvailablePlugins == null)
		{
			return;
		}
		foreach (PluginModel availablePlugin in ApplcationClass.AvailablePlugins)
		{
			if (Configurations.BackupLastDateTime == DateTime.MinValue && availablePlugin.Info.PluginID == "13f79fe4cfc98747c78794a943886bcd")
			{
				PluginContainer.Instance.GetPlugin(availablePlugin.Info.PluginID).Init();
			}
			availablePlugin.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == "IsSelected")
				{
					PluginModel plugin = sender as PluginModel;
					if (plugin.IsSelected)
					{
						System.Windows.Application.Current.Dispatcher.Invoke(delegate
						{
							OnPluginChecked(plugin);
						});
					}
				}
			};
		}
		PluginArr = new ObservableCollection<PluginModel>(ApplcationClass.AvailablePlugins);
		if (PluginArr.Count != 0)
		{
			PluginArr[0].IsSelected = true;
		}
	}

	private void OnPluginChecked(PluginModel pModel)
	{
		LogHelper.LogInstance.Info("start plugin:" + pModel.Info.PluginName);
		HostProxy.HostNavigation.CurrentPluginID = pModel.Info.PluginID;
		DeviceConnectViewModel.Instance.ChangeEnadbled(pModel.Info.PluginID);
		MiddleBannerVisibility = ((coupon == null || MiddleBannerManualClose) ? Visibility.Collapsed : Visibility.Visible);
		if (pModel.IsLoaded)
		{
			pModel.Plugin.OnInit(null);
		}
		else
		{
			pModel.IsLoaded = true;
			IPlugin plugin = PluginContainer.Instance.GetPlugin(pModel.Info.PluginID);
			if (plugin == null)
			{
				return;
			}
			plugin.Init();
			pModel.Plugin = plugin;
			pModel.UiElement = plugin.CreateControl(ApplcationClass.ApplcationStartWindow);
		}
		if (pModel.Info.PluginID != "8ab04aa975e34f1ca4f9dc3a81374e2c")
		{
			ApplcationClass.ApplcationStartWindow.ShowMutilIcon(showIcon: false, showList: true);
		}
		if (HostProxy.HostNavigation.CurrentPluginID == "8ab04aa975e34f1ca4f9dc3a81374e2c" && !HostProxy.User.user.IsB2BSupportMultDev)
		{
			IconDriverVisibility = Visibility.Visible;
		}
		else
		{
			IconDriverVisibility = Visibility.Collapsed;
		}
	}

	public void GotoPluginById(string pluginId, object data = null)
	{
		PluginModel pluginModel = PluginArr.FirstOrDefault((PluginModel p) => p.Info.PluginID == pluginId);
		pluginModel.IsSelected = true;
		IPlugin plugin = PluginContainer.Instance.GetPlugin(pluginModel.Info.PluginID);
		plugin.OnInit(data);
	}

	[DllImport("gdi32.dll", SetLastError = true)]
	public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

	private MainWindowViewModel()
	{
		AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString() + (Configurations.IsReleaseVersion ? string.Empty : " (Test)");
		UserOperation = new UserOperationMenuViewModel();
		SystemOperation = new SystemOperationViewModel();
		HelpOperation = new HelpOperationViewModel();
		DeletePersonalDataCommand = new ReplayCommand(DeletePersonalDataCommandHandler);
		DeletePersonalDataViewModel = new DeletePersonalDataOperationGuideViewModel();
		GoMyDeviceCommandClick = new ReplayCommand(GoMyDeviceCommandClickHandler);
		GoRescueCommandClick = new ReplayCommand(GoRescueCommandClickHandler);
		GoBackupRestoreCommandClick = new ReplayCommand(GoBackupRestoreCommandClickHandler);
		GoToolboxCommandClick = new ReplayCommand(GoToolboxCommandClickHandler);
		BannerClickCommand = new ReplayCommand(BannerClickCommandHandler);
		System.Windows.Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
		AnalyticsManager.Current.DispatchPeriod = TimeSpan.Zero;
		AnalyticsManager.Current.ReportUncaughtExceptions = true;
		AnalyticsManager.Current.AutoAppLifetimeMonitoring = true;
		AnalyticsManager.Current.HitSent += AnalyticsManager_HitSent;
		AnalyticsManager.Current.HitMalformed += AnalyticsManager_HitMalformed;
		AnalyticsManager.Current.HitFailed += AnalyticsManager_HitFailed;
		AnalyticsManager.Current.PlatformTrackingInfo.OnTracking();
		Task.Run(delegate
		{
			InitDevicDataCollection();
			LanguageUpdateHelper.Instance.CheckNewLanguagePackage();
			InitUserRecordUploadAsync();
			InitializeDownloadResources();
			InitializeEvents();
			InitCheckScreen();
		});
		GlobalFun.WriteRegistryKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Rescue and Smart Assistant", "downloadpath", Configurations.DownloadPath);
	}

	public void Initialize()
	{
		global::Smart.GoogleAnalyticsTracker.Tracker.ScreenName = "rsa-startPage";
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateScreenView().Build());
		HiddenBanner();
		UserService.Single.OnlineUserChanged += Single_OnlineUserChanged;
		LoadPlugin();
		StartDeviceListener();
	}

	public void ShowBanner(object data)
	{
		WarrantyInfoBaseModel model = data as WarrantyInfoBaseModel;
		if (HostProxy.User.user.IsB2BSupportMultDev || model == null || string.IsNullOrEmpty(model.imei) || model.imei.Length <= 10)
		{
			return;
		}
		Task.Run(delegate
		{
			string twoLetterISORegionName = GlobalFun.GetRegionInfo().TwoLetterISORegionName;
			Dictionary<string, object> parameter = new Dictionary<string, object>
			{
				{ "country", twoLetterISORegionName },
				{
					"warrantyStatus",
					model.InWarranty ? 1 : 0
				},
				{
					"imei",
					model.InWarranty ? model.imei : null
				},
				{
					"clientUuid",
					GlobalFun.GetClientUUID()
				}
			};
			motoCareInfo = AppContext.WebApi.RequestContent<MotoCareInfo>(WebApiUrl.LOAD_WARRANTY_BANNER, parameter);
			if (motoCareInfo != null)
			{
				motoCareInfo.imei = model.imei;
				motoCareInfo.sn = model.msn ?? motoCareInfo.sn;
				motoCareInfo.WarrantyStartDate = model.WarrantyStartDate;
				motoCareInfo.WarrantyEndDate = model.WarrantyEndDate;
				motoCareInfo.InWarranty = model.InWarranty;
			}
			return motoCareInfo;
		}).ContinueWith(delegate
		{
			LoadCoupon(inWarranty: true).ContinueWith(delegate
			{
				if (coupon != null)
				{
					System.Windows.Application.Current.Dispatcher.Invoke(delegate
					{
						Discount = coupon.discountInfo;
						MotoCare = (motoCareInfo.InWarranty ? "K1610" : "K1613");
						string text = LangTranslation.Translate("K1391");
						text = text + " " + Discount;
						SubText = string.Format(LangTranslation.Translate("K1390"), text);
						PositionStart = SubText.IndexOf(text);
						PositionCount = text.Length;
						MiddleBannerVisibility = Visibility.Visible;
					});
				}
			});
			if (motoCareInfo == null)
			{
				HiddenBanner();
			}
			else
			{
				ChangeBannerPopup(show: true);
			}
		});
	}

	public void ChangeBannerPopup(bool show)
	{
		System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			if (motoCareInfo != null)
			{
				if (show)
				{
					System.Windows.Application.Current.MainWindow.Height = 772.0;
					SmallBannerVisibility = Visibility.Collapsed;
					BannerVisibility = Visibility.Visible;
					BannerImage = Convert2ImageSource(picture);
				}
				else
				{
					couponlocker.Set();
					System.Windows.Application.Current.MainWindow.Height = 700.0;
					SmallBannerVisibility = Visibility.Visible;
					BannerVisibility = Visibility.Collapsed;
					BannerImage = Convert2ImageSource(thumbnailImg);
				}
			}
		});
	}

	private void BannerClickCommandHandler(object data)
	{
		switch (data.ToString())
		{
		case "banner_click":
			couponlocker.WaitOne();
			if (coupon != null)
			{
				ShowCouponWindow();
			}
			else if (!string.IsNullOrEmpty(motoCareInfo?.url))
			{
				GlobalFun.OpenUrlByBrowser(motoCareInfo.url);
			}
			ChangeBannerPopup(show: false);
			break;
		case "small_banner_click":
			ChangeBannerPopup(show: true);
			break;
		case "banner_close":
			ChangeBannerPopup(show: false);
			break;
		case "middle_banner_close":
			MiddleBannerManualClose = true;
			MiddleBannerVisibility = Visibility.Collapsed;
			break;
		case "middle_banner_click":
			ShowCouponWindow();
			break;
		}
	}

	private async Task<CouponInfo> LoadCoupon(bool inWarranty)
	{
		couponlocker.Reset();
		coupon = await Task.Run(delegate
		{
			if (motoCareInfo == null || !inWarranty)
			{
				return (CouponInfo)null;
			}
			if (CacheCouponDic.ContainsKey(motoCareInfo.id))
			{
				return CacheCouponDic[motoCareInfo.id];
			}
			Dictionary<string, object> parameter = new Dictionary<string, object>
			{
				{ "motoCareId", motoCareInfo.id },
				{
					"clientUuid",
					GlobalFun.GetClientUUID()
				}
			};
			CouponInfo data = AppContext.WebApi.RequestContent<CouponInfo>(WebApiUrl.LOAD_COUPON, parameter);
			if (data != null)
			{
				CacheCouponDic.AddOrUpdate(motoCareInfo.id, data, (int k, CouponInfo v) => data);
			}
			return data;
		});
		couponlocker.Set();
		return coupon;
	}

	private void ShowCouponWindow()
	{
		System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			MiddleBannerVisibility = Visibility.Collapsed;
			IUserMsgControl ui = new CouponWindow
			{
				DataContext = new CouponWindowModel(motoCareInfo, coupon)
			};
			ApplcationClass.ApplcationStartWindow.ShowMessage(ui);
		});
	}

	private void HiddenBanner()
	{
		System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			System.Windows.Application.Current.MainWindow.Height = 700.0;
			SmallBannerVisibility = Visibility.Collapsed;
			BannerVisibility = Visibility.Collapsed;
			MiddleBannerVisibility = Visibility.Collapsed;
		});
	}

	private ImageSource Convert2ImageSource(string url)
	{
		if (!string.IsNullOrEmpty(url))
		{
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(url, UriKind.Absolute);
			bitmapImage.EndInit();
			return bitmapImage;
		}
		return null;
	}

	private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		LogHelper.LogInstance.Error("lenovo.mbg.service.lmsa.App.Current_DispatcherUnhandledException: Exception", e.Exception);
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateException(e.Exception.Message, isFatal: true).Build());
	}

	private void InitializeEvents()
	{
		MainWindowControl.Instance.OnPluginError += FirePluginError;
		WebApiHttpRequest.WebApiCallback = delegate(string t, object v)
		{
			if (AppContext.IsLogIn)
			{
				if (t == "NONETWORK")
				{
					return System.Windows.Application.Current.Dispatcher.Invoke(() => MessageBoxEx.Show(HostProxy.HostMaskLayerWrapper, ((bool)v) ? "K0983" : "K1179", MessageBoxButton.OK));
				}
				if (t == "TOKEN_EXPRIED")
				{
					bool? flag = ApplcationClass.ApplcationStartWindow.ShowMessage("K1863");
					System.Windows.Application.Current.Dispatcher.Invoke(delegate
					{
						LogoutMenuItemViewModel.LogOut(force: true);
					});
				}
			}
			return (object)null;
		};
	}

	private void FirePluginError(object sender, PluginErrorEventArgs e)
	{
		if (Interlocked.Read(ref lockFaltalErrorMsg) != 0)
		{
			return;
		}
		Interlocked.Exchange(ref lockFaltalErrorMsg, 1L);
		try
		{
			string strTitleTip = string.Format(Resources.ResourceManager.GetString("Plugin_Fatal_Error_Message", CultureInfo.CurrentCulture), e.Plugin.PluginInfo.PluginName);
			MessageBox_Common msgExit = new MessageBox_Common(System.Windows.Application.Current.MainWindow, TypeItems.MessageBoxType.OK, strTitleTip, "K0327", "");
			HostProxy.HostMaskLayerWrapper.New(msgExit, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				msgExit.ShowDialog();
			});
			MainWindowControl.Instance.ClosePlugin(e.Plugin);
		}
		finally
		{
			Interlocked.Exchange(ref lockFaltalErrorMsg, 0L);
		}
	}

	private void OnOpenNoNetworkDialog(object sender, bool isNoIntelnet)
	{
		System.Windows.Application.Current.Dispatcher.Invoke(() => MessageBoxEx.Show(HostProxy.HostMaskLayerWrapper, isNoIntelnet ? "K0983" : "K1179", MessageBoxButton.OK));
	}

	private async Task ClearResoucesAysnc()
	{
		await Task.Run(delegate
		{
			GlobalFun.KillProcess("adb");
		});
		await Task.Run(delegate
		{
			GlobalFun.KillProcess("qcomdloader");
		});
	}

	private void StartDeviceListener()
	{
		ClearResoucesAysnc().ContinueWith(delegate
		{
			Thread.Sleep(50);
			global::Smart.DeviceManagerEx.Start();
			global::Smart.DeviceManagerEx.MasterDeviceChanged += DeviceManagerEx_MasterDeviceChanged;
			global::Smart.DeviceManagerEx.Connecte += DeviceManagerEx_Connecte;
			global::Smart.DeviceManagerEx.DisConnecte += DeviceManagerEx_DisConnecte;
		});
	}

	private void DeviceManagerEx_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			if (e.Current is TcpAndroidDevice tcpAndroidDevice)
			{
				tcpAndroidDevice.AppAssistTips += D_AppAssistTips;
				tcpAndroidDevice.TcpConnectStepChanged += D_TcpConnectStepChanged;
			}
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		else
		{
			ApplcationClass.ApplcationStartWindow.ChangePinStoryboard(start: true);
		}
		if (e.Previous != null)
		{
			if (e.Previous is TcpAndroidDevice tcpAndroidDevice2)
			{
				tcpAndroidDevice2.AppAssistTips -= D_AppAssistTips;
				tcpAndroidDevice2.TcpConnectStepChanged -= D_TcpConnectStepChanged;
			}
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
		TryCloseTips();
	}

	private void DeviceManagerEx_DisConnecte(object sender, DeviceEx e)
	{
		if ((e.ConnectType == ConnectType.Adb || e.ConnectType == ConnectType.Wifi) && e is TcpAndroidDevice tcpAndroidDevice)
		{
			tcpAndroidDevice.PermissionsCheckConfirmEvent -= D_PermissionsCheckConfirmEvent;
			tcpAndroidDevice.PhysicalStatusChanged -= Current_PhysicalStatusChanged;
		}
	}

	private void DeviceManagerEx_Connecte(object sender, DeviceEx e)
	{
		if ((e.ConnectType == ConnectType.Adb || e.ConnectType == ConnectType.Wifi) && e is TcpAndroidDevice tcpAndroidDevice)
		{
			tcpAndroidDevice.PermissionsCheckConfirmEvent += D_PermissionsCheckConfirmEvent;
			tcpAndroidDevice.PhysicalStatusChanged += Current_PhysicalStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		if (e == DeviceSoftStateEx.Online)
		{
			TcpAndroidDevice d = sender as TcpAndroidDevice;
			if (d != null)
			{
				ApplcationClass.ApplcationStartWindow.ChangePinStoryboard(start: false);
				Task.Run(delegate
				{
					WebServiceProxy.SingleInstance.reportConnectedAppType(d.ConnectedAppType);
				});
			}
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			TryCloseTips();
		});
	}

	private void D_PermissionsCheckConfirmEvent(object sender, PermissionsCheckConfirmEventArgs e)
	{
		if (e != null)
		{
			permissionsCheckTipViewManager.AutoProcess((TcpAndroidDevice)sender, e);
		}
	}

	private void D_TcpConnectStepChanged(object sender, TcpConnectStepChangedEventArgs e)
	{
		if ("InstallApp".Equals(e.Step) && e.Result == ConnectStepStatus.Fail && e.ErrorCode == ConnectErrorCode.ApkInstallFailWithHaveNoSpace)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				string title = "K0670";
				string okButtonText = "K0327";
				LenovoPopupWindow frameworkElement = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, title, "K0327", okButtonText, null);
				HostProxy.HostMaskLayerWrapper.Show(frameworkElement);
			});
		}
	}

	public void InitUserRecordUploadAsync()
	{
		UserRequestRecordService userRequestRecordService = new UserRequestRecordService();
		userRequestRecordService.UploadAsync();
	}

	public void InitCheckScreen()
	{
		Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
		IntPtr hdc = graphics.GetHdc();
		int deviceCaps = GetDeviceCaps(hdc, 117);
		double num = (double)deviceCaps / SystemParameters.PrimaryScreenHeight;
		Rectangle bounds = Screen.GetBounds(System.Drawing.Point.Empty);
		if (bounds.Width <= 1920 && bounds.Height <= 1080 && num >= 1.5)
		{
			ApplcationClass.ApplcationStartWindow.ShowMessage("K1518", MessageBoxButton.OK, MessageBoxImage.Asterisk, isCloseBtn: true);
		}
	}

	private void InitDevicDataCollection()
	{
		Task.Run(delegate
		{
			_DeviceDataCollection.Initialize();
			_DeviceDataCollection.OnNewDeviceFound += _DeviceDataCollection_OnNewDeviceFound;
		});
	}

	private bool _DeviceDataCollection_OnNewDeviceFound(DeviceModel arg)
	{
		try
		{
			if (Interlocked.Read(ref runninglocker) != 0)
			{
				return false;
			}
			Interlocked.Exchange(ref runninglocker, 1L);
			bool result = false;
			if (string.IsNullOrEmpty(arg.ModelName))
			{
				return result;
			}
			System.Windows.Application.Current.Dispatcher.Invoke(delegate
			{
				if (ApplcationClass.ApplcationStartWindow.ShowMessage("K0711", "K0707", "K0571", "K0570") == true)
				{
					result = true;
				}
			});
			return result;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(ex.ToString());
			return false;
		}
		finally
		{
			Interlocked.Exchange(ref runninglocker, 0L);
		}
	}

	private void D_AppAssistTips(object sender, AppAssistTipsEventArgs e)
	{
		if (e != null && e.AssistTipsType == AppAssistTips.AppIsReadyTips)
		{
			bool isReady = (bool)e.Data;
			ShowGrantAppPermissionTips(sender as DeviceEx, isReady);
		}
	}

	private void Current_PhysicalStatusChanged(object sender, DevicePhysicalStateEx e)
	{
		switch (e)
		{
		case DevicePhysicalStateEx.Unauthorized:
			TryCloseTips();
			ShowGrantDebugPermissionTips(sender as DeviceEx);
			break;
		case DevicePhysicalStateEx.UsbDebugSwitchClosed:
			TryCloseTips();
			ShowUsbDebugSwitchClosedTips(sender as DeviceEx);
			break;
		}
	}

	private void ShowGrantDebugPermissionTips(DeviceEx device)
	{
		if (device == null)
		{
			return;
		}
		System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			IUserMsgControl win = new DebugPermissionWindow();
			bool running = true;
			Task.Run(delegate
			{
				do
				{
					Thread.Sleep(500);
					if (device.PhysicalStatus != DevicePhysicalStateEx.Unauthorized)
					{
						running = false;
						try
						{
							System.Windows.Application.Current.Dispatcher.Invoke(delegate
							{
								win.GetMsgUi().Close();
								win.CloseAction?.Invoke(true);
							});
						}
						catch (Exception ex)
						{
							throw ex;
						}
					}
				}
				while (running);
			});
			ApplcationClass.ApplcationStartWindow.ShowMessage(win);
			running = false;
		});
	}

	private void ShowGrantAppPermissionTips(DeviceEx device, bool isReady)
	{
		if (isReady)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				HostProxy.HostMaskLayerWrapper.FireCloseConditionCheck();
			});
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			HostProxy.HostMaskLayerWrapper.Show(new NormalPermissionsTipView
			{
				ConnectedAppType = (device as TcpAndroidDevice)?.ConnectedAppType
			}, () => !(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice) || (tcpAndroidDevice != null && !tcpAndroidDevice.IsReady.HasValue) || (tcpAndroidDevice != null && tcpAndroidDevice.IsReady.HasValue && tcpAndroidDevice.IsReady.Value));
		});
	}

	private void ShowUsbDebugSwitchClosedTips(DeviceEx device)
	{
		System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			IUserMsgControl win = new DeviceTutorialsDialogViewV6();
			bool running = true;
			Task.Run(delegate
			{
				Thread.Sleep(500);
				do
				{
					if (device.PhysicalStatus != DevicePhysicalStateEx.UsbDebugSwitchClosed)
					{
						running = false;
						try
						{
							GlobalCmdHelper.Instance.Execute(new
							{
								type = GlobalCmdType.TABLET_OPEN_USBDEBUG
							});
							System.Windows.Application.Current.Dispatcher.Invoke(delegate
							{
								win.GetMsgUi().Close();
								win.CloseAction?.Invoke(true);
							});
						}
						catch (Exception)
						{
						}
					}
				}
				while (running);
			});
			ApplcationClass.ApplcationStartWindow.ShowMessage(win);
			running = false;
		});
	}

	private void TryCloseTips()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			HostProxy.HostMaskLayerWrapper.FireCloseConditionCheck();
		});
	}

	private void InitializeDownloadResources()
	{
		LoadDownloadResources loadDownloadResource = new LoadDownloadResources();
		Task.Factory.StartNew(delegate
		{
			loadDownloadResource.Load();
		});
	}

	private string Parse(Hit hit)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (hit != null)
		{
			foreach (string key in hit.Data.Keys)
			{
				stringBuilder.Append(key + ":" + hit.Data[key] + "  ");
			}
		}
		return stringBuilder.ToString();
	}

	private string GADebugLog(Hit hit, string message)
	{
		return "Request: " + Parse(hit) + Environment.NewLine + " Results: " + message;
	}

	private void AnalyticsManager_HitSent(object sender, HitSentEventArgs e)
	{
		if (AnalyticsManager.Current.IsDebug)
		{
			LogHelper.LogInstance.Debug(GADebugLog(e.Hit, e.Response));
		}
	}

	private void AnalyticsManager_HitFailed(object sender, HitFailedEventArgs e)
	{
		if (AnalyticsManager.Current.IsDebug)
		{
			LogHelper.LogInstance.Debug(GADebugLog(e.Hit, "**Hit Failed** " + Environment.NewLine + " " + e.Error.Message));
		}
	}

	private void AnalyticsManager_HitMalformed(object sender, HitMalformedEventArgs e)
	{
		if (AnalyticsManager.Current.IsDebug)
		{
			LogHelper.LogInstance.Debug(GADebugLog(e.Hit, "**Hit Malformed ** " + Environment.NewLine + " " + e.HttpStatusCode));
		}
	}

	private void Single_OnlineUserChanged(object sender, OnlineUserChangedEventArgs e)
	{
		if (e.IsOnline)
		{
			UserOperation.OnlineUserChangedHandler(sender, e);
			if (lenovo.mbg.service.lmsa.Login.Business.PermissionService.Single.CheckPermission(UserService.Single.CurrentLoggedInUser.UserId, "8", "1"))
			{
				DeletePersonalButtonVisibility = Visibility.Visible;
			}
		}
		else
		{
			DeletePersonalButtonVisibility = Visibility.Collapsed;
		}
	}

	private void DeletePersonalDataCommandHandler(object e)
	{
		LenovoWindow win = new LenovoWindow();
		win.SizeToContent = SizeToContent.WidthAndHeight;
		ClearupMainViewModel dataContext = new ClearupMainViewModel();
		win.Content = new ClearupMainView
		{
			DataContext = dataContext
		};
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
	}

	private void GoMyDeviceCommandClickHandler(object args)
	{
		GoToPluginAndCheckUserOnline("02928af025384c75ae055aa2d4f256c8", showAgain: true);
	}

	private void GoRescueCommandClickHandler(object args)
	{
		GoToPluginAndCheckUserOnline("8ab04aa975e34f1ca4f9dc3a81374e2c", showAgain: true);
	}

	private void GoBackupRestoreCommandClickHandler(object args)
	{
		GoToPluginAndCheckUserOnline("02928af025384c75ae055aa2d4f256c8", showAgain: true, "lmsa-plugin-Device-backuprestore");
	}

	private void GoToolboxCommandClickHandler(object args)
	{
		GoToPluginAndCheckUserOnline("dd537b5c6c074ae49cc8b0b2965ce54a", showAgain: true);
	}

	private void GoToPluginAndCheckUserOnline(string pluginId, bool showAgain, object initdata = null)
	{
		if (!UserService.Single.IsOnline)
		{
			if (!showAgain)
			{
				return;
			}
			LenovoIdWindow.ShowDialogEx(isRegister: false, delegate(Window win)
			{
				System.Windows.Application.Current.Dispatcher.Invoke(delegate
				{
					win?.Close();
				});
				HostProxy.HostNavigation.SwitchTo(pluginId, initdata);
			});
		}
		else
		{
			HostProxy.HostNavigation.SwitchTo(pluginId, initdata);
		}
	}

	public void CloseWindow()
	{
		if (CheckCanCloseWindow())
		{
			ApplcationClass.ApplcationStartWindow.ShowQuitSurvey();
			Dispose();
		}
	}

	public new void Dispose()
	{
		if (!isDisposing)
		{
			isDisposing = true;
			Exit(0);
		}
	}

	public void Exit(int code)
	{
		Task task = Task.Run(delegate
		{
			HostProxy.LanguageService.FeedbackNoTranslate();
		});
		Task[] tasks = new Task[1] { task };
		Task.WaitAll(tasks, new TimeSpan(20000000L));
		ApplcationClass.ApplcationStartWindow?.Close();
	}

	public bool IsExecuteWork()
	{
		int num = PluginArr.Count((PluginModel n) => n.Plugin != null && n.Plugin.IsExecuteWork());
		if (num > 0)
		{
			ApplcationClass.ApplcationStartWindow.ShowMessage("K0852", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return true;
		}
		return false;
	}

	public bool CheckCanCloseWindow()
	{
		if (IsExecuteWork())
		{
			return false;
		}
		if (ApplcationClass.IsUpdatingPlug && !CheckClientVersion.Instance.NewVersionModel.ForceType)
		{
			return ApplcationClass.ApplcationStartWindow.ShowMessage("K0296", MessageBoxButton.OKCancel) == true;
		}
		ClosingWindow win = new ClosingWindow
		{
			Owner = System.Windows.Application.Current.MainWindow
		};
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.Show();
		});
		return true;
	}
}
