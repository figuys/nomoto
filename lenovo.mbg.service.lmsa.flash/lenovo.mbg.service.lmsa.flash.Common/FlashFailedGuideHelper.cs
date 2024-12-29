using System;
using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.common.utilities;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class FlashFailedGuideHelper
{
	public static FlashDeviceData CollectFalshResult(string customerid, string modelname, bool success)
	{
		if (string.IsNullOrEmpty(customerid) || string.IsNullOrEmpty(modelname))
		{
			return new FlashDeviceData
			{
				failed = 0,
				success = 0
			};
		}
		JArray jArray = FileHelper.ReadJtokenWithAesDecrypt<JArray>(Configurations.RescueRecordsFile, "$.flash");
		List<CustomerFlashData> list = new List<CustomerFlashData>();
		if (jArray != null && jArray.HasValues)
		{
			list = JsonHelper.DeserializeJson2List<CustomerFlashData>(jArray.ToString());
		}
		CustomerFlashData customerFlashData = list.Where((CustomerFlashData n) => n.customerid == customerid)?.FirstOrDefault();
		if (customerFlashData == null)
		{
			customerFlashData = new CustomerFlashData
			{
				customerid = customerid,
				devices = new List<FlashDeviceData>()
			};
			list.Add(customerFlashData);
		}
		FlashDeviceData flashDeviceData = customerFlashData.devices.FirstOrDefault((FlashDeviceData n) => !string.IsNullOrEmpty(n.modelname) && n.modelname.Equals(modelname, StringComparison.CurrentCultureIgnoreCase));
		DateTime now = DateTime.Now;
		if (flashDeviceData == null)
		{
			flashDeviceData = new FlashDeviceData
			{
				modelname = modelname,
				failed = 0,
				success = 0
			};
			if (success)
			{
				flashDeviceData.successfirst = now;
				flashDeviceData.successlast = now;
			}
			else
			{
				flashDeviceData.failedfirst = now;
				flashDeviceData.failedlast = now;
			}
			customerFlashData.devices.Add(flashDeviceData);
		}
		else if (success)
		{
			flashDeviceData.successlast = now;
			if (flashDeviceData.successminutes >= FlashDeviceData.RESET_MINUTES)
			{
				flashDeviceData.success = 0;
				flashDeviceData.successfirst = now;
			}
		}
		else
		{
			flashDeviceData.failedlast = now;
			if (flashDeviceData.failedminutes >= FlashDeviceData.RESET_MINUTES)
			{
				flashDeviceData.failed = 0;
				flashDeviceData.failedfirst = now;
			}
		}
		if (success)
		{
			flashDeviceData.success++;
		}
		else
		{
			flashDeviceData.failed++;
		}
		FileHelper.WriteJsonWithAesEncrypt(Configurations.RescueRecordsFile, "flash", list);
		return flashDeviceData;
	}

	public static int GetCount(string customerid, string modelname, bool success)
	{
		JObject jObject = FileHelper.ReadJtokenWithAesDecrypt<JObject>(Configurations.RescueRecordsFile, "$.flash[?(@.customerid=='" + customerid + "')].devices[?(@.modelname=='" + modelname + "')]");
		if (jObject == null || !jObject.HasValues)
		{
			return 0;
		}
		try
		{
			if (success)
			{
				return jObject.Value<int>("success");
			}
			return jObject.Value<int>("failed");
		}
		catch
		{
			return 0;
		}
	}

	public static void Show(string category, bool success)
	{
	}
}
