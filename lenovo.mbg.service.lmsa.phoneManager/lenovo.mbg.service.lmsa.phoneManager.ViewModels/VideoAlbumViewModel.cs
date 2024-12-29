using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class VideoAlbumViewModel : ViewModelBase
{
	private bool _isSelected;

	private ImageSource _albumImage;

	public ServerAlbumInfo RawAlbumInfo { get; set; }

	public ObservableCollection<VideoInfoViewModel> VideoInfoList { get; set; }

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
				VideoInfoList.All(delegate(VideoInfoViewModel v)
				{
					v.IsSelected = _isSelected;
					return true;
				});
				OnPropertyChanged("IsSelected");
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

	public ImageSource AlbumImage
	{
		get
		{
			return _albumImage;
		}
		set
		{
			_albumImage = value;
			OnPropertyChanged("AlbumImage");
		}
	}

	public VideoAlbumViewModel()
	{
		VideoInfoList = new ObservableCollection<VideoInfoViewModel>();
	}
}
