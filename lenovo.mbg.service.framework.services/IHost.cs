using System;

namespace lenovo.mbg.service.framework.services;

public interface IHost : IServiceProvider
{
	IntPtr HostMainWindowHandle { get; }

	int HostProcessId { get; }

	T GetService<T>(string name);

	void RegisterService(string name, object value);
}
