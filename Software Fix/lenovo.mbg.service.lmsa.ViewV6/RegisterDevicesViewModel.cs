using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.ModelV6;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.ViewV6;

public class RegisterDevicesViewModel : NotifyBase
{
	public string Title { get; set; }

	public ObservableCollection<RegistedDevModel> RegDevArr { get; set; }

	public RegisterDevicesViewModel(RegisterDevView ui)
	{
		RegDevArr = new ObservableCollection<RegistedDevModel>();
	}

	public bool LoadLocalRegistedDev(string category)
	{
		JArray jArray = FileHelper.ReadJtokenWithAesDecrypt<JArray>(Configurations.DefaultOptionsFileName, "$.devices[?(@.userId =='" + HostProxy.User.user.UserId + "')].registeredModels");
		if (jArray == null || !jArray.HasValues)
		{
			return false;
		}
		List<JToken> list = null;
		list = ((!string.IsNullOrEmpty(category)) ? jArray.Where(delegate(JToken n)
		{
			string text = n.Value<string>("category");
			return !string.IsNullOrEmpty(text) && Regex.IsMatch(text, "^" + category + "$", RegexOptions.IgnoreCase);
		}).ToList() : jArray.ToList());
		string json = "[" + string.Join(",", list) + "]";
		List<RegistedDevModel> list2 = JsonHelper.DeserializeJson2List<RegistedDevModel>(json);
		if (list2.Count == 0)
		{
			return false;
		}
		list2.ForEach(delegate(RegistedDevModel p)
		{
			RegDevArr.Add(p);
		});
		return true;
	}

	public void RemoveDevice(RegistedDevModel device)
	{
		if (!RegDevArr.Remove(device))
		{
			return;
		}
		try
		{
			JObject jObject = FileHelper.ReadJtokenWithAesDecrypt<JObject>(Configurations.DefaultOptionsFileName, "$");
			JToken jToken = jObject.SelectToken("$.devices[?(@.userId =='" + HostProxy.User.user.UserId + "')]");
			jToken["registeredModels"] = JToken.FromObject(RegDevArr);
			FileHelper.WriteFileWithAesEncrypt(Configurations.DefaultOptionsFileName, jObject.ToString());
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Write Software Fix options.json error. Exception:" + ex);
		}
		if (!string.IsNullOrEmpty(device.id))
		{
			Task.Factory.StartNew(() => AppContext.WebApi.RequestContent(WebApiUrl.DELETE_USER_DEVICE, new Dictionary<string, List<int>> { 
			{
				"ids",
				new List<int> { int.Parse(device.id) }
			} }));
		}
	}
}
