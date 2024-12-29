using System.Collections.Generic;
using GoogleAnalytics;

namespace lenovo.mbg.service.framework.services;

public interface IGoogleAnalyticsTracker
{
	Tracker Tracker { get; }

	void Send(IDictionary<string, string> data);
}
