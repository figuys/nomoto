using lenovo.mbg.service.lmsa.Business;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.Login.ViewModel;

public class DeviceDetailViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private DeviceModel _Device;

	public DeviceModel Device
	{
		get
		{
			return _Device;
		}
		set
		{
			_Device = value;
			OnPropertyChanged("Device");
		}
	}

	public DeviceDetailViewModel(DeviceModel device)
	{
		Device = device;
	}
}
