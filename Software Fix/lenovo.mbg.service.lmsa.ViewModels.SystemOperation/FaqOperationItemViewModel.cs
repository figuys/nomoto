using System.Collections.Generic;
using lenovo.mbg.service.common.utilities;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.ViewModels.SystemOperation;

public class FaqOperationItemViewModel : MouseOverMenuItemViewModel
{
	private readonly List<string> Las_Countrys = new List<string>
	{
		"AR", "BO", "CL", "CO", "CR", "DO", "EC", "SV", "GT", "HN",
		"MX", "NI", "PA", "PY", "PE", "BZ", "UY", "VE"
	};

	private readonly Dictionary<string, string> UrlMap = new Dictionary<string, string>
	{
		{ "US", "https://en-us.support.motorola.com/app/product_page/faqs/p/11395" },
		{ "GB", "https://en-gb.support.motorola.com/app/product_page/faqs/p/1449" },
		{ "BR", "https://pt-br.support.motorola.com/app/product_page/faqs/p/11395" },
		{ "IN", "https://en-in.support.motorola.com/app/product_page/faqs/p/11395" },
		{ "AU", "https://en-au.support.motorola.com/app/product_page/faqs/p/11395" },
		{ "NZ", "https://en-au.support.motorola.com/app/product_page/faqs/p/11395" },
		{ "LAS", "https://es-latam.support.motorola.com/app/product_page/faqs/p/11395" }
	};

	public FaqOperationItemViewModel()
	{
		base.Header = "K1599";
	}

	public override void ClickCommandHandler(object e)
	{
		string twoLetterISORegionName = GlobalFun.GetRegionInfo().TwoLetterISORegionName;
		if (UrlMap.ContainsKey(twoLetterISORegionName))
		{
			GlobalFun.OpenUrlByBrowser(UrlMap[twoLetterISORegionName]);
		}
		else if (Las_Countrys.Contains(twoLetterISORegionName))
		{
			GlobalFun.OpenUrlByBrowser(UrlMap["LAS"]);
		}
	}
}
