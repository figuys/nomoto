using System.Collections.Generic;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class CategoryViewModel : ComboBoxModel
{
	private ImageSource _centerIconSelectedSource;

	private ImageSource _centerIconUnSelectedSource;

	private string _title = string.Empty;

	private bool isSupported = true;

	public int _count;

	private long totalSize;

	private string totalSizeWithUnit;

	private Dictionary<string, long> idAndSizeMapping;

	private int _transferCount;

	private bool _isTransferring;

	private bool _isSelected;

	private bool _isEnabled;

	public ImageSource CenterIconSelectedSource
	{
		get
		{
			return _centerIconSelectedSource;
		}
		set
		{
			if (_centerIconSelectedSource != value)
			{
				_centerIconSelectedSource = value;
				OnPropertyChanged("CenterIconSelectedSource");
			}
		}
	}

	public ImageSource CenterIconUnSelectedSource
	{
		get
		{
			return _centerIconUnSelectedSource;
		}
		set
		{
			if (_centerIconUnSelectedSource != value)
			{
				_centerIconUnSelectedSource = value;
				OnPropertyChanged("CenterIconUnSelectedSource");
			}
		}
	}

	public string Title
	{
		get
		{
			return _title;
		}
		set
		{
			if (!(_title == value))
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}
	}

	public bool IsSupported
	{
		get
		{
			return isSupported;
		}
		set
		{
			if (isSupported != value)
			{
				isSupported = value;
				if (!isSupported)
				{
					bool isEnabled = (IsSelected = false);
					IsEnabled = isEnabled;
				}
				else
				{
					bool isEnabled = (IsSelected = Count > 0);
					IsEnabled = isEnabled;
				}
				OnPropertyChanged("IsSupported");
			}
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
			if (_count == value)
			{
				return;
			}
			_count = value;
			if (IsSupported)
			{
				if (_count > 0)
				{
					IsEnabled = true;
					IsSelected = true;
				}
				else
				{
					IsEnabled = false;
					IsSelected = false;
				}
			}
			OnPropertyChanged("Count");
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
			if (!(totalSizeWithUnit == value))
			{
				totalSizeWithUnit = value;
				OnPropertyChanged("TotalSizeWithUnit");
			}
		}
	}

	public Dictionary<string, long> IdAndSizeMapping
	{
		get
		{
			return idAndSizeMapping;
		}
		set
		{
			if (idAndSizeMapping != value)
			{
				idAndSizeMapping = value;
				OnPropertyChanged("IdAndSizeMapping");
			}
		}
	}

	public int TransferCount
	{
		get
		{
			return _transferCount;
		}
		set
		{
			if (_transferCount != value)
			{
				_transferCount = value;
				OnPropertyChanged("TransferCount");
			}
		}
	}

	public bool IsTransferring
	{
		get
		{
			return _isTransferring;
		}
		set
		{
			if (_isTransferring != value)
			{
				_isTransferring = value;
				OnPropertyChanged("IsTransferring");
			}
		}
	}

	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			if (_isSelected != value)
			{
				if (value && Count == 0)
				{
					value = false;
				}
				_isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			if (_isEnabled != value)
			{
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
	}

	public string ResourceType { get; set; }

	public object Tag { get; set; }

	public List<CategoryViewModel> SubCategoryViewModelList { get; set; }

	public virtual bool DoProcess()
	{
		return true;
	}
}
