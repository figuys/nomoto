using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace lenovo.mbg.service.lmsa;

public static class SingleInstance<TApplication> where TApplication : Application, ISingleInstanceApp
{
	private class IPCRemoteService : MarshalByRefObject
	{
		public void InvokeFirstInstance(IList<string> args)
		{
			if (Application.Current != null)
			{
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(SingleInstance<TApplication>.ActivateFirstInstanceCallback), args);
			}
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}
	}

	private const string Delimiter = ":";

	private const string ChannelNameSuffix = "SingeInstanceIPCChannel";

	private const string RemoteServiceName = "SingleInstanceApplicationService";

	private const string IpcProtocol = "ipc://";

	private static Mutex singleInstanceMutex;

	private static IpcServerChannel channel;

	private static IList<string> commandLineArgs;

	public static IList<string> CommandLineArgs => commandLineArgs;

	public static bool InitializeAsFirstInstance(string uniqueName)
	{
		try
		{
			commandLineArgs = GetCommandLineArgs(uniqueName);
			string text = uniqueName + Environment.UserName;
			string channelName = text + ":" + "SingeInstanceIPCChannel";
			RestartCheck(commandLineArgs);
			singleInstanceMutex = new Mutex(initiallyOwned: true, text, out var createdNew);
			if (createdNew)
			{
				CreateRemoteService(channelName);
			}
			else
			{
				SignalFirstInstance(channelName, commandLineArgs);
			}
			return createdNew;
		}
		catch (Exception)
		{
			return true;
		}
	}

	public static void Cleanup()
	{
		if (singleInstanceMutex != null)
		{
			singleInstanceMutex.Close();
			singleInstanceMutex = null;
		}
		if (channel != null)
		{
			ChannelServices.UnregisterChannel(channel);
			channel = null;
		}
	}

	private static IList<string> GetCommandLineArgs(string uniqueApplicationName)
	{
		string[] array = null;
		if (AppDomain.CurrentDomain.ActivationContext == null)
		{
			array = Environment.GetCommandLineArgs();
		}
		else
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), uniqueApplicationName);
			string path2 = Path.Combine(path, "cmdline.txt");
			if (File.Exists(path2))
			{
				try
				{
					using (TextReader textReader = new StreamReader(path2, Encoding.Unicode))
					{
						array = NativeMethods.CommandLineToArgvW(textReader.ReadToEnd());
					}
					File.Delete(path2);
				}
				catch (IOException)
				{
				}
			}
		}
		if (array == null)
		{
			array = new string[0];
		}
		return new List<string>(array);
	}

	private static void RestartCheck(IList<string> commandLineArgs)
	{
		if (!commandLineArgs.Contains("restart") || commandLineArgs.Count != 3)
		{
			return;
		}
		int oldProcessId = -1;
		if (!int.TryParse(commandLineArgs[2], out oldProcessId))
		{
			return;
		}
		SpinWait.SpinUntil(delegate
		{
			try
			{
				Process processById = Process.GetProcessById(oldProcessId);
				return processById == null;
			}
			catch (Exception)
			{
				return true;
			}
		}, -1);
	}

	private static void CreateRemoteService(string channelName)
	{
		BinaryServerFormatterSinkProvider binaryServerFormatterSinkProvider = new BinaryServerFormatterSinkProvider();
		binaryServerFormatterSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;
		IDictionary dictionary = new Dictionary<string, string>();
		dictionary["name"] = channelName;
		dictionary["portName"] = channelName;
		dictionary["exclusiveAddressUse"] = "false";
		channel = new IpcServerChannel(dictionary, binaryServerFormatterSinkProvider);
		ChannelServices.RegisterChannel(channel, ensureSecurity: true);
		IPCRemoteService obj = new IPCRemoteService();
		RemotingServices.Marshal(obj, "SingleInstanceApplicationService");
	}

	private static void SignalFirstInstance(string channelName, IList<string> args)
	{
		IpcClientChannel chnl = new IpcClientChannel();
		ChannelServices.RegisterChannel(chnl, ensureSecurity: true);
		string url = "ipc://" + channelName + "/SingleInstanceApplicationService";
		((IPCRemoteService)RemotingServices.Connect(typeof(IPCRemoteService), url))?.InvokeFirstInstance(args);
	}

	private static object ActivateFirstInstanceCallback(object arg)
	{
		IList<string> args = arg as IList<string>;
		ActivateFirstInstance(args);
		return null;
	}

	private static void ActivateFirstInstance(IList<string> args)
	{
		if (Application.Current != null)
		{
			((TApplication)Application.Current).SignalExternalCommandLineArgs(args);
		}
	}
}
