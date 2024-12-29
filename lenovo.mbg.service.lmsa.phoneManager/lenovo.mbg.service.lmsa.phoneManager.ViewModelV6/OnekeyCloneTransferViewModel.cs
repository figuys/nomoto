using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.OneKeyClone;
using lenovo.mbg.service.lmsa.phoneManager.ModelV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class OnekeyCloneTransferViewModel : OnekeyCloneStartViewModel
{
	private static Dictionary<string, string> sBackupProgressResourceTypeTitleMapping = new Dictionary<string, string>
	{
		{
			"{773D71F7-CE8A-42D7-BE58-5F875DF58C16}",
			BackupRestoreStaticResources.SingleInstance.PIC
		},
		{
			"{242C8F16-6AC7-431B-BBF1-AE24373860F1}",
			BackupRestoreStaticResources.SingleInstance.MUSIC
		},
		{
			"{8BEBE14B-4E45-4D36-8726-8442E6242C01}",
			BackupRestoreStaticResources.SingleInstance.VIDEO
		},
		{
			"{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}",
			BackupRestoreStaticResources.SingleInstance.CONTACT
		}
	};

	protected ResourceExecuteResult executeResult;

	protected string TargetDeviceId;

	protected string MasterDeviceId;

	private int resourcesTotal;

	private int transferSuccessTotal;

	private int transferFailedTotal;

	private int progressMaxVal;

	private int progressCurrentValue;

	private volatile bool isCloning;

	private volatile bool isAllSuccess;

	private int currentTransferResourceTotal;

	private int currentResourceTransferedTotal;

	private string currentTransferResourceName;

	private Visibility retryButtonVisibility;

	private string _TargetModelName;

	private string rate;

	public ReplayCommand CancelCommand { get; }

	private CloneMode CurrentCloneMode { get; set; }

	public OnekeyCloneStartViewModel StartVm { get; set; }

	public ObservableCollection<OperResultModel> OperResultArr { get; set; }

	public int ResourcesTotal
	{
		get
		{
			return resourcesTotal;
		}
		set
		{
			resourcesTotal = value;
			OnPropertyChanged("ResourcesTotal");
		}
	}

	public int TransferSuccessTotal
	{
		get
		{
			return transferSuccessTotal;
		}
		set
		{
			transferSuccessTotal = value;
			OnPropertyChanged("TransferSuccessTotal");
		}
	}

	public int TransferFailedTotal
	{
		get
		{
			return transferFailedTotal;
		}
		set
		{
			transferFailedTotal = value;
			OnPropertyChanged("TransferFailedTotal");
		}
	}

	public int ProgressMaxVal
	{
		get
		{
			return progressMaxVal;
		}
		set
		{
			progressMaxVal = value;
			OnPropertyChanged("ProgressMaxVal");
		}
	}

	public int ProgressCurrentValue
	{
		get
		{
			return progressCurrentValue;
		}
		set
		{
			progressCurrentValue = value;
			OnPropertyChanged("ProgressCurrentValue");
		}
	}

	public bool IsCloning
	{
		get
		{
			return isCloning;
		}
		set
		{
			isCloning = value;
			OnPropertyChanged("IsCloning");
		}
	}

	public bool IsAllSuccess
	{
		get
		{
			return isAllSuccess;
		}
		set
		{
			isAllSuccess = value;
			OnPropertyChanged("IsAllSuccess");
		}
	}

	public int CurrentTransferResourceTotal
	{
		get
		{
			return currentTransferResourceTotal;
		}
		set
		{
			currentTransferResourceTotal = value;
			OnPropertyChanged("CurrentTransferResourceTotal");
		}
	}

	public int CurrentResourceTransferedTotal
	{
		get
		{
			return currentResourceTransferedTotal;
		}
		set
		{
			currentResourceTransferedTotal = value;
			OnPropertyChanged("CurrentResourceTransferedTotal");
		}
	}

	public string CurrentTransferResourceName
	{
		get
		{
			return currentTransferResourceName;
		}
		set
		{
			currentTransferResourceName = value;
			OnPropertyChanged("CurrentTransferResourceName");
		}
	}

	public Visibility RetryButtonVisibility
	{
		get
		{
			return retryButtonVisibility;
		}
		set
		{
			retryButtonVisibility = value;
			OnPropertyChanged("RetryButtonVisibility");
		}
	}

	public string TargetModelName
	{
		get
		{
			return _TargetModelName;
		}
		set
		{
			_TargetModelName = value;
			OnPropertyChanged("TargetModelName");
		}
	}

	public string Rate
	{
		get
		{
			return rate;
		}
		set
		{
			if (!(rate == value))
			{
				rate = value;
				OnPropertyChanged("Rate");
			}
		}
	}

	public OnekeyCloneTransferViewModel()
	{
		CancelCommand = new ReplayCommand(CancelCommandHandler);
		base.InitWorkFlag = false;
	}

	public override void LoadData(object data)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();
		StartVm = data as OnekeyCloneStartViewModel;
		base.ManagerV6 = StartVm.ManagerV6;
		base.SelectedMasterDevice = StartVm.SelectedMasterDevice;
		MasterDeviceId = base.SelectedMasterDevice.Id;
		base.SelectedTargetDevice = StartVm.SelectedTargetDevice;
		TargetDeviceId = base.SelectedTargetDevice.Id;
		TargetModelName = base.SelectedTargetDevice.ModelName;
		BusinessData businessData = new BusinessData(BusinessType.ONEKEYCLONE_CLONE, base.SelectedMasterDevice?.Device);
		OperResultArr = new ObservableCollection<OperResultModel>();
		executeResult = new ResourceExecuteResult();
		List<string> selectedResource = StartVm.SelectedResources;
		ResourcesTotal = base.ManagerV6.GetResourcesTotalCount(selectedResource);
		TransferSuccessTotal = 0;
		base.ManagerV6.FinishCloneCallback = delegate
		{
			IsCloning = false;
			ClearRate();
			foreach (OnkeyCloneWorkerAbstract currentActivedWorker in base.ManagerV6.CurrentActivedWorkers)
			{
				if (currentActivedWorker.SuccessedCount < currentActivedWorker.ResourcesCount)
				{
					IsAllSuccess = false;
					break;
				}
			}
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				CreateAndShowResult(selectedResource, base.ManagerV6);
			});
			Thread.Sleep(200);
			if (StartVm.ConnectedDevices.Count((ConnectedDeviceViewModel n) => n.Id == TargetDeviceId || n.Id == MasterDeviceId) != 2)
			{
				Context.MessageBox.ShowMessage("K0711", "K1016");
				Context.Switch(ViewType.ONEKEYCLONE, this, reload: false, reloadData: true);
			}
			else
			{
				Context.Switch(ViewType.ONEKEYCLONE_RESULT, this, reload: false, reloadData: true);
			}
			sw.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.ONEKEYCLONE_CLONE, businessData.Update(sw.ElapsedMilliseconds, (executeResult.Status == 1) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, executeResult.ResultMap));
		};
		base.ManagerV6.CancelCallback = delegate
		{
			sw.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.ONEKEYCLONE_CLONE, businessData.Update(sw.ElapsedMilliseconds, BusinessStatus.QUIT, null));
		};
		base.ManagerV6.StartCloneCallback = delegate(CloneMode cloneMode, int totalCount, long totalSize)
		{
			CurrentCloneMode = cloneMode;
			ProgressMaxVal = totalCount;
			ProgressCurrentValue = 0;
			IsCloning = true;
			IsAllSuccess = true;
		};
		base.ManagerV6.ResourceTypeStartBackupCallback = delegate(string resourceType, int resourceTotal)
		{
			CurrentTransferResourceTotal = resourceTotal;
			CurrentResourceTransferedTotal = 0;
			CurrentTransferResourceName = sBackupProgressResourceTypeTitleMapping[resourceType];
		};
		base.ManagerV6.ResourceItemBackupProgressCallback = delegate(string resourceType, string resourceId, int readLength, long readTotal, long totalSize)
		{
			UpdateRate(readTotal, totalSize);
		};
		base.ManagerV6.ResourceItemFinishBackupCallback = delegate(string resourceType, string id, string path, bool isSuccess)
		{
			ProgressCurrentValue += 1;
			CurrentResourceTransferedTotal += 1;
			if (isSuccess)
			{
				TransferSuccessTotal += 1;
			}
		};
		base.ManagerV6.ResourceTypeFinishBackupCallback = delegate
		{
			ClearRate();
		};
		Task.Run(delegate
		{
			base.ManagerV6.Clone(base.SelectedTargetDevice.Device, selectedResource);
		});
		base.LoadData();
	}

	public void RetryClone()
	{
		base.ManagerV6.CloneRetry();
	}

	private void ClearRate()
	{
		CurrentTransferResourceName = string.Empty;
		Rate = string.Empty;
		ProgressMaxVal = 0;
		ProgressCurrentValue = 0;
		CurrentTransferResourceTotal = 0;
		CurrentResourceTransferedTotal = 0;
	}

	public void UpdateRate(long readTotal, long total)
	{
		Rate = "(" + GlobalFun.ConvertLong2String(readTotal) + "/" + GlobalFun.ConvertLong2String(total) + ")";
	}

	private void CreateAndShowResult(List<string> selectedResources, OnekeyCloneManager ManagerV6)
	{
		OperResultArr = new ObservableCollection<OperResultModel>();
		foreach (OnkeyCloneWorkerAbstract worker in ManagerV6.GetWorkers(selectedResources))
		{
			OperResultModel operResultModel = new OperResultModel
			{
				IsBackup = true,
				ResName = sBackupProgressResourceTypeTitleMapping[worker.ResourceType],
				Total = worker.ResourcesCount,
				Complete = worker.SuccessedCount,
				IsComplete = (worker.ResourcesCount == worker.SuccessedCount),
				IsFailedItemsPanelClosed = false,
				FailedItemsPanelVisibility = Visibility.Visible
			};
			if ("{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}".Equals(worker.ResourceType) || "{89D4DB68-4258-4002-8557-E65959C558B3}".Equals(worker.ResourceType) || "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}".Equals(worker.ResourceType) || worker.ResourcesCount == worker.SuccessedCount)
			{
				operResultModel.FailedItemsPanelVisibility = Visibility.Collapsed;
			}
			executeResult.UpdateClone(worker.ResourceType, operResultModel.Complete, worker.ResourcesCount - worker.SuccessedCount);
			OperResultArr.Add(operResultModel);
			string value = string.Empty;
			foreach (KeyValuePair<string, long> failedItem in worker.FailedItems)
			{
				worker.IdAndPathList.TryGetValue(failedItem.Key, out value);
				operResultModel.AddFailedItem(new FailedItem
				{
					Id = failedItem.Key,
					Path = value
				}, string.IsNullOrEmpty(value) || "Missing".Equals(value));
			}
		}
		TransferFailedTotal = ResourcesTotal - TransferSuccessTotal;
		RetryButtonVisibility = ((TransferFailedTotal == 0) ? Visibility.Collapsed : Visibility.Visible);
	}

	private void CancelCommandHandler(object data)
	{
		if (ShowCancelConfirmMessage() != false)
		{
			base.ManagerV6?.Cancel();
			ClearRate();
			Context.Switch(ViewType.ONEKEYCLONE, null, reload: false, reloadData: true);
		}
	}

	private bool? ShowCancelConfirmMessage()
	{
		return Context.MessageBox.ShowMessage("K0596", "K0541", "K0327", "K0208");
	}
}
