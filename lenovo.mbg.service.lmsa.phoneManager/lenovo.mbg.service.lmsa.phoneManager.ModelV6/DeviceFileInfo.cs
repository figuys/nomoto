using System;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.ModelV6;

public class DeviceFileInfo
{
	[JsonIgnore]
	private long? _longFileSize;

	[JsonIgnore]
	private bool? _isFolder;

	[JsonIgnore]
	private DateTime? _dateTimeModifiedDate;

	[JsonProperty("path")]
	public string RawFilePath { get; set; }

	[JsonProperty("name")]
	public string RawFileName { get; set; }

	[JsonProperty("size")]
	public string RawFileSize { get; set; }

	[JsonProperty("type")]
	public string RawFileType { get; set; }

	[JsonProperty("isFolder")]
	public string RawIsFolder { get; set; }

	[JsonProperty("modifiedDate")]
	public string RawModifiedDate { get; set; }

	[JsonIgnore]
	public long LongFileSize
	{
		get
		{
			if (_longFileSize.HasValue)
			{
				return _longFileSize.Value;
			}
			if (!string.IsNullOrEmpty(RawFileSize))
			{
				long result = 0L;
				long.TryParse(RawFileSize, out result);
				_longFileSize = result;
				return result;
			}
			return 0L;
		}
	}

	[JsonIgnore]
	public bool BooleanIsFolder
	{
		get
		{
			if (_isFolder.HasValue)
			{
				return _isFolder.Value;
			}
			if (!string.IsNullOrEmpty(RawIsFolder))
			{
				bool result = false;
				bool.TryParse(RawIsFolder, out result);
				_isFolder = result;
				return result;
			}
			return false;
		}
	}

	[JsonIgnore]
	public DateTime DateTimeModifiedDate
	{
		get
		{
			if (_dateTimeModifiedDate.HasValue)
			{
				return _dateTimeModifiedDate.Value;
			}
			if (!string.IsNullOrEmpty(RawModifiedDate))
			{
				DateTime result = DateTime.MinValue;
				DateTime.TryParse(RawModifiedDate, out result);
				_dateTimeModifiedDate = result;
				return result;
			}
			return DateTime.MinValue;
		}
	}
}
