using System.Collections.Generic;
using System.Linq;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

public class ContactMergeHandler
{
	public static void Merge(IList<IContact> orignalContacts, out IDictionary<string, IList<IContact>> mergedContacts)
	{
		mergedContacts = new Dictionary<string, IList<IContact>>();
		new List<int>();
		int count = orignalContacts.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			num = 0;
			for (int j = 0; j < mergedContacts.Count; j++)
			{
				KeyValuePair<string, IList<IContact>> keyValuePair = mergedContacts.ElementAt(j);
				string phoneNumber = orignalContacts[i].PhoneNumber;
				string key = keyValuePair.Key;
				int num2 = key.Length - 1;
				int num3 = phoneNumber.Length - 1;
				while (num2 >= 0 && num3 >= 0 && key[num2].Equals(phoneNumber[num3]))
				{
					num2--;
					num3--;
				}
				if (num2 < 0 || num3 < 0)
				{
					num++;
					keyValuePair.Value.Add(orignalContacts[i]);
					break;
				}
			}
			if (num == 0)
			{
				mergedContacts[orignalContacts[i].PhoneNumber] = new List<IContact> { orignalContacts[i] };
			}
		}
	}

	public static List<SMSContactMerged> Merge(IList<IContact> orignalContacts)
	{
		IDictionary<string, SMSContactMerged> dictionary = new Dictionary<string, SMSContactMerged>();
		new List<int>();
		int count = orignalContacts.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			num = 0;
			for (int j = 0; j < dictionary.Count; j++)
			{
				KeyValuePair<string, SMSContactMerged> keyValuePair = dictionary.ElementAt(j);
				string phoneNumber = orignalContacts[i].PhoneNumber;
				string key = keyValuePair.Key;
				int num2 = key.Length - 1;
				int num3 = phoneNumber.Length - 1;
				while (num2 >= 0 && num3 >= 0 && key[num2].Equals(phoneNumber[num3]))
				{
					num2--;
					num3--;
				}
				if (num2 < 0 || num3 < 0)
				{
					num++;
					keyValuePair.Value.MergedList.Add(orignalContacts[i]);
					break;
				}
			}
			if (num == 0)
			{
				SMSContactMerged sMSContactMerged = new SMSContactMerged();
				sMSContactMerged.MergedList.Add(orignalContacts[i]);
				dictionary[orignalContacts[i].PhoneNumber] = sMSContactMerged;
			}
		}
		return dictionary.Values.ToList();
	}
}
