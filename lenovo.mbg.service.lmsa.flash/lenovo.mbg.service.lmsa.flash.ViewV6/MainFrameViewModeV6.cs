using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public class MainFrameViewModeV6 : ViewModelBase
{
	private Visibility footerVisible = Visibility.Collapsed;

	private Visibility backVisible = Visibility.Collapsed;

	private Visibility homeVisible = Visibility.Collapsed;

	private Visibility returnRescueVisible = Visibility.Collapsed;

	private FrameworkElement _CurrentView;

	private SolidColorBrush _PageBackground;

	private SolidColorBrush _BorderBrush;

	private Style _BackBtnStyle;

	private ObservableCollection<DeviceViewModel> devices = new ObservableCollection<DeviceViewModel>();

	public Visibility FooterVisible
	{
		get
		{
			return footerVisible;
		}
		set
		{
			footerVisible = value;
			OnPropertyChanged("FooterVisible");
		}
	}

	public Visibility BackVisible
	{
		get
		{
			return backVisible;
		}
		set
		{
			backVisible = value;
			OnPropertyChanged("BackVisible");
		}
	}

	public Visibility HomeVisible
	{
		get
		{
			return homeVisible;
		}
		set
		{
			homeVisible = value;
			OnPropertyChanged("HomeVisible");
		}
	}

	public Visibility ReturnRescueVisible
	{
		get
		{
			return returnRescueVisible;
		}
		set
		{
			returnRescueVisible = value;
			OnPropertyChanged("ReturnRescueVisible");
		}
	}

	public FrameworkElement CurrentView
	{
		get
		{
			return _CurrentView;
		}
		set
		{
			_CurrentView = value;
			OnPropertyChanged("CurrentView");
		}
	}

	public SolidColorBrush PageBackground
	{
		get
		{
			return _PageBackground;
		}
		set
		{
			_PageBackground = value;
			OnPropertyChanged("PageBackground");
		}
	}

	public SolidColorBrush BorderBrush
	{
		get
		{
			return _BorderBrush;
		}
		set
		{
			_BorderBrush = value;
			OnPropertyChanged("BorderBrush");
		}
	}

	public Style BackBtnStyle
	{
		get
		{
			return _BackBtnStyle;
		}
		set
		{
			_BackBtnStyle = value;
			OnPropertyChanged("BackBtnStyle");
		}
	}

	public ObservableCollection<DeviceViewModel> Devices
	{
		get
		{
			return devices;
		}
		set
		{
			devices = value;
			OnPropertyChanged("Devices");
		}
	}

	public int Count => Devices.Count((DeviceViewModel n) => !string.IsNullOrEmpty(n.Id));

	public Dictionary<PageIndex, FrameworkElement> PageViewArr { get; private set; }

	public ReplayCommand RemoveDeviceCommand { get; }

	public ReplayCommand ClickCommand { get; }

	public MainFrameViewModeV6()
	{
		Devices.Add(new DeviceViewModel
		{
			IsEnabled = true
		});
		Devices.Add(new DeviceViewModel());
		Devices.Add(new DeviceViewModel());
		RemoveDeviceCommand = new ReplayCommand(RemoveDeviceCommandHandler);
		ClickCommand = new ReplayCommand(ClickCommandHandler);
		PageViewArr = new Dictionary<PageIndex, FrameworkElement> { 
		{
			PageIndex.RESCUE_HOME,
			new RescueHomeViewV6()
		} };
		CurrentView = PageViewArr[PageIndex.RESCUE_HOME];
	}

	public IAMatchView InitPhoneMatchView(AutoMatchResource data, object wModel, FrameworkElement parentView, bool jumpToMatchView = false)
	{
		bool supportMulti = MainFrameV6.Instance.SupportMulti;
		DeviceViewModel deviceViewModel = ((!jumpToMatchView && supportMulti) ? Devices.FirstOrDefault((DeviceViewModel n) => n.Id == data.Id) : Devices.First());
		bool flag = deviceViewModel != null;
		if (!flag)
		{
			deviceViewModel = Devices.FirstOrDefault((DeviceViewModel n) => string.IsNullOrEmpty(n.Id));
			flag = deviceViewModel != null;
		}
		if (flag)
		{
			deviceViewModel.Id = data.Id;
			deviceViewModel.IMEI = (string.IsNullOrEmpty(data.deviceInfo.imei) ? null : data.deviceInfo.imei);
			deviceViewModel.ModelName = Regex.Replace(data.deviceInfo.modelName ?? data.resource.ModelName, "(^lenovo)|(^motorola)", "", RegexOptions.IgnoreCase).Trim();
			deviceViewModel.IsMotorola = !string.IsNullOrEmpty(data.resource.Brand) && data.resource.Brand.Equals("motorola", StringComparison.CurrentCultureIgnoreCase);
			deviceViewModel.IsSelected = true;
			deviceViewModel.IsEnabled = true;
			if (deviceViewModel.View == null)
			{
				deviceViewModel.View = new PhoneAMatchViewV6();
				MainFrameV6.Instance.RescueViews.Add(deviceViewModel.View);
				deviceViewModel.View.Init(data, wModel, parentView);
			}
			else if (!deviceViewModel.View.VM.UcDevice.Locked || jumpToMatchView)
			{
				if (deviceViewModel.View.VM.UcDevice.Locked)
				{
					deviceViewModel.View.VM.Free();
				}
				deviceViewModel.View.Init(data, wModel, parentView);
			}
		}
		return deviceViewModel?.View;
	}

	public void ChangeRescuingPercentage(string id, double percentage)
	{
		DeviceViewModel deviceViewModel = Devices.FirstOrDefault((DeviceViewModel n) => n.Id == id);
		if (deviceViewModel != null)
		{
			deviceViewModel.Percentage = percentage;
		}
	}

	public void ChangeRescuingNeedOperator(string id, bool needOperator)
	{
		DeviceViewModel found = Devices.FirstOrDefault((DeviceViewModel n) => n.Id == id);
		if (found != null)
		{
			Application.Current.Dispatcher.Invoke(() => found.NeedOperator = needOperator);
		}
	}

	public void ChangeDeviceEnable()
	{
		if (Devices.Where((DeviceViewModel n) => !string.IsNullOrEmpty(n.Id)).Count((DeviceViewModel n) => n.View.VM.UcDevice.RealFlash) > 0)
		{
			DeviceViewModel deviceViewModel = Devices.FirstOrDefault((DeviceViewModel n) => string.IsNullOrEmpty(n.Id));
			if (deviceViewModel != null)
			{
				deviceViewModel.IsEnabled = true;
			}
		}
		else
		{
			DeviceViewModel deviceViewModel2 = Devices.FirstOrDefault((DeviceViewModel n) => string.IsNullOrEmpty(n.Id) && n.IsEnabled);
			if (deviceViewModel2 != null)
			{
				deviceViewModel2.IsEnabled = false;
			}
		}
	}

	public void RemoveDeviceCommandHandler(object data)
	{
		lock (this)
		{
			if (data == null)
			{
				return;
			}
			DeviceViewModel deviceViewModel = data as DeviceViewModel;
			deviceViewModel.View.VM.Free();
			MainFrameV6.Instance.RescueViews.Remove(deviceViewModel.View);
			Devices.Remove(deviceViewModel);
			if (Devices.Count < 3)
			{
				Devices.Add(new DeviceViewModel());
				ChangeDeviceEnable();
			}
			if (Devices.Count((DeviceViewModel n) => !string.IsNullOrEmpty(n.Id)) == 0)
			{
				MainFrameV6.Instance.ChangeView(PageIndex.PHONE_ENTRANCE);
				return;
			}
			DeviceViewModel data2 = Devices.FirstOrDefault((DeviceViewModel n) => !string.IsNullOrEmpty(n.Id));
			ClickCommandHandler(data2);
		}
	}

	private void ClickCommandHandler(object data)
	{
		MouseButtonEventArgs mouseButtonEventArgs = null;
		DeviceViewModel deviceViewModel = data as DeviceViewModel;
		if (deviceViewModel == null)
		{
			mouseButtonEventArgs = data as MouseButtonEventArgs;
			deviceViewModel = ((!(mouseButtonEventArgs.Source is FrameworkElement frameworkElement)) ? ((mouseButtonEventArgs.Source as Run).DataContext as DeviceViewModel) : (frameworkElement.DataContext as DeviceViewModel));
		}
		if (deviceViewModel != null)
		{
			if (deviceViewModel.View == null)
			{
				if (Devices.Count((DeviceViewModel n) => n.View != null && !n.View.VM.SupportFastboot) > 0)
				{
					deviceViewModel.ShowTip = true;
				}
				else
				{
					MainFrameV6.Instance.ChangeView(PageIndex.PHONE_ENTRANCE);
				}
			}
			else
			{
				deviceViewModel.IsSelected = true;
				CurrentView = deviceViewModel.View.RescueView ?? (deviceViewModel.View as FrameworkElement);
				MainFrameV6.Instance.ChangeMutilDeviceShowType(MainFrameV6.Instance.SupportMulti);
			}
		}
		if (mouseButtonEventArgs != null)
		{
			mouseButtonEventArgs.Handled = true;
		}
	}
}
