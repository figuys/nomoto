using System;
using lenovo.mbg.service.lmsa.phoneManager.Business;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class VideoInfo
{
	public Video RawVideo { get; set; }

	public string Name { get; set; }

	public string Duration { get; set; }

	public long LongDuration { get; set; }

	public long Size { get; set; }

	public string Type { get; set; }

	public string ModifiedDate { get; set; }

	public string FilePath { get; set; }

	public string LocalVideoImagePath { get; set; }

	public string AlbumName { get; set; }

	public ServerAlbumInfo Owner { get; set; }

	public void SetData(Video rawVideoInfo, ServerAlbumInfo owner)
	{
		RawVideo = rawVideoInfo;
		Owner = owner;
		Name = rawVideoInfo.Name.Substring(0, rawVideoInfo.Name.LastIndexOf("."));
		Type = rawVideoInfo.Name.Substring(rawVideoInfo.Name.LastIndexOf(".") + 1).ToUpper();
		Size = rawVideoInfo.Size;
		ModifiedDate = ConvertMillisecond2DateTime(rawVideoInfo.ModifiyDate);
		FilePath = rawVideoInfo.FullFilePath;
		LongDuration = rawVideoInfo.Duration;
		Duration = new TimeSpan(0, 0, (int)rawVideoInfo.Duration / 1000).ToString("hh\\:mm\\:ss");
		AlbumName = owner.AlbumName;
	}

	private string ConvertMillisecond2DateTime(string millisecond)
	{
		long result = 0L;
		long.TryParse(millisecond, out result);
		if (result == 0L)
		{
			return string.Empty;
		}
		if (millisecond.Length > 10)
		{
			result /= 1000;
		}
		return new DateTime(1970, 1, 1).AddSeconds(result).ToLocalTime().ToString("yyyy-MM-dd HH:mm");
	}
}
