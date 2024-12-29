using System;

namespace lenovo.mbg.service.framework.updateversion;

public class UpdateWoker
{
	private IVersionCheck m_checkVersion;

	private IVersionDownload m_versionDownload;

	private IVersionInstall m_versionInstall;

	private IVersionUnInstall m_versionUnInstall;

	public event EventHandler<CheckVersionEventArgs> OnCheckVersionStatusChanged;

	public event EventHandler<DownloadStatusChangedArgs> OnDownloadStatusChanged;

	public event EventHandler<VersionInstallEventArgs> OnInstallStatusChanged;

	public event EventHandler<VersionUnInstallEventArgs> OnUnInstallStatusChanged;

	public UpdateWoker(IVersionCheck checkVersion, IVersionDownload versionDownload, IVersionInstall versionInstall = null, IVersionUnInstall versionUnInstall = null)
	{
		m_checkVersion = checkVersion;
		m_versionDownload = versionDownload;
		m_versionInstall = versionInstall;
		m_versionUnInstall = versionUnInstall;
		if (m_checkVersion != null)
		{
			m_checkVersion.OnCheckVersionStatusChanged += m_checkVersion_OnCheckVersionStatusChanged;
		}
		if (m_versionDownload != null)
		{
			m_versionDownload.OnDownloadStatusChanged += m_versionDownload_OnDownloadStatusChanged;
		}
		if (m_versionInstall != null)
		{
			m_versionInstall.OnInstallStatusChanged += m_versionInstall_OnInstallStatusChanged;
		}
		if (m_versionUnInstall != null)
		{
			m_versionUnInstall.OnUnInstallStatusChanged += m_versionUnInstall_OnUnInstallStatusChanged;
		}
	}

	public void CheckVersion(bool isAutoMode)
	{
		if (m_checkVersion != null)
		{
			m_checkVersion.Check(isAutoMode);
		}
	}

	public void DownloadVersion(object data)
	{
		if (m_versionDownload != null)
		{
			m_versionDownload.DownloadVersion(data);
		}
	}

	public void CancelDownloadVersion(object data)
	{
		if (m_versionDownload != null)
		{
			m_versionDownload.Cancel();
		}
	}

	public void InstallVersion(object data)
	{
		if (m_versionInstall != null)
		{
			m_versionInstall.InstallVersion(data);
		}
	}

	public void CancelInstallVersion()
	{
		if (m_versionInstall != null)
		{
			m_versionInstall.Cancel();
		}
	}

	public void UnInstallVersion(object data)
	{
		if (m_versionUnInstall != null)
		{
			m_versionUnInstall.UnInstallVersion(data);
		}
	}

	public void CancelUnInstallVersion()
	{
		if (m_versionUnInstall != null)
		{
			m_versionUnInstall.Cancel();
		}
	}

	private void m_checkVersion_OnCheckVersionStatusChanged(object sender, CheckVersionEventArgs e)
	{
		if (this.OnCheckVersionStatusChanged != null)
		{
			this.OnCheckVersionStatusChanged(sender, e);
		}
	}

	private void m_versionDownload_OnDownloadStatusChanged(object sender, DownloadStatusChangedArgs e)
	{
		if (this.OnDownloadStatusChanged != null)
		{
			this.OnDownloadStatusChanged(sender, e);
		}
	}

	private void m_versionInstall_OnInstallStatusChanged(object sender, VersionInstallEventArgs e)
	{
		if (this.OnInstallStatusChanged != null)
		{
			this.OnInstallStatusChanged(sender, e);
		}
	}

	private void m_versionUnInstall_OnUnInstallStatusChanged(object sender, VersionUnInstallEventArgs e)
	{
		if (this.OnUnInstallStatusChanged != null)
		{
			this.OnUnInstallStatusChanged(sender, e);
		}
	}
}
