using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services;

public interface IConfigService
{
	Dictionary<string, string> getConfigs(string type, List<string> keys);

	string getConfig(string type, string key);
}
