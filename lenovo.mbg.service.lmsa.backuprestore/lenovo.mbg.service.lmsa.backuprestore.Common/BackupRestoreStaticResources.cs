namespace lenovo.mbg.service.lmsa.backuprestore.Common;

public class BackupRestoreStaticResources
{
	private static BackupRestoreStaticResources _singleInstance;

	public readonly string MUSIC = "Songs";

	public readonly string PIC = "Pictures";

	public readonly string APP = "Apps";

	public readonly string CONTACT = "Contacts";

	public readonly string SMS = "SMS";

	public readonly string VIDEO = "Videos";

	public readonly string CALLLOG = "Call log";

	public readonly string FILE = "Files";

	public static BackupRestoreStaticResources SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new BackupRestoreStaticResources();
			}
			return _singleInstance;
		}
	}

	private BackupRestoreStaticResources()
	{
	}
}
