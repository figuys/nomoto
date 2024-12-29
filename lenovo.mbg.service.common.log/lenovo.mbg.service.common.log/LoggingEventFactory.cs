using System;
using System.Threading;
using log4net.Core;

namespace lenovo.mbg.service.common.log;

public class LoggingEventFactory : ILoggingEventFactory
{
	public LoggingEvent CreateEncryptedLoggingEvent(LoggingEvent loggingEvent, string encryptedLoggingMessage, string encryptedExceptionMessage = null)
	{
		if (loggingEvent == null)
		{
			throw new ArgumentNullException("source");
		}
		LoggingEventData loggingEventData = loggingEvent.GetLoggingEventData();
		loggingEventData.Message = encryptedLoggingMessage;
		if (!string.IsNullOrWhiteSpace(encryptedExceptionMessage))
		{
			loggingEventData.ExceptionString = encryptedExceptionMessage;
		}
		return new LoggingEvent(loggingEventData);
	}

	public LoggingEvent CreateErrorEvent(string ErrorMessage)
	{
		LoggingEventData data = default(LoggingEventData);
		data.Domain = "Software Fix";
		data.ExceptionString = ErrorMessage;
		data.Level = Level.Error;
		data.LoggerName = "lenovo.mbg.service.common.log.LogAesEncrypt";
		data.Message = "lenovo.mbg.service.common.log.LogAesEncrypt";
		data.ThreadName = Thread.CurrentThread.Name;
		data.TimeStampUtc = DateTime.Now.ToUniversalTime();
		data.UserName = Environment.UserName;
		return new LoggingEvent(data);
	}
}
