using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services;

public interface ICheckVersion
{
	bool Check(string device, string config, Dictionary<string, string> aparams);
}
