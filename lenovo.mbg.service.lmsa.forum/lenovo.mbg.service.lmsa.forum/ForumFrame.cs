using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DotNetBrowser.Navigation;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.forum;

public partial class ForumFrame : UserControl, IComponentConnector
{
	private static Dictionary<string, Dictionary<string, string>> lenovoPhonesUrlMapping = new Dictionary<string, Dictionary<string, string>>
	{
		{
			"lenovoPhone",
			new Dictionary<string, string>
			{
				{ "en-US", "https://forums.lenovo.com/t5/Lenovo-Phones/ct-p/lp_en" },
				{ "es-ES", "https://forums.lenovo.com/t5/Smartphones-Lenovo/ct-p/lp_es" },
				{ "pt-BR", "https://forums.lenovo.com/t5/Telefones-Lenovo/ct-p/phones_pt" },
				{ "pl-PL", "https://forums.lenovo.com/t5/Smartfony-Lenovo/ct-p/lp_pl" },
				{ "ru-RU", "https://forums.lenovo.com/t5/%D0%A1%D0%BC%D0%B0%D1%80%D1%82%D1%84%D0%BE%D0%BD%D1%8B/ct-p/lp_ru" }
			}
		},
		{
			"motoPhone",
			new Dictionary<string, string>
			{
				{ "en-US", "https://forums.lenovo.com/t5/Motorola-Community/ct-p/MotorolaCommunity?profile.language=en" },
				{ "es-ES", "https://forums.lenovo.com/t5/Comunidad-Motorola/ct-p/ComunidadMotorola?profile.language=es" },
				{ "pt-BR", "https://forums.lenovo.com/t5/Comunidade-Motorola/ct-p/ComunidadeMotorola" },
				{ "ru-RU", "https://forums.lenovo.com/t5/%D0%A1%D0%BE%D0%BE%D0%B1%D1%89%D0%B5%D1%81%D1%82%D0%B2%D0%BE-%D0%9C%D0%BE%D1%82%D0%BE%D1%80%D0%BE%D0%BB%D0%B0/ct-p/moto_community_ru" }
			}
		},
		{
			"lenovoTablet",
			new Dictionary<string, string>
			{
				{ "en-US", "https://forums.lenovo.com/t5/Lenovo-Tablets/ct-p/lt_en" },
				{ "es-ES", "https://forums.lenovo.com/t5/Tablets-Lenovo-IdeaPad-IdeaTab/ct-p/lt_es" },
				{ "pt-BR", "https://forums.lenovo.com/t5/Tablets-Lenovo/ct-p/lt_pt" },
				{ "pl-PL", "https://forums.lenovo.com/t5/Tablety-Lenovo/ct-p/lt_pl" },
				{ "ru-RU", "https://forums.lenovo.com/t5/%D0%9F%D0%BB%D0%B0%D0%BD%D1%88%D0%B5%D1%82%D1%8B/ct-p/lt_ru" }
			}
		}
	};

	private string mDefaultLanguageKey = "en-US";

	private string mCurrentLanguage = string.Empty;

	public ForumFrame()
	{
		InitializeComponent();
		popupWin.IsOpen = false;
		btnLenovoPhone.AddHandler(UIElement.MouseUp, (RoutedEventHandler)delegate
		{
			SwitchChileView();
			GoToTargetUrl("lenovoPhone");
		}, handledEventsToo: true);
		btnMotoPhone.AddHandler(UIElement.MouseUp, (RoutedEventHandler)delegate
		{
			SwitchChileView();
			GoToTargetUrl("motoPhone");
		}, handledEventsToo: true);
		btnLenovoTable.AddHandler(UIElement.MouseUp, (RoutedEventHandler)delegate
		{
			SwitchChileView();
			GoToTargetUrl("lenovoTablet");
		}, handledEventsToo: true);
	}

	private static void SetRegister()
	{
		string moduleName = Process.GetCurrentProcess().MainModule.ModuleName;
		if (Environment.Is64BitOperatingSystem)
		{
			Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", moduleName, 11001, RegistryValueKind.DWord);
		}
		else
		{
			Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", moduleName, 11001, RegistryValueKind.DWord);
		}
	}

	private string GetCurrentLanguageKey()
	{
		if (string.IsNullOrEmpty(mCurrentLanguage))
		{
			mCurrentLanguage = HostProxy.LanguageService.GetCurrentLanguage();
		}
		return mCurrentLanguage;
	}

	public void GoToTargetUrl(string rootLabel)
	{
		string targetUrl = string.Empty;
		string text = GetCurrentLanguageKey() ?? string.Empty;
		if (HostProxy.LanguageService.IsChinaRegionAndLanguage())
		{
			switch (rootLabel)
			{
			case "motoPhone":
				targetUrl = "https://club.lenovo.com.cn/moto/";
				break;
			case "lenovoPhone":
				targetUrl = "https://club.lenovo.com.cn/phone/";
				break;
			case "lenovoTablet":
				targetUrl = "https://club.lenovo.com.cn/forum-1349-1.html";
				break;
			}
		}
		else
		{
			Dictionary<string, string> dictionary = null;
			if (lenovoPhonesUrlMapping.ContainsKey(rootLabel))
			{
				dictionary = lenovoPhonesUrlMapping[rootLabel];
			}
			else
			{
				LogHelper.LogInstance.Debug("====>> url map nont exist rootLabel: " + rootLabel);
			}
			if (dictionary != null && dictionary.Keys.Contains(text))
			{
				targetUrl = dictionary[text];
			}
			else
			{
				targetUrl = dictionary[mDefaultLanguageKey];
			}
		}
		browserView.Tag = rootLabel;
		LogHelper.LogInstance.Debug($"Forum forward url [lgk:{text}][url:{targetUrl}]");
		popupWin.IsOpen = true;
		Task.Run(delegate
		{
			Dictionary<string, string> extraData = new Dictionary<string, string> { { "url", targetUrl } };
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			LoadResult loadResult = DotNetBrowserHelper.Instance.LoadUrl(browserView, targetUrl, delegate
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					popupWin.IsOpen = false;
				});
			});
			stopwatch.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.SUPPORT_FORUM, new BusinessData(BusinessType.SUPPORT_FORUM, null).Update(stopwatch.ElapsedMilliseconds, (loadResult == LoadResult.Failed) ? BusinessStatus.FALIED : BusinessStatus.SUCCESS, extraData));
		});
	}

	private void SwitchToHomeView()
	{
		panelSubView.Visibility = Visibility.Collapsed;
		panelHome.Visibility = Visibility.Visible;
		popupWin.IsOpen = false;
	}

	public void SwitchChileView()
	{
		panelSubView.Visibility = Visibility.Visible;
		panelHome.Visibility = Visibility.Collapsed;
	}

	private void btnBack_Click(object sender, RoutedEventArgs e)
	{
	}

	private void btnForward_Click(object sender, RoutedEventArgs e)
	{
	}

	private void btnHome_Click(object sender, RoutedEventArgs e)
	{
		string text = browserView.Tag as string;
		if (!string.IsNullOrEmpty(text))
		{
			SwitchChileView();
			GoToTargetUrl(text);
		}
	}

	private void btnRefresh_Click(object sender, RoutedEventArgs e)
	{
	}
}
