using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.Business.ExceptionDefine;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;
using lenovo.mbg.service.lmsa.backuprestore.View;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.backuprestore.ViewModel;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Backup;

public class DeviceBackupMgt
{
	private static ConcurrentDictionary<string, int> resourceCountMapping = new ConcurrentDictionary<string, int>();

	private DeviceBusinessCommon deviceBusinessCommon = new DeviceBusinessCommon();

	public ConcurrentDictionary<string, int> GetResourceCountMapping()
	{
		deviceBusinessCommon.GetConcatCount(out var contactCnt, out var callLogCnt);
		resourceCountMapping["{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}"] = contactCnt + callLogCnt;
		resourceCountMapping["{89D4DB68-4258-4002-8557-E65959C558B3}"] = callLogCnt;
		resourceCountMapping["{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}"] = deviceBusinessCommon.GetSmsCount();
		resourceCountMapping["{773D71F7-CE8A-42D7-BE58-5F875DF58C16}"] = deviceBusinessCommon.GetPicCount();
		resourceCountMapping["{242C8F16-6AC7-431B-BBF1-AE24373860F1}"] = deviceBusinessCommon.GetMusicCount();
		resourceCountMapping["{8BEBE14B-4E45-4D36-8726-8442E6242C01}"] = deviceBusinessCommon.GetVideoCount();
		return resourceCountMapping;
	}

	public void AddOrUpdateBackup(BackupDescription desc)
	{
		AddOrUpdateBackup(new List<BackupDescription> { desc });
	}

	public void AddOrUpdateBackup(List<BackupDescription> descriptions)
	{
		string phoneBackupCacheDir = Configurations.PhoneBackupCacheDir;
		FileInfo fileInfo = new FileInfo(Path.Combine(phoneBackupCacheDir, "backupStorage.db.temp"));
		bool flag = true;
		try
		{
			List<BackupDescription> backupList = GetBackupList();
			foreach (BackupDescription item in descriptions)
			{
				if (backupList.Exists((BackupDescription n) => n.Id == item.Id))
				{
					Context.MessageBox.ShowMessage("K0610");
					descriptions.Clear();
					flag = false;
					return;
				}
				backupList.Add(item);
			}
			using IBackupStorage backupStorage = new BackupStorage(fileInfo);
			backupStorage.Delete();
			IBackupResourceWriter backupResourceWriter = backupStorage.OpenWrite("{71E53A7C-495F-447A-B3DD-E70AC2FBA48F}");
			backupResourceWriter.BeginWrite();
			foreach (BackupDescription item2 in backupList)
			{
				BackupResource backupResource = new BackupResource();
				backupResource.ParentId = 0;
				backupResource.Value = JsonUtils.Stringify(item2);
				backupResourceWriter.Write(backupResource);
			}
			backupResourceWriter.EndWrite();
		}
		finally
		{
			if (flag)
			{
				string text = Path.Combine(phoneBackupCacheDir, "backupStorage.db");
				new FileInfo(text).Delete();
				fileInfo.MoveTo(text);
			}
		}
	}

	public List<BackupDescription> GetBackupList()
	{
		List<BackupDescription> list = new List<BackupDescription>();
		FileInfo fileInfo = new FileInfo(Path.Combine(Configurations.PhoneBackupCacheDir, "backupStorage.db"));
		if (!fileInfo.Exists)
		{
			return list;
		}
		List<BackupDescription> list2 = new List<BackupDescription>();
		List<BackupDescription> list3 = new List<BackupDescription>();
		string version = string.Empty;
		try
		{
			using IBackupStorage backupStorage = new BackupStorage(fileInfo);
			IBackupResourceReader backupResourceReader = backupStorage.OpenRead(out version);
			backupResourceReader.IsSetPassword();
			List<BackupResource> childResources = backupResourceReader.GetChildResources(null);
			JsonHelper.SerializeObject2Json(childResources);
			if (childResources == null)
			{
				return list;
			}
			foreach (BackupResource item in childResources)
			{
				BackupDescription backupDescription = JsonUtils.Parse<BackupDescription>(item.Value);
				list2.Add(backupDescription);
				if (CheckValidity(backupDescription))
				{
					list.Add(backupDescription);
				}
				else
				{
					list3.Add(backupDescription);
				}
			}
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Read file[" + fileInfo.FullName + "] throw exception:" + ex);
		}
		if (list3.Count > 0)
		{
			DeleteBackupFile(list3, list2);
		}
		return list;
	}

