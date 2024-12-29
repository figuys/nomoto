using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class Contact : BaseNotify
{
	private bool _isSelected;

	private string _name = "";

	private string _number = "";

	private List<Phone> _numberList;

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

	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonIgnore]
	public string Acronym { get; set; }

	[JsonProperty("name")]
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

	[JsonProperty("sortKey")]
	public string SortKey { get; set; }

	[JsonProperty("phone")]
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

	[JsonProperty("phoneList")]
	public List<Phone> NumberList
	{
		get
		{
			return _numberList;
		}
		set
		{
			_numberList = value;
		}
	}

	[JsonProperty("type")]
	public ContactType Type { get; set; }
}
