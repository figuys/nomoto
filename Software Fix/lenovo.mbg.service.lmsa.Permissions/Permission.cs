using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Component;
using lenovo.themes.generic.ControlsV6;
using lenovo.themes.generic.Dialog.Permissions;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.Permissions;

public class Permission : IPermission
{
	public bool? AppModuleIsReady(DeviceEx device, List<string> permissionModuleNames)
	{
		if (device == null || device.SoftStatus != DeviceSoftStateEx.Online)
		{
			return null;
		}
		if (!(device is TcpAndroidDevice tcpAndroidDevice))
		{
			return null;
		}
		MessageReaderAndWriter messageReaderAndWriter = null;
		try
		{
			return tcpAndroidDevice.CheckPermissions(permissionModuleNames);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Check app module is ready failed:" + ex);
		}
		finally
		{
			if (messageReaderAndWriter != null)
			{
				try
				{
					messageReaderAndWriter.Dispose();
					messageReaderAndWriter = null;
				}
				catch
				{
				}
			}
		}
		return null;
	}

	public void BeginCheckAppIsReady(DeviceEx device, List<string> permissionModuleNames, Action<bool?> finalResultRepoter)
	{
		if (device == null || device.SoftStatus != DeviceSoftStateEx.Online || permissionModuleNames == null || permissionModuleNames.Count == 0)
		{
			finalResultRepoter?.Invoke(null);
			return;
		}
		Task.Run(delegate
		{
			bool? obj = AppModuleIsReady(device, permissionModuleNames);
			finalResultRepoter?.Invoke(obj);
		});
	}

	public void BeginRollPollingAppModuleIsReady(DeviceEx device, List<string> permissionModuleNames, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter, int periodTime)
	{
		if (device == null || device.SoftStatus != DeviceSoftStateEx.Online || permissionModuleNames == null || permissionModuleNames.Count == 0)
		{
			finalResultRepoter?.Invoke(null);
			return;
		}
		Task.Run(delegate
		{
			bool? flag = null;
			do
			{
				flag = AppModuleIsReady(device, permissionModuleNames);
				if (flag.HasValue && !flag.Value)
				{
					Thread.Sleep(periodTime);
				}
			}
			while ((cancelToken == null || !cancelToken.IsCancellationRequested) && flag.HasValue && !flag.Value);
			finalResultRepoter?.Invoke(flag);
		});
	}

