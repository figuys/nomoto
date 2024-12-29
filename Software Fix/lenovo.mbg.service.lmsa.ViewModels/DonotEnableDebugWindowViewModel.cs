using System.Collections.Generic;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class DonotEnableDebugWindowViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private List<object> m_usbDebugList;

	private int m_usbDebugSelectedIndex = -1;

	private Visibility m_android10Visibility = Visibility.Collapsed;

	private Visibility m_android8Visibility = Visibility.Collapsed;

	private Visibility m_android7Visibility = Visibility.Collapsed;

	public List<object> UsbDebugList
	{
		get
		{
			return m_usbDebugList;
		}
		set
		{
			m_usbDebugList = value;
			OnPropertyChanged("UsbDebugList");
		}
	}

	public int UsbDebugSelectedIndex
	{
		get
		{
			return m_usbDebugSelectedIndex;
		}
		set
		{
			m_usbDebugSelectedIndex = value;
			android7Visibility = ((value != 0) ? Visibility.Collapsed : Visibility.Visible);
			android8Visibility = ((value != 1) ? Visibility.Collapsed : Visibility.Visible);
			android10Visibility = ((value != 2) ? Visibility.Collapsed : Visibility.Visible);
			OnPropertyChanged("UsbDebugSelectedIndex");
		}
	}

	public Visibility android10Visibility
	{
		get
		{
			return m_android10Visibility;
		}
		set
		{
			m_android10Visibility = value;
			OnPropertyChanged("android10Visibility");
		}
	}

	public Visibility android8Visibility
	{
		get
		{
			return m_android8Visibility;
		}
		set
		{
			m_android8Visibility = value;
			OnPropertyChanged("android8Visibility");
		}
	}

	public Visibility android7Visibility
	{
		get
		{
			return m_android7Visibility;
		}
		set
		{
			m_android7Visibility = value;
			OnPropertyChanged("android7Visibility");
		}
	}

	public DonotEnableDebugWindowViewModel()
	{
		List<object> usbDebugList = new List<object> { "K0439", "K0440", "K0438" };
		UsbDebugList = usbDebugList;
		UsbDebugSelectedIndex = 0;
	}
}
