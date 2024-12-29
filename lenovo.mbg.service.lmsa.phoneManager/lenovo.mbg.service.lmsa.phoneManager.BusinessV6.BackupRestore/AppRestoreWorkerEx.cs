using System;
using System.IO;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.lmsa.phonemanager.apps.Common;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

[RestoreStorageInfo("{958781C8-0788-4F87-A4C3-CBD793AAB1A0}", "{9A7CEC3B-8DE0-439D-B3CF-5DAD3691C605}")]
[RestoreStorageInfo("{958781C8-0788-4F87-A4C3-CBD793AAB1A0}", "{F6DC6725-E73D-41B6-9DB0-B964C48BCEAF}")]
public class AppRestoreWorkerEx : RestoreWorkerAbstractEx
{
	private DeviceAppManager AppManager;

	public AppRestoreWorkerEx(TcpAndroidDevice device, IBackupResourceReader backupResourceReader, string resourceType)
		: base(device, backupResourceReader, resourceType, -1, string.Empty)
	{
		AppManager = DeviceAppManager.Instance;
	}

	public override void DoProcess(object state)
	{
		CheckCancel();
		if (base.Device == null)
		{
			throw new BackupRestoreException("Current device instance is null,bakcup pic failed!");
		}
		foreach (BackupResource childResourceNode in base.ChildResourceNodes)
		{
			CheckCancel();
			if (!"file".Equals(childResourceNode.Tag))
			{
				continue;
			}
			base.ItemStartRestoreCallback?.Invoke(childResourceNode.Value);
			IBackupResourceStreamReader backupResourceStreamReader = base.BackupResourceReader.Seek(childResourceNode);
			int num = 1024;
			byte[] buffer = new byte[num];
			int num2 = 0;
			string text = Path.Combine(Configurations.TempDir, Guid.NewGuid().ToString("N") + ".apk");
			base.ItemRestoreProgressCallback(childResourceNode.Value, 0, childResourceNode.AssociatedStreamSize, childResourceNode.AssociatedStreamSize);
			try
			{
				using (FileStream fileStream = new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write))
				{
					do
					{
						CheckCancel();
						num2 = backupResourceStreamReader.Read(buffer, 0, num);
						fileStream.Write(buffer, 0, num2);
					}
					while (num2 != 0);
				}
				int num3 = AppManager.Install(text, base.Device);
				base.ItemFinishRestoreCallback?.Invoke(childResourceNode.Value, childResourceNode.Value, num3 >= 1);
			}
			catch (Exception ex)
			{
				base.ItemFinishRestoreCallback?.Invoke(childResourceNode.Value, childResourceNode.Value, arg3: false);
				LogHelper.LogInstance.Error("Do process throw ex:" + ex);
			}
			finally
			{
				try
				{
					File.Delete(text);
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
