using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

using log4net;
using log4net.Config;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.common.log;

public class LogHelper
{
    public static string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");

    private static List<string> m_UnsafeText = new List<string>();

    protected BusinessLog _businessLog;

    private static object locker = new object();

    private static LogHelper m_Instance;

    private ILog Log => LogManager.GetLogger("");

    public static LogHelper LogInstance
    {
        get
        {
            if (m_Instance == null)
            {
                lock (locker)
                {
                    if (m_Instance == null)
                    {
                        Init();
                    }
                }
            }
            return m_Instance;
        }
    }

    public static void SetConfig()
    {
        XmlConfigurator.Configure();
    }

    public static void SetConfig(string configPath)
    {
        XmlConfigurator.Configure(new FileInfo(configPath));
    }

    private static void Init()
    {
        NameValueCollection nameValueCollection = new NameValueCollection();
        nameValueCollection["configType"] = "FILE-WATCH";
        if (AppDomain.CurrentDomain.IsDefaultAppDomain())
        {
            nameValueCollection["configFile"] = ConfigFilePath;
        }
        else
        {
            nameValueCollection["configFile"] = ".\\log4net.plugin.config";
        }

        m_Instance = new LogHelper();
    }

    public void Debug(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Debug(currentMethod + " - " + message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.DEBUG);
        }
    }

    public void Debug(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Debug(currentMethod + " - " + message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.DEBUG, exception);
        }
    }

    public void Info(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Info(currentMethod + " - " + message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.INFO);
        }
    }

    public void Info(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Info(currentMethod + " - " + message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.INFO, exception);
        }
    }

    public void Warn(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Warn(currentMethod + " - " + message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.WARN);
        }
    }

    public void Warn(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Warn(currentMethod + " - " + message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.WARN, exception);
        }
    }

    public void Error(string message, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Error(currentMethod + " - " + message);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.ERROE);
        }
    }

    public void Error(string message, Exception exception, bool upload = false)
    {
        message = MessageDesensitization(message);
        string currentMethod = GetCurrentMethod();
        Log.Error(currentMethod + " - " + message, exception);
        if (upload)
        {
            WriteLogAsync(message, currentMethod, LogLevel.ERROE, exception);
        }
    }

    public void AnalyzeUnsafeText(string _msg)
    {
        if (string.IsNullOrEmpty(_msg))
        {
            return;
        }
        try
        {
            List<string> obj = new List<string> { "content.name", "content.fullName" };
            JObject jObject = JObject.Parse(_msg);
            foreach (string item in obj)
            {
                string text = jObject.SelectToken(item)?.ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    m_UnsafeText.Add(text);
                }
            }
        }
        catch
        {
        }
    }

    private static string MessageDesensitization(string _msg)
    {
        m_UnsafeText.ForEach(delegate (string m)
        {
            _msg = _msg.Replace(m, "***");
        });
        return _msg;
    }

    public void WriteLogForUser(string message, int resultCode)
    {
        try
        {
            message = MessageDesensitization(message);
            string text = "not start";
            switch (resultCode)
            {
                case 0:
                    text = "fail";
                    break;
                case 1:
                    text = "pass";
                    break;
                case 2:
                    text = "quit";
                    break;
            }
            string contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{text}] - {message}{Environment.NewLine}";
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"{DateTime.Now:yyyy-MM}-friendly.log"), contents);
        }
        catch (Exception arg)
        {
            Error($"WriteLogForUser - message:[{message}] exception:[{arg}]");
        }
    }

    private void WriteLogAsync(string message, string method, LogLevel level)
    {
        WriteLogAsync(message, method, level, null);
    }

    private void WriteLogAsync(string message, string method, LogLevel level, Exception exception)
    {
        _businessLog?.Write(method, message, level, exception);
    }

    internal static string GetCurrentMethod()
    {
        return new StackTrace(2, fNeedFileInfo: true).GetFrame(0).GetMethod().Name;
    }

    private LogHelper()
    {
        _businessLog = new BusinessLog();
    }
}
