using lenovo.mbg.service.framework.updateversion;
using lenovo.mbg.service.lmsa.UpdateVersion.Tool;

namespace lenovo.mbg.service.lmsa.UpdateVersion;

public class UpdateManager
{
	private static object locker = new object();

	private static UpdateManager m_instance;

	private UpdateWoker m_toolWorker;

	private UpdateWoker m_pluginWorker = null;

	private UpdateVersionAutoPush m_toolAutoPush;

	public UpdateVersionAutoPush toolAutoPush
	{
		get
		{
			if (m_toolAutoPush == null)
			{
				m_toolAutoPush = new UpdateVersionAutoPush(m_toolWorker);
				return m_toolAutoPush;
			}
			return m_toolAutoPush;
		}
	}

	public static UpdateManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				lock (locker)
				{
					if (m_instance == null)
					{
						m_instance = new UpdateManager();
					}
				}
			}
			return m_instance;
		}
	}

	public UpdateWoker ToolUpdateWorker => m_toolWorker;

	public UpdateWoker PluginUpdateWorker => m_pluginWorker;

	private UpdateManager()
	{
	}

	public void InitializeToolVersionUpdate()
	{
		IVersionData verionData = new ToolVersionDataFromDb();
		IVersionCheck checkVersion = new ToolCheckVersion(verionData);
		IVersionDownload versionDownload = new VersionDownload();
		IVersionInstall versionInstall = new ToolInstall();
		m_toolWorker = new UpdateWoker(checkVersion, versionDownload, versionInstall);
	}
}
