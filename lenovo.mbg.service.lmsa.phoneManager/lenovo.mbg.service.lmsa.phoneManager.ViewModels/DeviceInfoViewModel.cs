using System;
using System.Collections.ObjectModel;
using System.Windows;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class DeviceInfoViewModel : ViewModelBase
{
	public class DeviceBatteryInfoItemViewModel : DeviceInfoItemViewModel
	{
		private int content;

		public new int Content
		{
			get
			{
				return content;
			}
			set
			{
				if (content != value)
				{
					content = value;
					OnPropertyChanged("Content");
				}
			}
		}

		public override void Clear()
		{
			Content = 0;
		}
	}

	public class DeviceStorageInfoItemViewModel : DeviceInfoItemViewModel
	{
		private string totalWithUnit;

		private string useWithUnit;

		private string freeWithUnit;

		private double total;

		private double use;

		private double free;

		private double usePercent;

		public string TotalWithUnit
		{
			get
			{
				return totalWithUnit;
			}
			set
			{
				if (!(totalWithUnit == value))
				{
					totalWithUnit = value;
					OnPropertyChanged("TotalWithUnit");
				}
			}
		}

		public string UseWithUnit
		{
			get
			{
				return useWithUnit;
			}
			set
			{
				if (!(useWithUnit == value))
				{
					useWithUnit = value;
					OnPropertyChanged("UseWithUnit");
				}
			}
		}

		public string FreeWithUnit
		{
			get
			{
				return freeWithUnit;
			}
			set
			{
				if (!(freeWithUnit == value))
				{
					freeWithUnit = value;
					OnPropertyChanged("FreeWithUnit");
				}
			}
		}

		public double Total
		{
			get
			{
				return total;
			}
			set
			{
				if (total != value)
				{
					total = value;
					UpdateUsePercent();
					OnPropertyChanged("Total");
				}
			}
		}

		public double Use
		{
			get
			{
				return use;
			}
			set
			{
				if (use != value)
				{
					use = value;
					UpdateUsePercent();
					OnPropertyChanged("Use");
				}
			}
		}

		public double Free
		{
			get
			{
				return free;
			}
			set
			{
				if (free != value)
				{
					free = value;
					OnPropertyChanged("Free");
				}
			}
		}

		public double UsePercent
		{
			get
			{
				return usePercent;
			}
			set
			{
				if (usePercent != value)
				{
					usePercent = value;
					OnPropertyChanged("UsePercent");
				}
			}
		}

		private void UpdateUsePercent()
		{
			if (Total > 0.0)
			{
				UsePercent = Math.Round(Use / Total * 100.0, 2);
			}
			else
			{
				UsePercent = 0.0;
			}
		}

		public override void Clear()
		{
			Total = 0.0;
			Use = 0.0;
			Free = 0.0;
			TotalWithUnit = string.Empty;
			UseWithUnit = string.Empty;
			FreeWithUnit = string.Empty;
		}
	}

	private DeviceInfoItemViewModel modelName = new DeviceInfoItemViewModel
	{
		Title = "K0455"
	};

	private DeviceBatteryInfoItemViewModel battery = new DeviceBatteryInfoItemViewModel
	{
		DateTemplateTag = "Battery",
		Title = "K0457"
	};

	private DeviceInfoItemViewModel processor = new DeviceInfoItemViewModel
	{
		Title = "K0458"
	};

	private DeviceInfoItemViewModel imei1 = new DeviceInfoItemViewModel
	{
		CopyVisibility = Visibility.Visible,
		Title = "K0460"
	};

	private DeviceInfoItemViewModel imei2 = new DeviceInfoItemViewModel
	{
		CopyVisibility = Visibility.Visible,
		Title = "K0461"
	};

	private DeviceInfoItemViewModel sn = new DeviceInfoItemViewModel
	{
		CopyVisibility = Visibility.Visible,
		Title = "K0462"
	};

	private DeviceStorageInfoItemViewModel instorage = new DeviceStorageInfoItemViewModel
	{
		DateTemplateTag = "Storage",
		Title = "K0463"
	};

	private DeviceStorageInfoItemViewModel exstorage = new DeviceStorageInfoItemViewModel
	{
		DateTemplateTag = "Storage",
		Title = "K0464"
	};

	private DeviceInfoItemViewModel upTime = new DeviceInfoItemViewModel
	{
		Title = "Up time:"
	};

	private DeviceInfoItemViewModel androidVer = new DeviceInfoItemViewModel
	{
		Title = "K0468"
	};

	private DeviceInfoItemViewModel currentVersion = new DeviceInfoItemViewModel
	{
		Title = "K0467"
	};

	private ObservableCollection<DeviceInfoItemViewModel> items;

	public ObservableCollection<DeviceInfoItemViewModel> Items
	{
		get
		{
			return items;
		}
		set
		{
			if (items != value)
			{
				items = value;
				OnPropertyChanged("Items");
			}
		}
	}

	public DeviceStorageInfoItemViewModel InstorageInfo
	{
		get
		{
			return instorage;
		}
		set
		{
			if (instorage != value)
			{
				instorage = value;
				OnPropertyChanged("InstorageInfo");
			}
		}
	}

	public DeviceInfoViewModel()
	{
		Items = new ObservableCollection<DeviceInfoItemViewModel>();
		Items.Add(modelName);
		Items.Add(battery);
		Items.Add(processor);
		Items.Add(imei1);
		Items.Add(imei2);
		Items.Add(sn);
		Items.Add(instorage);
		Items.Add(androidVer);
		Items.Add(currentVersion);
		HostProxy.deviceManager.MasterDeviceChanged += delegate(object s, MasterDeviceChangedEventArgs e)
		{
			if (e.Current != null)
			{
				e.Current.SoftStatusChanged += Current_SoftStatusChanged;
			}
			if (e.Previous != null)
			{
				e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
			}
		};
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		switch (e)
		{
		case DeviceSoftStateEx.Offline:
			UpdateDeviceInfo();
			break;
		case DeviceSoftStateEx.Online:
			UpdateDeviceInfo();
			break;
		case DeviceSoftStateEx.Connecting:
		case DeviceSoftStateEx.Reconncting:
			break;
		}
	}

	public void UpdateDeviceInfo()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { Property: var property })
			{
				if (property != null)
				{
					modelName.Content = property.ModelName;
					battery.Content = (int)property.BatteryQuantityPercentage;
					processor.Content = property.Processor;
					imei1.Content = property.IMEI1;
					if (string.IsNullOrEmpty(property.IMEI2))
					{
						imei1.Title = "K0459";
						imei2.ItemVisibility = Visibility.Collapsed;
					}
					else
					{
						imei1.Title = "K0460";
						imei2.Content = property.IMEI2;
						imei2.ItemVisibility = Visibility.Visible;
					}
					sn.Content = property.SN;
					instorage.Total = property.TotalInternalStorage;
					instorage.Use = property.UsedInternalStorage;
					instorage.Free = property.FreeInternalStorage;
					instorage.TotalWithUnit = property.TotalInternalStorageWithUnit;
					instorage.UseWithUnit = property.UsedInternalStorageWithUnit;
					instorage.FreeWithUnit = property.FreeInternalStorageWithUnit;
					exstorage.Total = property.TotalExternalStorage;
					exstorage.Use = property.UsedExternalStorage;
					exstorage.Free = property.FreeExternalStorage;
					exstorage.TotalWithUnit = property.TotalExternalStorageWithUnit;
					exstorage.UseWithUnit = property.UsedExternalStorageWithUnit;
					exstorage.FreeWithUnit = property.FreeExternalStorageWithUnit;
					androidVer.Content = property.AndroidVersion;
					currentVersion.Content = property.GetPropertyValue("ro.build.display.id");
					int num = 0;
					{
						foreach (DeviceInfoItemViewModel item in Items)
						{
							if (item.ItemVisibility == Visibility.Visible)
							{
								item.IsOddRow = num % 2 == 0;
								num++;
							}
						}
						return;
					}
				}
				Clear();
			}
		});
	}

	public void Clear()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			foreach (DeviceInfoItemViewModel item in Items)
			{
				item.Clear();
			}
		});
	}
}
