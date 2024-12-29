using System;
using System.Collections.Generic;
using System.IO;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class CopyLogs : BaseStep
{
	public override void Run()
	{
		string text = base.Info.Args.LogLocation;
		string text2 = base.Info.Args.LogSubDir;
		string searchPattern = base.Info.Args.SearchPattern ?? "*.log";
		bool flag = base.Info.Args.LatestSubDir ?? ((object)false);
		if (text.StartsWith("$"))
		{
			if (!base.Cache.ContainsKey(text.Substring(1)))
			{
				base.Log.AddResult(this, Result.SKIPPED, "LogLocation configuration error in recipe file");
				return;
			}
			text = base.Cache[text.Substring(1)];
			text = Path.GetDirectoryName(text);
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text = Path.Combine(text, text2);
		}
		if (flag)
		{
			text = GlobalFun.GetLasterDirectory(text);
		}
		List<string> allFiles = GlobalFun.GetAllFiles(text, searchPattern);
		Result result = Result.PASSED;
		if (allFiles != null && allFiles.Count > 0)
		{
			try
			{
				string tmpfolder = Path.Combine(Path.GetTempPath(), "lmsatemp");
				if (!Directory.Exists(tmpfolder))
				{
					Directory.CreateDirectory(tmpfolder);
				}
				else
				{
					GlobalFun.DeleteFileInDirectory(tmpfolder);
				}
				LogHelper.LogInstance.Info("Copy files to dir: " + tmpfolder, upload: true);
				allFiles.ForEach(delegate(string n)
				{
					File.Copy(n, Path.Combine(tmpfolder, new FileInfo(n).Name), overwrite: true);
				});
			}
			catch (Exception ex)
			{
				result = Result.COPYLOGS_FAILED;
				base.Log.AddLog("copy tool log file error!", upload: true, ex);
			}
		}
		else
		{
			LogHelper.LogInstance.Info("No log file need to copy!", upload: true);
		}
		base.RunResult = result.ToString();
		base.Log.AddResult(this, result);
	}
}
