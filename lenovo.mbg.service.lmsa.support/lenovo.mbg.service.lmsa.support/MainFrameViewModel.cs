using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.messenger;
using lenovo.mbg.service.lmsa.support.Commons;
using lenovo.mbg.service.lmsa.support.UserControls;
using lenovo.mbg.service.lmsa.support.ViewContext;
using lenovo.mbg.service.lmsa.support.ViewModel;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.support;

public class MainFrameViewModel : ViewModelBase
{
	public static string DEVICE_TYPE_STORAGE_KEY = "{D86FEEC4-62D8-4DD5-88ED-181481200D4A}";

	public static string MOTO_PHONE = "motoPhone";

	public static string LENOVO_PHONE = "lenovoPhone";

	public static string LENOVO_TABLET = "lenovoTablet";

	private ReplayCommand backHomeCommand;

	private object _currentView;

	private string _robotPageCountry = string.Empty;

	private CategoryItemViewModel selectedCategoryItem;

	protected volatile bool specialFlag;

	protected ViewType specialViewType;

	protected ViewType currentViewType;

	private LeftNavigationItemViewModel _selecttedItem;

	private MainFrameControlTemplateSelector mainFrameControlTemplateSelector;

	private ReplayCommand deviceTypeSelectedCommand;

	private ObservableCollection<CategoryItemViewModel> categoryItemsSource;

	private string testP;

	public ReplayCommand BackHomeCommand
	{
		get
		{
			return backHomeCommand;
		}
		set
		{
			if (backHomeCommand != value)
			{
				backHomeCommand = value;
				OnPropertyChanged("BackHomeCommand");
			}
		}
	}

	public object CurrentView
	{
		get
		{
			return _currentView;
		}
		set
		{
			_currentView = value;
			OnPropertyChanged("CurrentView");
		}
	}

	public ObservableCollection<LeftNavigationItemViewModel> Items { get; set; }

	protected string RobotPageUri { get; set; }

	public CategoryItemViewModel SelectedCategoryItem
	{
		get
		{
			return selectedCategoryItem;
		}
		set
		{
			if (!specialFlag && (selectedCategoryItem == value || value == null))
			{
				return;
			}
			selectedCategoryItem = value;
			FileHelper.WriteJsonWithAesEncrypt(Configurations.DefaultOptionsFileName, DEVICE_TYPE_STORAGE_KEY, value.Key);
			AddOrUpdateRobotNavigateItem(value.Key);
			if (specialFlag)
			{
				if (CurrentView != null && currentViewType != specialViewType)
				{
					FrameworkElement frameworkElement = Context.FindView(Context.view[specialViewType].ViewType);
					if (frameworkElement != null)
					{
						Context.ChangeCategory(specialViewType, value.Key);
						(frameworkElement.DataContext as ViewModelBase).LoadData(value.Key);
					}
				}
				specialFlag = false;
			}
			else if (CurrentView != null)
			{
				Context.ChangeCategory(currentViewType, value.Key);
				((CurrentView as FrameworkElement).DataContext as ViewModelBase).LoadData(value.Key);
			}
			OnPropertyChanged("SelectedCategoryItem");
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
			if (_selecttedItem != value && value != null)
			{
				_selecttedItem = value;
				ViewType viewType = (currentViewType = (ViewType)Enum.Parse(typeof(ViewType), value.Key.ToString()));
				Context.Switch(viewType, SelectedCategoryItem?.Key);
				if (viewType == ViewType.MOLI)
				{
					((MessengerFrame)CurrentView).LoadMoliOrLenaUrl(RobotPageUri, _robotPageCountry);
				}
				OnPropertyChanged("SelectedItem");
			}
		}
	}

	public MainFrameControlTemplateSelector MainFrameControlTemplateSelector
	{
		get
		{
			return mainFrameControlTemplateSelector;
		}
		set
		{
			if (mainFrameControlTemplateSelector != value)
			{
				mainFrameControlTemplateSelector = value;
				OnPropertyChanged("MainFrameControlTemplateSelector");
			}
		}
	}

