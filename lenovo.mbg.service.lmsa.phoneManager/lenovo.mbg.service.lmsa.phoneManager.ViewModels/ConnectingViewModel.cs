using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class ConnectingViewModel : ViewModelBase
{
	private string connectStepName;

	private int connectStepPercent;

	private string connectType;

	public string ConnectStepName
	{
		get
		{
			return connectStepName;
		}
		set
		{
			if (!(connectStepName == value))
			{
				connectStepName = value;
				OnPropertyChanged("ConnectStepName");
			}
		}
	}

	public int ConnectStepPercent
	{
		get
		{
			return connectStepPercent;
		}
		set
		{
			if (connectStepPercent != value)
			{
				connectStepPercent = value;
				OnPropertyChanged("ConnectStepPercent");
			}
		}
	}

	public string ConnectType
	{
		get
		{
			return connectType;
		}
		set
		{
			if (!(connectType == value))
			{
				connectType = value;
				OnPropertyChanged("ConnectType");
			}
		}
	}

	public ConnectingViewModel()
	{
		HostProxy.deviceManager.MasterDeviceChanged += delegate(object s, MasterDeviceChangedEventArgs e)
		{
			TcpAndroidDevice tcpAndroidDevice = e.Current as TcpAndroidDevice;
			if (tcpAndroidDevice != null)
			{
				tcpAndroidDevice.TcpConnectStepChanged += Device_TcpConnectStepChanged;
			}
			if (e.Previous is TcpAndroidDevice tcpAndroidDevice2)
			{
				tcpAndroidDevice2.TcpConnectStepChanged -= Device_TcpConnectStepChanged;
			}
			ConnectType = tcpAndroidDevice?.ConnectType.ToString() ?? lenovo.mbg.service.framework.services.Device.ConnectType.Adb.ToString();
		};
	}

	private void Device_TcpConnectStepChanged(object sender, TcpConnectStepChangedEventArgs e)
	{
		string text = string.Empty;
		switch (e.Step)
		{
		case "AppVersionIsMatched":
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				text = "K0765";
				break;
			case ConnectStepStatus.Starting:
				text = "K0758";
				break;
			case ConnectStepStatus.Success:
				text = "K0766";
				break;
			}
			break;
		case "UnInstallApp":
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				text = "Uninstalling Assistant App failed";
				break;
			case ConnectStepStatus.Starting:
				text = "K0775";
				break;
			case ConnectStepStatus.Success:
				text = "K0776";
				break;
			}
			break;
		case "InstallApp":
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				text = "Installing Assistant App failed";
				break;
			case ConnectStepStatus.Starting:
				text = "K0759";
				break;
			case ConnectStepStatus.Success:
				text = "K0760";
				break;
			}
			break;
		case "TcpConnect":
		{
			TcpAndroidDevice tcpAndroidDevice = sender as TcpAndroidDevice;
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				text = ((!"Moto".Equals(tcpAndroidDevice?.ConnectedAppType)) ? "K0768" : ((e.ErrorCode != ConnectErrorCode.TcpConnectFailWithAppNotAllowed) ? "K0792" : "K0801"));
				break;
			case ConnectStepStatus.Starting:
				text = ((!"Moto".Equals(tcpAndroidDevice?.ConnectedAppType)) ? "K0738" : "K0789");
				break;
			case ConnectStepStatus.Success:
				text = "K0761";
				break;
			}
			break;
		}
		case "LoadDeviceProperty":
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				text = "Load device data failed";
				break;
			case ConnectStepStatus.Starting:
				text = "K0445";
				break;
			case ConnectStepStatus.Success:
				text = "K0762";
				break;
			}
			break;
		}
		ConnectStepName = text;
		ConnectStepPercent = e.Percent;
	}
}
