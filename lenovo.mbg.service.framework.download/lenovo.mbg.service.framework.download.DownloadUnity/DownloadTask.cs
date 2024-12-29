using System;
using lenovo.mbg.service.framework.download.DownloadControllerImpl;

namespace lenovo.mbg.service.framework.download.DownloadUnity;

public sealed class DownloadTask
{
	private DownloadTaskProcessor m_downloadTaskProcessor;

	private AbstractDownloadInfo m_downloadInfo { get; set; }

	public DateTime? StartTime { get; set; }

	public DateTime? EndTime { get; set; }

	public Type DownloadInfoClassType { get; set; }

	public AbstractDownloadController Controller { get; set; }

	public DownloadTaskProcessor downloadTaskProcessor
	{
		get
		{
			if (m_downloadTaskProcessor == null)
			{
				m_downloadTaskProcessor = new DownloadTaskProcessor(this);
			}
			return m_downloadTaskProcessor;
		}
	}

	public AbstractDownloadInfo DownloadInfo => m_downloadInfo;

	public long totalSizeOfSec { get; set; }

	public event EventHandler<DownloadEventArgs> OnDownloadStatusChanged;

	public event EventHandler<DownloadEventArgs> OnExistedDownloadTask;

	public event EventHandler<DownloadEventArgs> OnStopLowerControllerLevelTask;

	public DownloadTask(AbstractDownloadInfo downloadInfo, AbstractDownloadController controller)
	{
		totalSizeOfSec = 0L;
		m_downloadInfo = downloadInfo;
		Controller = controller;
		StartTime = DateTime.Now;
	}

	public void Update(AbstractDownloadInfo downloadInfo)
	{
		m_downloadInfo.downloadFileSize = downloadInfo.downloadFileSize;
		m_downloadInfo.downloadMD5 = downloadInfo.downloadMD5;
		m_downloadInfo.downloadUrl = downloadInfo.downloadUrl;
		m_downloadInfo.downloadFileName = downloadInfo.downloadFileName;
	}

	public void FireDownloadStatusChanged()
	{
		if (this.OnDownloadStatusChanged != null)
		{
			this.OnDownloadStatusChanged(this, new DownloadEventArgs(this));
		}
	}

	public void FireExistedDownloadTask(DownloadEventArgs args)
	{
		if (this.OnExistedDownloadTask != null)
		{
			Delegate[] invocationList = this.OnExistedDownloadTask.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((EventHandler<DownloadEventArgs>)invocationList[i]).BeginInvoke(this, args, null, null);
			}
		}
	}

	public void FireStopLowerControllerLevelTask()
	{
		if (this.OnStopLowerControllerLevelTask != null)
		{
			this.OnStopLowerControllerLevelTask(this, new DownloadEventArgs(this));
		}
	}
}
