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
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class PhoneQueryViewModelV6 : NotifyBase
{
	private PhoneQueryViewV6 _View;

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

	public ObservableCollection<Tuple<string, string>> ImeiArr { get; set; }

	public PhoneQueryViewModelV6(PhoneQueryViewV6 ui)
	{
		_View = ui;
		ImeiArr = new ObservableCollection<Tuple<string, string>>();
		List<Tuple<string, string>> registerDeviceImei = GetRegisterDeviceImei();
		registerDeviceImei?.ForEach(delegate(Tuple<string, string> p)
		{
			ImeiArr.Add(p);
		});
		ToggleButtonVisibility = ((registerDeviceImei == null || registerDeviceImei.Count <= 0) ? Visibility.Collapsed : Visibility.Visible);
	}

	public bool ValidateImei(string strImei)
	{
		if (string.IsNullOrEmpty(strImei))
		{
			return false;
		}
		if (Regex.IsMatch(strImei, "^[1-9][0-9]{13,14}$"))
		{
			if (Regex.IsMatch(strImei, "(\\d)\\1{5,15}"))
			{
				return false;
			}
			if (IsOrderNumber(strImei))
			{
				return false;
			}
			string text = strImei;
			if (text.Length > 14)
			{
				text = text.Substring(0, 14);
			}
			int num = 0;
			List<char> list = new List<char>();
			for (int i = 1; i < text.Length; i += 2)
			{
				string text2 = (int.Parse(text[i].ToString()) * 2).ToString();
				list.AddRange(text2.ToCharArray());
			}
			for (int j = 0; j < text.Length; j += 2)
			{
				list.Add(text[j]);
			}
			foreach (char item in list)
			{
				num += int.Parse(item.ToString());
			}
			num %= 10;
			if (num != 0)
			{
				num = 10 - num;
			}
			text += num;
			return text.ToLowerInvariant() == strImei.ToLowerInvariant();
		}
		return false;
	}

	private bool IsOrderNumber(string imei)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 1; i < imei.Length; i++)
		{
			num3 = imei[i] - imei[i - 1];
			num = ((num2 != num3) ? num++ : 0);
			num2 = num3;
			if (num >= 4)
			{
				return true;
			}
		}
		return false;
	}

	public void PhoneMatchByIemi()
	{
		if (!ValidateImei(SearchKeyText))
		{
			SearchWarnText = "K0938";
			return;
		}
		object wModel = null;
		IsBtnEnable = false;
		SearchWarnText = string.Empty;
		Plugin.OperateTracker("ImeiSearchBtnClick", "User search phone firmware by IMEI");
		LogHelper.LogInstance.Info("search phone firmware by IMEI:[" + SearchKeyText + "].");
		Dictionary<string, object> aparams = new Dictionary<string, object>();
		aparams.Add("imei", SearchKeyText);
		WaitTips tip = new WaitTips("K1006");
		var task = Task.Run(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			ResponseModel<List<ResourceResponseModel>> responseModel = FlashContext.SingleInstance.service.Request<List<ResourceResponseModel>>(WebServicesContext.GET_RESOURCES_BY_IMEI, aparams);
			stopwatch.Stop();
			List<ResourceResponseModel> content = responseModel.content;
			BusinessStatus status = BusinessStatus.FALIED;
			if (content != null && content.Count > 0)
			{
				status = BusinessStatus.SUCCESS;
				aparams.Add("romMatchId", content[0].romMatchId);
			}
			if (responseModel.code == "0000" || responseModel.code == "3010" || responseModel.code == "3020" || responseModel.code == "3030" || responseModel.code == "3040")
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
		task.ContinueWith(param =>
		{
			if (!Plugin.SupportMulti)
			{
				MainFrameV6.Instance.IMsgManager.CallMotoCare(SearchKeyText, wModel);
			}
		});
		HostProxy.HostMaskLayerWrapper.New(tip, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => tip.ShowDialog());
		var result = task.Result;
		IsBtnEnable = true;
		Dictionary<string, object> dictionary = new Dictionary<string, object>(aparams);
		if (result.code == "3000")
		{
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_IMEI_SEARCH, new BusinessData(BusinessType.RESCUE_IMEI_SEARCH, null).Update(result.time, BusinessStatus.FALIED, dictionary));
			MainFrameV6.Instance.IMsgManager.ShowMessage(FlashStaticResources.IMEI_INVALIDATE_TITLE, FlashStaticResources.IMEI_INVALIDATE_CONTENT);
		}
		else if (result.code == "3010")
		{
			if (result.resp == null || result.resp.Count == 0)
			{
				HostProxy.BehaviorService.Collect(BusinessType.RESCUE_IMEI_SEARCH, new BusinessData(BusinessType.RESCUE_IMEI_SEARCH, null).Update(result.time, BusinessStatus.FALIED, dictionary));
				MainFrameV6.Instance.IMsgManager.ShowMessage("K0711", "K0113");
				return;
			}
			dictionary["romMatchId"] = result.resp[0].romMatchId;
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_IMEI_SEARCH, new BusinessData(BusinessType.RESCUE_IMEI_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, result.resp[0].ModelName, dictionary));
			Match3010View userUi = new Match3010View(DevCategory.Phone, result.resp[0].ModelName, SearchKeyText, wModel);
			MainFrameV6.Instance.IMsgManager.ShowMessage(userUi);
		}
		else if (result.code == "3020")
		{
			ResourceResponseModel resourceResponseModel = result.resp[0];
			MainFrameV6.Instance.RomMatchId = resourceResponseModel.romMatchId;
			dictionary["romMatchId"] = resourceResponseModel.romMatchId;
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_IMEI_SEARCH, new BusinessData(BusinessType.RESCUE_IMEI_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, resourceResponseModel.ModelName, dictionary));
			MainFrameV6.Instance.ShowGifGuideSteps(_showTextDetect: true, resourceResponseModel.ModelName);
		}
		else if (result.code == "3030")
		{
			dictionary["romMatchId"] = result.resp[0].romMatchId;
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_IMEI_SEARCH, new BusinessData(BusinessType.RESCUE_IMEI_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, result.resp[0].ModelName, dictionary));
			Match3030View.ProcMatch3030(null, null, _View, result.resp[0], SearchKeyText, DevCategory.Phone, null, wModel);
		}
		else if (result.code == "3040" || result.code == "0000")
		{
			ResourceResponseModel resourceResponseModel2 = result.resp[0];
			if (true != Match3040View.ProcMatch3040(resourceResponseModel2, SearchKeyText, DevCategory.Phone, wModel))
			{
				dictionary["romMatchId"] = resourceResponseModel2.romMatchId;
				HostProxy.BehaviorService.Collect(BusinessType.RESCUE_IMEI_SEARCH, new BusinessData(BusinessType.RESCUE_IMEI_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, result.resp[0].ModelName, dictionary));
				return;
			}
			if (result.resp.Count != 1)
			{
				resourceResponseModel2 = MultiRomsSelView.SelectOneFormRomArr(result.resp);
				if (resourceResponseModel2 == null)
				{
					HostProxy.BehaviorService.Collect(BusinessType.RESCUE_IMEI_SEARCH, new BusinessData(BusinessType.RESCUE_IMEI_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, dictionary));
					return;
				}
			}
			RescueDeviceInfoModel rescueDeviceInfoModel = new RescueDeviceInfoModel
			{
				modelName = resourceResponseModel2.ModelName,
				imei = SearchKeyText,
				marketName = resourceResponseModel2.marketName,
				romMatchId = resourceResponseModel2.romMatchId,
				saleModel = resourceResponseModel2.SalesModel
			};
			dictionary["romMatchId"] = resourceResponseModel2.romMatchId;
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_IMEI_SEARCH, new BusinessData(BusinessType.RESCUE_IMEI_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, resourceResponseModel2.ModelName, dictionary));
			AutoMatchResource data = new AutoMatchResource(null, rescueDeviceInfoModel, resourceResponseModel2, new MatchInfo(MatchType.IMEI, aparams, rescueDeviceInfoModel));
			MainFrameV6.Instance.JumptoRescueView(DevCategory.Phone, data, wModel, _View);
		}
		else
		{
			if (!(result.code == "1000"))
			{
				return;
			}
			HostProxy.BehaviorService.Collect(BusinessType.RESCUE_IMEI_SEARCH, new BusinessData(BusinessType.RESCUE_IMEI_SEARCH, null).Update(result.time, BusinessStatus.SUCCESS, dictionary));
			InvalidView invalidView = new InvalidView(DevCategory.Phone, 1);
			MainFrameV6.Instance.IMsgManager.ShowMessage(invalidView);
			if (true != invalidView.Result)
			{
				return;
			}
			if (invalidView.IsManualModel)
			{
				MainFrameV6.Instance.ChangeView(PageIndex.PHONE_MANUAL);
				return;
			}
			_View.Dispatcher.BeginInvoke((Action)delegate
			{
				MainFrameV6.Instance.ShowGifGuideSteps(_showTextDetect: true, null);
			});
		}
	}

	private List<Tuple<string, string>> GetRegisterDeviceImei()
	{
		if (HostProxy.User.user != null)
		{
			JArray jArray = FileHelper.ReadJtokenWithAesDecrypt<JArray>(Configurations.DefaultOptionsFileName, "$.devices[?(@.userId =='" + HostProxy.User.user.UserId + "')].registeredModels");
			if (jArray != null && jArray.HasValues)
			{
				List<JToken> list = jArray.OrderBy((JToken n) => n.Value<string>("modelName")).ToList();
				if (list == null)
				{
					return null;
				}
				List<Tuple<string, string>> list2 = new List<Tuple<string, string>>();
				string empty = string.Empty;
				{
					foreach (JToken item2 in list)
					{
						string item = item2.Value<string>("modelName");
						string text = item2.Value<string>("category");
						if (!string.IsNullOrEmpty(text) && text.Equals("phone", StringComparison.CurrentCultureIgnoreCase))
						{
							empty = item2.Value<string>("imei");
							if (string.IsNullOrEmpty(empty))
							{
								empty = item2.Value<string>("imei2");
							}
							if (!string.IsNullOrEmpty(empty))
							{
								list2.Add(new Tuple<string, string>(item, empty));
							}
						}
					}
					return list2;
				}
			}
		}
		return null;
	}
}
