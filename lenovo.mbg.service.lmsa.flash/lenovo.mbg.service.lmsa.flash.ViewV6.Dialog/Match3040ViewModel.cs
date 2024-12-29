using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public class Match3040ViewModel : NotifyBase
{
	private string _Informaiton;

	private string _InformaitonEx;

	private bool _IsOkBtnEnable;

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

	public ImageSource DevImagePath { get; set; }

	public WarrantyInfoViewModelV6 WarrantyVm { get; set; }

	public Match3040ViewModel(Match3040View ui, ResourceResponseModel response, string number, DevCategory category, object wModel)
	{
		WarrantyInfoBaseModel warrantyInfoBaseModel = wModel as WarrantyInfoBaseModel;
		WarrantyVm = new WarrantyInfoViewModelV6(warrantyInfoBaseModel);
		string soreceLanguage = ((category == DevCategory.Phone) ? "K0079" : "K0082");
		List<ParamPropertyWithValues> paramProperties = response.ParamProperties;
		if (paramProperties != null && paramProperties.Count >= 3)
		{
			Information = HostProxy.LanguageService.Translate("K0931") + ": " + response.marketName + "\n" + HostProxy.LanguageService.Translate("K0087") + ": " + response.ModelName + "\n" + HostProxy.LanguageService.Translate(soreceLanguage) + ": " + number;
			InformationEx = string.Empty;
		}
		else
		{
			Information = HostProxy.LanguageService.Translate("K0931") + ": " + response.marketName + "\n" + HostProxy.LanguageService.Translate("K0087") + ": " + response.ModelName;
			InformationEx = HostProxy.LanguageService.Translate(soreceLanguage) + ": " + number + "\n";
		}
		if (!string.IsNullOrEmpty(warrantyInfoBaseModel?.ShipLocation))
		{
			Information = Information + "\n" + HostProxy.LanguageService.Translate("K0270") + ": " + warrantyInfoBaseModel.ShipLocation;
		}
		List<ParamPropertyWithValues> paramProperties2 = response.ParamProperties;
		if (paramProperties2 != null && paramProperties2.Count > 0)
		{
			string temp = string.Empty;
			response.ParamProperties.ForEach(delegate(ParamPropertyWithValues n)
			{
				temp = temp + HostProxy.LanguageService.Translate(n.ParamProperty.PropertyName) + ": " + n.ParamValues.First() + "\n";
			});
			InformationEx += temp.Trim('\n');
		}
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
		IsOkBtnEnable = true;
	}
}
