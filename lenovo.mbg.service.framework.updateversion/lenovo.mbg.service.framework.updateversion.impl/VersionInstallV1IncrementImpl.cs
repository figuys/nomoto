using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.pipes;
using lenovo.mbg.service.framework.updateversion.model;

namespace lenovo.mbg.service.framework.updateversion.impl;

public class VersionInstallV1IncrementImpl : IVersionInstallV1, IVersionEvent
{
	public VersionModel Model { get; }

	public string UpdateDirectory { get; private set; }

	public string DestDirectory { get; private set; }

	public string BackuPath { get; private set; }

	public event EventHandler<VersionV1EventArgs> OnVersionEvent;

	public VersionInstallV1IncrementImpl(VersionModel model)
	{
		Model = model;
		UpdateDirectory = Path.Combine(Path.GetTempPath(), "lmsaupdate");
		BackuPath = Path.Combine(Path.GetTempPath(), "lmsabackup.7z");
		DestDirectory = AppDomain.CurrentDomain.BaseDirectory;
		if (Directory.Exists(UpdateDirectory))
		{
			GlobalFun.DeleteDirectory(UpdateDirectory);
		}
		Directory.CreateDirectory(UpdateDirectory);
	}

	public void Install()
	{
		if (SevenZipHelper.Instance.Extractor(Model.localPath, UpdateDirectory) != 0)
		{
			this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_FAILED), null, null);
			LogHelper.LogInstance.Info("version increment upgradepackage extrator failed");
			return;
		}
		string[] files = Directory.GetFiles(UpdateDirectory, "*", SearchOption.AllDirectories);
		this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_START, files.Length), null, null);
		Task<bool> task = Backup();
		task.Wait();
		if (!task.Result)
		{
			this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_FAILED), null, null);
			LogHelper.LogInstance.Info("version increment backup failed");
		}
		else if (!DoUpgrade(UpdateDirectory, DestDirectory))
		{
			Rollback().ContinueWith(delegate(Task<int> s)
			{
				RollbackCallback(s.Result == 0);
			});
		}
		else
		{
			this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_SUCCESS, null), null, null);
		}
	}

	private bool DoUpgrade(string sourceFolder, string destFolder)
	{
		try
		{
			if (!Directory.Exists(destFolder))
			{
				Directory.CreateDirectory(destFolder);
			}
			string[] files = Directory.GetFiles(sourceFolder);
			foreach (string text in files)
			{
				this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_PERCENT, text), null, null);
				File.Copy(text, Path.Combine(destFolder, Path.GetFileName(text)), overwrite: true);
			}
			files = Directory.GetDirectories(sourceFolder);
			foreach (string text2 in files)
			{
				DoUpgrade(text2, Path.Combine(destFolder, Path.GetFileName(text2)));
			}
			return true;
		}
		catch (Exception ex)
		{
			this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_FAILED), null, null);
			LogHelper.LogInstance.Error("version increment copy file failed, exception: " + ex.ToString());
			return false;
		}
	}

	private Task<bool> Backup()
	{
		this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_BACKUP, null), null, null);
		return Task.Factory.StartNew(delegate
		{
			if (File.Exists(BackuPath))
			{
				File.Delete(BackuPath);
			}
			return SevenZipHelper.Instance.Compress(DestDirectory, BackuPath, delegate(int p)
			{
				this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_BACKUP_PERCENT, p), null, null);
			});
		});
	}

	private Task<int> Rollback()
	{
		this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_ROLLBACK, null), null, null);
		return Task.Factory.StartNew(delegate
		{
			GlobalFun.DeleteDirectory(DestDirectory);
			return SevenZipHelper.Instance.Extractor(BackuPath, DestDirectory, delegate(int p)
			{
				this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_ROLLBACK_PERCENT, p), null, null);
			});
		});
	}

	private void RollbackCallback(bool success)
	{
		if (success)
		{
			this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_ROLLBACK_SUCCESS, null), null, null);
			return;
		}
		this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_ROLLBACK_FAILED, null), null, null);
		new ClientPipe().Connect(5000);
		Thread.Sleep(500);
		LogHelper.LogInstance.Info("========================== LMSA client application is closing: Dispose Resource And Exit The Application ===================== ");
		Environment.Exit(0);
	}
}
