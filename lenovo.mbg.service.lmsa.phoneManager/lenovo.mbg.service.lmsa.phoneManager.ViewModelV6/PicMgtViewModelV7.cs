using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class PicMgtViewModelV7 : ViewModelBase
{
	private DevicePicManagementBLLV7 bll = new DevicePicManagementBLLV7();

	private volatile bool _picListIsLoad;

	public List<ServerPicGroupInfo> picGroup;

	protected SdCardViewModel sdCarVm;

	public int ListStartIndex;

	private ObservableCollection<PicAlbumViewModelV7> _albums;

	public int PicloadIndex;

	private int _albumCount;

	private string _searchText = string.Empty;

	private int _allAlbumPicCount;

	private int _allAlbumPicSelectedCount;

	private bool _isPicSelectedAll;

	private PicAlbumViewModelV7 _focusedAlbum;

	private Visibility _storageSelectPanelVisibility;

	private Visibility _PicDisplayModeVisible;

	private bool? _isSelectedAllAlbumPic = false;

	public ScrollViewer DateGroupScrollViewer { get; set; }

	public ScrollViewer PicNotGroupScrollViewer { get; set; }

	public bool PicListIsLoad
	{
		get
		{
			return _picListIsLoad;
		}
		set
		{
			_picListIsLoad = value;
		}
	}

	public List<string> Ids { get; } = new List<string>();

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

	public List<PicAlbumViewModelV7> AlbumsOriginal { get; set; }

	public ObservableCollection<PicAlbumViewModelV7> Albums
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

	public int AllAlbumPicCount
	{
		get
		{
			return _allAlbumPicCount;
		}
		set
		{
			if (_allAlbumPicCount != value)
			{
				_allAlbumPicCount = value;
				OnPropertyChanged("AllAlbumPicCount");
			}
		}
	}

	public int AllAlbumPicSelectedCount
	{
		get
		{
			return _allAlbumPicSelectedCount;
		}
		set
		{
			if (_allAlbumPicSelectedCount != value)
			{
				_allAlbumPicSelectedCount = value;
				OnPropertyChanged("AllAlbumPicSelectedCount");
			}
		}
	}

	public bool IsPicSelectedAll
	{
		get
		{
			return _isPicSelectedAll;
		}
		set
		{
			if (_isPicSelectedAll != value)
			{
				_isPicSelectedAll = value;
				OnPropertyChanged("IsPicSelectedAll");
			}
		}
	}

	public PicAlbumViewModelV7 FocusedAlbum
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
				DateGroupScrollViewer?.ScrollToTop();
				PicNotGroupScrollViewer?.ScrollToTop();
			}
		}
	}

	public PicAlbumViewModelV7 FocusedAlbumOriginal { get; private set; }

	public Visibility StorageSelectPanelVisibility
	{
		get
		{
			return _storageSelectPanelVisibility;
		}
		set
		{
			if (_storageSelectPanelVisibility != value)
			{
				_storageSelectPanelVisibility = value;
				OnPropertyChanged("StorageSelectPanelVisibility");
			}
		}
	}

	public ReplayCommand SearchCommand { get; set; }

	public OperatorButtonViewModelV6 PicToolImportBtn { get; private set; }

	public OperatorButtonViewModelV6 PicToolExportBtn { get; private set; }

	public OperatorButtonViewModelV6 PicToolDeleteBtn { get; private set; }

	public OperatorButtonViewModelV6 PicToolRefreshBtn { get; private set; }

	public Visibility PicDisplayModeVisible
	{
		get
		{
			return _PicDisplayModeVisible;
		}
		set
		{
			_PicDisplayModeVisible = value;
			OnPropertyChanged("PicDisplayModeVisible");
		}
	}

	public bool? IsSelectedAllAlbumPic
	{
		get
		{
			return _isSelectedAllAlbumPic;
		}
		set
		{
			if (_isSelectedAllAlbumPic != value)
			{
				_isSelectedAllAlbumPic = value;
				OnPropertyChanged("IsSelectedAllAlbumPic");
			}
		}
	}

	public bool IsSelectedInternal => SdCarVm.StorageSelIndex == 0;

	public void RefreshAllAlbumPicSelectedCount()
	{
		int num = 0;
		foreach (PicAlbumViewModelV7 album in Albums)
		{
			foreach (PicInfoViewModelV7 cachedAllPic in album.CachedAllPics)
			{
				if (cachedAllPic.IsSelected)
				{
					num++;
				}
			}
		}
		AllAlbumPicSelectedCount = num;
	}

	public PicMgtViewModelV7()
	{
		SdCarVm = new SdCardViewModel();
		AlbumsOriginal = new List<PicAlbumViewModelV7>();
		Albums = new ObservableCollection<PicAlbumViewModelV7>();
		Albums.CollectionChanged += Albums_CollectionChanged;
		PicToolBarInitialize();
	}

	private void Albums_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		AlbumCount = ((Albums != null) ? Albums.Count : 0);
	}

	public override void LoadData()
	{
		SdCarVm.LoadData(Context.CurrentDevice);
		PicDisplayModeVisible = ((!(HostProxy.deviceManager.MasterDevice.ConnectedAppType == "Ma")) ? Visibility.Collapsed : Visibility.Visible);
		base.LoadData();
		ResetEx();
		AsyncDataLoader.BeginLoading(delegate
		{
			bll.ReLoadData();
			return true;
		});
	}

	public override void Reset()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			PicsThumbnailFileLoader.Instance.ResetInitialize();
			base.Reset();
			ClearAlbums();
			FocusedAlbum = null;
		});
	}

	public void ResetEx()
	{
		PicToolExportBtn.IsEnabled = false;
		PicToolDeleteBtn.IsEnabled = false;
		PicToolRefreshBtn.IsEnabled = true;
		IsPicSelectedAll = false;
	}

	public void ClearAlbums()
	{
		Ids?.Clear();
		Albums?.Clear();
		AlbumsOriginal?.Clear();
	}

	private void PicToolBarInitialize()
	{
		PicToolImportBtn = new OperatorButtonViewModelV6
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarImportImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarImportDisabledImage") as ImageSource),
			ButtonText = "K0429",
			ButtonTextDisplay = "K0429",
			Visibility = Visibility.Visible,
			IsEnabled = true,
			ClickCommand = new ReplayCommand(PicToolImportCommandHandler)
		};
		PicToolExportBtn = new OperatorButtonViewModelV6
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarExportImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarExportDisabledImage") as ImageSource),
			ButtonText = "K0484",
			ButtonTextDisplay = "K0484",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(PicToolExportCommandHandler)
		};
		PicToolDeleteBtn = new OperatorButtonViewModelV6
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarDeleteImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarDeleteDisabledImage") as ImageSource),
			ButtonText = "K0583",
			ButtonTextDisplay = "K0583",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(PicToolDeleteCommandHandler)
		};
		PicToolRefreshBtn = new OperatorButtonViewModelV6
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarRefreshImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarRefreshDisabledImage") as ImageSource),
			ButtonText = "K0473",
			ButtonTextDisplay = "K0473",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(PicToolRefreshCommandHandler)
		};
		SearchCommand = new ReplayCommand(SearchCommandHandler);
	}

	private void SearchCommandHandler(object prameter)
	{
		if (prameter == null)
		{
			return;
		}
		foreach (PicInfoViewModelV7 _single in FocusedAlbum.CachedAllPics)
		{
			FocusedAlbumOriginal.CachedAllPics.First((PicInfoViewModelV7 m) => m.RawPicInfo.Id == _single.RawPicInfo.Id).IsSelected = _single.IsSelected;
		}
		foreach (PicInfoListViewModelV7 _single2 in FocusedAlbum.CachedPicList)
		{
			foreach (PicInfoViewModelV7 _singleChild in _single2.Pics)
			{
				FocusedAlbumOriginal.CachedPicList.First((PicInfoListViewModelV7 m) => m.GroupKey == _single2.GroupKey).Pics.First((PicInfoViewModelV7 m) => m.RawPicInfo.Id == _singleChild.RawPicInfo.Id).IsSelected = _singleChild.IsSelected;
			}
		}
		string searchText = prameter.ToString().ToLower();
		FocusedAlbum.CachedAllPics = new ObservableCollection<PicInfoViewModelV7>(FocusedAlbumOriginal.CachedAllPics.Where((PicInfoViewModelV7 m) => m.RawPicInfo.VirtualFileName.ToLower().Contains(searchText)));
		new ObservableCollection<PicInfoListViewModelV7>();
		for (int i = 0; i < FocusedAlbumOriginal.CachedPicList.Count; i++)
		{
			IEnumerable<PicInfoViewModelV7> collection = FocusedAlbumOriginal.CachedPicList[i].Pics.Where((PicInfoViewModelV7 m) => m.RawPicInfo.VirtualFileName.ToLower().Contains(searchText));
			FocusedAlbum.CachedPicList[i].SetPics(new ObservableCollection<PicInfoViewModelV7>(collection));
		}
	}

	private void PicToolImportCommandHandler(object parameter)
	{
		bll.ImportPicToDevice(FocusedAlbum?.AlbumPath);
		PicToolRefreshCommandHandler(null);
	}

	private void PicToolExportCommandHandler(object parameter)
	{
		bll.ExportDevicePic();
	}

	private void PicToolDeleteCommandHandler(object parameter)
	{
		List<string> list = new List<string>();
		foreach (PicAlbumViewModelV7 album in Albums)
		{
			foreach (PicInfoViewModelV7 cachedAllPic in album.CachedAllPics)
			{
				if (cachedAllPic.IsSelected)
				{
					list.Add(cachedAllPic.RawPicInfo.Id);
				}
			}
		}
		if (list.Count != 0)
		{
			bool? flag = Context.MessageBox.ShowMessage(ResourcesHelper.StringResources.SingleInstance.PIC_DELETE_CONTENT, MessageBoxButton.OKCancel);
			if (flag.HasValue && flag.Value)
			{
				bll.DeleteDevicePic(list);
				PicToolRefreshCommandHandler(null);
			}
		}
	}

	public void PicToolRefreshCommandHandler(object parameter)
	{
		lock (this)
		{
			PicToolDeleteBtn.IsEnabled = false;
			PicToolExportBtn.IsEnabled = false;
			PicToolRefreshBtn.IsEnabled = false;
		}
		try
		{
			IsSelectedAllAlbumPic = false;
			SearchText = string.Empty;
			foreach (PicAlbumViewModelV7 album in Albums)
			{
				album.IsSelectedAllPic = false;
			}
			List<PicServerAlbumInfo> picAlbumsViewModel = bll.GetPicAlbumsViewModel();
			LogHelper.LogInstance.Info("Get Albums finished!");
			bll.LoadAlbumsInfo(picAlbumsViewModel);
			LogHelper.LogInstance.Info("Load Albums finished!");
		}
		catch (Exception)
		{
			PicToolRefreshBtn.IsEnabled = true;
			ReSetPicToolBtnStatus();
		}
	}

	public void SetToolBarEnable()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			PicToolRefreshBtn.IsEnabled = true;
		});
		if (FocusedAlbum == null)
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			bool isEnabled = FocusedAlbum.CachedAllPics.Any((PicInfoViewModelV7 p) => p.IsSelected);
			PicToolDeleteBtn.IsEnabled = isEnabled;
			PicToolExportBtn.IsEnabled = isEnabled;
		});
	}

	public void PicSelectionHandler(bool? result)
	{
		ReSetPicToolBtnStatus();
	}

	public void ReSetPicToolBtnStatus()
	{
		PicAlbumViewModelV7 focusedAlbum = FocusedAlbum;
		if (focusedAlbum != null)
		{
			ObservableCollection<PicInfoViewModelV7> cachedAllPics = focusedAlbum.CachedAllPics;
			if (cachedAllPics != null && cachedAllPics.FirstOrDefault((PicInfoViewModelV7 m) => m.IsSelected) != null)
			{
				PicToolExportBtn.IsEnabled = PicToolRefreshBtn.IsEnabled;
				PicToolDeleteBtn.IsEnabled = PicToolRefreshBtn.IsEnabled;
			}
			else
			{
				PicToolExportBtn.IsEnabled = false;
				PicToolDeleteBtn.IsEnabled = false;
			}
		}
	}

	public void FocusedAlbumClone()
	{
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			PicAlbumViewModelV7 picAlbumViewModelV = AlbumsOriginal.FirstOrDefault((PicAlbumViewModelV7 n) => n.IsSelected);
			if (picAlbumViewModelV != null)
			{
				string json = JsonHelper.SerializeObject2Json(picAlbumViewModelV);
				FocusedAlbumOriginal = JsonHelper.DeserializeJson2Object<PicAlbumViewModelV7>(json);
				foreach (PicInfoListViewModelV7 cachedPic in FocusedAlbumOriginal.CachedPicList)
				{
					foreach (PicInfoViewModelV7 pic in cachedPic.Pics)
					{
						pic.ResetCloneImageUri();
					}
				}
			}
		});
	}
}
