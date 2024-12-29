using System;
using System.IO;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.Language;

public class LanguageUpdateHelper
{
	private readonly string LANG_PACKAGE_INFO = Configurations.ServiceInterfaceUrl + "/client/languagePack.jhtml";

	private static LanguageUpdateHelper instance;

	public static LanguageUpdateHelper Instance => instance ?? (instance = new LanguageUpdateHelper());

	private LanguageUpdateHelper()
	{
	}

	public void CheckNewLanguagePackage()
	{
		LanguagePackageVersion languagePackageVersion = AppContext.WebApi.RequestContent<LanguagePackageVersion>(LANG_PACKAGE_INFO, null);
		if (languagePackageVersion == null || languagePackageVersion.languagePackVersion <= Configurations.LanguagePackageVersion)
		{
			return;
		}
		try
		{
			string text = LMSAContext.LanguagePackageRootPath + "_bak";
			if (Directory.Exists(LMSAContext.LanguagePackageRootPath))
			{
				GlobalFun.DeleteDirectory(text);
				Directory.Move(LMSAContext.LanguagePackageRootPath, text);
			}
			if (DownloadLatestVersion(languagePackageVersion.languageFile, LMSAContext.LanguagePackageRootPath))
			{
				Configurations.LanguagePackageVersion = languagePackageVersion.languagePackVersion;
				GlobalFun.DeleteDirectory(text);
			}
			else
			{
				Directory.Move(text, LMSAContext.LanguagePackageRootPath);
			}
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Call CheckNewLanguagePackage() occur error, info: " + ex.ToString());
		}
	}

	private bool DownloadLatestVersion(string _url, string saveDir)
	{
		try
		{
			long filesize = 0L;
			GlobalFun.GetFileSize(_url, out filesize);
			if (filesize == 0)
			{
				return false;
			}
			string _languageZipPath = Path.Combine(Configurations.TempDir, "language_package.zip");
			bool streamFormServer = GlobalFun.GetStreamFormServer(_url, delegate(Stream stream)
			{
				using FileStream fileStream = new FileStream(_languageZipPath, FileMode.OpenOrCreate);
				stream.CopyTo(fileStream);
				fileStream.Flush();
			});
			if (streamFormServer)
			{
				SevenZipHelper.Instance.Extractor(_languageZipPath, saveDir);
			}
			return streamFormServer;
		}
		catch (Exception ex)
		{
			string message = ((ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
			LogHelper.LogInstance.Error(message);
			return false;
		}
	}
}
