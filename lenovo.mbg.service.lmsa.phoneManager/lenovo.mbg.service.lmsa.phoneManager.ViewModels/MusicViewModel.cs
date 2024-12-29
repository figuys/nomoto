using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class MusicViewModel : ViewModelBase
{
	private static MusicViewModel _singleInstance = null;

	private MusicInfoViewModel selectedItem;

	private int _SelectedIndex = -1;

	protected static readonly int PAGE_COUNT = 10;

	private ObservableCollection<MusicAlbumViewModel> _albums;

	private ObservableCollection<MusicInfoViewModel> _songList;

	private bool _isAllSelected;

	private int _albumCount;

	private int _musicCount;

	public static MusicViewModel SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new MusicViewModel();
			}
			return _singleInstance;
		}
	}

	public DeviceMusicManagement Service { get; private set; }

	public MusicAlbumViewModel FocuseAlbum { get; set; }

	public MusicInfoViewModel SelectedItem
	{
		get
		{
			return selectedItem;
		}
		set
		{
			if (selectedItem != value)
			{
				selectedItem = value;
				OnPropertyChanged("SelectedItem");
			}
		}
	}

	protected int _CurrentMaxID
	{
		get
		{
			if (SongList == null || SongList.Count() == 0)
			{
				return -1;
			}
			return SongList.Max((MusicInfoViewModel n) => n.ID);
		}
	}

	public int SelectedIndex
	{
		get
		{
			return _SelectedIndex;
		}
		set
		{
			if (_SelectedIndex != value)
			{
				_SelectedIndex = value;
				OnPropertyChanged("SelectedIndex");
			}
		}
	}

	public ObservableCollection<MusicAlbumViewModel> Albums
	{
		get
		{
			return _albums;
		}
		set
		{
			if (_albums != value)
			{
				_albums = value;
				OnPropertyChanged("Albums");
			}
		}
	}

	public ObservableCollection<MusicInfoViewModel> SongList
	{
		get
		{
			return _songList;
		}
		set
		{
			if (_songList != value)
			{
				_songList = value;
				OnPropertyChanged("SongList");
			}
		}
	}

	public bool IsAllSelected
	{
		get
		{
			return _isAllSelected;
		}
		set
		{
			_isAllSelected = value;
			OnPropertyChanged("IsAllSelected");
		}
	}

	public int AlbumCount
	{
		get
		{
			return _albumCount;
		}
		set
		{
			if (_albumCount != value)
			{
				_albumCount = value;
				OnPropertyChanged("AlbumCount");
			}
		}
	}

	public int MusicCount
	{
		get
		{
			return _musicCount;
		}
		set
		{
			if (_musicCount != value)
			{
				_musicCount = value;
				OnPropertyChanged("MusicCount");
			}
		}
	}

	public bool FirstLoadData { get; private set; }

	public ReplayCommand MusicSearchCommand { get; set; }

	public MusicViewModel()
	{
		InitializeResource();
	}

	public override void LoadData()
	{
		base.LoadData();
		ResetEx();
		Load();
	}

	public void ResetEx()
	{
		IsAllSelected = false;
		_ = LeftNavigationViewModel.SingleInstance.Items.FirstOrDefault((LeftNavigationItemViewModel n) => n.Key.ToString() == "lmsa-plugin-Device-music").View;
	}

	public override void Reset()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			base.Reset();
			_albums.Clear();
			_songList.Clear();
			SelectedIndex = -1;
			AlbumCount = 0;
			MusicCount = 0;
			GlobalFun.DeleteFileInDirectory(Configurations.MusicCacheDir);
		});
	}

	public void LoadMusic(string albumID)
	{
		List<MusicInfo> musicList = Service.GetMusicList(albumID, _CurrentMaxID, -1);
		UpdateAlbumList(musicList);
		UpdateMusicList(musicList);
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			MusicCount = SongList.Count;
			AlbumCount = Albums.Count;
		});
	}

	public void LoadAlbumMusic(string albumId, Action<List<MusicInfo>> callBack)
	{
		SongList.Clear();
		AsyncDataLoader.BeginLoading(delegate
		{
			Service.ClearCacheMusic();
			LoadMusic("-1");
			List<MusicInfo> obj = Service.LoadMusicFormCache(albumId);
			callBack?.Invoke(obj);
		}, ViewContext.SingleInstance.MainViewModel);
	}

	public void UpdateMusicListAfterClear(List<MusicInfo> music)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			SongList.Clear();
		});
		UpdateMusicList(music);
	}

	public void UpdateMusicList(List<MusicInfo> music)
	{
		if (music == null || music.Count <= 0)
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			music.ForEach(delegate(MusicInfo n)
			{
				SongList.Add(new MusicInfoViewModel(n));
			});
		});
	}

	public void UpdateAlbumList(List<MusicInfo> music)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			Albums.Clear();
			if (music != null)
			{
				Service.GenericAlbumInfo(music).ForEach(delegate(MusicAlbumViewModel n)
				{
					Albums.Add(n);
				});
			}
		});
	}

	private void InitializeResource()
	{
		_albumCount = 0;
		_albums = new ObservableCollection<MusicAlbumViewModel>();
		_songList = new ObservableCollection<MusicInfoViewModel>();
		Service = new DeviceMusicManagement();
		MusicSearchCommand = new ReplayCommand(MusicSearchCommandHandler);
	}

	public void Load(int index = 0)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			FirstLoadData = true;
			SongList.Clear();
			SelectedIndex = index;
		});
		AsyncDataLoader.BeginLoading(delegate
		{
			Service.ClearCacheMusic();
			LoadMusic("-1");
			FirstLoadData = false;
		}, ViewContext.SingleInstance.MainViewModel);
	}

	private void MusicSearchCommandHandler(object parameter)
	{
		string keyWords = parameter?.ToString();
		string albumId = "-1";
		if (FocuseAlbum != null)
		{
			albumId = FocuseAlbum.AlbumID;
		}
		List<MusicInfo> music = Service.SearchMusicList(keyWords, albumId);
		AsyncDataLoader.BeginLoading(delegate
		{
			UpdateMusicListAfterClear(music);
		}, ViewContext.SingleInstance.MainViewModel);
	}

	private List<MusicInfo> ConverterViewModelListToModel(List<MusicInfoViewModel> models)
	{
		List<MusicInfo> list = new List<MusicInfo>();
		foreach (MusicInfoViewModel model in models)
		{
			MusicInfo rawMusicInfo = model.RawMusicInfo;
			list.Add(rawMusicInfo);
		}
		return list;
	}
}
