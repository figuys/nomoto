using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class OperatorListBoxViewModelV6 : ViewModelBase
{
	private readonly ObservableCollection<OperatorButtonViewModelV6> oItems;

	public ObservableCollection<OperatorButtonViewModelV6> Items => oItems;

	public OperatorButtonViewModelV6 ClickItem { get; set; }

	public void Visibility(Visibility visibility, params string[] keys)
	{
		if (keys == null)
		{
			return;
		}
		foreach (OperatorButtonViewModelV6 item in Items.Where((OperatorButtonViewModelV6 m) => keys.Contains(m.ButtonText)))
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
		foreach (OperatorButtonViewModelV6 item in Items.Where((OperatorButtonViewModelV6 m) => keys.Contains(m.ButtonText)))
		{
			item.IsEnabled = isEnabled;
		}
	}

	public OperatorListBoxViewModelV6()
	{
		oItems = new ObservableCollection<OperatorButtonViewModelV6>();
	}
}
