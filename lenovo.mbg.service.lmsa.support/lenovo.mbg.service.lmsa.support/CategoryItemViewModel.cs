using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.support;

public class CategoryItemViewModel : ViewModelBase
{
	private object icon;

	private object hoverIcon;

	private string key;

	private string _value;

	public object Icon
	{
		get
		{
			return icon;
		}
		set
		{
			if (icon != value)
			{
				icon = value;
				OnPropertyChanged("Icon");
			}
		}
	}

	public object HoverIcon
	{
		get
		{
			return hoverIcon;
		}
		set
		{
			if (hoverIcon != value)
			{
				hoverIcon = value;
				OnPropertyChanged("HoverIcon");
			}
		}
	}

	public string Key
	{
		get
		{
			return key;
		}
		set
		{
			if (!(key == value))
			{
				key = value;
				OnPropertyChanged("Key");
			}
		}
	}

	public string Value
	{
		get
		{
			return _value;
		}
		set
		{
			if (!(_value == value))
			{
				_value = value;
				OnPropertyChanged("Value");
			}
		}
	}
}
