using System;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Restore;

public class CallLog : NotifyBase
{
	private bool _isSelected;

	[JsonIgnore]
	private DateTime _callDate = DateTime.MinValue;

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

	[JsonProperty("contact")]
	public string Contact { get; set; }

	[JsonProperty("callType")]
	public CallType CallType { get; set; }

	[JsonIgnore]
	public ContactType SimUsed { get; set; }

	[JsonProperty("duration")]
	public long Duration { get; set; }

	[JsonProperty("date")]
	public string ROWCallDate { get; set; }

	[JsonIgnore]
	public DateTime CallDate
	{
		get
		{
			if (_callDate != DateTime.MinValue)
			{
				return _callDate;
			}
			if (!string.IsNullOrEmpty(ROWCallDate))
			{
				DateTime.TryParse(ROWCallDate, out _callDate);
			}
			return _callDate;
		}
	}
}
