using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class CategoriesViewModel : ViewModelBase
{
	private double _progressBarMinimum;

	private double _progressBarMaximum = 100.0;

	private double _progressBarValue;

	private bool _isTransferring;

	private bool _isEnabled = true;

	private Visibility _LoadingProcessVisibility = Visibility.Collapsed;

	private CategoryViewModel _SelectedItem;

	public ReplayCommand ToggleCilckCommand { get; }

	public double ProgressBarMinimum
	{
		get
		{
			return _progressBarMinimum;
		}
		set
		{
			if (_progressBarMinimum != value)
			{
				_progressBarMinimum = value;
				OnPropertyChanged("ProgressBarMinimum");
			}
		}
	}

	public double ProgressBarMaximum
	{
		get
		{
			return _progressBarMaximum;
		}
		set
		{
			if (_progressBarMaximum != value)
			{
				_progressBarMaximum = value;
				OnPropertyChanged("ProgressBarMaximum");
			}
		}
	}

	public double ProgressBarValue
	{
		get
		{
			return _progressBarValue;
		}
		set
		{
			if (_progressBarValue != value)
			{
				_progressBarValue = value;
				OnPropertyChanged("ProgressBarValue");
			}
		}
	}

	public bool IsTransferring
	{
		get
		{
			return _isTransferring;
		}
		set
		{
			if (_isTransferring != value)
			{
				_isTransferring = value;
				OnPropertyChanged("IsTransferring");
			}
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			if (_isEnabled != value)
			{
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
	}

	protected static bool IsMotoHelper
	{
		get
		{
			if (Context.CurrentDevice != null)
			{
				return Context.CurrentDevice.ConnectedAppType == "Moto";
			}
			return false;
		}
	}

	private static bool IsSMSDisabled
	{
		get
		{
			DeviceEx currentDevice = Context.CurrentDevice;
			if (currentDevice != null && currentDevice.Property?.ApiLevel >= 35)
			{
				return Configurations.AppVersionCode < 50;
			}
			return false;
		}
	}

	public Visibility LoadingProcessVisibility
	{
		get
		{
			return _LoadingProcessVisibility;
		}
		set
		{
			if (_LoadingProcessVisibility != value)
			{
				_LoadingProcessVisibility = value;
				OnPropertyChanged("LoadingProcessVisibility");
			}
		}
	}

	public List<CategoryViewModel> CategoriesList { get; private set; }

	public ObservableCollection<CategoryViewModel> Categories { get; set; }

	public CategoryViewModel SelectedItem
	{
		get
		{
			return _SelectedItem;
		}
		set
		{
			_SelectedItem = value;
			FireCategorySelectionChanged(this, null);
			OnPropertyChanged("SelectedItem");
		}
	}

	public event EventHandler<CategorySelectionChangedEventArgs> CategorySelectionChanged;

	public CategoriesViewModel(bool isBackup)
	{
		Categories = new ObservableCollection<CategoryViewModel>();
		ToggleCilckCommand = new ReplayCommand(ToggleCilckCommandHandler);
		List<CategoryViewModel> list = new List<CategoryViewModel>
		{
			new CategoryViewModel
			{
				CenterIconSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Sms"),
				CenterIconUnSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Sms"),
				CountNum = 0,
				Title = "K0479",
				IsSelected = false,
				IsChecked = false,
				IsEnabled = true,
				Opacity = (IsSMSDisabled ? 0.5 : 1.0),
				ResourceType = "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}",
				SubTitle = (isBackup ? "K1516" : "K1402"),
				CategoryClickAction = FireAndroid15CategoryClickAction,
				ToggleButtonVisibility = Visibility.Collapsed,
				Key = LangTranslation.Translate("K0479")
			},
			new CategoryViewModel
			{
				CenterIconSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Calllog"),
				CenterIconUnSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Calllog"),
				CountNum = 0,
				Title = "K1628",
				IsSelected = false,
				IsChecked = false,
				IsEnabled = true,
				Opacity = (IsMotoHelper ? 0.5 : 1.0),
				SubTitle = (isBackup ? "K1516" : "K1402"),
				ResourceType = "{89D4DB68-4258-4002-8557-E65959C558B3}",
				ToggleButtonVisibility = Visibility.Collapsed,
				CategoryClickAction = FireCategoryClickAction,
				Key = LangTranslation.Translate("K0509")
			},
			new CategoryViewModel
			{
				CenterIconSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Contacts"),
				CenterIconUnSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Contacts"),
				CountNum = 0,
				Title = "K0478",
				IsSelected = false,
				IsChecked = false,
				SubTitle = (isBackup ? "K1516" : "K1402"),
				ResourceType = "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}",
				ToggleButtonVisibility = Visibility.Collapsed,
				Key = LangTranslation.Translate("K0478")
			},
			new CategoryViewModel
			{
				CenterIconSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Files"),
				CenterIconUnSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Files"),
				CountNum = 0,
				Title = "K1517",
				IsChecked = false,
				IsSelected = false,
				IsEnabled = true,
				Opacity = (IsMotoHelper ? 0.5 : 1.0),
				ResourceType = "{580C48C8-6CEF-4BBB-AF37-D880B349D142}",
				SubTitle = (isBackup ? "K1516" : "K1402"),
				ToggleButtonVisibility = Visibility.Collapsed,
				CategoryClickAction = FireCategoryClickAction,
				Key = LangTranslation.Translate("K1517")
			},
			new CategoryViewModel
			{
				CenterIconSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Pictures"),
				CenterIconUnSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Pictures"),
				CountNum = 0,
				IsSelected = false,
				IsChecked = false,
				Title = "K0475",
				ResourceType = "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}",
				SubTitle = (isBackup ? "K1516" : "K1402"),
				ToggleButtonVisibility = Visibility.Collapsed,
				Key = LangTranslation.Translate("K0475")
			},
			new CategoryViewModel
			{
				CenterIconSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Music"),
				CenterIconUnSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Music"),
				CountNum = 0,
				IsSelected = false,
				IsChecked = false,
				Title = "K0476",
				ResourceType = "{242C8F16-6AC7-431B-BBF1-AE24373860F1}",
				SubTitle = (isBackup ? "K1516" : "K1402"),
				ToggleButtonVisibility = Visibility.Collapsed,
				Key = LangTranslation.Translate("K0476")
			},
			new CategoryViewModel
			{
				CenterIconSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Videos"),
				CenterIconUnSelectedSource = (ImageSource)Application.Current.FindResource("v6_Icon_Videos"),
				Title = "K0477",
				CountNum = 0,
				IsChecked = false,
				IsSelected = false,
				ResourceType = "{8BEBE14B-4E45-4D36-8726-8442E6242C01}",
				SubTitle = (isBackup ? "K1516" : "K1402"),
				ToggleButtonVisibility = Visibility.Collapsed,
				Key = LangTranslation.Translate("K0477")
			}
		};
		if (isBackup)
		{
			CategoriesList = list;
		}
		else
		{
			CategoryViewModel item = list[0];
			list.RemoveAt(0);
			list.Add(item);
			CategoriesList = list;
		}
		if (!(Context.CurrentDevice.Property.Category == "tablet"))
		{
			return;
		}
		foreach (CategoryViewModel categories in CategoriesList)
		{
			if (categories.ResourceType == "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}" || categories.ResourceType == "{89D4DB68-4258-4002-8557-E65959C558B3}")
			{
				categories.Opacity = 0.51;
			}
		}
	}

	private void ToggleCilckCommandHandler(object data)
	{
		CategoryViewModel obj = data as CategoryViewModel;
		obj.IsOpen = !obj.IsOpen;
	}

	private static void FireAndroid15CategoryClickAction(CategoryViewModel category)
	{
		Context.MessageBox.ShowMessage("K0071", "K1893", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
	}

	private static void FireCategoryClickAction(CategoryViewModel category)
	{
		if (Context.MessageBox.ShowMessage("K0071", "K1598", "K0327", "K0208", isCloseBtn: false, null, MessageBoxImage.Exclamation) == true)
		{
			Configurations.AppMinVersionCodeOfMoto = 900805000;
			if (Context.CurrentDevice is AdbDeviceEx adbDeviceEx)
			{
				adbDeviceEx.PhysicalStatus = DevicePhysicalStateEx.Offline;
			}
		}
	}

	public void LoadCategories(List<CategoryViewModel> datas)
	{
		Categories.Clear();
		datas?.ForEach(delegate(CategoryViewModel p)
		{
			Categories.Add(p);
		});
	}

	public override void Reset()
	{
		CategoriesList.ForEach(delegate(CategoryViewModel p)
		{
			p.Count = 0;
			p.CountNum = 0;
			p.Childs?.Clear();
			p.Childs = null;
			p.IsSelected = false;
			p.ToggleButtonVisibility = Visibility.Collapsed;
		});
	}

	private void FireCategorySelectionChanged(object sender, CategorySelectionChangedEventArgs e)
	{
		this.CategorySelectionChanged?.Invoke(sender, e);
	}
}
