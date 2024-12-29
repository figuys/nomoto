using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using lenovo.mbg.service.common.log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.common.utilities;

public class JsonHelper
{
	public static string SerializeObject2Json(object obj)
	{
		try
		{
			return JsonConvert.SerializeObject(obj);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("serialize object to json failed, exception: " + ex);
			return string.Empty;
		}
	}

	public static string SerializeObject2JsonExceptNull(object obj)
	{
		try
		{
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};
			return JsonConvert.SerializeObject(obj, settings);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("serialize object to json failed, exception: " + ex);
			return string.Empty;
		}
	}

	public static string SerializeObject2FormatJson(object obj)
	{
		try
		{
			return JsonConvert.SerializeObject(obj, Formatting.Indented);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("serialize object to json failed, exception: " + ex);
			return string.Empty;
		}
	}

	public static T DeserializeJson2Object<T>(string json) where T : class, new()
	{
		if (string.IsNullOrEmpty(json))
		{
			return null;
		}
		try
		{
			return JsonConvert.DeserializeObject<T>(json);
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"deserialize json to object failed, json: [{json}]. Exception: {arg}");
			return null;
		}
	}

	public static List<T> DeserializeJson2List<T>(string json) where T : class, new()
	{
		if (string.IsNullOrEmpty(json))
		{
			return null;
		}
		try
		{
			return JsonConvert.DeserializeObject<List<T>>(json);
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"deserialize json to list failed, json: [{json}]. Exception: {arg}");
			return null;
		}
	}

	public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
	{
		if (string.IsNullOrEmpty(json))
		{
			return default(T);
		}
		try
		{
			return JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"deserialize anonymous type failed, json: [{json}]. Exception: {arg} ");
			return default(T);
		}
	}

	public static object DeserializeJson2Object(string json)
	{
		if (string.IsNullOrEmpty(json))
		{
			return null;
		}
		try
		{
			return JsonConvert.DeserializeObject(json);
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"deserialize json to object failed, json: [{json}]. Exception: {arg}");
			return null;
		}
	}

	public static bool SerializeObject2File(string filePath, object obj)
	{
		try
		{
			using (StreamWriter textWriter = File.CreateText(filePath))
			{
				JsonSerializer jsonSerializer = new JsonSerializer();
				jsonSerializer.Formatting = Formatting.Indented;
				jsonSerializer.Serialize(textWriter, obj);
			}
			return true;
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"serialize object to file failed, filePath: [{filePath}] exception: {arg}");
			return false;
		}
	}

	public static T DeserializeJson2ObjcetFromFile<T>(string path) where T : class
	{
		try
		{
			if (!File.Exists(path))
			{
				return null;
			}
			using StreamReader reader = File.OpenText(path);
			return new JsonSerializer().Deserialize(reader, typeof(T)) as T;
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"deserialize json to objcet from file failed, filePath: [{path}] exception: {arg}");
			return null;
		}
	}

	public static List<T> DeserializeJson2ListFromFile<T>(string path)
	{
		try
		{
			if (!File.Exists(path))
			{
				return null;
			}
			using StreamReader reader = File.OpenText(path);
			return new JsonSerializer().Deserialize(reader, typeof(List<T>)) as List<T>;
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"deserialize json to list from file failed, filePath: [{path}] exception: {arg}");
			return null;
		}
	}

	public static JObject DeserializeJson2Jobjcet(string json, bool isDateAsStr = false)
	{
		if (string.IsNullOrEmpty(json))
		{
			return null;
		}
		try
		{
			if (isDateAsStr)
			{
				return (JObject)JsonConvert.DeserializeObject(json, new JsonSerializerSettings
				{
					DateParseHandling = DateParseHandling.None
				});
			}
			return (JObject)JsonConvert.DeserializeObject(json);
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"deserialize json to jobject failed, json: [{json}] Exception: {arg}");
			return null;
		}
	}

	public static JArray DeserializeJson2JArray(string json)
	{
		if (string.IsNullOrEmpty(json))
		{
			return null;
		}
		try
		{
			return (JArray)JsonConvert.DeserializeObject(json);
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"deserialize json to jarray failed, json: [{json}] Exception: {arg}");
			return null;
		}
	}

	public static string ReadText(string filepath)
	{
		try
		{
			return File.ReadAllText(filepath, Encoding.UTF8);
		}
		catch (Exception)
		{
			using Stream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
			return streamReader.ReadToEnd();
		}
	}

	public static void WriteText(string filepath, string content)
	{
		FileInfo fileInfo = new FileInfo(filepath);
		if (!fileInfo.Directory.Exists)
		{
			Directory.CreateDirectory(fileInfo.Directory.FullName);
		}
		File.WriteAllText(filepath, content, Encoding.UTF8);
	}
}
