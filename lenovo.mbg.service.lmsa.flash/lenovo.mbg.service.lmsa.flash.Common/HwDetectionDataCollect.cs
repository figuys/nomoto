using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class HwDetectionDataCollect
{
	public void BeginCollect()
	{
		Task.Factory.StartNew(delegate
		{
			try
			{
				Dictionary<string, string> dictionary = ExportFileFromDevice();
				if (dictionary != null)
				{
					Dictionary<string, HwDetection> dictionary2 = ParsingXmlFile(dictionary);
					if (dictionary2 != null)
					{
						Dictionary<string, string> dictionary3 = SubmitData(dictionary2);
						if (dictionary3 != null)
						{
							DeleteFileFromDevice(dictionary3);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Hw data collect throw exception:" + ex.ToString());
			}
		});
	}

	private Dictionary<string, string> ExportFileFromDevice()
	{
		TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (tcpAndroidDevice?.MessageManager == null)
		{
			return null;
		}
		using (MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter())
		{
			if (messageReaderAndWriter == null)
			{
				return null;
			}
			IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
			ISequence sequence = HostProxy.Sequence;
			long num = sequence.New();
			if (!messageReaderAndWriter.TryEnterLock(10000))
			{
				return null;
			}
			try
			{
				List<string> receiveData = null;
				if (messageReaderAndWriter.Send<object>("getHwDetectionFilePaths", null, num = sequence.New()) && messageReaderAndWriter.Receive("getHwDetectionFilePathsResponse", out receiveData, 15000) && receiveData != null)
				{
					if (receiveData == null || receiveData.Count == 0)
					{
						return null;
					}
					num = sequence.New();
					messageReaderAndWriter.Send("exportHwDetectionFiles", receiveData, num);
					string localStorageDir = Path.Combine(Configurations.TempDir);
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					using (FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num))
					{
						if (fileTransferWrapper != null)
						{
							TransferFileInfo transferFileInfo = null;
							foreach (string item in receiveData)
							{
								_ = item;
								if (fileTransferWrapper.ReceiveFile(localStorageDir, out transferFileInfo))
								{
									dictionary[transferFileInfo.FilePath] = transferFileInfo.localFilePath;
									fileTransferWrapper.NotifyFileReceiveComplete();
									continue;
								}
								return dictionary;
							}
						}
					}
					List<PropItem> receiveData2 = null;
					messageReaderAndWriter.Receive("exportHwDetectionFilesResponse", out receiveData2, 15000);
					return dictionary;
				}
			}
			finally
			{
				messageReaderAndWriter.ExitLock();
			}
		}
		return null;
	}

	private Dictionary<string, HwDetection> ParsingXmlFile(Dictionary<string, string> fileMapping)
	{
		if (fileMapping == null)
		{
			return null;
		}
		Dictionary<string, HwDetection> dictionary = new Dictionary<string, HwDetection>();
		foreach (KeyValuePair<string, string> item in fileMapping)
		{
			try
			{
				if (!new FileInfo(item.Value).Exists)
				{
					continue;
				}
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(item.Value);
				HwDetection hwDetection = new HwDetection();
				hwDetection.StartTime = xmlDocument.SelectSingleNode("/hwDetection/startTime").InnerText;
				hwDetection.FinishTime = xmlDocument.SelectSingleNode("/hwDetection/finishTime").InnerText;
				int result = 0;
				int.TryParse(xmlDocument.SelectSingleNode("/hwDetection/hwDetectionResult").InnerText, out result);
				hwDetection.Result = result;
				hwDetection.ErrorMessage = xmlDocument.SelectSingleNode("/hwDetection/errorMessage").InnerText;
				List<HwDetectionItem> list = new List<HwDetectionItem>();
				foreach (XmlNode item2 in xmlDocument.SelectNodes("/hwDetection/testItem"))
				{
					HwDetectionItem hwDetectionItem = new HwDetectionItem();
					hwDetectionItem.Name = item2.SelectSingleNode("itemName").InnerText;
					result = 0;
					int.TryParse(item2.SelectSingleNode("itemResult").InnerText, out result);
					hwDetectionItem.Result = result;
					hwDetectionItem.ErrorMessage = item2.SelectSingleNode("errorMessage").InnerText;
					list.Add(hwDetectionItem);
				}
				hwDetection.Items = list;
				dictionary[item.Key] = hwDetection;
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error($"Parsing hw detection file[{item.Value}] throw exception{ex.ToString()}");
			}
		}
		return dictionary;
	}

	private Dictionary<string, string> SubmitData(Dictionary<string, HwDetection> datasMapping)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return null;
		}
		IAndroidDevice property = tcpAndroidDevice.Property;
		_ = string.Empty;
		string modelName = property.ModelName;
		string sN = property.SN;
		string iMEI = property.IMEI1;
		string iMEI2 = property.IMEI2;
		string androidVersion = property.AndroidVersion;
		string clientUUID = GlobalFun.GetClientUUID();
		foreach (KeyValuePair<string, HwDetection> item in datasMapping)
		{
			item.Value.Model = modelName;
			item.Value.SN = sN;
			item.Value.Imei1 = iMEI;
			item.Value.Imei2 = iMEI2;
			item.Value.AndroidVersion = androidVersion;
			item.Value.clientUuid = clientUUID;
		}
		ApiService apiService = new ApiService();
		string uPLOAD_HW_DETECTION = WebServicesContext.UPLOAD_HW_DETECTION;
		ResponseModel<object> responseModel = null;
		_ = string.Empty;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, HwDetection> item2 in datasMapping)
		{
			responseModel = apiService.RequestBase(uPLOAD_HW_DETECTION, item2.Value);
			if (responseModel != null)
			{
				dictionary[item2.Key] = responseModel.code;
			}
			else
			{
				dictionary[item2.Key] = null;
			}
		}
		return dictionary;
	}

	private void DeleteFileFromDevice(Dictionary<string, string> resultMapping)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter != null)
		{
			_ = tcpAndroidDevice.FileTransferManager;
			long sequence = HostProxy.Sequence.New();
			List<PropItem> receiveData = null;
			messageReaderAndWriter.SendAndReceiveSync("deleteHwDetecionFiles", "deleteHwDetecionFilesResponse", (from m in resultMapping
				where "0000".Equals(m.Value)
				select m.Key).ToList(), sequence, out receiveData);
		}
	}
}
