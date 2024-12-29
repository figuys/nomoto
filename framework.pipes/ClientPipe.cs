using System;
using System.IO.Pipes;

namespace lenovo.mbg.service.framework.pipes;

public class ClientPipe : BasicPipe
{
	protected NamedPipeClientStream clientPipeStream;

	public ClientPipe()
		: this(".", delegate(BasicPipe bp)
		{
			bp.StartStringReaderAsync();
		})
	{
	}

	public ClientPipe(string serverName, Action<BasicPipe> asyncReaderStart)
	{
		base.asyncReaderStart = asyncReaderStart;
		clientPipeStream = new NamedPipeClientStream(serverName, PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
		pipeStream = clientPipeStream;
	}

	public void Connect(int timeout)
	{
		clientPipeStream.Connect(timeout);
		asyncReaderStart(this);
	}
}
