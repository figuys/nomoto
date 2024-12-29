using System;
using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.hostproxy;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.backuprestore.Business;

public class DeviceBusinessCommon
{
	public bool GetConcatCount(out int contactCnt, out int callLogCnt)
	{
		bool result = false;
		contactCnt = 0;
		callLogCnt = 0;
		if (Context.CurrentDevice != null)
		{
			using (MessageReaderAndWriter messageReaderAndWriter = (Context.CurrentDevice as TcpAndroidDevice).MessageManager.CreateMessageReaderAndWriter())
			{
				if (messageReaderAndWriter == null)
				{
					return false;
				}
				long sequence = HostProxy.Sequence.New();
				List<PropItem> receiveData = null;
				if (messageReaderAndWriter.SendAndReceiveSync("getContactCount", "getContactCountResponse", new List<string>(), sequence, out receiveData) && receiveData != null && receiveData.Count == 1)
				{
					result = int.TryParse(receiveData[0].Key, out contactCnt);
				}
				return result;
			}
		}
		return false;
	}

	public int GetPicCount()
	{
		if (Context.CurrentDevice != null)
		{
			using (MessageReaderAndWriter messageReaderAndWriter = (Context.CurrentDevice as TcpAndroidDevice).MessageManager.CreateMessageReaderAndWriter())
			{
				if (messageReaderAndWriter == null)
				{
					return 0;
				}
				long sequence = HostProxy.Sequence.New();
				if (messageReaderAndWriter.SendAndReceiveSync("getPICAlbumsInfo", "getPICAlbumsInfoResponse", new List<string>(), sequence, out List<JObject> receiveData) && receiveData != null)
				{
					int total = 0;
					receiveData.ForEach(delegate(JObject n)
					{
						total += n.Value<int>("picsCount");
					});
					return total;
				}
				return 0;
			}
		}
		return 0;
	}

	public int GetMusicCount()
	{
		if (Context.CurrentDevice != null)
		{
			TcpAndroidDevice tcpAndroidDevice = Context.CurrentDevice as TcpAndroidDevice;
			try
			{
				using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
				if (messageReaderAndWriter == null)
				{
					return 0;
				}
				long sequence = HostProxy.Sequence.New();
				List<int> receiveData = null;
				if (messageReaderAndWriter.SendAndReceiveSync("getMusicIdList", "getMusicIdListResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
				{
					return receiveData.Count;
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error(ex.ToString());
			}
		}
		return 0;
	}

	public int GetVideoCount()
	{
		if (Context.CurrentDevice != null)
		{
			using (MessageReaderAndWriter messageReaderAndWriter = (Context.CurrentDevice as TcpAndroidDevice).MessageManager.CreateMessageReaderAndWriter())
			{
				int num = 0;
				if (messageReaderAndWriter != null)
				{
					long sequence = HostProxy.Sequence.New();
					if (messageReaderAndWriter.SendAndReceiveSync("getAlbums", "getAlbumsResponse", new List<string>(), sequence, out List<PropItem> receiveData) && receiveData != null)
					{
						foreach (PropItem item in receiveData)
						{
							int.TryParse(item.Value, out var result);
							num += result;
						}
					}
				}
				return num;
			}
		}
		return 0;
	}

	public int GetSmsCount(string address = null)
	{
		int result = -1;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return 0;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		long sequence = HostProxy.Sequence.New();
		try
		{
			if (messageReaderAndWriter.SendAndReceiveSync("getSmsCount", "getSmsCountResponse", new List<string> { address }, sequence, out List<PropItem> receiveData) && receiveData != null)
			{
				string text = (from m in receiveData
					where "count".Equals(m.Key)
					select m.Value).FirstOrDefault();
				if (!string.IsNullOrEmpty(text))
				{
					int.TryParse(text, out result);
				}
			}
		}
		catch
		{
		}
		return result;
	}
}
