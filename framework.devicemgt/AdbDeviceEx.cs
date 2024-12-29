using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt.DeviceInfo;
using lenovo.mbg.service.framework.devicemgt.DeviceOperator;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.framework.devicemgt;

public class AdbDeviceEx : TcpAndroidDevice
{
	private class TcpConnectHandler
	{
		protected AdbDeviceEx outer;

		public string AppPackageName { get; set; }

		public ConnectErrorCode VersionMatchCode { get; set; }

		public TcpConnectHandler(AdbDeviceEx outer, string appPackageName)
		{
			this.outer = outer;
			AppPackageName = appPackageName;
			outer.ConnectedAppType = "Ma";
		}

		protected virtual Dictionary<string, object> GetAppVersionCode()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("code", 0);
			dictionary.Add("name", null);
			outer.AppVersion = 0;
			string command = "shell dumpsys package " + AppPackageName;
			string text = outer.DeviceOperator.Command(command, 10000, outer.Identifer);
			if (!string.IsNullOrEmpty(text))
			{
				Match match = Regex.Match(text, "(?<key>versioncode=)(?<value>\\d*)(\\s*.*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				Match match2 = Regex.Match(text, "(?<key>versionname=)(?<value>[\\d\\.]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
				string value = match.Groups["value"].Value;
				string value2 = match2.Groups["value"].Value;
				int.TryParse(value, out var result);
				dictionary["code"] = result;
				dictionary["name"] = value2;
				outer.AppVersion = result;
			}
			return dictionary;
		}

		public virtual void AppVersionIsMatched()
		{
			Dictionary<string, object> appVersionCode = GetAppVersionCode();
			int num = int.Parse(appVersionCode["code"].ToString());
			VersionMatchCode = ((Configurations.AppVersionCode <= num) ? ConnectErrorCode.OK : ConnectErrorCode.AppVersionNotMatched);
			LogHelper.LogInstance.Info(string.Format("Client version: {0}, App version: {1}, Name: {2}", Configurations.AppVersionCode, num, appVersionCode["name"]));
		}

		public virtual ConnectErrorCode InstallApp()
		{
			if (outer.PhysicalStatus != DevicePhysicalStateEx.Online || outer.WorkType.HasFlag(DeviceWorkType.Rescue))
			{
				return ConnectErrorCode.Unknown;
			}
			if (outer.InstallAppCallback == null || outer.InstallAppCallback())
			{
				return Install();
			}
			LogHelper.LogInstance.Debug("User refuses to install app");
			return ConnectErrorCode.ApkInstallFail;
		}

		public ConnectErrorCode Install()
		{
			ConnectErrorCode result = ConnectErrorCode.Unknown;
			LogHelper.LogInstance.Info("Begin install app");
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "apk", "lmsa.apk");
			LogHelper.LogInstance.Debug("LMSA apk path : " + text);
			if (File.Exists(text))
			{
				string text2 = outer.DeviceOperator.Command($"install -r \"{text}\"", 20000, outer.Identifer);
				LogHelper.LogInstance.Info("Apk install cmd response:" + text2);
				text2 = ((text2 != null) ? text2.ToUpper() : string.Empty);
				if (text2.Contains("SUCCESS"))
				{
					result = ConnectErrorCode.OK;
				}
				else
				{
					if (!text2.Contains("NO SPACE LEFT ON DEVICE") && !text2.Contains("NOT ENOUGH SPACE"))
					{
						AppVersionIsMatched();
						if (VersionMatchCode != ConnectErrorCode.AppVersionNotMatched)
						{
							return ConnectErrorCode.OK;
						}
						return ConnectErrorCode.ApkInstallFail;
					}
					result = ConnectErrorCode.ApkInstallFailWithHaveNoSpace;
				}
			}
			else
			{
				LogHelper.LogInstance.Info("Apk file not existed:" + text);
			}
			return result;
		}

		public virtual ConnectErrorCode UnInstallApp()
		{
			if (outer.PhysicalStatus != DevicePhysicalStateEx.Online)
			{
				return ConnectErrorCode.Unknown;
			}
			LogHelper.LogInstance.Debug("Begin uninstall apk");
			string text = outer.DeviceOperator.Command("uninstall " + AppPackageName, 20000, outer.Identifer);
			LogHelper.LogInstance.Info("Uninstall apk,command response value:" + text);
			return ConnectErrorCode.OK;
		}

