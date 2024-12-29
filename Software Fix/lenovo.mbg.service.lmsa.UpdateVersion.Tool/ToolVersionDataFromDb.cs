using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;
using lenovo.mbg.service.lmsa.UpdateVersion.Proxy;

namespace lenovo.mbg.service.lmsa.UpdateVersion.Tool;

public class ToolVersionDataFromDb : ToolVersionDataFromJson
{
	private UpdateVersionService m_updateVersionProxy;

	private UpdateVersionService updateVersionProxy
	{
		get
		{
			if (m_updateVersionProxy == null)
			{
				m_updateVersionProxy = new UpdateVersionService();
			}
			return m_updateVersionProxy;
		}
	}

	public override object GetData()
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("country", GlobalFun.GetRegionInfo().TwoLetterISORegionName);
			ToolVersionModel clientVersion = updateVersionProxy.GetClientVersion(dictionary);
			if (clientVersion != null)
			{
				return new VersionModel
				{
					Version = clientVersion.VersionNumber,
					downloadUrl = clientVersion.FilePath,
					downloadMD5 = clientVersion.MD5,
					ForceType = clientVersion.IsForce,
					downloadFileSize = clientVersion.FileSize,
					downloadFileName = FileNameAnalysis(clientVersion.FilePath),
					saveLocalPath = ApplcationClass.DownloadPath,
					ReleaseNotes = clientVersion.ReleaseNotes,
					ReleaseDate = QrCodeUtility.ConvertDateTime(clientVersion.ReleaseDate)
				};
			}
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.lmsa.UpdateVersion.Tool.ToolVersionDataFromDb.GetData: Get Tool Version Data Failed", exception);
		}
		return null;
	}

	private string FileNameAnalysis(string data)
	{
		if (!string.IsNullOrEmpty(data))
		{
			string[] array = Regex.Split(data.Split('?')[0], "\\\\|/");
			if (array != null && array.Length != 0)
			{
				return array[array.Length - 1];
			}
		}
		return string.Empty;
	}
}
