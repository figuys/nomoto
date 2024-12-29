using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using DotNetBrowser.Navigation.Events;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.LenovoId;

public partial class LenovoIdWindow : Window, IComponentConnector
{
	private bool isWebBrowser;

	private bool isManualLogin;

	private LenovoIdWindowViewModel _vm;

	protected static List<string> TracertList = new List<string>();

	protected static Process process;

	public Action<Window> OnLoginEvent { get; set; }

	public LenovoIdWindow(bool isRegister)
	{
		InitializeComponent();
		isManualLogin = false;
		_vm = new LenovoIdWindowViewModel();
		base.DataContext = _vm;
		base.Closed += Closedhandler;
		base.Loaded += LenovoIdWindow_Loaded;
	}

	private void LenovoIdWindow_Loaded(object sender, RoutedEventArgs e)
	{
		Task.Factory.StartNew(delegate
		{
			string text = LMSAContext.CurrentLanguage;
			if (!string.IsNullOrEmpty(text))
			{
				switch (text)
				{
				case "sk-SK":
				case "ro-RO":
				case "bg-BG":
					goto IL_0048;
				}
				if (!(text == "sr-RS"))
				{
					goto IL_004e;
				}
			}
			goto IL_0048;
			IL_0048:
			text = "en-US";
			goto IL_004e;
			IL_004e:
			object arg = AppContext.WebApi.RequestContent(WebApiUrl.CALL_API_URL, new
			{
				key = "TIP_URL"
			});
			string text2 = $"{arg}&lenovoid.lang={text.Replace('-', '_')}";
			isWebBrowser = !DotNetBrowserHelper.Instance.IsEngineLoaded;
			if (isWebBrowser)
			{
				if (!string.IsNullOrEmpty(_webbrowser.Document?.Cookie))
				{
					int length = _webbrowser.Document.Cookie.Length;
					_webbrowser.Document.Cookie.Remove(0, length);
				}
				_webbrowser.ScriptErrorsSuppressed = true;
				_webbrowser.IsWebBrowserContextMenuEnabled = false;
				_webbrowser.Navigated -= _webbrowser_Navigated;
				_webbrowser.Navigated += _webbrowser_Navigated;
				_webbrowser.Navigate(text2);
			}
			else
			{
				Tracert(new Uri(text2));
				DotNetBrowserHelper.Instance.LoadUrl(browserView, text2, delegate(NavigationFinishedEventArgs ee)
				{
					if (!string.IsNullOrEmpty(ee?.Url) && false == ee?.Url?.StartsWith("about"))
					{
						LogHelper.LogInstance.Info("Load url finished, and redirect to " + ee?.Url);
					}
					if (ee != null && ee.Browser != null)
					{
						LoginCallback(new Uri(ee.Browser.Url));
					}
				});
			}
		});
	}

	private void Tracert(Uri uri)
	{
		Task.Run(delegate
		{
			process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.EnableRaisingEvents = true;
			process.StartInfo.CreateNoWindow = true;
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				if (!string.IsNullOrEmpty(e.Data))
				{
					TracertList.Add(e.Data);
				}
			};
			process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				if (!string.IsNullOrEmpty(e.Data))
				{
					TracertList.Add(e.Data);
				}
			};
			process.Start();
			process.StandardInput.WriteLine("tracert " + uri.DnsSafeHost + " &exit");
			process.StandardInput.AutoFlush = true;
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();
		});
	}

	public static List<string> GetTracertDatas()
	{
		if (process != null && !process.SafeHandle.IsClosed)
		{
			try
			{
				process.Dispose();
			}
			catch
			{
			}
		}
		return TracertList;
	}

	private void _webbrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
	{
		LoginCallback(e.Url);
	}

	private void LoginCallback(Uri uri)
	{
		if (uri.AbsoluteUri == "https://passport.lenovo.com/glbwebauthnv6/preLogin")
		{
			isManualLogin = true;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			_vm.Init(isWebBrowser);
		});
		if (uri.LocalPath != "/Tips/lenovoIdSuccess.html")
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			_vm.DoNetBrowserVisibility = Visibility.Collapsed;
			_vm.WinformBrowserVisibility = Visibility.Collapsed;
		});
		LogHelper.LogInstance.Info("lenovoid login successfully, goto successfully ui");
		Task.Run(delegate
		{
			_vm.TokenAnalysis(uri.AbsoluteUri, delegate
			{
				if (isManualLogin)
				{
					FileHelper.WriteJsonWithAesEncrypt(Configurations.DefaultOptionsFileName, "AutoLogin", true);
				}
				OnLoginEvent?.Invoke(this);
			});
		});
	}

	private void Closedhandler(object sender, EventArgs e)
	{
		DotNetBrowserHelper.Instance.Remove(browserView);
	}

	public void Reload()
	{
		DotNetBrowserHelper.Instance.Reload(browserView);
	}

	private void MouseLeftButtonDownHandler(object sender, MouseButtonEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed)
		{
			DragMove();
		}
	}

	public static void ShowDialogEx(bool isRegister = false, Action<Window> callBack = null)
	{
		LenovoIdWindow wnd = new LenovoIdWindow(isRegister)
		{
			OnLoginEvent = callBack
		};
		HostProxy.HostMaskLayerWrapper.New(wnd, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => wnd.ShowDialog());
	}
}
