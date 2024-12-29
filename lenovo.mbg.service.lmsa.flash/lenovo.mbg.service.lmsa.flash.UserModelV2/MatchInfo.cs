using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

public class MatchInfo
{
	[JsonConverter(typeof(StringEnumConverter))]
	public MatchType matchType { get; set; }

	public object matchParams { get; set; }

	public object matchDevice { get; set; }

	public MatchInfo(MatchType matchType, object matchParams, object matchDevice)
	{
		this.matchType = matchType;
		this.matchParams = matchParams;
		this.matchDevice = matchDevice;
	}

	public override string ToString()
	{
		return JsonHelper.SerializeObject2Json(this);
	}
}
