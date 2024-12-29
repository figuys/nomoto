using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DotNetBrowser.Navigation;
using DotNetBrowser.Navigation.Events;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.tips;

public partial class TipsFrame : UserControl, IComponentConnector
{
	private const string URL_SUFFIX_FROMAT = "/Tips/index.html?country={0}&language={1}&user={2}&t={3}&modelName={4}&deviceType={5}";

	private const string URL_TIPS_LIST = "/Tips/list.html";

	private static string NAVIGATE_URL;

	private long lockrefresh;

	private string _OldUserID;

	private string _NewUserID;

	private string _baseHttpUrl;

	private string currentUrl;

	private string _Country;

	private string _Langue;

	public TipsFrame()
	{
		InitializeComponent();
		base.Loaded += TipsFrame_Loaded;
	}

	public void TipsFrame_Loaded(object sender, RoutedEventArgs e)
	{
		InitializeUserChanged();
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		if (e != DeviceSoftStateEx.Online)
		{
			return;
		}
		string text = LoadCurrentDevice();
		NAVIGATE_URL = _baseHttpUrl + $"/Tips/index.html?country={_Country}&language={_Langue}&user={_OldUserID}&t={DateTime.Now.Ticks}&modelName={text}&deviceType={GetCurrentCategoryKey()}";
		if (currentUrl != null && NAVIGATE_URL != currentUrl)
		{
			Task.Run(delegate
			{
				DotNetBrowserHelper.Instance.LoadUrl(browserView, NAVIGATE_URL, null);
			});
		}
	}

