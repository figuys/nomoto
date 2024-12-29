using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace lenovo.mbg.service.framework.services.Download;

[Serializable]
public class DownloadInfo : EventArgs, INotifyPropertyChanged
{
	[JsonIgnore]
	public Action<DownloadStatus> OnComplete;

	private bool isManualMatch;

	private string _originalFileName;

	private string _FileName;

	private string _Speed;

	private double _Progress;

	private DownloadStatus _Status;

	private long _LocalFileSize;

	private long _FileSize;

	private string _FileType;

	private string _DownloadUrl;

	private string _LocalPath;

	private bool _ShowInUI = true;

	private string _MD5;

	private string _NeedTakesTime;

	private bool _UnZip;

	private string _LocalFileSizeStr = "0MB";

	private string _FileSizeStr = "0MB";

	private string _ErrorMessage = "";

	public string FileType
	{
		get
		{
			return _FileType;
		}
		set
		{
			_FileType = FileTypeConverter(value);
			FirePropertyChangedEvent("FileType");
		}
	}

	public string FileUrl { get; private set; }

	public string DownloadUrl
	{
		get
		{
			return _DownloadUrl;
		}
		set
		{
			_DownloadUrl = value;
			FileUrl = _DownloadUrl.Split('?')[0];
			OriginalFileName = GetFileName(FileUrl);
		}
	}

	[JsonIgnore]
	public string FileName
	{
		get
		{
			return _FileName;
		}
		private set
		{
			_FileName = value;
			FirePropertyChangedEvent("FileName");
		}
	}

	public string OriginalFileName
	{
		get
		{
			return _originalFileName;
		}
		private set
		{
			_originalFileName = value;
			Regex regex = new Regex("-|\\\\|\\/|:|\\*|\\?|\\<|\\>|\"");
			FileName = regex.Replace(HttpUtility.UrlDecode(_originalFileName), "_");
		}
	}

	public string LocalPath
	{
		get
		{
			return _LocalPath;
		}
		set
		{
			_LocalPath = value;
			FirePropertyChangedEvent("LocalPath");
		}
	}

	public long FileSize
	{
		get
		{
			return _FileSize;
		}
		set
		{
			_FileSize = value;
			FileSizeStr = ConvertLong2String2(value);
			FirePropertyChangedEvent("FileSize");
		}
	}

	public string MD5
	{
		get
		{
			return _MD5;
		}
		set
		{
			_MD5 = value;
			FirePropertyChangedEvent("MD5");
		}
	}

	public bool ShowInUI
	{
		get
		{
			return _ShowInUI;
		}
		set
		{
			_ShowInUI = value;
			FirePropertyChangedEvent("ShowInUI");
		}
	}

	private DateTime _CreateDateTime { get; set; }

	public DateTime CreateDateTime
	{
		get
		{
			return _CreateDateTime;
		}
		set
		{
			_CreateDateTime = value;
		}
	}

	[JsonIgnore]
	public string Speed
	{
		get
		{
			return _Speed;
		}
		set
		{
			_Speed = value;
			FirePropertyChangedEvent("Speed");
		}
	}

	public string NeedTakesTime
	{
		get
		{
			return _NeedTakesTime;
		}
		set
		{
			_NeedTakesTime = value;
			FirePropertyChangedEvent("NeedTakesTime");
		}
	}

