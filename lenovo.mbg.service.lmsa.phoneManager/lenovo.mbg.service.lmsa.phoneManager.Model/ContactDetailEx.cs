using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class ContactDetailEx
{
	[JsonProperty("id")]
	public string ContactId { get; set; }

	[JsonProperty("name")]
	public string DisplayName { get; set; }

	[JsonProperty("avatarId")]
	public string AvatarId { get; set; }

	[JsonProperty("rawContactList")]
	public List<RawContactDetail> RawContactList { get; set; }
}
