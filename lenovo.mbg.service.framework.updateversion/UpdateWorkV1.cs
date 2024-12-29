using System;
using lenovo.mbg.service.framework.updateversion.impl;
using lenovo.mbg.service.framework.updateversion.model;

namespace lenovo.mbg.service.framework.updateversion;

public class UpdateWorkV1 : IVersionEvent
{
	public IVersionCheckV1 versionCheckV1 { get; private set; }

	public IVersionDownloadV1 versionDownloadV1 { get; private set; }

	public IVersionInstallV1 versionInstallV1 { get; private set; }

	public IVersionDataV1 versionDataV1 { get; private set; }

	public VersionModel model => versionDataV1.Data as VersionModel;

	public event EventHandler<VersionV1EventArgs> OnVersionEvent;

	public UpdateWorkV1(bool autoCheck)
	{
		versionDataV1 = new VersionDataV1Impl();
		versionCheckV1 = new VersionCheckV1Impl(versionDataV1);
		versionDownloadV1 = new VersionDownloadV1Impl(model);
		InnerRegisterEvents();
		if (autoCheck)
		{
			CheckAsync(async: true);
		}
	}

	public UpdateWorkV1(IVersionCheckV1 versionCheckV1, IVersionDataV1 versionDataV1, IVersionDownloadV1 versionDownloadV1, IVersionInstallV1 versionInstallV1)
	{
		this.versionCheckV1 = versionCheckV1;
		this.versionDataV1 = versionDataV1;
		this.versionDownloadV1 = versionDownloadV1;
		this.versionInstallV1 = versionInstallV1;
		InnerRegisterEvents();
	}

	public void CheckAsync(bool async)
	{
		if (async)
		{
			versionCheckV1.CheckAsync();
		}
		else
		{
			versionCheckV1.Check();
		}
	}

	public void Download()
	{
		versionDownloadV1.Start();
	}

	public void Install()
	{
		if (versionInstallV1 == null)
		{
			RegisterInstall();
			versionInstallV1.OnVersionEvent += OnVersionEventHandler;
		}
		versionInstallV1.Install();
	}

	public void RegisterInstall()
	{
		if (model.isFull)
		{
			versionInstallV1 = new VersionInstallV1FullImpl(model);
		}
		else
		{
			versionInstallV1 = new VersionInstallV1IncrementImpl(model);
		}
	}

	private void InnerRegisterEvents()
	{
		if (versionDataV1 != null)
		{
			versionDataV1.OnVersionEvent += OnVersionEventHandler;
		}
		if (versionCheckV1 != null)
		{
			versionCheckV1.OnVersionEvent += OnVersionEventHandler;
		}
		if (versionDownloadV1 != null)
		{
			versionDownloadV1.OnVersionEvent += OnVersionEventHandler;
		}
		if (versionInstallV1 != null)
		{
			versionInstallV1.OnVersionEvent += OnVersionEventHandler;
		}
	}

	private void OnVersionEventHandler(object sender, VersionV1EventArgs e)
	{
		this.OnVersionEvent?.Invoke(sender, e);
	}
}
