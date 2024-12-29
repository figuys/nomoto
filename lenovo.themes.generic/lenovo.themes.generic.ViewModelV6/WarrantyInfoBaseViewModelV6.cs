using lenovo.themes.generic.ModelV6;

namespace lenovo.themes.generic.ViewModelV6;

public class WarrantyInfoBaseViewModelV6 : ViewModelBase
{
	public WarrantyInfoBaseModel Model;

	private const string DEFAULT_VALUE = "-";

	private string _ExpriedDays = string.Empty;

	private string _WarrantyContent = "-";

	private int _MonthType = -1;

	public string WarrantyStartDate
	{
		get
		{
			return Model.WarrantyStartDate;
		}
		set
		{
			Model.WarrantyStartDate = value;
			OnPropertyChanged("WarrantyStartDate");
		}
	}

	public string WarrantyEndDate
	{
		get
		{
			return Model.WarrantyEndDate;
		}
		set
		{
			Model.WarrantyEndDate = value;
			OnPropertyChanged("WarrantyEndDate");
		}
	}

	public string ExpriedDays
	{
		get
		{
			return _ExpriedDays;
		}
		set
		{
			_ExpriedDays = value;
			OnPropertyChanged("ExpriedDays");
		}
	}

	public bool InWarranty
	{
		get
		{
			return Model.InWarranty;
		}
		set
		{
			Model.InWarranty = value;
			if (Model.InWarranty)
			{
				WarrantyContent = "K1364";
				if (Model.ExpriedMonth < 3)
				{
					MonthType = 1;
				}
				else
				{
					MonthType = 2;
				}
			}
			else
			{
				WarrantyContent = "K1365";
				MonthType = 0;
			}
			OnPropertyChanged("InWarranty");
		}
	}

	public string WarrantyContent
	{
		get
		{
			return _WarrantyContent;
		}
		set
		{
			_WarrantyContent = value;
			OnPropertyChanged("WarrantyContent");
		}
	}

	public int MonthType
	{
		get
		{
			return _MonthType;
		}
		set
		{
			_MonthType = value;
			OnPropertyChanged("MonthType");
		}
	}

	public WarrantyInfoBaseViewModelV6(bool detail)
	{
		Model = new WarrantyInfoBaseModel();
	}

	public void UpdateModel(WarrantyInfoBaseModel model)
	{
		if (model != null)
		{
			Model.ExpriedDays = model.ExpriedDays;
			Model.Brand = model.Brand;
			Model.ExpriedMonth = model.ExpriedMonth;
			ExpriedDays = ((model.ExpriedDays > 0) ? model.ExpriedDays.ToString() : "0");
			WarrantyStartDate = model.WarrantyStartDate;
			WarrantyEndDate = model.WarrantyEndDate;
			InWarranty = model.InWarranty;
		}
		else
		{
			WarrantyStartDate = "-";
			WarrantyEndDate = "-";
			ExpriedDays = "-";
			MonthType = -1;
			WarrantyContent = "K1409";
		}
	}
}
