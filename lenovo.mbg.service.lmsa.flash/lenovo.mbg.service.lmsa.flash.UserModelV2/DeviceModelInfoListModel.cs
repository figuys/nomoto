using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

[Serializable]
public class DeviceModelInfoListModel
{
	[JsonProperty("models")]
	public List<DeviceModelInfoModel> ModelList { get; set; }

	[JsonProperty("moreModels")]
	public List<DeviceModelInfoModel> MoreModelList { get; set; }
}
