using System.Collections.ObjectModel;
using System.Linq;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class VideoAlbumViewModelV7 : ViewModelBase
{
	private bool _isSelected;

	private bool? _isSeletedAllVideo = false;

	private ObservableCollection<VideoInfoViewModelV7> _allVideos;

	public ServerAlbumInfo RawAlbumInfo { get; set; }

	public ReplayCommand SingleClickCommand { get; set; }

	public ObservableCollection<VideoInfoGroupViewModelV7> CacheVideoGroupList { get; set; }

	private VideoMgtViewModelV7 GetCurrentViewModel => Context.FindViewModel<VideoMgtViewModelV7>(typeof(VideoMgtViewV7));

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

	public bool? IsSelectedAllVideo
	{
		get
		{
			return _isSeletedAllVideo;
		}
		set
		{
			if (_isSeletedAllVideo != value)
			{
				_isSeletedAllVideo = value;
				SelectAllPicInAlbum(value);
				OnPropertyChanged("IsSelectedAllVideo");
			}
		}
	}

	public string AlbumName => RawAlbumInfo?.AlbumName;

	public string AlbumPath => RawAlbumInfo?.AlbumPath;

	public int FileCount
	{
		get
		{
			return RawAlbumInfo.FileCount;
		}
		set
		{
			RawAlbumInfo.FileCount = value;
			OnPropertyChanged("FileCount");
		}
	}

	public ObservableCollection<VideoInfoViewModelV7> CachedAllVideos
	{
		get
		{
			return _allVideos;
		}
		set
		{
			if (_allVideos != value)
			{
				_allVideos = value;
				OnPropertyChanged("CachedAllVideos");
			}
		}
	}

	public bool IsInternalPath
	{
		get
		{
			if (HostProxy.deviceManager.MasterDevice.ConnectedAppType == "Moto")
			{
				return true;
			}
			return AlbumPath.StartsWith("/storage/emulated/0");
		}
	}

	public VideoAlbumViewModelV7()
	{
		SingleClickCommand = new ReplayCommand(EnterAlbumVideoList);
		CacheVideoGroupList = new ObservableCollection<VideoInfoGroupViewModelV7>();
		CachedAllVideos = new ObservableCollection<VideoInfoViewModelV7>();
	}

	private void EnterAlbumVideoList(object obj)
	{
		IsSelected = true;
		GetCurrentViewModel.FocusedAlbum = this;
		GetCurrentViewModel.FocusedAlbumClone();
	}

	private void SelectAllPicInAlbum(bool? _selected)
	{
		if (_selected.HasValue)
		{
			foreach (VideoInfoViewModelV7 cachedAllVideo in CachedAllVideos)
			{
				cachedAllVideo.IsNotUserClick = true;
				cachedAllVideo.IsSelected = _selected.Value;
				cachedAllVideo.IsNotUserClick = false;
			}
			foreach (VideoInfoGroupViewModelV7 cacheVideoGroup in CacheVideoGroupList)
			{
				cacheVideoGroup.IsGroupSelected = _selected.Value;
			}
			ObservableCollection<VideoAlbumViewModelV7> albumList = GetCurrentViewModel.AlbumList;
			if (_selected.Value)
			{
				if (albumList.Count((VideoAlbumViewModelV7 m) => m.IsSelectedAllVideo != true) == 0)
				{
					GetCurrentViewModel.IsSelectedAllAlbumVideo = true;
				}
				else
				{
					GetCurrentViewModel.IsSelectedAllAlbumVideo = null;
				}
			}
			else if (albumList.Count((VideoAlbumViewModelV7 m) => m.IsSelectedAllVideo != false) == 0)
			{
				GetCurrentViewModel.IsSelectedAllAlbumVideo = false;
			}
			else
			{
				GetCurrentViewModel.IsSelectedAllAlbumVideo = null;
			}
		}
		GetCurrentViewModel.RefreshAllAlbumPicSelectedCount();
	}

	public void ResetVideoGroupList()
	{
		foreach (VideoInfoViewModelV7 _single in CachedAllVideos)
		{
			VideoInfoGroupViewModelV7 videoInfoGroupViewModelV = CacheVideoGroupList.FirstOrDefault((VideoInfoGroupViewModelV7 p) => p.GroupKey == _single.GroupKey);
			if (videoInfoGroupViewModelV == null)
			{
				videoInfoGroupViewModelV = new VideoInfoGroupViewModelV7
				{
					GroupKey = _single.GroupKey
				};
				CacheVideoGroupList.Insert(0, videoInfoGroupViewModelV);
			}
			videoInfoGroupViewModelV.Videos.Insert(0, _single);
		}
	}
}
