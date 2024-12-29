using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class LoadFiles : BaseStep
{
	public override void Run()
	{
		if (!base.Resources.IsResourceDirExist())
		{
			base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED, "Resources directory does not exist!");
			return;
		}
		foreach (object item in base.Info.Args.Files)
		{
			string text = (string)(dynamic)item;
			string filename = null;
			if (base.Cache.ContainsKey("FastbootMatchFlashFile"))
			{
				filename = base.Cache["FastbootMatchFlashFile"];
			}
			string localFilePath = base.Resources.GetLocalFilePath(text, filename);
			if (!GlobalFun.Exists(localFilePath))
			{
				base.Log.AddResult(this, ("countryCodes" == text) ? Result.LOAD_RESOURCE_FAILED_COUNTRYCODE : Result.LOAD_RESOURCE_FAILED, "resource not found");
				return;
			}
			base.Log.AddLog(text + "=" + localFilePath, upload: true);
			if (!FlashFileCheck(localFilePath))
			{
				base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED, "resource not found");
				return;
			}
			if (!FlashFileMd5Check(localFilePath))
			{
				base.Log.AddLog("Flash file md5 check failed, path is " + localFilePath);
				base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED, "md5 check failed");
				return;
			}
			if (localFilePath.EndsWith("CmdDloader.exe", StringComparison.CurrentCultureIgnoreCase))
			{
				string path = "BMAFrame9.dll";
				string text2 = Path.Combine(new FileInfo(localFilePath).Directory.FullName, path);
				if (!File.Exists(text2))
				{
					File.Copy(Path.Combine(Environment.CurrentDirectory, path), text2, overwrite: true);
				}
			}
			base.Cache[text] = localFilePath;
		}
		if (base.Info.Args.Values != null)
		{
			foreach (object item2 in base.Info.Args.Values)
			{
				string text3 = (string)(dynamic)item2;
				string value = null;
				if (base.Recipe.Device != null)
				{
					value = base.Recipe.Device.Property.GetPropertyValue(text3);
				}
				if (string.IsNullOrEmpty(value))
				{
					string text4 = ProcessRunner.ProcessString(Configurations.FastbootPath, EncapsulationFastbootCommand("getvar " + text3), 5000);
					if (!string.IsNullOrEmpty(text4))
					{
						value = new Regex("(?<key>" + text3 + "):\\s+(?<value>[^\\r]*)", RegexOptions.Multiline).Match(text4).Groups["value"].Value;
					}
				}
				if (string.IsNullOrEmpty(value))
				{
					base.Log.AddLog("cannot found device, " + text3 + " value is null");
				}
				base.Cache[text3] = value;
			}
		}
		base.Log.AddResult(this, Result.PASSED);
	}

	protected bool FlashFileCheck(string filepath)
	{
		if (Regex.IsMatch(filepath, ".*scatter\\.txt$"))
		{
			MatchCollection matchCollection = Regex.Matches(File.ReadAllText(filepath), "(?<key>file_name):\\s+(?<value>.*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			string path = base.Resources.Get(RecipeResources.Rom);
			foreach (Match item in matchCollection)
			{
				string value = item.Groups["value"].Value;
				if (!string.IsNullOrEmpty(value) && !"NONE".Equals(value, StringComparison.CurrentCultureIgnoreCase))
				{
					string text = Path.Combine(path, value);
					if (!File.Exists(text))
					{
						base.Log.AddLog(text + " not exists", upload: true);
						return false;
					}
				}
			}
		}
		return true;
	}

	protected bool FlashFileMd5Check(string xml)
	{
		if (!xml.EndsWith("xml"))
		{
			return true;
		}
		string text = string.Empty;
		try
		{
			text = File.ReadAllText(xml);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(text);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("/flashing/steps");
			string directoryName = Path.GetDirectoryName(xml);
			if (xmlNode != null)
			{
				XmlNodeList xmlNodeList = xmlNode.SelectNodes("step");
				if (xmlNodeList != null)
				{
					foreach (XmlNode item in xmlNodeList)
					{
						XmlNode namedItem;
						string text2 = (((namedItem = item.Attributes.GetNamedItem("filename")) != null) ? namedItem.Value : string.Empty).Trim();
						string text3 = (((namedItem = item.Attributes.GetNamedItem("MD5")) != null) ? namedItem.Value : string.Empty).Trim();
						if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3))
						{
							string text4 = Path.Combine(directoryName, text2);
							if (!File.Exists(text4))
							{
								string log = $"file: {text4} not exist!";
								base.Log.AddLog(log, upload: true);
								return false;
							}
							if (!GlobalFun.MD5Check(text4, text3))
							{
								string log2 = $"file: {text4} md5 check failed";
								base.Log.AddLog(log2, upload: true);
								return false;
							}
						}
					}
				}
			}
		}
		catch (XmlException ex)
		{
			base.Log.AddLog("Error xml content: " + text, upload: true, ex);
			return false;
		}
		catch (Exception ex2)
		{
			base.Log.AddLog("Read xml file for md5 check failed!", upload: true, ex2);
			return false;
		}
		return true;
	}
}
