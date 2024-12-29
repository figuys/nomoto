using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class VideoInfoViewModelV7 : ViewModelBase
{
	private bool _isSelected;

	private ImageSource _videoImage;

	public string GroupKey { get; set; }

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
				if (!IsNotUserClick)
				{
					SingleMouseClickCommandHandler();
				}
				OnPropertyChanged("IsSelected");
			}
		}
	}

	public bool IsNotUserClick { get; set; }

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

	[JsonIgnore]
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

	public string VideoPath => RawVideoInfo.FilePath;

	private VideoMgtViewModelV7 GetCurrentViewModel => Context.FindViewModel<VideoMgtViewModelV7>(typeof(VideoMgtViewV7));

	public void SetGroupKey()
	{
		if (RawVideoInfo.ModifiedDate.Length > 4)
		{
			GroupKey = RawVideoInfo.ModifiedDate.Substring(0, 7);
		}
		else
		{
			GroupKey = "2000-01";
		}
	}

	private void SingleMouseClickCommandHandler()
	{
		ObservableCollection<VideoInfoViewModelV7> cachedAllVideos = GetCurrentViewModel.FocusedAlbum.CachedAllVideos;
		VideoInfoGroupViewModelV7 videoInfoGroupViewModelV = GetCurrentViewModel.FocusedAlbum.CacheVideoGroupList.FirstOrDefault((VideoInfoGroupViewModelV7 m) => m.GroupKey == GroupKey);
		if (IsSelected)
		{
			if (videoInfoGroupViewModelV != null)
			{
				if (videoInfoGroupViewModelV.Videos.Count((VideoInfoViewModelV7 m) => !m.IsSelected) > 0)
				{
					videoInfoGroupViewModelV.IsGroupSelected = null;
				}
				else
				{
					videoInfoGroupViewModelV.IsGroupSelected = true;
				}
			}
			if (cachedAllVideos.Count((VideoInfoViewModelV7 m) => !m.IsSelected) > 0)
			{
				GetCurrentViewModel.FocusedAlbum.IsSelectedAllVideo = null;
				GetCurrentViewModel.IsSelectedAllAlbumVideo = null;
			}
			else
			{
				GetCurrentViewModel.FocusedAlbum.IsSelectedAllVideo = true;
				if (GetCurrentViewModel.AlbumList.Count((VideoAlbumViewModelV7 m) => m.IsSelectedAllVideo != true) == 0)
				{
					GetCurrentViewModel.IsSelectedAllAlbumVideo = true;
				}
			}
		}
		else
		{
			if (videoInfoGroupViewModelV != null)
			{
				if (videoInfoGroupViewModelV.Videos.Count((VideoInfoViewModelV7 m) => m.IsSelected) > 0)
				{
					videoInfoGroupViewModelV.IsGroupSelected = null;
				}
				else
				{
					videoInfoGroupViewModelV.IsGroupSelected = false;
				}
			}
			if (cachedAllVideos.Count((VideoInfoViewModelV7 m) => m.IsSelected) > 0)
			{
				GetCurrentViewModel.FocusedAlbum.IsSelectedAllVideo = null;
				GetCurrentViewModel.IsSelectedAllAlbumVideo = null;
			}
			else
			{
				GetCurrentViewModel.FocusedAlbum.IsSelectedAllVideo = false;
				if (GetCurrentViewModel.AlbumList.Count((VideoAlbumViewModelV7 m) => m.IsSelectedAllVideo != false) == 0)
				{
					GetCurrentViewModel.IsSelectedAllAlbumVideo = false;
				}
			}
		}
		GetCurrentViewModel.RefreshAllAlbumPicSelectedCount();
	}

	public void ResetCloneImageUri()
	{
		VideoImage = ImageHandleHelper.LoadBitmap(RawVideoInfo.LocalVideoImagePath);
	}
}
