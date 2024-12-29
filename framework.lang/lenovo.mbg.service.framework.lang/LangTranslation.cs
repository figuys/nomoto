using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.framework.lang;

public class LangTranslation
{
	private static ILanguage languageService;

	public static void SetService(ILanguage service)
	{
		languageService = service;
	}

	public static string Translate(string lang)
	{
		if (languageService == null)
		{
			return lang;
		}
		if (languageService.IsNeedTranslate())
		{
			return languageService.Translate(lang);
		}
		return lang;
	}
}
