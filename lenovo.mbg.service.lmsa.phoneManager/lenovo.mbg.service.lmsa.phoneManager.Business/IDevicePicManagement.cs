using System;
using System.Collections.Generic;
using System.Threading;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public interface IDevicePicManagement
{
	List<PicServerAlbumInfo> GetAlbums();

	List<ServerPicGroupInfo> GetPicGroupList(string albumPath);

	List<ServerPicInfo> GetPicInfoList(string alblumId, string groupKey);

	bool FillPicPath(ref List<ServerPicInfo> idArr);

	bool ExportThumbnailFromDevice(IAsyncTaskContext context, List<ServerPicInfo> pics, string localStroageDir, Action<ServerPicInfo, bool> callBack, CancellationTokenSource cancelSource);

	bool DeleteAlbum(string ambumPath);

	bool DeletePics(DateTime? date);

	bool DeletePicFromList(List<ServerPicInfo> pics);

	bool ExportThumbnailFromDevice(List<PicServerAlbumInfo> albums, string localStroageDir, Action<PicServerAlbumInfo, ServerPicInfo, bool> callBack);

	bool ExportThumbnailFromDevice(PicServerAlbumInfo album, int fileCount, string localStroageDir, Action<PicServerAlbumInfo, ServerPicInfo, bool> callBack);
}
