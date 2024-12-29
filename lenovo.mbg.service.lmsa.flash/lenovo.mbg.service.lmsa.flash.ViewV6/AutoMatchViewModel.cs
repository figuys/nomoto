using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Download;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.smartdevice;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class AutoMatchViewModel : ViewModelBase
{
	private bool ForceReMatch;

	private bool ModelNameMatch;

	private bool _RescueFinished;

	private bool _IsSetCountryCode;

	private ResourceResponseModel _FlashRes;

	private AutoMatchResource _MatchRes;

	protected IRecipeMessage Message;

	protected Dictionary<string, string> DegradeInfos;

	protected BusinessData businessData;

	protected bool IsReady;

	protected List<DownloadInfo> DownloadResources;

	protected RescueFrameV6 RescueFrame;

	protected BusinessType businessType;

	protected int? b2bOrderId;

	protected string IMEI;

	protected List<string> deleteResources = new List<string>();

	protected string fileLostMessage;

	private string _RomFileSize = "-";

	private string _RomFileName;

	private Visibility targetFirmwareVisibile = Visibility.Collapsed;

	private string _TargetFirmwareVersion;

	private string _TargetAndroidVersion;

	private string _FlashBtnText;

	private bool _FlashBtnEnable;

	private WarrantyInfoViewModelV6 warranty;

	private string marketName;

	private static readonly ImageSource DownloadImageIcon = Application.Current.Resources["v6_download_icon"] as ImageSource;

	private static readonly ImageSource DownloadingImageIcon = Application.Current.Resources["v6_downloading_icon"] as ImageSource;

	private ImageSource downloadImage;

	private Visibility reSelectVisibile = Visibility.Collapsed;

	private Visibility helpVisibile = Visibility.Collapsed;

	private ObservableCollection<DeviceInfoModel> itemArr;

	private DateTime startRescueTime = DateTime.Now;

	private bool startRescueTimeTag;

	public DevCategory Category { get; private set; }

	public UseCaseDevice UcDevice { get; private set; }

	public RescueDeviceInfoModel DeviceInfo { get; private set; }

	public string RomFileSize
	{
		get
		{
			return _RomFileSize;
		}
		set
		{
			_RomFileSize = value;
			OnPropertyChanged("RomFileSize");
		}
	}

	public string RomFileName
	{
		get
		{
			return _RomFileName;
		}
		set
		{
			_RomFileName = value;
			OnPropertyChanged("RomFileName");
		}
	}

	public Visibility TargetFirmwareVisibile
	{
		get
		{
			return targetFirmwareVisibile;
		}
		set
		{
			targetFirmwareVisibile = value;
			OnPropertyChanged("TargetFirmwareVisibile");
		}
	}

	public string TargetFirmwareVersion
	{
		get
		{
			return _TargetFirmwareVersion;
		}
		set
		{
			_TargetFirmwareVersion = value;
			OnPropertyChanged("TargetFirmwareVersion");
		}
	}

	public string TargetAndroidVersion
	{
		get
		{
			return _TargetAndroidVersion;
		}
		set
		{
			_TargetAndroidVersion = value;
			OnPropertyChanged("TargetAndroidVersion");
		}
	}

	public string FlashBtnText
	{
		get
		{
			return _FlashBtnText;
		}
		set
		{
			_FlashBtnText = value;
			OnPropertyChanged("FlashBtnText");
		}
	}

	public bool FlashBtnEnable
	{
		get
		{
			return _FlashBtnEnable;
		}
		set
		{
			_FlashBtnEnable = value;
			OnPropertyChanged("FlashBtnEnable");
		}
	}

	public WarrantyInfoViewModelV6 Warranty
	{
		get
		{
			return warranty;
		}
		set
		{
			warranty = value;
			OnPropertyChanged("Warranty");
		}
	}

	public string MarketName
	{
		get
		{
			return marketName;
		}
		set
		{
			marketName = value;
			OnPropertyChanged("MarketName");
		}
	}

	public ImageSource DownloadImage
	{
		get
		{
			return downloadImage;
		}
		set
		{
			downloadImage = value;
			OnPropertyChanged("DownloadImage");
		}
	}

	public Visibility ReSelectVisibile
	{
		get
		{
			return reSelectVisibile;
		}
		set
		{
			reSelectVisibile = value;
			OnPropertyChanged("ReSelectVisibile");
		}
	}

	public Visibility HelpVisibile
	{
		get
		{
			return helpVisibile;
		}
		set
		{
			helpVisibile = value;
			OnPropertyChanged("HelpVisibile");
		}
	}

	public string ModelName { get; set; }

	public ObservableCollection<DeviceInfoModel> ItemArr
	{
		get
		{
			return itemArr;
		}
		set
		{
			itemArr = value;
			OnPropertyChanged("ItemArr");
		}
	}

	public ReplayCommand CopyCommand { get; }

	public ReplayCommand ReSelectCommand { get; }

	protected IAMatchView View { get; }

	public bool SupportFastboot => _FlashRes.fastboot;

	public AutoMatchViewModel(IAMatchView ui, DevCategory category)
	{
		View = ui;
		Category = category;
		CopyCommand = new ReplayCommand(CopyCommandHandler);
		ReSelectCommand = new ReplayCommand(ReSelectCommandHandler);
	}

	public void Init(AutoMatchResource data, WarrantyInfoBaseModel warranty)
	{
		_MatchRes = data;
		Message = new RecipeMessageImpl(new MessageViewHelper(this, delegate(UserControl view)
		{
			MainFrameV6.Instance.VM.ChangeRescuingNeedOperator(data.Id, view != null);
		}));
		Warranty = new WarrantyInfoViewModelV6(warranty);
		_FlashRes = data.resource;
		DeviceInfo = data.deviceInfo;
		UcDevice = new UseCaseDevice(data.device, data.Id);
		ReSelectVisibile = ((View.ParentView == null) ? Visibility.Collapsed : Visibility.Visible);
		IMEI = data.deviceInfo.imei;
		string text = data.device?.Property?.IMEI2;
		string sn = data.deviceInfo.sn;
		MarketName = data.deviceInfo.marketName ?? data.resource.marketName;
		ModelName = data.deviceInfo.modelName ?? data.resource.ModelName;
		string blurVersion = data.deviceInfo.blurVersion;
		string fingerPrint = data.deviceInfo.fingerPrint;
		string roCarrier = data.deviceInfo.roCarrier;
		string fsgVersion = data.deviceInfo.fsgVersion;
		string saleModel = data.deviceInfo.saleModel;
		string hwCode = data.deviceInfo.hwCode;
		string country = data.deviceInfo.country;
		string simCount = data.deviceInfo.simCount;
		string memory = data.deviceInfo.memory;
		string item = null;
		string item2 = null;
		if (data.device != null)
		{
			if (data.device.ConnectType == ConnectType.Fastboot)
			{
				data.device.Property.GetPropertyValue("cpu");
				item = data.device.Property.GetPropertyValue("androidVer");
				item2 = data.device.Property.GetPropertyValue("softwareVersion");
			}
			else
			{
				_ = data.device.Property.Processor;
				item = data.device.Property.AndroidVersion;
				item2 = data.device.Property.GetPropertyValue("ro.build.display.id");
			}
		}
		if (!string.IsNullOrEmpty(_FlashRes.fingerprint))
		{
			string[] array = _FlashRes.fingerprint.Split('/');
			if (array.Length >= 4)
			{
				string[] array2 = array[2].Split(':');
				if (array2.Length == 2)
				{
					TargetAndroidVersion = array2[1];
				}
				if (!_FlashRes.ModelName.StartsWith("Lenovo", StringComparison.OrdinalIgnoreCase))
				{
					TargetFirmwareVersion = array[3];
				}
				TargetFirmwareVisibile = ((string.IsNullOrEmpty(TargetAndroidVersion) && string.IsNullOrEmpty(TargetFirmwareVersion)) ? Visibility.Collapsed : Visibility.Visible);
			}
		}
		List<DeviceInfoModel> source = new List<DeviceInfoModel>
		{
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Model_Name"] as ImageSource, "K0455", ModelName, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Code"] as ImageSource, "K1766", saleModel, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Code"] as ImageSource, string.IsNullOrEmpty(text) ? "K0459" : "K0460", IMEI, 1, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Code"] as ImageSource, "K0461", text, 1, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Code"] as ImageSource, "K0462", sn, 1, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_BlurVersion"] as ImageSource, "Blur version:", blurVersion, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Fingerprint"] as ImageSource, "Fingerprint:", fingerPrint, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_RoCarrier"] as ImageSource, "Ro.carrier:", roCarrier, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Fsg_version"] as ImageSource, "Fsg version:", fsgVersion, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Code"] as ImageSource, "K1125", hwCode, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Country"] as ImageSource, "K1126", country, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_SimCount"] as ImageSource, "K1127", simCount, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Memory"] as ImageSource, "K1128", memory, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Android_version"] as ImageSource, "K0468", item, 0, 0),
			new DeviceInfoModel(Application.Current.Resources["v6_Icon_Firmware_version"] as ImageSource, "K0467", item2, 0, 0)
		};
		ItemArr = new ObservableCollection<DeviceInfoModel>(source.Where((DeviceInfoModel n) => !string.IsNullOrEmpty(n.Item2)));
		RomFileSize = "-";
		RomFileName = _FlashRes.RomResources.Name;
		InitDownloadResources();
		CheckResourcesIsReady();
	}

	private void InitDownloadResources()
	{
		ResourceResponseModel flashRes = _FlashRes;
		DownloadResources = new List<DownloadInfo>();
		if (flashRes.RomResources != null)
		{
			DownloadInfo rom = new DownloadInfo
			{
				DownloadUrl = flashRes.RomResources.URI,
				MD5 = flashRes.RomResources.MD5,
				FileType = flashRes.RomResources.Type,
				UnZip = flashRes.RomResources.UnZip,
				IsManualMatch = false
			};
			DownloadResources.Add(rom);
			Task.Run(delegate
			{
				DownloadInfo downloadedResource = FlashContext.SingleInstance.DownloadManager.DownloadService.GetDownloadedResource(rom.FileUrl);
				long size;
				if (downloadedResource != null && downloadedResource.FileSize > 0)
				{
					size = downloadedResource.FileSize;
				}
				else
				{
					GlobalFun.GetFileSize(rom.DownloadUrl, out size, throwException: false);
				}
				rom.FileSize = size;
				Application.Current.Dispatcher.Invoke(() => RomFileSize = GlobalFun.ConvertLong2String(size));
			});
		}
		if (flashRes.ToolResource != null)
		{
			DownloadResources.Add(new DownloadInfo
			{
				DownloadUrl = flashRes.ToolResource.URI,
				MD5 = flashRes.ToolResource.MD5,
				FileType = flashRes.ToolResource.Type,
				UnZip = flashRes.ToolResource.UnZip,
				IsManualMatch = false
			});
		}
		if (flashRes.CountryCode != null)
		{
			DownloadResources.Add(new DownloadInfo
			{
				DownloadUrl = flashRes.CountryCode.URI,
				MD5 = flashRes.CountryCode.MD5,
				FileType = flashRes.CountryCode.Type,
				UnZip = flashRes.CountryCode.UnZip,
				IsManualMatch = false
			});
		}
	}

	protected void SupportMutilChange(bool mutil)
	{
		Plugin.SupportMulti = mutil;
		if (Category == DevCategory.Phone)
		{
			MainFrameV6.Instance.ChangeMutilDeviceShowType(mutil);
		}
	}

	public void OnRescue()
	{
		if (IsReady)
		{
			Task.Run(delegate
			{
				try
				{
					UcDevice.Locked = true;
					UcDevice.RecipeLocked = false;
					MainFrameV6.Instance.ChangeStatusWhenStartRescue(rescuing: true, Category == DevCategory.Phone);
					if (MainFrameV6.Instance.ValidOtherCategoryRescueing(Category) || !ProcessB2B())
					{
						return;
					}
					startRescueTime = DateTime.Now;
					int result = MainFrameV6.Instance.IsDevAllowRescue(_MatchRes.device, isMatch: false).Result;
					DeviceInfo.rescueMark = ((result != -1) ? result : 0);
					if (result != -1 && result != 1)
					{
						Message.Show("K1555", "K1478", "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait();
						RescueCollectionModel rescueCollectionModel = GengrateSubmitData(2);
						FlashContext.SingleInstance.service.RequestContent(WebServicesContext.UPLOAD_WHEN_FLASH_FINISHED, rescueCollectionModel);
						LogHelper.LogInstance.WriteLogForUser(JsonHelper.SerializeObject2Json(rescueCollectionModel), rescueCollectionModel.rescueResult);
						return;
					}
					if (((!MainFrameV6.Instance.SupportMulti || Category != 0) && IsConnectedMutilDevice()) || (_FlashRes.latest && Message.Show("K0711", "K0736", "K0327", "K0208").Result != true) || Message.Show("K0939", "K0940", "K0397", "K0208", showClose: true, MessageBoxImage.Asterisk, null, isPrivacy: true).Result != true)
					{
						return;
					}
					new HwDetectionDataCollect().BeginCollect();
					if (_FlashRes.IsShowBackupTip || !_FlashRes.Category.Equals("Smart", StringComparison.OrdinalIgnoreCase))
					{
						string message = (_FlashRes.IsShowBackupTip ? "K0107" : "K1594");
						if (Message.BackConfirm("K0108", message, "K1489", "K1490", showClose: true, isNotifyText: true).Result != true)
						{
							return;
						}
					}
					if (_FlashRes.CountryCode != null)
					{
						Message.Show(null, FlashStaticResources.FLASH_LESSSFREESPACE).Wait();
					}
				}
				finally
				{
					MainFrameV6.Instance.ChangeStatusWhenStartRescue(rescuing: false, Category == DevCategory.Phone);
					UcDevice.Locked = false;
					startRescueTimeTag = false;
					if (UcDevice.Device != null)
					{
						UcDevice.Device.WorkType = DeviceWorkType.None;
					}
				}
				UcDevice.Locked = true;
				if (UcDevice.Device != null)
				{
					UcDevice.Device.WorkType = DeviceWorkType.Rescue;
				}
				bool isManual = _MatchRes.matchInfo.matchType == MatchType.MANUAL;
				if (Category == DevCategory.Phone)
				{
					businessType = (isManual ? BusinessType.RESCUE_MANUAL_PHONE_FLASH : BusinessType.RESCUE_AUTO_PHONE_FLASH);
				}
				else if (Category == DevCategory.Tablet)
				{
					businessType = (isManual ? BusinessType.RESCUE_MANUAL_TABLET_FLASH : BusinessType.RESCUE_AUTO_TABLET_FLASH);
				}
				else
				{
					businessType = (isManual ? BusinessType.RESCUE_MANUAL_SMART_FLASH : BusinessType.RESCUE_AUTO_SMART_FLASH);
				}
				businessData = new BusinessData(businessType, _MatchRes.device);
				Plugin.OperateTracker("RescueBtnClick", "Begin to rescue button click");
				LogHelper.LogInstance.Info("begin to rescue button click");
				Application.Current.Dispatcher.Invoke(delegate
				{
					ChangeRescueBtn(FlashStatusV6.Rescuing);
				});
				Application.Current.Dispatcher.Invoke(delegate
				{
					if (isManual && View.ParentView.DataContext is ManualMatchViewModel manualMatchViewModel)
					{
						manualMatchViewModel.RegisterDeviceAsync();
					}
				});
				UcDevice.Register(LoadRecipeResources(), Message, RescueFlowMonitorFun);
				Task.Run(delegate
				{
					UseCaseRunner.Run(UseCase.LMSA_Recovery, UcDevice);
				});
			});
			return;
		}
		Plugin.OperateTracker("DownloadBtnClick", "Begin download button clicked!");
		LogHelper.LogInstance.Info("begin download button clicked.");
		Message.ShowDownloadCenter(show: true);
		FlashBtnEnable = false;
		DownloadImage = DownloadingImageIcon;
		FlashBtnText = "K0293";
		DownloadResourcesManager.SingleInstance.PrepareFlashingResources(_FlashRes.ModelName, DownloadResources, delegate(string modelname, DownloadStatus status)
		{
			if (modelname == _FlashRes?.ModelName)
			{
				switch (status)
				{
				case DownloadStatus.DOWNLOADING:
					FlashBtnEnable = false;
					FlashBtnText = "K0293";
					break;
				case DownloadStatus.UNZIPPING:
					FlashBtnEnable = false;
					FlashBtnText = "K1596";
					break;
				case DownloadStatus.SUCCESS:
				case DownloadStatus.ALREADYEXISTS:
					Message.ShowDownloadCenter(show: false);
					IsReady = true;
					ChangeRescueBtn(FlashStatusV6.Ready);
					break;
				case DownloadStatus.MANUAL_PAUSE:
				case DownloadStatus.FAILED:
				case DownloadStatus.UNDEFINEERROR:
					IsReady = false;
					ChangeRescueBtn(FlashStatusV6.Download);
					break;
				}
			}
		});
		if (_MatchRes.matchInfo.matchType == MatchType.MANUAL && View.ParentView.DataContext is ManualMatchViewModel manualMatchViewModel2)
		{
			manualMatchViewModel2.SaveDownloadUri2LocalFile();
		}
	}

	private void CheckResourcesIsReady()
	{
		FlashBtnText = "K0100";
		FlashBtnEnable = false;
		Task.Run(() => DownloadResourcesManager.SingleInstance.ResourceReadly(DownloadResources)).ContinueWith(delegate(Task<bool> r)
		{
			IsReady = r.Result;
			ChangeRescueBtn(IsReady ? FlashStatusV6.Ready : FlashStatusV6.Download);
		});
	}

	private bool IsConnectedMutilDevice()
	{
		if (HostProxy.deviceManager.ConntectedDevices.Count > 1)
		{
			Message.Show("K0071", FlashStaticResources.AUTO_DEVICECOUNT_CHECK, "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait();
			return true;
		}
		return false;
	}

	private RecipeResources LoadRecipeResources()
	{
		RecipeResources recipeResources = new RecipeResources();
		recipeResources.Add(RecipeResources.ModelName, _FlashRes?.ModelName);
		recipeResources.Add(RecipeResources.RecipeUrl, _FlashRes.RecipeResource);
		recipeResources.Add("Platform", _FlashRes.Platform);
		recipeResources.Add("IsFastboot", _FlashRes.fastboot.ToString());
		recipeResources.AddResource(RecipeResources.Rom, _FlashRes.RomResources.URI);
		if (_FlashRes.ToolResource != null)
		{
			recipeResources.AddResource(RecipeResources.TooL, _FlashRes.ToolResource.URI);
		}
		if (_FlashRes.CountryCode != null)
		{
			recipeResources.AddResource(RecipeResources.CountryCode, _FlashRes.CountryCode.URI);
		}
		recipeResources.Add("category", _FlashRes.Category);
		recipeResources.Add("connectType", "auto");
		return recipeResources;
	}

	private bool ProcessB2B()
	{
		if (Category == DevCategory.Phone)
		{
			Application.Current.Dispatcher.Invoke(() => FlashBtnEnable = false);
			b2bOrderId = null;
			ResponseModel<RespOrders> responseModel = FlashContext.SingleInstance.service.Request<RespOrders>(WebApiUrl.CALL_B2B_QUERY_ORDER_URL, new
			{
				macAddressRsa = RsaHelper.RSAEncrypt(WebApiContext.RSA_PUBLIC_KEY, GlobalFun.GetMacAddr()),
				imei = (DeviceInfo.imei ?? ""),
				modelName = ModelName
			});
			if (responseModel.code == "0003" || responseModel.code == "9999")
			{
				Application.Current.Dispatcher.Invoke(() => FlashBtnEnable = true);
				return true;
			}
			RespOrders data = responseModel.content;
			if (data?.enableOrderDtos != null && data.enableOrderDtos.Count > 0)
			{
				b2bOrderId = data.enableOrderDtos[0].orderId;
			}
			RespOrders respOrders = data;
			if (respOrders != null && respOrders.popUp)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					if (data.popMode == 0)
					{
						MainFrameV6.Instance.IMsgManager.ShowB2BRemind(data.usedFreeAmount, data.freeAmount);
					}
					else if (data.popMode == 1)
					{
						MainFrameV6.Instance.IMsgManager.ShowB2BExpired(1);
					}
					else if (data.popMode == 2)
					{
						MainFrameV6.Instance.IMsgManager.ShowB2BExpired(2);
					}
					else if (data.popMode == 3)
					{
						MainFrameV6.Instance.IMsgManager.ShowMessage("K1772", MessageBoxButton.OK, MessageBoxImage.Exclamation, isCloseBtn: true);
					}
				});
			}
			Application.Current.Dispatcher.Invoke(() => FlashBtnEnable = true);
			return b2bOrderId.HasValue;
		}
		return true;
	}

	private void InstallDriver()
	{
		Action confrimAction = delegate
		{
			Message.Show("K0711", FlashStaticResources.DRIVER_INSTALL_CONFIRM, "K0295").Wait();
		};
		if (Regex.IsMatch(ModelName, "L19111", RegexOptions.IgnoreCase))
		{
			LogHelper.LogInstance.Debug("check l19111 driver");
			DriversHelper.CheckAndInstallInfDriver(DriverType.Unisoc_L19111, confrimAction, out var output);
			if (!string.IsNullOrEmpty(output))
			{
				LogHelper.LogInstance.Info(output);
			}
		}
		else if (Regex.IsMatch(ModelName, "SP101FU", RegexOptions.IgnoreCase))
		{
			LogHelper.LogInstance.Debug("check Lenovo SP101FU driver");
			DriversHelper.CheckAndInstallInfDriver(DriverType.PNP, confrimAction, out var output2);
			if (!string.IsNullOrEmpty(output2))
			{
				LogHelper.LogInstance.Info(output2);
			}
		}
		else if (Regex.IsMatch(ModelName, "CD-17302F", RegexOptions.IgnoreCase))
		{
			LogHelper.LogInstance.Debug("check Lenovo CD-17302F driver");
			DriversHelper.CheckAndInstallInfDriver(DriverType.ADBDRIVER, null, out var output3);
			if (!string.IsNullOrEmpty(output3))
			{
				LogHelper.LogInstance.Info(output3);
			}
		}
	}

	private object RescueFlowMonitorFun(RecipeMessageType messageType, object content)
	{
		RecipeMessage message = default(RecipeMessage);
		if (messageType != RecipeMessageType.MODELNAME)
		{
			message = (RecipeMessage)content;
		}
		switch (messageType)
		{
		case RecipeMessageType.UNDO:
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				Free();
				if (Category != 0 || !Plugin.SupportMulti)
				{
					MainFrameV6.Instance.ChangeContainerHorizontalAlignment(HorizontalAlignment.Center);
				}
				else
				{
					MainFrameV6.Instance.ChangeContainerHorizontalAlignment(HorizontalAlignment.Left);
				}
				MainFrameV6.Instance.VM.CurrentView = View as FrameworkElement;
			});
			ChangeRescueBtn(FlashStatusV6.Ready);
			RescueCollectionModel rescueCollectionModel = GengrateSubmitData(-1);
			FlashContext.SingleInstance.service.RequestContent(WebServicesContext.UPLOAD_WHEN_FLASH_FINISHED, rescueCollectionModel);
			LogHelper.LogInstance.WriteLogForUser(JsonHelper.SerializeObject2Json(rescueCollectionModel), rescueCollectionModel.rescueResult);
			break;
		}
		case RecipeMessageType.START:
			UpdateRescuingView(message.Message?.ToString());
			if (!startRescueTimeTag)
			{
				startRescueTimeTag = true;
				startRescueTime = DateTime.Now;
			}
			break;
		case RecipeMessageType.STEP:
			UpdateRescuingView(null, message.Progress);
			if (message.Progress == 100.0)
			{
				if (message.OverallResult == Result.PASSED && !_RescueFinished)
				{
					_RescueFinished = true;
				}
				else if (_RescueFinished)
				{
					_IsSetCountryCode = true;
				}
			}
			break;
		case RecipeMessageType.PROGRESS:
			UpdateRescuingView(null, message.Progress);
			break;
		case RecipeMessageType.MODELNAME:
			return ValidRescuingData(content as Dictionary<string, string>);
		case RecipeMessageType.FINISH:
			FireRescueFinish(message);
			break;
		case RecipeMessageType.REALFLASH:
			MainFrameV6.Instance.RealFalshing(_MatchRes.Id);
			break;
		}
		return true;
	}

	private void FireRescueFinish(RecipeMessage message)
	{
		int overallResult = (int)message.OverallResult;
		bool flag = overallResult == 1;
		string text = null;
		bool flag2 = false;
		bool? flag3 = false;
		if (_IsSetCountryCode)
		{
			text = (flag ? FlashStaticResources.COUNTRYCODE_SETTING_SUCCESS_TITLE : FlashStaticResources.COUNTRYCODE_SETTING_FAILED_TITLE);
		}
		if (message.FailedResult.HasValue)
		{
			Result? failedResult = message.FailedResult;
			int count = FlashFailedGuideHelper.GetCount(HostProxy.User.user.UserId, DeviceInfo.modelName, success: false);
			if (failedResult == Result.FASTBOOT_FLASH_ERASEDATE_FAILED)
			{
				flag3 = true;
				text = (flag ? "K1619" : "K1238");
			}
			else if (count != 3 && count != 10 && (failedResult == Result.FIND_COMPORT_FAILED || failedResult == Result.ADB_CONNECT_FAILED || failedResult == Result.FASTBOOT_CONNECT_FAILED || failedResult == Result.SHELL_CONNECTED_FAILED))
			{
				text = "K1202";
				flag2 = true;
			}
			else if (failedResult == Result.FASTBOOT_FLASH_SINGLEPARTITION_FAILED)
			{
				text = "K1239";
				flag3 = true;
			}
			else if (failedResult == Result.AUTRORIZED_FAILED)
			{
				text = "K1333";
				flag2 = true;
			}
			else if (failedResult == Result.PROCESS_FORCED_TEREMINATION)
			{
				text = "K1204";
			}
			else if (failedResult == Result.ROM_UNMATCH_FAILED)
			{
				text = "K1205";
			}
			else if (failedResult == Result.LOAD_RESOURCE_FAILED || failedResult == Result.CHECK_ROM_FILE_FAILED || failedResult == Result.LOAD_RESOURCE_FAILED_COUNTRYCODE || failedResult == Result.LOAD_RESOURCE_FAILED_REPLACE || failedResult == Result.ROM_DIRECTORY_NOT_EXISTS)
			{
				DeviceInfo.rescueMark = 7;
				fileLostMessage = "K1418";
				flag3 = null;
				if (failedResult == Result.LOAD_RESOURCE_FAILED_COUNTRYCODE)
				{
					if (!string.IsNullOrEmpty(_FlashRes.CountryCode?.Name))
					{
						deleteResources.Add(_FlashRes.CountryCode.Name);
					}
				}
				else if (failedResult == Result.LOAD_RESOURCE_FAILED_REPLACE || failedResult == Result.ROM_DIRECTORY_NOT_EXISTS)
				{
					if (!string.IsNullOrEmpty(_FlashRes.RomResources?.Name))
					{
						deleteResources.Add(_FlashRes.RomResources.Name);
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(_FlashRes.RomResources?.Name))
					{
						deleteResources.Add(_FlashRes.RomResources.Name);
					}
					if (!string.IsNullOrEmpty(_FlashRes.ToolResource?.Name))
					{
						deleteResources.Add(_FlashRes.ToolResource.Name);
					}
					if (!string.IsNullOrEmpty(_FlashRes.CountryCode?.Name))
					{
						deleteResources.Add(_FlashRes.CountryCode.Name);
					}
				}
			}
			else if (failedResult == Result.FASTBOOT_DEGRADE_QUIT)
			{
				DeviceInfo.rescueMark = 4;
			}
			else if (failedResult == Result.FASTBOOT_ERROR_RULES_QUIT)
			{
				DeviceInfo.rescueMark = 8;
			}
			else if (overallResult == 2)
			{
				if (DeviceInfo.rescueMark == 0 && failedResult == Result.INTERCEPTOR_QUIT)
				{
					DeviceInfo.rescueMark = 9;
				}
				text = null;
			}
			else
			{
				text = "K1663";
			}
		}
		else if (!flag)
		{
			if (overallResult == 0)
			{
				text = "K1663";
				flag2 = true;
			}
		}
		else if (!string.IsNullOrEmpty(message.Message as string))
		{
			text = message.Message.ToString();
		}
		if (message.FailedResult == Result.CLIENT_VERSION_LOWER_QUIT)
		{
			Task.Run(() => MainFrameV6.Instance.IMsgManager.ShowMessage("K0711", "K1885", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation, delegate
			{
				MainFrameV6.Instance.IMsgManager.LogOut();
			}));
		}
		else
		{
			SubmitRescueRecord(message);
		}
		if (overallResult != 2)
		{
			FlashFailedGuideHelper.CollectFalshResult(HostProxy.User.user.UserId, DeviceInfo.modelName, flag);
			Configurations.AddRescueResult(flag);
		}
		if (flag)
		{
			ChangeRescueBtn(FlashStatusV6.PASS, text);
			return;
		}
		ChangeRescueBtn((overallResult == 2) ? FlashStatusV6.QUIT : FlashStatusV6.FAIL, new Dictionary<string, object>
		{
			{ "msg", text },
			{ "normal", flag3 },
			{ "retry", flag2 }
		});
	}

	private int ValidRescuingData(Dictionary<string, string> dic)
	{
		string tractId = dic["tractId"];
		string text = dic["modelname"];
		string text2 = dic["imei"];
		string text3 = dic["fdrallowed"];
		string text4 = dic["securestate"];
		string device = dic["softwareVersion"];
		string text5 = dic["cid"];
		string value = dic["versioncheck"];
		bool flag = dic["onlyCheckModelName"].Equals("True", StringComparison.CurrentCultureIgnoreCase);
		bool flag2 = bool.Parse(dic["errorRules"]);
		string text6 = dic["errorRuleMessage"];
		int rescueMark = DeviceInfo.rescueMark;
		int num = -1;
		UcDevice.Log.AddLog("connected fastboot device modelname: " + text + ", resource match modelname: " + _FlashRes.ModelName + " | " + _FlashRes.RealModelName + ", fdrallowed: " + text3 + "，securestate: " + text4 + ", cid: " + text5, upload: true);
		try
		{
			bool flag3 = text3?.ToLower() == "no";
			bool flag4 = text4?.ToLower() == "flashing_locked";
			if (string.IsNullOrEmpty(text) || Regex.IsMatch(text, "^[0]+$"))
			{
				DeviceInfo.rescueMark = 5;
				Message.Show("K0071", "K1478", "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait();
				num = 0;
			}
			else if (text != _FlashRes.ModelName && text != _FlashRes.RealModelName)
			{
				DeviceInfo.rescueMark = 5;
				ModelNameMatch = false;
				string message = string.Format(HostProxy.LanguageService.Translate(FlashStaticResources.RESUCE_AUTO_MATCH_FASTBOOT_DEVICE_CONFIRM), text);
				if (!flag && Message.Show("K0071", message, "K0571", "K0570", showClose: false, MessageBoxImage.Exclamation).Result == true)
				{
					ForceReMatch = true;
					MainFrameV6.Instance.RemoveDevice(_MatchRes.Id, ForceReMatch);
					MainFrameV6.Instance.ChangeView(PageIndex.PHONE_ENTRANCE);
					DeviceEx device2 = HostProxy.deviceManager.ConntectedDevices.FirstOrDefault((DeviceEx n) => n.Identifer == tractId);
					Task.Run(delegate
					{
						MainFrameV6.Instance.AutoMatch(device2);
					});
				}
				num = 0;
			}
			else if (flag2)
			{
				string message2 = (string.IsNullOrEmpty(text6) ? "K1478" : HostProxy.LanguageService.Translate(text6));
				Message.Show("K1555", message2, "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait();
				num = 6;
			}
			else if (!flag)
			{
				if (flag4)
				{
					DeviceInfo.rescueMark = 2;
					num = 3;
				}
				else if (flag3)
				{
					DeviceInfo.rescueMark = 1;
				}
				else if (MainFrameV6.Instance.FastbootErrorStatusArr.Contains(text5))
				{
					num = 5;
					DeviceInfo.rescueMark = 6;
				}
				if (num != -1)
				{
					if (flag3 && flag4)
					{
						DeviceInfo.rescueMark = 2;
					}
					Message.Show("K0071", "K1478", "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait();
					return num;
				}
				bool? flag5 = null;
				if (!string.IsNullOrEmpty(value))
				{
					flag5 = Convert.ToBoolean(value);
				}
				if (flag5 == true || (!flag5.HasValue && !new CheckFingerPrintVersion().Check(device, _FlashRes.fingerprint, null)))
				{
					Message.Show("K0071", "K1119", "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait();
					DegradeInfos = new Dictionary<string, string>
					{
						{ "modelName", text },
						{ "romFingerPrint", _FlashRes.fingerprint },
						{ "romMatchId", _FlashRes.romMatchId }
					};
					DeviceInfo.rescueMark = 4;
					return 1;
				}
			}
			return num;
		}
		finally
		{
			if (flag)
			{
				DeviceInfo.rescueMark = rescueMark;
			}
			else if (!ForceReMatch && num != 0)
			{
				if (!string.IsNullOrEmpty(text2))
				{
					DeviceInfo.imei = text2;
				}
				if (!string.IsNullOrEmpty(text))
				{
					DeviceInfo.modelName = text;
				}
				if (dic.ContainsKey("channelid") && !string.IsNullOrEmpty(dic["channelid"]))
				{
					DeviceInfo.channelId = dic["channelid"];
				}
				if (!string.IsNullOrEmpty(text5))
				{
					DeviceInfo.cid = text5;
				}
			}
		}
	}

	private void SubmitRescueRecord(RecipeMessage message)
	{
		RescueCollectionModel rescueCollectionModel = GengrateSubmitData(Convert.ToInt32(message.OverallResult));
		if (message.Info != null && message.Info.Count > 0)
		{
			RescueDeviceInfoModel rescueDeviceInfoModel = FlashBusiness.ConvertFastbootDeviceInfo(message.Info);
			if (ModelNameMatch)
			{
				FlashBusiness.Copy(rescueDeviceInfoModel, rescueCollectionModel);
				if (rescueCollectionModel.rescueMark == 0 && rescueDeviceInfoModel.rescueMark != 0)
				{
					rescueCollectionModel.rescueMark = rescueDeviceInfoModel.rescueMark;
				}
			}
			message.Info.TryGetValue("rescuemark", out var value);
			if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var result) && result > 0)
			{
				rescueCollectionModel.rescueMark = result;
			}
			_MatchRes.matchInfo.matchDevice = rescueDeviceInfoModel;
		}
		if (!string.IsNullOrEmpty(message.FailedStepName))
		{
			rescueCollectionModel.resultDescription = message.FailedStepName;
		}
		if (!string.IsNullOrEmpty(message.failedDescription))
		{
			string failureCode = message.failedDescription;
			if (message.failedDescription.Length >= 255)
			{
				failureCode = message.failedDescription.Substring(0, 255);
			}
			rescueCollectionModel.failureCode = failureCode;
		}
		double totalMilliseconds = DateTime.Parse(rescueCollectionModel.rescueTime).Subtract(DateTime.Parse(rescueCollectionModel.startRescueTime)).TotalMilliseconds;
		UcDevice.Log.AddLog(_MatchRes.matchInfo.ToString(), upload: true);
		if (message.OverallResult != Result.PASSED)
		{
			rescueCollectionModel.errorMsg = UcDevice.Log.UploadLogs;
			rescueCollectionModel.description = message.failedDescription;
		}
		else if (totalMilliseconds <= 90000.0)
		{
			string path = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString("N") + ".tmp");
			File.WriteAllText(path, UcDevice.Log.UploadLogs, Encoding.UTF8);
			List<string> files = new List<string> { path };
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{
					"version",
					LMSAContext.MainProcessVersion
				},
				{ "username", "RSA-Rescue-Less90" },
				{
					"crashTime",
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				}
			};
			Task.Run(() => FlashContext.SingleInstance.service.UploadAsync(WebApiUrl.RESUCE_FAILED_UPLOAD, files, dic, extraHeader: true)).ContinueWith((Task<bool> s) => GlobalFun.TryDeleteFile(path));
		}
		string recipeName = Path.GetFileName(_FlashRes.RecipeResource).Split('?')[0];
		Dictionary<string, string> degradeInfos = null;
		if (DegradeInfos != null)
		{
			degradeInfos = new Dictionary<string, string>(DegradeInfos);
		}
		RescueSubmitManager.Instance.CreateSubmitTask(UcDevice.Log.ClassGuid, rescueCollectionModel, businessType, BusinessData.Clone(businessData), recipeName, degradeInfos);
	}

	private RescueCollectionModel GengrateSubmitData(int rescueStatus)
	{
		RescueCollectionModel rescueCollectionModel = new RescueCollectionModel();
		FlashBusiness.Copy(DeviceInfo, rescueCollectionModel);
		rescueCollectionModel.rescueMark = DeviceInfo.rescueMark;
		rescueCollectionModel.version = _FlashRes.RomResources.Name;
		rescueCollectionModel.rescueResult = rescueStatus;
		rescueCollectionModel.startRescueTime = startRescueTime.ToString("yyyy-MM-dd HH:mm:ss");
		rescueCollectionModel.rescueTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		rescueCollectionModel.automatched = _MatchRes.device != null;
		rescueCollectionModel.clientUuid = GlobalFun.GetClientUUID();
		rescueCollectionModel.orderId = b2bOrderId;
		rescueCollectionModel.category = _FlashRes.Category;
		rescueCollectionModel.romMatchType = Convert.ToInt32(_MatchRes.matchInfo.matchType);
		return rescueCollectionModel;
	}

	private void RescueResultClick(bool success)
	{
		string category = _FlashRes.Category;
		string text = (_FlashRes.Brand + category).ToLowerInvariant();
		int count = FlashFailedGuideHelper.GetCount(HostProxy.User.user.UserId, DeviceInfo.modelName, success);
		if (success)
		{
			LogHelper.LogInstance.Debug($"{DeviceInfo.modelName} successcount: {count}");
			if (count != 5)
			{
				return;
			}
			RescueSuccessSubmitView vv = null;
			Application.Current.Dispatcher.Invoke(delegate
			{
				RescueSuccessSubmitView obj = new RescueSuccessSubmitView(DeviceInfo.modelName)
				{
					CloseAction = delegate(bool? r)
					{
						Message.Close(r);
					}
				};
				RescueSuccessSubmitView result = obj;
				vv = obj;
				return result;
			});
			Message.ShowCustom(vv).Wait();
			if (vv.Result.HasValue)
			{
				Message.Show("K0711", (true == vv.Result) ? "K0733" : "K0734").Wait();
			}
			return;
		}
		LogHelper.LogInstance.Debug($"{DeviceInfo.modelName} failedcount: {count}, brandcategory: {text}");
		switch (count)
		{
		case 3:
		{
			string categorycv = ((text == "motorolaphone") ? "motoPhone" : ((text == "lenovophone") ? "lenovoPhone" : "lenovoTablet"));
			RescueFailedSubmitView vv3 = null;
			Application.Current.Dispatcher.Invoke(delegate
			{
				RescueFailedSubmitView obj3 = new RescueFailedSubmitView(categorycv)
				{
					CloseAction = delegate(bool? r)
					{
						Message.Close(r);
					}
				};
				RescueFailedSubmitView result3 = obj3;
				vv3 = obj3;
				return result3;
			});
			Message.ShowCustom(vv3).Wait();
			break;
		}
		case 10:
		{
			if (!(LMSAContext.CurrentLanguage == "en-US"))
			{
				break;
			}
			RescueFailedForFreedbackView vv2 = null;
			Application.Current.Dispatcher.Invoke(delegate
			{
				RescueFailedForFreedbackView obj2 = new RescueFailedForFreedbackView
				{
					CloseAction = delegate(bool? r)
					{
						Message.Close(r);
					}
				};
				RescueFailedForFreedbackView result2 = obj2;
				vv2 = obj2;
				return result2;
			});
			Message.ShowCustom(vv2).Wait();
			break;
		}
		}
	}

	protected void ChangeRescueBtn(FlashStatusV6 status, object data = null)
	{
		switch (status)
		{
		case FlashStatusV6.Download:
			DownloadImage = DownloadImageIcon;
			FlashBtnText = "K0100";
			FlashBtnEnable = true;
			ForceReMatch = false;
			ModelNameMatch = true;
			break;
		case FlashStatusV6.Ready:
			DownloadImage = null;
			FlashBtnText = "K0102";
			FlashBtnEnable = true;
			DegradeInfos = null;
			_RescueFinished = false;
			_IsSetCountryCode = false;
			ForceReMatch = false;
			ModelNameMatch = true;
			break;
		case FlashStatusV6.Rescuing:
			FlashBtnText = "K0103";
			deleteResources.Clear();
			fileLostMessage = null;
			RescueFrame = new RescueFrameV6(this);
			View.RescueView = RescueFrame;
			RescueFrame.ChangeView(status, delegate(FrameworkElement view)
			{
				RescuingViewV6 rescuingViewV = view as RescuingViewV6;
				if (Plugin.SupportMulti)
				{
					rescuingViewV.Init(ModelName, IMEI);
				}
			});
			MainFrameV6.Instance.ChangeContainerHorizontalAlignment(HorizontalAlignment.Center);
			MainFrameV6.Instance.VM.CurrentView = View.RescueView;
			break;
		case FlashStatusV6.PASS:
			Application.Current.Dispatcher.Invoke(delegate
			{
				RescueFrame.ChangeView(status, delegate(FrameworkElement view)
				{
					RescuingSuccessViewV6 obj2 = view as RescuingSuccessViewV6;
					obj2.OkAction = delegate
					{
						Task.Run(delegate
						{
							string romFile = _FlashRes.RomResources.Name;
							if (!Plugin.SupportMulti && !string.IsNullOrEmpty(romFile) && ConfigurationManager.AppSettings["NotShowDeleteRom"]?.ToString() != "true")
							{
								DelRomView vv = null;
								Application.Current.Dispatcher.Invoke(delegate
								{
									vv = new DelRomView();
									vv.Init(romFile);
									vv.CloseAction = delegate(bool? r)
									{
										Message.Close(r);
									};
								});
								Message.ShowCustom(vv).Wait();
							}
							if (!Plugin.SupportMulti && Category != DevCategory.Smart && !(Configurations.BackupLastDateTime < DateTime.Now.AddDays(-1.0)))
							{
								RescueRestoreView rrv = null;
								Application.Current.Dispatcher.Invoke(delegate
								{
									RescueRestoreView obj3 = new RescueRestoreView
									{
										CloseAction = delegate(bool? r)
										{
											Message.Close(r);
										}
									};
									RescueRestoreView result = obj3;
									rrv = obj3;
									return result;
								});
								if (Message.ShowCustom(rrv).Result == true)
								{
									Application.Current.Dispatcher.BeginInvoke((Action)delegate
									{
										HostProxy.HostNavigation.SwitchTo("13f79fe4cfc98747c78794a943886bcd");
									});
								}
							}
							RescueResultClick(success: true);
							Application.Current.Dispatcher.Invoke(delegate
							{
								Free();
								if (Category == DevCategory.Phone)
								{
									MainFrameV6.Instance.RemoveDevice(_MatchRes.Id);
								}
								else
								{
									MainFrameV6.Instance.FormRescueSuccessViewToPrevView(View);
								}
								ChangeRescueBtn(FlashStatusV6.Ready);
							});
						});
					};
					obj2.Init(data as string, ModelName, DeviceInfo.imei);
				});
			});
			break;
		case FlashStatusV6.FAIL:
		case FlashStatusV6.QUIT:
			if (RescueFrame == null)
			{
				break;
			}
			Application.Current.Dispatcher.Invoke(delegate
			{
				RescueFrame.ChangeView(status, delegate(FrameworkElement view)
				{
					RescuingFailViewV6 obj = view as RescuingFailViewV6;
					obj.ClickAction = delegate(int opType)
					{
						Task.Run(delegate
						{
							if (status != FlashStatusV6.QUIT)
							{
								RescueResultClick(success: false);
							}
							bool? delete = null;
							if (deleteResources.Count > 0)
							{
								delete = Message.Show("K0071", fileLostMessage, "K0327", "K0208", showClose: false, MessageBoxImage.Exclamation).Result;
								Application.Current.Dispatcher.Invoke(delegate
								{
									if (delete == true)
									{
										GlobalCmdHelper.Instance.Execute(new
										{
											type = GlobalCmdType.DELETE_ROM_AFTER_RESCUE_RETRY,
											data = deleteResources
										});
									}
								});
							}
							Application.Current.Dispatcher.Invoke(delegate
							{
								Free(opType != 1);
								if (delete == false)
								{
									ChangeRescueBtn(FlashStatusV6.Ready);
									if (Category == DevCategory.Phone)
									{
										MainFrameV6.Instance.RemoveDevice(_MatchRes.Id);
									}
									else
									{
										MainFrameV6.Instance.FormRescueSuccessViewToPrevView(View);
									}
								}
								else
								{
									if (Category != 0 || !Plugin.SupportMulti)
									{
										MainFrameV6.Instance.ChangeContainerHorizontalAlignment(HorizontalAlignment.Center);
									}
									else
									{
										MainFrameV6.Instance.ChangeContainerHorizontalAlignment(HorizontalAlignment.Left);
									}
									MainFrameV6.Instance.VM.CurrentView = View as FrameworkElement;
									if (delete == true)
									{
										IsReady = false;
										ChangeRescueBtn(FlashStatusV6.Download);
									}
									else if (opType == 1)
									{
										ChangeRescueBtn(FlashStatusV6.Ready);
										if (HostProxy.HostNavigation.CurrentPluginID == "8ab04aa975e34f1ca4f9dc3a81374e2c")
										{
											OnRescue();
										}
									}
									else
									{
										ChangeRescueBtn(FlashStatusV6.Ready);
									}
								}
							});
						});
					};
					Dictionary<string, object> dictionary = data as Dictionary<string, object>;
					obj.Init(status, ModelName, IMEI, dictionary["msg"] as string, dictionary["normal"] as bool?, (bool)dictionary["retry"], Category);
				});
			});
			break;
		}
		if (!ForceReMatch)
		{
			MainFrameV6.Instance.ChangeStatus(_MatchRes.Id, status);
		}
	}

	protected void UpdateRescuingView(string message, double percentage = 0.0)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			percentage /= 100.0;
			if (percentage > 1.0)
			{
				percentage = 1.0;
			}
			if (percentage > 0.0)
			{
				MainFrameV6.Instance.VM.ChangeRescuingPercentage(_MatchRes.Id, percentage);
			}
			if (RescueFrame != null && RescueFrame.View is RescuingViewV6 rescuingViewV)
			{
				rescuingViewV.ChangeData(message, percentage);
			}
		});
	}

	protected void ReSelectCommandHandler(object args)
	{
		MainFrameV6.Instance.FromRescueViewToPreviousView();
	}

	public void Free(bool freeWorkType = true)
	{
		UcDevice.Locked = false;
		UcDevice.RealFlash = false;
		if (UcDevice.Device != null && freeWorkType)
		{
			UcDevice.Device.WorkType = DeviceWorkType.None;
		}
		RescueFrame = null;
		View.RescueView = null;
		MainFrameV6.Instance.VM.ChangeRescuingPercentage(_MatchRes.Id, 0.0);
	}

	protected void CopyCommandHandler(object args)
	{
		DeviceInfoModel data = args as DeviceInfoModel;
		Clipboard.SetDataObject(data.Item2);
		data.Item5 = true;
		Task.Run(delegate
		{
			Thread.Sleep(800);
		}).ContinueWith((Task s) => HostProxy.CurrentDispatcher.Invoke(() => data.Item5 = false));
	}
}
