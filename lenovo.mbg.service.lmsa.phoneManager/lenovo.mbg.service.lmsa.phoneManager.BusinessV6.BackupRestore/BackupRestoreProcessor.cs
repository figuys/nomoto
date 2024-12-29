using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class BackupRestoreProcessor
{
	private const int HR_ERROR_HANDLE_DISK_FULL = -2147024857;

	private const int HR_ERROR_DISK_FULL = -2147024784;

	protected Dictionary<string, int> resourceTypeAndServiceCodeMaping = new Dictionary<string, int>
	{
		{ "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", 17 },
		{ "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", 19 },
		{ "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", 20 },
		{ "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", 23 }
	};

	protected Dictionary<string, string> typeActionMapping = new Dictionary<string, string>
	{
		{ "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", "getIdAndSizeMapping" },
		{ "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", "getPictureFolderDirectoryGroups" },
		{ "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", "getMusicFolderDirectoryGroups" },
		{ "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", "getVideoFolderDirectoryGroups" }
	};

	public void LoadDeviceIdAndSize(TcpAndroidDevice device, List<string> resourcesType, Action<bool, string, Dictionary<string, long>> callback)
	{
		Dictionary<string, int> resourceTypeAndServiceCodeMaping = new Dictionary<string, int>
		{
			{ "{773D71F7-CE8A-42D7-BE58-5F875DF58C16}", 17 },
			{ "{242C8F16-6AC7-431B-BBF1-AE24373860F1}", 19 },
			{ "{8BEBE14B-4E45-4D36-8726-8442E6242C01}", 20 },
			{ "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", 23 }
		};
		Task[] array = new Task[resourcesType.Count];
		int num = 0;
		foreach (string item in resourcesType)
		{
			Task task = Task.Run(delegate
			{
				Dictionary<string, long> arg = null;
				try
				{
					string value = new AppServiceRequest(device.ExtendDataFileServiceEndPoint, device.RsaSocketEncryptHelper).RequestString(resourceTypeAndServiceCodeMaping[item], "getIdAndSizeMapping", null);
					if (!string.IsNullOrEmpty(value))
					{
						arg = JsonConvert.DeserializeObject<Dictionary<string, long>>(value);
					}
				}
				catch (Exception ex)
				{
					arg = new Dictionary<string, long>();
					LogHelper.LogInstance.Error("Backup:get resource id list failed, error: " + ex);
				}
				callback?.Invoke(arg1: false, item, arg);
			});
			array[num] = task;
			num++;
		}
		Task.WhenAll(array).ContinueWith(delegate
		{
			callback?.Invoke(arg1: true, null, null);
		});
	}

	public void LoadDeviceIdAndSizeV2(TcpAndroidDevice device, List<string> resourcesType, Action<string, Dictionary<string, long>> callback1, Action<string, List<JObject>> callback2, Action<bool> completedCallbak)
	{
		List<Task> tasks = new List<Task>();
		resourcesType.ForEach(delegate(string n)
		{
			if (device.ConnectedAppType == "Moto" || n == "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}")
			{
				Task item = Task.Run(delegate
				{
					Dictionary<string, long> arg = new Dictionary<string, long>();
					try
					{
						string value = new AppServiceRequest(device.ExtendDataFileServiceEndPoint, device.RsaSocketEncryptHelper).RequestString(resourceTypeAndServiceCodeMaping[n], typeActionMapping["{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}"], null);
						if (!string.IsNullOrEmpty(value))
						{
							arg = JsonConvert.DeserializeObject<Dictionary<string, long>>(value);
						}
					}
					catch (Exception ex)
					{
						LogHelper.LogInstance.Error("Backup:get resource id list failed, error: " + ex);
					}
					callback1?.Invoke(n, arg);
				});
				tasks.Add(item);
			}
			else
			{
				Task item2 = Task.Run(delegate
				{
					List<JObject> receiveData = null;
					try
					{
						using MessageReaderAndWriter messageReaderAndWriter = device.MessageManager.CreateMessageReaderAndWriter();
						if (messageReaderAndWriter != null)
						{
							long sequence = HostProxy.Sequence.New();
							messageReaderAndWriter.SendAndReceive<string, JObject>(typeActionMapping[n], null, sequence, out receiveData);
						}
					}
					catch (Exception ex2)
					{
						LogHelper.LogInstance.Error("Backup:get resource id list failed, error: " + ex2);
					}
					callback2?.Invoke(n, receiveData);
				});
				tasks.Add(item2);
			}
		});
		Task.WhenAll(tasks).ContinueWith(delegate
		{
			completedCallbak?.Invoke(obj: true);
		});
	}
}
