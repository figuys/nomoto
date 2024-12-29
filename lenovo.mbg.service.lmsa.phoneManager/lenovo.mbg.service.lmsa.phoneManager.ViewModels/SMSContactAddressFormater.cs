using System.Collections.Generic;
using System.Text;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class SMSContactAddressFormater
{
	public static List<string> Format(string addressList)
	{
		if (string.IsNullOrEmpty(addressList))
		{
			return new List<string>();
		}
		addressList += ";";
		List<string> list = new List<string>();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in addressList)
		{
			if (c >= '0' && c <= '9')
			{
				stringBuilder.Append(c);
			}
			if (c == ';' && stringBuilder.Length > 0)
			{
				string item = stringBuilder.ToString();
				if (!list.Contains(item))
				{
					list.Add(item);
				}
				stringBuilder.Clear();
			}
		}
		stringBuilder.Clear();
		return list;
	}

	public static string Format(List<string> addressList)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string address in addressList)
		{
			stringBuilder.Append(address).Append(";");
		}
		return stringBuilder.ToString();
	}
}
