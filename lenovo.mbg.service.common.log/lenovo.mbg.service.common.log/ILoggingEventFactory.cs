using log4net.Core;

namespace lenovo.mbg.service.common.log;

public interface ILoggingEventFactory
{
	LoggingEvent CreateEncryptedLoggingEvent(LoggingEvent loggingEvent, string encryptedLoggingMessage, string encryptedExceptionMessage = null);

	LoggingEvent CreateErrorEvent(string ErrorMessage);
}
