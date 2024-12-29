using System.Collections.Generic;

namespace lenovo.mbg.service.framework.socket;

public class JsonEndPoints
{
	public List<JsonEndPoint> MonitoringEndPoints { get; set; }

	public JsonEndPoints(List<JsonEndPoint> eps)
	{
		MonitoringEndPoints = eps;
	}
}
