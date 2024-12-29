using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class LComboBoxViewModel : ViewModelBase
{
	private string itemText;

	private object tab;

	private string selectionText = "";

	public string ItemText
	{
		get
		{
			return itemText;
		}
		set
		{
			if (!(itemText == value))
			{
				itemText = value;
				OnPropertyChanged("ItemText");
			}
		}
	}

	public object Tag
	{
		get
		{
			return tab;
		}
		set
		{
			if (tab != value)
			{
				tab = value;
				OnPropertyChanged("Tag");
			}
		}
	}

	public string SelectionText
	{
		get
		{
			return selectionText;
		}
		set
		{
			if (!(selectionText == value))
			{
				selectionText = value;
				OnPropertyChanged("SelectionText");
			}
		}
	}
}
