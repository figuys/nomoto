using System.Collections.Generic;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class MusicAlbumViewModel
{
	public int _fileCount;

	private ImageSource _folderImageSource = LeftNavResources.SingleInstance.GetResource("albumMusicCoverDrawingImage") as ImageSource;

	private ImageSource _albumIconImageSource = LeftNavResources.SingleInstance.GetResource("albumMusicDrawingImage") as ImageSource;

	public string AlbumID { get; set; }

	public string AlbumPath => AlbumID;

	public string AlbumName { get; set; }

	public int FileCount
	{
		get
		{
			return _fileCount;
		}
		set
		{
			_fileCount = value;
		}
	}

	public ImageSource FolderImageSource
	{
		get
		{
			return _folderImageSource;
		}
		set
		{
			_folderImageSource = value;
		}
	}

	public ImageSource AlbumIconImageSource
	{
		get
		{
			return _albumIconImageSource;
		}
		set
		{
			_albumIconImageSource = value;
		}
	}

	public ReplayCommand DoubleClickCommand { get; set; }

	public List<MusicInfo> Musics { get; set; }
}
