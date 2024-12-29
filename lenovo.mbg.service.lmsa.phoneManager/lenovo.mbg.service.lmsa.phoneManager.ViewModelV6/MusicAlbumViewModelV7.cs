using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class MusicAlbumViewModelV7 : ViewModelBase
{
	private bool _isSeleted;

	private ObservableCollection<MusicInfo> _cachedAllMusics;

	private bool? _isSelectedAllMusics = false;

	public string AlbumID { get; set; }

	public string AlbumPath => AlbumID;

	public ReplayCommand SingleClickCommand { get; set; }

	public bool IsSelected
	{
		get
		{
			return _isSeleted;
		}
		set
		{
			if (_isSeleted != value)
			{
				_isSeleted = value;
				OnPropertyChanged("IsSelected");
			}
		}
	}

	public string AlbumPath1 { get; set; }

	public string AlbumName { get; set; }

	public int FileCount { get; set; }

	public List<MusicInfo> Musics { get; set; }

	public ObservableCollection<MusicInfo> CachedAllMusics
	{
		get
		{
			return _cachedAllMusics;
		}
		set
		{
			if (_cachedAllMusics != value)
			{
				_cachedAllMusics = value;
				OnPropertyChanged("CachedAllMusics");
			}
		}
	}

	public bool? IsSelectedAllMusics
	{
		get
		{
			return _isSelectedAllMusics;
		}
		set
		{
			if (_isSelectedAllMusics != value)
			{
				_isSelectedAllMusics = value;
				SelectAllMusicInAlbum(value);
				OnPropertyChanged("IsSelectedAllMusics");
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

	private MusicMgtViewModelV7 GetCurrentViewModel => Context.FindViewModel<MusicMgtViewModelV7>(typeof(MusicMgtViewV7));

	public MusicAlbumViewModelV7()
	{
		SingleClickCommand = new ReplayCommand(EnterAlbumMusicList);
	}

	private void EnterAlbumMusicList(object obj)
	{
		IsSelected = true;
		GetCurrentViewModel.FocusedAlbum = this;
		GetCurrentViewModel.FocusedAlbumClone();
	}

	private void SelectAllMusicInAlbum(bool? _selected)
	{
		if (_selected.HasValue)
		{
			foreach (MusicInfo cachedAllMusic in CachedAllMusics)
			{
				cachedAllMusic.IsNotUserClick = true;
				cachedAllMusic.IsSelected = _selected.Value;
				cachedAllMusic.IsNotUserClick = false;
			}
			ObservableCollection<MusicAlbumViewModelV7> albums = GetCurrentViewModel.Albums;
			if (_selected.Value)
			{
				if (albums.Count((MusicAlbumViewModelV7 m) => m.IsSelectedAllMusics != true) == 0)
				{
					GetCurrentViewModel.IsSelectedAllAlbumMusic = true;
				}
				else
				{
					GetCurrentViewModel.IsSelectedAllAlbumMusic = null;
				}
			}
			else if (albums.Count((MusicAlbumViewModelV7 m) => m.IsSelectedAllMusics != false) == 0)
			{
				GetCurrentViewModel.IsSelectedAllAlbumMusic = false;
			}
			else
			{
				GetCurrentViewModel.IsSelectedAllAlbumMusic = null;
			}
		}
		GetCurrentViewModel.RefreshAllAlbumMusicSelectedCount();
	}
}
