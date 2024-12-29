using System.Collections.Generic;
using lenovo.mbg.service.framework.socket;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class AddOrEditContactModel
{
	[JsonProperty("editMode")]
	public string EditMode { get; set; }

	[JsonProperty("contactDetail")]
	public ContactDetailEx ContactDetail { get; set; }

	[JsonProperty("avatars")]
	public List<PropItem> Avatars { get; set; }
}
