using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.support.Business;

public class EnumMapping
{
	public static Dictionary<string, string> DeliveryTypeMapping = new Dictionary<string, string>
	{
		{ "UNKNOWN", "Unknown" },
		{ "BRING_IN", "Bring in" },
		{ "COURIER", "Courier" },
		{ "CRU", "Cru" },
		{ "DEPOT", "Depot" },
		{ "ADV_EXCHANGE", "Adv exchange" },
		{ "ON_SITE", "On site" },
		{ "PARTS_ONLY", "Parts only" },
		{ "TECH_SUPPORT", "Tech support" }
	};
}
