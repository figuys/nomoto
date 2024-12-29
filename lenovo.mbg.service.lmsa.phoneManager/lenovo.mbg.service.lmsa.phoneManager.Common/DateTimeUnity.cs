using System;

namespace lenovo.mbg.service.lmsa.phoneManager.Common;

public class DateTimeUnity
{
	public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTimeStamp).ToLocalTime();
	}

	public static string getTime(int time)
	{
		string text = "";
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		num3 = time / 1000;
		if (num3 > 60)
		{
			num2 = num3 / 60;
			num3 %= 60;
		}
		if (num2 > 60)
		{
			num = num2 / 60;
			num2 %= 60;
		}
		if (num > 0)
		{
			text = text + num + ":";
		}
		return text + num2 + ":" + num3;
	}
}
