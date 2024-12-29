using System;
using lenovo.mbg.service.framework.pipes;
using LmsaWindowsService.Contracts;
using LmsaWindowsService.PipeWorkers;

namespace LmsaWindowsService.Tasks;

public class PipeTask : ITask, IDisposable
{
	protected ServerPipe serverPipe;

	protected PipeMessageDistribution _PipeMessageDistribution;

	public bool IsRunning { get; private set; }

	public string Name { get; private set; }

	public PipeTask()
		: this(typeof(PipeTask).Name)
	{
	}

	public PipeTask(string taskName)
	{
		Name = taskName;
		_PipeMessageDistribution = new PipeMessageDistribution();
	}

	public void Start()
	{
		IsRunning = true;
		serverPipe = new ServerPipe(delegate(BasicPipe bp)
		{
			bp.StartStringReaderAsync();
		});
		serverPipe.DataReceived += ServerPipeDataReceivedHandler;
	}

	public void Stop()
	{
		IsRunning = false;
		serverPipe.Close();
	}

	public void WriteAsync(PipeMessage message, object data)
	{
		serverPipe?.WriteStringAsync(message, data);
	}

	public void Dispose()
	{
		Stop();
		serverPipe = null;
	}

	private void ServerPipeDataReceivedHandler(object sender, PipeEventArgs e)
	{
		_PipeMessageDistribution.Received(e.message, e.data);
	}
}
