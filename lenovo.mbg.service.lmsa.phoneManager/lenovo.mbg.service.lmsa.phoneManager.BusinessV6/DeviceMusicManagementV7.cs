using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.common.DataBase;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;
using lenovo.themes.generic;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

public class DeviceMusicManagementV7 : DeviceCommonManagement, IDeviceMusicManagement
{
	protected ConcurrentDictionary<string, List<MusicInfo>> _CacheMusic = new ConcurrentDictionary<string, List<MusicInfo>>();

	protected ConcurrentDictionary<string, MusicInfo> _CacheMusicFile = new ConcurrentDictionary<string, MusicInfo>();

	public CacheMapping<int, string> Mapping { get; private set; }

	public DeviceMusicManagementV7()
	{
		Mapping = new CacheMapping<int, string>(CacheDataType.MUSIC);
	}

	public List<MusicInfo> GetMusicList(string albumID, int currentMaxId, int pageCount)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return null;
		}
		if ("Moto".Equals(tcpAndroidDevice.ConnectedAppType))
		{
			return GetMusicListMoto(albumID, currentMaxId, pageCount);
		}
		return GetMusicListMA(albumID, currentMaxId, pageCount);
	}

	public List<MusicInfo> GetMusicListMA(string albumID, int currentMaxId, int pageCount)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return null;
		}
		try
		{
			using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter == null)
			{
				return null;
			}
			long sequence = HostProxy.Sequence.New();
			List<ServerMusicInfo> receiveData = null;
			if (messageReaderAndWriter.SendAndReceiveSync("getMusicAlbumFilesInfo", "getMusicAlbumFilesInfoResponse", new List<object> { albumID, currentMaxId, pageCount }, sequence, out receiveData) && receiveData != null)
			{
				List<MusicInfo> list = new List<MusicInfo>();
				foreach (ServerMusicInfo item in receiveData)
				{
					MusicInfo musicInfo = new MusicInfo();
					musicInfo.ID = item.ID;
					musicInfo.AlbumID = item.AlbumID;
					musicInfo.Artist = item.RawArtist;
					musicInfo.AlbumPath = item.AlbumName;
					musicInfo.Name = item.VirtualFileName;
					musicInfo.DisplayName = item.DisplayName;
					musicInfo.FilePath = item.RawFilePath;
					musicInfo.Duration = (int)item.DoubleDuration;
					musicInfo.Size = (string.IsNullOrEmpty(item.RawFileSize) ? 0 : long.Parse(item.RawFileSize));
					if (!string.IsNullOrEmpty(item.RawModifiedDate))
					{
						long.TryParse(item.RawModifiedDate, out var result);
						if (item.RawModifiedDate.Length > 10)
						{
							result /= 1000;
						}
						DateTime dModifiedDate = new DateTime(1970, 1, 1).AddSeconds(result);
						musicInfo.ModifiedDate = dModifiedDate.ToString("yyyy-MM-dd");
						musicInfo.DModifiedDate = dModifiedDate;
					}
					musicInfo.Frequency = item.DoubleFrequency;
					musicInfo.RawMusicInfo = item;
					list.Add(musicInfo);
				}
				list = list.OrderBy((MusicInfo m) => m.DModifiedDate).ToList();
				CacheMusic(albumID, list);
				return list;
			}
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(ex.ToString());
		}
		return null;
	}

	public List<MusicInfo> GetMusicListMoto(string albumID, int currentMaxId, int pageCount)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return null;
		}
		try
		{
			using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter == null)
			{
				return null;
			}
			long sequence = HostProxy.Sequence.New();
			List<ServerMusicInfoMoto> receiveData = null;
			if (messageReaderAndWriter.SendAndReceiveSync("getMusicAlbumFilesInfo", "getMusicAlbumFilesInfoResponse", new List<object> { albumID, currentMaxId, pageCount }, sequence, out receiveData) && receiveData != null)
			{
				List<MusicInfo> list = new List<MusicInfo>();
				foreach (ServerMusicInfoMoto item in receiveData)
				{
					MusicInfo musicInfo = new MusicInfo();
					musicInfo.ID = item.ID;
					musicInfo.AlbumID = item.AlbumID;
					musicInfo.Artist = item.RawArtist;
					musicInfo.AlbumPath = item.AlbumName;
					musicInfo.Name = item.VirtualFileName;
					musicInfo.DisplayName = item.DisplayName;
					musicInfo.FilePath = item.RawFilePath;
					musicInfo.Duration = (int)item.DoubleDuration;
					musicInfo.Size = (string.IsNullOrEmpty(item.RawFileSize) ? 0 : long.Parse(item.RawFileSize));
					if (!string.IsNullOrEmpty(item.RawModifiedDate))
					{
						DateTime dModifiedDate = new DateTime(1970, 1, 1).AddSeconds(double.Parse(item.RawModifiedDate));
						musicInfo.ModifiedDate = dModifiedDate.ToString("yyyy-MM-dd");
						musicInfo.DModifiedDate = dModifiedDate;
					}
					musicInfo.Frequency = item.DoubleFrequency;
					musicInfo.RawMusicInfo = new ServerMusicInfo
					{
						AlbumID = item.AlbumID.ToString(),
						AlbumName = item.AlbumName,
						ID = item.ID,
						AlbumInfo = item.AlbumInfo,
						DisplayName = item.DisplayName,
						RawArtist = item.RawArtist,
						RawDuration = item.RawDuration,
						RawFileName = item.RawFileName,
						RawFilePath = item.RawFilePath,
						RawFileSize = item.RawFileSize,
						RawFrequency = item.RawFrequency,
						RawModifiedDate = item.RawModifiedDate
					};
					list.Add(musicInfo);
				}
				list = list.OrderBy((MusicInfo m) => m.DModifiedDate).ToList();
				CacheMusic(albumID.ToString(), list);
				return list;
			}
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(ex.ToString());
		}
		return null;
	}

	public List<MusicAlbumViewModelV7> GenericAlbumInfo(List<MusicInfo> musics)
	{
		if (musics == null)
		{
			return null;
		}
		List<MusicAlbumViewModelV7> list = (from n in musics
			group n by n.AlbumPath into n
			select new MusicAlbumViewModelV7
			{
				AlbumPath1 = n.FirstOrDefault().AlbumPath,
				AlbumID = n.Key,
				AlbumName = Path.GetFileName(n.Key),
				FileCount = n.Count(),
				Musics = n.ToList(),
				CachedAllMusics = new ObservableCollection<MusicInfo>(n)
			}).ToList();
		List<MusicInfo> value = new List<MusicInfo>();
		_CacheMusic.TryGetValue("-1", out value);
		_CacheMusic.Clear();
		_CacheMusic.TryAdd("-1", value);
		list.ForEach(delegate(MusicAlbumViewModelV7 n)
		{
			CacheMusic(n.AlbumID, n.Musics);
		});
		return list;
	}

	public List<MusicInfo> SearchMusicList(string keyWords, string albumId)
	{
		List<MusicInfo> list = LoadMusicFormCache(albumId);
		if (string.IsNullOrEmpty(keyWords))
		{
			return list;
		}
		if (list == null || list.Count == 0)
		{
			return null;
		}
		string upperKeywords = keyWords.ToUpper();
		return list.Where((MusicInfo music) => music != null && ((music.DisplayName != null && music.DisplayName.ToUpper().Contains(upperKeywords)) || (music.Artist != null && music.Artist.ToUpper().Contains(upperKeywords)) || (music.RawMusicInfo != null && music.RawMusicInfo.AlbumInfo != null && music.RawMusicInfo.AlbumInfo.AlbumName != null && music.RawMusicInfo.AlbumInfo.AlbumName.Contains(upperKeywords)))).ToList();
	}

	public int SetMusicAsRingtone(MusicInfo model, MusicType type)
	{
		int num = SetRingtone(model, type);
		if (num == 2)
		{
			if (HostProxy.CurrentDispatcher?.Invoke(() => Context.MessageBox.ShowMessage("Warn", "Set ringtone need your permission, Please allow it on the phone and click 'Continue'.", "K0612", "K0208")) != true)
			{
				return 2;
			}
			return SetMusicAsRingtone(model, type);
		}
		return num;
	}

	private int SetRingtone(MusicInfo model, MusicType type)
	{
		if (model == null)
		{
			return 1;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return 1;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return 1;
		}
		long sequence = HostProxy.Sequence.New();
		List<PropItem> receiveData = null;
		List<string> obj = new List<string> { model.RawMusicInfo.ID.ToString() };
		int num = (int)type;
		obj.Add(num.ToString());
		if (messageReaderAndWriter.SendAndReceiveSync("setMusicAsRingtone", "setMusicAsRingtoneResponse", obj, sequence, out receiveData) && receiveData != null)
		{
			PropItem propItem = receiveData.FirstOrDefault((PropItem p) => p.Key == "actionStatus");
			if (propItem.Value == "2")
			{
				return 2;
			}
			if (propItem.Value == "0" || propItem.Value == "true")
			{
				return 0;
			}
			return 1;
		}
		return 1;
	}

	public void ClearCacheMusic()
	{
		if (_CacheMusic == null)
		{
			_CacheMusic = new ConcurrentDictionary<string, List<MusicInfo>>();
		}
		else
		{
			_CacheMusic.Clear();
		}
	}

	public void CacheRemove(List<int> removes)
	{
		if (removes == null || removes.Count <= 0)
		{
			return;
		}
		removes.ForEach(delegate(int n)
		{
			MusicInfo musicInfo = _CacheMusic["-1"].FirstOrDefault((MusicInfo m) => m.ID == n);
			if (musicInfo != null)
			{
				_CacheMusic["-1"].Remove(musicInfo);
			}
		});
	}

	public List<MusicInfo> LoadMusicFormCache(string albumID)
	{
		List<MusicInfo> value = null;
		_CacheMusic?.TryGetValue(albumID, out value);
		return value;
	}

	private void CacheMusic(string albumID, List<MusicInfo> musics)
	{
		if (!_CacheMusic.ContainsKey(albumID))
		{
			_CacheMusic.TryAdd(albumID, musics);
		}
		else
		{
			_CacheMusic[albumID].AddRange(musics);
		}
		if (albumID != "-1")
		{
			List<MusicInfo> list = musics.Except(_CacheMusic["-1"], EqualityComparerHelper<MusicInfo>.CreateComparer((MusicInfo n) => n.ID))?.ToList();
			if (list != null)
			{
				_CacheMusic["-1"].AddRange(list);
			}
		}
		_CacheMusic[albumID] = _CacheMusic[albumID].OrderBy((MusicInfo m) => m.DModifiedDate).ToList();
	}

	public bool DeleteMusicFromList(List<MusicInfo> models, Action<bool> callback)
	{
		if (models == null)
		{
			return false;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		long sequence = HostProxy.Sequence.New();
		List<PropItem> receiveData = null;
		List<string> parameter = models.Select((MusicInfo m) => m.ID.ToString()).ToList();
		if (messageReaderAndWriter.SendAndReceiveSync("deleteMusicFiles", "deleteMusicFilesResponse", parameter, sequence, out receiveData) && receiveData != null)
		{
			callback?.Invoke(receiveData.Exists((PropItem m) => "true".Equals(m.Value)));
		}
		return false;
	}

	public void ImportMusicToDevice(string _importDir)
	{
		List<string> fileNames = new List<string>();
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Title = "K0496";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			openFileDialog.Title = HostProxy.LanguageService.Translate("K0496");
		}
		openFileDialog.Filter = string.Format("{0}|*.mp3;*.wav;*.ogg;*.aac;*.midi;*.amr", "Songs");
		openFileDialog.Multiselect = true;
		openFileDialog.FileName = string.Empty;
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() == true)
		{
			int length = openFileDialog.SafeFileNames.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				fileNames.Add(openFileDialog.FileNames[i]);
			}
		}
		fileNames = DeviceCommonManagementV6.CheckImportFiles(fileNames);
		if (fileNames.Count != 0)
		{
			TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
			if (string.IsNullOrEmpty(_importDir))
			{
				_importDir = Path.Combine(tcpAndroidDevice.Property.InternalStoragePath, "Music");
			}
			string appSaveDir = _importDir;
			if (HostProxy.deviceManager.MasterDevice.ConnectedAppType == "Moto")
			{
				appSaveDir = Path.Combine(tcpAndroidDevice.Property.InternalStoragePath, _importDir, "/");
			}
			new ImportAndExportWrapper().ImportFile(BusinessType.SONG_IMPORT, 19, ResourcesHelper.StringResources.SingleInstance.MUSIC_IMPORT_MESSAGE, "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", BackupRestoreStaticResources.SingleInstance.MUSIC, () => fileNames, (string sourcePath) => Path.Combine(appSaveDir, Path.GetFileName(sourcePath)));
		}
	}

	public void ExportDeviceMusic(List<string> _musicIds)
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() != DialogResult.Cancel)
		{
			string saveDir = folderBrowserDialog.SelectedPath.Trim();
			new ImportAndExportWrapper().ExportFile(BusinessType.SONG_EXPORT, 19, _musicIds, ResourcesHelper.StringResources.SingleInstance.MUSIC_EXPORT_MESSAGE, "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", ResourcesHelper.StringResources.SingleInstance.MUSIC_CONTENT, saveDir, null);
		}
	}

	public void DeleteDeviceMusic(List<string> _musicIds)
	{
		if (_musicIds.Exists((string n) => n == MusicPlayerViewModelV7.SingleInstance.CurrentPlayId.ToString()))
		{
			MusicPlayerViewModelV7.SingleInstance.Stop();
			MusicPlayerViewModelV7.SingleInstance.ResetSongText();
		}
		AsyncDataLoader.Loading(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			Dictionary<string, int> result = new Dictionary<string, int>();
			BusinessData businessData = new BusinessData(BusinessType.SONG_DELETE, Context.CurrentDevice);
			stopwatch.Start();
			new DeviceCommonManagementV6().DeleteDevFilesWithConfirm("deleteAudiosById", _musicIds, ref result);
			stopwatch.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.SONG_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, (result["success"] > 0) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, result));
		});
	}
}
