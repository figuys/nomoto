using System;
using lenovo.mbg.service.framework.download.DownloadMode;
using lenovo.mbg.service.framework.download.ICondition;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace lenovo.mbg.service.framework.download.DownloadUnity;

public abstract class AbstractDownloadInfo
{
	[JsonIgnore]
	private string m_downloadSpeed = "0.0B/s";

	private DownloadStatus m_downloadStatus;

	public abstract string downloadUrl { get; set; }

	public abstract string downloadFileName { get; set; }

	public abstract string saveLocalPath { get; set; }

	public abstract long downloadFileSize { get; set; }

	public abstract string downloadMD5 { get; set; }

	public abstract int downloadLevel { get; set; }

	[JsonIgnore]
	public IDownload download => AnalyzeDownloadMode();

	[JsonIgnore]
	public virtual IDownloadCondition Condition => null;

	[JsonIgnore]
	public string downloadSpeed
	{
		get
		{
			return m_downloadSpeed;
		}
		set
		{
			m_downloadSpeed = value + "/s";
		}
	}

	[JsonIgnore]
	public long downloadedSize { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	public DownloadStatus downloadStatus
	{
		get
		{
			return m_downloadStatus;
		}
		set
		{
			m_downloadStatus = value;
		}
	}

	public virtual string tempFileName => downloadFileName + ".tmp";

	private IDownload AnalyzeDownloadMode()
	{
		if (downloadUrl.Trim().StartsWith("ftp", StringComparison.CurrentCultureIgnoreCase))
		{
			return new FtpDownload();
		}
		return new HttpDownload();
	}
}
