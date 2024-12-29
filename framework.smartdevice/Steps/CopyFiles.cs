using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class CopyFiles : BaseStep
{
	private string failedResponse;

	public override void Run()
	{
		JArray jArray = base.Info.Args.Files;
		SortedList<string, string> sortedList = new SortedList<string, string>();
		if (jArray != null && jArray.HasValues)
		{
			foreach (JToken item in jArray)
			{
				string text = item.Value<string>("SourcePath");
				if (string.IsNullOrEmpty(text))
				{
					failedResponse = "SourcePath format error";
					break;
				}
				string text2 = item.Value<string>("DestPath");
				if (string.IsNullOrEmpty(text2))
				{
					failedResponse = "DestPath format error";
					break;
				}
				JArray jArray2 = item.Value<JArray>("SourceFiles");
				if (jArray2 == null || !jArray2.HasValues)
				{
					failedResponse = "SourceFiles format error";
					break;
				}
				List<string> list = jArray2.Values<string>().ToList();
				List<string> list2 = null;
				int count = list.Count;
				int num = -1;
				JArray jArray3 = item.Value<JArray>("DestFiles");
				if (jArray3 != null && jArray3.HasValues)
				{
					list2 = jArray3.Values<string>().ToList();
					num = list2.Count;
				}
				if (text.StartsWith("$"))
				{
					string key = text.Substring(1);
					text = base.Cache[key];
				}
				if (text2.StartsWith("$"))
				{
					string key2 = text2.Substring(1);
					text2 = base.Cache[key2];
				}
				for (int i = 0; i < count; i++)
				{
					string text3 = Path.Combine(text, list[i]);
					string value = Path.Combine(text2, list[i]);
					if (num > i)
					{
						value = Path.Combine(text2, list2[i]);
					}
					if (!File.Exists(text3))
					{
						failedResponse = text3 + " not exists";
						sortedList.Clear();
						break;
					}
					sortedList.Add(text3, value);
				}
			}
			foreach (KeyValuePair<string, string> item2 in sortedList)
			{
				File.Copy(item2.Key, item2.Value, overwrite: true);
			}
		}
		base.Log.AddResult(this, string.IsNullOrEmpty(failedResponse) ? Result.PASSED : Result.COPYFILES_FAILED, failedResponse);
	}
}
