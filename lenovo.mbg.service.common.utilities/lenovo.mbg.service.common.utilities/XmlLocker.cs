using System;
using System.Threading;

namespace lenovo.mbg.service.common.utilities;

public class XmlLocker
{
	private static readonly string MUTEX_NAME_STRING = "Global\\XmlLocker";

	private static Mutex s_Mutex = null;

	private static XmlLocker s_Instance = null;

	public static XmlLocker Instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = new XmlLocker();
				try
				{
					s_Mutex = Mutex.OpenExisting(MUTEX_NAME_STRING);
				}
				catch (WaitHandleCannotBeOpenedException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}
				catch (Exception)
				{
				}
				if (s_Mutex == null)
				{
					s_Mutex = new Mutex(initiallyOwned: false, MUTEX_NAME_STRING);
				}
			}
			return s_Instance;
		}
	}

	private XmlLocker()
	{
	}

	public void ThreadSafeOperation(Action a)
	{
		if (a != null)
		{
			try
			{
				s_Mutex.WaitOne();
				a();
			}
			finally
			{
				s_Mutex.ReleaseMutex();
			}
		}
	}
}
