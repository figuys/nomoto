namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

public class BackupRestoreStaticResources
{
	private static BackupRestoreStaticResources _singleInstance;

	public readonly string MUSIC = "Songs";

	public readonly string PIC = "Pictures";

	public readonly string APP = "Apps";

	public readonly string CONTACT = "Contacts";

	public readonly string FILE = "Files";

	public readonly string SMS = "SMS";

	public readonly string VIDEO = "Videos";

	public readonly string CALLLOG = "Call log";

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
