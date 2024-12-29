using System.Threading;

namespace lenovo.themes.generic.Component.Progress;

public class ProgressRateInfo
{
	private long totalPace;

	private int pace;

	public long TotalPace => totalPace;

	public int Pace
	{
		get
		{
			return pace;
		}
		set
		{
			pace = value;
			Interlocked.Add(ref totalPace, value);
		}
	}

	public long TotalLength { get; set; }

	public string ResourceKey { get; set; }

	public void Clear()
	{
		TotalLength = 0L;
		ResourceKey = string.Empty;
		Interlocked.Exchange(ref totalPace, 0L);
	}
}
