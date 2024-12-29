using System;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.UpdateVersion.Core;

public class UpgradeUserOptionManager
{
	public const string REMIND_TYPE_DATA_STORE_KEY = "{CF3ED816-2454-41C8-B5B5-633570481663}";

	public UpgradeRemindType GetRemindType(string newVersion)
	{
		string mainProcessVersion = LMSAContext.MainProcessVersion;
		UpgradeRemindTypeInfo upgradeRemindTypeInfo = JsonHelper.DeserializeJson2Object<UpgradeRemindTypeInfo>(FileHelper.ReadWithAesDecrypt(Configurations.DefaultOptionsFileName, "{CF3ED816-2454-41C8-B5B5-633570481663}"));
		if (upgradeRemindTypeInfo != null && upgradeRemindTypeInfo.NewVersion.Equals(newVersion))
		{
			return upgradeRemindTypeInfo.RemindType;
		}
		return UpgradeRemindType.None;
	}

	public bool IsRemindToday(string newVersion)
	{
		string mainProcessVersion = LMSAContext.MainProcessVersion;
		UpgradeRemindTypeInfo upgradeRemindTypeInfo = JsonHelper.DeserializeJson2Object<UpgradeRemindTypeInfo>(FileHelper.ReadWithAesDecrypt(Configurations.DefaultOptionsFileName, "{CF3ED816-2454-41C8-B5B5-633570481663}"));
		if (upgradeRemindTypeInfo != null)
		{
			switch (upgradeRemindTypeInfo.RemindType)
			{
			case UpgradeRemindType.None:
				return true;
			case UpgradeRemindType.RemindTomorrow:
			{
				DateTime now = DateTime.Now;
				DateTime setDate = upgradeRemindTypeInfo.SetDate;
				return now.Year > setDate.Year || now.Month > setDate.Month || now.Day > setDate.Day;
			}
			case UpgradeRemindType.NotRemindForTheCurrentVersion:
				return !newVersion.Equals(upgradeRemindTypeInfo.NewVersion);
			}
		}
		return true;
	}

	public void SaveUpgradeRemindType(UpgradeRemindTypeInfo upgradeRemindTypeInfo)
	{
		if (upgradeRemindTypeInfo != null)
		{
			string data = JsonHelper.SerializeObject2FormatJson(upgradeRemindTypeInfo);
			FileHelper.WriteJsonWithAesEncrypt(Configurations.DefaultOptionsFileName, "{CF3ED816-2454-41C8-B5B5-633570481663}", data);
		}
	}
}
