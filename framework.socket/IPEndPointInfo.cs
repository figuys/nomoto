using System.Net;

namespace lenovo.mbg.service.framework.socket;

public class IPEndPointInfo
{
	public string IPAddress { get; set; }

	public int Point { get; set; }

	public IPEndPointInfo(string ipAddress, int point)
	{
		IPAddress = ipAddress;
		Point = point;
	}

	public static implicit operator IPEndPoint(IPEndPointInfo ipEndPointInfo)
	{
		if (ipEndPointInfo == null)
		{
			return null;
		}
		return new IPEndPoint(System.Net.IPAddress.Parse(ipEndPointInfo.IPAddress), ipEndPointInfo.Point);
	}

	public static implicit operator IPEndPointInfo(IPEndPoint ipEndPoint)
	{
		if (ipEndPoint == null)
		{
			return null;
		}
		return new IPEndPointInfo(ipEndPoint.Address.ToString(), ipEndPoint.Port);
	}
}
