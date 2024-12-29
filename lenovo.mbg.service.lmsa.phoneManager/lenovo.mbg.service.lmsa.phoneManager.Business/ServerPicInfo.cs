using System;
using System.IO;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class ServerPicInfo : BaseNotify
{
	private string _virtualFileName = string.Empty;

	private DateTime _midifiedDateTime = DateTime.MinValue;

	private long _RawFileSize;

	private string _ImageType;

	[JsonProperty("id")]
	public string Id { get; set; }

	public string RawAlbumPath { get; set; }

	public string RawFileName { get; set; }

	[JsonProperty("FileName")]
	public string VirtualFileName
	{
		get
		{
			if (!string.IsNullOrEmpty(_virtualFileName))
			{
				return _virtualFileName;
			}
			if (!string.IsNullOrEmpty(RawFilePath))
			{
				string fileName = Path.GetFileName(RawFilePath);
				if (string.IsNullOrEmpty(fileName))
				{
					_virtualFileName = RawFileName;
				}
				else
				{
					_virtualFileName = fileName;
				}
			}
			return _virtualFileName;
		}
		set
		{
			_virtualFileName = value;
			OnPropertyChanged("VirtualFileName");
		}
	}

	public string RawFilePath { get; set; }

	public string RawModifiedDateTime { get; set; }

	public DateTime ModifiedDateTime
	{
		get
		{
			if (!_midifiedDateTime.Equals(DateTime.MinValue))
			{
				return _midifiedDateTime;
			}
			if (!string.IsNullOrEmpty(RawModifiedDateTime))
			{
				DateTime.TryParse(RawModifiedDateTime, out _midifiedDateTime);
			}
			return _midifiedDateTime;
		}
		set
		{
			_midifiedDateTime = value;
			OnPropertyChanged("ModifiedDateTime");
		}
	}

	[JsonProperty("FileSize")]
	public long RawFileSize
	{
		get
		{
			return _RawFileSize;
		}
		set
		{
			_RawFileSize = value;
			OnPropertyChanged("RawFileSize");
		}
	}

	public string LocalFilePath { get; set; }

	[JsonProperty("ImageType")]
	public string ImageType
	{
		get
		{
			if (!string.IsNullOrEmpty(_ImageType))
			{
				if (_ImageType.Contains("/"))
				{
					return _ImageType.Substring(_ImageType.LastIndexOf("/") + 1).ToUpper();
				}
				return _ImageType;
			}
			if (!string.IsNullOrEmpty(VirtualFileName))
			{
				return Path.GetExtension(VirtualFileName)?.ToUpper();
			}
			return string.Empty;
		}
		set
		{
			_ImageType = value;
			OnPropertyChanged("ImageType");
		}
	}
}
