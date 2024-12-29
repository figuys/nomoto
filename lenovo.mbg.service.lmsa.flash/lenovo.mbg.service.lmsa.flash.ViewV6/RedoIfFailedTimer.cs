using System;
using System.Threading;
using System.Timers;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class RedoIfFailedTimer
{
	private System.Timers.Timer timer;

	protected long loaddtalocker;

	public volatile bool IsLoadFailed;

	public Action RedoProAction { get; set; }

	public RedoIfFailedTimer(Action callBack)
	{
		loaddtalocker = 0L;
		IsLoadFailed = false;
		RedoProAction = callBack;
		timer = new System.Timers.Timer(5000.0);
		timer.Elapsed += LoopDataHandler;
	}

	public void RegisterLoadDataTimer()
	{
		timer.Enabled = true;
	}

	public void DestoryLoadDataTimer()
	{
		if (timer != null)
		{
			timer.Enabled = false;
			timer.Dispose();
			timer = null;
		}
	}

	private void LoopDataHandler(object sender, ElapsedEventArgs e)
	{
		if (Interlocked.Read(ref loaddtalocker) == 0L)
		{
			Interlocked.Exchange(ref loaddtalocker, 1L);
			if (IsLoadFailed && !string.IsNullOrEmpty(GlobalFun.GetStringFromUrl(WebApiUrl.NETWORK_CONNECT_CHECK)))
			{
				RedoProAction?.Invoke();
			}
			Interlocked.Exchange(ref loaddtalocker, 0L);
		}
	}
}