	public ReplayCommand DeviceTypeSelectedCommand
	{
		get
		{
			return deviceTypeSelectedCommand;
		}
		set
		{
			if (deviceTypeSelectedCommand != value)
			{
				deviceTypeSelectedCommand = value;
				OnPropertyChanged("DeviceTypeSelectedCommand");
			}
		}
	}

	public ObservableCollection<CategoryItemViewModel> CategoryItemsSource
	{
		get
		{
			return categoryItemsSource;
		}
		set
		{
			if (categoryItemsSource != value)
			{
				categoryItemsSource = value;
				OnPropertyChanged("CategoryItemsSource");
			}
		}
	}

	public string TestP
	{
		get
		{
			return testP;
		}
		set
		{
			if (!(testP == value))
			{
				testP = value;
				OnPropertyChanged("TestP");
			}
		}
	}

	public override void LoadData()
	{
		base.LoadData();
		InitializeNav();
	}

	private void BackHomeCommandHandler(object args)
	{
		((ViewModelBase)lenovo.mbg.service.lmsa.support.Commons.ViewContext.SwitchView<SearchViewEx>().DataContext).LoadData(null);
	}

	private void InitializeNav()
	{
		MainFrameControlTemplateSelector = new MainFrameControlTemplateSelector();
		DeviceTypeSelectedCommand = new ReplayCommand(DeviceTypeSelectedCommandHandler);
		BackHomeCommand = new ReplayCommand(BackHomeCommandHandler);
		CategoryItemsSource = new ObservableCollection<CategoryItemViewModel>();
		CategoryItemsSource.Add(new CategoryItemViewModel
		{
			Key = MOTO_PHONE,
			Value = "K0812",
			Icon = Application.Current.FindResource("CommonboxmotoPhonerawingImage"),
			HoverIcon = Application.Current.FindResource("CommonboxmotoPhonerawingImageWhite")
		});
		CategoryItemsSource.Add(new CategoryItemViewModel
		{
			Key = LENOVO_PHONE,
			Value = "K0810",
			Icon = Application.Current.FindResource("CommonboxPhonerawingImage"),
			HoverIcon = Application.Current.FindResource("CommonboxPhonerawingImageWhite")
		});
		CategoryItemsSource.Add(new CategoryItemViewModel
		{
			Key = LENOVO_TABLET,
			Value = "K0811",
			Icon = Application.Current.FindResource("CommonboxTabletrawingImage"),
			HoverIcon = Application.Current.FindResource("CommonboxTabletrawingImageWhite")
		});
		Items = new ObservableCollection<LeftNavigationItemViewModel>();
		Items.Add(new LeftNavigationItemViewModel
		{
			IconImageSource = (Application.Current.Resources["support_nav_tips_drawingImage"] as ImageSource),
			IconHoverImageSource = (Application.Current.Resources["support_nav_tips_selected_drawingImage"] as ImageSource),
			Text = "K0260",
			Key = ViewType.TIPS
		});
		Items.Add(new LeftNavigationItemViewModel
		{
			IconImageSource = (Application.Current.Resources["support_nav_forum_drawingImage"] as ImageSource),
			IconHoverImageSource = (Application.Current.Resources["support_nav_forum_selected_drawingImage"] as ImageSource),
			Text = "K0259",
			Key = ViewType.FORUM
		});
		Items.Add(new LeftNavigationItemViewModel
		{
			IconImageSource = (Application.Current.Resources["support_nav_warrenty_drawingImage"] as ImageSource),
			IconHoverImageSource = (Application.Current.Resources["support_nav_warrenty_selected_drawingImage"] as ImageSource),
			Text = "K0263",
			Key = ViewType.WARRTETY,
			ItemVisibility = Visibility.Collapsed
		});
		Items.Add(new LeftNavigationItemViewModel
		{
			IconImageSource = (Application.Current.Resources["support_nav_moli_drawingImage"] as ImageSource),
			IconHoverImageSource = (Application.Current.Resources["support_nav_moli_selected_drawingImage"] as ImageSource),
			Key = ViewType.MOLI,
			ItemVisibility = Visibility.Collapsed
		});
		LoadWarrantyAsync();
		string category = GetCurrentCategoryKey();
		SelectedCategoryItem = ((!string.IsNullOrEmpty(category)) ? CategoryItemsSource.FirstOrDefault((CategoryItemViewModel m) => m.Key.Equals(category)) : null);
		SelectedItem = Items.First();
	}

