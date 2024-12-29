using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class MusicMgtViewModelV7 : ViewModelBase
{
	private MusicInfoViewModelV7 selectedItem;

	private int _allAlbumMusicSelectedCount;

	private ObservableCollection<MusicAlbumViewModelV7> _albums;

	private MusicAlbumViewModelV7 _focusedAlbum;

	protected SdCardViewModel sdCarVm;

	private bool _isAllSelected;

	private int _allAlbumMusicCount;

	private bool? _isSelectedAllAlbumMusic = false;

	private string _searchText = string.Empty;

	public DeviceMusicManagementV7 m_bll { get; private set; }

	public ReplayCommand MusicSearchCommand { get; set; }

	public OperatorButtonViewModelV6 MusicToolImportBtn { get; private set; }

	public OperatorButtonViewModelV6 MusicToolExportBtn { get; private set; }

	public OperatorButtonViewModelV6 MusicToolDeleteBtn { get; private set; }

	public OperatorButtonViewModelV6 MusicToolRefreshBtn { get; private set; }

	public MusicInfoViewModelV7 SelectedItem
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

	public int AllAlbumMusicSelectedCount
	{
		get
		{
			return _allAlbumMusicSelectedCount;
		}
		set
		{
			if (_allAlbumMusicSelectedCount != value)
			{
				_allAlbumMusicSelectedCount = value;
				OnPropertyChanged("AllAlbumMusicSelectedCount");
			}
		}
	}

	public ObservableCollection<MusicAlbumViewModelV7> Albums
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

	public List<MusicAlbumViewModelV7> AlbumsOriginal { get; set; }

	public MusicAlbumViewModelV7 FocusedAlbumOriginal { get; private set; }

	public MusicAlbumViewModelV7 FocusedAlbum
	{
		get
		{
			return _focusedAlbum;
		}
		set
		{
			if (_focusedAlbum != value)
			{
				_focusedAlbum = value;
				OnPropertyChanged("FocusedAlbum");
			}
		}
	}

	public SdCardViewModel SdCarVm
	{
		get
		{
			return sdCarVm;
		}
		set
		{
			sdCarVm = value;
			OnPropertyChanged("SdCarVm");
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

	public int AllAlbumMusicCount
	{
		get
		{
			return _allAlbumMusicCount;
		}
		set
		{
			if (_allAlbumMusicCount != value)
			{
				_allAlbumMusicCount = value;
				OnPropertyChanged("AllAlbumMusicCount");
			}
		}
	}

	public bool IsSelectedInternal => SdCarVm.StorageSelIndex == 0;

	public bool? IsSelectedAllAlbumMusic
	{
		get
		{
			return _isSelectedAllAlbumMusic;
		}
		set
		{
			if (_isSelectedAllAlbumMusic != value)
			{
				_isSelectedAllAlbumMusic = value;
				OnPropertyChanged("IsSelectedAllAlbumMusic");
			}
		}
	}

	public string SearchText
	{
		get
		{
			return _searchText;
		}
		set
		{
			if (!(_searchText == value))
			{
				_searchText = value;
				OnPropertyChanged("SearchText");
			}
		}
	}

	public MusicMgtViewModelV7()
	{
		m_bll = new DeviceMusicManagementV7();
		SdCarVm = new SdCardViewModel();
		Albums = new ObservableCollection<MusicAlbumViewModelV7>();
		AlbumsOriginal = new List<MusicAlbumViewModelV7>();
		MusicToolBarInitialize();
	}

	public override void Dispose()
	{
		base.Dispose();
		MusicPlayerViewModelV7.SingleInstance.Dispose();
	}

	public override void LoadData()
	{
		SdCarVm.LoadData(Context.CurrentDevice);
		base.LoadData();
		LoadMusic();
	}

	public override void Reset()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			base.Reset();
			_albums.Clear();
			GlobalFun.DeleteFileInDirectory(Configurations.MusicCacheDir);
		});
	}

	public void LoadMusic()
	{
		MusicToolRefreshBtn.IsEnabled = false;
		AsyncDataLoader.BeginLoading(delegate
		{
			m_bll.ClearCacheMusic();
			List<MusicInfo> musicList = m_bll.GetMusicList("-1", -1, -1);
			UpdateAlbumList(musicList);
			IsSelectedAllAlbumMusic = false;
		}, ViewContext.SingleInstance.MainViewModel);
	}

	public void ReSetPicToolBtnStatus()
	{
		MusicAlbumViewModelV7 focusedAlbum = FocusedAlbum;
		if (focusedAlbum != null)
		{
			ObservableCollection<MusicInfo> cachedAllMusics = focusedAlbum.CachedAllMusics;
			if (cachedAllMusics != null && cachedAllMusics.FirstOrDefault((MusicInfo m) => m.IsSelected) != null)
			{
				MusicToolExportBtn.IsEnabled = MusicToolRefreshBtn.IsEnabled;
				MusicToolDeleteBtn.IsEnabled = MusicToolRefreshBtn.IsEnabled;
			}
			else
			{
				MusicToolExportBtn.IsEnabled = false;
				MusicToolDeleteBtn.IsEnabled = false;
			}
		}
	}

	private void MusicToolBarInitialize()
	{
		MusicToolImportBtn = new OperatorButtonViewModelV6
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarImportImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarImportDisabledImage") as ImageSource),
			ButtonText = "K0429",
			ButtonTextDisplay = "K0429",
			Visibility = Visibility.Visible,
			IsEnabled = true,
			ClickCommand = new ReplayCommand(MusicToolImportCommandHandler)
		};
		MusicToolExportBtn = new OperatorButtonViewModelV6
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarExportImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarExportDisabledImage") as ImageSource),
			ButtonText = "K0484",
			ButtonTextDisplay = "K0484",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(MusicToolExportCommandHandler)
		};
		MusicToolDeleteBtn = new OperatorButtonViewModelV6
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarDeleteImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarDeleteDisabledImage") as ImageSource),
			ButtonText = "K0583",
			ButtonTextDisplay = "K0583",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(MusicToolDeleteCommandHandler)
		};
		MusicToolRefreshBtn = new OperatorButtonViewModelV6
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarRefreshImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarRefreshDisabledImage") as ImageSource),
			ButtonText = "K0473",
			ButtonTextDisplay = "K0473",
			IsEnabled = true,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(MusicToolRefreshCommandHandler)
		};
		MusicSearchCommand = new ReplayCommand(SearchCommandHandler);
	}

	private void SearchCommandHandler(object prameter)
	{
		if (prameter == null)
		{
			return;
		}
		foreach (MusicInfo _single in FocusedAlbum.CachedAllMusics)
		{
			FocusedAlbumOriginal.CachedAllMusics.First((MusicInfo m) => m.ID == _single.ID).IsSelected = _single.IsSelected;
		}
		string searchText = prameter.ToString().ToLower();
		FocusedAlbum.CachedAllMusics = new ObservableCollection<MusicInfo>(FocusedAlbumOriginal.CachedAllMusics.Where((MusicInfo m) => m.DisplayName.ToLower().Contains(searchText)));
	}

	private void MusicToolImportCommandHandler(object parameter)
	{
		m_bll.ImportMusicToDevice(FocusedAlbum?.AlbumPath);
		MusicToolRefreshCommandHandler(null);
	}

	private void MusicToolExportCommandHandler(object parameter)
	{
		List<string> list = new List<string>();
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		foreach (MusicAlbumViewModelV7 album in Albums)
		{
			foreach (MusicInfo cachedAllMusic in album.CachedAllMusics)
			{
				if (cachedAllMusic.IsSelected)
				{
					if (cachedAllMusic.Size > 4294967296L)
					{
						dictionary.Add(cachedAllMusic.RawMusicInfo.AlbumID, cachedAllMusic.Size);
					}
					else
					{
						list.Add(cachedAllMusic.RawMusicInfo.ID.ToString());
					}
				}
			}
		}
		DeviceCommonManagementV6.CheckExportFiles(dictionary);
		if (list.Count != 0)
		{
			m_bll.ExportDeviceMusic(list);
		}
	}

	private void MusicToolDeleteCommandHandler(object parameter)
	{
		List<string> list = new List<string>();
		foreach (MusicAlbumViewModelV7 album in Albums)
		{
			foreach (MusicInfo cachedAllMusic in album.CachedAllMusics)
			{
				if (cachedAllMusic.IsSelected)
				{
					list.Add(cachedAllMusic.RawMusicInfo.ID.ToString());
				}
			}
		}
		if (list.Count != 0)
		{
			bool? flag = Context.MessageBox.ShowMessage(ResourcesHelper.StringResources.SingleInstance.CONTACT_DELETE_TITLE, ResourcesHelper.StringResources.SingleInstance.MUSIC_DELETE_CONTENT, "K0327", "K0208");
			if (flag.HasValue && flag.Value)
			{
				m_bll.DeleteDeviceMusic(list);
				MusicToolRefreshCommandHandler(null);
			}
		}
	}

	public void MusicToolRefreshCommandHandler(object parameter)
	{
		lock (this)
		{
			MusicToolDeleteBtn.IsEnabled = false;
			MusicToolExportBtn.IsEnabled = false;
			MusicToolRefreshBtn.IsEnabled = false;
		}
		try
		{
			SearchText = string.Empty;
			LoadMusic();
		}
		catch (Exception)
		{
			MusicToolRefreshBtn.IsEnabled = true;
			ReSetPicToolBtnStatus();
		}
	}

	public void SetToolBarEnable()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			MusicToolRefreshBtn.IsEnabled = true;
		});
		if (FocusedAlbum == null)
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			bool isEnabled = FocusedAlbum.CachedAllMusics.Any((MusicInfo p) => p.IsSelected);
			MusicToolDeleteBtn.IsEnabled = isEnabled;
			MusicToolExportBtn.IsEnabled = isEnabled;
		});
	}

	private void UpdateAlbumList(List<MusicInfo> music)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			Albums.Clear();
			AlbumsOriginal.Clear();
			AllAlbumMusicCount = 0;
			if (FocusedAlbum != null)
			{
				FocusedAlbum.CachedAllMusics.Clear();
				FocusedAlbum.IsSelectedAllMusics = false;
				FocusedAlbum = null;
			}
			if (music == null)
			{
				MusicToolRefreshBtn.IsEnabled = true;
			}
			else
			{
				m_bll.GenericAlbumInfo(music).ForEach(delegate(MusicAlbumViewModelV7 n)
				{
					AlbumsOriginal.Add(n);
				});
				IEnumerable<MusicAlbumViewModelV7> enumerable = AlbumsOriginal.Where((MusicAlbumViewModelV7 m) => m.IsInternalPath == (SdCarVm.StorageSelIndex == 0));
				if (AlbumsOriginal.Count > 0)
				{
					if (enumerable.FirstOrDefault() != null)
					{
						enumerable.FirstOrDefault().IsSelected = true;
						enumerable.FirstOrDefault().SingleClickCommand.Execute(true);
					}
					Albums = new ObservableCollection<MusicAlbumViewModelV7>(enumerable);
					AllAlbumMusicCount = enumerable.Sum((MusicAlbumViewModelV7 n) => n.FileCount);
				}
				else
				{
					Albums = new ObservableCollection<MusicAlbumViewModelV7>();
					AllAlbumMusicCount = 0;
				}
				MusicToolRefreshBtn.IsEnabled = true;
			}
		});
	}

	public void RefreshAllAlbumMusicSelectedCount()
	{
		int num = 0;
		foreach (MusicAlbumViewModelV7 album in Albums)
		{
			foreach (MusicInfo cachedAllMusic in album.CachedAllMusics)
			{
				if (cachedAllMusic.IsSelected)
				{
					num++;
				}
			}
		}
		AllAlbumMusicSelectedCount = num;
		MusicToolExportBtn.IsEnabled = num > 0;
		MusicToolDeleteBtn.IsEnabled = num > 0;
	}

	public void FocusedAlbumClone()
	{
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			MusicAlbumViewModelV7 musicAlbumViewModelV = AlbumsOriginal.FirstOrDefault((MusicAlbumViewModelV7 n) => n.IsSelected);
			if (musicAlbumViewModelV != null)
			{
				string json = JsonHelper.SerializeObject2Json(musicAlbumViewModelV);
				FocusedAlbumOriginal = JsonHelper.DeserializeJson2Object<MusicAlbumViewModelV7>(json);
			}
		});
	}
}
