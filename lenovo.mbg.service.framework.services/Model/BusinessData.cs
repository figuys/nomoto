using System;
using lenovo.mbg.service.framework.services.Device;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.services.Model;

public class BusinessData
{
	public string appType { get; set; }

	public string connectType { get; set; }

	public string androidVersion { get; set; }

	public string modelName { get; set; }

	public string useCaseStep { get; set; }

	public long cycleTime { get; set; }

	public BusinessStatus status { get; set; }

	public string clientDate { get; set; }

	public JObject extraData { get; set; }

	public BusinessData()
	{
	}

	public static BusinessData Clone(BusinessData data)
	{
		return new BusinessData((BusinessType)Enum.Parse(typeof(BusinessType), data.useCaseStep), null)
		{
			appType = data.appType,
			connectType = data.connectType,
			androidVersion = data.androidVersion,
			useCaseStep = data.useCaseStep,
			status = data.status,
			clientDate = data.clientDate,
			extraData = new JObject(data.extraData),
			modelName = data.modelName,
			cycleTime = data.cycleTime
		};
	}

	public BusinessData(BusinessType subBusiness, DeviceEx device, BusinessStatus status = BusinessStatus.CLICK)
	{
		cycleTime = 0L;
		useCaseStep = subBusiness.ToString();
		this.status = status;
		appType = device?.ConnectedAppType;
		connectType = device?.ConnectType.ToString();
		androidVersion = device?.Property?.AndroidVersion;
		modelName = device?.Property?.ModelName;
		clientDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		extraData = new JObject();
		JToken value = null;
		if (device != null && device.Property != null)
		{
			value = JToken.FromObject(device.Property);
		}
		extraData.Add("device", value);
		extraData.Add("data", null);
	}

	public BusinessData Update(long useTime, BusinessStatus status, object extraData)
	{
		return Update(useTime, status, null, extraData);
	}

	public BusinessData Update(long useTime, BusinessStatus status, string modelName, object extraData)
	{
		cycleTime = useTime / 1000;
		this.status = status;
		if (!string.IsNullOrEmpty(modelName))
		{
			this.modelName = modelName;
		}
		if (extraData != null)
		{
			this.extraData["data"] = JToken.FromObject(extraData);
		}
		return this;
	}
}
