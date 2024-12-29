using System.Collections.Generic;
using GoogleAnalytics;
using GoogleAnalytics.WPF.Managed;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.Services;

public class GoogleAnalyticsTracker : IGoogleAnalyticsTracker
{
	protected static readonly string GAPropertyId = "UA-100938782-1";

	public Tracker Tracker { get; }

	public GoogleAnalyticsTracker()
	{
		Tracker = AnalyticsManager.Current.CreateTracker(GAPropertyId);
	}

	public void Send(IDictionary<string, string> data)
	{
		try
		{
			Tracker.Send(data);
		}
		catch
		{
		}
	}
}
