using System;
using System.ComponentModel;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class CallLogItemViewModel : ViewModelBase
{
	private bool _isSelected;

	private string contact = string.Empty;

	private CallType callType = CallType.BLOCKED;

	private string duration = string.Empty;

	private long longDuration = -1L;

	private string date = string.Empty;

	private DateTime sortDateTime;

	private ReplayCommand checkClickCommand;

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

	public string Contact
	{
		get
		{
			return contact;
		}
		set
		{
			if (!(contact == value))
			{
				contact = value;
				OnPropertyChanged("Contact");
			}
		}
	}

	public CallType CallType
	{
		get
		{
			return callType;
		}
		set
		{
			if (callType != value)
			{
				callType = value;
				OnPropertyChanged("CallType");
			}
		}
	}

	public string Duration
	{
		get
		{
			return duration;
		}
		private set
		{
			if (!(duration == value))
			{
				duration = value;
				OnPropertyChanged("Duration");
			}
		}
	}

	public long LongDuration
	{
		get
		{
			return longDuration;
		}
		set
		{
			if (longDuration != value)
			{
				longDuration = value;
				if (value > 0)
				{
					Duration = TimeSpan.FromSeconds(value).ToString("hh\\:mm\\:ss");
				}
				else
				{
					Duration = "00:00:00";
				}
			}
		}
	}

	public string Date
	{
		get
		{
			return date;
		}
		set
		{
			if (!(date == value))
			{
				date = value;
				OnPropertyChanged("Date");
			}
		}
	}

	public DateTime SortDateTime
	{
		get
		{
			return sortDateTime;
		}
		set
		{
			if (!(sortDateTime == value))
			{
				sortDateTime = value;
				OnPropertyChanged("SortDateTime");
			}
		}
	}

	public int DataVersion { get; set; }

	public bool PropertyDataUpdated { get; set; }

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
}
