using System.Collections.Generic;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class ContactGroupItemViewModelV6 : ViewModelBase
{
	private string id = string.Empty;

	private string name = string.Empty;

	private bool _isSelected;

	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			if (!(id == value))
			{
				id = value;
				OnPropertyChanged("Id");
			}
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (!(name == value))
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}
	}

	public bool isSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;
			OnPropertyChanged("isSelected");
		}
	}

	public int DataVersion { get; set; }

	public List<ContactItemViewModelV6> Contacts { get; set; }
}
