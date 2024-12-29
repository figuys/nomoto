using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class MusicInfo : ViewModelBase
{
	private bool _isSeleted;

	public int ID { get; set; }

	public string AlbumID { get; set; }

	public string Name { get; set; }

	public string DisplayName { get; set; }

	public string Artist { get; set; }

	public string Album { get; set; }

	public string AlbumName => Path.GetFileName(AlbumPath);

	public string AlbumPath { get; set; }

	public long Size { get; set; }

	public int Duration { get; set; }

	public string DurationString => new TimeSpan(0, 0, Duration / 1000).ToString("mm\\:ss");

	public string ModifiedDate { get; set; }

	public DateTime DModifiedDate { get; set; }

	public double Frequency { get; set; }

	public string FilePath { get; set; }

	public ServerMusicInfo RawMusicInfo { get; set; }

	private MusicMgtViewModelV7 GetCurrentViewModel => Context.FindViewModel<MusicMgtViewModelV7>(typeof(MusicMgtViewV7));

	public bool IsNotUserClick { get; set; }

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
				if (!IsNotUserClick)
				{
					SingleMouseClickCommandHandler();
				}
				OnPropertyChanged("IsSelected");
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
			return RawMusicInfo.AlbumID.StartsWith("/storage/emulated/0");
		}
	}

	private void SingleMouseClickCommandHandler()
	{
		ObservableCollection<MusicInfo> cachedAllMusics = GetCurrentViewModel.FocusedAlbum.CachedAllMusics;
		if (IsSelected)
		{
			if (cachedAllMusics.Count((MusicInfo m) => !m.IsSelected) > 0)
			{
				GetCurrentViewModel.FocusedAlbum.IsSelectedAllMusics = null;
				GetCurrentViewModel.IsSelectedAllAlbumMusic = null;
			}
			else
			{
				GetCurrentViewModel.FocusedAlbum.IsSelectedAllMusics = true;
				if (GetCurrentViewModel.Albums.Count((MusicAlbumViewModelV7 m) => m.IsSelectedAllMusics != true) == 0)
				{
					GetCurrentViewModel.IsSelectedAllAlbumMusic = true;
				}
			}
		}
		else if (cachedAllMusics.Count((MusicInfo m) => m.IsSelected) > 0)
		{
			GetCurrentViewModel.FocusedAlbum.IsSelectedAllMusics = null;
			GetCurrentViewModel.IsSelectedAllAlbumMusic = null;
		}
		else
		{
			GetCurrentViewModel.FocusedAlbum.IsSelectedAllMusics = false;
			if (GetCurrentViewModel.Albums.Count((MusicAlbumViewModelV7 m) => m.IsSelectedAllMusics != false) == 0)
			{
				GetCurrentViewModel.IsSelectedAllAlbumMusic = false;
			}
		}
		GetCurrentViewModel.RefreshAllAlbumMusicSelectedCount();
	}
}
