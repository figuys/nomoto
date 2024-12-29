using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.ModelV6;

public class ManualComboboxViewModel : NotifyBase
{
	private string itemText;

	private object tab;

	private bool isUsed;

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

	public bool IsUsed
	{
		get
		{
			return isUsed;
		}
		set
		{
			isUsed = value;
			OnPropertyChanged("IsUsed");
		}
	}

	public bool IsMore { get; set; }
}
