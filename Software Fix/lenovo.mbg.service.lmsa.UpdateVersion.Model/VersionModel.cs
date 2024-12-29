using System;
using lenovo.mbg.service.framework.download.DownloadUnity;

namespace lenovo.mbg.service.lmsa.UpdateVersion.Model;

public class VersionModel : AbstractDownloadInfo
{
	public string VersionName { get; set; }

	public string DisplayName { get; set; }

	public string Version { get; set; }

	public bool ForceType { get; set; }

	public bool haveNewVersion { get; set; }

	public override string downloadFileName { get; set; }

	public override long downloadFileSize { get; set; }

	public override int downloadLevel { get; set; }

	public override string downloadMD5 { get; set; }

	public override string downloadUrl { get; set; }

	public override string saveLocalPath { get; set; }

	public DateTime? ReleaseDate { get; set; }

	public string ReleaseNotes { get; set; }
}
