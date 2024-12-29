using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

public class SMSContactMerged : BaseNotify
{
	private string _phonePersion = string.Empty;

	private string _phoneNumber = string.Empty;

	[JsonIgnore]
	private bool _isSelected;

	[JsonIgnore]
	private int _total;

	[JsonIgnore]
	private long _latest;

	public string PhonePerson
	{
		get
		{
			if (string.IsNullOrEmpty(_phonePersion))
			{
				IContact contact = null;
				if (MergedList != null && (contact = MergedList.OrderBy((IContact m) => m.PhoneNumber.Length).FirstOrDefault()) != null)
				{
					_phonePersion = contact.PhonePerson;
				}
			}
			return _phonePersion;
		}
	}

	public string PhoneNumber
	{
		get
		{
			if (string.IsNullOrEmpty(_phoneNumber))
			{
				IContact contact = null;
				if (MergedList != null && (contact = MergedList.OrderBy((IContact m) => m.PhoneNumber.Length).FirstOrDefault()) != null)
				{
					_phoneNumber = contact.PhoneNumber;
				}
			}
			return _phoneNumber;
		}
	}

	public List<IContact> MergedList { get; }

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

	[JsonIgnore]
	public int total
	{
		get
		{
			if (_total == 0)
			{
				return _total = ((MergedList != null) ? MergedList.Sum((IContact m) => m.MsgTotal) : 0);
			}
			return _total;
		}
	}

	[JsonIgnore]
	public long latest
	{
		get
		{
			if (_latest == 0L)
			{
				return _latest = ((MergedList == null) ? 0 : MergedList.Max((IContact m) => m.Latest));
			}
			return _latest;
		}
	}

	public SMSContactMerged()
	{
		MergedList = new List<IContact>();
	}
}
