using System;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class PackageDownloadEventArgs : EventArgs
{
	public PackageDownloadModel Package { get; private set; }

	public PackageDownloadEventArgs()
	{
	}

	public PackageDownloadEventArgs(PackageDownloadModel info)
	{
		Package = info;
	}
}
