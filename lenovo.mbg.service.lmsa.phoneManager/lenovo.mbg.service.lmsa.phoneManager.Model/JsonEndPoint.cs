using System.Runtime.Serialization;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

[DataContract]
public class JsonEndPoint
{
	[DataMember]
	public string IP { get; set; }

	[DataMember]
	public int Port { get; set; }

	public JsonEndPoint(string ip, int port)
	{
		IP = ip;
		Port = port;
	}
}
