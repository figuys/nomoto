using System;

namespace lenovo.mbg.service.framework.hostcontroller;

public class PluginErrorEventArgs : EventArgs
{
	public Plugin Plugin { get; private set; }

	public string Message { get; private set; }

	public Exception Exception { get; private set; }

	public PluginErrorEventArgs(Plugin plugin, string message, Exception exception)
	{
		Plugin = plugin;
		Message = message;
		Exception = exception;
	}
}
