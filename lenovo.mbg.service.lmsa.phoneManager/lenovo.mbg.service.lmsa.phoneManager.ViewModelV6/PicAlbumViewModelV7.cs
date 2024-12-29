using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class PicAlbumViewModelV7 : ViewModelBase
{
	private DevicePicManagementBLLV7 bll = new DevicePicManagementBLLV7();

	private ObservableCollection<PicInfoViewModelV7> _allpics;

	private ObservableCollection<PicInfoListViewModelV7> _picList;

	private PicServerAlbumInfo _serverAlbumInfo;

	private string _AlbumPath;

	private int _fileCount;

	private bool _isSeleted;

	private bool? _isSeletedAllPic = false;

	private int PicloadIndex;

	private PicInfoListViewModelV7 StartList;

	private int ListStartIndex;

	public int DataVersion { get; set; }

	private PicMgtViewModelV7 GetCurrentViewModel => Context.FindViewModel<PicMgtViewModelV7>(typeof(PICMgtViewV7));

	public ObservableCollection<PicInfoViewModelV7> CachedAllPics
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

	public ObservableCollection<PicInfoListViewModelV7> CachedPicList
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

	public bool? IsSelectedAllPic
	{
		get
		{
			return _isSeletedAllPic;
		}
		set
		{
			if (_isSeletedAllPic != value)
			{
				_isSeletedAllPic = value;
				SelectAllPicInAlbum(value);
				OnPropertyChanged("IsSelectedAllPic");
			}
		}
	}

	public ReplayCommand SingleClickCommand { get; set; }

	public PicServerAlbumInfo AlbumInfo { get; set; }

	public PicAlbumViewModelV7()
	{
		SingleClickCommand = new ReplayCommand(EnterAlbumPicList);
		CachedPicList = new ObservableCollection<PicInfoListViewModelV7>();
		CachedAllPics = new ObservableCollection<PicInfoViewModelV7>();
	}

	private void EnterAlbumPicList(object obj)
	{
		bool flag = false;
		if (obj is bool)
		{
			flag = Convert.ToBoolean(obj);
		}
		if (GetCurrentViewModel.FocusedAlbum != this)
		{
			GetCurrentViewModel.PicToolRefreshBtn.IsEnabled = false;
			Task.Factory.StartNew(delegate(object albums)
			{
				PicAlbumViewModelV7 focusedAlbum = albums as PicAlbumViewModelV7;
				new DevicePicManagementBLLV7().RefreshAlbumPicList(focusedAlbum, delegate
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						GetCurrentViewModel.PicToolRefreshBtn.IsEnabled = true;
						GetCurrentViewModel.ReSetPicToolBtnStatus();
						GetCurrentViewModel.FocusedAlbumClone();
					});
				});
			}, this);
			if (!flag)
			{
				GetCurrentViewModel.FocusedAlbum = this;
			}
		}
		if (!flag)
		{
			IsSelected = true;
			GetCurrentViewModel.FocusedAlbumClone();
		}
	}

	private void SelectAllPicInAlbum(bool? _selected)
	{
		if (_selected.HasValue)
		{
			foreach (PicInfoViewModelV7 cachedAllPic in CachedAllPics)
			{
				cachedAllPic.IsNotUserClick = true;
				cachedAllPic.IsSelected = _selected.Value;
				cachedAllPic.IsNotUserClick = false;
			}
			foreach (PicInfoListViewModelV7 cachedPic in CachedPicList)
			{
				cachedPic.IsListSelected = _selected.Value;
			}
			ObservableCollection<PicAlbumViewModelV7> albums = GetCurrentViewModel.Albums;
			if (_selected.Value)
			{
				if (albums.Count((PicAlbumViewModelV7 m) => m.IsSelectedAllPic != true) == 0)
				{
					GetCurrentViewModel.IsSelectedAllAlbumPic = true;
				}
				else
				{
					GetCurrentViewModel.IsSelectedAllAlbumPic = null;
				}
			}
			else if (albums.Count((PicAlbumViewModelV7 m) => m.IsSelectedAllPic != false) == 0)
			{
				GetCurrentViewModel.IsSelectedAllAlbumPic = false;
			}
			else
			{
				GetCurrentViewModel.IsSelectedAllAlbumPic = null;
			}
		}
		GetCurrentViewModel.RefreshAllAlbumPicSelectedCount();
	}

	public void CountPicIds(int startIndex, int endIndex)
	{
		string id = Id;
		List<PicInfoViewModelV7> list = new List<PicInfoViewModelV7>();
		ObservableCollection<PicInfoViewModelV7> cachedAllPics = CachedAllPics;
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
		GetCurrentViewModel.PicToolRefreshBtn.IsEnabled = false;
		bll.LoadAlbumPicsThumbnail(id, list, delegate
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				GetCurrentViewModel.PicToolRefreshBtn.IsEnabled = true;
			});
		});
	}

	internal void LoadPicThumbnailList(double extentHeight, double extentWidth, double verticalOffset, double viewportHeight, double viewportWidth)
	{
		string id = Id;
		List<PicInfoViewModelV7> list = new List<PicInfoViewModelV7>();
		ObservableCollection<PicInfoListViewModelV7> cachedPicList = CachedPicList;
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
					if (cachedPicList[j].Pics[k].RawPicInfo != null && string.IsNullOrEmpty(cachedPicList[j].Pics[k].RawPicInfo.LocalFilePath) && cachedPicList[j].Pics[k].RawPicInfo.RawAlbumPath.StartsWith("/storage/emulated/0") == (GetCurrentViewModel.SdCarVm.StorageSelIndex == 0))
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
		GetCurrentViewModel.PicToolRefreshBtn.IsEnabled = false;
		bll.LoadAlbumPicsThumbnail(id, list, delegate
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				GetCurrentViewModel.PicToolRefreshBtn.IsEnabled = true;
			});
		});
	}

	public void Remove(ServerPicInfo serverPicInfo)
	{
		ObservableCollection<PicInfoViewModelV7> cachedAllPics = CachedAllPics;
		ObservableCollection<PicInfoListViewModelV7> cachedPicList = CachedPicList;
		PicInfoViewModelV7 picInfoViewModelV = cachedAllPics.FirstOrDefault((PicInfoViewModelV7 m) => m.RawPicInfo != null && m.RawPicInfo.Id.Equals(serverPicInfo.Id));
		if (picInfoViewModelV != null)
		{
			cachedAllPics.Remove(picInfoViewModelV);
		}
		List<PicInfoListViewModelV7> list = new List<PicInfoListViewModelV7>();
		foreach (PicInfoListViewModelV7 item in cachedPicList)
		{
			if (item.Pics == null)
			{
				continue;
			}
			picInfoViewModelV = item.Pics.FirstOrDefault((PicInfoViewModelV7 m) => m.RawPicInfo != null && m.RawPicInfo.Id.Equals(serverPicInfo.Id));
			if (picInfoViewModelV != null)
			{
				item.Pics.Remove(picInfoViewModelV);
				if (item.Pics.Count == 0)
				{
					list.Add(item);
				}
			}
		}
		foreach (PicInfoListViewModelV7 item2 in list)
		{
			cachedPicList.Remove(item2);
		}
	}
}
