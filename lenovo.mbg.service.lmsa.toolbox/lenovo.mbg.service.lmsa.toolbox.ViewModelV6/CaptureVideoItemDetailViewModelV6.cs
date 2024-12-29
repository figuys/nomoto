using System;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.ViewModelV6;

public class CaptureVideoItemDetailViewModelV6 : ViewModelBase
{
	private string _id;

	private string videoName;

	private long longDuration;

	private long size;

	private string modifiedDate;

	private ImageSource iconSource;

	private bool isChecked;

	public string Id
	{
		get
		{
			return _id;
		}
		set
		{
			if (!(_id == value))
			{
				_id = value;
				OnPropertyChanged("Id");
			}
		}
	}

	public string VideoName
	{
		get
		{
			return videoName;
		}
		set
		{
			if (!(videoName == value))
			{
				videoName = value;
				OnPropertyChanged("VideoName");
			}
		}
	}

	public string Duration => new TimeSpan(0, 0, (int)LongDuration / 1000).ToString("hh\\:mm\\:ss");

	public long LongDuration
	{
		get
		{
			return longDuration;
		}
		set
		{
			if (longDuration != value)
			{
				longDuration = value;
				OnPropertyChanged("LongDuration");
			}
		}
	}

	public long Size
	{
		get
		{
			return size;
		}
		set
		{
			if (size != value)
			{
				size = value;
				OnPropertyChanged("Size");
			}
		}
	}

	public string ModifiedDate
	{
		get
		{
			return modifiedDate;
		}
		set
		{
			if (!(modifiedDate == value))
			{
				modifiedDate = value;
				OnPropertyChanged("ModifiedDate");
			}
		}
	}

	public ImageSource IconSource
	{
		get
		{
			return iconSource;
		}
		set
		{
			if (iconSource != value)
			{
				iconSource = value;
				OnPropertyChanged("IconSource");
			}
		}
	}

	public bool IsChecked
	{
		get
		{
			return isChecked;
		}
		set
		{
			if (isChecked != value)
			{
				isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}
