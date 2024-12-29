using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class ContactItemViewModel : ViewModelBase
{
	private bool _isSelected;

	private string _name = "";

	private string _number = "";

	private List<ContactItemPhoneViewModel> numberList;

	private ReplayCommand checkClickCommand;

	private ReplayCommand showContactDetailClickCommand;

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

	[DefaultValue("-1")]
	public string Id { get; set; }

	public string Acronym { get; set; }

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
			OnPropertyChanged("Name");
		}
	}

	public string SortKey { get; set; }

	public string Number
	{
		get
		{
			return _number;
		}
		set
		{
			_number = value;
			OnPropertyChanged("Number");
		}
	}

	public List<ContactItemPhoneViewModel> NumberList
	{
		get
		{
			return numberList;
		}
		set
		{
			if (numberList == value)
			{
				return;
			}
			numberList = value;
			if (numberList != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				ContactItemPhoneViewModel contactItemPhoneViewModel = null;
				for (int i = 0; i < numberList.Count; i++)
				{
					contactItemPhoneViewModel = numberList[i];
					if (!string.IsNullOrEmpty(contactItemPhoneViewModel.Content))
					{
						stringBuilder.Append((i == 0) ? "" : ",").Append(contactItemPhoneViewModel.Content);
					}
				}
				Number = stringBuilder.ToString();
			}
			OnPropertyChanged("NumberList");
		}
	}

	public ContactType Type { get; set; }

	public bool PropertyDataUpdated { get; set; }

	public int DataVersion { get; set; }

	public ReplayCommand CheckClickCommand
	{
		get
		{
			return checkClickCommand;
		}
		set
		{
			if (checkClickCommand != value)
			{
				checkClickCommand = value;
				OnPropertyChanged("CheckClickCommand");
			}
		}
	}

	public ReplayCommand ShowContactDetailClickCommand
	{
		get
		{
			return showContactDetailClickCommand;
		}
		set
		{
			if (showContactDetailClickCommand != value)
			{
				showContactDetailClickCommand = value;
				OnPropertyChanged("ShowContactDetailClickCommand");
			}
		}
	}
}
