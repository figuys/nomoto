using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.backuprestore.Business;
using lenovo.mbg.service.lmsa.backuprestore.Business.Backup;
using lenovo.mbg.service.lmsa.backuprestore.Common;
using lenovo.mbg.service.lmsa.backuprestore.View;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.common;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Dialog;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class BackupMainViewModel : ViewModelBase
{
	private enum Result
	{
		Unknown,
		Allow,
		Deny,
		Waiting
	}

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
			"{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}",
			BackupRestoreStaticResources.SingleInstance.SMS
		},
		{
			"{89D4DB68-4258-4002-8557-E65959C558B3}",
			BackupRestoreStaticResources.SingleInstance.CALLLOG
		},
		{
			"{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}",
			BackupRestoreStaticResources.SingleInstance.CONTACT
		},
		{
			"{580C48C8-6CEF-4BBB-AF37-D880B349D142}",
			BackupRestoreStaticResources.SingleInstance.FILE
		}
	};

	protected Stopwatch sw;

	protected ResourceExecuteResult executeResult;

	private volatile bool _IsLoading;

	public WorkTransferWindowViewModel _VM;

	public volatile bool doRetry;

	protected Dictionary<string, List<string>> retryselectedTypes;

	protected DeviceBackupWorkerEx backupWorker;

	private bool _isEnabled;

	private string _storagePath = Configurations.BackupPath;

	public object _tips;

	private bool _isCheckedProtect;

	private bool isEncryptBackup = true;

	private FontSizeConverter _fontSizeConverter;

	private string _NotesValue;

	private bool _NotesTextBoxVisibility = true;

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

	public Action<bool> UpdateSelSotrageAction { get; set; }

	public bool IsInternalStorage { get; set; }

	public CategoriesViewModel CategoriesViewModel { get; set; }

	public ReplayCommand BackupCommand { get; private set; }

	public ReplayCommand ModifyButtonClickCommand { get; private set; }

	public ReplayCommand ViewButtonClickCommand { get; private set; }

	private int _SelectedCount => CategoriesViewModel.Categories.Count((CategoryViewModel n) => n.IsSelected);

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			if (_isEnabled != value)
			{
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
	}

	public string StoragePath
	{
		get
		{
			return _storagePath;
		}
		set
		{
			if (_storagePath != value)
			{
				_storagePath = value;
				SetBackupButtonIsEnable();
				OnPropertyChanged("StoragePath");
			}
		}
	}

	public object Tips
	{
		get
		{
			return _tips;
		}
		set
		{
			if (_tips != value)
			{
				_tips = value;
				OnPropertyChanged("Tips");
			}
		}
	}

	public bool IsCheckedProtect
	{
		get
		{
			return _isCheckedProtect;
		}
		set
		{
			if (_isCheckedProtect != value)
			{
				_isCheckedProtect = value;
				OnPropertyChanged("IsCheckedProtect");
			}
		}
	}

	public bool IsEncryptBackup
	{
		get
		{
			return isEncryptBackup;
		}
		set
		{
			if (isEncryptBackup != value)
			{
				isEncryptBackup = value;
				OnPropertyChanged("IsEncryptBackup");
			}
		}
	}

	private FontSizeConverter FontSizeConverter
	{
		get
		{
			if (_fontSizeConverter == null)
			{
				_fontSizeConverter = new FontSizeConverter();
			}
			return _fontSizeConverter;
		}
	}

	public string NotesValue
	{
		get
		{
			return _NotesValue;
		}
		set
		{
			_NotesValue = value;
			OnPropertyChanged("NotesValue");
		}
	}

	public bool NotesTextBoxVisibility
	{
		get
		{
			return _NotesTextBoxVisibility;
		}
		set
		{
			_NotesTextBoxVisibility = value;
			OnPropertyChanged("NotesTextBoxVisibility");
		}
	}

	public BackupMainViewModel()
	{
		IsInternalStorage = true;
		CategoriesViewModel = new CategoriesViewModel(isBackup: true);
		CategoriesViewModel.CategorySelectionChanged += delegate
		{
			SetBackupButtonIsEnable();
		};
		BackupCommand = new ReplayCommand(BackupCommandHandler);
		ModifyButtonClickCommand = new ReplayCommand(ModifyButtonClickCommandHandler);
		ViewButtonClickCommand = new ReplayCommand(ViewButtonClickCommandHandler);
	}

	private void LoadResourceByStorage()
	{
		foreach (CategoryViewModel category in CategoriesViewModel.Categories)
		{
			category.ShowSubWindow = false;
			category.IsSelected = false;
			ObservableCollection<ComboBoxModel> childs = category.Childs;
			if (childs != null && childs.Count > 0)
			{
				List<CategoryViewModel> list = category.Childs.OfType<CategoryViewModel>().ToList();
				foreach (CategoryViewModel item in list)
				{
					item.SubItemVisible = Visibility.Visible;
				}
				int count = (category.CountNum = list.Sum((CategoryViewModel p) => p.CountNum));
				category.Count = count;
			}
			else if (!IsInternalStorage)
			{
				category.CountNum = 0;
				category.IsEnabled = false;
			}
			else
			{
				category.CountNum = category.Count;
				if (Context.CurrentDevice?.ConnectedAppType == "Moto")
				{
					if (category.ResourceType == "{580C48C8-6CEF-4BBB-AF37-D880B349D142}" || category.ResourceType == "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}" || category.ResourceType == "{89D4DB68-4258-4002-8557-E65959C558B3}")
					{
						category.IsEnabled = true;
					}
					else
					{
						category.IsEnabled = category.Count > 0;
					}
				}
				else
				{
					category.IsEnabled = category.Count > 0;
				}
			}
			category.ShowSubWindow = true;
		}
	}

	public override void LoadData()
	{
		base.LoadData();
		IsInternalStorage = true;
		UpdateSelSotrageAction?.Invoke(obj: true);
		CategoriesViewModel.LoadingProcessVisibility = Visibility.Visible;
		CategoriesViewModel.Reset();
		CategoriesViewModel.LoadCategories(CategoriesViewModel.CategoriesList);
		NotesTextBoxVisibility = true;
		NotesValue = string.Empty;
		Task.Run(delegate
		{
			LoadResourceInfo();
		});
	}

	public override void Reset()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			base.Reset();
			NotesTextBoxVisibility = true;
			NotesValue = string.Empty;
		});
	}

	public void LoadResourceInfo()
	{
		if (_IsLoading || Context.CurrentDevice == null)
		{
			return;
		}
		_IsLoading = true;
		TcpAndroidDevice device = Context.CurrentDevice as TcpAndroidDevice;
		BackupRestoreProcessor backupRestoreProcessor = new BackupRestoreProcessor();
		List<string> resourcesType = CategoriesViewModel.Categories.Select((CategoryViewModel m) => m.ResourceType).ToList();
		backupRestoreProcessor.LoadDeviceIdAndSizeV2(device, resourcesType, IsInternalStorage, delegate(string k, Dictionary<string, long> v)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				CategoryViewModel categoryViewModel = CategoriesViewModel.Categories.First((CategoryViewModel n) => n.ResourceType == k);
				categoryViewModel.CountNum = v.Count;
				categoryViewModel.Count = v.Count;
				categoryViewModel.IdAndSizeMapping = v;
			});
		}, delegate(string k, List<JObject> v)
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				CategoryViewModel found = CategoriesViewModel.Categories.First((CategoryViewModel n) => n.ResourceType == k);
				List<CategoryViewModel> childList = new List<CategoryViewModel>();
				if (v != null && v.Count > 0)
				{
					v.ForEach(delegate(JObject n)
					{
						if (n != null && n.HasValues)
						{
							string text = n.Value<string>("name");
							n.Value<string>("path");
							text = text.Substring(text.LastIndexOf("/") + 1);
							JToken value = n.GetValue("idSizeMap");
							Dictionary<string, long> dictionary = new Dictionary<string, long>();
							if (value != null && value.HasValues)
							{
								dictionary = JsonHelper.DeserializeJson2Object<Dictionary<string, long>>(value.ToString());
							}
							CategoryViewModel categoryViewModel2 = new CategoryViewModel
							{
								Key = text,
								Parent = found,
								CountNum = dictionary.Count,
								IdAndSizeMapping = dictionary
							};
							categoryViewModel2.SubItemVisible = Visibility.Visible;
							childList.Add(categoryViewModel2);
						}
					});
				}
				int num = childList.Sum((CategoryViewModel p) => p.CountNum);
				found.CountNum = num;
				found.Count = num;
				found.Childs = new ObservableCollection<ComboBoxModel>(childList);
				found.ToggleButtonVisibility = ((num <= 0) ? Visibility.Collapsed : Visibility.Visible);
			});
		}, delegate
		{
			_IsLoading = false;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				CategoryViewModel categoryViewModel3 = CategoriesViewModel.Categories.FirstOrDefault((CategoryViewModel n) => n.ResourceType == "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}");
				if (categoryViewModel3?.Childs != null && categoryViewModel3.Childs.Count == 1)
				{
					categoryViewModel3.Key = categoryViewModel3.Childs.First().Key;
					categoryViewModel3.CountNum = categoryViewModel3.CountNum;
				}
				LoadResourceByStorage();
				CategoriesViewModel.LoadingProcessVisibility = Visibility.Collapsed;
			});
		});
	}

	private void SetBackupButtonIsEnable()
	{
		IsEnabled = _SelectedCount > 0 && !string.IsNullOrEmpty(StoragePath);
	}

	private void BackupCommandHandler(object parameter)
	{
		if (string.IsNullOrEmpty(StoragePath))
		{
			return;
		}
		Dictionary<string, Dictionary<string, long>> selResTypeArr = new Dictionary<string, Dictionary<string, long>>();
		foreach (CategoryViewModel item in CategoriesViewModel.Categories.Where((CategoryViewModel n) => n.IsSelected))
		{
			if (item.IsSelected)
			{
				Dictionary<string, long> idAndSizeMapping = item.IdAndSizeMapping;
				if (idAndSizeMapping != null && idAndSizeMapping.Count > 0)
				{
					Dictionary<string, long> value = item.IdAndSizeMapping.ToDictionary((KeyValuePair<string, long> p) => p.Key, (KeyValuePair<string, long> p) => p.Value);
					selResTypeArr.Add(item.ResourceType, value);
					continue;
				}
			}
			ObservableCollection<ComboBoxModel> childs = item.Childs;
			if (childs != null && childs.Count > 0)
			{
				Dictionary<string, long> value2 = (from p in item.Childs.OfType<CategoryViewModel>()
					where p.IsSelected
					select p).SelectMany((CategoryViewModel p) => p.IdAndSizeMapping).ToDictionary((KeyValuePair<string, long> p) => p.Key, (KeyValuePair<string, long> p) => p.Value);
				selResTypeArr.Add(item.ResourceType, value2);
			}
		}
		if (selResTypeArr.Count == 0)
		{
			LogHelper.LogInstance.Debug("click backup button, selectedResourcesType is null");
			Context.MessageBox.ShowMessage("K1252");
			return;
		}
		TcpAndroidDevice device = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (device != null && device.ConnectType == ConnectType.Wifi && Context.MessageBox.ShowMessage("K0711", "K1440", "K0612", "K0208") == false)
		{
			return;
		}
		Check(delegate
		{
			Action<string> task = delegate(string password)
			{
				if (!GlobalFun.Exists(StoragePath))
				{
					FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
					{
						SelectedPath = Configurations.BackupPath,
						ShowNewFolderButton = true,
						Description = HostProxy.LanguageService.Translate("Please select a new save path")
					};
					if (DialogResult.OK != folderBrowserDialog.ShowDialog())
					{
						return;
					}
					StoragePath = folderBrowserDialog.SelectedPath;
					Configurations.BackupPath = StoragePath;
				}
				NotesTextBoxVisibility = false;
				LogHelper.LogInstance.Info("========================backup start===========================");
				string text = Path.Combine(StoragePath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".mabk");
				HostProxy.ResourcesLoggingService.RegisterFile(text);
				retryselectedTypes = new Dictionary<string, List<string>>();
				FileInfo storageFileInfo = new FileInfo(text);
				doRetry = false;
				new List<string>();
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				List<string> list3 = new List<string>();
				_ = string.Empty;
				BackupRestoreProcessor backupRestoreProcessor = new BackupRestoreProcessor();
				foreach (KeyValuePair<string, Dictionary<string, long>> _tmpResType in selResTypeArr)
				{
					list3.Clear();
					foreach (KeyValuePair<string, long> item2 in _tmpResType.Value)
					{
						if (item2.Value > 4294967296L)
						{
							LogHelper.LogInstance.Warn("The file is larger than 4G in size. key:[" + item2.Key + "] size:[" + GlobalFun.ConvertLong2String(item2.Value) + "].");
							list3.Add(item2.Key);
							list.Add(backupRestoreProcessor.GetFileFullPathById(device, _tmpResType.Key, item2.Key));
						}
					}
					list3.ForEach(delegate(string m)
					{
						_tmpResType.Value.Remove(m);
					});
					if (_tmpResType.Value.Count == 0)
					{
						list2.Add(_tmpResType.Key);
					}
					LogHelper.LogInstance.Debug($"backup resources type:[{ResourceTypeDefine.ResourceTypeMap[_tmpResType.Key]}], file count:[{_tmpResType.Value.Count}], file total size:[{GlobalFun.ConvertLong2String(_tmpResType.Value.Sum((KeyValuePair<string, long> n) => n.Value))}].");
				}
				if (list.Count > 0)
				{
					File.WriteAllText(Configurations.TRANSFER_FILE_ERROR_TXT_PATH, string.Join(Environment.NewLine, list));
					Larger4GBDialogView content = new Larger4GBDialogView(0);
					Context.MessageBox.ContentMssage(content, "K0071", "K0327", null, isCloseBtn: true, MessageBoxImage.Exclamation);
				}
				list2.ForEach(delegate(string m)
				{
					selResTypeArr.Remove(m);
				});
				if (selResTypeArr.Count > 0)
				{
					DoBackupEx(HostProxy.deviceManager.MasterDevice as TcpAndroidDevice, selResTypeArr, storageFileInfo, password);
				}
			};
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				if (!IsEncryptBackup)
				{
					task(string.Empty);
				}
				else
				{
					SetPasswordViewModel setPasswordViewModel = new SetPasswordViewModel();
					IUserMsgControl userUi = new SetPasswordWindow
					{
						DataContext = setPasswordViewModel
					};
					Context.MessageBox.ShowMessage(userUi);
					if (setPasswordViewModel.Result == true)
					{
						task(setPasswordViewModel.PassWord);
					}
					else if (setPasswordViewModel.Result == false && Context.MessageBox.ShowMessage("K0800", "K0796", "K0612", "K0208") == true)
					{
						task(null);
					}
				}
			});
		});
	}

	public void DoBackupEx(TcpAndroidDevice currentDevice, Dictionary<string, Dictionary<string, long>> selectedTypes, FileInfo storageFileInfo, string password)
	{
		if (currentDevice == null || currentDevice.Property == null)
		{
			return;
		}
		IAndroidDevice property = currentDevice.Property;
		string id2 = Guid.NewGuid().ToString();
		BackupDescription description = new BackupDescription
		{
			Id = id2,
			ModelName = property.ModelName,
			AndroidVersion = property.AndroidVersion,
			BackupDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
			BuildNumber = property.CustomerVersion,
			Notes = NotesValue,
			Category = (currentDevice.Property.Category ?? "phone"),
			StoragePath = storageFileInfo.FullName
		};
		AsyncTaskContext taskContext = new AsyncTaskContext(null);
		backupWorker = new DeviceBackupWorkerEx(currentDevice, taskContext, storageFileInfo, description);
		try
		{
			backupWorker.PrepareWorker(selectedTypes, password);
		}
		catch (Exception ex)
		{
			if (ex.HResult == -2147024857 || ex.HResult == -2147024784)
			{
				HostProxy.CurrentDispatcher.Invoke(() => Context.MessageBox.ShowMessage("K0071", "K0784", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation));
				return;
			}
		}
		WorkTransferWindowWrapper progress = new WorkTransferWindowWrapper(Context.MessageBox, Context.CurrentDevice.Property.ModelName, "K1397", "K1400");
		VM = progress.Vm;
		Context.MessageBox.ChangeIsEnabled(isEnabled: false);
		BackupRestoreMainViewModel mv11 = HostProxy.ViewContext.FindViewModel<BackupRestoreMainViewModel>(typeof(BackupRestoreMainView));
		if (mv11 != null)
		{
			mv11.TitleBarlVisible = Visibility.Collapsed;
			mv11.BtnHelpGuidelVisible = Visibility.Collapsed;
		}
		BusinessData businessData = new BusinessData(BusinessType.BACKUPRESTORE_BACKUP, currentDevice);
		progress.OnRetryCommandFired = delegate
		{
			if (retryselectedTypes.Count > 0)
			{
				LogHelper.LogInstance.Info("========================backup retry===========================");
				LogHelper.LogInstance.Debug("retry data:[" + JsonHelper.SerializeObject2Json(retryselectedTypes) + "]");
				doRetry = true;
				int total = retryselectedTypes.Sum((KeyValuePair<string, List<string>> m) => m.Value.Count);
				progress.Init(total);
				Task.Factory.StartNew(delegate
				{
					backupWorker.RetryCallback?.Invoke(retryselectedTypes);
				});
			}
		};
		progress.CompletedCallback = delegate
		{
			progress.UpdateResultTitle("K1399");
			LogHelper.LogInstance.Info("========================backup end===========================");
		};
		progress.BeginProcess(delegate(WorkTransferWindowWrapper self)
		{
			bool spaceNoteEnough = false;
			bool isCancel = false;
			bool isFailed = false;
			self.OnCancelCmmandFired = delegate
			{
				isCancel = true;
				LogHelper.LogInstance.Info("========================backup cancel===========================");
			};
			self.ProgressMaxValue = selectedTypes.Sum((KeyValuePair<string, Dictionary<string, long>> m) => m.Value.Count);
			self.CloseWindowCallback = delegate(int code)
			{
				Task.Run(delegate
				{
					if (executeResult.HasFailed && HostProxy.GlobalCache.Get("BackupRestoreHasFailed") == null)
					{
						HostProxy.GlobalCache.AddOrUpdate("BackupRestoreHasFailed", true);
					}
					sw.Stop();
					BusinessData data = businessData.Update(sw.ElapsedMilliseconds, (code == -1) ? BusinessStatus.QUIT : ((executeResult.Status == 1) ? BusinessStatus.SUCCESS : BusinessStatus.FALIED), executeResult.ResultMap);
					HostProxy.BehaviorService.Collect(BusinessType.BACKUPRESTORE_BACKUP, data);
				});
				SysSleepManager.ResetSleep();
				if (backupWorker != null)
				{
					backupWorker?.Dispose();
					backupWorker = null;
				}
				if (executeResult.Status == 1 && !isFailed && !isCancel && !spaceNoteEnough)
				{
					description.StorageSize = storageFileInfo.Length;
					new DeviceBackupMgt().AddOrUpdateBackup(description);
					Configurations.BackupLastDateTime = Convert.ToDateTime(description.BackupDateTime);
				}
				taskContext.Cancel();
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					NotesTextBoxVisibility = true;
					IsEncryptBackup = true;
					if (code == 1 && !isFailed && true == Context.MessageBox.ShowMessage(new BackupTips()))
					{
						HostProxy.HostNavigation.SwitchTo("8ab04aa975e34f1ca4f9dc3a81374e2c", "mydevicerescue");
					}
				});
				LoadData();
				if (mv11 != null)
				{
					mv11.TitleBarlVisible = Visibility.Visible;
					mv11.BtnHelpGuidelVisible = Visibility.Visible;
				}
				Context.MessageBox.ChangeIsEnabled(isEnabled: true);
				if (Context.CurrentDevice == null || (Context.CurrentDevice != null && Context.CurrentDevice.SoftStatus == DeviceSoftStateEx.Offline))
				{
					Context.CurrentRestoreViewType = ViewType.MAIN;
					Context.Switch(ViewType.START);
				}
				LogHelper.LogInstance.Debug("refresh backup data.");
			};
			try
			{
				string resourceType_Ref = string.Empty;
				Context.RegisterWorker(backupWorker);
				backupWorker.ResourceTypeStartBackupCallback = delegate(string resourceType, int count)
				{
					HostProxy.CurrentDispatcher.Invoke(delegate
					{
						resourceType_Ref = sBackupProgressResourceTypeTitleMapping[resourceType];
						LogHelper.LogInstance.Info("start backup resource type:[" + resourceType_Ref + "].");
						if (!doRetry)
						{
							self.SetSubProgressInfo(resourceType_Ref, count);
						}
						else
						{
							HostProxy.CurrentDispatcher.Invoke(delegate
							{
								self.ChangeCurrentDeatilsGroupViewMode(resourceType_Ref, count);
							});
						}
					});
				};
				backupWorker.ResourceItemFinishBackupCallback = delegate(string resType, string id, string path, AppDataTransferHelper.BackupRestoreResult _result)
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
						isFailed = true;
						if (!string.IsNullOrEmpty(id))
						{
							if (retryselectedTypes.ContainsKey(resType))
							{
								List<string> list = retryselectedTypes[resType];
								if (!list.Contains(id))
								{
									list.Add(id);
								}
							}
							else
							{
								retryselectedTypes.Add(resType, new List<string> { id });
							}
						}
						if (!doRetry)
						{
							self.AddFailCount(resType, id, path, 1, _result);
						}
						else
						{
							self.RetryFailed(1, _result);
						}
					}
				};
				backupWorker.ResourceItemBackupProgressCallback = delegate(string resType, string name, int rl, long rt, long len)
				{
					HostProxy.CurrentDispatcher.Invoke(delegate
					{
						self.UpdateRate(name, rt, len);
					});
				};
				executeResult = new ResourceExecuteResult();
				executeResult.SetIsInternal(IsInternalStorage);
				sw = new Stopwatch();
				sw.Start();
				SysSleepManager.PreventSleep();
				backupWorker.DoProcess(null);
			}
			catch (Exception ex2)
			{
				isFailed = true;
				spaceNoteEnough = ex2.HResult == -2147024857 || ex2.HResult == -2147024784;
				if (!isCancel)
				{
					LogHelper.LogInstance.Error("backup exception:", ex2);
					self.Finish();
				}
				GlobalFun.TryDeleteFile(storageFileInfo.FullName);
			}
			if (spaceNoteEnough)
			{
				self.StopAndCloseWindow();
				HostProxy.CurrentDispatcher.Invoke(() => Context.MessageBox.ShowMessage("K0071", "K0784", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation));
			}
			if (isCancel)
			{
				GlobalFun.TryDeleteFile(storageFileInfo.FullName);
			}
		});
	}

	private void Check(Action<bool> callback, bool isRetry = true)
	{
		TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (currentDevice.ConnectedAppType == "Ma")
		{
			callback?.Invoke(obj: true);
			return;
		}
		currentDevice.CallAppToFrontstage();
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		CancellationToken cancellationToken = cancellationTokenSource.Token;
		IUserMsgControl ucwin = new UserConsentWindow(cancellationTokenSource);
		Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(() => Context.MessageBox.ShowMessage(ucwin)));
		Task<Result> task1 = Task.Run(() => DoTask(currentDevice, cancellationToken), cancellationToken);
		Task.Run(delegate
		{
			Result result = Result.Unknown;
			if (task1.Wait(300000))
			{
				result = task1.Result;
			}
			else
			{
				cancellationTokenSource.Cancel();
			}
			System.Windows.Application.Current.Dispatcher.Invoke(delegate
			{
				ucwin.GetMsgUi().Close();
			});
			switch (result)
			{
			case Result.Allow:
				callback(obj: true);
				break;
			case Result.Deny:
				if (currentDevice != null && currentDevice == HostProxy.deviceManager.MasterDevice && currentDevice.SoftStatus == DeviceSoftStateEx.Online)
				{
					System.Windows.Application.Current.Dispatcher.Invoke(() => Context.MessageBox.ShowMessage("K0791"));
				}
				break;
			}
		});
	}

	private Result DoTask(TcpAndroidDevice currentDevice, CancellationToken cancellationToken, int pollingTimeout = 10000)
	{
		if (currentDevice == null || currentDevice.SoftStatus != DeviceSoftStateEx.Online || cancellationToken.IsCancellationRequested)
		{
			return Result.Unknown;
		}
		try
		{
			ISequence sequence = HostProxy.Sequence;
			List<PropItem> receiveData = null;
			string action = "obtainUserConsent";
			string responseAction = MessageConstant.getResponseAction(action);
			do
			{
				long secquence = sequence.New();
				using MessageReaderAndWriter messageReaderAndWriter = currentDevice.MessageManager.CreateMessageReaderAndWriter();
				if (cancellationToken.IsCancellationRequested)
				{
					return Result.Unknown;
				}
				if (messageReaderAndWriter.Send(action, new List<string> { "Backup" }, secquence) && messageReaderAndWriter.Receive(responseAction, out receiveData, pollingTimeout) && receiveData != null && receiveData.Count > 0 && !cancellationToken.IsCancellationRequested)
				{
					if (receiveData.Exists((PropItem m) => "1".Equals(m.Value)))
					{
						return Result.Allow;
					}
					if (receiveData.Exists((PropItem m) => "0".Equals(m.Value)))
					{
						LogHelper.LogInstance.Info("User refuses backup authorization");
						return Result.Deny;
					}
				}
			}
			while (!cancellationToken.IsCancellationRequested && currentDevice != null && currentDevice.PhysicalStatus == DevicePhysicalStateEx.Online);
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("backup authorization error", exception);
		}
		return Result.Unknown;
	}

	private void ModifyButtonClickCommandHandler(object parameter)
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		folderBrowserDialog.SelectedPath = Configurations.BackupPath;
		folderBrowserDialog.ShowNewFolderButton = true;
		if (DialogResult.OK == folderBrowserDialog.ShowDialog())
		{
			StoragePath = folderBrowserDialog.SelectedPath;
			Configurations.BackupPath = StoragePath;
		}
	}

	private void ViewButtonClickCommandHandler(object parameter)
	{
		try
		{
			NotesTextBoxVisibility = true;
			if (Directory.Exists(StoragePath))
			{
				Process.Start("explorer.exe", StoragePath);
			}
		}
		catch (Exception)
		{
		}
	}
}
