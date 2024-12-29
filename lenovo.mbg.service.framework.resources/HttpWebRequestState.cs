using System;
using System.IO;
using System.Net;
using System.Threading;

namespace lenovo.mbg.service.framework.resources;

public class HttpWebRequestState
{
	public int MAX_BUFFER_SIZE = 1024000;

	public AutoResetEvent WriteDoneEvent;

	public WebRequest Request { get; set; }

	public WebResponse Response { get; set; }

	public DateTime StartTime { get; set; }

	public int BufferReadIndex { get; set; }

	public FileStream FStream { get; set; }

	public DriveInfo DiskInfo { get; set; }

	public Stream ResponseStream { get; set; }

	public RegisteredWaitHandle WaitHandle { get; set; }

	public byte[][] BufferRead { get; }

	public long SecTotalByte { get; set; }

	public long ReadLength { get; set; }

	public long ReadAllLength { get; set; }

	public HttpWebRequestState()
	{
		BufferRead = new byte[2][]
		{
			new byte[MAX_BUFFER_SIZE],
			new byte[MAX_BUFFER_SIZE]
		};
		WriteDoneEvent = new AutoResetEvent(initialState: true);
	}
}
