using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Model;

namespace lenovo.mbg.service.lmsa.phoneManager.Business;

public class DeviceContactManager : DeviceCommonManagement
{
	private static string cacheDir = Configurations.ContactCacheDir;

	public bool GetCount(out int contactCnt, out int callLogCnt)
	{
		bool result = false;
		contactCnt = 0;
		callLogCnt = 0;
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
		if (messageReaderAndWriter.SendAndReceiveSync("getContactCount", "getContactCountResponse", new List<string>(), sequence, out receiveData) && receiveData != null && receiveData.Count == 1)
		{
			result = int.TryParse(receiveData[0].Key, out contactCnt);
		}
		return result;
	}

	public List<ContactGroup> GetContactGroup()
	{
		List<ContactGroup> receiveData = null;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return receiveData;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return receiveData;
		}
		long sequence = HostProxy.Sequence.New();
		if (messageReaderAndWriter.SendAndReceiveSync("getContactGroups", "getContactGroupsResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
		{
			receiveData.Insert(0, new ContactGroup
			{
				Id = "0",
				Name = "K0514"
			});
		}
		else
		{
			receiveData = new List<ContactGroup>();
		}
		receiveData.Add(new ContactGroup
		{
			Id = "",
			Name = "K0516"
		});
		return receiveData;
	}

	public List<Contact> GetAllContacts()
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return new List<Contact>();
		}
		List<Contact> receiveData = new List<Contact>();
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return receiveData;
		}
		long sequence = HostProxy.Sequence.New();
		if (messageReaderAndWriter.SendAndReceiveSync<object, Contact>("getContacts", "getContactsResponse", null, sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return new List<Contact>();
	}

	public List<Contact> GetGroupContacts(string groupId)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return new List<Contact>();
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return new List<Contact>();
		}
		long sequence = HostProxy.Sequence.New();
		List<Contact> receiveData = null;
		List<string> list = new List<string>();
		if (!string.IsNullOrEmpty(groupId))
		{
			list.Add(groupId);
		}
		if (messageReaderAndWriter.SendAndReceiveSync("getGroupContacts", "getGroupContactsResponse", list, sequence, out receiveData))
		{
		}
		if (receiveData == null)
		{
			receiveData = new List<Contact>();
		}
		return receiveData;
	}

	public ContactDetail GetContactDetail(string contactId)
	{
		ContactDetail contactDetail = null;
		if (!string.IsNullOrEmpty(contactId))
		{
			if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
			{
				return null;
			}
			using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter == null)
			{
				return null;
			}
			IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
			ISequence sequence = HostProxy.Sequence;
			long sequence2 = sequence.New();
			List<ContactDetail> receiveData = null;
			if (messageReaderAndWriter.SendAndReceiveSync("getContactDetail", "getContactDetailResponse", new List<string> { contactId }, sequence2, out receiveData) && receiveData != null)
			{
				if (receiveData != null && receiveData.Count > 0)
				{
					contactDetail = receiveData[0];
					if (contactDetail.NumberList != null)
					{
						foreach (Phone number in contactDetail.NumberList)
						{
							if (number.PhoneType != DetailType.Home && number.PhoneType != DetailType.Work)
							{
								number.PhoneType = DetailType.Other;
							}
						}
					}
				}
				if (contactDetail != null && !string.IsNullOrEmpty(contactDetail.RowAvatarPath))
				{
					if (!messageReaderAndWriter.TryEnterLock(10000))
					{
						return contactDetail;
					}
					try
					{
						sequence2 = sequence.New();
						if (messageReaderAndWriter.Send("exportPICFiles", new List<string> { contactDetail.RowAvatarPath }, sequence2))
						{
							using FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(sequence2);
							if (fileTransferWrapper == null)
							{
								return null;
							}
							TransferFileInfo transferFileInfo = null;
							fileTransferWrapper.ReceiveFile(cacheDir, out transferFileInfo);
							contactDetail.AvatarPath = transferFileInfo.localFilePath;
							fileTransferWrapper.NotifyFileReceiveComplete();
						}
						List<PropItem> receiveData2 = null;
						if (messageReaderAndWriter.Receive("exportPICFilesResponse", out receiveData2, 15000))
						{
							receiveData2?.Exists((PropItem m) => "true".Equals(m.Value));
						}
					}
					finally
					{
						messageReaderAndWriter.ExitLock();
					}
				}
			}
		}
		return contactDetail;
	}

	public ContactDetailEx GetContactDetailEx(string contactId)
	{
		ContactDetailEx result = null;
		if (!string.IsNullOrEmpty(contactId))
		{
			if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
			{
				return null;
			}
			using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter == null)
			{
				return null;
			}
			_ = tcpAndroidDevice.FileTransferManager;
			long sequence = HostProxy.Sequence.New();
			List<ContactDetailEx> receiveData = null;
			if (messageReaderAndWriter.SendAndReceiveSync("getContactDetailEx", "getContactDetailExResponse", new List<string> { contactId }, sequence, out receiveData) && receiveData != null)
			{
				result = receiveData.FirstOrDefault();
			}
		}
		return result;
	}

	public List<ContactAvatar> ExportContactAvatar(List<string> avatarIdList)
	{
		List<ContactAvatar> receiveData = null;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return receiveData;
		}
		using (MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter())
		{
			if (messageReaderAndWriter == null)
			{
				return receiveData;
			}
			IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
			ISequence sequence = HostProxy.Sequence;
			long num = sequence.New();
			_ = string.Empty;
			if (!messageReaderAndWriter.TryEnterLock(10000))
			{
				return receiveData;
			}
			List<TransferFileInfo> list = new List<TransferFileInfo>();
			try
			{
				num = sequence.New();
				if (messageReaderAndWriter.Send("exportContactAvatar", avatarIdList, num))
				{
					using FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num);
					if (fileTransferWrapper == null)
					{
						return receiveData;
					}
					TransferFileInfo transferFileInfo = null;
					foreach (string avatarId in avatarIdList)
					{
						_ = avatarId;
						if (!fileTransferWrapper.ReceiveFile(cacheDir, out transferFileInfo) || transferFileInfo == null)
						{
							return receiveData;
						}
						list.Add(transferFileInfo);
						fileTransferWrapper.NotifyFileReceiveComplete();
					}
				}
				if (messageReaderAndWriter.Receive("exportContactAvatarResponse", out receiveData, 15000) && receiveData != null && receiveData != null)
				{
					foreach (TransferFileInfo item in list)
					{
						foreach (ContactAvatar item2 in receiveData)
						{
							if (item2.ThumbnailFIlePath.Equals(item.FilePath))
							{
								item2.ThumbnailFIlePath = item.localFilePath;
							}
						}
					}
				}
			}
			finally
			{
				messageReaderAndWriter.ExitLock();
			}
		}
		return receiveData;
	}

	public bool AddOrEditContactDetail(EditMode editModel, ContactDetailEx detail, Dictionary<string, System.IO.FileInfo> newAvatarFileInfoList)
	{
		GlobalFun.EncodeBase64(Encoding.UTF8.GetBytes(JsonUtils.Stringify(detail)));
		bool result = false;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using (MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter())
		{
			if (messageReaderAndWriter == null)
			{
				return false;
			}
			_ = tcpAndroidDevice.FileTransferManager;
			long sequence = HostProxy.Sequence.New();
			string appSaveDir = tcpAndroidDevice.Property.InternalStoragePath + "/LMSA/Pic/";
			if (newAvatarFileInfoList != null && newAvatarFileInfoList.Count > 0)
			{
				messageReaderAndWriter.TryEnterLock(1000);
				try
				{
					bool importIsSuccess = false;
					new ImportAndExportWrapper().ImportFileWithNoProgress(17, newAvatarFileInfoList.Values.Select((System.IO.FileInfo m) => m.FullName).ToList(), (string sourcePath) => appSaveDir + Path.GetFileName(sourcePath), delegate(string path, bool isSuccess)
					{
						importIsSuccess = isSuccess;
					});
				}
				catch (Exception ex)
				{
					LogHelper.LogInstance.Error("Add or edit contact throw exception:" + ex.ToString());
					return false;
				}
				finally
				{
					messageReaderAndWriter.ExitLock();
				}
			}
			AddOrEditContactModel addOrEditContactModel = new AddOrEditContactModel();
			addOrEditContactModel.EditMode = editModel.ToString();
			addOrEditContactModel.ContactDetail = detail;
			if (newAvatarFileInfoList != null && newAvatarFileInfoList.Count > 0)
			{
				addOrEditContactModel.Avatars = newAvatarFileInfoList.Select((KeyValuePair<string, System.IO.FileInfo> m) => new PropItem
				{
					Key = m.Key,
					Value = m.Value.Name
				}).ToList();
			}
			List<PropItem> receiveData = null;
			if (messageReaderAndWriter.SendAndReceiveSync("addOrEditContactEx", "addOrEditContactExResponse", new List<AddOrEditContactModel> { addOrEditContactModel }, sequence, out receiveData) && receiveData != null)
			{
				result = receiveData.Exists((PropItem m) => "true".Equals(m.Value));
			}
		}
		return result;
	}

	public bool DeleteContact(List<string> ids)
	{
		bool flag = true;
		int num = 0;
		int count = ids.Count;
		if (ids != null && ids.Count > 0)
		{
			while (flag && num < count)
			{
				List<string> list = new List<string>();
				while (num < count)
				{
					list.Add(ids[num]);
					if (++num % 50 == 0)
					{
						break;
					}
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
				if (messageReaderAndWriter.SendAndReceiveSync("deleteContacts", "deleteContactsResponse", list, sequence, out receiveData) && receiveData != null)
				{
					flag = receiveData.Exists((PropItem m) => "true".Equals(m.Value));
				}
			}
		}
		return flag;
	}

	public bool Export(List<string> ids, string loacalDir)
	{
		bool flag = true;
		if (ids != null && ids.Count > 0)
		{
			if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
			{
				return false;
			}
			using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter == null)
			{
				return false;
			}
			IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
			long num = HostProxy.Sequence.New();
			int num2 = 0;
			int count = ids.Count;
			List<string> list = new List<string>();
			string path = Path.Combine(loacalDir, "contacts_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".vcf");
			HostProxy.ResourcesLoggingService.RegisterFile(path);
			if (!messageReaderAndWriter.TryEnterLock(10000))
			{
				return false;
			}
			try
			{
				using FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite);
				while (flag && num2 < count)
				{
					List<string> list2 = new List<string>();
					while (num2 < count)
					{
						list2.Add(ids[num2]);
						if (++num2 % 50 == 0)
						{
							break;
						}
					}
					if (!(flag &= messageReaderAndWriter.Send("exportContacts", list2.ToList(), num)))
					{
						continue;
					}
					using (FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num))
					{
						if (fileTransferWrapper == null)
						{
							return false;
						}
						TransferFileInfo transferFileInfo = null;
						fileTransferWrapper.ReceiveFile(loacalDir, out transferFileInfo);
						if (File.Exists(transferFileInfo.localFilePath))
						{
							string text = Path.Combine(loacalDir, transferFileInfo.VirtualFileName);
							list.Add(text);
							byte[] bytes = Encoding.UTF8.GetBytes(File.ReadAllText(text));
							fileStream.Write(bytes, 0, bytes.Length);
							File.Delete(text);
						}
					}
					List<PropItem> receiveData = null;
					flag = (flag &= messageReaderAndWriter.Receive("exportContactsResponse", out receiveData, 15000) && receiveData != null) && (flag & receiveData.Exists((PropItem m) => "true".Equals(m.Value)));
				}
			}
			finally
			{
				foreach (string item in list)
				{
					try
					{
						File.Delete(item);
					}
					catch (Exception)
					{
					}
				}
				messageReaderAndWriter.ExitLock();
			}
		}
		return flag;
	}

	public bool Import(string file)
	{
		bool result = false;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		IFileTransferManager fileTransferManager = tcpAndroidDevice.FileTransferManager;
		long num = HostProxy.Sequence.New();
		if (!messageReaderAndWriter.TryEnterLock(10000))
		{
			return result;
		}
		try
		{
			if (messageReaderAndWriter.Send("importContacts", new List<string> { "1" }, num))
			{
				using (FileTransferWrapper fileTransferWrapper = fileTransferManager.CreateFileTransfer(num))
				{
					if (fileTransferWrapper == null)
					{
						return result;
					}
					fileTransferWrapper.SendFile(file, num);
					fileTransferWrapper.WaitFileReceiveCompleteNotify(13000);
				}
				List<PropItem> receiveData = null;
				if (messageReaderAndWriter.Receive("restoreContactResponse", out receiveData, 15000) && receiveData != null)
				{
					result = receiveData.Exists((PropItem m) => "true".Equals(m.Value));
				}
			}
		}
		finally
		{
			messageReaderAndWriter.ExitLock();
		}
		return result;
	}

	public List<CallLog> GetCallLogs()
	{
		List<CallLog> result = new List<CallLog>();
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return result;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return result;
		}
		long sequence = HostProxy.Sequence.New();
		List<CallLog> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getCallLogs", "getCallLogsResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return result;
	}

	public bool ExportCallLogs(List<CallLog> logs, string loacalDir)
	{
		bool result = false;
		if (logs != null && logs.Count > 0)
		{
			string text = Path.Combine(loacalDir, DateTime.Now.ToString("yyyyMMddHHmm") + ".calllog");
			HostProxy.ResourcesLoggingService.RegisterFile(text);
			result = JsonHelper.SerializeObject2File(text, logs);
		}
		return result;
	}

	public bool ImportCallLogs(string filepath)
	{
		bool result = false;
		if (File.Exists(filepath))
		{
			try
			{
				List<CallLog> list = JsonHelper.DeserializeJson2List<CallLog>(File.ReadAllText(filepath));
				if (list == null || list.Count == 0)
				{
					return result;
				}
				if (list.Count > 0)
				{
					DateTime tempTime = DateTime.MinValue;
					if (list.Any(delegate(CallLog c)
					{
						if (c.Contact == null)
						{
							return true;
						}
						if (c.ROWCallDate == null)
						{
							return true;
						}
						return !DateTime.TryParse(c.ROWCallDate, out tempTime);
					}))
					{
						return result;
					}
				}
				return MassOpImpl("importCallLogs", "importCallLogsResponse", list);
			}
			catch
			{
				return false;
			}
		}
		return result;
	}

	public bool DeleteCallLog(List<string> ids)
	{
		if (ids == null || ids.Count == 0)
		{
			return false;
		}
		bool flag = true;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using (MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter())
		{
			if (messageReaderAndWriter == null)
			{
				return false;
			}
			ISequence sequence = HostProxy.Sequence;
			try
			{
				messageReaderAndWriter.TryEnterLock(10000);
				int num = 0;
				int count = ids.Count;
				while (num < count)
				{
					List<string> list = new List<string>();
					do
					{
						list.Add(ids[num++]);
					}
					while (num < count && num % 20 != 0);
					if (flag &= messageReaderAndWriter.Send("deleteCallLogs", list, sequence.New()))
					{
						List<PropItem> receiveData = null;
						flag = (flag &= messageReaderAndWriter.Receive("deleteCallLogsResponse", out receiveData, 15000) && receiveData != null) && (flag & receiveData.Exists((PropItem m) => "true".Equals(m.Value)));
					}
					if (!flag)
					{
						break;
					}
				}
			}
			finally
			{
				messageReaderAndWriter.ExitLock();
			}
		}
		return flag;
	}

	public bool AddContactGroup(string groupName)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(groupName))
		{
			if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
			{
				return false;
			}
			using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
			if (messageReaderAndWriter == null)
			{
				return false;
			}
			_ = tcpAndroidDevice.FileTransferManager;
			long sequence = HostProxy.Sequence.New();
			List<PropItem> receiveData = null;
			if (messageReaderAndWriter.SendAndReceiveSync("addContactGroup", "addContactGroupResponse", new List<string> { groupName }, sequence, out receiveData) && receiveData != null)
			{
				result = receiveData.Exists((PropItem m) => "true".Equals(m.Value));
			}
		}
		return result;
	}

	private bool MassOpImpl(string req, string resp, List<CallLog> param)
	{
		bool flag = true;
		_ = string.Empty;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return false;
		}
		using (MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter())
		{
			if (messageReaderAndWriter == null)
			{
				return false;
			}
			ISequence sequence = HostProxy.Sequence;
			messageReaderAndWriter.TryEnterLock(100000);
			try
			{
				if (param != null && param.Count > 0)
				{
					int num = 0;
					int count = param.Count;
					while (num < count)
					{
						List<CallLog> list = new List<CallLog>();
						do
						{
							list.Add(param[num++]);
						}
						while (num % 5 != 0 && num < count);
						List<ImportResponse> receiveData = null;
						flag = messageReaderAndWriter.SendAndReceive(req, resp, list, sequence.New(), out receiveData) && receiveData != null && (flag & (receiveData?.All((ImportResponse e) => e.result) ?? false));
						if (!flag)
						{
							break;
						}
					}
				}
				else
				{
					flag = false;
				}
			}
			finally
			{
				messageReaderAndWriter.ExitLock();
			}
		}
		return flag;
	}

	public List<string> GetAllContactsId()
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return new List<string>();
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return new List<string>();
		}
		long sequence = HostProxy.Sequence.New();
		List<string> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync<object, string>("getAllContactId", "getAllContactIdResponse", null, sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return new List<string>();
	}

	public List<string> getContactsIdListByGroupId(string groupId)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return new List<string>();
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return new List<string>();
		}
		long sequence = HostProxy.Sequence.New();
		List<string> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getContactIdListByGroupId", "getContactIdListByGroupIdResponse", new List<string> { groupId }, sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return new List<string>();
	}

	public List<Contact> getContactListByContactId(List<string> contactsIdList)
	{
		List<Contact> result = null;
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return result;
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return result;
		}
		long sequence = HostProxy.Sequence.New();
		List<Contact> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getContactListByContactId", "getContactListByContactIdResponse", contactsIdList, sequence, out receiveData) && receiveData != null)
		{
			result = receiveData;
		}
		return result;
	}

	public List<string> getHaveBeenGroupedContactIdList()
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return new List<string>();
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return new List<string>();
		}
		long sequence = HostProxy.Sequence.New();
		List<string> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getHaveBeenGroupedContactIdList", "getHaveBeenGroupedContactIdListResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return new List<string>();
	}

	public List<string> queryContactIdListByKeyWords(string keyWords)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return new List<string>();
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return new List<string>();
		}
		long sequence = HostProxy.Sequence.New();
		List<string> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("queryContactIdListByKeyWords", "queryContactIdListByKeyWordsResponse", new List<string> { keyWords }, sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return new List<string>();
	}

	public List<string> GetAllCallLogIdList()
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return new List<string>();
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return new List<string>();
		}
		long sequence = HostProxy.Sequence.New();
		List<string> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getAllCallLogId", "getAllCallLogIdResponse", new List<string>(), sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return new List<string>();
	}

	public List<CallLog> GetCallLogListById(List<string> idList)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return new List<CallLog>();
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return new List<CallLog>();
		}
		long sequence = HostProxy.Sequence.New();
		List<CallLog> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("getCallLogById", "getCallLogByIdResponse", idList, sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return new List<CallLog>();
	}

	public List<string> queryCallLogIdListByKeyWords(string keyWords)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return new List<string>();
		}
		using MessageReaderAndWriter messageReaderAndWriter = tcpAndroidDevice.MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return new List<string>();
		}
		long sequence = HostProxy.Sequence.New();
		List<string> receiveData = null;
		if (messageReaderAndWriter.SendAndReceiveSync("queryCallLogIdListByKeyWords", "queryCallLogIdListByKeyWordsResponse", new List<string> { keyWords }, sequence, out receiveData) && receiveData != null)
		{
			return receiveData;
		}
		return new List<string>();
	}
}
