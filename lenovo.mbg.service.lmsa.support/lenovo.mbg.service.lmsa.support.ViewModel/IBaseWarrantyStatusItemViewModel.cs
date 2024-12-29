using System;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.support.ViewModel;

public class IBaseWarrantyStatusItemViewModel : ViewModelBase
{
	private string startDate;

	private string stopDate;

	private string iconType;

	private string daysRemaining;

	private string description;

	private string status;

	private bool isShowDottedLine = true;

	public string StartDate
	{
		get
		{
			return startDate;
		}
		set
		{
			if (!(startDate == value))
			{
				startDate = value;
				OnPropertyChanged("StartDate");
			}
		}
	}

	public string StopDate
	{
		get
		{
			return stopDate;
		}
		set
		{
			if (stopDate == value)
			{
				return;
			}
			stopDate = value;
			if (DateTime.TryParse(stopDate, out var result))
			{
				int num = (result - DateTime.UtcNow).Days + 1;
				if (num > 90)
				{
					Status = "Active";
					IconType = "Active";
				}
				else if (num > 0)
				{
					Status = "Active";
					IconType = "Active";
				}
				else
				{
					Status = "Expired";
					IconType = "Warn";
				}
				DaysRemaining = ((num > 0) ? num.ToString() : "0");
			}
			OnPropertyChanged("StopDate");
		}
	}

	public string IconType
	{
		get
		{
			return iconType;
		}
		set
		{
			if (!(iconType == value))
			{
				iconType = value;
				OnPropertyChanged("IconType");
			}
		}
	}

	public string DaysRemaining
	{
		get
		{
			return daysRemaining;
		}
		set
		{
			if (!(daysRemaining == value))
			{
				daysRemaining = value;
				OnPropertyChanged("DaysRemaining");
			}
		}
	}

	public string Description
	{
		get
		{
			return description;
		}
		set
		{
			if (!(description == value))
			{
				description = value;
				OnPropertyChanged("Description");
			}
		}
	}

	public string Status
	{
		get
		{
			return status;
		}
		set
		{
			if (!(status == value))
			{
				status = value;
				OnPropertyChanged("Status");
			}
		}
	}

	public bool IsShowDottedLine
	{
		get
		{
			return isShowDottedLine;
		}
		set
		{
			if (isShowDottedLine != value)
			{
				isShowDottedLine = value;
				OnPropertyChanged("IsShowDottedLine");
			}
		}
	}
}
