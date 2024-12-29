using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class VideoMgtViewModel : ViewModelBase
{
	private static VideoMgtViewModel _singleInstance;

	private int _videoCount;

	private int _albumCount;

	private bool _isAllSelected;

	private List<VideoInfoViewModel> _tempVideoList;

	public ImageSource _defaultImage;

	private VideoViewType _MainView;

	private VideoViewType _AlbumView;

	private string _VideosTotalSize;

	private int _VideosChoosed;

	private int _AlbumChoosed;

	private int _VideosChoosedInSelectAlbum;

	public Action<VideoViewType> AlbumViewCallback;

	public static VideoMgtViewModel SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new VideoMgtViewModel();
			}
			return _singleInstance;
		}
	}

	public int VideoCount
	{
		get
		{
			return _videoCount;
		}
		set
		{
			_videoCount = value;
			OnPropertyChanged("VideoCount");
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
			_albumCount = value;
			OnPropertyChanged("AlbumCount");
		}
	}

	public string AlbumCountLabel => "K0547";

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

	public ObservableCollection<VideoInfoViewModel> VideoList { get; set; }

	public ObservableCollection<VideoAlbumViewModel> AlbumList { get; set; }

	public VideoAlbumViewModel SelectedAlbum { get; set; }

	public VideoViewType MainView
	{
		get
		{
			return _MainView;
		}
		set
		{
			_MainView = value;
			OnPropertyChanged("MainView");
		}
	}

	public VideoViewType AlbumView
	{
		get
		{
			return _AlbumView;
		}
		set
		{
			_AlbumView = value;
			OnPropertyChanged("AlbumView");
		}
	}

	public DeviceVideoManager VideoManager { get; set; }

	public ReplayCommand ImportVideo { get; set; }

	public ReplayCommand ExportVideo { get; set; }

	public Action OnDeleteEvent { get; set; }

	public ReplayCommand DeleteVideo { get; set; }

	public ReplayCommand Refresh { get; set; }

	public ReplayCommand BackToAlbum { get; set; }

	public ReplayCommand SearchCommand { get; set; }

	public string VideosTotalSize
	{
		get
		{
			return _VideosTotalSize;
		}
		set
		{
			_VideosTotalSize = value;
			OnPropertyChanged("VideosTotalSize");
		}
	}

	public int VideosChoosed
	{
		get
		{
			return _VideosChoosed;
		}
		set
		{
			_VideosChoosed = value;
			OnPropertyChanged("VideosChoosed");
		}
	}

	public int AlbumChoosed
	{
		get
		{
			return _AlbumChoosed;
		}
		set
		{
			_AlbumChoosed = value;
			OnPropertyChanged("AlbumChoosed");
		}
	}

	public int VideosChoosedInSelectAlbum
	{
		get
		{
			return _VideosChoosedInSelectAlbum;
		}
		set
		{
			_VideosChoosedInSelectAlbum = value;
			OnPropertyChanged("VideosChoosedInSelectAlbum");
		}
	}

	public VideoMgtViewModel()
	{
		OnDeleteEvent = null;
		MainView = VideoViewType.Video;
		AlbumView = VideoViewType.Album;
		VideoList = new ObservableCollection<VideoInfoViewModel>();
		AlbumList = new ObservableCollection<VideoAlbumViewModel>();
		VideoManager = new DeviceVideoManager();
		_tempVideoList = new List<VideoInfoViewModel>();
		ImportVideo = new ReplayCommand(ImportVideoCommandHandler);
		ExportVideo = new ReplayCommand(ExportVideoCommandHandler);
		DeleteVideo = new ReplayCommand(DeleteVideoCommandHandler);
		Refresh = new ReplayCommand(RefreshCommandHandler);
		BackToAlbum = new ReplayCommand(BackToAlbumCommandHandler);
		SearchCommand = new ReplayCommand(SearchCommandHandler);
	}

	public override void LoadData()
	{
		base.LoadData();
		ResetEx();
		RefreshCommandHandler(null);
	}

	public void ResetEx()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			VideoCount = 0;
			AlbumCount = 0;
			VideoList.Clear();
			AlbumList.Clear();
			_ = LeftNavigationViewModel.SingleInstance.Items.FirstOrDefault((LeftNavigationItemViewModel n) => n.Key.ToString() == "lmsa-plugin-Device-video").View;
		});
	}

	public void UpdateVideoList()
	{
		_tempVideoList.Clear();
		VideoList.Clear();
		List<VideoInfoViewModel> list = new List<VideoInfoViewModel>();
		foreach (VideoAlbumViewModel album in AlbumList)
		{
			list.AddRange(album.VideoInfoList);
		}
		foreach (VideoInfoViewModel item in list.OrderByDescending((VideoInfoViewModel n) => n.ModifiedDate).ToList())
		{
			VideoList.Add(item);
		}
	}

	public void UpdateAlbumVideoList()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (SelectedAlbum != null)
			{
				_tempVideoList.Clear();
				VideoList.Clear();
				string selectedAlbumPath = SelectedAlbum.AlbumPath;
				VideoAlbumViewModel videoAlbumViewModel = AlbumList.Where((VideoAlbumViewModel a) => a.AlbumPath == selectedAlbumPath).SingleOrDefault();
				if (videoAlbumViewModel != null && videoAlbumViewModel.VideoInfoList != null)
				{
					foreach (VideoInfoViewModel item in videoAlbumViewModel.VideoInfoList.OrderByDescending((VideoInfoViewModel n) => n.ModifiedDate).ToList())
					{
						VideoList.Add(item);
					}
				}
			}
		});
	}

	private void ImportVideoCommandHandler(object prameter)
	{
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Title = "K0496";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			openFileDialog.Title = HostProxy.LanguageService.Translate("K0496");
		}
		openFileDialog.Filter = "Videos|*.mp4;*.rm;*.rmvb;*.avi";
		openFileDialog.Multiselect = true;
		openFileDialog.FileName = string.Empty;
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() != true)
		{
			return;
		}
		string path = openFileDialog.FileName.Substring(0, openFileDialog.FileName.LastIndexOf("\\"));
		List<string> fileNames = new List<string>();
		foreach (string item in openFileDialog.SafeFileNames.ToList())
		{
			fileNames.Add(Path.Combine(path, item));
		}
		if (HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { Property: not null } tcpAndroidDevice)
		{
			string appSaveDir = tcpAndroidDevice.Property.InternalStoragePath + "/LMSA/Video/";
			new ImportAndExportWrapper().ImportFile(BusinessType.VIDEO_IMPORT, 20, ResourcesHelper.StringResources.SingleInstance.VIDEO_IMPORT_MESSAGE, "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", BackupRestoreStaticResources.SingleInstance.VIDEO, () => fileNames, (string sourcePath) => appSaveDir + Path.GetFileName(sourcePath));
			RefreshCommandHandler(null);
		}
	}

	private void GetVideoList(List<ServerAlbumInfo> albums, int videoCount, int albumCount)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			VideoCount = 0;
			AlbumCount = 0;
			VideoList.Clear();
			AlbumList.Clear();
		});
		List<Video> videos = VideoManager.GetVideos(albums);
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			VideoCount = videoCount;
			AlbumCount = albumCount;
			foreach (ServerAlbumInfo album in albums)
			{
				VideoAlbumViewModel item = new VideoAlbumViewModel
				{
					RawAlbumInfo = album,
					AlbumImage = _defaultImage
				};
				AlbumList.Add(item);
			}
			foreach (Video video in videos.OrderByDescending((Video n) => n.ModifiyDate).ToList())
			{
				VideoInfoViewModel videoInfoViewModel = new VideoInfoViewModel();
				videoInfoViewModel.RawVideoInfo = new VideoInfo();
				videoInfoViewModel.RawVideoInfo.SetData(video, albums.Where((ServerAlbumInfo a) => a.AlbumPath == video.Album).SingleOrDefault());
				AlbumList.Where((VideoAlbumViewModel a) => a.AlbumPath == video.Album).SingleOrDefault()?.VideoInfoList.Add(videoInfoViewModel);
				videoInfoViewModel.VideoImage = _defaultImage;
				VideoList.Add(videoInfoViewModel);
			}
		});
	}

	private void RefreshVideoList()
	{
		AsyncDataLoader.BeginLoading(delegate
		{
			try
			{
				if (!VideoManager.GetVideoCount(out var videoCnt, out var albumCnt))
				{
					return new Tuple<bool, string, string>(item1: false, "K0658", "K0659");
				}
				List<ServerAlbumInfo> albums = VideoManager.GetAlbums();
				GetVideoList(albums, videoCnt, albumCnt);
			}
			catch
			{
				return new Tuple<bool, string, string>(item1: false, "K0660", "K0661");
			}
			return (Tuple<bool, string, string>)null;
		}, ViewContext.SingleInstance.MainViewModel);
	}

	private void ExportVideoCommandHandler(object prameter)
	{
		List<Video> selectedVideos = GetSelectedVideos();
		if (selectedVideos == null || selectedVideos.Count == 0)
		{
			return;
		}
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() != DialogResult.Cancel)
		{
			string saveDir = folderBrowserDialog.SelectedPath.Trim();
			ImportAndExportWrapper importAndExportWrapper = new ImportAndExportWrapper();
			List<string> idList = selectedVideos.Select((Video m) => m.Id.ToString()).ToList();
			importAndExportWrapper.ExportFile(BusinessType.VIDEO_EXPORT, 20, idList, ResourcesHelper.StringResources.SingleInstance.VIDEO_EXPORT_MESSAGE, "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", ResourcesHelper.StringResources.SingleInstance.VIDEO_CONTENT, saveDir, null);
		}
	}

	private void DeleteVideoCommandHandler(object prameter)
	{
		if (OnDeleteEvent != null)
		{
			OnDeleteEvent();
		}
		List<Video> vedios = GetSelectedVideos();
		if (vedios.Count < 1 || !MessageBoxHelper.DeleteConfirmMessagebox(ResourcesHelper.StringResources.SingleInstance.CONTACT_DELETE_TITLE, ResourcesHelper.StringResources.SingleInstance.VIDEO_DELETE_CONTENT))
		{
			return;
		}
		BusinessData businessData = new BusinessData(BusinessType.VIDEO_DELETE, Context.CurrentDevice);
		List<string> idArr = vedios.Select((Video p) => $"{p.Id}").ToList();
		AsyncDataLoader.Loading(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			Dictionary<string, int> result = new Dictionary<string, int>();
			stopwatch.Start();
			bool num = VideoManager.DeleteDevFilesWithConfirm("deleteVideosById", idArr, ref result);
			stopwatch.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.VIDEO_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, BusinessStatus.SUCCESS, result));
			if (num)
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					foreach (Video item in vedios)
					{
						VideoInfoViewModel video = VideoList.Where((VideoInfoViewModel v) => v.RawVideoInfo.RawVideo.Id == item.Id).FirstOrDefault();
						if (video != null)
						{
							VideoList.Remove(video);
							int videoCount = VideoCount;
							VideoCount = videoCount - 1;
							VideoAlbumViewModel videoAlbumViewModel = AlbumList.Where((VideoAlbumViewModel a) => a.AlbumName == video.RawVideoInfo.AlbumName).FirstOrDefault();
							if (videoAlbumViewModel != null)
							{
								videoAlbumViewModel.VideoInfoList.Remove(video);
								videoAlbumViewModel.FileCount--;
							}
						}
					}
					foreach (VideoAlbumViewModel item2 in AlbumList.Where((VideoAlbumViewModel a) => a.VideoInfoList.Count == 0).ToList())
					{
						AlbumList.Remove(item2);
						int videoCount = AlbumCount;
						AlbumCount = videoCount - 1;
					}
					IsAllSelected = false;
					OnDeleteEvent?.Invoke();
				});
			}
		});
	}

	private void RefreshCommandHandler(object prameter)
	{
		if (prameter != null)
		{
			(prameter as System.Windows.Controls.TextBox).Text = string.Empty;
		}
		AsyncDataLoader.BeginLoading(delegate
		{
			try
			{
				if (!VideoManager.GetVideoCount(out var videoCnt, out var albumCnt))
				{
					return new Tuple<bool, string, string>(item1: false, "K0668", "Maybe your storage space is insufficient, please check");
				}
				List<ServerAlbumInfo> albums = VideoManager.GetAlbums();
				GetVideoList(albums, videoCnt, albumCnt);
				VideosTotalSize = GlobalFun.ConvertLong2String(VideoList.Sum((VideoInfoViewModel n) => n.Size), "F2");
				VideosChoosed = 0;
				AlbumChoosed = 0;
				VideosChoosedInSelectAlbum = 0;
				Task.Factory.StartNew(delegate
				{
					UpdateVideoImages(albums);
				});
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					if (MainView == VideoViewType.Album)
					{
						bool flag = false;
						if (AlbumList != null && SelectedAlbum != null && AlbumView == VideoViewType.Video)
						{
							flag = AlbumList.Count((VideoAlbumViewModel n) => n.AlbumName == SelectedAlbum.AlbumName) != 0;
						}
						if (!flag)
						{
							AlbumViewCallback?.Invoke(VideoViewType.Album);
						}
						else
						{
							UpdateAlbumVideoList();
						}
					}
					IsAllSelected = false;
				});
			}
			catch
			{
				return new Tuple<bool, string, string>(item1: false, "K0668", "Maybe your storage space is insufficient, please check");
			}
			return (Tuple<bool, string, string>)null;
		}, ViewContext.SingleInstance.MainViewModel);
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
			VideoManager.ExportVideoThumbnails(albums, videoCacheDir, delegate(ServerAlbumInfo album, VideoThumbnails video, bool res)
			{
				if (res)
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						VideoAlbumViewModel videoAlbumViewModel = AlbumList.Where((VideoAlbumViewModel a) => a.AlbumName == album.AlbumName).FirstOrDefault();
						if (videoAlbumViewModel != null)
						{
							VideoInfoViewModel videoInfoViewModel = videoAlbumViewModel.VideoInfoList.Where((VideoInfoViewModel v) => v.RawVideoInfo.RawVideo.Name == video.Name).FirstOrDefault();
							if (videoInfoViewModel != null)
							{
								videoInfoViewModel.RawVideoInfo.LocalVideoImagePath = video.LocalFilePath;
								videoInfoViewModel.VideoImage = ImageHandleHelper.LoadBitmap(video.LocalFilePath);
								videoAlbumViewModel.AlbumImage = videoAlbumViewModel.VideoInfoList.First()?.VideoImage;
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

	private void BackToAlbumCommandHandler(object prameter)
	{
		SelectedAlbum = null;
	}

	private void SearchCommandHandler(object prameter)
	{
		if (prameter == null)
		{
			return;
		}
		string searchText = prameter as string;
		if (searchText == null)
		{
			return;
		}
		if (searchText == "")
		{
			if (MainView == VideoViewType.Video)
			{
				UpdateVideoList();
			}
			else
			{
				UpdateAlbumVideoList();
			}
			return;
		}
		searchText = searchText.ToLower();
		foreach (VideoInfoViewModel video in _tempVideoList.Where((VideoInfoViewModel v) => v.VideoName.ToLower().Contains(searchText)).ToList())
		{
			if (!VideoList.Any((VideoInfoViewModel v) => v.RawVideoInfo.Owner.AlbumName == video.RawVideoInfo.Owner.AlbumName && v.VideoName == video.VideoName))
			{
				VideoList.Add(video);
			}
			_tempVideoList.Remove(video);
		}
		foreach (VideoInfoViewModel video2 in VideoList.Where((VideoInfoViewModel v) => !v.VideoName.ToLower().Contains(searchText)).ToList())
		{
			if (!_tempVideoList.Any((VideoInfoViewModel v) => v.RawVideoInfo.Owner.AlbumName == video2.RawVideoInfo.Owner.AlbumName && v.VideoName == video2.VideoName))
			{
				_tempVideoList.Add(video2);
			}
			VideoList.Remove(video2);
		}
	}

	private List<Video> GetSelectedVideos()
	{
		List<Video> list = new List<Video>();
		List<VideoInfoViewModel> list2 = new List<VideoInfoViewModel>();
		if (MainView == VideoViewType.Album && AlbumView == VideoViewType.Album)
		{
			VideoAlbumViewModel videoAlbumViewModel = AlbumList.Where((VideoAlbumViewModel a) => a.IsSelected).SingleOrDefault();
			if (videoAlbumViewModel == null)
			{
				return list;
			}
			list2 = videoAlbumViewModel.VideoInfoList.ToList();
		}
		else
		{
			list2 = VideoList.Where((VideoInfoViewModel v) => v.IsSelected).ToList();
		}
		foreach (VideoInfoViewModel item in list2)
		{
			list.Add(item.RawVideoInfo.RawVideo);
		}
		return list;
	}

	public override void Reset()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			base.Reset();
			if (DataIsLoaded)
			{
				AlbumChoosed = 0;
				VideosChoosed = 0;
				VideoCount = 0;
				VideosTotalSize = null;
				VideoList.Clear();
				AlbumList.Clear();
			}
		});
	}
}
