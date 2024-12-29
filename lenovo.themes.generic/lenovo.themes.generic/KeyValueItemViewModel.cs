using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic;

public class KeyValueItemViewModel : ViewModelBase
{
	private object itemKey = string.Empty;

	private object itemValue = string.Empty;

	public object ItemKey
	{
		get
		{
			return itemKey;
		}
		set
		{
			if (itemKey != value)
			{
				itemKey = value;
				OnPropertyChanged("ItemKey");
			}
		}
	}

	public object ItemValue
	{
		get
		{
			return itemValue;
		}
		set
		{
			if (itemValue != value)
			{
				itemValue = value;
				OnPropertyChanged("ItemValue");
			}
		}
	}
}
