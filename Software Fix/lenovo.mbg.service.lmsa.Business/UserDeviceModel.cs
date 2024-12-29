using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Business;

public class UserDeviceModel
{
	[JsonProperty("userId")]
	public string UserID { get; set; }

	[JsonProperty("registeredModels")]
	public List<DeviceModel> Devices { get; set; }
}
