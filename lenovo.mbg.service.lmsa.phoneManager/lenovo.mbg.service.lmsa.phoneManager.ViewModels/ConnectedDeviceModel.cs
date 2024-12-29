using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.framework.services.Device;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class ConnectedDeviceModel : NotifyBase
{
	private string m_Value;

	private string m_SN;

	private DeviceEx device;

	private ImageSource icon;

	private ImageSource hoverIcon;

	public DeviceEx Device
	{
		get
		{
			return device;
		}
		set
		{
			device = value;
			if (device != null)
			{
				if (device.ConnectType == ConnectType.Adb)
				{
					Icon = (ImageSource)Application.Current.FindResource("bottom_usbDrawingImage");
					HoverIcon = (ImageSource)Application.Current.FindResource("bottom_usbDrawingImage_ffffff");
				}
				else if (device.ConnectType == ConnectType.Wifi)
				{
					Icon = (ImageSource)Application.Current.FindResource("connected_wifiDrawingImage");
					HoverIcon = (ImageSource)Application.Current.FindResource("connected_wifiDrawingImage_ffffff");
				}
			}
		}
	}

	public string Key { get; set; }

	public string Value
	{
		get
		{
			return m_Value;
		}
		set
		{
			m_Value = value;
			OnPropertyChanged("Value");
		}
	}

	public string SN
	{
		get
		{
			return m_SN;
		}
		set
		{
			m_SN = value;
			OnPropertyChanged("SN");
		}
	}

	public ImageSource Icon
	{
		get
		{
			return icon;
		}
		set
		{
			if (icon != value)
			{
				icon = value;
				OnPropertyChanged("Icon");
			}
		}
	}

	public ImageSource HoverIcon
	{
		get
		{
			return hoverIcon;
		}
		set
		{
			if (hoverIcon != value)
			{
				hoverIcon = value;
				OnPropertyChanged("HoverIcon");
			}
		}
	}
}
