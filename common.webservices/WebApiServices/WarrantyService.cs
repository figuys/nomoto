using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.Properties;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.common.webservices.WebApiServices;

public class WarrantyService : ApiService
{
	public static string SupportUrl = "https://supportapi.lenovo.com/v3/warranty/";

	public static string SdeToken = "https://microapi-us-sde.lenovo.com/token";

	public static string SdeUrl = "https://microapi-us-sde.lenovo.com/v1.0/service/poi_request";

	public static string IbaseUrl = "https://ibase.lenovo.com/POIRequest.aspx";

	protected static string Brand_Lenovo = "Lenovo";

	protected static string Brand_Motorola = "Motorola";

	public string GetSupportWarranty(string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			return null;
		}
		return RequestContent(WebApiUrl.CALL_API_URL, new
		{
			key = "WARRANTY_URL",
			param = data
		})?.ToString();
	}

	public string GetSdeWarranty(string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			return null;
		}
		data = ((data.Length > 10) ? "IMEI" : "") + data.Trim();
		object obj = RequestContent(WebApiUrl.CALL_API_URL, new
		{
			key = "POI_V1_URL",
			param = data
		});
		if (obj == null)
		{
			return null;
		}
		string text = obj.ToString();
		if (!text.Contains("errorCode"))
		{
			return text;
		}
		return string.Empty;
	}

	public string GetIbaseWarranty(string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			return null;
		}
		data = ((data.Length > 10) ? "IMEI" : "") + data.Trim();
		return RequestContent(WebApiUrl.CALL_API_URL, new
		{
			key = "POI_URL",
			param = data
		})?.ToString();
	}

	public static int GetBuyedWarranty(string region, string imei)
	{
		string key = "grant_type";
		string value = "client_credentials";
		if (RequestPostFormData("https://api-pre-mds-us.lenovo.com/auth/oauth/token", out var result, new Dictionary<string, string> { { key, value } }, Resources.Name, Resources.Code) && !string.IsNullOrEmpty(result))
		{
			string text = JsonHelper.DeserializeJson2Jobjcet(result).Value<string>("access_token");
			string body = JsonHelper.SerializeObject2Json(new Dictionary<string, object>
			{
				{ "country", region },
				{ "entitlement_flag", "ALL_ENTITLEMENTS" },
				{ "service_channel", 2 },
				{ "unit_identifier", imei },
				{ "warranty_flag", "Y" }
			});
			Dictionary<string, string> headers = new Dictionary<string, string> { 
			{
				"Authorization",
				"Bearer " + text
			} };
			JObject jObject = JsonHelper.DeserializeJson2Jobjcet(WebApiHttpRequest.Request("https://api-pre-mds-us.lenovo.com/order/order/rnt/getUnit", body, headers).content?.ToString());
			if (jObject != null)
			{
				JToken jToken = jObject.SelectToken("$.data.entitlement.device_service_contract");
				if (jToken != null && jToken is JArray)
				{
					if (JArray.FromObject(jToken).Count <= 0)
					{
						return 0;
					}
					return 1;
				}
			}
		}
		return -1;
	}

	public Task<T> GetSupportWarrantyInfoAsync<T>(string data) where T : class, new()
	{
		return Task.Run(delegate
		{
			string supportWarranty = GetSupportWarranty(data);
			if (!string.IsNullOrEmpty(supportWarranty) && !supportWarranty.Contains("Duplicated"))
			{
				JObject jObject = JsonHelper.DeserializeJson2Jobjcet(supportWarranty);
				JToken jToken = jObject.SelectToken("$.Warranties")?.OrderByDescending((JToken n) => n.Value<string>("End")).FirstOrDefault();
				string value = jObject.SelectToken("$.Serial")?.ToString();
				if (jToken != null)
				{
					string value2 = null;
					JToken jToken2 = jObject.SelectToken("$.Countries");
					if (jToken2 != null && jToken2.HasValues)
					{
						List<string> values = (from n in JArray.FromObject(jToken2)
							select n.Value<string>("Name")).ToList();
						value2 = string.Join(", ", values);
					}
					try
					{
						DateTime dateTime = DateTime.Parse(jToken.Value<string>("End"));
						DateTime dateTime2 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
						string value3 = DateTime.Parse(jToken.Value<string>("Start")).ToString("yyyy-MM-dd");
						string value4 = dateTime.ToString("yyyy-MM-dd");
						int num = (dateTime.Year - dateTime2.Year) * 12 + dateTime.Month - dateTime2.Month;
						int days = dateTime.Subtract(dateTime2).Days;
						string value5 = dateTime2.ToString("MMM.yyyy", new CultureInfo("en-US"));
						string value6 = dateTime.ToString("MMM.yyyy", new CultureInfo("en-US"));
						return JsonHelper.DeserializeJson2Object<T>(JsonHelper.SerializeObject2Json(new Dictionary<string, object>
						{
							{ "Start", value5 },
							{ "End", value6 },
							{ "MonthDiff", num },
							{ "ExpriedMonth", num },
							{ "ExpriedDays", days },
							{
								"InWarranty",
								dateTime2 <= dateTime
							},
							{ "ShipLocation", value2 },
							{ "Brand", Brand_Lenovo },
							{ "WarrantyStartDate", value3 },
							{ "WarrantyEndDate", value4 },
							{ "msn", value }
						}));
					}
					catch
					{
						return (T)null;
					}
				}
			}
			return (T)null;
		});
	}

	public Task<T> GetSdeWarrantyInfoAsync<T>(string data) where T : class, new()
	{
		return Task.Run(delegate
		{
			string sdeWarranty = GetSdeWarranty(data);
			if (!string.IsNullOrEmpty(sdeWarranty))
			{
				JObject jObject = JsonHelper.DeserializeJson2Jobjcet(sdeWarranty);
				JToken jToken = jObject.SelectToken("$.serviceInfoList[0]");
				string text = jObject.SelectToken("$.machineInfo.productName")?.ToString();
				string value = jObject.SelectToken("$.machineInfo.serialNumber")?.ToString();
				string value2 = Brand_Lenovo;
				if (!string.IsNullOrEmpty(text) && Regex.IsMatch(text, "moto", RegexOptions.IgnoreCase))
				{
					value2 = Brand_Motorola;
				}
				if (jToken != null)
				{
					try
					{
						string value3 = jToken.Value<string>("countryName");
						string value4 = jToken.Value<string>("warrantyStartDate");
						string text2 = jToken.Value<string>("warrantyEndDate");
						DateTime dateTime = DateTime.Parse(text2);
						DateTime dateTime2 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
						int num = (dateTime.Year - dateTime2.Year) * 12 + dateTime.Month - dateTime2.Month;
						int days = dateTime.Subtract(dateTime2).Days;
						string value5 = dateTime2.ToString("MMM.yyyy", new CultureInfo("en-US"));
						string value6 = dateTime.ToString("MMM.yyyy", new CultureInfo("en-US"));
						return JsonHelper.DeserializeJson2Object<T>(JsonHelper.SerializeObject2Json(new Dictionary<string, object>
						{
							{ "Start", value5 },
							{ "End", value6 },
							{ "MonthDiff", num },
							{ "ExpriedMonth", num },
							{ "ExpriedDays", days },
							{
								"InWarranty",
								dateTime2 <= dateTime
							},
							{ "ShipLocation", value3 },
							{ "Brand", value2 },
							{ "WarrantyStartDate", value4 },
							{ "WarrantyEndDate", text2 },
							{ "msn", value }
						}));
					}
					catch
					{
						return (T)null;
					}
				}
			}
			return (T)null;
		});
	}

	public Task<T> GetIbaseWarrantyInfoAsync<T>(string data) where T : class, new()
	{
		return Task.Run(delegate
		{
			string ibaseWarranty = GetIbaseWarranty(data);
			if (!string.IsNullOrEmpty(ibaseWarranty))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(ibaseWarranty);
				XmlNode xmlNode = xmlDocument.SelectSingleNode("//serviceInfo/warstart[1]");
				XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//serviceInfo/wed[1]");
				XmlNode xmlNode3 = xmlDocument.SelectSingleNode("//machineInfo/productName");
				XmlNode xmlNode4 = xmlDocument.SelectSingleNode("//serviceInfo/countryDesc[1]");
				XmlNode xmlNode5 = xmlDocument.SelectSingleNode("//machineInfo/serial");
				string value = xmlNode?.InnerText;
				string text = xmlNode2?.InnerText;
				string value2 = xmlNode4?.InnerText;
				string text2 = xmlNode3?.InnerText;
				string value3 = Brand_Lenovo;
				string value4 = xmlNode5?.InnerText;
				if (!string.IsNullOrEmpty(text2) && Regex.IsMatch(text2, "moto", RegexOptions.IgnoreCase))
				{
					value3 = Brand_Motorola;
				}
				if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(text))
				{
					try
					{
						DateTime dateTime = DateTime.Parse(text);
						DateTime dateTime2 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
						int num = (dateTime.Year - dateTime2.Year) * 12 + dateTime.Month - dateTime2.Month;
						int days = dateTime.Subtract(dateTime2).Days;
						string value5 = dateTime2.ToString("MMM.yyyy", new CultureInfo("en-US"));
						string value6 = dateTime.ToString("MMM.yyyy", new CultureInfo("en-US"));
						return JsonHelper.DeserializeJson2Object<T>(JsonHelper.SerializeObject2Json(new Dictionary<string, object>
						{
							{ "Start", value5 },
							{ "End", value6 },
							{ "MonthDiff", num },
							{ "ExpriedMonth", num },
							{ "ExpriedDays", days },
							{
								"InWarranty",
								dateTime2 <= dateTime
							},
							{ "ShipLocation", value2 },
							{ "Brand", value3 },
							{ "WarrantyStartDate", value },
							{ "WarrantyEndDate", text },
							{ "msn", value4 }
						}));
					}
					catch (Exception)
					{
						return (T)null;
					}
				}
			}
			return (T)null;
		});
	}

	public Task<T> GetWarrantyInfo<T>(string data) where T : class, new()
	{
		Task<T>[] array = new Task<T>[2]
		{
			GetSupportWarrantyInfoAsync<T>(data),
			GetSdeWarrantyInfoAsync<T>(data)
		};
		Task[] tasks = array;
		Task.WaitAll(tasks);
		return array.FirstOrDefault((Task<T> p) => p.Result != null) ?? array[0];
	}

	private static bool RequestPostFormData(string url, out string result, Dictionary<string, string> formdata, string username, string password)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		result = null;
		try
		{
			HttpClient val = new HttpClient();
			try
			{
				MultipartFormDataContent val2 = new MultipartFormDataContent();
				try
				{
					((HttpHeaders)((HttpContent)val2).Headers).Add("ContentType", "multipart/form-data;charset=UTF-8");
					if (formdata != null && formdata.Count > 0)
					{
						foreach (KeyValuePair<string, string> formdatum in formdata)
						{
							val2.Add((HttpContent)new StringContent(formdatum.Value ?? ""), formdatum.Key);
						}
					}
					if (!string.IsNullOrEmpty(username))
					{
						string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
						val.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", text);
					}
					Task<HttpResponseMessage> task = val.PostAsync(url, (HttpContent)(object)val2);
					task.Wait();
					if (task.Result.IsSuccessStatusCode)
					{
						Task<string> task2 = task.Result.Content.ReadAsStringAsync();
						task2.Wait();
						result = task2.Result;
						return true;
					}
					return false;
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("request url: " + url, exception);
			return false;
		}
	}
}
