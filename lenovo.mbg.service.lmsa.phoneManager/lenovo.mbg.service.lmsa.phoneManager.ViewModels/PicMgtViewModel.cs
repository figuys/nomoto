using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class PicMgtViewModel : ViewModelBase
{
	private DevicePicManagementBLL bll = new DevicePicManagementBLL();

	private static PicMgtViewModel _singleInstance = null;

	private static object locksingle = new object();

	public bool AlbumThumbnailIsLoad;

	private volatile bool _picListIsLoad;

	public List<ServerPicGroupInfo> picGroup;

	private List<string> _ids = new List<string>();

	public int ListStartIndex;

	private ObservableCollection<PicAlbumViewModel> _albums;

	public int PicloadIndex;

	private int _albumCount;

	private long _fileSize;

	private int _topNavgationSelectedIndex;

	private PicAlbumViewModel _camareAlbum;

	private PicAlbumViewModel _focusedAlbum;

	private PicMgtContentViewDisplayMode _picCurrentGroupMode = PicMgtContentViewDisplayMode.PicListWithDateGroup;

	private Visibility _picToolBarVisibility;

	private OperatorButtonViewModel _picToolBackToAlbumsBtn;

	private OperatorButtonViewModel _picToolImportBtn;

	private OperatorButtonViewModel _picToolExportBtn;

	private OperatorButtonViewModel _picToolDeleteBtn;

	private OperatorButtonViewModel _picToolRefreshBtn;

	private Visibility _albumToolBarVisibility = Visibility.Collapsed;

	private OperatorButtonViewModel _albumToolImportBtn;

	private OperatorButtonViewModel _albumToolExportBtn;

	private OperatorButtonViewModel _albumToolDeleteBtn;

	private OperatorButtonViewModel _albumToolRefreshBtn;

	private bool _isPicSelectedAll;

	private bool _isAlbumSelectedAll;

	private PicMgtContentViewDisplayMode _currentViewModel = PicMgtContentViewDisplayMode.PicListWithDateGroup;

	public ScrollViewer DateGroupScrollViewer { get; set; }

	public ScrollViewer PicNotGroupScrollViewer { get; set; }

	public static PicMgtViewModel SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				lock (locksingle)
				{
					if (_singleInstance == null)
					{
						_singleInstance = new PicMgtViewModel();
					}
				}
			}
			return _singleInstance;
		}
	}

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

	public List<string> Ids => _ids;

	public ObservableCollection<PicAlbumViewModel> Albums
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

	public long FileSize
	{
		get
		{
			return _fileSize;
		}
		set
		{
			if (_fileSize != value)
			{
				_fileSize = value;
				OnPropertyChanged("FileSize");
			}
		}
	}

	public int TopNavgationSelectedIndex
	{
		get
		{
			return _topNavgationSelectedIndex;
		}
		set
		{
			if (_topNavgationSelectedIndex != value)
			{
				_topNavgationSelectedIndex = value;
				OnPropertyChanged("TopNavgationSelectedIndex");
			}
		}
	}

	public PicAlbumViewModel CamareAlbum
	{
		get
		{
			return _camareAlbum;
		}
		set
		{
			if (_camareAlbum != value)
			{
				_camareAlbum = value;
				OnPropertyChanged("CamareAlbum");
			}
		}
	}

	public PicAlbumViewModel FocusedAlbum
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

	public ReplayCommand CameraNavgationClickCommand { get; set; }

	public ReplayCommand AlubmNavgationClickCommand { get; set; }

	public PicMgtContentViewDisplayMode PicCurrentGroupMode
	{
		get
		{
			return _picCurrentGroupMode;
		}
		set
		{
			if (_picCurrentGroupMode != value)
			{
				_picCurrentGroupMode = value;
				OnPropertyChanged("PicCurrentGroupMode");
			}
		}
	}

	public ReplayCommand DataGroupModelSwitchCommand { get; set; }

	public ReplayCommand FlowWaterModelSwitchCommand { get; set; }

	public Visibility PicToolBarVisibility
	{
		get
		{
			return _picToolBarVisibility;
		}
		set
		{
			if (_picToolBarVisibility != value)
			{
				_picToolBarVisibility = value;
				OnPropertyChanged("PicToolBarVisibility");
			}
		}
	}

	public OperatorButtonViewModel PicToolBackToAlbumsBtn => _picToolBackToAlbumsBtn;

	public OperatorButtonViewModel PicToolImportBtn => _picToolImportBtn;

	public OperatorButtonViewModel PicToolExportBtn => _picToolExportBtn;

	public OperatorButtonViewModel PicToolDeleteBtn => _picToolDeleteBtn;

	public OperatorButtonViewModel PicToolRefreshBtn => _picToolRefreshBtn;

	public Visibility AlbumToolBarVisibility
	{
		get
		{
			return _albumToolBarVisibility;
		}
		set
		{
			if (_albumToolBarVisibility != value)
			{
				_albumToolBarVisibility = value;
				OnPropertyChanged("AlbumToolBarVisibility");
			}
		}
	}

	public OperatorButtonViewModel AlbumToolImportBtn => _albumToolImportBtn;

	public OperatorButtonViewModel AlbumToolExportBtn => _albumToolExportBtn;

	public OperatorButtonViewModel AlbumToolDeleteBtn => _albumToolDeleteBtn;

	public OperatorButtonViewModel AlbumToolRefreshBtn => _albumToolRefreshBtn;

	public ReplayCommand PicSelectAllCommand { get; set; }

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

	public ReplayCommand AlbumSelectAllCommand { get; set; }

	public bool IsAlbumSelectedAll
	{
		get
		{
			return _isAlbumSelectedAll;
		}
		set
		{
			if (_isAlbumSelectedAll != value)
			{
				_isAlbumSelectedAll = value;
				OnPropertyChanged("IsAlbumSelectedAll");
			}
		}
	}

	public PicMgtContentViewDisplayMode CurrentViewModel
	{
		get
		{
			return _currentViewModel;
		}
		set
		{
			if (_currentViewModel != value)
			{
				_currentViewModel = value;
				PicMgtContentViewDisplayModeChangedHandler(_currentViewModel);
				OnPropertyChanged("CurrentViewModel");
			}
		}
	}

	public PicMgtViewModel()
	{
		Albums = new ObservableCollection<PicAlbumViewModel>();
		Albums.CollectionChanged += Albums_CollectionChanged;
		PicToolBarInitialize();
		AlbumToolBarInitialize();
		InitializeOperatorCommand();
	}

	private void Albums_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		AlbumCount = ((Albums != null) ? Albums.Count : 0);
	}

	public override void LoadData()
	{
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
			CamareAlbum = null;
			FocusedAlbum = null;
			CurrentViewModel = PicMgtContentViewDisplayMode.PicListWithDateGroup;
			TopNavgationSelectedIndex = 0;
		});
	}

	public void ResetEx()
	{
		PicToolExportBtn.IsEnabled = false;
		PicToolDeleteBtn.IsEnabled = false;
		PicToolRefreshBtn.IsEnabled = true;
		IsPicSelectedAll = false;
	}

	private void InitializeOperatorCommand()
	{
		PicSelectAllCommand = new ReplayCommand(PicSelectAllCommandHandler);
		AlbumSelectAllCommand = new ReplayCommand(AlbumSelectAllCommandHandler);
		CameraNavgationClickCommand = new ReplayCommand(CameraNavgationClickCommandHandler);
		AlubmNavgationClickCommand = new ReplayCommand(AlubmNavgationClickCommandHandler);
		DataGroupModelSwitchCommand = new ReplayCommand(DataGroupModelSwitchCommandHandler);
		FlowWaterModelSwitchCommand = new ReplayCommand(FlowWaterModelSwitchCommandHandler);
	}

	public void ClearAlbums()
	{
		Ids?.Clear();
		Albums?.Clear();
		ResetAlbumToolBarStatus();
	}

	private void CameraNavgationClickCommandHandler(object parameter)
	{
		if (parameter == null || !(parameter is ListBox listBox))
		{
			return;
		}
		listBox.SelectedIndex = 0;
		PicToolBackToAlbumsBtn.Visibility = Visibility.Collapsed;
		PicToolRefreshBtn.IsEnabled = false;
		CurrentViewModel = PicCurrentGroupMode;
		if (FocusedAlbum != CamareAlbum)
		{
			ResetSelectPicStatus();
			PicToolExportBtn.IsEnabled = false;
			PicToolDeleteBtn.IsEnabled = false;
			_ = FocusedAlbum;
			FocusedAlbum = CamareAlbum;
			try
			{
				bll.RefreshAlbumPicList(FocusedAlbum, delegate
				{
					SetToolBarEnable();
				});
				return;
			}
			catch (Exception)
			{
				PicToolRefreshBtn.IsEnabled = true;
				return;
			}
		}
		PicToolRefreshBtn.IsEnabled = true;
	}

	private void AlubmNavgationClickCommandHandler(object parameter)
	{
		if (parameter != null && parameter is ListBox listBox)
		{
			listBox.SelectedIndex = 1;
			CurrentViewModel = PicMgtContentViewDisplayMode.AlbumList;
		}
	}

	private void DataGroupModelSwitchCommandHandler(object parameter)
	{
		PicCurrentGroupMode = PicMgtContentViewDisplayMode.PicListWithDateGroup;
		CurrentViewModel = PicMgtContentViewDisplayMode.PicListWithDateGroup;
	}

	private void FlowWaterModelSwitchCommandHandler(object parameter)
	{
		PicCurrentGroupMode = PicMgtContentViewDisplayMode.PicListWithFlowWater;
		CurrentViewModel = PicMgtContentViewDisplayMode.PicListWithFlowWater;
	}

	private void PicToolBarInitialize()
	{
		_picToolBackToAlbumsBtn = new OperatorButtonViewModel
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("backDrawingImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("backDrawingImage") as ImageSource),
			ButtonText = "K0559",
			ButtonTextDisplay = "K0559",
			Visibility = Visibility.Collapsed,
			ClickCommand = new ReplayCommand(PicToolBackToAlbumsBtnCommandHandler)
		};
		_picToolImportBtn = new OperatorButtonViewModel
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarImportImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarImportDisabledImage") as ImageSource),
			ButtonText = "K0429",
			ButtonTextDisplay = "K0429",
			Visibility = Visibility.Visible,
			IsEnabled = true,
			ClickCommand = new ReplayCommand(PicToolImportCommandHandler)
		};
		_picToolExportBtn = new OperatorButtonViewModel
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarExportImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarExportDisabledImage") as ImageSource),
			ButtonText = "K0484",
			ButtonTextDisplay = "K0484",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(PicToolExportCommandHandler)
		};
		_picToolDeleteBtn = new OperatorButtonViewModel
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarDeleteImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarDeleteDisabledImage") as ImageSource),
			ButtonText = "K0583",
			ButtonTextDisplay = "K0583",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(PicToolDeleteCommandHandler)
		};
		_picToolRefreshBtn = new OperatorButtonViewModel
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarRefreshImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarRefreshDisabledImage") as ImageSource),
			ButtonText = "K0473",
			ButtonTextDisplay = "K0473",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(PicToolRefreshCommandHandler)
		};
	}

	private void PicToolBackToAlbumsBtnCommandHandler(object parameter)
	{
		PicToolBackToAlbumsBtn.Visibility = Visibility.Collapsed;
		CurrentViewModel = PicMgtContentViewDisplayMode.AlbumList;
	}

	private void PicToolImportCommandHandler(object parameter)
	{
		bll.ImportPicToDevice();
		PicToolRefreshCommandHandler(null);
	}

	private void PicToolExportCommandHandler(object parameter)
	{
		bll.ExportDevicePic();
	}

	private void PicToolDeleteCommandHandler(object parameter)
	{
		PicAlbumViewModel focusedAlbum = SingleInstance.FocusedAlbum;
		if (focusedAlbum == null)
		{
			return;
		}
		ObservableCollection<PicInfoViewModel> cachedAllPics = focusedAlbum.CachedAllPics;
		if (cachedAllPics != null && cachedAllPics.Count != 0)
		{
			List<string> list = (from p in cachedAllPics
				where p.IsSelected
				select p.RawPicInfo?.Id).ToList();
			if (list.Count != 0 && MessageBoxHelper.DeleteConfirmMessagebox(ResourcesHelper.StringResources.SingleInstance.CONTACT_DELETE_TITLE, ResourcesHelper.StringResources.SingleInstance.PIC_DELETE_CONTENT))
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
			if (!PicToolRefreshBtn.IsEnabled)
			{
				return;
			}
			ResetSelectPicStatus();
			PicToolDeleteBtn.IsEnabled = false;
			PicToolExportBtn.IsEnabled = false;
			PicToolRefreshBtn.IsEnabled = false;
		}
		try
		{
			SingleInstance.AlbumThumbnailIsLoad = false;
			List<PicServerAlbumInfo> picAlbumsViewModel = bll.GetPicAlbumsViewModel();
			LogHelper.LogInstance.Info("Get Albums finished!");
			bll.LoadAlbumsInfo(picAlbumsViewModel);
			LogHelper.LogInstance.Info("Load Albums finished!");
			bll.RefreshAlbumPicList(FocusedAlbum, delegate
			{
				lock (this)
				{
					SetToolBarEnable();
				}
				bll.RefreshAlbumThumbnailList();
			});
		}
		catch (Exception)
		{
			PicToolRefreshBtn.IsEnabled = true;
		}
	}

	public void SetToolBarEnable()
	{
		if (FocusedAlbum == null)
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			bool isEnabled = FocusedAlbum.CachedAllPics.Any((PicInfoViewModel p) => p.IsSelected);
			PicToolDeleteBtn.IsEnabled = isEnabled;
			PicToolExportBtn.IsEnabled = isEnabled;
			PicToolRefreshBtn.IsEnabled = true;
		});
	}

	private void AlbumToolBarInitialize()
	{
		_albumToolImportBtn = new OperatorButtonViewModel
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarImportImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarImportDisabledImage") as ImageSource),
			ButtonText = "K0429",
			ButtonTextDisplay = "K0429",
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(AlbumToolImportCommandHandler)
		};
		_albumToolExportBtn = new OperatorButtonViewModel
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarExportImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarExportDisabledImage") as ImageSource),
			ButtonText = "K0484",
			ButtonTextDisplay = "K0484",
			Visibility = Visibility.Visible,
			IsEnabled = false,
			ClickCommand = new ReplayCommand(AlbumToolExportCommandHandler)
		};
		_albumToolDeleteBtn = new OperatorButtonViewModel
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarDeleteImage") as ImageSource),
			ButtonDisabledImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarDeleteDisabledImage") as ImageSource),
			ButtonText = "K0583",
			ButtonTextDisplay = "K0583",
			Visibility = Visibility.Visible,
			IsEnabled = false,
			ClickCommand = new ReplayCommand(AlbumToolDeleteCommandHandler)
		};
		_albumToolRefreshBtn = new OperatorButtonViewModel
		{
			ButtonImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarRefreshImage") as ImageSource),
			ButtonSelectedImageSource = (LeftNavResources.SingleInstance.GetResource("TopBarRefreshImage") as ImageSource),
			ButtonText = "K0473",
			ButtonTextDisplay = "K0473",
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(AlbumToolRefreshCommandHandler)
		};
	}

	private void AlbumToolImportCommandHandler(object parameter)
	{
		bll.ImportAlbumToDevice();
	}

	private void AlbumToolExportCommandHandler(object parameter)
	{
		bll.ExportDeviceAlbum();
	}

	private void AlbumToolDeleteCommandHandler(object parameter)
	{
	}

	private void AlbumToolRefreshCommandHandler(object parameter)
	{
		AlbumToolRefreshBtn.IsEnabled = false;
		bll.RefreshAlbumList(null);
	}

	public void ResetPicToolBarStatus()
	{
		PicSelectionHandler(result: false);
	}

	private void PicSelectAllCommandHandler(object parameter)
	{
		PicAlbumViewModel focusedAlbum = FocusedAlbum;
		if (focusedAlbum == null || focusedAlbum.CachedPicList.Count == 0)
		{
			return;
		}
		bool value = (parameter as CheckBox).IsChecked.Value;
		foreach (PicInfoListViewModel cachedPic in focusedAlbum.CachedPicList)
		{
			cachedPic.SelectAll(value);
		}
	}

	public void PicSelectionHandler(bool result)
	{
		ResetPicSelectAllButtonStatus();
		ReSetPicToolBtnStatus();
	}

	public void ResetSelectPicStatus()
	{
		foreach (PicAlbumViewModel album in Albums)
		{
			album.IsSelected = false;
		}
		PicAlbumViewModel focusedAlbum = FocusedAlbum;
		if (focusedAlbum == null)
		{
			return;
		}
		ObservableCollection<PicInfoViewModel> cachedAllPics = focusedAlbum.CachedAllPics;
		if (cachedAllPics == null)
		{
			return;
		}
		foreach (PicInfoViewModel item in cachedAllPics.Where((PicInfoViewModel m) => m.IsSelected))
		{
			item.IsSelected = false;
		}
	}

	private void ReSetPicToolBtnStatus()
	{
		PicAlbumViewModel focusedAlbum = FocusedAlbum;
		if (focusedAlbum != null)
		{
			ObservableCollection<PicInfoViewModel> cachedAllPics = focusedAlbum.CachedAllPics;
			if (cachedAllPics != null && cachedAllPics.Where((PicInfoViewModel m) => m.IsSelected).FirstOrDefault() != null)
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

	private void ResetPicSelectAllButtonStatus()
	{
		PicAlbumViewModel focusedAlbum = FocusedAlbum;
		if (focusedAlbum == null)
		{
			return;
		}
		ObservableCollection<PicInfoViewModel> cachedAllPics = focusedAlbum.CachedAllPics;
		if (cachedAllPics == null || cachedAllPics.Count == 0)
		{
			IsPicSelectedAll = false;
			return;
		}
		IsPicSelectedAll = cachedAllPics.Where((PicInfoViewModel m) => !m.IsSelected).FirstOrDefault() == null;
	}

	public void ResetAlbumToolBarStatus()
	{
		AlbumSclectionHandler(selected: false);
	}

	private void AlbumSelectAllCommandHandler(object parameter)
	{
		ObservableCollection<PicAlbumViewModel> albums = Albums;
		if (albums == null)
		{
			return;
		}
		bool value = (parameter as CheckBox).IsChecked.Value;
		foreach (PicAlbumViewModel item in albums)
		{
			item.IsSelected = value;
		}
	}

	public void AlbumSclectionHandler(bool selected)
	{
		ResetAlbumSelectAllButtonStatus();
	}

	private void ReSetAlbumToolBtnStatus()
	{
		ObservableCollection<PicAlbumViewModel> albums = Albums;
		if (albums != null && albums.Where((PicAlbumViewModel m) => m.IsSelected).FirstOrDefault() != null)
		{
			AlbumToolExportBtn.IsEnabled = true;
		}
		else
		{
			AlbumToolExportBtn.IsEnabled = false;
		}
	}

	private void ResetAlbumSelectAllButtonStatus()
	{
		ObservableCollection<PicAlbumViewModel> albums = Albums;
		if (albums == null || albums.Count == 0)
		{
			IsAlbumSelectedAll = false;
			return;
		}
		IsAlbumSelectedAll = albums.Where((PicAlbumViewModel m) => !m.IsSelected).FirstOrDefault() == null;
	}

	private void PicMgtContentViewDisplayModeChangedHandler(PicMgtContentViewDisplayMode currentMode)
	{
		switch (currentMode)
		{
		case PicMgtContentViewDisplayMode.AlbumList:
			AlbumToolBarVisibility = Visibility.Visible;
			PicToolBarVisibility = Visibility.Collapsed;
			break;
		case PicMgtContentViewDisplayMode.PicListWithDateGroup:
			PicCurrentGroupMode = PicMgtContentViewDisplayMode.PicListWithDateGroup;
			AlbumToolBarVisibility = Visibility.Collapsed;
			PicToolBarVisibility = Visibility.Visible;
			break;
		case PicMgtContentViewDisplayMode.PicListWithFlowWater:
			PicCurrentGroupMode = PicMgtContentViewDisplayMode.PicListWithFlowWater;
			AlbumToolBarVisibility = Visibility.Collapsed;
			PicToolBarVisibility = Visibility.Visible;
			break;
		}
	}
}
