using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class VideoInfoViewModel : ViewModelBase
{
	private bool _isSelected;

	private ImageSource _videoImage;

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

	public VideoInfo RawVideoInfo { get; set; }

	public string VideoName => RawVideoInfo.Name;

	public long LongDuration => RawVideoInfo.LongDuration;

	public string Duration
	{
		get
		{
			return RawVideoInfo.Duration;
		}
		set
		{
			RawVideoInfo.Duration = value;
			OnPropertyChanged("Duration");
		}
	}

	public long Size
	{
		get
		{
			return RawVideoInfo.Size;
		}
		set
		{
			RawVideoInfo.Size = value;
			OnPropertyChanged("Size");
		}
	}

	public string Type
	{
		get
		{
			return RawVideoInfo.Type;
		}
		set
		{
			RawVideoInfo.Type = value;
			OnPropertyChanged("Type");
		}
	}

	public string ModifiedDate
	{
		get
		{
			return RawVideoInfo.ModifiedDate;
		}
		set
		{
			RawVideoInfo.ModifiedDate = value;
			OnPropertyChanged("ModifiedDate");
		}
	}

	public ImageSource VideoImage
	{
		get
		{
			return _videoImage;
		}
		set
		{
			_videoImage = value;
			OnPropertyChanged("VideoImage");
		}
	}
}
