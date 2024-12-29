using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.hostproxy;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.Language;

public class LanguageHelper : ILanguage
{
	private Action notifyChange;

	private ResourceDictionary enLang;

	private ResourceDictionary curLang;

	private ResourceDictionary updateLang;

	private Regex _regex = new Regex("^[\0-\u007f‘’]*$");

	private Regex _regexOnlyNum = new Regex("^[0-9 /,.+!@#$%^&*()]+$");

	private List<string> haveRecordNoTranslateLanguageList = new List<string>();

	private readonly object haveRecordNoTranslateLanguageListLock = new object();

	public NotifyEventProxy LanguageChangedAction { get; set; }

	public Action OnNotifyLanguageChangeAction
	{
		get
		{
			return notifyChange;
		}
		set
		{
			notifyChange = value;
		}
	}

	public bool IsNeedTranslate()
	{
		return true;
	}

	public string GetLanguageRootDir()
	{
		return Path.Combine(LMSAContext.LanguagePackageRootPath, LMSAContext.CurrentLanguage);
	}

	public string GetCurrentLanguage()
	{
		return LMSAContext.CurrentLanguage;
	}

	public void SetCurrentLanguage(string selectLanguage)
	{
		LMSAContext.SetCurrentLanguage(selectLanguage);
	}

	public string Translate(string srcLanguage)
	{
		if (string.IsNullOrEmpty(srcLanguage) || !IsNeedTranslate() || curLang == null || !_regex.IsMatch(srcLanguage))
		{
			return srcLanguage;
		}
		if (_regexOnlyNum.IsMatch(srcLanguage))
		{
			return srcLanguage;
		}
		if (updateLang != null)
		{
			foreach (object key in updateLang.Keys)
			{
				if (srcLanguage.Equals(key.ToString()))
				{
					srcLanguage = updateLang[key].ToString();
				}
			}
		}
		if (Regex.IsMatch(srcLanguage, "^K\\d{4}"))
		{
			if (curLang.Contains(srcLanguage))
			{
				return curLang[srcLanguage].ToString();
			}
		}
		else
		{
			foreach (object key2 in enLang.Keys)
			{
				if (enLang[key2].Equals(srcLanguage) && curLang.Contains(key2))
				{
					return curLang[key2].ToString();
				}
			}
		}
		RecordNoTranslate(srcLanguage);
		return srcLanguage;
	}

	private void RecordNoTranslate(string source)
	{
		lock (haveRecordNoTranslateLanguageListLock)
		{
			if (haveRecordNoTranslateLanguageList.Contains(source))
			{
				return;
			}
			haveRecordNoTranslateLanguageList.Add(source);
			if (!"TRUE".Equals(ConfigurationManager.AppSettings["RecordNoTranslate"], StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			try
			{
				string currentLanguage = GetCurrentLanguage();
				string contents = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [Target:" + currentLanguage + "] " + source + Environment.NewLine;
				File.AppendAllText("NoTranslateLog.txt", contents);
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Record no translate string throw exception:" + ex.ToString());
			}
		}
	}

	public void FeedbackNoTranslate()
	{
		if (haveRecordNoTranslateLanguageList.Count > 0 && false)
		{
			LogHelper.LogInstance.Info(string.Format("Feed back untranslated sentences. Count: [{0}]. Content:[{1}]", haveRecordNoTranslateLanguageList.Count, string.Join(", ", haveRecordNoTranslateLanguageList.ToArray())));
			JObject jObject = new JObject();
			jObject.Add("content", JToken.FromObject(haveRecordNoTranslateLanguageList));
			AppContext.WebApi.RequestContent(WebApiUrl.FEEDBACK_NO_TRANSLATE, jObject);
		}
	}

	public void LoadCommLanguagePackage()
	{
		enLang = LoadLanguagePackage(LMSAContext.DEF_LANGUAGE);
		if (LMSAContext.CurrentLanguage == LMSAContext.DEF_LANGUAGE)
		{
			curLang = enLang;
		}
		else
		{
			curLang = LoadLanguagePackage(LMSAContext.CurrentLanguage);
		}
	}

	private ResourceDictionary LoadLanguagePackage(string langCode)
	{
		string text = Path.Combine(LMSAContext.LanguagePackageRootPath, langCode);
		if (!Directory.Exists(text))
		{
			LogHelper.LogInstance.Error("Call LoadLanguagePackage() occur error! language pack directory is not exist.");
			return enLang;
		}
		string text2 = Path.Combine(LMSAContext.LanguagePackageRootPath, LMSAContext.DEF_LANGUAGE, LMSAContext.UPDATE_XML_FILE);
		if (File.Exists(text2))
		{
			updateLang = ReadLanguagePackage(text2);
		}
		try
		{
			string langFile = Path.Combine(text, langCode);
			return ReadLanguagePackage(langFile);
		}
		catch (Exception ex)
		{
			string text3 = ((ex.InnerException == null) ? ex.Message : ex.InnerException.Message);
			LogHelper.LogInstance.Error("Call LoadLanguagePackage() occur error! error info: " + text3);
			return enLang;
		}
	}

	private ResourceDictionary ReadLanguagePackage(string langFile)
	{
		if (File.Exists(langFile))
		{
			using (FileStream stream = File.Open(langFile, FileMode.Open))
			{
				return (ResourceDictionary)XamlReader.Load(stream);
			}
		}
		LogHelper.LogInstance.Error("Call ReadLanguagePackage() occur error! Language File not exists: [" + langFile + "]");
		return null;
	}

	public RegionInfo GetCurrentRegionInfo()
	{
		return GlobalFun.GetRegionInfo();
	}

	public bool IsChinaRegionAndLanguage()
	{
		RegionInfo currentRegionInfo = HostProxy.LanguageService.GetCurrentRegionInfo();
		string currentLanguage = HostProxy.LanguageService.GetCurrentLanguage();
		return "CHN".Equals(currentRegionInfo.ThreeLetterISORegionName) && "zh-CN".Equals(currentLanguage, StringComparison.OrdinalIgnoreCase);
	}
}
