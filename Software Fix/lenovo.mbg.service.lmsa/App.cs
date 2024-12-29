using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.pipes;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.LenovoId;
using lenovo.mbg.service.lmsa.Services;
using lenovo.mbg.service.lmsa.ViewV6;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa;

public partial class App : Application, ISingleInstanceApp, IComponentConnector
{
	private const string UniqueEventName = "rescue_and_smart_assistant_unique_event_name";

	private const string UniqueMutexName = "rescue_and_smart_assistant_unique_mutext_name";

	private static EventWaitHandle eventWaitHandle;

	private static Mutex mutex;

	public static readonly string Category = "lmsa-host";

	private static object shutdownSelfLock = new object();

	private static volatile bool isShutdownSelf = false;

	[STAThread]
	[HandleProcessCorruptedStateExceptions]
	public static void Main(string[] args)
	{
		mutex = new Mutex(initiallyOwned: true, "rescue_and_smart_assistant_unique_mutext_name", out var createdNew);
		GC.KeepAlive(mutex);
		eventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset, "rescue_and_smart_assistant_unique_event_name");
		if (args.Length != 0)
		{
			createdNew = true;
		}
		if (createdNew)
		{
			Thread thread = new Thread((ThreadStart)delegate
			{
				while (eventWaitHandle.WaitOne())
				{
					Application.Current.Dispatcher.BeginInvoke((Action)delegate
					{
						if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
						{
							Application.Current.MainWindow.WindowState = WindowState.Normal;
						}
						Application.Current.MainWindow.Activate();
						Application.Current.MainWindow.Topmost = true;
						Application.Current.MainWindow.Topmost = false;
						Application.Current.MainWindow.Focus();
					});
				}
			})
			{
				IsBackground = true
			};
			thread.Start();
			App app = new App
			{
				ShutdownMode = ShutdownMode.OnMainWindowClose
			};
			app.InitializeComponent();
			app.Run();
		}
		else
		{
			eventWaitHandle.Set();
		}
	}

	public bool SignalExternalCommandLineArgs(IList<string> args)
	{
		if (args != null && !args.Contains("restart"))
		{
			if (base.MainWindow != null)
			{
				if (base.MainWindow.WindowState == WindowState.Minimized)
				{
					base.MainWindow.WindowState = WindowState.Normal;
				}
				base.MainWindow.Activate();
			}
			return true;
		}
		return false;
	}

	protected override void OnExit(ExitEventArgs e)
	{
		RsaServiceHelper.PostUserBehavior();
		List<string> values = (from n in Dns.GetHostAddresses(Dns.GetHostName())
			where n.AddressFamily == AddressFamily.InterNetwork
			select n.ToString()).ToList();
		LogHelper.LogInstance.Info("Local ip address: " + string.Join(",", values));
		LogHelper.LogInstance.Info("Tracert ip address: " + string.Join(Environment.NewLine, LenovoIdWindow.GetTracertDatas()));
		LogHelper.LogInstance.Info("========================== LMSA client application is closing: Dispose Resource And Exit The Application ===================== ");
		base.OnExit(e);
		Environment.Exit(0);
	}

	protected override void OnStartup(StartupEventArgs e)
	{
		LogHelper.LogInstance.Info("========================== LMSA client application is starting(v" + LMSAContext.MainProcessVersion + ") ==========================");
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		Application.Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
		AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
		base.OnStartup(e);
		SetRegister();
		AppContext.Init();
		long hardDiskFreeSpace = HardDisk.GetHardDiskFreeSpace(Environment.GetEnvironmentVariable("systemdrive"));
		if (hardDiskFreeSpace < 1073741824)
		{
			MessageBoxV6 messageBoxV = new MessageBoxV6();
			messageBoxV.Init("K0071", "K0985", "K0327", null, isCloseBtn: false, MessageBoxImage.Exclamation);
			messageBoxV.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			messageBoxV.ShowDialog();
			return;
		}
		TryStartNtserviceMonitorTask();
		DealOldVersionData();
		Task.Run(() => ProcessRunner.Shell("netsh advfirewall firewall delete rule name=\"lenovo.mbg.service.lmsa\""));
		SplashScreenWindow splashScreenWindow = (SplashScreenWindow)(base.MainWindow = new SplashScreenWindow());
		splashScreenWindow.ShowDialog();
	}

	private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
	{
		try
		{
			LogHelper.LogInstance.Error($"TaskScheduler unhandled exception:e instance info{e}, exception:{e.Exception}");
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Save taskScheduler unhandled exception failed:" + ex);
		}
		finally
		{
			e.SetObserved();
		}
	}

	private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		try
		{
			LogHelper.LogInstance.Error($"appdomain unhandled exception:e instance info:{e}, exceptionObj:{e.ExceptionObject}");
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Save appdomain unhandled exception failed:" + ex);
		}
		finally
		{
		}
	}

	private static void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		try
		{
			LogHelper.LogInstance.Error($"Application dispatcher unhandled e instance info:{e}, exception:{e.Exception}");
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Save application dispatcher unhandled exception:" + ex);
		}
		finally
		{
			e.Handled = true;
		}
	}

	private static void ShutdownSelfWhithUnhandledException()
	{
		if (isShutdownSelf)
		{
			return;
		}
		lock (shutdownSelfLock)
		{
			if (isShutdownSelf)
			{
				return;
			}
			LogHelper.LogInstance.Info("Unhandle exception occur, will shutdown self");
			isShutdownSelf = true;
			Application current = Application.Current;
			if (current != null && current.Dispatcher != null)
			{
				current.Dispatcher.Invoke(delegate
				{
					current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
					current.MainWindow?.Close();
					current.Shutdown(101);
				});
			}
			Environment.Exit(101);
		}
	}

	private static void SetRegister()
	{
		Task.Factory.StartNew(delegate
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
		});
	}

	private void TryStartNtserviceMonitorTask()
	{
		Task.Run(delegate
		{
			using PipeClientService pipeClientService = new PipeClientService();
			try
			{
				while (!GlobalFun.CheckServerIsRunning("LmsaWindowsService"))
				{
					Thread.Sleep(500);
				}
				pipeClientService.Create(5000);
				pipeClientService.Send(PipeMessage.LMSA_RUNNING, true);
			}
			catch (Exception exception)
			{
				LogHelper.LogInstance.Error("Start NT Service failed", exception);
			}
		});
	}

	private void DealOldVersionData()
	{
		UserConfigHleper.Instance.MigrateDataToOptions();
	}
}
