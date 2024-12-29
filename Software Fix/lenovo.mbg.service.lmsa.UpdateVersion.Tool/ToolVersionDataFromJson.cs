using System;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.updateversion;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.UpdateVersion.Tool;

public class ToolVersionDataFromJson : IVersionData
{
	public virtual object GetData()
	{
		VersionModel versionModel = new VersionModel();
		try
		{
			string stringFromUrl = GlobalFun.GetStringFromUrl(ApplcationClass.ToolNewVersionConfig);
			JObject jObject = JsonHelper.DeserializeJson2Jobjcet(stringFromUrl);
			if (jObject != null && jObject.HasValues)
			{
				foreach (JProperty item in jObject.Properties())
				{
					versionModel.saveLocalPath = ApplcationClass.DownloadPath;
					switch (item.Name.ToUpper())
					{
					case "VERSIONNAME":
						versionModel.VersionName = item.Value.ToString();
						break;
					case "VERSION":
						versionModel.Version = item.Value.ToString();
						break;
					case "VERSIONURL":
						versionModel.downloadUrl = item.Value.ToString();
						break;
					case "MD5":
						versionModel.downloadMD5 = item.Value.ToString();
						break;
					case "FORCETYPE":
					{
						bool result2 = false;
						bool.TryParse(item.Value.ToString(), out result2);
						versionModel.ForceType = result2;
						break;
					}
					case "SIZE":
					{
						long result = 0L;
						long.TryParse(item.Value.ToString(), out result);
						versionModel.downloadFileSize = result;
						break;
					}
					case "FILENAME":
						versionModel.downloadFileName = item.Value.ToString();
						break;
					}
				}
			}
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.lmsa.UpdateVersion.Tool.ToolVersionDataFromJson.GetData: Get Tool Version Data Failed", exception);
			return null;
		}
		return versionModel;
	}

	public void UpdateData(object data)
	{
	}
}
