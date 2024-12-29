using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.Business;
using lenovo.mbg.service.lmsa.backuprestore.Business.Restore;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;
using lenovo.mbg.service.lmsa.backuprestore.Common;
using lenovo.mbg.service.lmsa.backuprestore.View;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.common;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class RestoreViewModel : ViewModelBase
{
	private static Dictionary<string, string> sRestorceProgressResourceTypeTitleMapping = new Dictionary<string, string>
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
		},
		{
			"{580C48C8-6CEF-4BBB-AF37-D880B349D142}",
			BackupRestoreStaticResources.SingleInstance.FILE
		},
		{
			"{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}",
			BackupRestoreStaticResources.SingleInstance.SMS
		},
		{
			"{89D4DB68-4258-4002-8557-E65959C558B3}",
			BackupRestoreStaticResources.SingleInstance.CALLLOG
		}
	};

	protected List<string> retryselectedTypes;

	protected Dictionary<string, List<string>> retryIdList;

	protected DeviceRestoreWorkerEx restoreWorker;

	protected Stopwatch sw;

	protected ResourceExecuteResult executeResult;

	public volatile bool doRetry;

	private IBackupStorage storage;

	private CategoriesViewModel _CategoriesViewModel = new CategoriesViewModel(isBackup: false);

	public WorkTransferWindowViewModel _VM;

	private bool _isEnabled;

	private int _SelectedCount;

	public CategoriesViewModel CategoriesViewModel
	{
		get
		{
			return _CategoriesViewModel;
		}
		set
		{
			_CategoriesViewModel = value;
			OnPropertyChanged("CategoriesViewModel");
		}
	}

	public WorkTransferWindowViewModel VM
	{
		get
		{
			return _VM;
		}
		set
		{
			_VM = value;
			OnPropertyChanged("VM");
		}
	}

	public ReplayCommand RestoreCommand { get; }

	public ReplayCommand CancelCommand { get; }

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			_isEnabled = value;
			OnPropertyChanged("IsEnabled");
		}
	}

	public int SelectedCount
	{
		get
		{
			return _SelectedCount;
		}
		set
		{
			_SelectedCount = value;
			IsEnabled = _SelectedCount > 0;
			OnPropertyChanged("SelectedCount");
		}
	}

	public RestoreViewModel()
	{
		CancelCommand = new ReplayCommand(CancelCommandHandler);
		RestoreCommand = new ReplayCommand(RestoreCommandHandler);
	}

	public override void LoadData(object data)
	{
		base.LoadData(data);
		storage = data as IBackupStorage;
		if (storage == null)
		{
			return;
		}
		List<BackupResource> list = new List<BackupResource>();
		string version = string.Empty;
		Dictionary<string, int> dictionary = null;
		Dictionary<string, List<BackupResource>> dictionary2 = null;
		try
		{
			list = storage.OpenRead(out version).GetChildResources(null);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Read file[" + storage.StoragePath + "] failed,throw exception:" + ex);
		}
		if (list == null || list.Count == 0)
		{
			return;
		}
		try
		{
			list = (from m in list.ToLookup((BackupResource m) => m.Tag)
				select m.First()).ToList();
			_ = HostProxy.deviceManager.MasterDevice;
			restoreWorker = new DeviceRestoreWorkerEx(null, null, storage);
			restoreWorker.PrepareWorker(list.Select((BackupResource m) => m.Tag).ToList());
			dictionary = restoreWorker.ResourcesCountMapping;
			dictionary2 = restoreWorker.TypeResourceMapping;
		}
		catch (Exception ex2)
		{
			LogHelper.LogInstance.Error("Read resource file info[" + storage.StoragePath + "] failed,throw exception:" + ex2);
		}
		TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		List<CategoryViewModel> list2 = new List<CategoryViewModel>();
		foreach (CategoryViewModel view in CategoriesViewModel.CategoriesList)
		{
			if (view.ResourceType == "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}" || view.ResourceType == "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}" || view.ResourceType == "{89D4DB68-4258-4002-8557-E65959C558B3}" || tcpAndroidDevice.ConnectedAppType == "Moto")
			{
				if (true == dictionary?.ContainsKey(view.ResourceType))
				{
					view.IsChecked = false;
					view.CountNum = dictionary[view.ResourceType];
					view.Count = dictionary[view.ResourceType];
					view.ToggleButtonVisibility = Visibility.Collapsed;
					view.Tag = new List<BackupResource>(dictionary2[view.ResourceType]);
					list2.Add(view);
				}
			}
			else if (true == dictionary2?.ContainsKey(view.ResourceType))
			{
				List<BackupResource> list3 = dictionary2[view.ResourceType];
				view.CountNum = list3.Count;
				view.Count = list3.Count;
				List<CategoryViewModel> collection = (from n in list3.GroupBy(delegate(BackupResource n)
					{
						string[] array = n.Value.Split('/');
						return (array == null || array.Length <= 1) ? string.Empty : array[array.Length - 2];
					})
					select new CategoryViewModel
					{
						Key = n.Key,
						CountNum = n.Count(),
						ExtraData = n.ToList(),
						Parent = view
					}).ToList();
				view.Childs = new ObservableCollection<ComboBoxModel>(collection);
				view.IsChecked = false;
				list2.Add(view);
				view.ToggleButtonVisibility = ((view.CountNum <= 0) ? Visibility.Collapsed : Visibility.Visible);
			}
		}
		CategoriesViewModel.LoadCategories(list2);
		FireCategorySelectionChanged();
	}

	private void CancelCommandHandler(object parameter)
	{
		DeviceEx currentDevice = Context.CurrentDevice;
		if (currentDevice != null && currentDevice.SoftStatus == DeviceSoftStateEx.Online)
		{
			Context.CurrentRestoreViewType = ViewType.RESTOREMAIN;
			Context.Switch(ViewType.RESTOREMAIN, level2: true);
		}
		else
		{
			Context.Switch(ViewType.START);
		}
	}

	private void GoRestoreMainView()
	{
		DeviceEx currentDevice = Context.CurrentDevice;
		if (currentDevice != null && currentDevice.SoftStatus == DeviceSoftStateEx.Online)
		{
			Context.CurrentRestoreViewType = ViewType.RESTOREMAIN;
			Context.Switch(ViewType.RESTOREMAIN, null, reload: true, level2: true);
		}
		else
		{
			Context.Switch(ViewType.START);
		}
	}

	private void RestoreCommandHandler(object parameter)
	{
		if (!(Context.CurrentDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return;
		}
		doRetry = false;
		restoreWorker._workerList.ForEach(delegate(IWorker p)
		{
			RestoreWorkerAbstractEx obj = p as RestoreWorkerAbstractEx;
			obj.ChildResourceNodes.Clear();
			obj.RetryNodes?.Clear();
		});
		LogHelper.LogInstance.Info("========================restore start===========================");
		foreach (CategoryViewModel category in CategoriesViewModel.Categories)
		{
			if (!category.IsSelected)
			{
				continue;
			}
			List<BackupResource> list = null;
			if (category.ResourceType == "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}")
			{
				new AppServiceRequest(tcpAndroidDevice.ExtendDataFileServiceEndPoint, tcpAndroidDevice.RsaSocketEncryptHelper).RequestString(21, "getIdAndSizeMapping", null);
				list = category.Tag as List<BackupResource>;
			}
			else if (!(category.ResourceType == "{89D4DB68-4258-4002-8557-E65959C558B3}"))
			{
				list = ((!(category.ResourceType == "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}") && !(tcpAndroidDevice.ConnectedAppType == "Moto")) ? (from p in category.Childs?.OfType<CategoryViewModel>()
					where p.IsSelected
					select p).SelectMany((CategoryViewModel n) => (n.ExtraData != null) ? (n.ExtraData as List<BackupResource>) : new List<BackupResource>())?.ToList() : (category.Tag as List<BackupResource>));
			}
			else
			{
				new AppServiceRequest(tcpAndroidDevice.ExtendDataFileServiceEndPoint, tcpAndroidDevice.RsaSocketEncryptHelper).RequestString(22, "getIdAndSizeMapping", null);
				list = category.Tag as List<BackupResource>;
			}
			if (list != null && list.Count > 0)
			{
				(restoreWorker._workerList.FirstOrDefault((IWorker p) => p.WorkerId == category.ResourceType) as RestoreWorkerAbstractEx).ChildResourceNodes.AddRange(list);
				LogHelper.LogInstance.Debug($"restore resources type:[{ResourceTypeDefine.ResourceTypeMap[category.ResourceType]}], file count:[{list.Count}], file total size:[{GlobalFun.ConvertLong2String(list.Sum((BackupResource n) => n.AssociatedStreamSize))}].");
			}
		}
		DoRestoreEx();
	}

	private void DoRestoreEx()
	{
		if (storage == null)
		{
			return;
		}
		TcpAndroidDevice device = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (device == null)
		{
			return;
		}
		retryselectedTypes = new List<string>();
		retryIdList = new Dictionary<string, List<string>>();
		string modelName = Context.CurrentDevice.Property.ModelName;
		WorkTransferWindowWrapper progress = new WorkTransferWindowWrapper(Context.MessageBox, modelName, "K1405", "K1400");
		VM = progress.Vm;
		Context.MessageBox.ChangeIsEnabled(isEnabled: false);
		BackupRestoreMainViewModel mv11 = HostProxy.ViewContext.FindViewModel<BackupRestoreMainViewModel>(typeof(BackupRestoreMainView));
		if (mv11 != null)
		{
			mv11.TitleBarlVisible = Visibility.Collapsed;
			mv11.BtnHelpGuidelVisible = Visibility.Collapsed;
		}
		progress.DeviceId = device.Identifer;
		BusinessData businessData = new BusinessData(BusinessType.BACKUPRESTORE_RESTORE, device);
		restoreWorker.TaskContext = new AsyncTaskContext(null);
		restoreWorker.UpdateDevice(device);
		if (!CheckSpace(restoreWorker))
		{
			return;
		}
		progress.OnRetryCommandFired = delegate
		{
			if (restoreWorker != null && device != null)
			{
				LogHelper.LogInstance.Info("========================restore retry===========================");
				int total = retryIdList.Sum((KeyValuePair<string, List<string>> p) => p.Value.Count);
				doRetry = true;
				progress.Init(total);
				Task.Run(delegate
				{
					restoreWorker.PrepareWorker(retryselectedTypes, retryIdList);
					retryIdList.Clear();
					retryselectedTypes.Clear();
					restoreWorker.DoProcess(null);
				});
			}
		};
		progress.CompletedCallback = delegate
		{
			progress.UpdateResultTitle("K1407");
			LogHelper.LogInstance.Info("========================restore end===========================");
		};
		progress.BeginProcess(delegate(WorkTransferWindowWrapper self)
		{
			bool isCancel = false;
			try
			{
				self.OnCancelCmmandFired = delegate
				{
					isCancel = true;
					LogHelper.LogInstance.Info("========================restore cancel===========================");
				};
				self.CloseWindowCallback = delegate(int code)
				{
					Task.Run(delegate
					{
						if (executeResult.HasFailed && HostProxy.GlobalCache.Get("BackupRestoreHasFailed") == null)
						{
							HostProxy.GlobalCache.AddOrUpdate("BackupRestoreHasFailed", true);
						}
						sw.Stop();
						HostProxy.BehaviorService.Collect(BusinessType.BACKUPRESTORE_RESTORE, businessData.Update(sw.ElapsedMilliseconds, (code == -1) ? BusinessStatus.QUIT : ((executeResult.Status == 1) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED), executeResult.ResultMap));
					});
					SysSleepManager.ResetSleep();
					storage?.Dispose();
					restoreWorker.TaskContext.Cancel();
					GoRestoreMainView();
					if (mv11 != null)
					{
						mv11.TitleBarlVisible = Visibility.Visible;
						mv11.BtnHelpGuidelVisible = Visibility.Visible;
					}
					Context.MessageBox.ChangeIsEnabled(isEnabled: true);
				};
				self.ProgressMaxValue = restoreWorker.GetFileCount();
				string resourceType_Ref = string.Empty;
				_ = string.Empty;
				Context.RegisterWorker(restoreWorker);
				restoreWorker.ResourceTypeStartRestoreCallback = delegate(string resourceType, int count)
				{
					HostProxy.CurrentDispatcher.Invoke(delegate
					{
						resourceType_Ref = sRestorceProgressResourceTypeTitleMapping[resourceType];
						LogHelper.LogInstance.Info("start restore resource type:[" + resourceType_Ref + "].");
						if (doRetry)
						{
							HostProxy.CurrentDispatcher.Invoke(delegate
							{
								self.ChangeCurrentDeatilsGroupViewMode(resourceType_Ref, count);
							});
						}
						else
						{
							self.SetSubProgressInfo(resourceType_Ref, count);
						}
					});
				};
				restoreWorker.ResourceItemFinishRestoreCallback = delegate(string resType, string id, string path, AppDataTransferHelper.BackupRestoreResult _result)
				{
					executeResult.Update(resType, 1, _result, doRetry);
					if (_result == AppDataTransferHelper.BackupRestoreResult.Success)
					{
						if (doRetry)
						{
							self.RetrySuccess(id, path);
						}
						else
						{
							self.AddSuccessCount(id, 1);
						}
					}
					else
					{
						if (!retryselectedTypes.Contains(resType))
						{
							retryselectedTypes.Add(resType);
						}
						if (retryIdList.ContainsKey(resType))
						{
							retryIdList[resType].Add(id);
						}
						else
						{
							retryIdList.Add(resType, new List<string> { id });
						}
						if (doRetry)
						{
							self.RetryFailed(1, _result);
						}
						else
						{
							self.AddFailCount(resType, id, path, 1, _result);
						}
					}
				};
				restoreWorker.ResourceItemRestoreProgressCallback = delegate(string resType, string path, int rl, long rt, long len)
				{
					HostProxy.CurrentDispatcher.Invoke(delegate
					{
						self.UpdateRate(path, rt, len);
					});
				};
				executeResult = new ResourceExecuteResult();
				executeResult.SetIsInternal(_isInternal: true);
				sw = new Stopwatch();
				sw.Start();
				SysSleepManager.PreventSleep();
				restoreWorker.DoProcess(null);
			}
			catch (Exception ex)
			{
				if (!isCancel)
				{
					LogHelper.LogInstance.Error("DoRestore throw ex:" + ex);
					self.Finish();
				}
			}
		});
	}

	private bool CheckStorageValid(long size)
	{
		TcpAndroidDevice tcpAndroidDevice = Context.CurrentDevice as TcpAndroidDevice;
		if (Context.CurrentDevice == null)
		{
			return false;
		}
		tcpAndroidDevice.LoadProperty();
		return size < tcpAndroidDevice.Property.FreeInternalStorage;
	}

	private bool CheckSpace(DeviceRestoreWorkerEx work)
	{
		long size = 0L;
		foreach (IWorker worker in work._workerList)
		{
			if (worker is RestoreWorkerAbstractEx restoreWorkerAbstractEx)
			{
				restoreWorkerAbstractEx.ChildResourceNodes.ForEach(delegate(BackupResource p)
				{
					size += p.AssociatedStreamSize;
				});
			}
		}
		if (size > 0 && !CheckStorageValid(size))
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				Context.MessageBox.ShowMessage("K0670", "K0615");
			});
			return false;
		}
		return true;
	}

	public void FireCategorySelectionChanged()
	{
		SelectedCount = CategoriesViewModel.Categories.Count((CategoryViewModel n) => n.IsSelected);
	}
}
