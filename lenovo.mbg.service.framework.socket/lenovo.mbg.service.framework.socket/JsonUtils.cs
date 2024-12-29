using System;
using lenovo.mbg.service.common.log;
using Newtonsoft.Json;

namespace lenovo.mbg.service.framework.socket;

public class JsonUtils
{
	public static T Parse<T>(string jsonString)
	{
		try
		{
			return JsonConvert.DeserializeObject<T>(jsonString);
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Debug("Try to pase json string to object throw exception", exception);
			return default(T);
		}
	}

	public static string Stringify(object jsonObject)
	{
		try
		{
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};
			return JsonConvert.SerializeObject(jsonObject, Formatting.None, settings);
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}
}
