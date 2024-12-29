using System.Collections.Generic;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

public class SMSContactContainer
{
	[JsonProperty("contacts")]
	public List<SMSContact> Contacts { get; set; }

	[JsonProperty("total")]
	public int Total { get; set; }
}
