using System;

namespace LmsaWindowsService.Contracts;

public interface ITask : IDisposable
{
	string Name { get; }

	bool IsRunning { get; }

	void Start();

	void Stop();
}
