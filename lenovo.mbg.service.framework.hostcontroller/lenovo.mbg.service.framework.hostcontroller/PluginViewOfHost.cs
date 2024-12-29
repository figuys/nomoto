using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.framework.hostcontroller;

public class PluginViewOfHost : IHost, IServiceProvider
{
	private IntPtr _hostMainWindowHandle = IntPtr.Zero;

	public int HostProcessId => Process.GetCurrentProcess().Id;

	public Exception LastError { get; private set; }

	public IntPtr HostMainWindowHandle
	{
		get
		{
			if (_hostMainWindowHandle == IntPtr.Zero)
			{
				_hostMainWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
			}
			return _hostMainWindowHandle;
		}
	}

	public object GetService(Type serviceType)
	{
		if (serviceType.IsClass && !serviceType.IsAbstract)
		{
			return Activator.CreateInstance(serviceType);
		}
		return null;
	}

	public T GetService<T>(string name)
	{
		T result = default(T);
		try
		{
			result = ContainerManager.Instance.GetService<T>(name);
			return result;
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.hostcontroller.PluginViewOfHost.GetService: Exception", exception);
		}
		return result;
	}

	public void RegisterService(string name, object value)
	{
		ContainerManager.Instance.RegisterService(name, value);
	}
}