		public virtual ConnectErrorCode LoadProperty()
		{
			try
			{
				AndroidDeviceProperty androidDeviceProperty = new AndroidDeviceProperty(new PropInfoLoader().LoadAll(outer));
				outer.Property = androidDeviceProperty;
				if (string.IsNullOrEmpty(androidDeviceProperty.SN))
				{
					string text = outer.DeviceOperator.Command("shell getprop", -1, outer.Identifer);
					if (!string.IsNullOrEmpty(text))
					{
						using StringReader stringReader = new StringReader(text);
						(new char[1])[0] = ':';
						string text2 = null;
						string[] array = new string[AndroidDeviceProperty.SN_PROP_FIELDS.Length];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = "[" + AndroidDeviceProperty.SN_PROP_FIELDS[i] + "]";
						}
						bool flag = false;
						while ((text2 = stringReader.ReadLine()) != null && !flag)
						{
							string empty = string.Empty;
							for (int j = 0; j < array.Count(); j++)
							{
								empty = array[j];
								if (text2.StartsWith(empty))
								{
									string value = text2.Substring(text2.LastIndexOf('[') + 1).TrimEnd(']');
									if (!string.IsNullOrEmpty(value))
									{
										flag = true;
										androidDeviceProperty.AddOrUpdate(new PropItem
										{
											Key = AndroidDeviceProperty.SN_PROP_FIELDS[j],
											Value = value
										});
										break;
									}
								}
							}
						}
					}
				}
				if (string.IsNullOrEmpty(androidDeviceProperty.GetPropertyValue("ro.build.version.full")))
				{
					string text3 = outer.DeviceOperator.Command("shell getprop ro.build.version.full", -1, outer.Identifer);
					if (string.IsNullOrEmpty(text3))
					{
						text3 = "";
					}
					androidDeviceProperty.AddOrUpdate(new PropItem
					{
						Key = "ro.build.version.full",
						Value = text3.Replace("\r\n", "")
					});
				}
				return ConnectErrorCode.OK;
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Load data throw exception:" + ex.ToString());
				return ConnectErrorCode.PropertyLoadFail;
			}
		}

		public virtual void FocuseApp()
		{
			string text = outer.DeviceOperator.Command("shell \"dumpsys window | grep mCurrentFocus\"", -1, outer.Identifer);
			LogHelper.LogInstance.Debug("shell \"dumpsys window | grep mCurrentFocus\". Response:[" + text + "]");
			if (!text.Contains("com.lmsa.lmsaappclient"))
			{
				outer.DeviceOperator.Command("shell am start -n " + AppPackageName + "/" + AppPackageName + ".ui.FlashActivity", -1, outer.Identifer);
			}
		}

		public virtual ConnectErrorCode StartApp()
		{
			WifiDeviceData serviceEndPointInfo = outer.serviceAndLocalForwardEndPoint.ServiceEndPointInfo;
			string text = $"shell am start -n {AppPackageName}/{AppPackageName}.ui.FlashActivity --ei cmdPort {serviceEndPointInfo.CmdPort} --ei dataPort {serviceEndPointInfo.DataPort} --ei extendDataPort {serviceEndPointInfo.ExtendDataPort}";
			LogHelper.LogInstance.Debug("Start app cmd:" + text);
			string text2 = outer.DeviceOperator.Command(text, -1, outer.Identifer);
			LogHelper.LogInstance.Debug("Start app response:" + text2);
			string command = $"shell am broadcast -a SocketServiceStart -n {AppPackageName}/{AppPackageName}.service.SocketBroadcastReceiver --ei cmdPort {serviceEndPointInfo.CmdPort} --ei dataPort {serviceEndPointInfo.DataPort} --ei extendDataPort {serviceEndPointInfo.ExtendDataPort}";
			string text3 = outer.DeviceOperator.Command(command, -1, outer.Identifer);
			LogHelper.LogInstance.Debug("Broadcast response:" + text3);
			if (text2.ToUpper().Contains("ERROR") || text2.ToUpper().Contains("DOES NOT EXIST"))
			{
				return ConnectErrorCode.LaunchAppFail;
			}
			return ConnectErrorCode.OK;
		}

		public virtual bool CheckPermissionsPerpare()
		{
			FocuseApp();
			return true;
		}

		public virtual int GetCreateMessageTimeout()
		{
			return 300000;
		}

