using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;

namespace lenovo.mbg.service.framework.devicemgt;

public abstract class TcpAndroidDevice : DeviceEx
{
	public int RetryConnect = 1;

	private TcpConnectStepChangedEventArgs currentTcpConnectStepChangedEventArgs;

	private AppAssistTipsEventArgs currentAppAssistTips;

	private PermissionsCheckConfirmEventArgs currentPermissionsCheckConfirmEventArgs;

	private AutoResetEvent autoResetEvent;

	protected volatile bool isconnect;

	public IPEndPoint ExtendDataFileServiceEndPoint { get; set; }

	public IMessageManager MessageManager { get; set; }

	public IFileTransferManager FileTransferManager { get; set; }

	public override IAndroidDevice Property { get; set; }

	public bool? IsReady { get; protected set; }

	public RsaSocketDataSecurityFactory RsaSocketEncryptHelper { get; private set; }

	private event EventHandler<TcpConnectStepChangedEventArgs> tcpConnectStepChanged;

	public event EventHandler<TcpConnectStepChangedEventArgs> TcpConnectStepChanged
	{
		add
		{
			tcpConnectStepChanged += value;
			if (currentTcpConnectStepChangedEventArgs != null)
			{
				value(this, currentTcpConnectStepChangedEventArgs);
			}
		}
		remove
		{
			tcpConnectStepChanged -= value;
		}
	}

	private event EventHandler<AppAssistTipsEventArgs> appAssistTips;

	public event EventHandler<AppAssistTipsEventArgs> AppAssistTips
	{
		add
		{
			appAssistTips += value;
			value.BeginInvoke(this, currentAppAssistTips, null, null);
		}
		remove
		{
			appAssistTips -= value;
		}
	}

	private event EventHandler<PermissionsCheckConfirmEventArgs> permissionsCheckConfirmEvent;

	public event EventHandler<PermissionsCheckConfirmEventArgs> PermissionsCheckConfirmEvent
	{
		add
		{
			permissionsCheckConfirmEvent += value;
			value(this, currentPermissionsCheckConfirmEventArgs);
		}
		remove
		{
			permissionsCheckConfirmEvent -= value;
		}
	}

