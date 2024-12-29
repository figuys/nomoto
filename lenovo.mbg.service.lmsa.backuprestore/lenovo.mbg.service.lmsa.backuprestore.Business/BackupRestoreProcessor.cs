using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.backuprestore.Business;

public class BackupRestoreProcessor
{
	private const int HR_ERROR_HANDLE_DISK_FULL = -2147024857;

	private const int HR_ERROR_DISK_FULL = -2147024784;

	protected Dictionary<string, int> resourceTypeAndServiceCodeMaping = new Dictionary<string, int>
	{
		{ "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", 17 },
		{ "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", 19 },
		{ "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", 20 },
		{ "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", 23 },
		{ "{580C48C8-6CEF-4BBB-AF37-D880B349D142}", 25 },
		{ "{89D4DB68-4258-4002-8557-E65959C558B3}", 22 },
		{ "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}", 21 }
	};

	protected Dictionary<string, string> typeActionMapping = new Dictionary<string, string>
	{
		{ "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", "getIdAndSizeMapping" },
		{ "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}", "getIdAndSizeMapping" },
		{ "{89D4DB68-4258-4002-8557-E65959C558B3}", "getIdAndSizeMapping" },
		{ "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", "getPictureFolderDirectoryGroups" },
		{ "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", "getMusicFolderDirectoryGroups" },
		{ "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", "getVideoFolderDirectoryGroups" },
		{ "{580C48C8-6CEF-4BBB-AF37-D880B349D142}", "getFileDirectoryGroups" }
	};

	protected Dictionary<string, string> resourceTypeAndFullPathMapping = new Dictionary<string, string>
	{
		{ "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", "getPicturePathById" },
		{ "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", "getMusicPathById" },
		{ "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", "getVideoPathById" }
	};

	public void LoadDeviceIdAndSizeV2(TcpAndroidDevice device, List<string> resourcesType, bool _isInternal, Action<string, Dictionary<string, long>> callback1, Action<string, List<JObject>> callback2, Action<bool> completedCallbak)
	{
		List<Task> tasks = new List<Task>();
		resourcesType.ForEach(delegate(string n)
		{
			if (device.ConnectedAppType == "Moto" || n == "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}" || n == "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}" || n == "{89D4DB68-4258-4002-8557-E65959C558B3}")
			{
				tasks.Add(Task.Run(delegate
				{
					try
					{
						string value = new AppServiceRequest(device.ExtendDataFileServiceEndPoint, device.RsaSocketEncryptHelper).RequestString(resourceTypeAndServiceCodeMaping[n], typeActionMapping["{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}"], null);
						if (!string.IsNullOrEmpty(value))
						{
							Dictionary<string, long> arg = JsonConvert.DeserializeObject<Dictionary<string, long>>(value);
							callback1?.Invoke(n, arg);
						}
					}
					catch (Exception ex)
					{
						LogHelper.LogInstance.Error("Backup:get resource id list failed, error: " + ex.Message);
					}
				}));
			}
			else
			{
				tasks.Add(Task.Run(delegate
				{
					try
					{
						using MessageReaderAndWriter messageReaderAndWriter = device.MessageManager?.CreateMessageReaderAndWriter();
						if (messageReaderAndWriter != null)
						{
							long sequence = HostProxy.Sequence.New();
							List<JObject> receiveData = null;
							List<string> parameter = new List<string> { _isInternal ? "0" : "1" };
							messageReaderAndWriter.SendAndReceive(typeActionMapping[n], parameter, sequence, out receiveData);
							callback2?.Invoke(n, receiveData);
						}
					}
					catch (Exception ex2)
					{
						LogHelper.LogInstance.Error("Backup:get resource id list failed, error: " + ex2);
					}
				}));
			}
		});
		Task.WhenAll(tasks).ContinueWith(delegate
		{
			completedCallbak?.Invoke(obj: true);
		});
	}

	public string GetFileFullPathById(TcpAndroidDevice device, string _resType, string _fileId)
	{
		try
		{
			if (!resourceTypeAndFullPathMapping.ContainsKey(_resType))
			{
				return _fileId;
			}
			using MessageReaderAndWriter messageReaderAndWriter = device.MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter == null)
			{
				return _fileId;
			}
			long sequence = HostProxy.Sequence.New();
			List<string> parameter = new List<string> { _fileId };
			List<string> receiveData = new List<string>();
			messageReaderAndWriter.SendAndReceive(resourceTypeAndFullPathMapping[_resType], parameter, sequence, out receiveData);
			return receiveData.FirstOrDefault();
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"Backup get resource full path failed, type:[{_resType}], file id:[{_fileId}], error:[{arg}] ");
			return _fileId;
		}
	}
}
