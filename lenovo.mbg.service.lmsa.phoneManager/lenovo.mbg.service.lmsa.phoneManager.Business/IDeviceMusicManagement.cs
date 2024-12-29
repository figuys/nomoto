using System;
using System.Collections.Generic;
using lenovo.mbg.service.lmsa.phoneManager.Model;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public interface IDeviceMusicManagement
{
	List<MusicInfo> GetMusicList(string albumID, int currentMaxId, int pageCount);

	int SetMusicAsRingtone(MusicInfo model, MusicType type);

	List<MusicInfo> SearchMusicList(string keyWords, string albumId);

	bool DeleteMusicFromList(List<MusicInfo> models, Action<bool> callback);
}
