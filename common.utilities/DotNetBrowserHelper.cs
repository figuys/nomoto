using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;
using System.Windows;
using DotNetBrowser.Browser;
using DotNetBrowser.Engine;
using DotNetBrowser.Logging;
using DotNetBrowser.Navigation;
using DotNetBrowser.Navigation.Events;
using DotNetBrowser.Wpf;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.common.utilities;

public class DotNetBrowserHelper
{
	private static DotNetBrowserHelper instance = null;

	private static object locker = new object();

	private IEngine engine;

	private ConcurrentDictionary<BrowserView, IBrowser> browserMapping = new ConcurrentDictionary<BrowserView, IBrowser>();

	public static DotNetBrowserHelper Instance
	{
		get
		{
			if (instance == null)
			{
				lock (locker)
				{
					if (instance == null)
					{
						instance = new DotNetBrowserHelper();
					}
				}
			}
			return instance;
		}
	}

	public bool IsEngineLoaded
	{
		get
		{
			lock (locker)
			{
				return engine != null;
			}
		}
	}

	public string ChromiumLogFilePath
	{
		get
		{
			string text = Path.Combine(Configurations.ProgramDataPath, "DotNetBrowserLogs");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	public void InitEngineAsync()
	{
		if (engine != null && !engine.IsDisposed)
		{
			return;
		}
		lock (locker)
		{
			if (engine != null && !engine.IsDisposed)
			{
				return;
			}
			try
			{
				LoggerProvider.Instance.Level = SourceLevels.All;
				LoggerProvider.Instance.FileLoggingEnabled = false;
				Configurations.ChromiumLogFilePath = Path.Combine(ChromiumLogFilePath, DateTime.Now.ToString("yyyy-MM-dd") + "-browser.log");
				try
				{
					using (File.Open(Configurations.ChromiumLogFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
					{
						LoggerProvider.Instance.OutputFile = Configurations.ChromiumLogFilePath;
					}
				}
				catch (Exception)
				{
					Configurations.ChromiumLogFilePath = Path.Combine(ChromiumLogFilePath, $"{DateTime.Now:yyyy-MM-dd}-{DateTime.Now:MMddHHmmss}-browser.log");
					LoggerProvider.Instance.OutputFile = Configurations.ChromiumLogFilePath;
				}
				string chromiumDirectory = Path.Combine(Path.GetTempPath(), "rsa-dotnetbrowser");
				string userDataDirectory = Path.Combine(Path.GetTempPath(), "rsa-dotnetbrowser-data");
				string licenseKey = "BV3R4DG2WYL9JWPH0QU15QX6LHH0T40NHD6MDQCTO1IE629YGZP8K9QOYUAQ76YK7212C4XCZ23LND5JMI98L2O0MUFPI73P5M4ZGVC1FNY6S5DDOMKS47WS5Y";
				while (true)
				{
					try
					{
						engine = EngineFactory.Create(new EngineOptions.Builder
						{
							LicenseKey = licenseKey,
							ChromiumDirectory = chromiumDirectory,
							UserDataDirectory = userDataDirectory,
							RenderingMode = RenderingMode.HardwareAccelerated
						}.Build());
						LogHelper.LogInstance.Info("SoftWare Fix client create browser engine successed!");
						break;
					}
					catch (EngineInitializationException ex2)
					{
						LogHelper.LogInstance.Warn("Create browser engine failed, will retry" + ex2.ToString());
						Directory.EnumerateDirectories(Path.GetTempPath(), "rsa-dotnetbrowser-data*").ToList()?.ForEach(delegate(string n)
						{
							GlobalFun.DeleteDirectoryEx(n);
						});
						userDataDirectory = Path.Combine(Path.GetTempPath(), "rsa-dotnetbrowser-data-" + DateTime.Now.ToString("MMddHHmmss"));
					}
				}
			}
			catch (Exception ex3)
			{
				LogHelper.LogInstance.Warn("Create browser engine failed:" + ex3.ToString());
				try
				{
					engine?.Dispose();
					engine = null;
				}
				catch (Exception)
				{
				}
			}
		}
	}

	public LoadResult LoadUrl(BrowserView browserView, string url, Action<NavigationFinishedEventArgs> loadFinishedCallback, bool recordlog = true)
	{
		LogHelper.LogInstance.Info("LoadUrl:url:" + url);
		if (engine == null || engine.IsDisposed)
		{
			LogHelper.LogInstance.Info("Donetbrowser engine has't created!");
			return LoadResult.Failed;
		}
		try
		{
			if (recordlog)
			{
				LoggerProvider.Instance.FileLoggingEnabled = true;
			}
			IBrowser browser = null;
			browserMapping.TryGetValue(browserView, out browser);
			if (browser == null)
			{
				lock (locker)
				{
					try
					{
						LogHelper.LogInstance.Info("LoadUrl:url1:" + url);
						browser = engine.CreateBrowser();
						browserView.Dispatcher.Invoke(delegate
						{
							browserView.InitializeFrom(browser);
						});
						browser.Navigation.NavigationFinished += delegate(object s, NavigationFinishedEventArgs e)
						{
							loadFinishedCallback?.Invoke(e);
						};
						browserMapping.TryAdd(browserView, browser);
					}
					catch (Exception exception)
					{
						string text = "engine is null";
						if (engine != null)
						{
							text = "created, isDisposed:" + engine.IsDisposed;
						}
						string text2 = "browser is null";
						if (browser != null)
						{
							text2 = "created, isDisposed:" + browser.IsDisposed;
						}
						LogHelper.LogInstance.Warn("engine:[" + text + "] browser:[" + text2 + "].");
						LogHelper.LogInstance.Warn("Create browser instance failed!", exception);
						return LoadResult.Failed;
					}
				}
			}
			LogHelper.LogInstance.Info("Donetbrowser load url[" + url + "] start");
			Task<LoadResult> task = browser.Navigation.LoadUrl(url);
			LogHelper.LogInstance.Info($"Donetbrowser load url result: {task}!");
			return task.Result;
		}
		catch (Exception exception2)
		{
			LogHelper.LogInstance.Warn("Call dotnetbrowser load url failed!", exception2);
			if (TryRequestUrl(url) && url.Contains("Tips/lenovoIdSuccess.html"))
			{
				ClearCookies();
			}
			loadFinishedCallback?.Invoke(null);
			return LoadResult.Failed;
		}
		finally
		{
			LoggerProvider.Instance.FileLoggingEnabled = false;
			LogHelper.LogInstance.Info("Call LoadUrl method finished!");
		}
	}

	public string GetUrl(BrowserView browserView)
	{
		IBrowser value = null;
		browserMapping.TryGetValue(browserView, out value);
		if (value != null)
		{
			try
			{
				return value.Url;
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				Exception ex3 = ex2;
				LogHelper.LogInstance.Warn($"Get url failed:{ex3}");
				browserView.Dispatcher.Invoke(delegate
				{
					MessageBox.Show(ex3.ToString());
				});
			}
		}
		return string.Empty;
	}

	public bool CanGoBack(BrowserView browserView)
	{
		IBrowser value = null;
		browserMapping.TryGetValue(browserView, out value);
		if (value != null)
		{
			try
			{
				return value.Navigation.CanGoBack();
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				Exception ex3 = ex2;
				LogHelper.LogInstance.Warn($"Get property[CanGoBack] failed:{ex3}");
				browserView.Dispatcher.Invoke(delegate
				{
					MessageBox.Show(ex3.ToString());
				});
			}
		}
		return false;
	}

	public bool GoBack(BrowserView browserView)
	{
		IBrowser value = null;
		browserMapping.TryGetValue(browserView, out value);
		if (value != null)
		{
			try
			{
				value.Navigation.GoBack();
				return true;
			}
			catch (Exception arg)
			{
				LogHelper.LogInstance.Warn($"Execute method[GoBack] failed:{arg}");
			}
		}
		return false;
	}

	public bool CanGoForward(BrowserView browserView)
	{
		IBrowser value = null;
		browserMapping.TryGetValue(browserView, out value);
		if (value != null)
		{
			try
			{
				return value.Navigation.CanGoForward();
			}
			catch (Exception arg)
			{
				LogHelper.LogInstance.Warn($"Get property[CanGoForward] failed:{arg}");
			}
		}
		return false;
	}

	public bool GoForward(BrowserView browserView)
	{
		IBrowser value = null;
		browserMapping.TryGetValue(browserView, out value);
		if (value != null)
		{
			try
			{
				value.Navigation.GoForward();
				return true;
			}
			catch (Exception arg)
			{
				LogHelper.LogInstance.Warn($"Execute method[GoBaGoForwardck] failed:{arg}");
			}
		}
		return false;
	}

	public bool Reload(BrowserView browserView)
	{
		IBrowser value = null;
		browserMapping.TryGetValue(browserView, out value);
		if (value != null)
		{
			try
			{
				value.Navigation.ReloadIgnoringCache();
				return true;
			}
			catch (Exception arg)
			{
				LogHelper.LogInstance.Warn($"Execute method[Reload] failed:{arg}");
			}
		}
		return false;
	}

	public void Remove(BrowserView browserView)
	{
		IBrowser value = null;
		browserMapping.TryRemove(browserView, out value);
		value?.Dispose();
	}

	public void ClearCookies()
	{
		engine?.Profiles.Default.CookieStore.DeleteAllCookies();
	}

	public void Dispose()
	{
		if (engine != null && !engine.IsDisposed)
		{
			try
			{
				LogHelper.LogInstance.Debug("Begin DotNetBrowser Dispose.");
				engine.Dispose();
				engine = null;
				LogHelper.LogInstance.Debug("End DotNetBrowser Dispose.");
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("DotNetBrowser Dispose Exception:" + ex.ToString());
			}
		}
	}

	public bool TryRequestUrl(string url)
	{
		try
		{
			HttpWebRequest obj = WebRequest.Create(url) as HttpWebRequest;
			obj.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
			obj.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36";
			obj.Headers.Add("Request-Tag: lmsa");
			obj.KeepAlive = false;
			obj.Method = "GET";
			obj.Timeout = 30000;
			obj.ContentType = "multipart/form-data;charset=UTF-8";
			HttpWebResponse httpWebResponse = obj.GetResponse() as HttpWebResponse;
			LogHelper.LogInstance.Info($"====>>Rsa request {url} {httpWebResponse.StatusCode}!");
			return httpWebResponse.StatusCode == HttpStatusCode.OK;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Info("====>>Rsa request " + url + " occur exception, message: " + ex.Message + "!");
			return false;
		}
	}
}