	public void BeginAutoCheckAppIsReady(DeviceEx device, List<string> permissionModuleNames, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter, int periodTime = 1000)
	{
		if (device == null || device.SoftStatus != DeviceSoftStateEx.Online || permissionModuleNames == null || permissionModuleNames.Count == 0)
		{
			finalResultRepoter?.Invoke(null);
			return;
		}
		HostProxy.PermissionService.BeginCheckAppIsReady(HostProxy.deviceManager.MasterDevice, permissionModuleNames, delegate(bool? isReady)
		{
			if (!isReady.HasValue)
			{
				finalResultRepoter?.Invoke(isReady);
			}
			else if (!isReady.Value)
			{
				object lockObj = new object();
				dynamic winCloseCondition = new ExpandoObject();
				winCloseCondition.CanClose = false;
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					HostProxy.HostMaskLayerWrapper.Show(new NormalPermissionsTipView
					{
						ConnectedAppType = ((TcpAndroidDevice)device)?.ConnectedAppType
					}, delegate
					{
						if (device != null && device.SoftStatus == DeviceSoftStateEx.Online)
						{
							lock (lockObj)
							{
								return winCloseCondition.CanClose;
							}
						}
						return true;
					});
				});
				if (cancelToken != null && cancelToken.IsCancellationRequested)
				{
					finalResultRepoter?.Invoke(false);
				}
				else
				{
					HostProxy.PermissionService.BeginRollPollingAppModuleIsReady(HostProxy.deviceManager.MasterDevice, permissionModuleNames, null, delegate(bool? appIsReady)
					{
						if (!appIsReady.HasValue)
						{
							finalResultRepoter?.Invoke(appIsReady);
						}
						else if (appIsReady.Value)
						{
							lock (lockObj)
							{
								winCloseCondition.CanClose = true;
							}
							finalResultRepoter?.Invoke(true);
						}
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							HostProxy.HostMaskLayerWrapper.FireCloseConditionCheck();
						});
					});
				}
			}
			else
			{
				finalResultRepoter?.Invoke(true);
			}
		});
	}

	public void BeginConfirmAppIsReady(DeviceEx device, List<string> permissionModuleNames, FrameworkElement tipView, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter)
	{
		LogHelper.LogInstance.Info($"Permission.BeginConfirmAppIsReady called, {finalResultRepoter.Target}");
		if (device == null || device.SoftStatus != DeviceSoftStateEx.Online || permissionModuleNames == null || permissionModuleNames.Count == 0)
		{
			finalResultRepoter?.Invoke(null);
			return;
		}
		HostProxy.PermissionService.BeginCheckAppIsReady(HostProxy.deviceManager.MasterDevice, permissionModuleNames, delegate(bool? isReady)
		{
			if (!isReady.HasValue)
			{
				finalResultRepoter?.Invoke(isReady);
			}
			else if (!isReady.Value)
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					if (tipView == null)
					{
						tipView = new NormalPermissionsTipView();
					}
					ConfirmAppPermissionIsReadyViewModelV6 confirmVM = new ConfirmAppPermissionIsReadyViewModelV6();
					confirmVM.LenovoPrivacyVisibility = Visibility.Visible;
					confirmVM.ViewContent = tipView;
					if (permissionModuleNames.Contains("ScreenRecord"))
					{
						confirmVM.HeaderVisibility = Visibility.Collapsed;
					}
					if (permissionModuleNames.Contains("Apps"))
					{
						confirmVM.ManualColseNotifyEvent = finalResultRepoter;
					}
					ConfirmAppPermissionIsReadyV6 confirmView = new ConfirmAppPermissionIsReadyV6
					{
						DataContext = confirmVM
					};
					confirmVM.ConfirmCommand = new ReplayCommand(delegate
					{
						confirmVM.ConfirmButtonEnabled = false;
						BeginCheckAppIsReady(device, permissionModuleNames, delegate(bool? currentIsReady)
						{
							if (!currentIsReady.HasValue)
							{
								HostProxy.CurrentDispatcher?.Invoke(delegate
								{
									HostProxy.HostMaskLayerWrapper.Close(confirmView);
								});
								finalResultRepoter?.Invoke(null);
							}
							else
							{
								if (currentIsReady.Value)
								{
									HostProxy.CurrentDispatcher?.Invoke(delegate
									{
										HostProxy.HostMaskLayerWrapper.Close(confirmView);
									});
									finalResultRepoter?.Invoke(true);
								}
								confirmVM.ConfirmButtonEnabled = true;
							}
						});
					});
					HostProxy.HostMaskLayerWrapper.Show(confirmView, HostMaskLayerWrapper.CloseMaskOperation.ForceCloseWhenDeviceDisconnect);
				});
				if (cancelToken != null && cancelToken.IsCancellationRequested)
				{
					finalResultRepoter?.Invoke(null);
				}
			}
			else
			{
				finalResultRepoter?.Invoke(true);
			}
		});
	}

	public void BeginConfirmAppIsReady(DeviceEx device, string permissionModule, CancellationTokenSource cancelToken, Action<bool?> finalResultRepoter)
	{
		LogHelper.LogInstance.Info($"BeginConfirmAppIsReady entered; Device:[{device}], moduleName:[{permissionModule}]");
		FrameworkElement view = null;
		if (Thread.CurrentThread.IsBackground)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				view = GetDefaultTipView(device, permissionModule);
			});
		}
		else
		{
			view = GetDefaultTipView(device, permissionModule);
		}
		BeginConfirmAppIsReady(device, new List<string> { permissionModule }, view, cancelToken, finalResultRepoter);
		LogHelper.LogInstance.Info($"BeginConfirmAppIsReady existed; Device:[{device}], moduleName:[{permissionModule}]");
	}

	private FrameworkElement GetDefaultTipView(DeviceEx device, string moduleName)
	{
		LogHelper.LogInstance.Info($"Permission.GetDefaultTipView entered; Device:[{device}], moduleName:[{moduleName}]");
		FrameworkElement frameworkElement = null;
		switch (moduleName)
		{
		case "Apps":
		{
			string appIdendity = ((device is TcpAndroidDevice { SoftStatus: DeviceSoftStateEx.Online } tcpAndroidDevice) ? tcpAndroidDevice.ConnectedAppType : "Ma");
			frameworkElement = new AppPermissionsTipViewV6(appIdendity);
			break;
		}
		case "BasicInfo":
		case "Pictures":
		case "Songs":
		case "Videos":
		case "CallLogs":
		case "Contacts":
		case "SMS":
		case "Backup":
		case "Restore":
		case "File":
		{
			NormalPermissionsTipView normalPermissionsTipView = new NormalPermissionsTipView();
			normalPermissionsTipView.ConnectedAppType = (device as TcpAndroidDevice)?.ConnectedAppType;
			normalPermissionsTipView.TipContent = "K0834";
			frameworkElement = normalPermissionsTipView;
			break;
		}
		case "ScreenRecord":
			frameworkElement = new ScreenRecoderPermissionsTipViewV6();
			break;
		case "SetRingTone":
			frameworkElement = new SetAsRingTongPermissionsTipViewV6();
			break;
		default:
		{
			NormalPermissionsTipView normalPermissionsTipView = new NormalPermissionsTipView();
			normalPermissionsTipView.ConnectedAppType = (device as TcpAndroidDevice)?.ConnectedAppType;
			normalPermissionsTipView.TipContent = "K0834";
			frameworkElement = normalPermissionsTipView;
			break;
		}
		}
		LogHelper.LogInstance.Info("Permission.GetDefaultTipView returned, viewName:[" + frameworkElement?.GetType()?.Name + "]");
		return frameworkElement;
	}
}
