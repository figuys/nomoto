using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Controls.Windows;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class TabletQMatchViewModelV6 : NotifyBase
{
	private TabletQMatchViewV6 _View;

	private string _SearchKeyText;

	private string _SearchWarnText;

	private bool _IsBtnEnable;

	private Visibility _ToggleButtonVisibility;

	public string SearchKeyText
	{
		get
		{
			return _SearchKeyText;
		}
		set
		{
			_SearchKeyText = value;
			OnPropertyChanged("SearchKeyText");
		}
	}

	public string SearchWarnText
	{
		get
		{
			return _SearchWarnText;
		}
		set
		{
			_SearchWarnText = value;
			OnPropertyChanged("SearchWarnText");
		}
	}

	public bool IsBtnEnable
	{
		get
		{
			return _IsBtnEnable;
		}
		set
		{
			_IsBtnEnable = value;
			OnPropertyChanged("IsBtnEnable");
		}
	}

	public Visibility ToggleButtonVisibility
	{
		get
		{
			return _ToggleButtonVisibility;
		}
		set
		{
			_ToggleButtonVisibility = value;
			OnPropertyChanged("ToggleButtonVisibility");
		}
	}

	public ObservableCollection<Tuple<string, string>> SnArr { get; set; }

	public TabletQMatchViewModelV6(TabletQMatchViewV6 ui)
	{
		_View = ui;
		SnArr = new ObservableCollection<Tuple<string, string>>();
		List<Tuple<string, string>> registerDevInfo = GetRegisterDevInfo();
		registerDevInfo?.ForEach(delegate(Tuple<string, string> p)
		{
			SnArr.Add(p);
		});
		ToggleButtonVisibility = ((registerDevInfo == null || registerDevInfo.Count <= 0) ? Visibility.Collapsed : Visibility.Visible);
	}

	public void TabletMatchBySN()
	{
		if (!ValidateSN(SearchKeyText))
		{
			SearchWarnText = "K1184";
			return;
		}
		object wModel = null;
		IsBtnEnable = false;
		SearchWarnText = string.Empty;
		Plugin.OperateTracker("SNSearchBtnClick", "User search tablet firmware by SN");
		WaitTips tip = new WaitTips("K1006");
		Dictionary<string, object> aparams = new Dictionary<string, object>();
		aparams.Add("sn", SearchKeyText);
		LogHelper.LogInstance.Info("search tablet firmware by SN:[" + SearchKeyText + "].");
		var task = Task.Run(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			ResponseModel<List<ResourceResponseModel>> responseModel = FlashContext.SingleInstance.service.Request<List<ResourceResponseModel>>(WebServicesContext.GET_RESOURCES_BY_SN, aparams);
			stopwatch.Stop();
			BusinessStatus status = BusinessStatus.FALIED;
			List<ResourceResponseModel> content = responseModel.content;
			if (content != null && content.Count > 0)
			{
				status = BusinessStatus.SUCCESS;
			}
			if (responseModel.code == "0000" || responseModel.code == "3010" || responseModel.code == "3030" || responseModel.code == "3040")
			{
				wModel = MainFrameV6.Instance.LoadWarranty(SearchKeyText);
			}
			HostProxy.CurrentDispatcher.Invoke(delegate
			{
				tip.Close();
			});
			return new
			{
				code = responseModel.code,
				resp = content,
				time = stopwatch.ElapsedMilliseconds,
				status = status
			};
		});
		HostProxy.HostMaskLayerWrapper.New(tip, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => tip.ShowDialog());
		var result = task.Result;
		IsBtnEnable = true;
		Dictionary<string, object> dictionary = new Dictionary<string, object>(aparams);
		if (result.code == "3010")
		{
			if (result.resp == null || result.resp.Count == 0)
			{
				HostProxy.BehaviorService.Collect(BusinessType.RESCUE_SN_SEARCH, new BusinessData(BusinessType.RESCUE_SN_SEARCH, null).Update(result.time, result.status, dictionary));
				MainFrameV6.Instance.IMsgManager.ShowMessage("K0711", "K0113");
				return;
			}
			dictionary["romMatchId"] = result.resp[0].romMatchId;
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_SN_SEARCH, new BusinessData(BusinessType.RESCUE_SN_SEARCH, null).Update(result.time, result.status, result.resp[0].ModelName, dictionary));
			Match3010View userUi = new Match3010View((!(result.resp[0].Category?.ToLower() == "smart")) ? DevCategory.Tablet : DevCategory.Smart, result.resp[0].ModelName, SearchKeyText, wModel);
			MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
		}
		else if (result.code == "3030")
		{
			dictionary["romMatchId"] = result.resp[0].romMatchId;
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_SN_SEARCH, new BusinessData(BusinessType.RESCUE_SN_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, result.resp[0].ModelName, dictionary));
			DevCategory category = ((!(result.resp[0].Category?.ToLower() == "smart")) ? DevCategory.Tablet : DevCategory.Smart);
			Match3030View.ProcMatch3030(null, null, _View, result.resp[0], SearchKeyText, category, null, wModel);
		}
		else if (result.code == "3040" || result.code == "0000")
		{
			ResourceResponseModel resourceResponseModel = result.resp[0];
			DevCategory category2 = ((!(result.resp[0].Category?.ToLower() == "smart")) ? DevCategory.Tablet : DevCategory.Smart);
			if (true != Match3040View.ProcMatch3040(resourceResponseModel, SearchKeyText, category2, wModel))
			{
				dictionary["romMatchId"] = result.resp[0].romMatchId;
				HostProxy.BehaviorService.Collect(BusinessType.RESCUE_SN_SEARCH, new BusinessData(BusinessType.RESCUE_SN_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, result.resp[0].ModelName, dictionary));
				return;
			}
			if (result.resp.Count != 1)
			{
				resourceResponseModel = MultiRomsSelView.SelectOneFormRomArr(result.resp);
				if (resourceResponseModel == null)
				{
					HostProxy.BehaviorService.Collect(BusinessType.RESCUE_SN_SEARCH, new BusinessData(BusinessType.RESCUE_SN_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, dictionary));
					return;
				}
			}
			RescueDeviceInfoModel rescueDeviceInfoModel = new RescueDeviceInfoModel
			{
				modelName = resourceResponseModel.ModelName,
				sn = SearchKeyText,
				marketName = resourceResponseModel.marketName,
				romMatchId = resourceResponseModel.romMatchId
			};
			dictionary["romMatchId"] = resourceResponseModel.romMatchId;
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_SN_SEARCH, new BusinessData(BusinessType.RESCUE_SN_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, resourceResponseModel.ModelName, dictionary));
			AutoMatchResource data = new AutoMatchResource(null, rescueDeviceInfoModel, resourceResponseModel, new MatchInfo(MatchType.SN, aparams, rescueDeviceInfoModel));
			MainFrameV6.Instance.JumptoRescueView(category2, data, wModel, _View);
		}
		else
		{
			if (!(result.code == "1000"))
			{
				return;
			}
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_SN_SEARCH, new BusinessData(BusinessType.RESCUE_SN_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, dictionary));
			InvalidView invalidView = new InvalidView(DevCategory.Tablet, 1);
			MainFrameV6.Instance.IMsgManager.ShowMessage(invalidView);
			if (true == invalidView.Result)
			{
				if (invalidView.IsManualModel)
				{
					MainFrameV6.Instance.ChangeView(PageIndex.TABLET_MANUAL);
					return;
				}
				DeviceTutorialsDialogViewV6 userUi2 = new DeviceTutorialsDialogViewV6(isTabletOnly: true);
				MainFrameV6.Instance.IMsgManager.ShowMessage(userUi2);
			}
		}
	}

	public bool ValidateSN(string sn)
	{
		return Regex.IsMatch(sn, "^([a-zA-Z]{1}[a-hj-np-zA-HJ-NP-Z0-9]{7})$");
	}

	private List<Tuple<string, string>> GetRegisterDevInfo()
	{
		if (HostProxy.User.user != null)
		{
			JArray jArray = FileHelper.ReadJtokenWithAesDecrypt<JArray>(Configurations.DefaultOptionsFileName, "$.devices[?(@.userId =='" + HostProxy.User.user.UserId + "')].registeredModels");
			if (jArray != null && jArray.HasValues)
			{
				string empty = string.Empty;
				List<Tuple<string, string>> list = new List<Tuple<string, string>>();
				List<JToken> list2 = jArray.OrderBy((JToken n) => n.Value<string>("modelName")).ToList();
				if (list2 == null)
				{
					return null;
				}
				{
					foreach (JToken item2 in list2)
					{
						string item = item2.Value<string>("modelName");
						string text = item2.Value<string>("category");
						if (!string.IsNullOrEmpty(text) && text.Equals("tablet", StringComparison.CurrentCultureIgnoreCase))
						{
							empty = item2.Value<string>("sn");
							if (!string.IsNullOrEmpty(empty))
							{
								list.Add(new Tuple<string, string>(item, empty));
							}
						}
					}
					return list;
				}
			}
		}
		return null;
	}
}
