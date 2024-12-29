using System.Windows;
using lenovo.mbg.service.framework.lang;
using lenovo.themes.generic.ModelV6;

namespace lenovo.themes.generic.ViewModelV6;

public class WarrantyInfoBaseViewModel : ViewModelBase
{
	public WarrantyInfoBaseModel Model;

	private const string DEFAULT_VALUE = "-";

	private string _WarrantyContent = "-";

	private string _MonthContent;

	private int _MonthType = -1;

	private Visibility _TipIconVisibility = Visibility.Collapsed;

	private string _MonthTip;

	private Visibility _NewLineVisible;

	public string Start
	{
		get
		{
			return Model.Start;
		}
		set
		{
			Model.Start = value;
			OnPropertyChanged("Start");
		}
	}

	public string End
	{
		get
		{
			return Model.End;
		}
		set
		{
			Model.End = value;
			OnPropertyChanged("End");
		}
	}

	public int MonthDiff
	{
		get
		{
			return Model.MonthDiff;
		}
		set
		{
			Model.MonthDiff = value;
			OnPropertyChanged("MonthDiff");
		}
	}

	public int ExpriedMonth
	{
		get
		{
			return Model.ExpriedMonth;
		}
		set
		{
			Model.ExpriedMonth = value;
			if (Model.ExpriedMonth < 3)
			{
				TipIconVisibility = Visibility.Visible;
			}
			else
			{
				TipIconVisibility = Visibility.Collapsed;
			}
			OnPropertyChanged("ExpriedMonth");
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
					MonthContent = "< " + LangTranslation.Translate("K1360");
					MonthTip = ((Model.Brand == "Motorola") ? "K1363" : "K1442");
					MonthType = 1;
				}
				else
				{
					MonthContent = "> " + LangTranslation.Translate("K1360");
					MonthType = 2;
				}
			}
			else
			{
				WarrantyContent = "K1409";
				MonthContent = "< " + LangTranslation.Translate("K1361");
				MonthTip = ((Model.Brand == "Motorola") ? "K1362" : "K1443");
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

	public string MonthContent
	{
		get
		{
			return _MonthContent;
		}
		set
		{
			_MonthContent = value;
			OnPropertyChanged("MonthContent");
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

	public Visibility TipIconVisibility
	{
		get
		{
			return _TipIconVisibility;
		}
		set
		{
			_TipIconVisibility = value;
			OnPropertyChanged("TipIconVisibility");
		}
	}

	public string MonthTip
	{
		get
		{
			return _MonthTip;
		}
		set
		{
			_MonthTip = value;
			OnPropertyChanged("MonthTip");
		}
	}

	public Visibility NewLineVisible
	{
		get
		{
			return _NewLineVisible;
		}
		set
		{
			_NewLineVisible = value;
			OnPropertyChanged("NewLineVisible");
		}
	}

	public bool Detail { get; private set; }

	public WarrantyInfoBaseViewModel(bool detail)
	{
		Model = new WarrantyInfoBaseModel();
		Detail = detail;
		NewLineVisible = (detail ? Visibility.Collapsed : Visibility.Visible);
	}

	public WarrantyInfoBaseViewModel(WarrantyInfoBaseModel model, bool detail)
		: this(detail)
	{
		UpdateModel(model);
	}

	public void UpdateModel(WarrantyInfoBaseModel model)
	{
		if (model != null)
		{
			Model.Brand = model.Brand;
			ExpriedMonth = model.ExpriedDays / 30;
			Start = model.Start;
			End = model.End;
			MonthDiff = model.MonthDiff;
			InWarranty = model.InWarranty;
		}
		else
		{
			MonthDiff = 0;
			MonthType = -1;
			WarrantyContent = "K1409";
		}
	}
}
