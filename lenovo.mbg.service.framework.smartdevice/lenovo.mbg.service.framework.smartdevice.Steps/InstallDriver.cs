using System.Collections.Generic;
using System.Threading;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.resources;
using lenovo.mbg.service.framework.services.Download;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class InstallDriver : BaseStep
{
	protected DownloadInfo driverInfo;

	protected AutoResetEvent autoResetEvent = new AutoResetEvent(initialState: false);

	protected List<string> downloads = new List<string>();

	public override void Run()
	{
		string displayPattern = base.Info.Args.DisplayPattern;
		DriversHelper.CheckDriverInstall(displayPattern);
		if (false)
		{
			base.Log.AddResult(this, Result.PASSED);
			return;
		}
		int num = 1;
		Result result = Result.QUIT;
		bool num2 = base.Info.Args.ShowPrePop ?? ((object)false);
		string text = null;
		if (num2)
		{
			num = ((base.Recipe.UcDevice.MessageBox.Show("K0711", "K1606", "K0327", "K0208").Result == true) ? 1 : 0);
		}
		if (num == 1)
		{
			driverInfo = new DownloadInfo
			{
				DownloadUrl = base.Info.Args.DownloadUrl,
				FileType = "ROM"
			};
			HostProxy.DownloadServerV6.OnRemoteDownloadStatusChanged += DownloadServer_OnDownloadStatusChanged;
			HostProxy.DownloadServerV6.Add(driverInfo);
			base.Recipe.UcDevice.MessageBox.ShowDownloadCenter(show: true);
			autoResetEvent.WaitOne();
			base.Recipe.UcDevice.MessageBox.ShowDownloadCenter(show: false);
			HostProxy.DownloadServerV6.OnRemoteDownloadStatusChanged -= DownloadServer_OnDownloadStatusChanged;
			if (downloads.Count > 0)
			{
				List<string> driverPaths = new List<string>();
				downloads.ForEach(delegate(string n)
				{
					Rsd.Instance.GetDownloadedResource(n, out var filePath);
					driverPaths.Add(filePath);
				});
				base.Recipe.UcDevice.MessageBox.SetMainWindowDriverBtnStatus("installing");
				DriversHelper.InstallDriver(driverPaths);
				bool num3 = DriversHelper.CheckDriverInstall(displayPattern);
				base.Recipe.UcDevice.MessageBox.SetMainWindowDriverBtnStatus("installed");
				if (!num3)
				{
					text = "The user exited during the driver installation process";
					base.Log.AddLog(text, upload: true);
				}
				result = (num3 ? Result.PASSED : Result.QUIT);
			}
			else
			{
				string text2 = base.Info.Args.ErrorPromptText?.ToString();
				if (!string.IsNullOrEmpty(text2))
				{
					base.Recipe.UcDevice.MessageBox.Show(base.Name, text2, "K0327", null, showClose: false, MessageBoxImage.Exclamation).Wait();
				}
				text = $"driver {driverInfo.DownloadUrl} download failed, status: {driverInfo.Status}";
				base.Log.AddLog(text, upload: true);
			}
		}
		else
		{
			text = "The user refuses to download driver";
			base.Log.AddLog(text, upload: true);
		}
		base.Log.AddResult(this, result, text);
	}

	private void DownloadServer_OnDownloadStatusChanged(object sender, RemoteDownloadStatusEventArgs e)
	{
		LogHelper.LogInstance.Debug($"install driver download, url is match: {e.FileUrl.Equals(driverInfo.FileUrl)}, status: {e.Status}");
		if (e.FileUrl.Equals(driverInfo.FileUrl))
		{
			switch (e.Status)
			{
			case DownloadStatus.SUCCESS:
			case DownloadStatus.ALREADYEXISTS:
			case DownloadStatus.UNZIPSUCCESS:
				downloads.Add(driverInfo.FileUrl);
				autoResetEvent.Set();
				break;
			default:
				autoResetEvent.Set();
				break;
			case DownloadStatus.WAITTING:
			case DownloadStatus.DOWNLOADING:
			case DownloadStatus.AUTO_PAUSE:
			case DownloadStatus.MANUAL_PAUSE:
			case DownloadStatus.UNZIPPING:
				break;
			}
		}
	}
}
