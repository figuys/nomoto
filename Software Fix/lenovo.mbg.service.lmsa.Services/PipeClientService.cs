using System;
using lenovo.mbg.service.framework.pipes;

namespace lenovo.mbg.service.lmsa.Services;

public class PipeClientService : IDisposable
{
	protected ClientPipe _clientPipe;

	public event PipeDataRecived OnPipeDataRecived;

	public void Create(int timeout)
	{
		if (_clientPipe == null)
		{
			_clientPipe = new ClientPipe(".", delegate(BasicPipe bp)
			{
				bp.StartStringReaderAsync();
			});
			_clientPipe.DataReceived += ClientPipeDataReceviedHandler;
		}
		try
		{
			_clientPipe.Connect(timeout);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}

	public void Send(PipeMessage message, object data)
	{
		_clientPipe.WriteStringAsync(message, data);
	}

	public void Close()
	{
		_clientPipe.Close();
		_clientPipe = null;
	}

	private void ClientPipeDataReceviedHandler(object sender, PipeEventArgs e)
	{
		this.OnPipeDataRecived?.Invoke(e.message, e.data);
	}

	public void Dispose()
	{
		Close();
	}
}
