using System;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.support.ViewModel;

public class SupportWarrantyStatusItemViewModel : ViewModelBase
{
	private bool groupExpanded;

	private string subTitle;

	private string id;

	private DateTime startDate;

	private DateTime stopDate;

	private string iconType;

	private string daysRemaining;

	private string deliveryType;

	private string status;

	private string description;

	private bool isShowDottedLine = true;

	private string category;

	public bool GroupExpanded
	{
		get
		{
			return groupExpanded;
		}
		set
		{
			if (groupExpanded != value)
			{
				groupExpanded = value;
				OnPropertyChanged("GroupExpanded");
			}
		}
	}

	public string SubTitle
	{
		get
		{
			return subTitle;
		}
		set
		{
			if (!(subTitle == value))
			{
				subTitle = value;
				OnPropertyChanged("SubTitle");
			}
		}
	}

	public string ID
	{
		get
		{
			return id;
		}
		set
		{
			if (!(id == value))
			{
				id = value;
				OnPropertyChanged("ID");
			}
		}
	}

	public DateTime StartDate
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

	public DateTime StopDate
	{
		get
		{
			return stopDate;
		}
		set
		{
			if (!(stopDate == value))
			{
				stopDate = value;
				int num = (value - DateTime.UtcNow).Days + 1;
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
				OnPropertyChanged("StopDate");
			}
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

	public string DeliveryType
	{
		get
		{
			return deliveryType;
		}
		set
		{
			if (!(deliveryType == value))
			{
				deliveryType = value;
				OnPropertyChanged("DeliveryType");
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

	public string Category
	{
		get
		{
			return category;
		}
		set
		{
			if (!(category == value))
			{
				category = value;
				OnPropertyChanged("Category");
			}
		}
	}

	public SupportWarrantyStatusItemViewModel()
	{
		GroupExpanded = false;
	}
}
