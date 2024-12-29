using System.Collections.Generic;
using System.Diagnostics;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

public class NavigationManagementBLLV6
{
	private DeviceMusicManagement mgt = new DeviceMusicManagement();

	private DevicePicManagement pmgt = new DevicePicManagement();

	private DeviceVideoManager vmgt = new DeviceVideoManager();

	private DeviceContactManager cmgt = new DeviceContactManager();

	private DeviceSmsManagement smgt = new DeviceSmsManagement();

	public static NavigationManagementBLLV6 Instance { get; set; }

	public Dictionary<string, int> ResDictionary { get; set; }

	public NavigationManagementBLLV6()
	{
		Instance = this;
		ResDictionary = new Dictionary<string, int>();
	}

	internal string PrepareToPlayMusic(MusicInfoViewModelV7 model, string dir)
	{
		string filName = string.Empty;
		_ = model.RawMusicInfo;
		ImportAndExportWrapper importAndExportWrapper = new ImportAndExportWrapper();
		Stopwatch stopwatch = new Stopwatch();
		ResourceExecuteResult executeResult = new ResourceExecuteResult();
		BusinessData businessData = new BusinessData(BusinessType.SONG_EXPORT, Context.CurrentDevice);
		stopwatch.Start();
		importAndExportWrapper.ExportFileWithNoProgress(19, new List<string> { model.RawMusicInfo.ID.ToString() }, dir, null, delegate(string id, bool isSuccess, string path)
		{
			filName = path;
			executeResult.Update(isSuccess);
		});
		stopwatch.Stop();
		HostProxy.BehaviorService.Collect(BusinessType.SONG_EXPORT, businessData.Update(stopwatch.ElapsedMilliseconds, (executeResult.Status == 1) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, executeResult.ResultMapEx));
		return filName;
	}
}
