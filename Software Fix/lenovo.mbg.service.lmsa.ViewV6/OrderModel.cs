using System;
using System.Collections.Generic;
using System.Windows;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewV6;

public class OrderModel : NotifyBase
{
	private string _Status;

	private string _ServerStatus;

	public string Id { get; set; }

	public string Package { get; set; }

	public DateTime Purchase { get; set; }

	public DateTime Expired { get; set; }

	public DateTime? StartUseDate { get; set; }

	public int ImeiUsedCount { get; set; }

	public string MacAddr { get; set; }

	public string DevInUse { get; set; }

	public int RemainDays { get; set; }

	public int RemainImei { get; set; }

	public bool EnableRefund { get; set; }

	public bool IsMonthly { get; set; }

	public Visibility SpliterVisible { get; set; }

	public string Status
	{
		get
		{
			return _Status;
		}
		set
		{
			_Status = value;
			OnPropertyChanged("Status");
		}
	}

	public string ServerStatus
	{
		get
		{
			return _ServerStatus;
		}
		set
		{
			_ServerStatus = value;
			OnPropertyChanged("ServerStatus");
		}
	}

	public List<FlashedDevModel> FlashedDevArr { get; set; }
}
