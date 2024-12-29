using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.common.DataBase;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class DeviceMusicManagement : DeviceCommonManagement, IDeviceMusicManagement
{
	protected ConcurrentDictionary<string, List<MusicInfo>> _CacheMusic = new ConcurrentDictionary<string, List<MusicInfo>>();

	protected ConcurrentDictionary<string, MusicInfo> _CacheMusicFile = new ConcurrentDictionary<string, MusicInfo>();

	public CacheMapping<int, string> Mapping { get; private set; }

	public DeviceMusicManagement()
	{
		Mapping = new CacheMapping<int, string>(CacheDataType.MUSIC);
	}

	public List<int> GetMusicIdList()
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
			List<int> receiveData = null;
			if (messageReaderAndWriter.SendAndReceiveSync("getMusicIdList", "getMusicIdListResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
			{
				return receiveData;
			}
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(ex.ToString());
		}
		return null;
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
					musicInfo.Album = item.AlbumName;
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
					musicInfo.Album = item.AlbumName;
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

	public List<MusicInfo> GetMusicListByName(string name)
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
			if (messageReaderAndWriter.SendAndReceiveSync("getMusicListByName", "getMusicListByNameResponse", new List<object> { name }, sequence, out receiveData) && receiveData != null)
			{
				List<MusicInfo> list = new List<MusicInfo>();
				foreach (ServerMusicInfo item in receiveData)
				{
					MusicInfo musicInfo = new MusicInfo();
					musicInfo.ID = item.ID;
					musicInfo.AlbumID = item.AlbumID;
					musicInfo.Artist = item.RawArtist;
					musicInfo.Album = item.AlbumName;
					musicInfo.Name = item.VirtualFileName;
					musicInfo.DisplayName = item.DisplayName;
					musicInfo.FilePath = item.RawFilePath;
					musicInfo.Duration = (int)item.DoubleDuration;
					musicInfo.Size = (string.IsNullOrEmpty(item.RawFileSize) ? 0 : long.Parse(item.RawFileSize));
					if (!string.IsNullOrEmpty(item.RawModifiedDate))
					{
						musicInfo.ModifiedDate = new DateTime(1970, 1, 1).AddSeconds(double.Parse(item.RawModifiedDate)).ToString("yyyy-MM-dd");
					}
					musicInfo.Frequency = item.DoubleFrequency;
					musicInfo.RawMusicInfo = item;
					list.Add(musicInfo);
				}
				return list;
			}
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(ex.ToString());
		}
		return null;
	}

	public List<MusicInfo> IncrementRefresh(string albumId, out List<int> removes)
	{
		List<int> list = GetMusicIdList();
		removes = new List<int>();
		if (list == null)
		{
			list = new List<int>();
		}
		if (!_CacheMusic.ContainsKey(albumId))
		{
			CacheMusic(albumId, new List<MusicInfo>());
		}
		List<int> list2 = _CacheMusic[albumId].Select((MusicInfo n) => n.ID).ToList();
		bool num = list.Except(list2).Count() > 0;
		removes = list2.Except(list).ToList();
		CacheRemove(removes);
		if (num)
		{
			int currentMaxId = -1;
			if (list2.Count > 0)
			{
				currentMaxId = list2.Max();
			}
			return GetMusicList(albumId, currentMaxId, -1);
		}
		return null;
	}

	public List<MusicAlbumViewModel> GenericAlbumInfo(List<MusicInfo> musics)
	{
		if (musics == null)
		{
			return null;
		}
		List<MusicAlbumViewModel> list = (from n in musics
			group n by new { n.AlbumID, n.Album } into n
			select new MusicAlbumViewModel
			{
				AlbumID = n.Key.AlbumID,
				AlbumName = n.Key.Album,
				FileCount = n.Count(),
				Musics = n.ToList()
			}).ToList();
		List<MusicInfo> value = new List<MusicInfo>();
		_CacheMusic.TryGetValue("-1", out value);
		_CacheMusic.Clear();
		_CacheMusic.TryAdd("-1", value);
		list.ForEach(delegate(MusicAlbumViewModel n)
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
			if (HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				LenovoPopupWindow win = new OkCancelWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "Warn", "Set ringtone need your permission, Please allow it on the phone and click 'Continue'.", "K0208", "K0612", null);
				HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
				{
					win.ShowDialog();
				});
				return win.WindowModel.GetViewModel<OKCancelViewModel>().IsOKResult;
			}) != true)
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

	private List<string> PreviewExport(List<MusicInfo> sources)
	{
		List<string> needcopy = null;
		if (!File.Exists(Mapping.SavePath))
		{
			return needcopy;
		}
		JObject jObject = Mapping.Get<JObject>();
		if (jObject != null)
		{
			foreach (JProperty item2 in jObject.Properties())
			{
				string text = item2.Value.ToString();
				int key = int.Parse(item2.Name);
				if (!Mapping.Cache.ContainsKey(key) && File.Exists(text))
				{
					Mapping.Insert(key, text);
				}
			}
			List<int> exists = new List<int>();
			sources.ForEach(delegate(MusicInfo n)
			{
				if (Mapping.Cache.ContainsKey(n.ID))
				{
					string item = Mapping[n.ID];
					needcopy.Add(item);
					exists.Add(n.ID);
				}
			});
			sources = sources.Where((MusicInfo n) => !exists.Contains(n.ID)).ToList();
		}
		return needcopy;
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

	public void ClearCacheMusic(string albumId)
	{
		if (_CacheMusic == null)
		{
			_CacheMusic = new ConcurrentDictionary<string, List<MusicInfo>>();
		}
		else if (_CacheMusic.ContainsKey(albumId))
		{
			List<MusicInfo> value = null;
			if (!_CacheMusic.TryRemove(albumId, out value))
			{
				_CacheMusic[albumId].Clear();
			}
		}
	}

	public void ClearCacheMusicFile()
	{
		if (_CacheMusicFile == null)
		{
			_CacheMusicFile = new ConcurrentDictionary<string, MusicInfo>();
		}
		else
		{
			_CacheMusicFile.Clear();
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

	public void CacheRemove(List<MusicInfo> removes)
	{
		if (removes != null && removes.Count > 0)
		{
			removes.ForEach(delegate(MusicInfo n)
			{
				_CacheMusic["-1"].Remove(n);
			});
		}
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

	public List<int> DeleteMultiMusic(List<string> idArr)
	{
		if (idArr == null)
		{
			return null;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return null;
		}
		List<int> list = new List<int>();
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return null;
		}
		int num = 0;
		List<string> list2 = null;
		long sequence = HostProxy.Sequence.New();
		while (true)
		{
			list2 = idArr.Skip(num * 20).Take(20).ToList();
			if (list2.Count == 0)
			{
				break;
			}
			MessageEx<int> messageEx = messageReaderAndWriter.SendAndReceive("deleteVideosById", list2, sequence);
			if (messageEx == null || messageEx.Action != "deleteVideosByIdResponse")
			{
				for (int i = 0; i < list2.Count; i++)
				{
					list.Add(1);
				}
			}
			else
			{
				list.AddRange(messageEx.Data);
			}
		}
		return list;
	}
}
