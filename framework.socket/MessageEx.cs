using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.framework.socket;

public class MessageEx<T>
{
	[JsonIgnore]
	private long _longSequecne = -1L;

	[JsonProperty("action")]
	public string Action { get; set; }

	[JsonProperty("sequence")]
	public string Sequence { get; set; }

	[JsonIgnore]
	public long LongSequence
	{
		get
		{
			if (_longSequecne != -1)
			{
				return _longSequecne;
			}
			long.TryParse(Sequence, out _longSequecne);
			return _longSequecne;
		}
	}

	[JsonProperty("params")]
	public List<T> Data { get; set; }

	public string GetJsonData()
	{
		return Convert.ToString(Data);
	}
}
