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

public class SmartQMatchViewModel : NotifyBase
{
	private SmartQMatchViewV6 _View;

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

	public SmartQMatchViewModel(SmartQMatchViewV6 ui)
	{
		_View = ui;
		List<Tuple<string, string>> registerDevInfo = GetRegisterDevInfo();
		registerDevInfo?.ForEach(delegate(Tuple<string, string> p)
		{
			SnArr.Add(p);
		});
		ToggleButtonVisibility = ((registerDevInfo == null || registerDevInfo.Count <= 0) ? Visibility.Collapsed : Visibility.Visible);
	}

	public void SmartMatchBySN()
	{
		if (!ValidateSN(SearchKeyText))
		{
			SearchWarnText = "K1184";
			return;
		}
		SearchWarnText = string.Empty;
		object wModel = null;
		IsBtnEnable = false;
		Plugin.OperateTracker("SNSearchBtnClick", "User search smart firmware by SN");
		WaitTips tip = new WaitTips("K1006");
		Dictionary<string, object> aparams = new Dictionary<string, object>();
		aparams.Add("sn", SearchKeyText);
		LogHelper.LogInstance.Info("search smart firmware by SN:[" + SearchKeyText + "].");
		var task = Task.Run(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			ResponseModel<List<ResourceResponseModel>> responseModel = FlashContext.SingleInstance.service.Request<List<ResourceResponseModel>>(WebServicesContext.GET_RESOURCES_BY_SN, aparams);
			stopwatch.Stop();
			List<ResourceResponseModel> content = responseModel.content;
			BusinessStatus status = BusinessStatus.FALIED;
			if (content != null && content.Count > 0)
			{
				status = BusinessStatus.SUCCESS;
				aparams.Add("romMatchId", content[0].romMatchId);
			}
			if (responseModel.code == "0000" || responseModel.code == "3010" || responseModel.code == "3030" || responseModel.code == "3040")
			{
				wModel = MainFrameV6.Instance.LoadWarranty(SearchKeyText);
			}
			_View.Dispatcher.Invoke(delegate
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
		task.ContinueWith(param =>
		{
			dynamic result = param.Result;
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_SN_SEARCH, new BusinessData(BusinessType.RESCUE_SN_SEARCH, null).Update(result.time, result.status, aparams));
		});
		HostProxy.HostMaskLayerWrapper.New(tip, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => tip.ShowDialog());
		var result2 = task.Result;
		IsBtnEnable = true;
		if (result2.code == "3010")
		{
			if (result2.resp == null || result2.resp.Count == 0)
			{
				MainFrameV6.Instance.IMsgManager.ShowMessage("K0711", "K0113");
				return;
			}
			Match3010View userUi = new Match3010View((result2.resp[0].Category?.ToLower() == "tablet") ? DevCategory.Tablet : DevCategory.Smart, result2.resp[0].ModelName, SearchKeyText, wModel);
			MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
		}
		else if (result2.code == "3030")
		{
			DevCategory category = ((result2.resp[0].Category?.ToLower() == "tablet") ? DevCategory.Tablet : DevCategory.Smart);
			Match3030View.ProcMatch3030(null, null, _View, result2.resp[0], SearchKeyText, category, null, wModel);
		}
		else if (result2.code == "3040" || result2.code == "0000")
		{
			ResourceResponseModel resourceResponseModel = result2.resp[0];
			DevCategory category2 = ((result2.resp[0].Category?.ToLower() == "tablet") ? DevCategory.Tablet : DevCategory.Smart);
			if (true != Match3040View.ProcMatch3040(resourceResponseModel, SearchKeyText, category2, wModel))
			{
				return;
			}
			if (result2.resp.Count != 1)
			{
				resourceResponseModel = MultiRomsSelView.SelectOneFormRomArr(result2.resp);
				if (resourceResponseModel == null)
				{
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
			AutoMatchResource data = new AutoMatchResource(null, rescueDeviceInfoModel, resourceResponseModel, new MatchInfo(MatchType.SN, aparams, rescueDeviceInfoModel));
			MainFrameV6.Instance.JumptoRescueView(category2, data, wModel, _View);
		}
		else
		{
			if (!(result2.code == "1000"))
			{
				return;
			}
			HostProxy.CurrentDispatcher.Invoke(delegate
			{
				InvalidView invalidView = new InvalidView(DevCategory.Smart, 0);
				MainFrameV6.Instance.IMsgManager.ShowMessage(invalidView);
				if (true == invalidView.Result)
				{
					if (invalidView.IsManualModel)
					{
						MainFrameV6.Instance.ChangeView(PageIndex.SMART_MANUAL);
					}
					else
					{
						DeviceTutorialsDialogViewV6 userUi2 = new DeviceTutorialsDialogViewV6(isTabletOnly: true);
						MainFrameV6.Instance.IMsgManager.ShowMessage(userUi2);
					}
				}
			});
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
						if (!string.IsNullOrEmpty(text) && text.Equals("Smart", StringComparison.CurrentCultureIgnoreCase))
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
