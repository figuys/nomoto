using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Contract;

[Serializable]
public class BannerInfoListModel
{
	[JsonProperty("banners")]
	public List<ResourceResponseModel> Banners { get; set; }
}
