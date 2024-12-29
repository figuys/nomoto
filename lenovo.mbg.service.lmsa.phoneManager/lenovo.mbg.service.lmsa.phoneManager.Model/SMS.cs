using System;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

[Serializable]
public class SMS : BaseNotify
{
	public const int SMS_TYPE_RECEIVE = 0;

	public const int SMS_TYPE_SEND = 1;

	[JsonIgnore]
	private bool _isSelected;

	private ReplayCommand _deleteSmsCommand;

	private string __id = string.Empty;

	private string _address;

	private string _body;

	private string _date = string.Empty;

	[JsonIgnore]
	private long _timestamp;

	private string _persion = string.Empty;

	private string _type = string.Empty;

	private string _sim = string.Empty;

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
	public ReplayCommand ForwardSmsCommand { get; set; }

	[JsonIgnore]
	public ReplayCommand DeleteSmsCommand
	{
		get
		{
			return _deleteSmsCommand;
		}
		set
		{
			if (_deleteSmsCommand != value)
			{
				_deleteSmsCommand = value;
				OnPropertyChanged("DeleteSmsCommand");
			}
		}
	}

	[JsonIgnore]
	public ReplayCommand CopySmsCommand { get; set; }

	public string _id
	{
		get
		{
			return __id;
		}
		set
		{
			__id = value;
		}
	}

	public string address
	{
		get
		{
			return _address;
		}
		set
		{
			_address = value;
			OnPropertyChanged("address");
		}
	}

	public string body
	{
		get
		{
			return _body;
		}
		set
		{
			_body = value;
			OnPropertyChanged("body");
		}
	}

	public string date
	{
		get
		{
			return _date;
		}
		set
		{
			_date = value;
			OnPropertyChanged("date");
		}
	}

	[JsonIgnore]
	public long timestamp
	{
		get
		{
			if (_timestamp != 0L)
			{
				return _timestamp;
			}
			long.TryParse(date, out _timestamp);
			return _timestamp;
		}
	}

	public string person
	{
		get
		{
			return _persion;
		}
		set
		{
			_persion = value;
		}
	}

	public string type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
		}
	}

	public string sim
	{
		get
		{
			return _sim;
		}
		set
		{
			_sim = value;
		}
	}

	[JsonIgnore]
	public int OrderBySequence { get; set; }

	public SMS()
	{
		CopySmsCommand = new ReplayCommand(CopySmsCommandHandler);
	}

	private void CopySmsCommandHandler(object parameter)
	{
		try
		{
			Clipboard.SetDataObject(body);
		}
		catch
		{
		}
	}
}
