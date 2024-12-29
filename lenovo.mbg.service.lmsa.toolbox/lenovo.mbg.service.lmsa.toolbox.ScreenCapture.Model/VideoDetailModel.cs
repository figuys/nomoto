using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenCapture.Model;

public class VideoDetailModel
{
	[JsonProperty("id")]
	private string id;

	[JsonProperty("name")]
	private string name;

	[JsonProperty("duration")]
	private long duration;

	[JsonProperty("size")]
	private long size;

	[JsonProperty("modifiedDate")]
	private long modifiedDate;

	[JsonProperty("modifiedDateDisplayString")]
	private string modifiedDateDisplayString;

	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public long Duration
	{
		get
		{
			return duration;
		}
		set
		{
			duration = value;
		}
	}

	public long Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
		}
	}

	public long ModifiedDate
	{
		get
		{
			return modifiedDate;
		}
		set
		{
			modifiedDate = value;
		}
	}

	public string ModifiedDateDisplayString
	{
		get
		{
			return modifiedDateDisplayString;
		}
		set
		{
			modifiedDateDisplayString = value;
		}
	}
}
