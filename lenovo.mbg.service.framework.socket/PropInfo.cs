using System;
using System.Collections.Generic;
using System.Linq;

namespace lenovo.mbg.service.framework.socket;

public class PropInfo
{
	public List<PropItem> Props { get; private set; }

	public PropInfo()
	{
		Props = new List<PropItem>();
	}

	public bool AddOrUpdateProp(List<PropItem> props)
	{
		foreach (PropItem prop in props)
		{
			AddOrUpdateProp(prop);
		}
		return true;
	}

	public bool AddOrUpdateProp(PropItem prop)
	{
		PropItem propItem = Props.FirstOrDefault((PropItem n) => n.Key.Equals(prop.Key));
		if (propItem != null)
		{
			propItem.Value = prop.Value;
		}
		else
		{
			Props.Add(prop);
		}
		return true;
	}

	public bool AddOrUpdateProp(string jsonProp)
	{
		if (!string.IsNullOrEmpty(jsonProp))
		{
			List<PropItem> list = JsonUtils.Parse<List<PropItem>>(jsonProp);
			if (list != null)
			{
				AddOrUpdateProp(list);
				return true;
			}
		}
		return false;
	}

	public bool AddOrUpdateProp(string jsonProp, Dictionary<string, string> replaceKey)
	{
		if (!string.IsNullOrEmpty(jsonProp))
		{
			List<PropItem> list = JsonUtils.Parse<List<PropItem>>(jsonProp);
			if (list != null)
			{
				foreach (KeyValuePair<string, string> item in replaceKey)
				{
					foreach (PropItem item2 in list.Where((PropItem m) => m.Key == item.Key))
					{
						item2.Key = item.Value;
					}
				}
			}
			AddOrUpdateProp(list);
		}
		return false;
	}

	public bool AddOrUpdateProp(List<PropItem> items, Dictionary<string, string> replaceKey)
	{
		if (items != null)
		{
			foreach (KeyValuePair<string, string> item in replaceKey)
			{
				foreach (PropItem item2 in items.Where((PropItem m) => m.Key == item.Key))
				{
					item2.Key = item.Value;
				}
			}
		}
		AddOrUpdateProp(items);
		return false;
	}

	public void Reset(string key, string value)
	{
		PropItem propItem = Props.Where((PropItem m) => m.Key.Equals(key)).FirstOrDefault();
		if (propItem != null)
		{
			propItem.Value = value;
		}
	}

	public string GetProp(string property)
	{
		if (Props.Count == 0)
		{
			return string.Empty;
		}
		return (from m in Props
			where m.Key.Equals(property)
			select m.Value).FirstOrDefault();
	}

	public long GetLongProp(string progerty)
	{
		long result = 0L;
		string prop = GetProp(progerty);
		if (!string.IsNullOrEmpty(prop))
		{
			long.TryParse(prop, out result);
		}
		return result;
	}

	public int GetIntProp(string progerty)
	{
		int result = 0;
		string prop = GetProp(progerty);
		if (!string.IsNullOrEmpty(prop))
		{
			int.TryParse(prop, out result);
		}
		return result;
	}

	public DateTime GetDateTimeProp(string progerty)
	{
		DateTime result = DateTime.MinValue;
		string prop = GetProp(progerty);
		if (!string.IsNullOrEmpty(prop))
		{
			DateTime.TryParse(prop, out result);
		}
		return result;
	}
}
