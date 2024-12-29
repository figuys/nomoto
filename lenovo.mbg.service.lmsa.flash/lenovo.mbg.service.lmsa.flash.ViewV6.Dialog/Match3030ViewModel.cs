using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Controls.Windows;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public class Match3030ViewModel : NotifyBase
{
	private Match3030View _View;

	private FrameworkElement ParentView;

	private DevCategory _Category;

	private ResourceRequestModel _ReqParams;

	private List<ResourceResponseModel> _MatchedResArr;

	public Stopwatch sw;

	protected bool LoadWarrantyFinished;

	protected bool LoadDataFinished;

	protected DeviceEx Device;

	protected RescueDeviceInfoModel deviceInfo;

	private string _Informaiton;

	private string _InformaitonEx;

	private bool _IsOkBtnEnable;

	public ImageSource DevImagePath { get; set; }

	public WarrantyInfoViewModelV6 WarrantyVm { get; set; }

	public WarrantyInfoBaseModel warrantyInfo { get; private set; }

	public MatchType matchType
	{
		get
		{
			if (_Category != 0)
			{
				return MatchType.SN;
			}
			return MatchType.IMEI;
		}
	}

	public BusinessType businessType
	{
		get
		{
			if (matchType != MatchType.IMEI)
			{
				return BusinessType.RESCUE_SN_MATCH;
			}
			return BusinessType.RESCUE_IMEI_MATCH;
		}
	}

	public BusinessData businessData { get; private set; }

	public string MatchKeyText { get; set; }

	public string Information
	{
		get
		{
			return _Informaiton;
		}
		set
		{
			_Informaiton = value;
			OnPropertyChanged("Information");
		}
	}

	public string InformationEx
	{
		get
		{
			return _InformaitonEx;
		}
		set
		{
			_InformaitonEx = value;
			OnPropertyChanged("InformationEx");
		}
	}

	public bool IsOkBtnEnable
	{
		get
		{
			return _IsOkBtnEnable;
		}
		set
		{
			_IsOkBtnEnable = value;
			OnPropertyChanged("IsOkBtnEnable");
		}
	}

	public ObservableCollection<LComboBoxViewModelV6> CbxConditionArr { get; set; }

	public Match3030ViewModel(Match3030View ui, DeviceEx device, RescueDeviceInfoModel deviceInfo, FrameworkElement parentView, ResourceResponseModel response, string number, DevCategory category, BusinessData businessData, object wModel)
	{
		warrantyInfo = wModel as WarrantyInfoBaseModel;
		WarrantyVm = new WarrantyInfoViewModelV6(warrantyInfo);
		Device = device;
		this.deviceInfo = deviceInfo ?? new RescueDeviceInfoModel();
		ParentView = parentView;
		_Category = category;
		sw = new Stopwatch();
		sw.Start();
		if (businessData == null)
		{
			businessData = new BusinessData(businessType, null);
		}
		this.businessData = businessData;
		_View = ui;
		IsOkBtnEnable = false;
		CbxConditionArr = new ObservableCollection<LComboBoxViewModelV6>();
		Initialize(response, number, category);
		switch (category)
		{
		case DevCategory.Tablet:
			DevImagePath = Application.Current.Resources["v6_warranty_tabletnew"] as ImageSource;
			break;
		case DevCategory.Smart:
			DevImagePath = Application.Current.Resources["v6_warranty_smartnew"] as ImageSource;
			break;
		default:
			DevImagePath = Application.Current.Resources["v6_warranty_phonenew"] as ImageSource;
			break;
		}
		LoadWarrantyFinished = true;
	}

	public void Initialize(ResourceResponseModel response, string matchText, DevCategory category)
	{
		MatchKeyText = matchText;
		_ReqParams = new ResourceRequestModel();
		_ReqParams.AddParameter("modelName", response.ModelName);
		_ReqParams.AddParameter("marketName", response.marketName);
		_ReqParams.AddParameter("romMatchId", response.romMatchId);
		string str = ((category == DevCategory.Phone) ? "K0079" : "K0082");
		Information = HostProxy.LanguageService.Translate("K0931") + ": " + response.marketName + "\n" + HostProxy.LanguageService.Translate("K0087") + ": " + response.ModelName + "\n" + HostProxy.LanguageService.Translate(str) + ": " + matchText;
		if (!string.IsNullOrEmpty(warrantyInfo?.ShipLocation))
		{
			Information = Information + "\n" + HostProxy.LanguageService.Translate("K0270") + ": " + warrantyInfo.ShipLocation;
		}
		str = string.Empty;
		List<ParamPropertyWithValues> paramProperties = response.ParamProperties;
		if (paramProperties != null && paramProperties.Count > 0)
		{
			response.ParamProperties.ForEach(delegate(ParamPropertyWithValues n)
			{
				if (n.ParamValues.Count == 1)
				{
					_ReqParams.AddParameter(n.ParamProperty.PropertyValue, n.ParamValues.First());
					string text = n.ParamProperty.PropertyValue?.ToLower();
					if (text != null && (text != "imei" || text != "sn"))
					{
						str = str + HostProxy.LanguageService.Translate(n.ParamProperty.PropertyName) + ": " + n.ParamValues.First() + "\n";
					}
				}
			});
		}
		InformationEx = str.Trim('\n');
		ResourceProc(response, isDef: true);
	}

	public void CreateNextNode(ResourceResponseModel resource)
	{
		LComboBoxViewModelV6 cbx = new LComboBoxViewModelV6();
		cbx.IsEditable = false;
		cbx.ComboBoxSelectedIndex = -1;
		cbx.ComboBoxWatermark = HostProxy.LanguageService.Translate(resource.ParamProperty?.PropertyName);
		cbx.ComboBoxFilter = (object sender, string e) => 0;
		cbx.SelectionChangedCommand = new RoutedCommand(cbx.GetHashCode().ToString(), _View.GetType());
		cbx.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
		cbx.Tag = HostProxy.LanguageService.Translate(resource.ParamProperty.PropertyValue);
		cbx.ItemSelChangedActon = delegate(object ob)
		{
			OnSubCbxSelected(cbx, ob);
		};
		cbx.StepComboBoxItemSource = new ObservableCollection<ManualComboboxViewModel>();
		resource.ParamValues.ForEach(delegate(string p)
		{
			cbx.StepComboBoxItemSource.Add(new ManualComboboxViewModel
			{
				ItemText = p,
				Tag = p
			});
		});
		_View.Dispatcher.Invoke(delegate
		{
			CbxConditionArr.Add(cbx);
		});
	}

	public void CreateNextNode(ParamPropertyWithValues pro)
	{
		LComboBoxViewModelV6 cbx = new LComboBoxViewModelV6();
		cbx.IsEditable = false;
		cbx.ComboBoxSelectedIndex = -1;
		cbx.ComboBoxWatermark = HostProxy.LanguageService.Translate(pro.ParamProperty?.PropertyName);
		cbx.ComboBoxFilter = (object sender, string e) => 0;
		cbx.SelectionChangedCommand = new RoutedCommand(cbx.GetHashCode().ToString(), _View.GetType());
		cbx.ComboBoxMoreButtonVisibility = Visibility.Collapsed;
		cbx.Tag = pro.ParamProperty.PropertyValue;
		cbx.ItemSelChangedActon = delegate(object ob)
		{
			OnSubCbxSelected(cbx, ob);
		};
		cbx.StepComboBoxItemSource = new ObservableCollection<ManualComboboxViewModel>();
		pro.ParamValues.ForEach(delegate(string p)
		{
			cbx.StepComboBoxItemSource.Add(new ManualComboboxViewModel
			{
				ItemText = p,
				Tag = p
			});
		});
		_View.Dispatcher.Invoke(delegate
		{
			CbxConditionArr.Add(cbx);
		});
	}

	private void OnSubCbxSelected(LComboBoxViewModelV6 cbx, object ob)
	{
		IsOkBtnEnable = false;
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
			QueryMatchResource(cbx);
		}
	}

	private void ResourceProc(ResourceResponseModel resource, bool isDef = false)
	{
		ParamPropertyWithValues paramPropertyWithValues = resource.ParamProperties?.FirstOrDefault((ParamPropertyWithValues p) => p.ParamValues.Count != 1);
		if (paramPropertyWithValues != null)
		{
			CreateNextNode(paramPropertyWithValues);
		}
		else
		{
			QueryMatchResource(null);
		}
	}

	private void QueryMatchResource(LComboBoxViewModelV6 model)
	{
		WaitTips tips = null;
		LoadDataFinished = false;
		Application.Current.Dispatcher.Invoke(() => tips = new WaitTips("K0348"));
		Task.Run(delegate
		{
			if (_Category == DevCategory.Tablet)
			{
				_ReqParams.AddParameter("countryCode", GlobalFun.GetRegionInfo().TwoLetterISORegionName);
			}
			ResponseModel<List<ResourceResponseModel>> responseModel = FlashContext.SingleInstance.service.Request<List<ResourceResponseModel>>(WebServicesContext.RESUCE_MANUAL_GETROM, _ReqParams.ParamsMapping);
			string parameter = _ReqParams.GetParameter("modelName");
			List<ResourceResponseModel> content = responseModel.content;
			if (content != null && content.Count > 0)
			{
				ResourceResponseModel resourceResponseModel = content.First();
				Application.Current.Dispatcher.Invoke(delegate
				{
					tips.Close();
				});
				if (resourceResponseModel.ParamProperty == null && resourceResponseModel.ParamValues == null)
				{
					LoadDataFinished = true;
					sw.Stop();
					string value = _ReqParams.ParamsMapping["romMatchId"];
					_ReqParams.ParamsMapping["romMatchId"] = resourceResponseModel.romMatchId;
					BusinessData businessData = BusinessData.Clone(this.businessData);
					businessData.useCaseStep = businessType.ToString();
					HostProxy.BehaviorService.Collect(businessType, businessData.Update(sw.ElapsedMilliseconds, BusinessStatus.SUCCESS, parameter, _ReqParams.ParamsMapping));
					_ReqParams.ParamsMapping["romMatchId"] = value;
					IsOkBtnEnable = LoadWarrantyFinished;
					_MatchedResArr = content;
				}
				else
				{
					CreateNextNode(resourceResponseModel);
				}
			}
			else
			{
				sw.Stop();
				BusinessData businessData2 = BusinessData.Clone(this.businessData);
				businessData2.useCaseStep = businessType.ToString();
				HostProxy.BehaviorService.Collect(businessType, businessData2.Update(sw.ElapsedMilliseconds, BusinessStatus.FALIED, parameter, _ReqParams.ParamsMapping));
				Application.Current.Dispatcher.Invoke(delegate
				{
					tips.Close();
					_View.CloseWnd(false);
				});
				sw.Reset();
				if (responseModel.code == "4000" || responseModel.code == "4010")
				{
					MainFrameV6.Instance.IMsgManager.ShowMessage("K1459");
				}
				else
				{
					MainFrameV6.Instance.IMsgManager.ShowMessage(FlashStaticResources.DEVICE_NOT_SUPPORT);
				}
			}
		});
		Application.Current.Dispatcher.Invoke(() => tips.ShowDialog());
	}

	public void OnConfirmMatched()
	{
		ResourceResponseModel resourceResponseModel = null;
		if (_MatchedResArr.Count > 1)
		{
			resourceResponseModel = MultiRomsSelView.SelectOneFormRomArr(_MatchedResArr);
			if (resourceResponseModel == null)
			{
				return;
			}
		}
		else
		{
			resourceResponseModel = _MatchedResArr.First();
		}
		_ReqParams.ParamsMapping["romMatchId"] = resourceResponseModel.romMatchId;
		deviceInfo.romMatchId = resourceResponseModel.romMatchId;
		deviceInfo.marketName = _ReqParams.GetParameter("marketName");
		deviceInfo.modelName = _ReqParams.GetParameter("modelName");
		if (_Category == DevCategory.Phone)
		{
			deviceInfo.imei = MatchKeyText;
		}
		else
		{
			deviceInfo.sn = MatchKeyText;
		}
		FlashBusiness.ConvertDeviceInfo(_ReqParams.ParamsMapping, deviceInfo);
		AutoMatchResource data = new AutoMatchResource(Device, deviceInfo, resourceResponseModel, new MatchInfo(matchType, _ReqParams.RequestParams, deviceInfo));
		MainFrameV6.Instance.JumptoRescueView(_Category, data, warrantyInfo, ParentView);
	}
}
