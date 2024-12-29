using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModelV6;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class VideoMgtViewModelV7 : ViewModelBase
{
	private readonly DeviceVideoManager bllVideo = new DeviceVideoManager();

	protected SdCardViewModel sdCarVm;

	private int _allAlbumVideoCount;

	private int _allAlbumVideoSelectedCount;

	private bool? _isAllVideoSelected;

	private ObservableCollection<VideoAlbumViewModelV7> _albumList;

	private VideoAlbumViewModelV7 _focusedAlbum;

	public ScrollViewer DateGroupScrollViewer { get; set; }

	public ScrollViewer PicNotGroupScrollViewer { get; set; }

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

	public bool IsSelectedInternal => SdCarVm.StorageSelIndex == 0;

	public int AllAlbumVideoCount
	{
		get
		{
			return _allAlbumVideoCount;
		}
		set
		{
			_allAlbumVideoCount = value;
			OnPropertyChanged("AllAlbumVideoCount");
		}
	}

	public int AllAlbumVideoSelectedCount
	{
		get
		{
			return _allAlbumVideoSelectedCount;
		}
		set
		{
			if (_allAlbumVideoSelectedCount != value)
			{
				_allAlbumVideoSelectedCount = value;
				OnPropertyChanged("AllAlbumVideoSelectedCount");
			}
		}
	}

	public bool? IsSelectedAllAlbumVideo
	{
		get
		{
			return _isAllVideoSelected;
		}
		set
		{
			_isAllVideoSelected = value;
			OnPropertyChanged("IsSelectedAllAlbumVideo");
		}
	}

	public ObservableCollection<VideoAlbumViewModelV7> AlbumList
	{
		get
		{
			return _albumList;
		}
		set
		{
			if (_albumList != value)
			{
				_albumList = value;
				OnPropertyChanged("AlbumList");
			}
		}
	}

	public List<VideoAlbumViewModelV7> AlbumsOriginal { get; set; }

	public VideoAlbumViewModelV7 FocusedAlbum
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

	public VideoAlbumViewModelV7 FocusedAlbumOriginal { get; private set; }

	public OperatorButtonViewModelV6 VideoToolImportBtn { get; private set; }

	public OperatorButtonViewModelV6 VideoToolExportBtn { get; private set; }

	public OperatorButtonViewModelV6 VideoToolDeleteBtn { get; private set; }

	public OperatorButtonViewModelV6 VideoToolRefreshBtn { get; private set; }

	public ReplayCommand SearchCommand { get; set; }

	public VideoMgtViewModelV7()
	{
		SdCarVm = new SdCardViewModel();
		AlbumList = new ObservableCollection<VideoAlbumViewModelV7>();
		AlbumsOriginal = new List<VideoAlbumViewModelV7>();
		bllVideo = new DeviceVideoManager();
		VideoToolImportBtn = new OperatorButtonViewModelV6
		{
			ButtonText = "K0429",
			ButtonTextDisplay = "K0429",
			Visibility = Visibility.Visible,
			IsEnabled = true,
			ClickCommand = new ReplayCommand(ImportVideoCommandHandler)
		};
		VideoToolExportBtn = new OperatorButtonViewModelV6
		{
			ButtonText = "K0484",
			ButtonTextDisplay = "K0484",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(ExportVideoCommandHandler)
		};
		VideoToolDeleteBtn = new OperatorButtonViewModelV6
		{
			ButtonText = "K0583",
			ButtonTextDisplay = "K0583",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(DeleteVideoCommandHandler)
		};
		VideoToolRefreshBtn = new OperatorButtonViewModelV6
		{
			ButtonText = "K0473",
			ButtonTextDisplay = "K0473",
			IsEnabled = false,
			Visibility = Visibility.Visible,
			ClickCommand = new ReplayCommand(RefreshCommandHandler)
		};
		SearchCommand = new ReplayCommand(SearchCommandHandler);
	}

	public override void LoadData()
	{
		SdCarVm.LoadData(Context.CurrentDevice);
		base.LoadData();
		ResetEx();
		RefreshCommandHandler(null);
	}

	public void ResetEx()
	{
		VideoToolExportBtn.IsEnabled = false;
		VideoToolDeleteBtn.IsEnabled = false;
		VideoToolRefreshBtn.IsEnabled = true;
		IsSelectedAllAlbumVideo = false;
	}

	private void ImportVideoCommandHandler(object prameter)
	{
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Title = "K0496";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			openFileDialog.Title = HostProxy.LanguageService.Translate("K0496");
		}
		openFileDialog.Filter = "Videos|*.mp4;*.rm;*.rmvb;*.avi;*.wmv;";
		openFileDialog.Multiselect = true;
		openFileDialog.FileName = string.Empty;
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() != true)
		{
			return;
		}
		List<string> fileNames = openFileDialog.FileNames.ToList();
		fileNames = DeviceCommonManagementV6.CheckImportFiles(fileNames);
		if (fileNames.Count != 0 && HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { Property: not null } tcpAndroidDevice)
		{
			string text = FocusedAlbum?.AlbumPath;
			if (string.IsNullOrEmpty(text))
			{
				text = Path.Combine(tcpAndroidDevice.Property.InternalStoragePath, "DCIM/Camera");
			}
			string appSaveDir = text;
			if (HostProxy.deviceManager.MasterDevice.ConnectedAppType == "Moto")
			{
				appSaveDir = Path.Combine(tcpAndroidDevice.Property.InternalStoragePath, text, "/");
			}
			new ImportAndExportWrapper().ImportFile(BusinessType.VIDEO_IMPORT, 20, ResourcesHelper.StringResources.SingleInstance.VIDEO_IMPORT_MESSAGE, "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", BackupRestoreStaticResources.SingleInstance.VIDEO, () => fileNames, (string sourcePath) => Path.Combine(appSaveDir, Path.GetFileName(sourcePath)));
			RefreshCommandHandler(null);
		}
	}

	private void ExportVideoCommandHandler(object prameter)
	{
		List<string> selectedVideoIds = GetSelectedVideoIds();
		if (selectedVideoIds.Count != 0)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if (folderBrowserDialog.ShowDialog() != DialogResult.Cancel)
			{
				string saveDir = folderBrowserDialog.SelectedPath.Trim();
				new ImportAndExportWrapper().ExportFile(BusinessType.VIDEO_EXPORT, 20, selectedVideoIds, ResourcesHelper.StringResources.SingleInstance.VIDEO_EXPORT_MESSAGE, "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", ResourcesHelper.StringResources.SingleInstance.VIDEO_CONTENT, saveDir, null);
			}
		}
	}

	private void DeleteVideoCommandHandler(object prameter)
	{
		List<string> _idList = GetSelectedVideoIds(_isRemoveLagerFile: false);
		if (_idList.Count == 0)
		{
			return;
		}
		bool? flag = Context.MessageBox.ShowMessage(ResourcesHelper.StringResources.SingleInstance.VIDEO_DELETE_CONTENT, MessageBoxButton.OKCancel);
		if (flag.HasValue && flag.Value)
		{
			AsyncDataLoader.Loading(delegate
			{
				Stopwatch stopwatch = new Stopwatch();
				Dictionary<string, int> result = new Dictionary<string, int>();
				TcpAndroidDevice device = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
				BusinessData businessData = new BusinessData(BusinessType.VIDEO_DELETE, device);
				stopwatch.Start();
				bllVideo.DeleteDevFilesWithConfirm("deleteVideosById", _idList, ref result);
				stopwatch.Stop();
				HostProxy.BehaviorService.Collect(BusinessType.VIDEO_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, (result["success"] > 0) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, result));
			});
			RefreshCommandHandler(null);
		}
	}

	public void RefreshCommandHandler(object prameter)
	{
		if (prameter != null)
		{
			(prameter as System.Windows.Controls.TextBox).Text = string.Empty;
		}
		VideoToolExportBtn.IsEnabled = false;
		VideoToolDeleteBtn.IsEnabled = false;
		VideoToolRefreshBtn.IsEnabled = false;
		AsyncDataLoader.BeginLoading(delegate
		{
			try
			{
				if (!bllVideo.GetVideoCount(out var _, out var _))
				{
					return new Tuple<bool, string, string>(item1: false, "K0668", "Maybe your storage space is insufficient, please check");
				}
				List<ServerAlbumInfo> albums = bllVideo.GetAlbums();
				GetVideoList(albums);
				IsSelectedAllAlbumVideo = false;
				Task.Factory.StartNew(delegate
				{
					UpdateVideoImages(albums);
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						VideoToolRefreshBtn.IsEnabled = true;
						FocusedAlbumClone();
					});
				});
			}
			catch
			{
				return new Tuple<bool, string, string>(item1: false, "K0668", "Maybe your storage space is insufficient, please check");
			}
			return (Tuple<bool, string, string>)null;
		}, ViewContext.SingleInstance.MainViewModel);
	}

	private void GetVideoList(List<ServerAlbumInfo> albums)
	{
		List<Video> videos = bllVideo.GetVideos(albums);
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			AlbumList.Clear();
			AlbumsOriginal.Clear();
			if (FocusedAlbum != null)
			{
				FocusedAlbum.CachedAllVideos.Clear();
				FocusedAlbum.IsSelectedAllVideo = false;
				FocusedAlbum = null;
			}
			int num = 0;
			foreach (ServerAlbumInfo album in albums)
			{
				VideoAlbumViewModelV7 videoAlbumViewModelV = new VideoAlbumViewModelV7
				{
					RawAlbumInfo = album
				};
				if (videoAlbumViewModelV.IsInternalPath == IsSelectedInternal)
				{
					num += videoAlbumViewModelV.FileCount;
					AlbumsOriginal.Add(videoAlbumViewModelV);
				}
			}
			AllAlbumVideoCount = num;
			AllAlbumVideoSelectedCount = 0;
			foreach (Video video in videos.OrderByDescending((Video n) => n.ModifiyDate).ToList())
			{
				VideoInfoViewModelV7 videoInfoViewModelV = new VideoInfoViewModelV7();
				videoInfoViewModelV.RawVideoInfo = new VideoInfo();
				videoInfoViewModelV.RawVideoInfo.SetData(video, albums.SingleOrDefault((ServerAlbumInfo a) => a.AlbumPath == video.Album));
				videoInfoViewModelV.SetGroupKey();
				AlbumsOriginal.SingleOrDefault((VideoAlbumViewModelV7 a) => a.AlbumPath == video.Album)?.CachedAllVideos.Add(videoInfoViewModelV);
			}
			foreach (VideoAlbumViewModelV7 item in AlbumsOriginal)
			{
				item.ResetVideoGroupList();
			}
			if (AlbumsOriginal.Count > 0)
			{
				IEnumerable<VideoAlbumViewModelV7> enumerable = AlbumsOriginal.Where((VideoAlbumViewModelV7 m) => m.IsInternalPath == IsSelectedInternal);
				if (enumerable.FirstOrDefault() != null)
				{
					enumerable.FirstOrDefault().IsSelected = true;
					enumerable.FirstOrDefault().SingleClickCommand.Execute(true);
				}
				AlbumList = new ObservableCollection<VideoAlbumViewModelV7>(enumerable);
			}
			else
			{
				AlbumList = new ObservableCollection<VideoAlbumViewModelV7>();
			}
		});
	}

	private void UpdateVideoImages(List<ServerAlbumInfo> albums)
	{
		if (albums == null || albums.Count == 0)
		{
			return;
		}
		try
		{
			string videoCacheDir = Configurations.VideoCacheDir;
			bllVideo.ExportVideoThumbnails(albums, videoCacheDir, delegate(ServerAlbumInfo album, VideoThumbnails video, bool res)
			{
				if (res)
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						VideoAlbumViewModelV7 videoAlbumViewModelV = AlbumsOriginal.FirstOrDefault((VideoAlbumViewModelV7 a) => a.AlbumPath == album.AlbumPath);
						if (videoAlbumViewModelV != null)
						{
							VideoInfoViewModelV7 videoInfoViewModelV = videoAlbumViewModelV.CachedAllVideos.FirstOrDefault((VideoInfoViewModelV7 v) => v.RawVideoInfo.RawVideo.Name == video.Name);
							if (videoInfoViewModelV != null)
							{
								videoInfoViewModelV.RawVideoInfo.LocalVideoImagePath = video.LocalFilePath;
								videoInfoViewModelV.VideoImage = ImageHandleHelper.LoadBitmap(video.LocalFilePath);
								LogHelper.LogInstance.Debug("albumVM.VideoImage:" + videoInfoViewModelV.VideoImage.ToString());
							}
						}
					});
				}
			});
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Export video image throw exception:" + ex.ToString());
		}
	}

	private void SearchCommandHandler(object prameter)
	{
		if (prameter == null)
		{
			return;
		}
		foreach (VideoInfoViewModelV7 _single in FocusedAlbum.CachedAllVideos)
		{
			FocusedAlbumOriginal.CachedAllVideos.First((VideoInfoViewModelV7 m) => m.RawVideoInfo.RawVideo.Id == _single.RawVideoInfo.RawVideo.Id).IsSelected = _single.IsSelected;
		}
		foreach (VideoInfoGroupViewModelV7 _single2 in FocusedAlbum.CacheVideoGroupList)
		{
			foreach (VideoInfoViewModelV7 _singleChild in _single2.Videos)
			{
				FocusedAlbumOriginal.CacheVideoGroupList.First((VideoInfoGroupViewModelV7 m) => m.GroupKey == _single2.GroupKey).Videos.First((VideoInfoViewModelV7 m) => m.RawVideoInfo.RawVideo.Id == _singleChild.RawVideoInfo.RawVideo.Id).IsSelected = _singleChild.IsSelected;
			}
		}
		string searchText = prameter.ToString().ToLower();
		FocusedAlbum.CachedAllVideos = new ObservableCollection<VideoInfoViewModelV7>(FocusedAlbumOriginal.CachedAllVideos.Where((VideoInfoViewModelV7 m) => m.VideoName.ToLower().Contains(searchText)));
		new ObservableCollection<VideoInfoGroupViewModelV7>();
		for (int i = 0; i < FocusedAlbumOriginal.CacheVideoGroupList.Count; i++)
		{
			IEnumerable<VideoInfoViewModelV7> collection = FocusedAlbumOriginal.CacheVideoGroupList[i].Videos.Where((VideoInfoViewModelV7 m) => m.VideoName.ToLower().Contains(searchText));
			FocusedAlbum.CacheVideoGroupList[i].SetVideos(new ObservableCollection<VideoInfoViewModelV7>(collection));
		}
	}

	private List<string> GetSelectedVideoIds(bool _isRemoveLagerFile = true)
	{
		List<string> list = new List<string>();
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		foreach (VideoAlbumViewModelV7 album in AlbumList)
		{
			foreach (VideoInfoViewModelV7 cachedAllVideo in album.CachedAllVideos)
			{
				if (cachedAllVideo.IsSelected)
				{
					if (cachedAllVideo.Size > 4294967296L && _isRemoveLagerFile)
					{
						dictionary.Add(cachedAllVideo.RawVideoInfo.FilePath, cachedAllVideo.Size);
					}
					else
					{
						list.Add(cachedAllVideo.RawVideoInfo.RawVideo.Id.ToString());
					}
				}
			}
		}
		DeviceCommonManagementV6.CheckExportFiles(dictionary);
		return list;
	}

	public override void Reset()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			base.Reset();
			if (DataIsLoaded)
			{
				AlbumList.Clear();
			}
		});
	}

	public void RefreshAllAlbumPicSelectedCount()
	{
		int num = 0;
		foreach (VideoAlbumViewModelV7 album in AlbumList)
		{
			foreach (VideoInfoViewModelV7 cachedAllVideo in album.CachedAllVideos)
			{
				if (cachedAllVideo.IsSelected)
				{
					num++;
				}
			}
		}
		VideoToolExportBtn.IsEnabled = num > 0;
		VideoToolDeleteBtn.IsEnabled = num > 0;
		AllAlbumVideoSelectedCount = num;
	}

	public void FocusedAlbumClone()
	{
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			VideoAlbumViewModelV7 videoAlbumViewModelV = AlbumsOriginal.FirstOrDefault((VideoAlbumViewModelV7 n) => n.IsSelected);
			if (videoAlbumViewModelV != null)
			{
				string json = JsonHelper.SerializeObject2Json(videoAlbumViewModelV);
				FocusedAlbumOriginal = JsonHelper.DeserializeJson2Object<VideoAlbumViewModelV7>(json);
				foreach (VideoInfoGroupViewModelV7 cacheVideoGroup in FocusedAlbumOriginal.CacheVideoGroupList)
				{
					foreach (VideoInfoViewModelV7 video in cacheVideoGroup.Videos)
					{
						video.ResetCloneImageUri();
					}
				}
			}
		});
	}
}