		public virtual void CallAppToFrontstage()
		{
			outer.DeviceOperator.Command("shell am start -n " + AppPackageName + "/" + AppPackageName + ".ui.FlashActivity", -1, outer.Identifer);
		}
	}

	private class MotoTcpConnectHandler : TcpConnectHandler
	{
		public MotoTcpConnectHandler(AdbDeviceEx outer, string appPackageName)
			: base(outer, appPackageName)
		{
			outer.ConnectedAppType = "Moto";
		}

		public override void AppVersionIsMatched()
		{
			Dictionary<string, object> appVersionCode = GetAppVersionCode();
			int num = int.Parse(appVersionCode["code"].ToString());
			LogHelper.LogInstance.Info(string.Format("Device Helper Version Code: {0},  Name: {1}", num, appVersionCode["name"]));
			bool flag = num >= Configurations.AppMinVersionCodeOfMoto;
			base.VersionMatchCode = (flag ? ConnectErrorCode.OK : ConnectErrorCode.AppVersionNotMatched);
		}

		public override ConnectErrorCode InstallApp()
		{
			return ConnectErrorCode.OK;
		}

		public override ConnectErrorCode UnInstallApp()
		{
			return ConnectErrorCode.OK;
		}

		public override void FocuseApp()
		{
		}

		public override ConnectErrorCode StartApp()
		{
			outer.DeviceOperator.Command("shell am force-stop " + base.AppPackageName, -1, outer.Identifer);
			WifiDeviceData serviceEndPointInfo = outer.serviceAndLocalForwardEndPoint.ServiceEndPointInfo;
			string text = $"shell am start -n {base.AppPackageName}/{base.AppPackageName}.main.start.LaunchActivity --ei cmdPort {serviceEndPointInfo.CmdPort} --ei dataPort {serviceEndPointInfo.DataPort} --ei extendDataPort {serviceEndPointInfo.ExtendDataPort}";
			LogHelper.LogInstance.Debug("Start app cmd:" + text);
			string text2 = outer.DeviceOperator.Command(text, -1, outer.Identifer);
			LogHelper.LogInstance.Debug("Start app response:" + text2);
			if (text2.ToUpper().Contains("ERROR") || text2.ToUpper().Contains("DOES NOT EXIST"))
			{
				return ConnectErrorCode.LaunchAppFail;
			}
			return ConnectErrorCode.OK;
		}

		public override bool CheckPermissionsPerpare()
		{
			return true;
		}

		public override int GetCreateMessageTimeout()
		{
			return 300000;
		}

		public override void CallAppToFrontstage()
		{
			outer.DeviceOperator.Command("shell am start -n " + base.AppPackageName + "/" + base.AppPackageName + ".main.start.LaunchActivity", -1, outer.Identifer);
		}
	}

	private class TcpConnectHandlerFactroy
	{
		public const string MA_APP_PACKAGE_NAME = "com.lmsa.lmsaappclient";

		public const string MOTO_APP_PACKAGE_NAME = "com.motorola.genie";

		private AdbDeviceEx outer;

		public TcpConnectHandlerFactroy(AdbDeviceEx outer)
		{
			this.outer = outer;
		}

		public TcpConnectHandler Create()
		{
			MotoTcpConnectHandler motoTcpConnectHandler = new MotoTcpConnectHandler(outer, "com.motorola.genie");
			motoTcpConnectHandler.AppVersionIsMatched();
			if (ConnectErrorCode.OK == motoTcpConnectHandler.VersionMatchCode)
			{
				LogHelper.LogInstance.Debug("create moto helper connect handler");
				return motoTcpConnectHandler;
			}
			LogHelper.LogInstance.Debug("create ma connect handler");
			return new TcpConnectHandler(outer, "com.lmsa.lmsaappclient");
		}

		public TcpConnectHandler CreateMA()
		{
			LogHelper.LogInstance.Debug("create ma connect handler");
			return new TcpConnectHandler(outer, "com.lmsa.lmsaappclient");
		}
	}

	private class ServiceAndLocalForwardEndPoint
	{
		private AdbDeviceEx outer;

		public WifiDeviceData ServiceEndPointInfo { get; private set; }

		public WifiDeviceData ForwardEndPointInfo { get; private set; }

		private int GetAvailablePort()
		{
			return NetworkUtility.GetAvailablePort(10000, 20000);
		}

		public ServiceAndLocalForwardEndPoint(AdbDeviceEx outer)
		{
			this.outer = outer;
			ServiceEndPointInfo = new WifiDeviceData
			{
				Ip = IPAddress.Loopback.ToString(),
				CmdPort = GetAvailablePort(),
				DataPort = GetAvailablePort(),
				ExtendDataPort = GetAvailablePort()
			};
			ForwardEndPointInfo = new WifiDeviceData
			{
				Ip = IPAddress.Loopback.ToString(),
				CmdPort = GetAvailablePort(),
				DataPort = GetAvailablePort(),
				ExtendDataPort = GetAvailablePort()
			};
		}

		public void ForwardPort()
		{
			LogHelper.LogInstance.Info($"adb device fordward: [id: {outer.Identifer}], [cmdPort: {ForwardEndPointInfo.CmdPort}], [dataPort:{ForwardEndPointInfo.DataPort}], [extendDataPort:{ForwardEndPointInfo.ExtendDataPort}]");
			outer.DeviceOperator.ForwardPort(outer.Identifer, ServiceEndPointInfo.CmdPort, ForwardEndPointInfo.CmdPort);
			outer.DeviceOperator.ForwardPort(outer.Identifer, ServiceEndPointInfo.DataPort, ForwardEndPointInfo.DataPort);
			outer.DeviceOperator.ForwardPort(outer.Identifer, ServiceEndPointInfo.ExtendDataPort, ForwardEndPointInfo.ExtendDataPort);
		}
	}

	private TcpConnectHandlerFactroy tcpConnectHandlerFactroy;

	private TcpConnectHandler _handler;

	private ServiceAndLocalForwardEndPoint _serviceAndLocalForwardEndPoint;

	private TcpConnectHandler handler
	{
		get
		{
			if (_handler == null)
			{
				_handler = tcpConnectHandlerFactroy.Create();
			}
			return _handler;
		}
		set
		{
			_handler = value;
		}
	}

	private ServiceAndLocalForwardEndPoint serviceAndLocalForwardEndPoint
	{
		get
		{
			lock (this)
			{
				if (_serviceAndLocalForwardEndPoint == null)
				{
					_serviceAndLocalForwardEndPoint = new ServiceAndLocalForwardEndPoint(this);
					_serviceAndLocalForwardEndPoint.ForwardPort();
				}
				return _serviceAndLocalForwardEndPoint;
			}
		}
		set
		{
			_serviceAndLocalForwardEndPoint = value;
		}
	}

	public AdbDeviceEx()
	{
		base.DeviceOperator = new AdbOperator();
		tcpConnectHandlerFactroy = new TcpConnectHandlerFactroy(this);
		Property = new AndroidDeviceProperty();
	}

	public override bool FocuseApp()
	{
		handler.FocuseApp();
		Thread.Sleep(3000);
		return true;
	}

	public override ConnectErrorCode LoadProperty()
	{
		return handler.LoadProperty();
	}

	protected override WifiDeviceData GetServiceHostEndPointInfo()
	{
		return serviceAndLocalForwardEndPoint.ForwardEndPointInfo;
	}

	protected override ConnectErrorCode CheckAppVersion()
	{
		_serviceAndLocalForwardEndPoint = null;
		handler = tcpConnectHandlerFactroy.Create();
		if (handler is MotoTcpConnectHandler)
		{
			return handler.VersionMatchCode;
		}
		handler.AppVersionIsMatched();
		return handler.VersionMatchCode;
	}

	public override ConnectErrorCode StartApp()
	{
		int num = 0;
		ConnectErrorCode connectErrorCode = ConnectErrorCode.LaunchAppFail;
		do
		{
			connectErrorCode = handler.StartApp();
			if (connectErrorCode == ConnectErrorCode.OK)
			{
				break;
			}
			Thread.Sleep(1000);
		}
		while (++num < 5);
		if (connectErrorCode == ConnectErrorCode.LaunchAppFail && base.ConnectedAppType == "Moto")
		{
			LogHelper.LogInstance.Warn("Startup moto helper error! Switch to MA connect handler!");
			handler = tcpConnectHandlerFactroy.CreateMA();
			ConnectErrorCode connectErrorCode2 = ConnectErrorCode.Unknown;
			for (int i = 0; i < 5; i++)
			{
				handler.AppVersionIsMatched();
				connectErrorCode2 = handler.VersionMatchCode;
				if (ConnectErrorCode.OK == connectErrorCode2)
				{
					break;
				}
				Thread.Sleep(1000);
			}
			if (connectErrorCode2 != ConnectErrorCode.OK)
			{
				InstallApp();
			}
			connectErrorCode = handler.StartApp();
		}
		return connectErrorCode;
	}

	protected override ConnectErrorCode InstallApp()
	{
		return handler.InstallApp();
	}

	public override void CallAppToFrontstage()
	{
		handler.CallAppToFrontstage();
	}

	protected override ConnectErrorCode UninstallApp()
	{
		return handler.UnInstallApp();
	}

	protected override bool CheckPermissionsPerpare()
	{
		return handler.CheckPermissionsPerpare();
	}

	public override void Load()
	{
		LoadProperty();
	}

	public void ForceInstallMa()
	{
		TcpConnectHandler tcpConnectHandler = new TcpConnectHandler(new AdbDeviceEx
		{
			Identifer = base.Identifer
		}, "com.lmsa.lmsaappclient");
		tcpConnectHandler.AppVersionIsMatched();
		if (tcpConnectHandler.VersionMatchCode != ConnectErrorCode.OK)
		{
			tcpConnectHandler.Install();
			_ = 1;
		}
	}
}
