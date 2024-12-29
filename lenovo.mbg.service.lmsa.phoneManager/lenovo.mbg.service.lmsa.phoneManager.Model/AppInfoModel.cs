using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class AppInfoModel : BaseNotify
{
	private bool _isSelected;

	private string _appName;

	private string _size;

	private string _dataSize;

	private string _version;

	private bool _isSystemApp;

	private BitmapImage _appImage;

	private string _packagename;

	private long _LSize;

	private long _LDataSize;

	public BitmapImage AppImage
	{
		get
		{
			return _appImage;
		}
		set
		{
			_appImage = value;
			OnPropertyChanged("AppImage");
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
				_isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
	}

	public string AppName
	{
		get
		{
			return _appName;
		}
		set
		{
			_appName = value;
			OnPropertyChanged("AppName");
		}
	}

	public string Size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
			LSize = FormatSize(_size);
			OnPropertyChanged("Size");
		}
	}

	public string DataSize
	{
		get
		{
			return _dataSize;
		}
		set
		{
			_dataSize = value;
			LDataSize = FormatSize(_dataSize);
			OnPropertyChanged("DataSize");
		}
	}

	public string Version
	{
		get
		{
			return _version;
		}
		set
		{
			_version = value;
			OnPropertyChanged("Version");
		}
	}

	public bool IsSystemApp
	{
		get
		{
			return _isSystemApp;
		}
		set
		{
			_isSystemApp = value;
			OnPropertyChanged("IsSystemApp");
		}
	}

	public string PackageName
	{
		get
		{
			return _packagename;
		}
		set
		{
			_packagename = value;
			OnPropertyChanged("PackageName");
		}
	}

	public long LSize
	{
		get
		{
			return _LSize;
		}
		set
		{
			_LSize = value;
			OnPropertyChanged("LSize");
		}
	}

	public long LDataSize
	{
		get
		{
			return _LDataSize;
		}
		set
		{
			_LDataSize = value;
			OnPropertyChanged("LDataSize");
		}
	}

	private long FormatSize(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0L;
		}
		Match match = Regex.Match(str.Trim(), "^(?<value>[\\d.]+)\\s*(?<unit>[A-Za-z]).*$");
		string value = match.Groups["value"].Value;
		string text = match.Groups["unit"].Value.ToLower();
		double result = 0.0;
		double.TryParse(value, out result);
		return text switch
		{
			"k" => (long)(result * 1024.0), 
			"m" => (long)(result * 1024.0 * 1024.0), 
			"g" => (long)(result * 1024.0 * 1024.0 * 1024.0), 
			"t" => (long)(result * 1024.0 * 1024.0 * 1024.0 * 1024.0), 
			_ => (long)result, 
		};
	}
}
