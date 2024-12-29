using lenovo.mbg.service.common.utilities;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class CategoryInfoViewModel : ViewModelBase
{
	private bool _isSelected;

	private string _Title;

	private int _count;

	private bool _IsEnabled;

	private long totalSize;

	private string totalSizeWithUnit;

	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;
			OnPropertyChanged("IsSelected");
		}
	}

	public string Title
	{
		get
		{
			return _Title;
		}
		set
		{
			_Title = value;
			OnPropertyChanged("Title");
		}
	}

	public int Count
	{
		get
		{
			return _count;
		}
		set
		{
			_count = value;
			if (_count > 0)
			{
				IsEnabled = true;
			}
			else
			{
				IsEnabled = false;
			}
			OnPropertyChanged("Count");
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _IsEnabled;
		}
		set
		{
			_IsEnabled = value;
			OnPropertyChanged("IsEnabled");
		}
	}

	public long TotalSize
	{
		get
		{
			return totalSize;
		}
		set
		{
			if (totalSize != value)
			{
				totalSize = value;
				if (value == 0L)
				{
					TotalSizeWithUnit = string.Empty;
				}
				else
				{
					TotalSizeWithUnit = "(" + GlobalFun.ConvertLong2String(value) + ")";
				}
				OnPropertyChanged("TotalSize");
			}
		}
	}

	public string TotalSizeWithUnit
	{
		get
		{
			return totalSizeWithUnit;
		}
		set
		{
			if (totalSizeWithUnit != value)
			{
				totalSizeWithUnit = value;
				OnPropertyChanged("TotalSizeWithUnit");
			}
		}
	}

	public string ResourceType { get; set; }
}
