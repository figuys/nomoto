using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class BackupStorageHead
{
	[JsonIgnore]
	public const int BackupStorageHeadByteLength = 1616;

	[JsonProperty("il")]
	public int IndexByteLength { get; set; }
}
