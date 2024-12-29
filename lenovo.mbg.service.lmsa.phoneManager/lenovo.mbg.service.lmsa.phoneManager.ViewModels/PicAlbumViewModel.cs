using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class PicAlbumViewModel : ViewModelBase
{
	private DevicePicManagementBLL bll = new DevicePicManagementBLL();

	private Action<bool> _selectionHandler;

	private ObservableCollection<PicInfoViewModel> _allpics;

	private ObservableCollection<PicInfoListViewModel> _picList;

	private PicServerAlbumInfo _serverAlbumInfo;

	private string _AlbumPath;

	private int _fileCount;

	private bool _isCameraAlbum;

	private bool _isSeleted;

	private ImageSource _folderImageSource;

	private ImageSource _albumIconImageSource;

	private ImageSource _firstThumbnailImage;

	private ImageSource _secondThumbnailImage;

	private ImageSource _thirdThumbnailImage;

	private ImageSource _fourthThumbnailImage;

	private int _currentThumbnailCount;

	private int PicloadIndex;

	private PicInfoListViewModel StartList;

	private int ListStartIndex;

	public int DataVersion { get; set; }

	public ObservableCollection<PicInfoViewModel> CachedAllPics
	{
		get
		{
			return _allpics;
		}
		set
		{
			if (_allpics != value)
			{
				_allpics = value;
				OnPropertyChanged("CachedAllPics");
			}
		}
	}

	public ObservableCollection<PicInfoListViewModel> CachedPicList
	{
		get
		{
			return _picList;
		}
		set
		{
			if (_picList != value)
			{
				_picList = value;
				OnPropertyChanged("CachedPicList");
			}
		}
	}

	public PicServerAlbumInfo ServerAlbumInfo
	{
		get
		{
			return _serverAlbumInfo;
		}
		set
		{
			if (_serverAlbumInfo != value)
			{
				_serverAlbumInfo = value;
			}
		}
	}

	public string Id { get; set; }

	public string AlbumName { get; set; }

	public string AlbumPath
	{
		get
		{
			return _AlbumPath;
		}
		set
		{
			_AlbumPath = value;
			OnPropertyChanged("AlbumPath");
		}
	}

	public int FileCount
	{
		get
		{
			return _fileCount;
		}
		set
		{
			if (_fileCount != value)
			{
				_fileCount = value;
				OnPropertyChanged("FileCount");
			}
		}
	}

	public bool IsCameraAlbum
	{
		get
		{
			return _isCameraAlbum;
		}
		set
		{
			if (_isCameraAlbum != value)
			{
				_isCameraAlbum = value;
				OnPropertyChanged("IsCameraAlbum");
			}
		}
	}

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
				_selectionHandler?.BeginInvoke(_isSeleted, null, null);
				OnPropertyChanged("IsSelected");
			}
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

	public ReplayCommand SingleClickCommand { get; set; }

	public PicServerAlbumInfo AlbumInfo { get; set; }

	public ImageSource FirstThumbnailImage
	{
		get
		{
			return _firstThumbnailImage;
		}
		set
		{
			if (_firstThumbnailImage != value)
			{
				_firstThumbnailImage = value;
				OnPropertyChanged("FirstThumbnailImage");
			}
		}
	}

	public ImageSource SecondThumbnailImage
	{
		get
		{
			return _secondThumbnailImage;
		}
		set
		{
			if (_secondThumbnailImage != value)
			{
				_secondThumbnailImage = value;
				OnPropertyChanged("SecondThumbnailImage");
			}
		}
	}

	public ImageSource ThirdThumbnailImage
	{
		get
		{
			return _thirdThumbnailImage;
		}
		set
		{
			if (_thirdThumbnailImage != value)
			{
				_thirdThumbnailImage = value;
				OnPropertyChanged("ThirdThumbnailImage");
			}
		}
	}

	public ImageSource FourthThumbnailImage
	{
		get
		{
			return _fourthThumbnailImage;
		}
		set
		{
			if (_fourthThumbnailImage != value)
			{
				_fourthThumbnailImage = value;
				OnPropertyChanged("FourthThumbnailImage");
			}
		}
	}

	public PicAlbumViewModel(Action<bool> selectionHandler)
	{
		_selectionHandler = selectionHandler;
		_folderImageSource = LeftNavResources.SingleInstance.GetResource("camera_rollDrawingImage") as ImageSource;
		_albumIconImageSource = LeftNavResources.SingleInstance.GetResource("camera_rollDrawingImage") as ImageSource;
		DoubleClickCommand = new ReplayCommand(EnterAlbumPicList);
		SingleClickCommand = new ReplayCommand(SingleClickCommandHandler);
		CachedPicList = new ObservableCollection<PicInfoListViewModel>();
		CachedAllPics = new ObservableCollection<PicInfoViewModel>();
	}

	private void EnterAlbumPicList(object obj)
	{
		if (PicMgtViewModel.SingleInstance.FocusedAlbum != this)
		{
			PicMgtViewModel.SingleInstance.PicToolRefreshBtn.IsEnabled = false;
			PicMgtViewModel.SingleInstance.ResetSelectPicStatus();
			Task.Factory.StartNew(delegate(object albums)
			{
				PicAlbumViewModel focusedAlbum = albums as PicAlbumViewModel;
				new DevicePicManagementBLL().RefreshAlbumPicList(focusedAlbum, delegate
				{
					HostProxy.CurrentDispatcher?.Invoke(() => PicMgtViewModel.SingleInstance.PicToolRefreshBtn.IsEnabled = true);
				});
			}, this);
			PicMgtViewModel.SingleInstance.FocusedAlbum = this;
		}
		PicMgtViewModel.SingleInstance.PicToolBackToAlbumsBtn.Visibility = Visibility.Visible;
		PicMgtViewModel.SingleInstance.CurrentViewModel = PicMgtContentViewDisplayMode.PicListWithDateGroup;
	}

	public void ResetServerAlbumInfo(PicServerAlbumInfo serverAlbumInfo)
	{
		if (serverAlbumInfo != null)
		{
			ServerAlbumInfo = serverAlbumInfo;
			Id = serverAlbumInfo.Id;
			AlbumName = serverAlbumInfo.AlbumName;
			FileCount = serverAlbumInfo.FileCount;
			AlbumPath = serverAlbumInfo.Path;
		}
	}

	private void SingleClickCommandHandler(object parameter)
	{
		IsSelected = !IsSelected;
	}

	public void AddThumbnail(string filePath)
	{
		if (!File.Exists(filePath))
		{
			return;
		}
		try
		{
			BitmapImage bitmapImage = new BitmapImage();
			try
			{
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.UriSource = new Uri(filePath);
				bitmapImage.EndInit();
			}
			catch
			{
				bitmapImage = null;
				bitmapImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/pic_default.png", UriKind.RelativeOrAbsolute));
			}
			switch (_currentThumbnailCount)
			{
			case 0:
				_currentThumbnailCount = 1;
				FirstThumbnailImage = bitmapImage;
				break;
			case 1:
				_currentThumbnailCount = 2;
				SecondThumbnailImage = bitmapImage;
				break;
			case 2:
				_currentThumbnailCount = 3;
				ThirdThumbnailImage = bitmapImage;
				break;
			case 3:
				_currentThumbnailCount = 4;
				FourthThumbnailImage = bitmapImage;
				break;
			}
		}
		catch (Exception)
		{
		}
	}

	public void CountPicIds(int startIndex, int endIndex)
	{
		string id = Id;
		List<PicInfoViewModel> list = new List<PicInfoViewModel>();
		ObservableCollection<PicInfoViewModel> cachedAllPics = CachedAllPics;
		PicloadIndex = startIndex;
		for (int i = startIndex; i < endIndex; i++)
		{
			if (cachedAllPics.Count > i && cachedAllPics[i] != null && cachedAllPics[i].RawPicInfo != null && string.IsNullOrEmpty(cachedAllPics[i].RawPicInfo.LocalFilePath))
			{
				list.Add(cachedAllPics[i]);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		PicMgtViewModel.SingleInstance.PicToolRefreshBtn.IsEnabled = false;
		bll.LoadAlbumPicsThumbnail(id, list, delegate
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				PicMgtViewModel.SingleInstance.PicToolRefreshBtn.IsEnabled = true;
			});
		});
	}

	internal void LoadPicThumbnailList(double extentHeight, double extentWidth, double verticalOffset, double viewportHeight, double viewportWidth)
	{
		string id = Id;
		List<PicInfoViewModel> list = new List<PicInfoViewModel>();
		ObservableCollection<PicInfoListViewModel> cachedPicList = CachedPicList;
		int startIndex = 0;
		bll.GetPicDisplayStartIndex(cachedPicList.ToList(), extentHeight, extentWidth, verticalOffset, viewportHeight, viewportWidth, 10.0, out var hitGroup, out startIndex, out var _, out var _, out var listIndex, out var pageCount);
		if (hitGroup == null)
		{
			return;
		}
		ListStartIndex = startIndex;
		StartList = hitGroup;
		int num = ((ListStartIndex + pageCount > hitGroup.Pics.Count) ? hitGroup.Pics.Count : (ListStartIndex + pageCount));
		int num2 = 0;
		for (int i = startIndex; i < num && hitGroup.PicCount - 1 >= i; i++)
		{
			if (hitGroup.Pics[i].RawPicInfo != null && string.IsNullOrEmpty(hitGroup.Pics[i].RawPicInfo.LocalFilePath))
			{
				list.Add(hitGroup.Pics[i]);
			}
		}
		num2 += list.Count;
		if (num2 < pageCount)
		{
			int num3 = listIndex + 1;
			for (int j = num3; j < cachedPicList.Count - listIndex; j++)
			{
				if (num2 >= pageCount || j - num3 > 2)
				{
					num3 = j;
					break;
				}
				for (int k = 0; k < pageCount - num2 && cachedPicList[j].PicCount - 1 >= k; k++)
				{
					if (cachedPicList[j].Pics[k].RawPicInfo != null && string.IsNullOrEmpty(cachedPicList[j].Pics[k].RawPicInfo.LocalFilePath))
					{
						list.Add(cachedPicList[j].Pics[k]);
					}
				}
				num2 += list.Count;
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		PicMgtViewModel.SingleInstance.PicToolRefreshBtn.IsEnabled = false;
		bll.LoadAlbumPicsThumbnail(id, list, delegate
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				PicMgtViewModel.SingleInstance.PicToolRefreshBtn.IsEnabled = true;
			});
		});
	}

	public void Remove(ServerPicInfo serverPicInfo)
	{
		ObservableCollection<PicInfoViewModel> cachedAllPics = CachedAllPics;
		ObservableCollection<PicInfoListViewModel> cachedPicList = CachedPicList;
		PicInfoViewModel picInfoViewModel = cachedAllPics.Where((PicInfoViewModel m) => m.RawPicInfo != null && m.RawPicInfo.Id.Equals(serverPicInfo.Id)).FirstOrDefault();
		if (picInfoViewModel != null)
		{
			cachedAllPics.Remove(picInfoViewModel);
		}
		List<PicInfoListViewModel> list = new List<PicInfoListViewModel>();
		foreach (PicInfoListViewModel item in cachedPicList)
		{
			if (item.Pics == null)
			{
				continue;
			}
			picInfoViewModel = item.Pics.Where((PicInfoViewModel m) => m.RawPicInfo != null && m.RawPicInfo.Id.Equals(serverPicInfo.Id)).FirstOrDefault();
			if (picInfoViewModel != null)
			{
				item.Pics.Remove(picInfoViewModel);
				if (item.Pics.Count == 0)
				{
					list.Add(item);
				}
			}
		}
		foreach (PicInfoListViewModel item2 in list)
		{
			cachedPicList.Remove(item2);
		}
	}

	public void Clear()
	{
		FirstThumbnailImage = null;
		SecondThumbnailImage = null;
		ThirdThumbnailImage = null;
		FourthThumbnailImage = null;
		_currentThumbnailCount = 0;
	}

	public void Refresh()
	{
		FileCount = ((CachedAllPics != null) ? CachedAllPics.Count : 0);
	}
}
