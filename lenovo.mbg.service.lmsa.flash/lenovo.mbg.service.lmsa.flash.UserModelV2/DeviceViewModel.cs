using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.ViewV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

public class DeviceViewModel : ViewModelBase
{
	private string id;

	private string modelName;

	private string imei;

	private string status;

	private string statusKey;

	private bool isMotorola;

	private bool isSelected;

	private bool isEnabled;

	private double opacity = 1.0;

	private double percentage;

	private bool isRescuing;

	private bool needOperator;

	private IAMatchView view;

	private bool showTip;

	public DevCategory Category { get; set; }

	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
			OnPropertyChanged("Id");
		}
	}

	public string ModelName
	{
		get
		{
			return modelName;
		}
		set
		{
			modelName = value;
			OnPropertyChanged("ModelName");
		}
	}

	public string IMEI
	{
		get
		{
			return imei;
		}
		set
		{
			imei = value;
			OnPropertyChanged("IMEI");
		}
	}

	public string StatusKey
	{
		get
		{
			return statusKey;
		}
		set
		{
			statusKey = value;
			OnPropertyChanged("StatusKey");
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
			status = value;
			OnPropertyChanged("Status");
		}
	}

	public bool IsMotorola
	{
		get
		{
			return isMotorola;
		}
		set
		{
			isMotorola = value;
			OnPropertyChanged("IsMotorola");
		}
	}

	public bool IsEnabled
	{
		get
		{
			return isEnabled;
		}
		set
		{
			isEnabled = value;
			OnPropertyChanged("IsEnabled");
		}
	}

	public bool ShowTip
	{
		get
		{
			return showTip;
		}
		set
		{
			showTip = value;
			OnPropertyChanged("ShowTip");
		}
	}

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
			OnPropertyChanged("IsSelected");
		}
	}

	public IAMatchView View
	{
		get
		{
			return view;
		}
		set
		{
			view = value;
			OnPropertyChanged("View");
		}
	}

	public double Opacity
	{
		get
		{
			return opacity;
		}
		set
		{
			opacity = value;
			OnPropertyChanged("Opacity");
		}
	}

	public double Percentage
	{
		get
		{
			return percentage;
		}
		set
		{
			percentage = value;
			OnPropertyChanged("Percentage");
		}
	}

	public bool IsRescuing
	{
		get
		{
			return isRescuing;
		}
		set
		{
			isRescuing = value;
			OnPropertyChanged("IsRescuing");
		}
	}

	public bool NeedOperator
	{
		get
		{
			return needOperator;
		}
		set
		{
			needOperator = value;
			OnPropertyChanged("NeedOperator");
		}
	}
}
