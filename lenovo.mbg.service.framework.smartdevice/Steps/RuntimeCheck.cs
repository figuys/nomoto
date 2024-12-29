using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.common.webservices.WebApiServices;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class RuntimeCheck : BaseStep
{
	protected ApiService service;

	public override void Run()
	{
		service = new ApiService();
		if (base.Recipe.Info.CheckClientVersion)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("country", GlobalFun.GetRegionInfo().TwoLetterISORegionName);
			ToolVersionModel toolVersionModel = service.RequestContent<ToolVersionModel>(WebApiUrl.CLIENT_VERSION, dictionary);
			if (toolVersionModel != null)
			{
				base.Log.AddResult(this, Result.CLIENT_VERSION_LOWER_QUIT, "A newer client version " + toolVersionModel.VersionNumber + " exists.");
				return;
			}
		}
		string text = base.Resources.Get(RecipeResources.Rom);
		if (!Directory.Exists(text))
		{
			base.Log.AddResult(this, Result.ROM_DIRECTORY_NOT_EXISTS, "The ROM package does not exist.");
		}
		else if (!CheckRomFileIsReplace(text))
		{
			base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED_REPLACE, "Resources in the ROM package are replaced");
		}
		else
		{
			base.Log.AddResult(this, Result.PASSED);
		}
	}

	protected bool CheckRomFileIsReplace(string directory)
	{
		string text = Path.Combine(Configurations.DownloadInfoSavePath, Path.GetFileName(directory) + ".check.json.dpapi");
		if (!File.Exists(text))
		{
			base.Log.AddLog("Verification ROM encrypt files does not exist.", upload: true);
			return true;
		}
		string text2 = FileHelper.ReadWithAesDecrypt(text);
		if (string.IsNullOrEmpty(text2))
		{
			base.Log.AddLog("Verify that the encrypted file content of the ROM is empty.", upload: true);
			return true;
		}
		string text3 = base.Resources.Get(RecipeResources.ModelName);
		object obj = service.RequestContent(WebApiUrl.ROMFILE_CHECK_RULES, new Dictionary<string, string> { { "modelName", text3 } });
		List<string> list = new List<string>();
		if (obj != null && obj is JArray { HasValues: not false } jArray)
		{
			list = jArray.Select((JToken n) => n.Value<string>()).ToList();
		}
		JArray jArray2 = JsonHelper.DeserializeJson2JArray(text2);
		bool flag;
		if (list != null && list.Count > 0)
		{
			if (jArray2 != null && jArray2.HasValues)
			{
				StepInfo stepInfo = base.Recipe.Info.Steps.FirstOrDefault((StepInfo n) => n.Step == "CopyFiles");
				List<string> source = new List<string>();
				flag = stepInfo != null;
				if (flag && stepInfo.Args != null && stepInfo.Args.Files != null && stepInfo.Args.Files is JArray)
				{
					source = (from n in ((JArray)stepInfo.Args.Files).SelectMany((JToken n) => n.Value<JArray>("SourceFiles"))
						select n.Value<string>()).Distinct().ToList();
				}
				foreach (string pattern in list)
				{
					List<JToken> list2 = jArray2.Where((JToken n) => Regex.IsMatch(n.Value<string>("Name"), pattern, RegexOptions.IgnoreCase)).ToList();
					if (list2 != null && list2.Count > 0)
					{
						foreach (JToken item in list2)
						{
							string text4 = item.Value<string>("Name");
							if (!source.Contains(text4, StringComparer.InvariantCultureIgnoreCase))
							{
								string text5 = item.Value<string>("Path");
								if (File.Exists(text5))
								{
									FileInfo fileInfo = new FileInfo(text5);
									long num = item.Value<long>("Size");
									long length = fileInfo.Length;
									long num2 = item.Value<long>("LastModifiedTime");
									long num3 = GlobalFun.ToUtcTimeStamp(fileInfo.LastWriteTime);
									if (num == length && num2 == num3)
									{
										continue;
									}
									base.Log.AddLog($"Check update time and size in the ROM package failed, File: {text5}{Environment.NewLine}Source File Size: {num}, Modified Date: {num2}{Environment.NewLine}Current File Size: {length}, Modified Date: {num3}", upload: true);
									flag = false;
								}
								else
								{
									base.Log.AddLog("The " + text5 + " file has been deleted", upload: true);
									flag = false;
								}
								goto IL_07a2;
							}
							base.Log.AddLog(text4 + " will be copied in the Copy step and does not need to be verified", upload: true);
						}
					}
					else
					{
						base.Log.AddLog("No need to verify the '" + pattern + "' type, because there is no such file in the ROM package", upload: true);
					}
				}
			}
			else
			{
				base.Log.AddLog("Verify that the encrypted file content of the ROM is empty.", upload: true);
			}
		}
		else
		{
			base.Log.AddLog("The server does not configure the " + text3 + " ROM file verification type", upload: true);
		}
		return true;
		IL_07a2:
		return flag;
	}
}
