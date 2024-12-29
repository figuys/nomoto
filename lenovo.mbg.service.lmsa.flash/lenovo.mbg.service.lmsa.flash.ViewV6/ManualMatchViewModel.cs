using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.smartdevice;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class ManualMatchViewModel : ViewModelBase
{
	private readonly FrameworkElement _Ui;

	private string _HelpImage;

	private string _RomFileName;

	private string _AndroidSettingText;

	private Visibility _ConfirmVisible;

	protected DevCategory _Category;

	protected Visibility _ModelMoreBtnVisible;

	protected ResourceRequestModel _ReqParams;

	protected ResourceResponseModel MatchResource;

	protected RescueDeviceInfoModel MatchDeviceInfo;

	protected DeviceEx MatchDevice;

	protected MatchType MatchType;

	private readonly RedoIfFailedTimer _ModelNameTimer;

	protected BusinessType businessMatchType;

	protected Dictionary<string, string> _CoditionMap;

	protected Dictionary<string, int> _FastbootDevUnMatchArr;

	protected List<Tuple<string, int, string>> _ModelNameTipsArr;

	protected RecipeMessageImpl Message;

	protected Stopwatch sw;

	protected string recipeName;

	protected string androidVer;

	protected BusinessData businessData;

	protected UseCaseDevice UcDevice;

	protected int fCode;

	public string HelpImage
	{
		get
		{
			return _HelpImage;
		}
		set
		{
			_HelpImage = value;
			OnPropertyChanged("HelpImage");
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

	public string AndroidSettingText
	{
		get
		{
			return _AndroidSettingText;
		}
		set
		{
			_AndroidSettingText = value;
			OnPropertyChanged("AndroidSettingText");
		}
	}

	public Visibility ConfirmVisible
	{
		get
		{
			return _ConfirmVisible;
		}
		set
		{
			_ConfirmVisible = value;
			OnPropertyChanged("ConfirmVisible");
		}
	}

	public LComboBoxViewModelV6 CbxModelNameVM { get; set; }

	public ObservableCollection<LComboBoxViewModelV6> CbxConditionArr { get; set; }

	public ObservableCollection<ResourceResponseModel> ReselRomArr { get; set; }

	public virtual void ChangeConfirmVisibile(Visibility visibile)
	{
	}

	public ManualMatchViewModel(FrameworkElement ui, DevCategory category)
	{
		_Ui = ui;
		Message = new RecipeMessageImpl(new MessageViewHelper(MainFrameV6.Instance.VM));
		_Category = category;
		_ModelNameTimer = new RedoIfFailedTimer(delegate
		{
			try
			{
				LoadModelName();
			}
			finally
			{
				CbxModelNameVM.IsDataLoading = false;
			}
		});
		_ReqParams = new ResourceRequestModel();
		_FastbootDevUnMatchArr = new Dictionary<string, int>();
		ReselRomArr = new ObservableCollection<ResourceResponseModel>();
		CbxConditionArr = new ObservableCollection<LComboBoxViewModelV6>();
		CbxModelNameVM = new LComboBoxViewModelV6
		{
			IsEditable = true,
			ComboBoxSelectedIndex = -1,
			ComboBoxWatermark = "K0725",
			SetTopClickCommand = new RoutedCommand(),
			MouseEnterCommand = new RoutedCommand(),
			SelectionChangedCommand = new RoutedCommand(),
			ComboBoxTextChangedCommand = new RoutedCommand(),
			ComboBoxMoreButtonCommand = new RoutedCommand(),
			ItemSelChangedActon = OnModelNameSelected
		};
		ConfirmVisible = Visibility.Collapsed;
	}

	public void GotoRescueView()
	{
		AutoMatchResource data = new AutoMatchResource(MatchDevice, MatchDeviceInfo, MatchResource, new MatchInfo(MatchType, _ReqParams.RequestParams, MatchDeviceInfo));
		MainFrameV6.Instance.JumptoRescueView(_Category, data, null, _Ui);
	}

	public void LoadModelName()
	{
		if (CbxModelNameVM.IsDataLoading)
		{
			return;
		}
		CbxModelNameVM.IsDataLoading = true;
		_ReqParams.AddParameter("country", GlobalFun.GetRegionInfo().EnglishName);
		_ReqParams.AddParameter("category", $"{_Category}");
		ResponseModel<DeviceModelInfoListModel> responseModel = FlashContext.SingleInstance.service.Request<DeviceModelInfoListModel>(WebServicesContext.GET_ALLMODELNAMES, _ReqParams.ParamsMapping);
		_ReqParams.Clear();
		DeviceModelInfoListModel content = responseModel.content;
		if (responseModel.code != "0000")
		{
			_ModelNameTimer.RegisterLoadDataTimer();
			_ModelNameTimer.IsLoadFailed = true;
		}
		else
		{
			_ModelNameTimer.DestoryLoadDataTimer();
			_ModelNameTimer.IsLoadFailed = false;
		}
		List<ManualComboboxViewModel> remote = new List<ManualComboboxViewModel>();
		if (content != null)
		{
			if (_Category == DevCategory.Smart)
			{
				content.ModelList?.ForEach(delegate(DeviceModelInfoModel p)
				{
					remote.Add(new ManualComboboxViewModel
					{
						ItemText = p.ModelName,
						Tag = p,
						IsMore = false
					});
				});
				content.MoreModelList?.ForEach(delegate(DeviceModelInfoModel p)
				{
					remote.Add(new ManualComboboxViewModel
					{
						ItemText = p.ModelName,
						Tag = p,
						IsMore = true
					});
				});
			}
			else
			{
				content.ModelList?.ForEach(delegate(DeviceModelInfoModel p)
				{
					remote.Add(new ManualComboboxViewModel
					{
						ItemText = p.MarketName + "\u3000" + p.ModelName,
						Tag = p,
						IsMore = false
					});
				});
				content.MoreModelList?.ForEach(delegate(DeviceModelInfoModel p)
				{
					remote.Add(new ManualComboboxViewModel
					{
						ItemText = p.MarketName + "\u3000" + p.ModelName,
						Tag = p,
						IsMore = true
					});
				});
			}
		}
		foreach (ManualComboboxViewModel item in LoadRescuedDevice($"$.modelname{_Category}"))
		{
			ManualComboboxViewModel manualComboboxViewModel = remote.FirstOrDefault((ManualComboboxViewModel p) => p.ItemText.Contains(item.ItemText));
			if (manualComboboxViewModel != null)
			{
				manualComboboxViewModel.IsUsed = true;
			}
		}
		if (_Category == DevCategory.Smart)
		{
			_ModelMoreBtnVisible = Visibility.Collapsed;
		}
		else
		{
			List<DeviceModelInfoModel> moreModelList = content.MoreModelList;
			if (moreModelList != null && moreModelList.Count > 0)
			{
				_ModelMoreBtnVisible = Visibility.Visible;
			}
			else
			{
				_ModelMoreBtnVisible = ((remote.FirstOrDefault((ManualComboboxViewModel p) => p.IsMore) == null) ? Visibility.Collapsed : Visibility.Visible);
			}
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			CbxModelNameVM.ComboBoxFilter = ModelNameInitFilter;
			CbxModelNameVM.StepComboBoxItemSource = new ObservableCollection<ManualComboboxViewModel>(remote);
			CbxModelNameVM.ComboBoxMoreButtonVisibility = _ModelMoreBtnVisible;
		});
		CbxModelNameVM.IsDataLoading = false;
	}

	protected virtual void OnModelNameSelected(object ob)
	{
		ResetForModelName();
		if (ob is ManualComboboxViewModel manualComboboxViewModel)
		{
			DeviceModelInfoModel deviceModelInfoModel = manualComboboxViewModel.Tag as DeviceModelInfoModel;
			CbxModelNameVM.IsDataLoading = true;
			DriverInstall(deviceModelInfoModel);
			LogHelper.LogInstance.Info("Model name selected, modelName:[" + deviceModelInfoModel.ModelName + "] marketName:[" + deviceModelInfoModel.MarketName + "].");
			if (deviceModelInfoModel.ReadSupport)
			{
				_ReqParams.AddParameter("modelName", deviceModelInfoModel.ModelName);
				_ReqParams.AddParameter("marketName", deviceModelInfoModel.MarketName);
				LoadFastbootData(_ReqParams.ParamsMapping);
				return;
			}
			_ReqParams.AddParameter("modelName", deviceModelInfoModel.ModelName);
			_ReqParams.AddParameter("marketName", deviceModelInfoModel.MarketName);
			businessMatchType = ((_Category == DevCategory.Tablet) ? BusinessType.RESCUE_MANUAL_TABLET_MATCH : ((_Category == DevCategory.Phone) ? BusinessType.RESCUE_MANUAL_PHONE_MATCH : BusinessType.RESCUE_MANUAL_SMART_MATCH));
			businessData = new BusinessData(businessMatchType, null);
			sw = new Stopwatch();
			sw.Start();
			ResourceMatching(CbxModelNameVM);
		}
	}

	protected void OnSubCbxSelected(LComboBoxViewModelV6 cbx, object ob)
	{
		ResetForSubSel();
		int num = CbxConditionArr.IndexOf(cbx);
		for (int num2 = CbxConditionArr.Count - 1; num2 > num; num2--)
		{
			_ReqParams.RemoveParameter(CbxConditionArr[num2].Tag);
			CbxConditionArr.RemoveAt(num2);
		}
		if (ob != null)
		{
			ManualComboboxViewModel manualComboboxViewModel = ob as ManualComboboxViewModel;
			_ReqParams.AddParameter(cbx.Tag, manualComboboxViewModel.ItemText);
			ResourceMatching(cbx);
		}
	}

	protected void MatchSuccess(ResourceResponseModel res, RescueDeviceInfoModel deviceInfo = null, DeviceEx device = null)
	{
		_CoditionMap = null;
		ChangeConfirmVisibile(Visibility.Visible);
		MatchResource = res;
		MatchDeviceInfo = deviceInfo;
		MatchDevice = device;
		MatchType = ((device == null) ? MatchType.MANUAL : MatchType.MANUAL_FASTBOOT);
		if (MatchDeviceInfo == null)
		{
			MatchDeviceInfo = new RescueDeviceInfoModel
			{
				category = $"{_Category}",
				romMatchId = MatchResource.romMatchId,
				marketName = _ReqParams.GetParameter("marketName"),
				modelName = _ReqParams.GetParameter("modelName"),
				hwCode = _ReqParams.GetParameter("hwCode"),
				country = _ReqParams.GetParameter("country"),
				simCount = _ReqParams.GetParameter("simCount"),
				memory = _ReqParams.GetParameter("memory")
			};
		}
		LogHelper.LogInstance.Info("match device info:[" + JsonHelper.SerializeObject2Json(MatchDeviceInfo) + "]");
		RomFileName = res.RomResources?.Name;
		ConfirmVisible = Visibility.Visible;
		_CoditionMap = null;
	}

	protected void ResourceMatching(LComboBoxViewModelV6 model)
	{
		Task.Run(delegate
		{
			model.IsDataLoading = true;
			string empty = string.Empty;
			ResponseModel<List<ResourceResponseModel>> responseModel = FlashContext.SingleInstance.service.Request<List<ResourceResponseModel>>(WebServicesContext.RESUCE_MANUAL_GETROM, _ReqParams.ParamsMapping);
			List<ResourceResponseModel> content = responseModel.content;
			model.IsDataLoading = false;
			if (content != null && content.Count == 1)
			{
				ResourceResponseModel resourceResponseModel = content.First();
				if (resourceResponseModel.ParamProperty == null && resourceResponseModel.ParamValues == null)
				{
					sw.Stop();
					Dictionary<string, string> extraData = new Dictionary<string, string>(_ReqParams.ParamsMapping) { ["romMatchId"] = resourceResponseModel.romMatchId };
					HostProxy.BehaviorService.Collect(businessMatchType, businessData.Update(sw.ElapsedMilliseconds, (resourceResponseModel.RomResources != null) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, _ReqParams.GetParameter("modelName"), extraData));
					if (resourceResponseModel.RomResources == null)
					{
						MainFrameV6.Instance.IMsgManager.ShowMessage(FlashStaticResources.RESUCE_CONDITION_UNMATCHED_EX);
					}
					else
					{
						MatchSuccess(resourceResponseModel);
					}
				}
				else
				{
					LComboBoxViewModelV6 cbx = new LComboBoxViewModelV6
					{
						IsEditable = false,
						ComboBoxSelectedIndex = -1,
						ComboBoxWatermark = resourceResponseModel.ParamProperty?.PropertyName,
						ComboBoxFilter = (object sender, string e) => 0
					};
					cbx.SelectionChangedCommand = new RoutedCommand(cbx.GetHashCode().ToString(), _Ui.GetType());
					cbx.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
					cbx.Tag = resourceResponseModel.ParamProperty.PropertyValue;
					cbx.ItemSelChangedActon = delegate(object ob)
					{
						OnSubCbxSelected(cbx, ob);
					};
					cbx.StepComboBoxItemSource = new ObservableCollection<ManualComboboxViewModel>();
					resourceResponseModel.ParamValues.ForEach(delegate(string p)
					{
						cbx.StepComboBoxItemSource.Add(new ManualComboboxViewModel
						{
							ItemText = p,
							Tag = p
						});
					});
					Application.Current.Dispatcher.Invoke(delegate
					{
						CbxConditionArr.Add(cbx);
					});
					string key = resourceResponseModel.ParamProperty.PropertyValue;
					Dictionary<string, string> coditionMap = _CoditionMap;
					if (coditionMap != null && coditionMap.ContainsKey(key))
					{
						ManualComboboxViewModel manualComboboxViewModel = cbx.StepComboBoxItemSource.FirstOrDefault((ManualComboboxViewModel p) => p.ItemText == _CoditionMap[key]);
						if (manualComboboxViewModel == null)
						{
							_CoditionMap = null;
							MainFrameV6.Instance.IMsgManager.ShowMessage("K0071", "K0098", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
							ResetCoditionSelUi();
						}
						else
						{
							cbx.ComboBoxSelectedValue = manualComboboxViewModel;
						}
					}
				}
			}
			else if (content != null && content.Count > 1)
			{
				ResourceResponseModel resourceResponseModel2 = MultiRomsSelView.SelectOneFormRomArr(content);
				sw.Stop();
				Dictionary<string, string> extraData2 = new Dictionary<string, string>(_ReqParams.ParamsMapping) { ["romMatchId"] = resourceResponseModel2?.romMatchId };
				HostProxy.BehaviorService.Collect(businessMatchType, businessData.Update(sw.ElapsedMilliseconds, (resourceResponseModel2 != null) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, _ReqParams.GetParameter("modelName"), extraData2));
				if (resourceResponseModel2 == null)
				{
					_CoditionMap = null;
					CbxModelNameVM.ComboBoxSelectedIndex = -1;
				}
				else
				{
					content.ForEach(delegate(ResourceResponseModel p)
					{
						ReselRomArr.Add(p);
					});
					MatchSuccess(resourceResponseModel2);
				}
			}
			else
			{
				sw.Stop();
				HostProxy.BehaviorService.Collect(businessMatchType, businessData.Update(sw.ElapsedMilliseconds, BusinessStatus.FALIED, _ReqParams.GetParameter("modelName"), _ReqParams.ParamsMapping));
				if (empty == "4000" || empty == "4010")
				{
					MainFrameV6.Instance.IMsgManager.ShowMessage("K1459");
				}
				else if (responseModel.success)
				{
					MainFrameV6.Instance.IMsgManager.ShowMessage(FlashStaticResources.DEVICE_NOT_SUPPORT);
				}
				CbxModelNameVM.ComboBoxSelectedIndex = -1;
			}
		});
	}

	protected void ResetForSubSel()
	{
		RomFileName = null;
		ReselRomArr.Clear();
		ConfirmVisible = Visibility.Collapsed;
		ChangeConfirmVisibile(Visibility.Collapsed);
	}

	protected virtual void ResetForModelName()
	{
		ResetForSubSel();
		Application.Current.Dispatcher.Invoke(delegate
		{
			CbxConditionArr.Clear();
		});
	}

	protected virtual void ResetCoditionSelUi()
	{
		CbxModelNameVM.ComboBoxSelectedValue = null;
	}

	public void ReSelectRom()
	{
		if (ReselRomArr.Count >= 2)
		{
			ResourceResponseModel resourceResponseModel = MultiRomsSelView.SelectOneFormRomArr(ReselRomArr.ToList());
			if (resourceResponseModel == null)
			{
				CbxModelNameVM.ComboBoxSelectedIndex = -1;
			}
			else
			{
				MatchSuccess(resourceResponseModel);
			}
		}
	}

	protected void ResetModelName(string modelName)
	{
		string format = HostProxy.LanguageService.Translate(FlashStaticResources.RESUCE_FASTBOOT_DEVICE_CONFIRM);
		if (MainFrameV6.Instance.IMsgManager.ShowMessage(string.Empty, string.Format(format, modelName), FlashStaticResources.FASTBOOT_MODELNAME_UNMATCH, null, isCloseBtn: true) == true)
		{
			ManualComboboxViewModel manualComboboxViewModel = CbxModelNameVM.StepComboBoxItemSource.FirstOrDefault((ManualComboboxViewModel p) => (p.Tag as DeviceModelInfoModel).ModelName == modelName);
			if (manualComboboxViewModel == null)
			{
				MainFrameV6.Instance.IMsgManager.ShowMessage(FlashStaticResources.RESUCE_FASTBOOT_CONDITION_UNMATCHED);
				CbxModelNameVM.ComboBoxSelectedIndex = -1;
			}
			else
			{
				CbxModelNameVM.IsDropDownEnabled = false;
				CbxModelNameVM.ComboBoxSelectedValue = manualComboboxViewModel;
			}
		}
		else
		{
			CbxModelNameVM.ComboBoxSelectedIndex = -1;
		}
	}

	protected void OnModelNameMoreBtnClicked(object sender, ExecutedRoutedEventArgs e)
	{
		CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
		CbxModelNameVM.ComboBoxFilter = DefaultFilter;
	}

	public virtual void RegisterDeviceAsync()
	{
		try
		{
			List<ManualComboboxViewModel> list = LoadRescuedDevice($"$.modelname{_Category}");
			ManualComboboxViewModel model = CbxModelNameVM.ComboBoxSelectedValue as ManualComboboxViewModel;
			model.IsUsed = true;
			int num = list.FindIndex((ManualComboboxViewModel n) => n.ItemText == model.ItemText);
			if (num != -1)
			{
				list.RemoveAt(num);
			}
			list.Insert(0, model);
			FileHelper.WriteJsonWithAesEncrypt(Configurations.RescueManualMatchFile, $"modelname{_Category}", list, async: true);
		}
		catch (Exception)
		{
		}
	}

	public virtual void SaveDownloadUri2LocalFile()
	{
		Dictionary<string, string> downloadInfo = new Dictionary<string, string>
		{
			{
				"category",
				$"{_Category}"
			},
			{
				"FileUrl",
				MatchResource.RomResources.URI.Split('?')[0]
			}
		};
		foreach (KeyValuePair<string, string> item in _ReqParams.ParamsMapping)
		{
			downloadInfo.Add(item.Key, item.Value);
		}
		List<Dictionary<string, string>> list = JsonHelper.DeserializeJson2ListFromFile<Dictionary<string, string>>(Configurations.DownloadedMatchPath);
		if (list == null)
		{
			list = new List<Dictionary<string, string>> { downloadInfo };
		}
		else if (list.FindIndex((Dictionary<string, string> n) => n.Values.Contains(downloadInfo["FileUrl"])) < 0)
		{
			list.Add(downloadInfo);
		}
		JsonHelper.SerializeObject2File(Configurations.DownloadedMatchPath, list);
	}

	public void LoadFastbootData(Dictionary<string, string> aparams)
	{
		Task.Run(() => FlashContext.SingleInstance.service.RequestContent<DeviceModelInfoModel>(WebServicesContext.GET_FASTBOOTDATA_RECIPE, aparams)).ContinueWith(delegate(Task<DeviceModelInfoModel> r)
		{
			if (r.Result != null)
			{
				sw = new Stopwatch();
				sw.Start();
				RecipeResources recipeResources = new RecipeResources();
				recipeResources.Add(RecipeResources.ModelName, r.Result.ModelName);
				recipeResources.Add(RecipeResources.RecipeUrl, r.Result.Recipe);
				recipeName = Path.GetFileName(r.Result.Recipe).Split('?')[0];
				LogHelper.LogInstance.Info("load fastboot data, model name:[" + r.Result.ModelName + "] recipe name:[" + recipeName + "].");
				UcDevice = new UseCaseDevice(null, r.Result.ModelName);
				UcDevice.Register(recipeResources, Message, FastbootResultMonitor);
				UseCaseRunner.Run(UseCase.LMSA_Read_Fastboot, UcDevice);
			}
		});
	}

	private object FastbootResultMonitor(RecipeMessageType messageType, object content)
	{
		RecipeMessage msg = (RecipeMessage)content;
		if (RecipeMessageType.DATA == messageType)
		{
			FireData(msg);
		}
		else if (RecipeMessageType.FINISH == messageType)
		{
			if (UcDevice.Device != null)
			{
				UcDevice.Device.WorkType = DeviceWorkType.None;
			}
			if (msg.OverallResult == Result.PASSED)
			{
				UcDevice.MessageBox.SetMainWindowDriverBtnStatus("installed");
			}
			else if (msg.OverallResult == Result.FAILED || msg.OverallResult == Result.QUIT)
			{
				CbxModelNameVM.ComboBoxSelectedIndex = -1;
				Application.Current.Dispatcher.Invoke(() => CbxModelNameVM.IsDataLoading = false);
			}
		}
		return true;
	}

	private void FireData(RecipeMessage msg)
	{
		SortedList<string, string> sortedList = null;
		sortedList = ((msg.Message is SortedList<string, string>) ? (msg.Message as SortedList<string, string>) : ((msg.Info == null) ? new SortedList<string, string>() : msg.Info));
		RescueDeviceInfoModel rescueDeviceInfoModel = FlashBusiness.ConvertFastbootDeviceInfo(sortedList);
		rescueDeviceInfoModel.category = _Category.ToString();
		sortedList.TryGetValue("androidVer", out androidVer);
		if (IsFastbootModelNameMatch(rescueDeviceInfoModel.modelName))
		{
			JObject parmas = GetParmas(rescueDeviceInfoModel, sortedList);
			OnFastbootConditionMatched(parmas, rescueDeviceInfoModel, msg.Device);
		}
	}

	private JObject GetParmas(RescueDeviceInfoModel deviceInfo, SortedList<string, string> infos)
	{
		ResourceRequestModel resourceRequestModel = new ResourceRequestModel();
		JObject jObject = new JObject();
		jObject.Add("modelName", deviceInfo.modelName);
		object obj = FlashContext.SingleInstance.service.RequestContent(WebServicesContext.RESUCE_AUTOMATCH_GETPARAMS_MAPPING, jObject);
		if (obj != null)
		{
			jObject = JObject.Parse(obj?.ToString());
		}
		if (jObject != null && jObject.HasValues)
		{
			infos.TryGetValue("androidVer", out var value);
			int.TryParse(value, out var result);
			bool flag = result >= 10;
			JArray jArray = jObject.Value<JArray>("params");
			if (jArray != null)
			{
				foreach (JToken item in jArray)
				{
					string text = item.ToString();
					string value2 = null;
					if (!(text == "blurVersion" && flag) && FlashBusiness.FastbootParamsToValue.ContainsKey(text))
					{
						infos.TryGetValue(FlashBusiness.FastbootParamsToValue[text], out value2);
						if (string.IsNullOrEmpty(value2))
						{
							value2 = ((!(text == "simCount")) ? "-1" : "Lack");
						}
						resourceRequestModel.AddParameter(text, value2);
					}
				}
			}
		}
		resourceRequestModel.ParamsMapping.Add("category", deviceInfo.category);
		return new JObject
		{
			{ "modelName", deviceInfo.modelName },
			{ "imei", deviceInfo.imei },
			{ "sn", deviceInfo.sn },
			{
				"params",
				JObject.FromObject(resourceRequestModel.ParamsMapping)
			},
			{ "matchType", 0 },
			{ "channelId", deviceInfo.channelId }
		};
	}

	private bool IsFastbootModelNameMatch(string modelname)
	{
		if (string.IsNullOrEmpty(modelname) || Regex.IsMatch(modelname, "^[0]+$"))
		{
			MainFrameV6.Instance.IMsgManager.ShowMessage("K0071", "K1478", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
			return false;
		}
		string parameter = _ReqParams.GetParameter("modelName");
		if (modelname != parameter)
		{
			ResetModelName(modelname);
			return false;
		}
		return true;
	}

	private void OnFastbootConditionMatched(JObject param, RescueDeviceInfoModel devInfo, DeviceEx device)
	{
		try
		{
			LogHelper.LogInstance.Info($"Fastboot condition match modelName:{devInfo.modelName}, fdrallowed:{devInfo.rescueMark}，securestate:{devInfo.securestate}, cid:{devInfo.cid}.");
			if (devInfo.securestate == "flashing_locked" || (!string.IsNullOrEmpty(devInfo.cid) && MainFrameV6.Instance.FastbootErrorStatusArr.Contains(devInfo.cid)))
			{
				ResetCoditionSelUi();
				MainFrameV6.Instance.IMsgManager.ShowMessage("K0071", "K1478", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
				return;
			}
			ResponseModel<List<ResourceResponseModel>> responseModel = FlashContext.SingleInstance.service.Request<List<ResourceResponseModel>>(WebServicesContext.RESUCE_AUTOMATCH_GETROM, param);
			List<ResourceResponseModel> content = responseModel.content;
			businessMatchType = BusinessType.RESCUE_MANUAL_FASTBOOT_MATCH;
			businessData = new BusinessData(businessMatchType, null);
			businessData.connectType = ConnectType.Fastboot.ToString();
			businessData.androidVersion = androidVer;
			if (responseModel.code == "0000")
			{
				ResourceResponseModel resourceResponseModel = null;
				if (content.Count > 1)
				{
					resourceResponseModel = MultiRomsSelView.SelectOneFormRomArr(content);
					if (resourceResponseModel == null)
					{
						CbxModelNameVM.ComboBoxSelectedValue = null;
						return;
					}
				}
				else
				{
					resourceResponseModel = content[0];
				}
				devInfo.cid = devInfo.cid;
				devInfo.channelId = devInfo.channelId;
				CollectFastbootMatchAsync(resourceResponseModel, param);
				FastbootCheckForDwonload(resourceResponseModel, devInfo, device);
			}
			else if (responseModel.code == "3010")
			{
				CollectFastbootMatchAsync(null, param);
				OnFastbootConditionUnMatch(devInfo.modelName, devInfo.imei);
			}
			else
			{
				LogHelper.LogInstance.Info("Manual fastboot match return code:" + responseModel.code + "!");
			}
		}
		finally
		{
			Application.Current.Dispatcher.Invoke(() => CbxModelNameVM.IsDataLoading = false);
		}
	}

	public void OnFastbootConditionUnMatch(string modelName, string imei = "")
	{
		if (string.IsNullOrEmpty(modelName))
		{
			modelName = HostProxy.deviceManager.MasterDevice?.Property.ModelName;
		}
		string key = modelName;
		if (!_FastbootDevUnMatchArr.ContainsKey(key))
		{
			_FastbootDevUnMatchArr.Add(key, 1);
		}
		else if (string.IsNullOrEmpty(modelName))
		{
			_FastbootDevUnMatchArr[key] = 1;
		}
		else
		{
			_FastbootDevUnMatchArr[key]++;
		}
		if (_FastbootDevUnMatchArr[key] > 1)
		{
			if (string.IsNullOrEmpty(imei))
			{
				imei = HostProxy.deviceManager.MasterDevice?.Property.IMEI1;
			}
			Application.Current.Dispatcher.Invoke(delegate
			{
				Match3010View userUi = new Match3010View(DevCategory.Phone, modelName, imei, null);
				MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
			});
		}
		else
		{
			MainFrameV6.Instance.IMsgManager.ShowMessage("K0711", FlashStaticResources.FASTBOOT_MANUALMATCH_FAILED_FIRST);
		}
		CbxModelNameVM.ComboBoxSelectedValue = null;
		ResetForModelName();
	}

	private void CollectFastbootMatchAsync(ResourceResponseModel res, object aparams)
	{
		Task.Run(delegate
		{
			sw.Stop();
			JObject jObject = JObject.FromObject(aparams);
			jObject["recipeName"] = recipeName;
			jObject["romMatchId"] = res?.romMatchId;
			HostProxy.BehaviorService.Collect(businessMatchType, businessData.Update(sw.ElapsedMilliseconds, (res != null) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, jObject.Value<string>("modelName"), jObject));
		});
	}

	private void FastbootCheckForDwonload(ResourceResponseModel res, RescueDeviceInfoModel devInfo, DeviceEx device)
	{
		Dictionary<string, string> aparams = new Dictionary<string, string>
		{
			{ "modelName", devInfo.modelName },
			{ "romFingerPrint", res.fingerprint },
			{ "romMatchId", res.romMatchId }
		};
		if (new CheckFingerPrintVersion().Check(devInfo.softwareVersion, res.fingerprint, aparams))
		{
			MatchSuccess(res, devInfo, device);
			return;
		}
		CbxModelNameVM.ComboBoxSelectedValue = null;
		ResetForModelName();
		MainFrameV6.Instance.IMsgManager.ShowMessage("K0071", "K1119", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
	}

	protected virtual int ModelNameInitFilter(object source, string keywords)
	{
		ManualComboboxViewModel manualComboboxViewModel = source as ManualComboboxViewModel;
		if (!manualComboboxViewModel.IsMore || manualComboboxViewModel.IsUsed)
		{
			return 0;
		}
		return 1;
	}

	protected virtual int SearchFilter(object source, string keywords)
	{
		ManualComboboxViewModel manualComboboxViewModel = source as ManualComboboxViewModel;
		if (string.IsNullOrEmpty(keywords))
		{
			CbxModelNameVM.ComboBoxMoreButtonVisibility = _ModelMoreBtnVisible;
			if (!manualComboboxViewModel.IsMore || manualComboboxViewModel.IsUsed)
			{
				return 0;
			}
			return 1;
		}
		CbxModelNameVM.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
		if (!manualComboboxViewModel.ItemText.ToLowerInvariant().Contains(keywords.ToLowerInvariant()))
		{
			return 1;
		}
		return 0;
	}

	protected virtual int DefaultFilter(object source, string keywords)
	{
		return 0;
	}

	protected List<ManualComboboxViewModel> LoadRescuedDevice(string token)
	{
		JArray jArray = FileHelper.ReadJtokenWithAesDecrypt<JArray>(Configurations.RescueManualMatchFile, token);
		if (jArray != null && jArray.HasValues)
		{
			List<ManualComboboxViewModel> list = JsonHelper.DeserializeJson2List<ManualComboboxViewModel>(jArray.ToString());
			if (list == null)
			{
				return new List<ManualComboboxViewModel>();
			}
			list.ForEach(delegate(ManualComboboxViewModel n)
			{
				n.Tag = JsonHelper.DeserializeJson2Object<DeviceModelInfoModel>(n.Tag.ToString());
			});
			return list;
		}
		return new List<ManualComboboxViewModel>();
	}

	private void DriverInstall(DeviceModelInfoModel device)
	{
		Action confrimAction = delegate
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				MainFrameV6.Instance.ShowOkWindow("K0711", FlashStaticResources.DRIVER_INSTALL_CONFIRM, "K0295");
			});
		};
		DriverType drivertype = DriverType.Lenovo;
		if (device.ReadSupport)
		{
			drivertype = DriverType.Motorola;
		}
		else if ("MTK".Equals(device.PlatForm, StringComparison.CurrentCultureIgnoreCase))
		{
			drivertype = DriverType.MTK;
		}
		else if ("Unisoc".Equals(device.PlatForm, StringComparison.CurrentCultureIgnoreCase))
		{
			drivertype = ((string.IsNullOrEmpty(device.ModelName) || !Regex.IsMatch(device.ModelName, "L19111", RegexOptions.IgnoreCase)) ? DriverType.Unisoc : DriverType.Unisoc_L19111);
		}
		if (!string.IsNullOrEmpty(device.ModelName))
		{
			if (Regex.IsMatch(device.ModelName, "SP101FU", RegexOptions.IgnoreCase))
			{
				drivertype = DriverType.PNP;
			}
			else if (Regex.IsMatch(device.ModelName, "CD-17302F", RegexOptions.IgnoreCase))
			{
				drivertype = DriverType.ADBDRIVER;
			}
		}
		string output = null;
		DriversHelper.CheckAndInstallInfDriver(drivertype, confrimAction, out output);
		if (!string.IsNullOrEmpty(output))
		{
			LogHelper.LogInstance.Info(output);
		}
	}
}
