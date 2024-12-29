using System;
using System.Collections.Generic;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

[Serializable]
public class ResourceRequestModel
{
	private readonly Dictionary<string, string> _parametersMapping;

	public Dictionary<string, string> ParamsMapping => _parametersMapping;

	public string RequestParams => JsonHelper.SerializeObject2Json(_parametersMapping);

	public ResourceRequestModel()
	{
		_parametersMapping = new Dictionary<string, string>();
	}

	public void AddParameter(string key, string value)
	{
		_parametersMapping[key] = value;
	}

	public void Clear()
	{
		_parametersMapping.Clear();
	}

	public void RemoveParameter(string key)
	{
		if (_parametersMapping.ContainsKey(key))
		{
			_parametersMapping.Remove(key);
		}
	}

	public string GetParameter(string key)
	{
		if (_parametersMapping.ContainsKey(key))
		{
			return _parametersMapping[key];
		}
		return null;
	}
}
