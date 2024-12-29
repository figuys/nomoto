using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class SMSContactList
{
	[JsonProperty("params")]
	public List<SMSContact> Params;
}
