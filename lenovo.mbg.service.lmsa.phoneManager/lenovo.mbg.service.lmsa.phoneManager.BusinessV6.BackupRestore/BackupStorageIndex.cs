using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

internal class BackupStorageIndex
{
	public const int ByteLength = 40;

	[JsonProperty("id")]
	public int Id { get; set; }

	[JsonProperty("pd")]
	public int ParentId { get; set; }

	[JsonProperty("ro")]
	public long ResourceOffset { get; set; }

	[JsonProperty("rl")]
	public long ResourceLength { get; set; }

	[JsonProperty("rso")]
	public long ResourceStreamOffset { get; set; }

	[JsonProperty("rsl")]
	public long ResourceStreamLength { get; set; }
}
