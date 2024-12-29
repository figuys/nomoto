namespace lenovo.mbg.service.framework.socket;

public class JsonEndPoint
{
	public string IP { get; set; }

	public int Port { get; set; }

	public JsonEndPoint(string ip, int port)
	{
		IP = ip;
		Port = port;
	}
}
