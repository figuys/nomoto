using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class SMSContact : BaseNotify, IContact
{
	private string _address = string.Empty;

	private string _person = string.Empty;

	[JsonIgnore]
	private bool _isSelected;

	public string address
	{
		get
		{
			return _address;
		}
		set
		{
			_address = value;
		}
	}

	public string person
	{
		get
		{
			return _person;
		}
		set
		{
			_person = value;
		}
	}

	[JsonIgnore]
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

	public int total { get; set; }

	public long latest { get; set; }

	[JsonIgnore]
	public string PhoneNumber
	{
		get
		{
			if (!string.IsNullOrEmpty(address))
			{
				return address;
			}
			return string.Empty;
		}
	}

	[JsonIgnore]
	public string PhonePerson
	{
		get
		{
			if (!string.IsNullOrEmpty(person))
			{
				return person;
			}
			return string.Empty;
		}
	}

	[JsonIgnore]
	public int MsgTotal => total;

	[JsonIgnore]
	public long Latest => latest;
}
