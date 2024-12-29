using System.Collections.ObjectModel;

namespace lenovo.mbg.service.lmsa;

public class NavigationItemCollection
{
	private ObservableCollection<NavigationItemDataModel> nagivationItems = new ObservableCollection<NavigationItemDataModel>();

	public ObservableCollection<NavigationItemDataModel> NagivationItems => nagivationItems;
}
