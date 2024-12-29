using System.Collections.ObjectModel;

namespace lenovo.mbg.service.lmsa;

public class MainViewDataModel
{
	public ObservableCollection<NavigationItemDataModel> NagivationItems { get; set; }

	public string CurrentVersion { get; set; }
}
