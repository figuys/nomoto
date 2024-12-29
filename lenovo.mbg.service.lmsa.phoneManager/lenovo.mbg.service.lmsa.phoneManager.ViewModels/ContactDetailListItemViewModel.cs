using System;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class ContactDetailListItemViewModel : ViewModelBase
{
	private string displayTitle;

	private string displayValue = string.Empty;

	private bool _isTop = true;

	public string DisplayTitle
	{
		get
		{
			if (!IsTop && !Enum.IsDefined(typeof(DetailType), displayTitle))
			{
				displayTitle = DetailType.Other.ToString();
			}
			return displayTitle;
		}
		set
		{
			if (!(displayTitle == value))
			{
				displayTitle = value;
				OnPropertyChanged("DisplayTitle");
			}
		}
	}

	public string DisplayValue
	{
		get
		{
			return displayValue;
		}
		set
		{
			if (!(displayValue == value))
			{
				displayValue = value;
				OnPropertyChanged("DisplayValue");
			}
		}
	}

	public bool IsTop
	{
		get
		{
			return _isTop;
		}
		set
		{
			if (_isTop != value)
			{
				_isTop = value;
				OnPropertyChanged("IsTop");
			}
		}
	}
}
