using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.framework.lang;

namespace lenovo.mbg.service.lmsa;

public class AppContext
{
	public static bool IsLogIn;

	private volatile bool isDisposing = false;

	public static AppContext Single { get; private set; }

	public static ApiService WebApi { get; private set; }

	public Dispatcher CurrentDispatcher { get; set; }

	public bool IsDisposing => isDisposing;

	private AppContext()
	{
		CurrentDispatcher = Dispatcher.CurrentDispatcher;
	}

	public static void Init()
	{
		Single = new AppContext();
		WebApi = new ApiService();
		LangTranslation.SetService(global::Smart.LanguageService);
		global::Smart.LanguageService.LoadCommLanguagePackage();
		Task.Run(delegate
		{
			Single.CheckMAVersion();
		});
	}

	private void CheckMAVersion()
	{
		try
		{
			var parameter = new
			{
				versionCode = Configurations.AppVersionCode
			};
			MAPackageVersion mAPackageVersion = WebApi.RequestContent<MAPackageVersion>(WebApiUrl.CHECK_MA_VERSION, parameter);
			if (string.IsNullOrEmpty(mAPackageVersion?.apkUrl))
			{
				return;
			}
			string _downloadFileName = Path.Combine(ApplcationClass.DownloadPath, "MaAppNew");
			LogHelper.LogInstance.Info("Begin download new MA url:[" + mAPackageVersion.apkUrl + "].");
			GlobalFun.TryDeleteFile(_downloadFileName);
			if (GlobalFun.GetStreamFormServer(mAPackageVersion.apkUrl, delegate(Stream stream)
			{
				using FileStream fileStream = new FileStream(_downloadFileName, FileMode.Create);
				stream.CopyTo(fileStream);
				fileStream.Flush();
			}))
			{
				LogHelper.LogInstance.Info("Begin check new MA Md5.");
				string md5Hash = GlobalFun.GetMd5Hash(_downloadFileName);
				if (mAPackageVersion.md5.ToUpper().Equals(md5Hash.ToUpper()))
				{
					LogHelper.LogInstance.Info($"Replace the new MA and record the new version code:[{mAPackageVersion.versionCode}].");
					string destFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "apk", "lmsa.apk");
					for (int i = 0; i < 3; i++)
					{
						try
						{
							File.Copy(_downloadFileName, destFileName, overwrite: true);
							Configurations.AppVersionCode = mAPackageVersion.versionCode;
							break;
						}
						catch (Exception)
						{
							Thread.Sleep(3000);
						}
					}
				}
				else
				{
					LogHelper.LogInstance.Warn($"Check new MA Md5 failed! Local md5:[{md5Hash}], size:[{new FileInfo(_downloadFileName).Length}], server md5:[{mAPackageVersion.md5}], size:[{mAPackageVersion.size}].");
				}
			}
			else
			{
				LogHelper.LogInstance.Warn("Download the new MA url:[" + mAPackageVersion.apkUrl + "] failed.");
			}
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"Download the new MA error. Exception:[{arg}].");
		}
	}

	public void Close()
	{
	}

	public void Dispose()
	{
	}

	public void Clear()
	{
		Dispose();
	}
}
