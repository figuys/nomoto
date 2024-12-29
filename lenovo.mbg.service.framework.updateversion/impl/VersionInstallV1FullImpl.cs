using System;
using System.Diagnostics;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.updateversion.model;

namespace lenovo.mbg.service.framework.updateversion.impl;

public class VersionInstallV1FullImpl : IVersionInstallV1, IVersionEvent
{
	public VersionModel Model { get; }

	public event EventHandler<VersionV1EventArgs> OnVersionEvent;

	public VersionInstallV1FullImpl(VersionModel model)
	{
		Model = model;
	}

	public void Install()
	{
		this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_INSTALL_START), null, null);
		Process.Start(new ProcessStartInfo
		{
			FileName = Model.localPath,
			UseShellExecute = false,
			CreateNoWindow = true,
			Verb = "runas"
		});
		LogHelper.LogInstance.Info("========================== LMSA client application is closing: Dispose Resource And Exit The Application ===================== ");
		Environment.Exit(0);
	}
}
