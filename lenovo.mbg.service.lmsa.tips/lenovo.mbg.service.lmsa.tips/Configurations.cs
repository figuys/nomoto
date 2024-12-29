using System;
using System.Collections.Generic;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.tips;

public class Configurations
{
	private static Dictionary<string, string> m_urlMapping;

	static Configurations()
	{
		m_urlMapping = new Dictionary<string, string>();
		string serviceInterfaceUrl = lenovo.mbg.service.common.utilities.Configurations.ServiceInterfaceUrl;
		Uri uri = new Uri(serviceInterfaceUrl);
		Dictionary<string, string> obj = new Dictionary<string, string> { { "webservice_getSubSerier", "/tipsHelper/getSerierByModelName.jhtml" } };
		new Uri(serviceInterfaceUrl);
		foreach (KeyValuePair<string, string> item in obj)
		{
			m_urlMapping[item.Key] = uri?.ToString() + item.Value;
		}
	}

	public static string GetConfig(string key)
	{
		return m_urlMapping[key];
	}
}
