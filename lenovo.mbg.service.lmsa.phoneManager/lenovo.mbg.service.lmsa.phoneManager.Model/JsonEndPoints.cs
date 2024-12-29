using System.Collections.Generic;
using System.Runtime.Serialization;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

[DataContract]
public class JsonEndPoints
{
	[DataMember]
	public List<JsonEndPoint> MonitoringEndPoints { get; set; }

	public JsonEndPoints(List<JsonEndPoint> eps)
	{
		MonitoringEndPoints = eps;
	}
}
