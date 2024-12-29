using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class OperatorListBoxViewModel : ViewModelBase
{
	private readonly ObservableCollection<OperatorButtonViewModel> oItems;

	public ObservableCollection<OperatorButtonViewModel> Items => oItems;

	public OperatorButtonViewModel ClickItem { get; set; }

	public void Visibility(Visibility visibility, params string[] keys)
	{
		if (keys == null)
		{
			return;
		}
		foreach (OperatorButtonViewModel item in Items.Where((OperatorButtonViewModel m) => keys.Contains(m.ButtonText)))
		{
			item.Visibility = visibility;
		}
	}

	public void Enable(bool isEnabled, params string[] keys)
	{
		if (keys == null)
		{
			return;
		}
		foreach (OperatorButtonViewModel item in Items.Where((OperatorButtonViewModel m) => keys.Contains(m.ButtonText)))
		{
			item.IsEnabled = isEnabled;
		}
	}

	public OperatorListBoxViewModel()
	{
		oItems = new ObservableCollection<OperatorButtonViewModel>();
	}
}
