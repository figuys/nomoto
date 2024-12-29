using System;
using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class ContactItemPhoneViewModel : ViewModelBase
{
	private DetailType _type;

	private string _content = string.Empty;

	public DetailType Type
	{
		get
		{
			return _type;
		}
		set
		{
			if (!Enum.IsDefined(typeof(DetailType), value))
			{
				value = DetailType.Other;
			}
			if (_type != value)
			{
				_type = value;
				OnPropertyChanged("Type");
			}
		}
	}

	public IEnumerable<DetailType> DetailTypeValues => Enum.GetValues(typeof(DetailType)).Cast<DetailType>();

	public string Content
	{
		get
		{
			return _content;
		}
		set
		{
			if (!(_content == value))
			{
				_content = value;
				OnPropertyChanged("Content");
			}
		}
	}

	public string Id { get; set; }

	public ContactInfoType ContactInfoType { get; set; }

	public string AddLangKey => ContactInfoType switch
	{
		ContactInfoType.Address => "K1334", 
		ContactInfoType.Email => "K1335", 
		ContactInfoType.Telephone => "K0686", 
		_ => "", 
	};

	public bool IsLast { get; set; }

	public bool IsInputMethodEnabled
	{
		get
		{
			if (ContactInfoType == ContactInfoType.Address)
			{
				return true;
			}
			return false;
		}
	}
}
