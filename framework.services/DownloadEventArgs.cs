using System;

namespace lenovo.mbg.service.framework.services;

public class DownloadEventArgs : EventArgs
{
	public string downloadUrl { get; set; }

	public string downloadFileName { get; set; }

	public string saveLocalPath { get; set; }

	public long downloadFileSize { get; set; }

	public string downloadMD5 { get; set; }

	public DownloadStatus downloadStatus { get; set; }

	public int priorityLevel { get; set; }
}
