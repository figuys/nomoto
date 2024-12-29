using System.Collections.ObjectModel;
using System.Windows;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels.SystemOperation;

public class HelpOperationViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private ObservableCollection<MouseOverMenuItemViewModel> menuItems;

	public ObservableCollection<MouseOverMenuItemViewModel> MenuItems
	{
		get
		{
			return menuItems;
		}
		set
		{
			if (menuItems != value)
			{
				menuItems = value;
				OnPropertyChanged("MenuItems");
			}
		}
	}

	public HelpOperationViewModel()
	{
		MenuItems = new ObservableCollection<MouseOverMenuItemViewModel>
		{
			new FaqOperationItemViewModel
			{
				ItemVisibility = Visibility.Visible
			},
			new UserGuidOperationItemViewModel
			{
				ItemVisibility = Visibility.Visible
			}
		};
	}
}
