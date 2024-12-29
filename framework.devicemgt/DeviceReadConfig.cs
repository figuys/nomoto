using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiServices;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.devicemgt;

public class DeviceReadConfig
{
	protected static ApiService servic = new ApiService();

	public static DeviceReadConfig Instance = new DeviceReadConfig();

	private static Task task;

	public Dictionary<string, dynamic> ModelConfigMapping { get; private set; }

	public dynamic this[string key]
	{
		get
		{
			task?.Wait();
			if (!ModelConfigMapping.ContainsKey(key))
			{
				return null;
			}
			return ModelConfigMapping[key];
		}
	}

	private DeviceReadConfig()
	{
	}

	public Task LoadTask()
	{
		ModelConfigMapping = new Dictionary<string, object>();
		task = Task.Factory.StartNew(delegate
		{
			object obj = servic.RequestContent(WebApiUrl.MODEL_READ_CONFIG, null);
			if (obj != null)
			{
				foreach (JToken item in obj as JArray)
				{
					string text = item.Value<string>("modelName");
					if (!string.IsNullOrEmpty(text) && !ModelConfigMapping.ContainsKey(text))
					{
						ModelConfigMapping.Add(text, item);
					}
				}
			}
		});
		return task;
	}

	public List<string> GetAllProps()
	{
		task?.Wait();
		List<string> result = new List<string>();
		if (ModelConfigMapping.Count > 0)
		{
			ModelConfigMapping.Values.ToList().ForEach(delegate(dynamic n)
			{
				if (n.pn != null)
				{
					result.Add(n.pn.Value);
				}
				if (n.sn != null)
				{
					result.Add(n.sn.Value);
				}
				if (n.imei != null)
				{
					result.Add(n.imei.Value);
				}
				if (n.imei2 != null)
				{
					result.Add(n.imei2.Value);
				}
			});
			string text = string.Join(",", result);
			result = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
		}
		return result;
	}
}