	private void NavigatedHandler(object sender, NavigationFinishedEventArgs e)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			grid.Visibility = Visibility.Visible;
			LoadintButton.Visibility = Visibility.Collapsed;
			SetEnable();
		});
	}

	public void GoToUrl(string categoryKey)
	{
		InitilizeCountry();
		InitilizeLangue();
		InitilizeUserId();
		string text = LoadCurrentDevice();
		_baseHttpUrl = HostProxy.ConfigService.getConfig("AppConfig", "BaseHttpUrl");
		long ticks = DateTime.Now.Ticks;
		string text2 = (string.IsNullOrEmpty(categoryKey) ? GetCurrentCategoryKey() : categoryKey);
		Dictionary<string, string> aparams = new Dictionary<string, string>();
		aparams.Add("aparams", _Country);
		aparams.Add("language", _Langue);
		aparams.Add("user", _OldUserID);
		aparams.Add("t", ticks.ToString());
		aparams.Add("modelName", text);
		aparams.Add("deviceType", text2);
		NAVIGATE_URL = _baseHttpUrl + $"/Tips/index.html?country={_Country}&language={_Langue}&user={_OldUserID}&t={ticks}&modelName={text}&deviceType={text2}";
		Stopwatch sw = new Stopwatch();
		BusinessData businessData = new BusinessData(BusinessType.SUPPORT_TIPS, HostProxy.deviceManager.MasterDevice);
		sw.Start();
		LogHelper.LogInstance.Debug(NAVIGATE_URL);
		Task.Run(delegate
		{
			LoadResult loadResult = DotNetBrowserHelper.Instance.LoadUrl(browserView, NAVIGATE_URL, delegate
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					SetEnable();
				});
				NavigatedHandler(null, null);
			});
			sw.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.SUPPORT_TIPS, businessData.Update(sw.ElapsedMilliseconds, (loadResult == LoadResult.Failed) ? BusinessStatus.FALIED : BusinessStatus.SUCCESS, aparams));
		});
	}

	private string GetCurrentCategoryKey()
	{
		return FileHelper.ReadWithAesDecrypt(lenovo.mbg.service.common.utilities.Configurations.DefaultOptionsFileName, "{D86FEEC4-62D8-4DD5-88ED-181481200D4A}");
	}

	private string LoadCurrentDevice()
	{
		if (HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { SoftStatus: DeviceSoftStateEx.Online } tcpAndroidDevice)
		{
			string modelName = tcpAndroidDevice.Property.ModelName2;
			if (!string.IsNullOrEmpty(modelName))
			{
				if (string.Equals("Tab2A7-10F", modelName, StringComparison.CurrentCultureIgnoreCase))
				{
					return "TAB 2 A7-10F";
				}
				if (string.Equals("Moto Z (2)", modelName, StringComparison.CurrentCultureIgnoreCase))
				{
					return "Moto Z (2nd Gen.)";
				}
				return Regex.Replace(modelName.Trim(), "lenovo|(\\s*?\\()|\\)", "", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace).Trim();
			}
		}
		return null;
	}

	private void InitializeUserChanged()
	{
		HostProxy.User.OnUserChanged += UserChangedHanlder;
	}

	private void UserChangedHanlder(object sender, UserInfoArgs e)
	{
		_NewUserID = e.User?.UserId;
		string url = DotNetBrowserHelper.Instance.GetUrl(browserView);
		if (_NewUserID != _OldUserID && !string.IsNullOrEmpty(url) && new Uri(url).AbsolutePath.StartsWith("/Tips/index.html"))
		{
			_OldUserID = _NewUserID;
			string text = LoadCurrentDevice();
			NAVIGATE_URL = _baseHttpUrl + $"/Tips/index.html?country={_Country}&language={_Langue}&user={_OldUserID}&t={DateTime.Now.Ticks}&modelName={text}&deviceType={GetCurrentCategoryKey()}";
			Task.Run(delegate
			{
				DotNetBrowserHelper.Instance.LoadUrl(browserView, NAVIGATE_URL, null);
			});
		}
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

	private void SetEnable()
	{
		try
		{
			currentUrl = DotNetBrowserHelper.Instance.GetUrl(browserView);
		}
		catch (Exception)
		{
		}
		if (currentUrl != null && new Uri(currentUrl).AbsolutePath.StartsWith("/Tips/index.html"))
		{
			back.IsEnabled = false;
			forward.IsEnabled = false;
		}
		else if (currentUrl != null && currentUrl.Contains("custhelp.com/app/home"))
		{
			back.IsEnabled = false;
		}
		else
		{
			back.IsEnabled = DotNetBrowserHelper.Instance.CanGoBack(browserView);
			forward.IsEnabled = DotNetBrowserHelper.Instance.CanGoForward(browserView);
		}
	}

	private void BackClick(object sender, RoutedEventArgs e)
	{
		if (!DotNetBrowserHelper.Instance.CanGoBack(browserView))
		{
			return;
		}
		string url = DotNetBrowserHelper.Instance.GetUrl(browserView);
		if (_NewUserID != _OldUserID && url != null && new Uri(url).AbsolutePath.StartsWith("/Tips/list.html"))
		{
			_OldUserID = _NewUserID;
			NAVIGATE_URL = _baseHttpUrl + $"/Tips/index.html?country={_Country}&language={_Langue}&user={_OldUserID}&t={DateTime.Now.Ticks}&modelName={null}&deviceType={GetCurrentCategoryKey()}";
			Task.Run(delegate
			{
				DotNetBrowserHelper.Instance.LoadUrl(browserView, NAVIGATE_URL, null);
			});
		}
		else
		{
			DotNetBrowserHelper.Instance.GoBack(browserView);
		}
	}

	private void ForwardClick(object sender, RoutedEventArgs e)
	{
		if (DotNetBrowserHelper.Instance.CanGoForward(browserView))
		{
			DotNetBrowserHelper.Instance.GoForward(browserView);
		}
	}

	private void HomeClick(object sender, RoutedEventArgs e)
	{
		Task.Run(delegate
		{
			DotNetBrowserHelper.Instance.LoadUrl(browserView, NAVIGATE_URL, delegate
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					SetEnable();
				});
			});
		});
	}

	private void RefreshClick(object sender, RoutedEventArgs e)
	{
		if (Interlocked.Read(ref lockrefresh) != 0L)
		{
			return;
		}
		try
		{
			Interlocked.Exchange(ref lockrefresh, 1L);
			DotNetBrowserHelper.Instance.Reload(browserView);
		}
		catch (Exception)
		{
		}
		finally
		{
			Interlocked.Exchange(ref lockrefresh, 0L);
		}
	}

	private void InitilizeUserId()
	{
		if (HostProxy.User.user != null)
		{
			_OldUserID = (_NewUserID = HostProxy.User.user.UserId);
		}
	}

	private void InitilizeLangue()
	{
		_Langue = LMSAContext.CurrentLanguage.Substring(0, LMSAContext.CurrentLanguage.IndexOf('-'));
	}

	private void InitilizeCountry()
	{
		if (string.IsNullOrEmpty(_Country))
		{
			RegionInfo regionInfo = GlobalFun.GetRegionInfo();
			_Country = regionInfo.TwoLetterISORegionName;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