	protected void FireTcpConnectStepChangedEvent(object sender, TcpConnectStepChangedEventArgs e)
	{
		lock (this)
		{
			EventHandler<TcpConnectStepChangedEventArgs> eventHandler = this.tcpConnectStepChanged;
			currentTcpConnectStepChangedEventArgs = e;
			if (eventHandler != null)
			{
				LogHelper.LogInstance.Info($"====>>Device {(sender as TcpAndroidDevice).Identifer} connecting info: step={e.Step}, result={e.Result}, code={e.ErrorCode}");
				Delegate[] invocationList = eventHandler.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					((EventHandler<TcpConnectStepChangedEventArgs>)invocationList[i]).BeginInvoke(this, e, null, null);
				}
			}
		}
	}

	protected void FireAssistTipsEvent(object sender, AppAssistTipsEventArgs e)
	{
		currentAppAssistTips = e;
		if (this.appAssistTips != null)
		{
			Delegate[] invocationList = this.appAssistTips.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((EventHandler<AppAssistTipsEventArgs>)invocationList[i]).BeginInvoke(this, e, null, null);
			}
		}
	}

	protected void FirePermissionsCheckConfirmEvent(object sender, PermissionsCheckConfirmEventArgs e)
	{
		currentPermissionsCheckConfirmEventArgs = e;
		if (this.permissionsCheckConfirmEvent != null)
		{
			Delegate[] array = this.permissionsCheckConfirmEvent?.GetInvocationList();
			for (int i = 0; i < array.Length; i++)
			{
				((EventHandler<PermissionsCheckConfirmEventArgs>)array[i]).BeginInvoke(this, e, null, null);
			}
		}
	}

	protected abstract bool CheckPermissionsPerpare();

	public bool CheckPermissions(List<string> permissionModules)
	{
		if (!CheckPermissionsPerpare() || MessageManager == null)
		{
			return false;
		}
		using MessageReaderAndWriter messageReaderAndWriter = MessageManager.CreateMessageReaderAndWriter();
		if (messageReaderAndWriter == null)
		{
			return false;
		}
		List<PropItem> receiveData = null;
		if (!messageReaderAndWriter.SendAndReceiveSync("checkPermissions", permissionModules, Sequence.SingleInstance.New(), out receiveData))
		{
			return false;
		}
		return receiveData?.Exists((PropItem m) => "true".Equals(m.Value)) ?? false;
	}

	protected override void OnPhysicalStatusChanged(DevicePhysicalStateEx prev, DevicePhysicalStateEx current)
	{
		if (current == DevicePhysicalStateEx.Online)
		{
			startConnect();
		}
		base.OnPhysicalStatusChanged(prev, current);
	}

	protected override void OnSoftStatusChanged(DeviceSoftStateEx prev, DeviceSoftStateEx current)
	{
		if (current == DeviceSoftStateEx.Offline)
		{
			MessageManager?.Dispose();
		}
		base.OnSoftStatusChanged(prev, current);
	}

	~TcpAndroidDevice()
	{
		autoResetEvent?.Dispose();
	}

	private void startConnect()
	{
		if (base.PhysicalStatus != DevicePhysicalStateEx.Online)
		{
			return;
		}
		Thread.Sleep(new Random().Next(100));
		if (isconnect)
		{
			return;
		}
		isconnect = true;
		Task.Run(delegate
		{
			try
			{
				do
				{
					if (base.ConnectType == ConnectType.Wifi)
					{
						RetryConnect = -1;
					}
					autoResetEvent = new AutoResetEvent(initialState: false);
					base.SoftStatus = DeviceSoftStateEx.Connecting;
					TcpConnectByStep(0, 0);
					autoResetEvent.WaitOne();
					RetryConnect--;
					if (base.ConnectedAppType == "Ma" || base.SoftStatus == DeviceSoftStateEx.Online)
					{
						RetryConnect = -1;
					}
					if (RetryConnect < 0)
					{
						break;
					}
					base.RetryConnectCallback?.Invoke();
				}
				while (RetryConnect >= 0);
			}
			finally
			{
				isconnect = false;
			}
		});
	}

	protected abstract ConnectErrorCode CheckAppVersion();

	public abstract ConnectErrorCode StartApp();

	public abstract void CallAppToFrontstage();

	protected abstract ConnectErrorCode InstallApp();

	protected abstract ConnectErrorCode UninstallApp();

	protected abstract WifiDeviceData GetServiceHostEndPointInfo();

	protected virtual MessageManager CreateMessageManager(IPEndPointInfo endPointInfo, out ConnectErrorCode errorCode)
	{
		int result = 0;
		bool running = true;
		bool isSecutiry = false;
		bool flag = false;
		if (base.ConnectedAppType == "Moto")
		{
			if (int.TryParse(Configurations.MotoHelperSecurityVersion, out result))
			{
				isSecutiry = base.AppVersion >= result;
			}
			if (int.TryParse(Configurations.MotoApkRandomKeyVersion, out result))
			{
				flag = base.AppVersion >= result;
			}
		}
		else
		{
			isSecutiry = true;
			flag = true;
		}
		RsaSocketEncryptHelper = new RsaSocketDataSecurityFactory(isSecutiry, flag);
		MessageManager msgManager = new MessageManager(endPointInfo, RsaSocketEncryptHelper);
		Task<ConnectErrorCode> task = Task.Run(delegate
		{
			ConnectErrorCode connectErrorCode = ConnectErrorCode.Unknown;
			while (base.PhysicalStatus == DevicePhysicalStateEx.Online)
			{
				MessageReaderAndWriter messageReaderAndWriter = null;
				try
				{
					messageReaderAndWriter = msgManager.CreateMessageReaderAndWriter(25000);
					Thread.Sleep(5000);
					if (messageReaderAndWriter != null)
					{
						List<PropItem> receiveData = null;
						if (messageReaderAndWriter.SendAndReceiveSync<object, PropItem>("ping", "pingResponse", null, Sequence.SingleInstance.New(), out receiveData) && receiveData != null)
						{
							connectErrorCode = ((receiveData.FirstOrDefault((PropItem m) => "deny".Equals(m.Value)) == null) ? ConnectErrorCode.OK : ConnectErrorCode.TcpConnectFailWithAppNotAllowed);
							if (connectErrorCode == ConnectErrorCode.OK)
							{
								break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogHelper.LogInstance.Warn("Connect and ping failed:" + ex);
				}
				finally
				{
					messageReaderAndWriter?.Dispose();
				}
				Thread.Sleep(1000);
				if (!running)
				{
					break;
				}
			}
			return connectErrorCode;
		});
		LogHelper.LogInstance.Info("====>>" + base.Identifer + " show validate code window!");
		base.BeforeValidateAction?.Invoke(new Tuple<bool, string>(flag, base.Identifer));
		bool flag2 = task.Wait(50000);
		errorCode = (flag2 ? task.Result : ConnectErrorCode.TcpConnectFailWithTimeout);
		if (!flag2)
		{
			running = false;
		}
		base.AfterValidateAction?.Invoke(new Tuple<bool, string>(flag, base.Identifer));
		LogHelper.LogInstance.Info("====>>" + base.Identifer + " close validate code window!");
		if (errorCode != ConnectErrorCode.OK && msgManager != null)
		{
			msgManager.Dispose();
		}
		return msgManager;
	}

	private void TcpConnectByStep(int connectCount, int currentPercent)
	{
		ConnectErrorCode errorCode = ConnectErrorCode.Unknown;
		if (connectCount >= 2)
		{
			base.SoftStatus = DeviceSoftStateEx.Offline;
			autoResetEvent.Set();
			return;
		}
		FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("AppVersionIsMatched", 1, ConnectStepStatus.Starting, errorCode, ref currentPercent));
		int num = 0;
		while (ConnectErrorCode.OK != (errorCode = CheckAppVersion()))
		{
			Thread.Sleep(1000);
			if (++num >= 5)
			{
				break;
			}
		}
		if (ConnectErrorCode.OK == errorCode)
		{
			FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("AppVersionIsMatched", 1, ConnectStepStatus.Success, errorCode, ref currentPercent));
			FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("TcpConnect", 4, ConnectStepStatus.Starting, errorCode, ref currentPercent));
			if (ConnectErrorCode.OK == (errorCode = PrivateConnect()))
			{
				int _checkPermissionsFailedType = 0;
				Func<bool> checkPermissionsAction = delegate
				{
					bool flag = CheckPermissions(new List<string> { "ACCESS_ALL_FILES" });
					if (flag)
					{
						flag = CheckPermissions(new List<string> { "Backup" });
						if (!flag)
						{
							_checkPermissionsFailedType = 2;
						}
					}
					else
					{
						_checkPermissionsFailedType = 1;
					}
					return flag;
				};
				Action<bool> havPermissionsFlowingStepActions = delegate(bool havePermissions)
				{
					FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("TcpConnect", 4, ConnectStepStatus.Success, errorCode, ref currentPercent));
					FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("LoadDeviceProperty", 5, ConnectStepStatus.Starting, errorCode, ref currentPercent));
					if (havePermissions && ConnectErrorCode.OK == (errorCode = LoadProperty()))
					{
						FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("LoadDeviceProperty", 5, ConnectStepStatus.Success, errorCode, ref currentPercent));
						base.SoftStatus = DeviceSoftStateEx.Online;
						autoResetEvent.Set();
					}
					else
					{
						currentPercent = 100;
						FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("LoadDeviceProperty", 5, ConnectStepStatus.Fail, errorCode, ref currentPercent));
						base.SoftStatus = DeviceSoftStateEx.Offline;
						autoResetEvent.Set();
					}
				};
				LoadProperty();
				if (checkPermissionsAction())
				{
					havPermissionsFlowingStepActions(obj: true);
					return;
				}
				FirePermissionsCheckConfirmEvent(this, new PermissionsCheckConfirmEventArgs(delegate(bool isContinue)
				{
					if (isContinue)
					{
						if (checkPermissionsAction())
						{
							havPermissionsFlowingStepActions.BeginInvoke(obj: true, null, null);
							return true;
						}
						return false;
					}
					havPermissionsFlowingStepActions.BeginInvoke(obj: false, null, null);
					return false;
				}, _checkPermissionsFailedType));
			}
			else
			{
				currentPercent = 100;
				FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("TcpConnect", 4, ConnectStepStatus.Fail, errorCode, ref currentPercent));
				base.SoftStatus = DeviceSoftStateEx.Offline;
				autoResetEvent.Set();
			}
		}
		else
		{
			FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("UnInstallApp", 2, ConnectStepStatus.Starting, errorCode, ref currentPercent));
			UninstallApp();
			FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("UnInstallApp", 2, ConnectStepStatus.Success, errorCode, ref currentPercent));
			FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("InstallApp", 3, ConnectStepStatus.Starting, errorCode, ref currentPercent));
			if (ConnectErrorCode.OK == (errorCode = InstallApp()))
			{
				FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("InstallApp", 3, ConnectStepStatus.Success, errorCode, ref currentPercent));
				TcpConnectByStep(++connectCount, currentPercent);
				return;
			}
			currentPercent = 100;
			FireTcpConnectStepChangedEvent(this, CreateTcpConnectStepChangedEventArgs("InstallApp", 3, ConnectStepStatus.Fail, errorCode, ref currentPercent));
			base.SoftStatus = DeviceSoftStateEx.Offline;
			autoResetEvent.Set();
		}
	}

	protected virtual ConnectErrorCode PrivateConnect()
	{
		ConnectErrorCode connectErrorCode = ConnectErrorCode.Unknown;
		if (base.PhysicalStatus != DevicePhysicalStateEx.Online)
		{
			return ConnectErrorCode.Unknown;
		}
		WifiDeviceData serviceHostEndPointInfo = GetServiceHostEndPointInfo();
		MessageManager messageManager = null;
		if (ConnectErrorCode.OK == (connectErrorCode = StartApp()))
		{
			if (base.PhysicalStatus != DevicePhysicalStateEx.Online)
			{
				return ConnectErrorCode.Unknown;
			}
			messageManager = CreateMessageManager(new IPEndPointInfo(serviceHostEndPointInfo.Ip, serviceHostEndPointInfo.CmdPort), out connectErrorCode);
		}
		if (messageManager == null || connectErrorCode != ConnectErrorCode.OK)
		{
			return connectErrorCode;
		}
		MessageManager = messageManager;
		ExtendDataFileServiceEndPoint = new IPEndPoint(IPAddress.Parse(serviceHostEndPointInfo.Ip), serviceHostEndPointInfo.ExtendDataPort);
		messageManager.HeartbeatStopped += MsgManager_HeartbeatStopped;
		FileTransferManager = new FileTransferManager(new IPEndPointInfo(serviceHostEndPointInfo.Ip, serviceHostEndPointInfo.DataPort), RsaSocketEncryptHelper);
		using (MessageReaderAndWriter messageReaderAndWriter = messageManager.CreateMessageReaderAndWriter())
		{
			if (messageReaderAndWriter != null)
			{
				List<PropItem> receiveData = null;
				messageReaderAndWriter.SendAndReceiveSync("setConnectStatus", "setConnectStatusResponse", new List<string> { (base.ConnectType == ConnectType.Adb) ? "4" : "2" }, Sequence.SingleInstance.New(), out receiveData);
				messageManager.StartHeartbeat(1000L, 10000L);
				serviceHostEndPointInfo.IpRSA = (messageManager.GetHeartbeatChannel().RawSocket.LocalEndPoint as IPEndPoint).Address.ToString();
				return ConnectErrorCode.OK;
			}
		}
		return ConnectErrorCode.Unknown;
	}

	private void MsgManager_HeartbeatStopped(object sender, HeartbeatStoppedEventArgs e)
	{
		if (base.ConnectType == ConnectType.Adb && base.PhysicalStatus == DevicePhysicalStateEx.Online && !DeviceConnectionManagerEx.IsLogOut)
		{
			if (base.WorkType != DeviceWorkType.Rescue)
			{
				base.SoftStatus = DeviceSoftStateEx.Offline;
				Thread.Sleep(3000);
				startConnect();
			}
		}
		else
		{
			base.SoftStatus = (e.IsDisconnectedByService ? DeviceSoftStateEx.ManualDisconnect : DeviceSoftStateEx.Offline);
			base.PhysicalStatus = DevicePhysicalStateEx.Offline;
		}
	}

	private TcpConnectStepChangedEventArgs CreateTcpConnectStepChangedEventArgs(string stepName, int currentStepIndex, ConnectStepStatus currentResult, ConnectErrorCode errorCode, ref int currentPercent)
	{
		return new TcpConnectStepChangedEventArgs(stepName, currentResult, errorCode, currentPercent = CalculatePercent(currentPercent, currentStepIndex, currentResult));
	}

	protected virtual int CalculatePercent(int currentPercent, int stepIndex, ConnectStepStatus result)
	{
		int num = 100 - currentPercent;
		switch (result)
		{
		case ConnectStepStatus.Starting:
			currentPercent += (int)((double)num * ((double)(10 + (stepIndex - 1) * 20) / 100.0));
			break;
		case ConnectStepStatus.Success:
			currentPercent += (int)((double)num * ((double)(20 + (stepIndex - 1) * 20) / 100.0));
			break;
		}
		return currentPercent;
	}

	public abstract ConnectErrorCode LoadProperty();

	public abstract bool FocuseApp();

	public override string ToString()
	{
		return $"device info [id:{base.Identifer}, mn:{((Property != null) ? Property.ModelName : string.Empty)},appType:{base.ConnectedAppType},ps:{base.PhysicalStatus},ss:{base.SoftStatus},connectType:{base.ConnectType}]";
	}

	public int GetAndroidMajorVerion()
	{
		string text = Property.AndroidVersion.Trim();
		int num = text.IndexOf('.');
		if (num > 0)
		{
			text = text.Substring(0, num);
		}
		int result = 0;
		int.TryParse(text, out result);
		return result;
	}
}
