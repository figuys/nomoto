using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class Video
{
	private long longModifyDate;

	[JsonProperty("id")]
	public int Id { get; set; }

	[JsonProperty("fileName")]
	public string Name { get; set; }

	[JsonProperty("duration")]
	public long Duration { get; set; }

	[JsonProperty("album")]
	public string Album { get; set; }

	[JsonProperty("path")]
	public string FullFilePath { get; set; }

	[JsonProperty("date")]
	public string ModifiyDate { get; set; }

	public long LongModifyDate
	{
		get
		{
			if (longModifyDate == 0L)
			{
				long.TryParse(ModifiyDate, out longModifyDate);
			}
			return longModifyDate;
		}
		set
		{
			longModifyDate = value;
		}
	}

	[JsonProperty("size")]
	public long Size { get; internal set; }
}
