namespace lenovo.mbg.service.lmsa;

public class DeviceConnectionControlPanelDataModel : ModelBase
{
	private bool mIsConnected = false;

	private string mConnectingDeviceDiplayName = string.Empty;

	private ConnectingWays mConnectingWay = ConnectingWays.USB;

	public bool IsConnected
	{
		get
		{
			return mIsConnected;
		}
		set
		{
			if (mIsConnected != value)
			{
				mIsConnected = value;
				FirePropertyChangedEvent("IsConnected");
			}
		}
	}

	public string ConnectingDeviceDiplayName
	{
		get
		{
			return mConnectingDeviceDiplayName;
		}
		set
		{
			if (!(mConnectingDeviceDiplayName == value))
			{
				mConnectingDeviceDiplayName = value;
				FirePropertyChangedEvent("ConnectingDeviceDiplayName");
			}
		}
	}

	public ConnectingWays ConnectingWay
	{
		get
		{
			return mConnectingWay;
		}
		set
		{
			if (mConnectingWay != value)
			{
				mConnectingWay = value;
				FirePropertyChangedEvent("ConnectingWay");
			}
		}
	}
}
