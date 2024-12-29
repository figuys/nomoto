using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class VideoInfoGroupViewModelV7 : ViewModelBase
{
	private ObservableCollection<VideoInfoViewModelV7> _videos;

	private string _groupKey = string.Empty;

	private bool? _isGroupSelected = false;

	private long _videoCount;

	public ObservableCollection<VideoInfoViewModelV7> Videos
	{
		get
		{
			return _videos;
		}
		set
		{
			if (_videos != value)
			{
				_videos = value;
				OnPropertyChanged("Videos");
			}
		}
	}

	public string GroupKey
	{
		get
		{
			return _groupKey;
		}
		set
		{
			if (!(_groupKey == value))
			{
				_groupKey = value;
				OnPropertyChanged("GroupKey");
			}
		}
	}

	public bool? IsGroupSelected
	{
		get
		{
			return _isGroupSelected;
		}
		set
		{
			_isGroupSelected = value;
			OnPropertyChanged("IsGroupSelected");
		}
	}

	public long VideoCount
	{
		get
		{
			return _videoCount;
		}
		set
		{
			if (_videoCount != value)
			{
				_videoCount = value;
				OnPropertyChanged("VideoCount");
			}
		}
	}

	public ReplayCommand VideoSelectAllCommand { get; set; }

	private void VideoSelectAllCommandHandler(object parameter)
	{
		bool value = (parameter as CheckBox).IsChecked.Value;
		foreach (VideoInfoViewModelV7 video in Videos)
		{
			video.IsSelected = value;
		}
	}

	public VideoInfoGroupViewModelV7()
	{
		VideoSelectAllCommand = new ReplayCommand(VideoSelectAllCommandHandler);
		_videos = new ObservableCollection<VideoInfoViewModelV7>();
		_videos.CollectionChanged += delegate(object s, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
			{
				VideoCount = Videos.Count;
			}
		};
	}

	public void SetVideos(ObservableCollection<VideoInfoViewModelV7> _setVideos)
	{
		Videos = _setVideos;
		VideoCount = _setVideos.Count;
	}
}
