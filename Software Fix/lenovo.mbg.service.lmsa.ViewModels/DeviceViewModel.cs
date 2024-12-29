using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class DeviceViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private ConnectType _ConnectTy;

	private bool _IsMotorola = true;

	private ImageSource _Image;

	private ImageSource _SelectedImage;

	private bool _IsEnabled = true;

	private bool _IsSelected = false;

	public string Id { get; set; }

	public string Text { get; set; }

	public DeviceEx Device { get; set; }

	public ConnectType ConnectTy
	{
		get
		{
			return _ConnectTy;
		}
		set
		{
			_ConnectTy = value;
			OnPropertyChanged("ConnectTy");
		}
	}

	public bool IsMotorola
	{
		get
		{
			return _IsMotorola;
		}
		set
		{
			_IsMotorola = value;
			OnPropertyChanged("IsMotorola");
		}
	}

	public ImageSource Image
	{
		get
		{
			return _Image;
		}
		set
		{
			_Image = value;
			OnPropertyChanged("Image");
		}
	}

	public ImageSource SelectedImage
	{
		get
		{
			return _SelectedImage;
		}
		set
		{
			_SelectedImage = value;
			OnPropertyChanged("SelectedImage");
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _IsEnabled;
		}
		set
		{
			_IsEnabled = value;
			OnPropertyChanged("IsEnabled");
		}
	}

	public bool IsSelected
	{
		get
		{
			return _IsSelected;
		}
		set
		{
			_IsSelected = value;
			OnPropertyChanged("IsSelected");
		}
	}

	public DeviceViewModel(string id, string text, ConnectType connectTy, DeviceEx device, bool isMotorola)
	{
		Id = id;
		Text = text;
		Device = device;
		IsMotorola = isMotorola;
		ConnectTy = connectTy;
		Init();
	}

	public void Update(DeviceViewModel deviceViewModel)
	{
		Device = deviceViewModel.Device;
		IsMotorola = deviceViewModel.IsMotorola;
		ConnectTy = deviceViewModel.ConnectTy;
		Init();
	}

	public void Init()
	{
		if (HostProxy.HostNavigation.CurrentPluginID == "02928af025384c75ae055aa2d4f256c8")
		{
			IsEnabled = ConnectTy != ConnectType.Fastboot;
		}
		if (ConnectTy == ConnectType.Wifi)
		{
			Image = Application.Current.Resources["v6_icon_wifi"] as ImageSource;
			SelectedImage = Application.Current.Resources["v6_icon_wifi_selected"] as ImageSource;
		}
		else
		{
			Image = Application.Current.Resources["v6_icon_usb"] as ImageSource;
			SelectedImage = Application.Current.Resources["v6_icon_usb_selected"] as ImageSource;
		}
	}
}
