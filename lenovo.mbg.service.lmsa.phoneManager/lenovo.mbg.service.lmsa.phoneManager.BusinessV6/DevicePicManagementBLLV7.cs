using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;
using lenovo.themes.generic;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

public class DevicePicManagementBLLV7
{
	private DevicePicManagementV7 dpm = new DevicePicManagementV7();

	private DeviceCommonManagementV6 m_CommonManage = new DeviceCommonManagementV6();

	private object handler = new object();

	private PicMgtViewModelV7 GetCurrentViewModel => Context.FindViewModel<PicMgtViewModelV7>(typeof(PICMgtViewV7));

	public bool CheckInternalStorageFreeSize(List<string> _pathList)
	{
		return m_CommonManage.CheckInternalStorageFreeSize(_pathList);
	}

	public List<PicServerAlbumInfo> GetPicAlbumsViewModel()
	{
		return dpm.GetAlbums();
	}

	public void LoadAlbumsInfo(List<PicServerAlbumInfo> serverAlbumsInfo)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			GetCurrentViewModel.FocusedAlbum?.CachedAllPics.Clear();
			GetCurrentViewModel.FocusedAlbum = null;
			GetCurrentViewModel.ClearAlbums();
		});
		if (serverAlbumsInfo == null)
		{
			return;
		}
		List<PicAlbumViewModelV7> curAlbums = GetCurrentViewModel.AlbumsOriginal;
		int num = 0;
		int newDataVersion = ((curAlbums.Count > 0) ? (curAlbums[0].DataVersion + 1) : 0);
		PicAlbumViewModelV7 picAlbumViewModelV = null;
		string empty = string.Empty;
		string text = HostProxy.deviceManager.MasterDevice?.Property?.InternalStoragePath;
		_ = string.Empty;
		if (!string.IsNullOrEmpty(text))
		{
			_ = text.ToUpper() + "/DCIM/CAMERA";
		}
		foreach (PicServerAlbumInfo album in serverAlbumsInfo)
		{
			picAlbumViewModelV = curAlbums.FirstOrDefault((PicAlbumViewModelV7 p) => true == p.ServerAlbumInfo?.Id.Equals(album.Id));
			if (picAlbumViewModelV == null)
			{
				PicAlbumViewModelV7 picAlbumViewModelV2 = new PicAlbumViewModelV7
				{
					ServerAlbumInfo = album,
					AlbumName = album.AlbumName,
					FileCount = album.FileCount,
					AlbumPath = album.Path,
					Id = album.Id,
					AlbumInfo = album,
					DataVersion = newDataVersion
				};
				if (picAlbumViewModelV2.IsInternalPath != GetCurrentViewModel.IsSelectedInternal)
				{
					continue;
				}
				empty = picAlbumViewModelV2.AlbumPath?.ToUpper();
				if (!string.IsNullOrEmpty(empty) && empty.Contains("/DCIM/CAMERA"))
				{
					curAlbums.Insert(0, picAlbumViewModelV2);
				}
				else
				{
					curAlbums.Add(picAlbumViewModelV2);
				}
			}
			else
			{
				picAlbumViewModelV.FileCount = album.FileCount;
				picAlbumViewModelV.DataVersion = newDataVersion;
				if (picAlbumViewModelV.IsInternalPath != GetCurrentViewModel.IsSelectedInternal)
				{
					continue;
				}
			}
			num += album.FileCount;
		}
		GetCurrentViewModel.AllAlbumPicCount = num;
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			foreach (PicAlbumViewModelV7 item in curAlbums.Where((PicAlbumViewModelV7 m) => m.DataVersion != newDataVersion).ToList())
			{
				curAlbums.Remove(item);
			}
		});
		if (curAlbums.Count > 0)
		{
			IEnumerable<PicAlbumViewModelV7> enumerable = curAlbums.Where((PicAlbumViewModelV7 m) => m.IsInternalPath == GetCurrentViewModel.IsSelectedInternal);
			if (enumerable.FirstOrDefault() != null)
			{
				enumerable.FirstOrDefault().IsSelected = true;
				enumerable.FirstOrDefault().SingleClickCommand.Execute(false);
			}
			GetCurrentViewModel.Albums = new ObservableCollection<PicAlbumViewModelV7>(enumerable);
			GetCurrentViewModel.AlbumCount = enumerable?.Count() ?? 0;
		}
		else
		{
			GetCurrentViewModel.PicToolRefreshBtn.IsEnabled = true;
			GetCurrentViewModel.Albums = new ObservableCollection<PicAlbumViewModelV7>();
			GetCurrentViewModel.AlbumCount = 0;
		}
	}

	public void LoadAlbumPicsThumbnail(string albumPath, List<PicInfoViewModelV7> pics, Action finiashedCallback)
	{
		LogHelper.LogInstance.Debug("Try to Load pictures in first Album!");
		string storageDir = Path.Combine(Configurations.PicCacheDir, albumPath.Replace("/", "\\").Trim('\\'));
		PicsThumbnailFileLoaderV7.Instance.RecivePicListTask(storageDir, pics, finiashedCallback);
	}

	public void ImportPicToDevice(string _importDir)
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
		fileNames = DeviceCommonManagementV6.CheckImportFiles(fileNames);
		if (fileNames.Count() == 0)
		{
			return;
		}
		TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (tcpAndroidDevice == null && tcpAndroidDevice.Property == null)
		{
			return;
		}
		if (CheckInternalStorageFreeSize(fileNames))
		{
			Context.MessageBox.ShowMessage("K0366", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}
		if (string.IsNullOrEmpty(_importDir))
		{
			_importDir = Path.Combine(tcpAndroidDevice.Property.InternalStoragePath, "DCIM/Camera");
		}
		string appSaveDir = _importDir;
		if (HostProxy.deviceManager.MasterDevice.ConnectedAppType == "Moto")
		{
			appSaveDir = Path.Combine(tcpAndroidDevice.Property.InternalStoragePath, _importDir, "/");
		}
		new ImportAndExportWrapper().ImportFile(BusinessType.PICTURE_IMPORT, 17, ResourcesHelper.StringResources.SingleInstance.PIC_IMPORT_MESSAGE, "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", BackupRestoreStaticResources.SingleInstance.PIC, () => fileNames, (string sourcePath) => Path.Combine(appSaveDir, Path.GetFileName(sourcePath)));
	}

	public void ReLoadData()
	{
		GetCurrentViewModel.PicToolRefreshBtn.IsEnabled = false;
		try
		{
			List<PicServerAlbumInfo> albums = GetPicAlbumsViewModel();
			if (albums == null)
			{
				GetCurrentViewModel.PicToolRefreshBtn.IsEnabled = true;
			}
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				LoadAlbumsInfo(albums);
			});
		}
		catch (Exception)
		{
			GetCurrentViewModel.PicToolRefreshBtn.IsEnabled = true;
		}
	}

	internal void ExportDevicePic()
	{
		List<string> list = new List<string>();
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		foreach (PicAlbumViewModelV7 album in GetCurrentViewModel.Albums)
		{
			foreach (PicInfoViewModelV7 cachedAllPic in album.CachedAllPics)
			{
				if (cachedAllPic.IsSelected)
				{
					if (cachedAllPic.RawPicInfo.RawFileSize > 4294967296L)
					{
						dictionary.Add(cachedAllPic.RawPicInfo.RawFilePath, cachedAllPic.RawPicInfo.RawFileSize);
					}
					else
					{
						list.Add(cachedAllPic.RawPicInfo.Id);
					}
				}
			}
		}
		DeviceCommonManagementV6.CheckExportFiles(dictionary);
		if (list.Count != 0)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if (folderBrowserDialog.ShowDialog() != DialogResult.Cancel)
			{
				string saveDir = folderBrowserDialog.SelectedPath.Trim();
				new ImportAndExportWrapper().ExportFile(BusinessType.PICTURE_EXPORT, 17, list, ResourcesHelper.StringResources.SingleInstance.PIC_EXPORT_MESSAGE, "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", ResourcesHelper.StringResources.SingleInstance.PIC_CONTENT, saveDir, null);
			}
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
			HostProxy.BehaviorService.Collect(BusinessType.PICTURE_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, (result["success"] > 0) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, result));
		});
		return true;
	}

	public void RefreshAlbumPicList(PicAlbumViewModelV7 focusedAlbum, Action<object, Exception> callback)
	{
		if (focusedAlbum == null || focusedAlbum.ServerAlbumInfo == null)
		{
			callback(null, null);
			return;
		}
		if (focusedAlbum.CachedPicList.Sum((PicInfoListViewModelV7 p) => p.Pics.Count) != focusedAlbum.CachedAllPics.Count)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				focusedAlbum.CachedPicList.Clear();
				focusedAlbum.CachedAllPics.Clear();
			});
		}
		string id = focusedAlbum.ServerAlbumInfo.Id;
		ObservableCollection<PicInfoListViewModelV7> cachedPicList = focusedAlbum.CachedPicList;
		ObservableCollection<PicInfoViewModelV7> cachedAllPicList = focusedAlbum.CachedAllPics;
		AsyncDataLoader.BeginLoading(ViewContext.SingleInstance.MainViewModel, callback, delegate
		{
			try
			{
				List<ServerPicGroupInfo> serverGroups = dpm.GetPicGroupList(id);
				List<PicInfoListViewModelV7> delArr = cachedPicList.Where((PicInfoListViewModelV7 p) => serverGroups.FirstOrDefault((ServerPicGroupInfo iter) => iter.GroupKey == p.GroupKey) == null).ToList();
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					delArr.ForEach(delegate(PicInfoListViewModelV7 p)
					{
						cachedPicList.Remove(p);
						ObservableCollection<PicInfoViewModelV7> pics = p.Pics;
						if (pics != null && pics.Count > 0)
						{
							foreach (PicInfoViewModelV7 pic in p.Pics)
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
					PicInfoListViewModelV7 targetGroup = null;
					if (num > 0)
					{
						targetGroup = cachedPicList.FirstOrDefault((PicInfoListViewModelV7 p) => p.GroupKey == serverGroup.GroupKey);
					}
					if (targetGroup == null)
					{
						targetGroup = new PicInfoListViewModelV7
						{
							GroupKey = serverGroup.GroupKey
						};
						targetGroup.SetSelectionHandler(GetCurrentViewModel.PicSelectionHandler);
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
							PicInfoViewModelV7 picInfoViewModelV = new PicInfoViewModelV7
							{
								GroupKey = targetGroup.GroupKey
							};
							if (focusedAlbum.IsSelectedAllPic.HasValue)
							{
								picInfoViewModelV.IsNotUserClick = true;
								picInfoViewModelV.IsSelected = focusedAlbum.IsSelectedAllPic.Value;
								picInfoViewModelV.IsNotUserClick = false;
							}
							targetGroup.Pics.Insert(0, picInfoViewModelV);
							cachedAllPicList.Insert(0, picInfoViewModelV);
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
			PicInfoListViewModelV7 picInfoListViewModelV = focusedAlbum.CachedPicList.FirstOrDefault();
			if (picInfoListViewModelV != null)
			{
				AutoResetEvent done = new AutoResetEvent(initialState: false);
				try
				{
					List<PicInfoViewModelV7> pics2 = focusedAlbum.CachedAllPics.Where((PicInfoViewModelV7 m) => m.RawPicInfo != null && !m.IsImageLoaded).Take(picInfoListViewModelV.NumberOfVisibleAreaImages).ToList();
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
			Context.FindViewModel<PicMgtViewModelV7>(typeof(PICMgtViewV7)).PicListIsLoad = true;
			GetCurrentViewModel.PicListIsLoad = true;
			return (Tuple<bool, Tuple<bool, string, string>>)null;
		});
	}

	public void LoadAlbumPicsId(PicAlbumViewModelV7 focusedAlbum)
	{
		LogHelper.LogInstance.Debug($"LoadAlbumPicsId thread start");
		if (focusedAlbum == null)
		{
			return;
		}
		ObservableCollection<PicInfoListViewModelV7> cachedPicList = focusedAlbum.CachedPicList;
		ObservableCollection<PicInfoViewModelV7> cachedAllPicList = focusedAlbum.CachedAllPics;
		foreach (PicInfoListViewModelV7 group in cachedPicList)
		{
			List<ServerPicInfo> picInfoList = dpm.GetPicInfoList(focusedAlbum.ServerAlbumInfo.Id, group.GroupKey);
			if (picInfoList == null || picInfoList.Count == 0)
			{
				continue;
			}
			int newDataVersion = ((group.Pics.Count > 0) ? (group.Pics[0].DataVersion + 1) : 0);
			List<PicInfoViewModelV7> source = group.Pics.Where((PicInfoViewModelV7 m) => !string.IsNullOrEmpty(m.RawPicInfo?.Id)).ToList();
			List<PicInfoViewModelV7> source2 = group.Pics.Where((PicInfoViewModelV7 m) => string.IsNullOrEmpty(m.RawPicInfo?.Id)).ToList();
			foreach (ServerPicInfo idIter in picInfoList)
			{
				PicInfoViewModelV7 temp = source.FirstOrDefault((PicInfoViewModelV7 p) => p.RawPicInfo?.Id == idIter.Id);
				if (temp == null)
				{
					temp = source2.FirstOrDefault((PicInfoViewModelV7 p) => p.RawPicInfo == null);
					if (temp == null)
					{
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							temp = new PicInfoViewModelV7();
							temp.GroupKey = group.GroupKey;
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

	public bool GetPicDisplayStartIndex(List<PicInfoListViewModelV7> groups, double extentHeight, double extendWidth, double verticalOffset, double viewPortHeight, double viewPortWidth, double offset, out PicInfoListViewModelV7 hitGroup, out int startIndex, out int columnCount, out int rowCount, out int listIndex, out int pageCount)
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
