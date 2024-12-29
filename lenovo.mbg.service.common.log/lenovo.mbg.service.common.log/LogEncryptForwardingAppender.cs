using System;
using System.Linq;
using log4net.Appender;
using log4net.Core;

namespace lenovo.mbg.service.common.log;

public class LogEncryptForwardingAppender : ForwardingAppender
{
	public ILogEncrypt LogEncrypt { get; set; }

	public ILoggingEventFactory LogEventFactory { get; set; }

	public LogEncryptForwardingAppender()
	{
		LogEncrypt = new LogAesEncrypt();
		LogEventFactory = new LoggingEventFactory();
	}

	protected override void Append(LoggingEvent loggingEvent)
	{
		LoggingEvent loggingEvent2 = GenerateEncryptedLogEvent(loggingEvent);
		base.Append(loggingEvent2);
	}

	protected override void Append(LoggingEvent[] loggingEvents)
	{
		LoggingEvent[] loggingEvents2 = loggingEvents.Select((LoggingEvent x) => GenerateEncryptedLogEvent(x)).ToArray();
		base.Append(loggingEvents2);
	}

	protected virtual LoggingEvent GenerateEncryptedLogEvent(LoggingEvent loggingEvent)
	{
		try
		{
			string text = $"{loggingEvent.TimeStamp:G}";
			string text2 = loggingEvent.ThreadName.PadLeft(2);
			string text3 = loggingEvent.Level.ToString().PadLeft(5);
			string renderedMessage = loggingEvent.RenderedMessage;
			string content = text + " [" + text2 + "] [" + text3 + "] " + renderedMessage;
			string encryptedLoggingMessage = LogEncrypt.Encrypt(content);
			string exceptionString = loggingEvent.GetExceptionString();
			string encryptedExceptionMessage = null;
			if (!string.IsNullOrWhiteSpace(exceptionString))
			{
				encryptedExceptionMessage = LogEncrypt.Encrypt(exceptionString);
			}
			return LogEventFactory.CreateEncryptedLoggingEvent(loggingEvent, encryptedLoggingMessage, encryptedExceptionMessage);
		}
		catch (Exception ex)
		{
			return LogEventFactory.CreateErrorEvent(ex.Message);
		}
	}
}
