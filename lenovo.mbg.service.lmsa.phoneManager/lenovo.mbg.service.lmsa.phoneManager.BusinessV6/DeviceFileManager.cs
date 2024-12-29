using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.common;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.ModelV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

public class DeviceFileManager
{
	public bool GetInternalAndExternamPath(out string internalPath, out string externalPath)
	{
		bool result = false;
		internalPath = string.Empty;
		externalPath = string.Empty;
		if (!(Context.CurrentDevice is TcpAndroidDevice tcpAndroidDevice))
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
		if (messageReaderAndWriter.SendAndReceiveSync("getInternalAndExternalStoragePath", "getInternalAndExternalStoragePathResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
		{
			string text = (from m in receiveData
				where "internalPath".Equals(m.Key)
				select m.Value).FirstOrDefault();
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				if (text.Length > 0 && text[0].Equals('/'))
				{
					internalPath = text + "/Download";
				}
			}
			string text2 = (from m in receiveData
				where "externalPath".Equals(m.Key)
				select m.Value).FirstOrDefault();
			if (!string.IsNullOrEmpty(text2))
			{
				text2 = text2.Trim();
				if (text2.Length > 0 && text2[0].Equals('/'))
				{
					externalPath = text2 + "/Download";
				}
			}
			result = true;
		}
		return result;
	}

	public List<DeviceFileInfo> GetFilesInfo(string path)
	{
		List<DeviceFileInfo> result = new List<DeviceFileInfo>();
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return result;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return result;
		}
		if (messageReaderAndWriter == null)
		{
			return result;
		}
		long sequence = HostProxy.Sequence.New();
		List<DeviceFileInfo> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getSubDirectoriesInfo", "getSubDirectoriesInfoResponse", new List<string> { path }, sequence, out receiveData) && receiveData != null)
		{
			result = receiveData;
		}
		return result;
	}

	public bool Import(string localFileName, string targetFileName, Action<long, long> progressNotify)
	{
		bool ret = false;
		if (localFileName == null)
		{
			return ret;
		}
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice))
		{
			return false;
		}
		ImportAndExportWrapper importAndExportWrapper = new ImportAndExportWrapper();
		Stopwatch stopwatch = new Stopwatch();
		ResourceExecuteResult resourceExecuteResult = new ResourceExecuteResult();
		BusinessData businessData = new BusinessData(BusinessType.FILE_IMPORT, Context.CurrentDevice);
		stopwatch.Start();
		importAndExportWrapper.ImportFileWithNoProgress(24, new List<string> { localFileName }, (string path) => targetFileName, delegate(string path, bool isSuccess)
		{
			ret = isSuccess;
		}, progressNotify);
		stopwatch.Stop();
		resourceExecuteResult.Update(ret);
		HostProxy.BehaviorService.Collect(BusinessType.FILE_IMPORT, businessData.Update(stopwatch.ElapsedMilliseconds, (resourceExecuteResult.Status == 1) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, resourceExecuteResult.ResultMapEx));
		return ret;
	}

	public bool Export(string fileName, string localStorageDir, Action<long, long> progressNotify)
	{
		bool ret = false;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice))
		{
			return false;
		}
		ImportAndExportWrapper importAndExportWrapper = new ImportAndExportWrapper();
		Stopwatch stopwatch = new Stopwatch();
		ResourceExecuteResult executeResult = new ResourceExecuteResult();
		BusinessData businessData = new BusinessData(BusinessType.FILE_EXPORT, Context.CurrentDevice);
		stopwatch.Start();
		importAndExportWrapper.ExportFileWithNoProgress(24, new List<string> { fileName }, localStorageDir, null, delegate(string id, bool isOk, string path)
		{
			ret = isOk;
			executeResult.Update(isOk);
		}, progressNotify);
		stopwatch.Stop();
		HostProxy.BehaviorService.Collect(BusinessType.FILE_EXPORT, businessData.Update(stopwatch.ElapsedMilliseconds, (executeResult.Status == 1) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, executeResult.ResultMapEx));
		return ret;
	}
}
