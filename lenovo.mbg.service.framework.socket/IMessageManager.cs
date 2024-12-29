using System;

namespace lenovo.mbg.service.framework.socket;

public interface IMessageManager : IDisposable
{
	SocketWrapper GetHeartbeatChannel();

	MessageReaderAndWriter CreateMessageReaderAndWriter(int timeout = 0);
}