	public void AddOrUpdateRobotNavigateItem(string category)
	{
		if (!(HostProxy.GlobalCache.Get("countrySupportedMoliLenaList") is JObject jObject))
		{
			RobotPageUri = string.Empty;
			return;
		}
		RobotPageUri = jObject.SelectToken("$.*[?(@.deviceType == '" + category + "')].url", errorWhenNoMatch: false)?.Value<string>();
		_robotPageCountry = jObject.SelectToken("$.*[?(@.deviceType == '" + category + "')].country", errorWhenNoMatch: false)?.Value<string>();
		_robotPageCountry = (string.IsNullOrWhiteSpace(_robotPageCountry) ? string.Empty : _robotPageCountry);
		LeftNavigationItemViewModel item = Items.FirstOrDefault((LeftNavigationItemViewModel p) => (ViewType)p.Key == ViewType.MOLI);
		if (string.IsNullOrEmpty(RobotPageUri))
		{
			HostProxy.CurrentDispatcher?.Invoke(() => item.ItemVisibility = Visibility.Collapsed);
			if (SelectedItem != null && (ViewType)SelectedItem.Key == ViewType.MOLI)
			{
				SelectedItem = Items.First();
			}
		}
		else
		{
			ProcessMoli(item, category);
		}
	}

	public void SelectTarget(ViewType viewType, string category)
	{
		specialFlag = true;
		specialViewType = viewType;
		SelectedCategoryItem = ((!string.IsNullOrEmpty(category)) ? CategoryItemsSource.FirstOrDefault((CategoryItemViewModel m) => m.Key.Equals(category)) : SelectedCategoryItem);
		SelectedItem = Items.FirstOrDefault((LeftNavigationItemViewModel m) => (ViewType)m.Key == viewType) ?? SelectedItem;
	}

	private void ProcessMoli(LeftNavigationItemViewModel item, string category)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			item.ItemVisibility = Visibility.Visible;
			if (category.Equals("motoPhone"))
			{
				item.Text = ((RobotPageUri.StartsWith("https://br.") || RobotPageUri.StartsWith("https://las.")) ? "MIA" : "Moli");
			}
			else
			{
				item.Text = "Lena";
			}
			if (SelectedItem == item)
			{
				((MessengerFrame)CurrentView).LoadMoliOrLenaUrl(RobotPageUri, _robotPageCountry);
			}
		});
	}

	private async Task LoadWarrantyAsync()
	{
		await Task.Run(delegate
		{
			if (!HostProxy.LanguageService.IsChinaRegionAndLanguage())
			{
				HostProxy.CurrentDispatcher.BeginInvoke((Action)delegate
				{
					Items.First((LeftNavigationItemViewModel n) => n.Key.ToString() == ViewType.WARRTETY.ToString()).ItemVisibility = Visibility.Visible;
				}, null);
			}
		});
	}

	private string GetCurrentCategoryKey()
	{
		return FileHelper.ReadWithAesDecrypt(Configurations.DefaultOptionsFileName, DEVICE_TYPE_STORAGE_KEY);
	}

	private void DeviceTypeSelectedCommandHandler(object args)
	{
		MainFrameControlTemplateSelector = new MainFrameControlTemplateSelector();
		SelectedCategoryItem = CategoryItemsSource.FirstOrDefault((CategoryItemViewModel m) => m.Key == args.ToString());
	}
}
