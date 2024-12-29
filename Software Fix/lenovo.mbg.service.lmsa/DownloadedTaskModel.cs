namespace lenovo.mbg.service.lmsa;

public class DownloadedTaskModel
{
	public string FileName { get; set; }

	public long TotalSize { get; set; }

	public DownloadItemStatus DownloadItemStatus { get; set; }

	public DownloadedTaskModel Self => this;
}
