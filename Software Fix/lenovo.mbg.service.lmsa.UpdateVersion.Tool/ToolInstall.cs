using System;
using System.Diagnostics;
using System.IO;
using lenovo.mbg.service.framework.updateversion;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;

namespace lenovo.mbg.service.lmsa.UpdateVersion.Tool;

public class ToolInstall : IVersionInstall
{
	public event EventHandler<VersionInstallEventArgs> OnInstallStatusChanged;

	public void Cancel()
	{
	}

	public void InstallVersion(object data)
	{
		try
		{
			if (data == null)
			{
				return;
			}
			this.OnInstallStatusChanged?.Invoke(null, null);
			if (!(data is VersionModel versionModel))
			{
				ApplcationClass.IsUpdatingPlug = false;
				return;
			}
			string text = Path.Combine(ApplcationClass.DownloadPath, versionModel.downloadFileName);
			if (!File.Exists(text))
			{
				ApplcationClass.IsUpdatingPlug = false;
				return;
			}
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = text;
			processStartInfo.UseShellExecute = false;
			processStartInfo.CreateNoWindow = true;
			processStartInfo.Verb = "runas";
			Process.Start(processStartInfo);
			Environment.Exit(0);
		}
		catch (Exception)
		{
			ApplcationClass.IsUpdatingPlug = false;
		}
	}
}
