using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GoogleAnalytics;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.UserControls;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class LeftNavigationViewModel : ViewModelBase
{
	private sealed class ViewKeys
	{
		public const string HOME = "lmsa-plugin-Device-home";

		public const string APP = "lmsa-plugin-Device-app";

		public const string PICTURE = "lmsa-plugin-Device-picture";

		public const string MUSIC = "lmsa-plugin-Device-music";

		public const string VIDEO = "lmsa-plugin-Device-video";

		public const string CONTACT = "lmsa-plugin-Device-contact";

		public const string SMS = "lmsa-plugin-Device-sms";

		public const string FILE_MGT = "lmsa-plugin-Device-fileMgt";

		public const string BACKUP_RESTORE = "lmsa-plugin-Device-backuprestore";

		public const string ONE_CLICK_CLONE = "lmsa-plugin-Device-oneclickclone";
	}

	private static LeftNavigationViewModel _singleInstance;

	private FrameworkElement _currentView;

	private bool isEnabled = true;

	private ObservableCollection<LeftNavigationItemViewModel> items;

	private LeftNavigationItemViewModel _selecttedItem;

	private LeftNavigationItemViewModel home;

	private ImageSource _connectingWayImageSource = LeftNavResources.SingleInstance.GetResource("bottom_usbDrawingImage") as ImageSource;

	private string connectingDeviceName = string.Empty;

	private Dictionary<string, string> viewAndPermissionModuleMapping = new Dictionary<string, string>
	{
		{ "lmsa-plugin-Device-home", "BasicInfo" },
		{ "lmsa-plugin-Device-app", "Apps" },
		{ "lmsa-plugin-Device-picture", "Pictures" },
		{ "lmsa-plugin-Device-music", "Songs" },
		{ "lmsa-plugin-Device-video", "Videos" },
		{ "lmsa-plugin-Device-contact", "Contacts" },
		{ "lmsa-plugin-Device-sms", "SMS" },
		{ "lmsa-plugin-Device-fileMgt", "File" },
		{ "lmsa-plugin-Device-backuprestore", "Backup" },
		{ "lmsa-plugin-Device-oneclickclone", "Backup" }
	};

	public static LeftNavigationViewModel SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new LeftNavigationViewModel();
			}
			return _singleInstance;
		}
	}

	public FrameworkElement CurrentView
	{
		get
		{
			return _currentView;
		}
		set
		{
			if (_currentView != value)
			{
				_currentView = value;
				OnPropertyChanged("CurrentView");
			}
		}
	}

	public bool IsEnabled
	{
		get
		{
			return isEnabled;
		}
		set
		{
			if (isEnabled != value)
			{
				isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
	}

	public ObservableCollection<LeftNavigationItemViewModel> Items
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

	public LeftNavigationItemViewModel SelectedItem
	{
		get
		{
			return _selecttedItem;
		}
		set
		{
			if (_selecttedItem != value)
			{
				_selecttedItem = value;
				Collect(value);
				PrivateSwitchView(_selecttedItem);
				Context.MainFrame.CurrentView = _selecttedItem.View;
				OnPropertyChanged("SelectedItem");
			}
		}
	}

	public ImageSource ConnectingWayImageSource
	{
		get
		{
			return _connectingWayImageSource;
		}
		set
		{
			if (_connectingWayImageSource != value)
			{
				_connectingWayImageSource = value;
				OnPropertyChanged("ConnectingWayImageSource");
			}
		}
	}

	public string ConnectingDeviceName
	{
		get
		{
			return connectingDeviceName;
		}
		set
		{
			if (!(connectingDeviceName == value))
			{
				connectingDeviceName = value;
				OnPropertyChanged("ConnectingDeviceName");
			}
		}
	}

	private void Collect(LeftNavigationItemViewModel menu)
	{
		BusinessType business = BusinessType.HOME;
		switch (menu.Key as string)
		{
		case "lmsa-plugin-Device-home":
			business = BusinessType.HOME;
			break;
		case "lmsa-plugin-Device-app":
			business = BusinessType.APP;
			break;
		case "lmsa-plugin-Device-picture":
			business = BusinessType.PICTURE;
			break;
		case "lmsa-plugin-Device-music":
			business = BusinessType.SONG;
			break;
		case "lmsa-plugin-Device-video":
			business = BusinessType.VIDEO;
			break;
		case "lmsa-plugin-Device-contact":
			business = BusinessType.CONTACT;
			break;
		case "lmsa-plugin-Device-fileMgt":
			business = BusinessType.FILE;
			break;
		case "lmsa-plugin-Device-backuprestore":
			business = BusinessType.BACKUPRESTORE;
			break;
		case "lmsa-plugin-Device-oneclickclone":
			business = BusinessType.ONEKEYCLONE;
			break;
		}
		HostProxy.BehaviorService.Collect(business, null);
	}

	private void PrivateSwitchView(LeftNavigationItemViewModel selectedItem)
	{
		if (selectedItem != null)
		{
			Context.Tracker.Send(HitBuilder.CreateScreenView(selectedItem.Key.ToString()).Build());
			SelectedItem = selectedItem;
		}
		ResetViewByConnectionStatus(DeviceSoftStateEx.Online);
	}

	public void ResetNavigationItemViewModel()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			foreach (LeftNavigationItemViewModel item in items)
			{
				if (item.View.DataContext is ViewModelBase viewModelBase)
				{
					viewModelBase.Reset();
				}
			}
		});
	}

	public void ResetViewByConnectionStatus(DeviceSoftStateEx status)
	{
		DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
		bool flag = masterDevice == null || masterDevice.SoftStatus != DeviceSoftStateEx.Online;
		FrameworkElement oldView = CurrentView;
		CurrentView = ((flag || SelectedItem == null) ? home.View : SelectedItem.View);
		if (oldView?.GetHashCode() == CurrentView.GetHashCode())
		{
			return;
		}
		if (oldView != null)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				(oldView.DataContext as ViewModelBase)?.Active(actived: false);
			});
		}
		ViewModelBase curVM = null;
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			curVM = CurrentView.DataContext as ViewModelBase;
			curVM.Active(actived: true);
		});
		if (HostProxy.deviceManager.MasterDevice == null)
		{
			return;
		}
		HostProxy.PermissionService.BeginConfirmAppIsReady(HostProxy.deviceManager.MasterDevice, viewAndPermissionModuleMapping[SelectedItem.Key.ToString()], null, delegate(bool? isReady)
		{
			if (isReady == true && !curVM.DataIsLoaded)
			{
				LogHelper.LogInstance.Info($"BeginConfirmAppIsReady action called! curVM:{curVM.GetType()}");
				curVM.LoadData();
			}
		});
	}

	private LeftNavigationViewModel()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			InitItems();
		});
	}

	private void InitItems()
	{
		Items = new ObservableCollection<LeftNavigationItemViewModel>();
		ObservableCollection<LeftNavigationItemViewModel> observableCollection = Items;
		LeftNavigationItemViewModel obj = new LeftNavigationItemViewModel
		{
			IconImageSource = GetIconImage("icon_home_unactived"),
			IconHoverImageSource = GetIconImage("icon_home_actived"),
			Text = "K0525",
			Key = "lmsa-plugin-Device-home",
			View = new HomePageFrame
			{
				DataContext = HomePageFrameViewModel.Single
			}
		};
		LeftNavigationItemViewModel item = obj;
		home = obj;
		observableCollection.Add(item);
		SelectedItem = home;
		Items.Add(new LeftNavigationItemViewModel
		{
			IconImageSource = GetIconImage("icon_apps_unactived"),
			IconHoverImageSource = GetIconImage("icon_apps_actived"),
			Text = "K0474",
			Key = "lmsa-plugin-Device-app",
			View = new AppMgtViewV7
			{
				DataContext = AppMgtViewModel.SingleInstance
			}
		});
		Items.Add(new LeftNavigationItemViewModel
		{
			IconImageSource = GetIconImage("icon_pictures_unactived"),
			IconHoverImageSource = GetIconImage("icon_pictures_actived"),
			Text = "K0475",
			Key = "lmsa-plugin-Device-picture",
			View = new PICMgtViewV7
			{
				DataContext = PicMgtViewModel.SingleInstance
			}
		});
		Items.Add(new LeftNavigationItemViewModel
		{
			IconImageSource = GetIconImage("icon_music_unactived"),
			IconHoverImageSource = GetIconImage("icon_music_actived"),
			Text = "K0476",
			Key = "lmsa-plugin-Device-music",
			View = new MusicMgtViewV7
			{
				DataContext = MusicViewModel.SingleInstance
			}
		});
		Items.Add(new LeftNavigationItemViewModel
		{
			IconImageSource = GetIconImage("icon_videos_unactived"),
			IconHoverImageSource = GetIconImage("icon_videos_actived"),
			Text = "K0477",
			Key = "lmsa-plugin-Device-video",
			View = new VideoMgtViewV7
			{
				DataContext = VideoMgtViewModel.SingleInstance
			}
		});
		Items.Add(new LeftNavigationItemViewModel
		{
			IconImageSource = GetIconImage("icon_contacts_unactived"),
			IconHoverImageSource = GetIconImage("icon_contacts_actived"),
			Text = "K0478",
			Key = "lmsa-plugin-Device-contact",
			View = new ContactMgtViewV6
			{
				DataContext = ContactViewModel.SingleInstance
			}
		});
	}

	private ImageSource GetIconImage(string _iconName)
	{
		return LeftNavResources.SingleInstance.GetResource(_iconName) as ImageSource;
	}

	public void SetConncetedDevice(string Model, ConnectType connectionSource)
	{
		ConnectingDeviceName = Model;
		if (ConnectType.Wifi == connectionSource)
		{
			ConnectingWayImageSource = LeftNavResources.SingleInstance.GetResource("bottom_wifiDrawingImage") as ImageSource;
		}
		else
		{
			ConnectingWayImageSource = LeftNavResources.SingleInstance.GetResource("bottom_usbDrawingImage") as ImageSource;
		}
	}

	public void DisconnectingDevice()
	{
		ConnectingDeviceName = string.Empty;
	}

	public void Switch(object key)
	{
		if (key != null)
		{
			SelectedItem = Items.FirstOrDefault((LeftNavigationItemViewModel n) => n.Key == key);
		}
	}
}
