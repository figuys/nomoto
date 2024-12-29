using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.common.utilities;

public class FileHelper
{
	private class SamePathSync
	{
		private ConcurrentDictionary<string, object> map = new ConcurrentDictionary<string, object>();

		public object GetSyncObj(string path)
		{
			object value = null;
			if (map.TryGetValue(path, out value))
			{
				return value;
			}
			value = new object();
			if (!map.TryAdd(path, value))
			{
				map.TryGetValue(path, out value);
			}
			return value;
		}
	}

	private static SamePathSync samePathSync = new SamePathSync();

	public static bool WriteFileWithAesEncrypt(string fileName, string content, FileAttributes fileAttribute = FileAttributes.ReadOnly)
	{
		string text = DefaultOptionsDirectory(fileName);
		lock (samePathSync.GetSyncObj(text))
		{
			bool result = true;
			try
			{
				if (File.Exists(text))
				{
					File.SetAttributes(text, FileAttributes.Normal);
				}
				byte[] array = Security.EncryptWithDPAPI(content);
				using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.ReadWrite))
				{
					fileStream.Write(array, 0, array.Length);
					fileStream.Flush();
				}
				File.SetAttributes(text, fileAttribute);
			}
			catch (Exception arg)
			{
				LogHelper.LogInstance.Error($"encrypt failed to file: {text},execption: {arg}");
				result = false;
			}
			return result;
		}
	}

	public static string ReadWithAesDecrypt(string fileName)
	{
		string text = DefaultOptionsDirectory(fileName);
		lock (samePathSync.GetSyncObj(text))
		{
			string result = string.Empty;
			if (File.Exists(text))
			{
				try
				{
					using MemoryStream memoryStream = new MemoryStream();
					byte[] array = new byte[4096];
					using (Stream stream = new FileStream(text, FileMode.Open, FileAccess.Read))
					{
						int count;
						while ((count = stream.Read(array, 0, array.Length)) > 0)
						{
							memoryStream.Write(array, 0, count);
						}
					}
					result = Security.DecryptWithDPAPI(memoryStream.ToArray());
				}
				catch (Exception arg)
				{
					LogHelper.LogInstance.Error($"decrypt failed from file: {text}, exception: {arg}");
				}
			}
			return result;
		}
	}

	public static string ReadWithAesDecrypt(string fileName, string key)
	{
		string text = DefaultOptionsDirectory(fileName);
		lock (samePathSync.GetSyncObj(text))
		{
			JObject jObject = JsonHelper.DeserializeJson2Jobjcet(ReadWithAesDecrypt(text));
			if (jObject == null)
			{
				return null;
			}
			if (key == null)
			{
				return jObject.ToString();
			}
			return jObject.Properties().FirstOrDefault((JProperty n) => n.Name == key)?.Value?.ToString();
		}
	}

	public static List<JToken> ReadJtokensWithAesDecrypt(string fileName, string jpath)
	{
		string text = DefaultOptionsDirectory(fileName);
		lock (samePathSync.GetSyncObj(text))
		{
			JObject jObject = JsonHelper.DeserializeJson2Jobjcet(ReadWithAesDecrypt(text));
			if (jObject == null || string.IsNullOrEmpty(jpath))
			{
				return null;
			}
			try
			{
				JToken jToken = jObject.SelectToken(jpath);
				if (jToken != null)
				{
					return new List<JToken> { jToken };
				}
				return null;
			}
			catch (Exception)
			{
				return jObject.SelectTokens(jpath)?.ToList();
			}
		}
	}

	public static T ReadJtokenWithAesDecrypt<T>(string fileName, string jpath, bool isDateAsStr = false)
	{
		string text = DefaultOptionsDirectory(fileName);
		lock (samePathSync.GetSyncObj(text))
		{
			JObject jObject = JsonHelper.DeserializeJson2Jobjcet(ReadWithAesDecrypt(text), isDateAsStr);
			if (jObject == null || string.IsNullOrEmpty(jpath))
			{
				return default(T);
			}
			try
			{
				JToken jToken = jObject.SelectToken(jpath);
				if (jToken == null)
				{
					return default(T);
				}
				return jToken.Value<T>();
			}
			catch (Exception)
			{
				return default(T);
			}
		}
	}

	public static void WriteJsonWithAesEncrypt(string fileName, Dictionary<string, object> properties, bool async = false)
	{
		Task task = Task.Factory.StartNew(delegate
		{
			string path = DefaultOptionsDirectory(fileName);
			using ThreadLocker threadLocker = new ThreadLocker(path, () => ReadWithAesDecrypt1(path, null), delegate(dynamic content)
			{
				FileHelper.WriteFileWithAesEncrypt(path, content);
			});
			dynamic val = threadLocker.Data;
			if (val == null || val["fileversion"] == null || string.IsNullOrWhiteSpace((string)val["fileversion"]))
			{
				val = new JObject();
				val["fileversion"] = 1.0;
			}
			foreach (KeyValuePair<string, object> property in properties)
			{
				if (property.Value != null)
				{
					val[property.Key] = JToken.FromObject(property.Value);
				}
			}
			threadLocker.Data = (object)val.ToString(Formatting.Indented);
		});
		if (!async)
		{
			Task.WaitAll(task);
		}
	}

	public static void WriteJsonWithAesEncrypt(string fileName, string key, object data, bool async = false)
	{
		WriteJsonWithAesEncrypt(DefaultOptionsDirectory(fileName), new Dictionary<string, object> { { key, data } });
	}

	public static string ReadText(string fileName)
	{
		string path = DefaultOptionsDirectory(fileName);
		lock (samePathSync.GetSyncObj(path))
		{
			try
			{
				return File.ReadAllText(path, Encoding.UTF8);
			}
			catch (IOException)
			{
				try
				{
					using Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
					return streamReader.ReadToEnd();
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}
	}

	public static void WriteText(string fileName, string text)
	{
		string text2 = DefaultOptionsDirectory(fileName);
		EnsureExists(text2);
		lock (samePathSync.GetSyncObj(text2))
		{
			File.WriteAllText(text2, text, Encoding.UTF8);
		}
	}

	private static dynamic ReadWithAesDecrypt1(string fileName, string key)
	{
		string text = DefaultOptionsDirectory(fileName);
		lock (samePathSync.GetSyncObj(text))
		{
			JObject jObject = JsonHelper.DeserializeJson2Jobjcet(ReadWithAesDecrypt(text));
			if (jObject == null)
			{
				return null;
			}
			if (key == null)
			{
				return jObject;
			}
			return jObject.Properties().FirstOrDefault((JProperty n) => n.Name == key)?.Value;
		}
	}

	private static void EnsureExists(string filePath)
	{
		FileInfo fileInfo = new FileInfo(filePath);
		if (!fileInfo.Directory.Exists)
		{
			Directory.CreateDirectory(fileInfo.Directory.FullName);
		}
	}

	private static string DefaultOptionsDirectory(string fileName)
	{
		return Path.Combine(Configurations.ProgramDataPath, fileName);
	}

	public static List<T> ReadJtokens<T>(string fileName, string jpath)
	{
		string text = DefaultOptionsDirectory(fileName);
		lock (samePathSync.GetSyncObj(text))
		{
			JArray jArray = JsonHelper.DeserializeJson2JArray(ReadText(fileName));
			if (jArray == null || string.IsNullOrEmpty(jpath))
			{
				return null;
			}
			try
			{
				List<T> list = new List<T>();
				foreach (JToken item in jArray.SelectTokens(jpath))
				{
					list.Add(item.ToObject<T>());
				}
				return list;
			}
			catch (Exception arg)
			{
				LogHelper.LogInstance.Error($"read json failed from file:[{text}], jpath:[{jpath}], exception:[{arg}].");
				return null;
			}
		}
	}
}
