using System.Collections.Generic;
using System.Linq;
using System.Windows;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.common;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class DeviceCommonManagement
{
	public bool AnalysisResult(string jsonResultString, string valueKey)
	{
		List<PropItem> list = JsonUtils.Parse<List<PropItem>>(jsonResultString);
		if (list != null)
		{
			string text = (from m in list
				where valueKey.Equals(m.Key)
				select m.Value).FirstOrDefault();
			if (!string.IsNullOrEmpty(text) && "true".Equals(text.ToLower()))
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckInternalStorageFreeSize(MessageReaderAndWriter msgRWHander, List<string> filePaths)
	{
		long result = 0L;
		List<PropItem> receiveData = null;
		if (msgRWHander.SendAndReceiveSync("getInternalStorageInfo", "getInternalStorageInfoResponse", new List<string>(), Sequence.SingleInstance.New(), out receiveData) && receiveData != null)
		{
			foreach (PropItem item in receiveData)
			{
				if (item.Key.Equals("free"))
				{
					long.TryParse(item.Value, out result);
				}
			}
		}
		return FileSizeCalculationHelper.GetFilesSize(filePaths) > result;
	}

	public List<int> DeleteDevFiles(string delCommandStr, List<string> fileIdArr)
	{
		if (fileIdArr == null || fileIdArr.Count == 0)
		{
			return null;
		}
		TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (tcpAndroidDevice?.MessageManager == null)
		{
			return null;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return null;
		}
		if (HostProxy.deviceManager.IsMasterConnectedByHelperForAndroid11())
		{
			return DeleteFileOneByOne(messageReaderAndWriter, delCommandStr, fileIdArr);
		}
		return DeleteMultiFile(messageReaderAndWriter, delCommandStr, fileIdArr);
	}

	private List<int> DeleteMultiFile(MessageReaderAndWriter wr, string delCommandStr, List<string> fileIdArr, int max = 2000)
	{
		int num = 0;
		List<string> list = null;
		List<int> list2 = new List<int>();
		long sequence = HostProxy.Sequence.New();
		while (true)
		{
			list = fileIdArr.Skip(num++ * max).Take(max).ToList();
			if (list.Count == 0)
			{
				break;
			}
			MessageEx<int> messageEx = wr.SendAndReceive(delCommandStr, list, sequence);
			if (messageEx == null || messageEx.Action != delCommandStr + "Response")
			{
				for (int i = 0; i < list.Count; i++)
				{
					list2.Add(1);
				}
			}
			else
			{
				list2.AddRange(messageEx.Data);
			}
		}
		return list2;
	}

	private List<int> DeleteFileOneByOne(MessageReaderAndWriter wr, string delCommandStr, List<string> fileIdArr)
	{
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		long sequence = HostProxy.Sequence.New();
		foreach (string item in fileIdArr)
		{
			list.Clear();
			list.Add(item);
			MessageEx<int> messageEx = wr.SendAndReceive(delCommandStr, list, sequence);
			if (messageEx == null || messageEx.Action != delCommandStr + "Response")
			{
				list2.Add(1);
			}
			else
			{
				list2.AddRange(messageEx.Data);
			}
		}
		return list2;
	}

	public bool DeleteDevFilesWithConfirm(string delCommandStr, List<string> fileIdArr, ref Dictionary<string, int> result)
	{
		if (result.Count == 0)
		{
			result = new Dictionary<string, int>
			{
				{ "success", 0 },
				{ "failed", 0 },
				{ "confirm", 0 }
			};
		}
		List<int> list = DeleteDevFiles(delCommandStr, fileIdArr);
		if (list == null)
		{
			return false;
		}
		int scount = list.Where((int p) => p == 0).Count();
		int fcount = list.Where((int p) => p == 1).Count();
		int rcount = list.Where((int p) => p == 2).Count();
		result["success"] += scount;
		result["failed"] += fcount;
		result["confirm"] += rcount;
		if (fcount == 0 && rcount == 0)
		{
			return true;
		}
		if (!HostProxy.CurrentDispatcher.Invoke(delegate
		{
			DeleteResultWindowV6 win = new DeleteResultWindowV6(scount, fcount, rcount);
			if (rcount == 0)
			{
				win.btnGoOn.Visibility = Visibility.Collapsed;
				win.btnCancel.Content = "K0327";
				win.txtWarning.Visibility = Visibility.Collapsed;
			}
			HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				win.ShowDialog();
			});
			return win.DialogResult == true;
		}))
		{
			return false;
		}
		for (int num = fileIdArr.Count - 1; num >= 0; num--)
		{
			if (list[num] == 0)
			{
				fileIdArr.RemoveAt(num);
			}
		}
		return DeleteDevFilesWithConfirm(delCommandStr, fileIdArr, ref result);
	}

	public bool DeleteDevFilesWithConfirmEx(Window ower, string delCommandStr, List<string> fileIdArr)
	{
		List<int> list = DeleteDevFiles(delCommandStr, fileIdArr);
		if (list == null)
		{
			return false;
		}
		int success = list.Where((int p) => p == 0).Count();
		int num = list.Where((int p) => p == 1).Count();
		int num2 = list.Where((int p) => p == 2).Count();
		if (num == 0 && num2 == 0)
		{
			return true;
		}
		if (new DeleteResultWindowV6(success, num, num2)
		{
			Owner = ower,
			WindowStartupLocation = WindowStartupLocation.CenterOwner
		}.ShowDialog() != true)
		{
			return false;
		}
		for (int num3 = fileIdArr.Count - 1; num3 >= 0; num3--)
		{
			if (list[num3] == 0)
			{
				fileIdArr.RemoveAt(num3);
			}
		}
		return DeleteDevFilesWithConfirmEx(ower, delCommandStr, fileIdArr);
	}
}
