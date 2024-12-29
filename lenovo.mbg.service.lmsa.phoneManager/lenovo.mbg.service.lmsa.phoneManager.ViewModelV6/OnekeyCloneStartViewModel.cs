using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class OnekeyCloneStartViewModel : ViewModelBase
{
	public ConnectedDeviceViewModel _SelectedMasterDevice;

	public ConnectedDeviceViewModel _SelectedTargetDevice;

	public Visibility _TargetDeviceVisibility = Visibility.Collapsed;

	public OnekeyCloneManager ManagerV6 { get; set; }

	public List<string> SelectedResources { get; set; }

	public bool InitWorkFlag { get; set; }

	public ObservableCollection<ConnectedDeviceViewModel> ConnectedDevices => Context.MainFrame.ConnectedDevices;

	public ReplayCommand StartCommand { get; }

	public ConnectedDeviceViewModel SelectedMasterDevice
	{
		get
		{
			return _SelectedMasterDevice;
		}
		set
		{
			if (_SelectedMasterDevice == value)
			{
				return;
			}
			_SelectedMasterDevice = value;
			if (value != null && InitWorkFlag)
			{
				Task.Run(delegate
				{
					InitMasterDeviceInfo(_SelectedMasterDevice);
				});
			}
			OnPropertyChanged("SelectedMasterDevice");
		}
	}

	public ConnectedDeviceViewModel SelectedTargetDevice
	{
		get
		{
			return _SelectedTargetDevice;
		}
		set
		{
			if (_SelectedTargetDevice != value)
			{
				_SelectedTargetDevice = value;
				TargetDeviceVisibility = ((SelectedTargetDevice == null) ? Visibility.Collapsed : Visibility.Visible);
				if (value == null)
				{
					ManagerV6.Dispose();
				}
				OnPropertyChanged("SelectedTargetDevice");
			}
		}
	}

	public Visibility TargetDeviceVisibility
	{
		get
		{
			return _TargetDeviceVisibility;
		}
		set
		{
			_TargetDeviceVisibility = value;
			OnPropertyChanged("TargetDeviceVisibility");
		}
	}

	public OnekeyCloneStartViewModel()
	{
		ManagerV6 = new OnekeyCloneManager();
		InitWorkFlag = true;
		StartCommand = new ReplayCommand(StartCommandHandler);
	}

	public override void LoadData(object data)
	{
		if (SelectedMasterDevice == null || SelectedMasterDevice.Id != Context.CurrentDevice.Identifer)
		{
			SelectedMasterDevice = ConnectedDevices.FirstOrDefault((ConnectedDeviceViewModel n) => n.Id == Context.CurrentDevice.Identifer);
		}
		else
		{
			InitMasterDeviceInfo(SelectedMasterDevice);
		}
		if (SelectedTargetDevice != null && SelectedTargetDevice.Id == SelectedMasterDevice.Id)
		{
			SelectedTargetDevice = null;
		}
		base.LoadData(data);
	}

	private void InitMasterDeviceInfo(ConnectedDeviceViewModel device)
	{
		ManagerV6.Dispose();
		ManagerV6.InitWorker(device.Device);
		Task.Run(delegate
		{
			ManagerV6.LoadData(delegate(bool Allcompleted, string resourceType, Dictionary<string, long> idAndSizeMapping)
			{
				CategoryInfoViewModel target = device.CategoryInfos.FirstOrDefault((CategoryInfoViewModel m) => m.ResourceType == resourceType);
				if (target != null && idAndSizeMapping != null)
				{
					HostProxy.CurrentDispatcher.Invoke(delegate
					{
						target.Count = idAndSizeMapping.Count;
						target.TotalSize = idAndSizeMapping.Values.Sum();
					});
				}
			});
		});
	}

	private void StartCommandHandler(object data)
	{
		if (SelectedMasterDevice == null || SelectedTargetDevice == null)
		{
			return;
		}
		if (SelectedTargetDevice.Id == SelectedMasterDevice.Id)
		{
			Context.MessageBox.ShowMessage("K1434");
			return;
		}
		SelectedResources = (from m in SelectedMasterDevice.CategoryInfos
			where m.IsSelected
			select m.ResourceType).ToList();
		if (SelectedResources.Count == 0 || ((SelectedMasterDevice.Device.ConnectType == ConnectType.Wifi || SelectedTargetDevice.Device.ConnectType == ConnectType.Wifi) && Context.MessageBox.ShowMessage("K0711", "K1440", "K0612", "K0208") == false))
		{
			return;
		}
		HostProxy.PermissionService.BeginConfirmAppIsReady(SelectedMasterDevice.Device, "Restore", null, delegate(bool? isReady)
		{
			if (isReady.HasValue && isReady.Value)
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					long resourcesTotalSize = ManagerV6.GetResourcesTotalSize(SelectedResources);
					long freeInternalStorage = SelectedTargetDevice.Device.Property.FreeInternalStorage;
					if (resourcesTotalSize + 209715200 > freeInternalStorage)
					{
						HostProxy.CurrentDispatcher.Invoke(() => Context.MessageBox.ShowMessage(ResourcesHelper.StringResources.SingleInstance.PC_SPACE_NOT_ENOUGH, MessageBoxButton.OK, MessageBoxImage.Exclamation));
					}
					else
					{
						HostProxy.CurrentDispatcher.Invoke(delegate
						{
							Context.Switch(ViewType.ONEKEYCLONE_TRANSFER, this, reload: false, reloadData: true);
						});
					}
				});
			}
		});
	}
}