	[JsonIgnore]
	public double Progress
	{
		get
		{
			return _Progress;
		}
		set
		{
			_Progress = value;
			FirePropertyChangedEvent("Progress");
		}
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public DownloadStatus Status
	{
		get
		{
			return _Status;
		}
		set
		{
			_Status = value;
			FirePropertyChangedEvent("Status");
		}
	}

	[JsonIgnore]
	public long LocalFileSize
	{
		get
		{
			return _LocalFileSize;
		}
		set
		{
			_LocalFileSize = value;
			LocalFileSizeStr = ConvertLong2String2(value);
			FirePropertyChangedEvent("LocalFileSize");
		}
	}

	public bool UnZip
	{
		get
		{
			return _UnZip;
		}
		set
		{
			_UnZip = value;
			FirePropertyChangedEvent("UnZip");
		}
	}

	public string ZipPwd { get; set; }

	[JsonIgnore]
	public string LocalFileSizeStr
	{
		get
		{
			return _LocalFileSizeStr;
		}
		set
		{
			_LocalFileSizeStr = value;
			FirePropertyChangedEvent("LocalFileSizeStr");
		}
	}

	[JsonIgnore]
	public string FileSizeStr
	{
		get
		{
			return _FileSizeStr;
		}
		set
		{
			_FileSizeStr = value;
			FirePropertyChangedEvent("FileSizeStr");
		}
	}

	[JsonIgnore]
	public string ErrorMessage
	{
		get
		{
			return _ErrorMessage;
		}
		set
		{
			_ErrorMessage = value;
			FirePropertyChangedEvent("ErrorMessage");
		}
	}

	public bool IsManualMatch
	{
		get
		{
			return isManualMatch;
		}
		set
		{
			isManualMatch = value;
			FirePropertyChangedEvent("IsManualMatch");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public virtual bool CanDownload()
	{
		return true;
	}

	public DownloadInfo()
	{
		Speed = "0KB/S";
		Progress = 0.0;
		FileType = "UNKNOWN";
		CreateDateTime = DateTime.Now;
	}

	public DownloadInfo(string FileUrl, string LocalPath, long FileSize, string MD5, string FileType = "UNKNOWN")
		: this(FileUrl, LocalPath, FileSize, MD5, FileType, null)
	{
	}

	public DownloadInfo(string FileUrl, string LocalPath, long FileSize, string MD5, string FileType, Action<DownloadStatus> OnComplete, bool ShowInUI = false)
	{
		this.FileType = FileType;
		DownloadUrl = FileUrl;
		this.LocalPath = LocalPath;
		this.FileSize = FileSize;
		this.MD5 = MD5;
		CreateDateTime = DateTime.Now;
		this.ShowInUI = true;
		Speed = "0KB/S";
		Progress = 0.0;
		this.OnComplete = OnComplete;
	}

	public void FireComplete(DownloadStatus status)
	{
		try
		{
			if (OnComplete != null)
			{
				OnComplete.BeginInvoke(status, null, null);
			}
		}
		catch (Exception)
		{
		}
	}

	protected string GetFileName(string fileUrl)
	{
		string[] array = Regex.Split(fileUrl, "\\\\|/");
		if (array != null && array.Length != 0)
		{
			return array[array.Length - 1];
		}
		return fileUrl;
	}

	protected void FirePropertyChangedEvent(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	private string FileTypeConverter(string data)
	{
		string result = "UNKNOWN";
		if (string.IsNullOrEmpty(data))
		{
			return result;
		}
		switch (data.ToUpper())
		{
		case "APK":
			return "APK";
		case "ICON":
			return "ICON";
		case "ROM":
			return "ROM";
		case "TOOL":
			return "TOOL";
		case "COUNTRYCODE":
		case "COUNTRY_CODE":
			return "COUNTRYCODE";
		case "JSON":
			return "JSON";
		case "BANNER":
		case "BANNER_ICON":
			return "BANNER";
		case "XAML":
			return "XAML";
		default:
			return "UNKNOWN";
		}
	}

	private string ConvertLong2String2(long bytes)
	{
		string text = "F1";
		float num = bytes;
		if (bytes == 0L)
		{
			return "0MB";
		}
		if (bytes > 1000)
		{
			if (bytes >= 1024000)
			{
				if (bytes >= 1024000000)
				{
					return (num / 1.0737418E+09f).ToString(text) + "GB";
				}
				return (num / 1048576f).ToString(text) + "MB";
			}
			return (num / 1024f).ToString(text) + "KB";
		}
		return bytes + "B";
	}
}
