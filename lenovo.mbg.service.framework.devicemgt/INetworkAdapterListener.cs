using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.framework.devicemgt;

public interface INetworkAdapterListener
{
	void OnWifiMonitoringEndPointChanged(List<Tuple<string, string>> endpoints);
}
