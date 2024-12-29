using System.Text;

namespace lenovo.mbg.service.framework.socket;

public static class Constants
{
	public const int WIFI_MONITORING_PORT_UPPER_LIMIT = 10000;

	public const int WIFI_MONITORING_PORT_BOTTOM_LIMIT = 20000;

	public const int WIFI_HEART_BEAT_INTERVAL = 5;

	public const int MAX_WIFI_CONCURRENCE_CONNECT = 1;

	public const int NETWORK_ADAPTER_REFRESH_INTERVAL = 5000;

	public const int SOCKET_RECEIVE_DATA_BUFFER = 1024;

	public const int MAX_WAITING_FOR_STOP_THREAD = 10000;

	public const int MAX_WAITING_FOR_CLOSE_SOCKET = 5000;

	public static readonly Encoding Encoding = Encoding.UTF8;
}
