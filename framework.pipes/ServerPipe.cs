using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace lenovo.mbg.service.framework.pipes;

public class ServerPipe : BasicPipe
{
	protected NamedPipeServerStream serverPipeStream;

	public event EventHandler<EventArgs> Connected;

	public ServerPipe()
		: this(delegate(BasicPipe bp)
		{
			bp.StartStringReaderAsync();
		})
	{
	}

	public ServerPipe(Action<BasicPipe> asyncReaderStart)
	{
		base.asyncReaderStart = asyncReaderStart;
		base.PipeClosed += ServerPipeClosedHandler;
		CreateAndListen();
	}

	public void CreateAndListen()
	{
		PipeSecurity pipeSecurity = new PipeSecurity();
		PipeAccessRule rule = new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow);
		pipeSecurity.AddAccessRule(rule);
		PipeAccessRule rule2 = new PipeAccessRule("Everyone", PipeAccessRights.ReadWrite, AccessControlType.Allow);
		pipeSecurity.AddAccessRule(rule2);
		serverPipeStream = new NamedPipeServerStream(PipeName, PipeDirection.InOut, -1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 512, 512, pipeSecurity, HandleInheritability.None);
		pipeStream = serverPipeStream;
		serverPipeStream.BeginWaitForConnection(PipeConnected, null);
	}

	private void ServerPipeClosedHandler(object sender, EventArgs e)
	{
		Close();
		CreateAndListen();
	}

	protected void PipeConnected(IAsyncResult ar)
	{
		serverPipeStream.EndWaitForConnection(ar);
		this.Connected?.Invoke(this, new EventArgs());
		asyncReaderStart(this);
	}
}
