using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;
using lenovo.themes.generic;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class DevicePicManagementBLL
{
	private DevicePicManagement dpm = new DevicePicManagement();

	private object handler = new object();

	public List<PicServerAlbumInfo> GetPicAlbumsViewModel()
	{
		return dpm.GetAlbums();
	}

	public void LoadAlbumsInfo(List<PicServerAlbumInfo> serverAlbumsInfo)
	{
		if (serverAlbumsInfo == null)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				PicMgtViewModel.SingleInstance.ClearAlbums();
				PicMgtViewModel.SingleInstance.CamareAlbum = null;
				PicMgtViewModel.SingleInstance.FocusedAlbum = null;
			});
			return;
		}
		ObservableCollection<PicAlbumViewModel> curAlbums = PicMgtViewModel.SingleInstance.Albums;
		int newDataVersion = ((curAlbums.Count > 0) ? (curAlbums[0].DataVersion + 1) : 0);
		foreach (PicAlbumViewModel item in curAlbums)
		{
			item.Clear();
		}
		PicAlbumViewModel picAlbumViewModel = null;
		string empty = string.Empty;
		string text = HostProxy.deviceManager.MasterDevice?.Property?.InternalStoragePath;
		string value = string.Empty;
		string value2 = "DCIM/CAMERA";
		string value3 = "/DCIM/CAMERA";
		if (!string.IsNullOrEmpty(text))
		{
			value = text.ToUpper() + "/DCIM/CAMERA";
		}
		foreach (PicServerAlbumInfo album in serverAlbumsInfo)
		{
			picAlbumViewModel = curAlbums.FirstOrDefault((PicAlbumViewModel p) => true == p.ServerAlbumInfo?.Id.Equals(album.Id));
			if (picAlbumViewModel == null)
			{
				PicAlbumViewModel picAlbumViewModel2 = new PicAlbumViewModel(PicMgtViewModel.SingleInstance.AlbumSclectionHandler);
				picAlbumViewModel2.ServerAlbumInfo = album;
				picAlbumViewModel2.AlbumName = album.AlbumName;
				picAlbumViewModel2.FileCount = album.FileCount;
				picAlbumViewModel2.AlbumPath = album.Path;
				picAlbumViewModel2.Id = album.Id;
				picAlbumViewModel2.AlbumInfo = album;
				picAlbumViewModel2.DataVersion = newDataVersion;
				empty = picAlbumViewModel2.AlbumPath?.ToUpper();
				if (!string.IsNullOrEmpty(empty) && ((!string.IsNullOrEmpty(value) && empty.StartsWith(value)) || empty.StartsWith(value2) || empty.StartsWith(value3)))
				{
					if (PicMgtViewModel.SingleInstance.CamareAlbum == null)
					{
						PicMgtViewModel.SingleInstance.CamareAlbum = picAlbumViewModel2;
					}
					if (PicMgtViewModel.SingleInstance.FocusedAlbum == null)
					{
						PicMgtViewModel.SingleInstance.FocusedAlbum = picAlbumViewModel2;
					}
					picAlbumViewModel2.AlbumIconImageSource = LeftNavResources.SingleInstance.GetResource("camera_rollDrawingImage") as ImageSource;
					picAlbumViewModel2.FolderImageSource = LeftNavResources.SingleInstance.GetResource("camera_rollDrawingImage") as ImageSource;
					curAlbums.Insert(0, picAlbumViewModel2);
				}
				else
				{
					picAlbumViewModel2.AlbumIconImageSource = LeftNavResources.SingleInstance.GetResource("other_albumsDrawingImage") as ImageSource;
					curAlbums.Add(picAlbumViewModel2);
				}
			}
			else
			{
				picAlbumViewModel.FileCount = album.FileCount;
				picAlbumViewModel.DataVersion = newDataVersion;
			}
		}
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			foreach (PicAlbumViewModel item2 in curAlbums.Where((PicAlbumViewModel m) => m.DataVersion != newDataVersion).ToList())
			{
				curAlbums.Remove(item2);
			}
		});
	}

	public void ReloadAlbumsThumbnail()
	{
		ObservableCollection<PicAlbumViewModel> albums = PicMgtViewModel.SingleInstance.Albums;
		if (albums == null || albums.Count == 0)
		{
			HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
			{
				PicMgtViewModel.SingleInstance.AlbumToolRefreshBtn.IsEnabled = true;
			});
			return;
		}
		string cacheDir = Configurations.PicCacheDir + "Album";
		foreach (PicAlbumViewModel item in albums)
		{
			item.Clear();
			int fileCount = ((item.FileCount > 4) ? 4 : item.FileCount);
			dpm.ExportThumbnailFromDevice(item.ServerAlbumInfo, fileCount, cacheDir, delegate(PicServerAlbumInfo album, ServerPicInfo file, bool result)
			{
				if (file != null)
				{
					HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
					{
						item.AddThumbnail(Path.Combine(cacheDir, file.VirtualFileName));
					});
				}
			});
		}
		LogHelper.LogInstance.Debug("RefreshAlbumThumbnail completed");
		HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
		{
			PicMgtViewModel.SingleInstance.AlbumToolRefreshBtn.IsEnabled = true;
		});
	}

	public void LoadAlbumPicsThumbnail(string albumPath, List<PicInfoViewModel> pics, Action finiashedCallback)
	{
		LogHelper.LogInstance.Debug("Try to Load pictures in first Album!");
		string storageDir = Path.Combine(Configurations.PicCacheDir, albumPath.Replace("/", "\\").Trim('\\'));
		PicsThumbnailFileLoader.Instance.RecivePicListTask(storageDir, pics, finiashedCallback);
	}

	public void ImportPicToDevice()
	{
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Title = "K0496";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			openFileDialog.Title = HostProxy.LanguageService.Translate("K0496");
		}
		openFileDialog.Filter = string.Format("{0}|*.gif;*.jpeg;*.jpg;*.png;*.webp;", "K0674");
		openFileDialog.Multiselect = true;
		openFileDialog.FileName = string.Empty;
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() != true)
		{
			return;
		}
		List<string> fileNames = openFileDialog.FileNames.ToList();
		if (fileNames.Count() == 0)
		{
			return;
		}
		TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (tcpAndroidDevice != null || tcpAndroidDevice.Property != null)
		{
			string appSaveDir = tcpAndroidDevice.Property.InternalStoragePath + "/LMSA/Pic/";
			new ImportAndExportWrapper().ImportFile(BusinessType.PICTURE_IMPORT, 17, ResourcesHelper.StringResources.SingleInstance.PIC_IMPORT_MESSAGE, "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", BackupRestoreStaticResources.SingleInstance.PIC, () => fileNames, (string sourcePath) => appSaveDir + Path.GetFileName(sourcePath));
		}
	}

	public void ReLoadData()
	{
		PicMgtViewModel.SingleInstance.PicToolRefreshBtn.IsEnabled = false;
		try
		{
			List<PicServerAlbumInfo> albums = GetPicAlbumsViewModel();
			if (albums == null)
			{
				PicMgtViewModel.SingleInstance.PicToolRefreshBtn.IsEnabled = true;
			}
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				LoadAlbumsInfo(albums);
			});
			RefreshAlbumPicList(PicMgtViewModel.SingleInstance.FocusedAlbum, delegate
			{
				try
				{
					ReloadAlbumsThumbnail();
				}
				finally
				{
					PicMgtViewModel.SingleInstance.SetToolBarEnable();
				}
			});
		}
		catch (Exception)
		{
			PicMgtViewModel.SingleInstance.PicToolRefreshBtn.IsEnabled = true;
		}
	}

	internal void ExportDevicePic()
	{
		PicAlbumViewModel focusedAlbum = PicMgtViewModel.SingleInstance.FocusedAlbum;
		if (focusedAlbum == null)
		{
			return;
		}
		ObservableCollection<PicInfoListViewModel> cachedPicList = focusedAlbum.CachedPicList;
		if (!cachedPicList.Any((PicInfoListViewModel p) => p.Pics.Any((PicInfoViewModel img) => img.IsSelected)))
		{
			return;
		}
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
		{
			return;
		}
		string saveDir = folderBrowserDialog.SelectedPath.Trim();
		List<string> list = new List<string>();
		foreach (PicInfoListViewModel item in cachedPicList)
		{
			foreach (PicInfoViewModel pic in item.Pics)
			{
				if (pic.IsSelected)
				{
					list.Add(pic.RawPicInfo.Id);
				}
			}
		}
		if (list != null && list.Count > 0)
		{
			new ImportAndExportWrapper().ExportFile(BusinessType.PICTURE_EXPORT, 17, list, ResourcesHelper.StringResources.SingleInstance.PIC_EXPORT_MESSAGE, "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", ResourcesHelper.StringResources.SingleInstance.PIC_CONTENT, saveDir, null);
		}
	}

	internal bool DeleteDevicePic(List<string> idArr)
	{
		if (idArr == null || idArr.Count == 0)
		{
			return false;
		}
		AsyncDataLoader.Loading(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			Dictionary<string, int> result = new Dictionary<string, int>();
			BusinessData businessData = new BusinessData(BusinessType.PICTURE_DELETE, Context.CurrentDevice);
			stopwatch.Start();
			dpm.DeleteDevFilesWithConfirm("deletePicturesById", idArr, ref result);
			stopwatch.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.PICTURE_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, BusinessStatus.SUCCESS, result));
		});
		return true;
	}

	public void ImportAlbumToDevice()
	{
		ImportPicToDevice();
	}

	public void ExportDeviceAlbum()
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string targetDir = folderBrowserDialog.SelectedPath.Trim();
		ObservableCollection<PicAlbumViewModel> albums = PicMgtViewModel.SingleInstance.Albums;
		if (albums == null || albums.Count == 0)
		{
			return;
		}
		List<PicServerAlbumInfo> serverAlbums = new List<PicServerAlbumInfo>();
		foreach (PicAlbumViewModel item in albums)
		{
			if (item.IsSelected)
			{
				serverAlbums.Add(item.AlbumInfo);
			}
		}
		AsyncDataLoader.BeginLoading(delegate
		{
			try
			{
				if (!dpm.ExportVirtualPicFromDevice(serverAlbums, targetDir, delegate
				{
				}))
				{
					throw new Exception("Export pic failed");
				}
			}
			catch (Exception)
			{
				return new Tuple<bool, string, string>(item1: true, "K0641", "K0648");
			}
			return new Tuple<bool, string, string>(item1: true, "Pictures exported", string.Empty);
		}, ViewContext.SingleInstance.MainViewModel);
	}

	public void RefreshAlbumList(List<PicServerAlbumInfo> albums)
	{
		AsyncDataLoader.BeginLoading(delegate
		{
			if (albums == null)
			{
				albums = GetPicAlbumsViewModel();
			}
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				LoadAlbumsInfo(albums);
			});
			try
			{
				ReloadAlbumsThumbnail();
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Debug("Load pictures error:" + ex.ToString());
				return (Tuple<bool, string, string>)null;
			}
			return (Tuple<bool, string, string>)null;
		}, null);
	}

	public void RefreshAlbumThumbnailList()
	{
		LogHelper.LogInstance.Debug($"RefreshAlbumThumbnailList start");
		if (PicMgtViewModel.SingleInstance.AlbumThumbnailIsLoad)
		{
			return;
		}
		AsyncDataLoader.BeginLoading(null, null, delegate(object data)
		{
			((dynamic)data).AlbumLoadResult = true;
			PicMgtViewModel.SingleInstance.AlbumThumbnailIsLoad = true;
			try
			{
				ReloadAlbumsThumbnail();
			}
			catch (Exception ex)
			{
				((dynamic)data).AlbumLoadResult = false;
				LogHelper.LogInstance.Debug("Refresh album thumbnail error:" + ex.ToString());
			}
			return (Tuple<bool, Tuple<bool, string, string>>)null;
		});
	}

	public void RefreshAlbumPicList(PicAlbumViewModel focusedAlbum, Action<object, Exception> callback)
	{
		if (focusedAlbum == null || focusedAlbum.ServerAlbumInfo == null)
		{
			callback(null, null);
			return;
		}
		if (focusedAlbum.CachedPicList.Sum((PicInfoListViewModel p) => p.Pics.Count) != focusedAlbum.CachedAllPics.Count)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				focusedAlbum.CachedPicList.Clear();
				focusedAlbum.CachedAllPics.Clear();
			});
		}
		string id = focusedAlbum.ServerAlbumInfo.Id;
		ObservableCollection<PicInfoListViewModel> cachedPicList = focusedAlbum.CachedPicList;
		ObservableCollection<PicInfoViewModel> cachedAllPicList = focusedAlbum.CachedAllPics;
		AsyncDataLoader.BeginLoading(ViewContext.SingleInstance.MainViewModel, callback, delegate
		{
			try
			{
				List<ServerPicGroupInfo> serverGroups = dpm.GetPicGroupList(id);
				List<PicInfoListViewModel> delArr = cachedPicList.Where((PicInfoListViewModel p) => serverGroups.FirstOrDefault((ServerPicGroupInfo iter) => iter.GroupKey == p.GroupKey) == null).ToList();
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					delArr.ForEach(delegate(PicInfoListViewModel p)
					{
						cachedPicList.Remove(p);
						ObservableCollection<PicInfoViewModel> pics = p.Pics;
						if (pics != null && pics.Count > 0)
						{
							foreach (PicInfoViewModel pic in p.Pics)
							{
								cachedAllPicList.Remove(pic);
							}
						}
					});
				});
				int fileCount = serverGroups.Sum((ServerPicGroupInfo m) => m.Count);
				focusedAlbum.ServerAlbumInfo.FileCount = fileCount;
				focusedAlbum.FileCount = fileCount;
				int num = ((cachedPicList.Count > 0) ? (cachedPicList[0].DataVersion + 1) : 0);
				ServerPicGroupInfo serverGroup = null;
				for (int num2 = serverGroups.Count - 1; num2 >= 0; num2--)
				{
					serverGroup = serverGroups[num2];
					PicInfoListViewModel targetGroup = null;
					if (num > 0)
					{
						targetGroup = cachedPicList.FirstOrDefault((PicInfoListViewModel p) => p.GroupKey == serverGroup.GroupKey);
					}
					if (targetGroup == null)
					{
						targetGroup = new PicInfoListViewModel(PicMgtViewModel.SingleInstance.PicSelectionHandler)
						{
							GroupKey = serverGroup.GroupKey
						};
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							cachedPicList.Insert(0, targetGroup);
						});
					}
					targetGroup.DataVersion = num;
					int addCount = serverGroup.Count - (int)targetGroup.PicCount;
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						for (int i = 0; i < addCount; i++)
						{
							PicInfoViewModel item = new PicInfoViewModel(null, targetGroup.PicSclectionHandler);
							targetGroup.Pics.Insert(0, item);
							cachedAllPicList.Insert(0, item);
						}
					});
				}
				LoadAlbumPicsId(focusedAlbum);
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error($"Load {focusedAlbum.AlbumName} pics id throw exception:{ex.ToString()}");
			}
			LogHelper.LogInstance.Debug("====>>Load default gray pic list finished!");
			return new Tuple<bool, Tuple<bool, string, string>>(item1: false, new Tuple<bool, string, string>(item1: false, string.Empty, string.Empty));
		}, delegate
		{
			LogHelper.LogInstance.Debug("====>>Load Thumbnails in Album!");
			PicInfoListViewModel picInfoListViewModel = focusedAlbum.CachedPicList.FirstOrDefault();
			if (picInfoListViewModel != null)
			{
				AutoResetEvent done = new AutoResetEvent(initialState: false);
				try
				{
					List<PicInfoViewModel> pics2 = focusedAlbum.CachedAllPics.Where((PicInfoViewModel m) => m.RawPicInfo != null && !m.IsImageLoaded).Take(picInfoListViewModel.NumberOfVisibleAreaImages).ToList();
					LoadAlbumPicsThumbnail(id, pics2, delegate
					{
						if (!done.SafeWaitHandle.IsClosed)
						{
							done.Set();
						}
					});
					done.WaitOne(20000);
				}
				finally
				{
					if (done != null)
					{
						((IDisposable)done).Dispose();
					}
				}
			}
			PicMgtViewModel.SingleInstance.PicListIsLoad = true;
			return (Tuple<bool, Tuple<bool, string, string>>)null;
		});
	}

	public void LoadAlbumPicsId(PicAlbumViewModel focusedAlbum)
	{
		LogHelper.LogInstance.Debug($"LoadAlbumPicsId thread start");
		if (focusedAlbum == null)
		{
			return;
		}
		ObservableCollection<PicInfoListViewModel> cachedPicList = focusedAlbum.CachedPicList;
		ObservableCollection<PicInfoViewModel> cachedAllPicList = focusedAlbum.CachedAllPics;
		foreach (PicInfoListViewModel group in cachedPicList)
		{
			List<ServerPicInfo> picInfoList = dpm.GetPicInfoList(focusedAlbum.ServerAlbumInfo.Id, group.GroupKey);
			if (picInfoList == null || picInfoList.Count == 0)
			{
				continue;
			}
			int newDataVersion = ((group.Pics.Count > 0) ? (group.Pics[0].DataVersion + 1) : 0);
			List<PicInfoViewModel> source = group.Pics.Where((PicInfoViewModel m) => !string.IsNullOrEmpty(m.RawPicInfo?.Id)).ToList();
			List<PicInfoViewModel> source2 = group.Pics.Where((PicInfoViewModel m) => string.IsNullOrEmpty(m.RawPicInfo?.Id)).ToList();
			foreach (ServerPicInfo idIter in picInfoList)
			{
				PicInfoViewModel temp = source.FirstOrDefault((PicInfoViewModel p) => p.RawPicInfo?.Id == idIter.Id);
				if (temp == null)
				{
					temp = source2.FirstOrDefault((PicInfoViewModel p) => p.RawPicInfo == null);
					if (temp == null)
					{
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							temp = new PicInfoViewModel(null, group.PicSclectionHandler);
							group.Pics.Insert(0, temp);
						});
					}
					temp.RawPicInfo = idIter;
				}
				temp.DataVersion = newDataVersion;
			}
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				for (int num = group.Pics.Count - 1; num >= 0; num--)
				{
					if (group.Pics[num].DataVersion != newDataVersion)
					{
						cachedAllPicList.Remove(group.Pics[num]);
						group.Pics.Remove(group.Pics[num]);
					}
				}
			});
		}
	}

	public bool GetPicDisplayStartIndex(List<PicInfoListViewModel> groups, double extentHeight, double extendWidth, double verticalOffset, double viewPortHeight, double viewPortWidth, double offset, out PicInfoListViewModel hitGroup, out int startIndex, out int columnCount, out int rowCount, out int listIndex, out int pageCount)
	{
		listIndex = 0;
		startIndex = 0;
		columnCount = 0;
		rowCount = 0;
		pageCount = 0;
		hitGroup = null;
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < groups.Count; i++)
		{
			hitGroup = groups[i];
			num = num2;
			num2 += hitGroup.BodyExtendHeight + hitGroup.HeadExtendHeight;
			if (!(num2 + offset > verticalOffset))
			{
				continue;
			}
			int num3 = (int)((verticalOffset - num - hitGroup.HeadExtendHeight) / hitGroup.ItemExtendHeight);
			columnCount = (int)(hitGroup.BodyExtendWidth / hitGroup.ItemExtendWidth);
			pageCount = hitGroup.NumberOfVisibleAreaImages;
			startIndex = num3 * columnCount;
			if (startIndex >= hitGroup.Pics.Count)
			{
				startIndex = 0;
				if (i + 1 < groups.Count)
				{
					hitGroup = groups[i + 1];
					listIndex = i + 1;
				}
			}
			return true;
		}
		return false;
	}
}