	public void ImportBackupFile(string stroagePath, out List<BackupDescription> importStorageList)
	{
		importStorageList = new List<BackupDescription>();
		string version = string.Empty;
		FileInfo fileInfo = new FileInfo(stroagePath);
		IBackupStorage backupStorage = new BackupStorage(fileInfo);
		IBackupResourceReader reader = backupStorage.OpenRead(out version);
		if (reader.IsSetPassword())
		{
			EnterPasswordViewModel enterPasswordViewModel = new EnterPasswordViewModel((string p) => reader.CheckPassword(p));
			EnterPasswordWindow userUi = new EnterPasswordWindow
			{
				DataContext = enterPasswordViewModel
			};
			Context.MessageBox.ShowMessage(userUi);
			if (enterPasswordViewModel.Result == false)
			{
				reader.Dispose();
				backupStorage.Dispose();
				return;
			}
		}
		BackupDescription backupDescription = JsonUtils.Parse<BackupDescription>(((reader.GetChildResources(null) ?? throw new BackupRestoreException("Storage root resource is null")).FirstOrDefault((BackupResource m) => m.Tag.Equals("{AF7750C4-A38C-400F-9A9C-5C3DAC0CA829}")) ?? throw new BackupRestoreException("Can not found description resource int storage:" + stroagePath)).Value);
		backupDescription.StorageSize = fileInfo.Length;
		backupDescription.StoragePath = stroagePath;
		importStorageList.Add(backupDescription);
		reader.Dispose();
		backupStorage.Dispose();
		AddOrUpdateBackup(importStorageList);
	}

	public void DeleteBackupFile(BackupDescription delete)
	{
		DeleteBackupFile(new List<BackupDescription> { delete });
	}

	public void DeleteBackupFile(List<BackupDescription> deletes, List<BackupDescription> storedBackupList = null)
	{
		string phoneBackupCacheDir = Configurations.PhoneBackupCacheDir;
		FileInfo fileInfo = null;
		string fileName = Path.Combine(phoneBackupCacheDir, "backupStorage.db.temp");
		try
		{
			fileInfo = new FileInfo(fileName);
			if (storedBackupList == null)
			{
				storedBackupList = GetBackupList();
			}
			for (int num = deletes.Count - 1; num >= 0; num--)
			{
				string id = deletes[num].Id;
				for (int num2 = storedBackupList.Count - 1; num2 >= 0; num2--)
				{
					if (id.Equals(storedBackupList[num2].Id))
					{
						storedBackupList.RemoveAt(num2);
					}
				}
			}
			using (IBackupStorage backupStorage = new BackupStorage(fileInfo))
			{
				backupStorage.Delete();
				IBackupResourceWriter backupResourceWriter = backupStorage.OpenWrite("{71E53A7C-495F-447A-B3DD-E70AC2FBA48F}");
				backupResourceWriter.BeginWrite();
				foreach (BackupDescription storedBackup in storedBackupList)
				{
					BackupResource backupResource = new BackupResource();
					backupResource.ParentId = 0;
					backupResource.Value = JsonUtils.Stringify(storedBackup);
					backupResourceWriter.Write(backupResource);
				}
				backupResourceWriter.EndWrite();
			}
			string text = Path.Combine(phoneBackupCacheDir, "backupStorage.db");
			File.Delete(text);
			if (fileInfo.Exists)
			{
				fileInfo.MoveTo(text);
			}
			deletes.ForEach(delegate(BackupDescription n)
			{
				if (!string.IsNullOrEmpty(n.StoragePath) && File.Exists(n.StoragePath))
				{
					try
					{
						File.Delete(n.StoragePath);
					}
					catch
					{
					}
				}
			});
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Delete backup file and record failed, ex:" + ex);
			try
			{
				fileInfo.Delete();
			}
			catch (Exception)
			{
			}
		}
	}

	private bool CheckValidity(BackupDescription source)
	{
		try
		{
			if (!File.Exists(source.StoragePath))
			{
				return false;
			}
			FileInfo fileInfo = new FileInfo(source.StoragePath);
			if (source.StorageSize != fileInfo.Length)
			{
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
