using System;
using lenovo.mbg.service.framework.updateversion.download;
using lenovo.mbg.service.framework.updateversion.model;

namespace lenovo.mbg.service.framework.updateversion.impl;

public class VersionDownloadV1Impl : IVersionDownloadV1, IVersionEvent
{
	private HttpDownload httpDownload;

	public event EventHandler<VersionV1EventArgs> OnVersionEvent;

	public VersionDownloadV1Impl(VersionModel model)
	{
		httpDownload = new HttpDownload(model, FireVersionEvent);
	}

	public void Start()
	{
		httpDownload.Start();
	}

	public void Stop()
	{
		httpDownload.Stop();
	}

	private void FireVersionEvent(VersionV1EventArgs args)
	{
		this.OnVersionEvent?.BeginInvoke(this, args, null, null);
	}
}
