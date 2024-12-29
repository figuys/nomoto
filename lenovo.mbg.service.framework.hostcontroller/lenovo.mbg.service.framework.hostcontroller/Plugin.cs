using System;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.framework.hostcontroller;

public class Plugin : IDisposable
{
	private bool _isDisposing;

	private bool _fatalErrorOccurred;

	private IPlugin mPlugin;

	public PluginInfo PluginInfo { get; private set; }

	public FrameworkElement View { get; private set; }

	public IPlugin PluginInstance => mPlugin;

	public event EventHandler<PluginErrorEventArgs> PluginError;

	public Plugin(PluginInfo pluginInfo, IPlugin plugin)
	{
		mPlugin = plugin;
		PluginInfo = pluginInfo;
	}

	public void Init()
	{
		if (mPlugin != null)
		{
			mPlugin.Init();
			return;
		}
		throw new Exception("IPlugin instance is null, init failed");
	}

	public void CreateView()
	{
		try
		{
			if (mPlugin != null)
			{
				View = mPlugin.CreateControl(null);
			}
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.hostcontroller.Plugin: CreateView() throw exception:", exception);
		}
	}

	public bool CanClose()
	{
		if (mPlugin != null)
		{
			return mPlugin.CanClose();
		}
		return true;
	}

	public bool IsExecuteWork()
	{
		if (mPlugin != null)
		{
			return mPlugin.IsExecuteWork();
		}
		return false;
	}

	public void OnSelected(string val)
	{
		if (mPlugin != null)
		{
			mPlugin.OnSelected(val);
		}
	}

	public void OnSelecting(string val)
	{
		if (mPlugin != null)
		{
			mPlugin.OnSelecting(val);
		}
	}

	public void Dispose()
	{
		_isDisposing = true;
		try
		{
			if (View is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("lenovo.mbg.service.framework.hostcontroller.Plugin.Dispose(): Exception", exception);
		}
	}

	private void OnProcessExited()
	{
		if (!_isDisposing && !_fatalErrorOccurred)
		{
			string message = $"'{PluginInfo.PluginName}' plugin process terminated unexpectedly";
			ReportError(message, null);
		}
	}

	private void OnFatalError(string message, Exception ex)
	{
		_fatalErrorOccurred = true;
		ReportError(message, ex);
	}

	private void ReportError(string message, Exception ex)
	{
		LogHelper.LogInstance.Error(message, ex);
		if (this.PluginError != null)
		{
			this.PluginError(this, new PluginErrorEventArgs(this, message, ex));
		}
	}
}
