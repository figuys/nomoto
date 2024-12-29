namespace lenovo.mbg.service.lmsa.backuprestore.Business;

public class BackupDescription
{
	public string Id { get; set; }

	public string ModelName { get; set; }

	public string AndroidVersion { get; set; }

	public string BuildNumber { get; set; }

	public string BackupDateTime { get; set; }

	public string Notes { get; set; }

	public string StoragePath { get; set; }

	public long StorageSize { get; set; }

	public string Category { get; set; }
}