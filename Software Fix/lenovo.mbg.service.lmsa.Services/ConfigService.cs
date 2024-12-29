using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.Services;

public class ConfigService : IConfigService
{
	public string getConfig(string type, string key)
	{
		if (type == "AppConfig")
		{
			return ConfigurationManager.AppSettings[key];
		}
		return string.Empty;
	}

	public Dictionary<string, string> getConfigs(string type, List<string> keys)
	{
		if (type == "AppConfig")
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			NameValueCollection appSettings = ConfigurationManager.AppSettings;
			foreach (string key in keys)
			{
				dictionary.Add(key, appSettings[key]);
			}
			return dictionary;
		}
		return null;
	}
}
