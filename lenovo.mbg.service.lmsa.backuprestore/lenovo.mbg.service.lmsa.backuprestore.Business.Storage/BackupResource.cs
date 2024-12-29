using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Storage;

public class BackupResource
{
	[JsonIgnore]
	public const int ByteLength = 6036;

	[JsonProperty("id")]
	public int Id { get; set; }

	[JsonProperty("pd")]
	public int ParentId { get; set; }

	[JsonProperty("n")]
	public string Name { get; set; }

	[JsonProperty("v")]
	public string Value { get; set; }

	[JsonProperty("t")]
	public string Tag { get; set; }

	[JsonProperty("ass")]
	public long AssociatedStreamSize { get; set; }

	[JsonProperty("attr")]
	public Dictionary<string, string> Attributes { get; set; }

	public void AddAttribute(string key, string val)
	{
		if (Attributes == null)
		{
			Attributes = new Dictionary<string, string>();
		}
		Attributes[key] = val;
	}
}
