using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.UserControls.CustomControls;

public partial class UCDeviceConnectionControlPanel : UserControl, IComponentConnector
{
	public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(UCDeviceConnectionControlPanel), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty DeviceConnectingWayProperty = DependencyProperty.Register("DeviceConnectingWay", typeof(ConnectingWays), typeof(UCDeviceConnectionControlPanel), new PropertyMetadata(ConnectingWays.USB));

	public static readonly DependencyProperty IsConnectedProperty = DependencyProperty.Register("IsConnected", typeof(bool), typeof(UCDeviceConnectionControlPanel), new PropertyMetadata(false));

	public string ButtonText
	{
		get
		{
			return (string)GetValue(ButtonTextProperty);
		}
		set
		{
			SetValue(ButtonTextProperty, value);
		}
	}

	public ConnectingWays DeviceConnectingWay
	{
		get
		{
			return (ConnectingWays)GetValue(DeviceConnectingWayProperty);
		}
		set
		{
			SetValue(DeviceConnectingWayProperty, value);
		}
	}

	public bool IsConnected
	{
		get
		{
			return (bool)GetValue(IsConnectedProperty);
		}
		set
		{
			SetValue(IsConnectedProperty, value);
		}
	}

	public UCDeviceConnectionControlPanel()
	{
		InitializeComponent();
	}
}
