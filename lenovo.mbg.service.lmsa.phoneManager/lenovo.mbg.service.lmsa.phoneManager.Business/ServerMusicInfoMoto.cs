using System;
using System.IO;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class ServerMusicInfoMoto
{
	[JsonIgnore]
	private string _virtualFileName = string.Empty;

	[JsonIgnore]
	private double _doubleDuration = -1.0;

	[JsonIgnore]
	private long _longFileSize = -1L;

	[JsonIgnore]
	private DateTime _modifiedDate = DateTime.MinValue;

	[JsonIgnore]
	private double _doubleFrequency = -1.0;

	[JsonIgnore]
	public ServerAlbumInfo AlbumInfo { get; set; }

	[JsonProperty("id")]
	public int ID { get; set; }

	[JsonProperty("albumId")]
	public string AlbumID { get; set; }

	[JsonProperty("albumName")]
	public string AlbumName { get; set; }

	[JsonProperty("fileName")]
	public string RawFileName { get; set; }

	[JsonProperty("filePath")]
	public string RawFilePath { get; set; }

	[JsonProperty("displayName")]
	public string DisplayName { get; set; }

	[JsonIgnore]
	public string VirtualFileName
	{
		get
		{
			if (!string.IsNullOrEmpty(_virtualFileName))
			{
				return _virtualFileName;
			}
			if (!string.IsNullOrEmpty(RawFileName))
			{
				string fileName = Path.GetFileName(RawFileName);
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
	}

	[JsonProperty("artist")]
	public string RawArtist { get; set; }

	[JsonProperty("duration")]
	public string RawDuration { get; set; }

	[JsonIgnore]
	public double DoubleDuration
	{
		get
		{
			if (_doubleDuration != -1.0)
			{
				return _doubleDuration;
			}
			_doubleDuration = 0.0;
			if (!string.IsNullOrEmpty(RawDuration))
			{
				double.TryParse(RawDuration, out _doubleDuration);
			}
			return _doubleDuration;
		}
	}

	[JsonProperty("fileSize")]
	public string RawFileSize { get; set; }

	[JsonIgnore]
	public long LongFileSize
	{
		get
		{
			if (_longFileSize != -1)
			{
				return _longFileSize;
			}
			_longFileSize = 0L;
			if (!string.IsNullOrEmpty(RawFileSize))
			{
				long.TryParse(RawFileSize, out _longFileSize);
			}
			return _longFileSize;
		}
	}

	[JsonProperty("modifiedDate")]
	public string RawModifiedDate { get; set; }

	[JsonIgnore]
	public DateTime ModifiedDate
	{
		get
		{
			if (_modifiedDate != DateTime.MinValue)
			{
				return _modifiedDate;
			}
			if (!string.IsNullOrEmpty(RawModifiedDate))
			{
				DateTime.TryParse(RawModifiedDate, out _modifiedDate);
			}
			return _modifiedDate;
		}
	}

	[JsonProperty("frequency")]
	public string RawFrequency { get; set; }

	[JsonIgnore]
	public double DoubleFrequency
	{
		get
		{
			if (_doubleFrequency != -1.0)
			{
				return _doubleFrequency;
			}
			_doubleFrequency = 0.0;
			if (!string.IsNullOrEmpty(RawDuration))
			{
				double.TryParse(RawDuration, out _doubleFrequency);
			}
			return _doubleFrequency;
		}
	}
}
