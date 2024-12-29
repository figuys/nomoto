using System.Globalization;

namespace lenovo.mbg.service.framework.services;

public interface ILanguage
{
	string GetLanguageRootDir();

	RegionInfo GetCurrentRegionInfo();

	string GetCurrentLanguage();

	bool IsNeedTranslate();

	string Translate(string soreceLanguage);

	void LoadCommLanguagePackage();

	void SetCurrentLanguage(string selectLanguage);

	bool IsChinaRegionAndLanguage();

	void FeedbackNoTranslate();
}
