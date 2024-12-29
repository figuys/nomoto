using System;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class FlashContext
{
	private static FlashContext _singleInstance;

	public ApiService service { get; private set; }

	public static FlashContext SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new FlashContext();
			}
			return _singleInstance;
		}
	}

	public DownloadManager DownloadManager { get; set; }

	public ILanguage LanuageService { get; private set; }

	public Action<object> TutorialClickAction { get; set; }

	private FlashContext()
	{
	}

	public void Initialize()
	{
		service = new ApiService();
		DownloadManager downloadManager = new DownloadManager();
		downloadManager.Initialize();
		DownloadManager = downloadManager;
		LanuageService = HostProxy.LanguageService;
	}
}
