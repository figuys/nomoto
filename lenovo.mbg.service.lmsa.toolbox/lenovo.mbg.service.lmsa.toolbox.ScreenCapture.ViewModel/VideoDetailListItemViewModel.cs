using System;
using System.Collections.Generic;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.toolbox.ScreenCapture.Model;
using lenovo.themes.generic.Attributes;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.ScreenCapture.ViewModel;

public class VideoDetailListItemViewModel : PageLoadingItemViewModelBase<string>
{
	private string name;

	private string duration;

	private string size;

	private string modifiedDate;

	private ImageSource iconSource;

	private VideoDetailModel detailModel;

	private long longDuration;

	private long longSize;

	private long longModifiedDate;

	private bool isChecked;

	private ReplayCommand checkCammand;

	private int sortIndex;

	private Dictionary<string, int> sortMapping;

	[SortProtertyName(SortPropertyName = "name", SortMemberPath = "Name")]
	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (!(name == value))
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}
	}

	public string Duration
	{
		get
		{
			return duration;
		}
		private set
		{
			if (!(duration == value))
			{
				duration = value;
				OnPropertyChanged("Duration");
			}
		}
	}

	[SortProtertyName(SortPropertyName = "duration", SortMemberPath = "LongDuration")]
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
				if (longDuration <= 0)
				{
					Duration = "0.0.0";
				}
				else
				{
					Duration = TimeSpan.FromMilliseconds(value).ToString("hh\\:mm\\:ss");
				}
			}
		}
	}

	public string Size
	{
		get
		{
			return size;
		}
		set
		{
			if (!(size == value))
			{
				size = value;
				OnPropertyChanged("Size");
			}
		}
	}

	[SortProtertyName(SortPropertyName = "size", SortMemberPath = "LongSize")]
	public long LongSize
	{
		get
		{
			return longSize;
		}
		set
		{
			if (longSize != value)
			{
				Size = GlobalFun.ConvertLong2String(value);
				longSize = value;
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

	[SortProtertyName(SortPropertyName = "modifiedDate", SortMemberPath = "LongModifiedDate")]
	public long LongModifiedDate
	{
		get
		{
			return longModifiedDate;
		}
		set
		{
			_ = longModifiedDate;
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

	public ReplayCommand CheckCammand
	{
		get
		{
			return checkCammand;
		}
		set
		{
			if (checkCammand != value)
			{
				checkCammand = value;
				OnPropertyChanged("CheckCammand");
			}
		}
	}

	public int SortIndex
	{
		get
		{
			return sortIndex;
		}
		set
		{
			if (sortIndex != value)
			{
				sortIndex = value;
				OnPropertyChanged("SortIndex");
			}
		}
	}

	public VideoDetailListItemViewModel()
	{
		sortMapping = new Dictionary<string, int>();
	}

	public VideoDetailListItemViewModel(VideoDetailModel detailModel)
	{
		this.detailModel = detailModel;
	}

	public void AddSortMapping(string sortMemberPath, int sortIndex)
	{
		sortMapping[sortMemberPath] = sortIndex;
	}

	public void SetCurrentSortIndex(string sortMemberPath)
	{
		SortIndex = sortMapping[sortMemberPath];
	}

	public void ClearSortStatus()
	{
		if (sortMapping != null)
		{
			sortMapping.Clear();
		}
	}
}
