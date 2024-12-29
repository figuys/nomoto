using System;
using System.Text.RegularExpressions;
using System.Timers;

namespace lenovo.mbg.service.common.utilities;

public class NetworkCheckHelper
{
	protected Timer LoopDataTimer;

	protected bool NetworkConnected;

	public static event EventHandler<bool> NetworkChanged;

	public NetworkCheckHelper()
	{
		LoopDataTimer = new Timer();
		LoopDataTimer.Elapsed += LoopCheckHandler;
	}

	public void Start(int interval = 5000)
	{
		LoopDataTimer.Interval = interval;
		LoopDataTimer.Enabled = true;
	}

	public void Stop()
	{
		if (LoopDataTimer != null)
		{
			LoopDataTimer.Enabled = false;
			LoopDataTimer = null;
		}
	}

	protected void LoopCheckHandler(object sender, ElapsedEventArgs e)
	{
		string stringFromUrl = GlobalFun.GetStringFromUrl("https://lsa.lenovo.com/lmsa-web/index.jsp");
		bool flag = false;
		if (!string.IsNullOrEmpty(stringFromUrl))
		{
			flag = Regex.IsMatch(stringFromUrl, "Web is running", RegexOptions.IgnoreCase | RegexOptions.Multiline);
		}
		if (flag != NetworkConnected)
		{
			NetworkConnected = !NetworkConnected;
			FireEvent(NetworkConnected);
		}
	}

	protected void FireEvent(bool connected)
	{
		EventHandler<bool> networkChanged = NetworkCheckHelper.NetworkChanged;
		if (networkChanged != null)
		{
			Delegate[] invocationList = networkChanged.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((EventHandler<bool>)invocationList[i]).BeginInvoke(this, connected, null, null);
			}
		}
	}
}
