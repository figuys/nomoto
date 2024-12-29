using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.framework.socket;

public class Header
{
	private Dictionary<string, string> headers;

	public Dictionary<string, string> Headers => headers;

	public Header(Dictionary<string, string> headers)
	{
		this.headers = headers;
	}

	public Header()
	{
		headers = new Dictionary<string, string>();
	}

	public bool ContainsKey(string key)
	{
		return Headers.ContainsKey(key);
	}

	public bool ContainsAndEqual(string key, string val)
	{
		string @string = GetString(key);
		if (val == null && @string == null)
		{
			return true;
		}
		return val?.Equals(@string) ?? false;
	}

	public void AddOrReplace(string key, string value)
	{
		headers[key] = value;
	}

	public string GetString(string key, string defaultVal = null)
	{
		string value = defaultVal;
		headers.TryGetValue(key, out value);
		return value;
	}

	public int GetInt32(string key, int defaultVal = 0)
	{
		string @string = GetString(key);
		int result = defaultVal;
		if (!string.IsNullOrEmpty(@string))
		{
			int.TryParse(@string, out result);
		}
		return result;
	}

	public long GetInt64(string key, long defaultVal = 0L)
	{
		string @string = GetString(key);
		long result = defaultVal;
		if (!string.IsNullOrEmpty(@string))
		{
			long.TryParse(@string, out result);
		}
		return result;
	}

	public bool GetBoolean(string key, bool defaultVal = false)
	{
		string @string = GetString(key);
		bool result = defaultVal;
		if (!string.IsNullOrEmpty(@string))
		{
			bool.TryParse(@string, out result);
		}
		return result;
	}

	public DateTime GetDateTime(string key, DateTime defaultVal)
	{
		string @string = GetString(key);
		DateTime result = defaultVal;
		if (!string.IsNullOrEmpty(@string))
		{
			DateTime.TryParse(@string, out result);
		}
		return result;
	}

	public void Clear()
	{
		headers.Clear();
	}
}
