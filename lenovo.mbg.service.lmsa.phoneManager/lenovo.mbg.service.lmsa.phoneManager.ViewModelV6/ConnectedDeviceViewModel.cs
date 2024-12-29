using System.Collections.ObjectModel;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class ConnectedDeviceViewModel : ViewModelBase
{
	private string _ModelName;

	private string _SN;

	private double _TotalDisk;

	private double _UsedDisk;

	private double _FreeDisk;

	private string _FreeDiskUnit;

	private string _UsedDiskUnit;

	private bool _IsEnabled = true;

	public TcpAndroidDevice Device { get; set; }

	public string Id => Device.Identifer;

	public string ModelName
	{
		get
		{
			return _ModelName;
		}
		set
		{
			_ModelName = value;
			OnPropertyChanged("ModelName");
		}
	}

	public string SN
	{
		get
		{
			return _SN;
		}
		set
		{
			_SN = value;
			OnPropertyChanged("SN");
		}
	}

	public double TotalDisk
	{
		get
		{
			return _TotalDisk;
		}
		set
		{
			_TotalDisk = value;
			OnPropertyChanged("TotalDisk");
		}
	}

	public double UsedDisk
	{
		get
		{
			return _UsedDisk;
		}
		set
		{
			if (value != _UsedDisk)
			{
				_UsedDisk = value;
				UsedDiskUnit = GlobalFun.ConvertLong2String((long)value);
				FreeDisk = TotalDisk - value;
				OnPropertyChanged("UsedDisk");
			}
		}
	}

	public double FreeDisk
	{
		get
		{
			return _FreeDisk;
		}
		set
		{
			_FreeDisk = value;
			FreeDiskUnit = GlobalFun.ConvertLong2String((long)value);
			OnPropertyChanged("FreeDisk");
		}
	}

	public string FreeDiskUnit
	{
		get
		{
			return _FreeDiskUnit;
		}
		set
		{
			_FreeDiskUnit = value;
			OnPropertyChanged("FreeDiskUnit");
		}
	}

	public string UsedDiskUnit
	{
		get
		{
			return _UsedDiskUnit;
		}
		set
		{
			_UsedDiskUnit = value;
			OnPropertyChanged("UsedDiskUnit");
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

	public ObservableCollection<CategoryInfoViewModel> CategoryInfos { get; set; }

	public ConnectedDeviceViewModel(TcpAndroidDevice device)
	{
		Device = device;
		if (device.Property != null)
		{
			ModelName = device.Property.ModelName;
			SN = device.Property.SN;
			TotalDisk = device.Property.TotalInternalStorage;
			UsedDisk = device.Property.UsedInternalStorage;
		}
		CategoryInfos = new ObservableCollection<CategoryInfoViewModel>();
		CategoryInfos.Add(new CategoryInfoViewModel
		{
			Title = "K0478",
			ResourceType = "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}"
		});
		CategoryInfos.Add(new CategoryInfoViewModel
		{
			Title = "K0475",
			ResourceType = "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}"
		});
		CategoryInfos.Add(new CategoryInfoViewModel
		{
			Title = "K0476",
			ResourceType = "{242C8F16-6AC7-431B-BBF1-AE24373860F1}"
		});
		CategoryInfos.Add(new CategoryInfoViewModel
		{
			Title = "K0477",
			ResourceType = "{8BEBE14B-4E45-4D36-8726-8442E6242C01}"
		});
	}

	public ConnectedDeviceViewModel(string vaule, string sn, double total, double used)
	{
		ModelName = vaule;
		SN = sn;
		TotalDisk = total;
		UsedDisk = used;
		CategoryInfos = new ObservableCollection<CategoryInfoViewModel>();
		CategoryInfos.Add(new CategoryInfoViewModel
		{
			Title = "K0478",
			ResourceType = "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}"
		});
		CategoryInfos.Add(new CategoryInfoViewModel
		{
			Title = "K0475",
			ResourceType = "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}"
		});
		CategoryInfos.Add(new CategoryInfoViewModel
		{
			Title = "K0476",
			ResourceType = "{242C8F16-6AC7-431B-BBF1-AE24373860F1}"
		});
		CategoryInfos.Add(new CategoryInfoViewModel
		{
			Title = "K0477",
			ResourceType = "{8BEBE14B-4E45-4D36-8726-8442E6242C01}"
		});
	}
}
