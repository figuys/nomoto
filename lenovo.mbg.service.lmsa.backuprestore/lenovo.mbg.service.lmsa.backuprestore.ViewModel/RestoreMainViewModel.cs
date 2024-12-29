using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.backuprestore.Business;
using lenovo.mbg.service.lmsa.backuprestore.Business.Backup;
using lenovo.mbg.service.lmsa.backuprestore.Business.Storage;
using lenovo.mbg.service.lmsa.backuprestore.Common;
using lenovo.mbg.service.lmsa.backuprestore.View;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Component.Progress;
using lenovo.themes.generic.ViewModelV6;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class RestoreMainViewModel : ViewModelBase
{
	private DeviceBackupMgt _mgt = new DeviceBackupMgt();

	private ObservableCollection<BackupHistoryListItemViewModel> _HistoryItems = new ObservableCollection<BackupHistoryListItemViewModel>();

	public Visibility _EmptyTipsVisibility = Visibility.Collapsed;

	private bool? _isAllSelected = false;

	private bool _RestoreEnabled;

	private bool _ExtractEnabled;

	private bool _DeleteEnabled;

	public ReplayCommand AllClickCommand { get; }

	public ReplayCommand SingleClickCommand { get; }

	public ReplayCommand ClickCommand { get; }

	public ReplayCommand DeleteCommand { get; }

	public ObservableCollection<BackupHistoryListItemViewModel> HistoryItems
	{
		get
		{
			return _HistoryItems;
		}
		set
		{
			_HistoryItems = value;
			if (_HistoryItems != null && _HistoryItems.Count() > 0)
			{
				EmptyTipsVisibility = Visibility.Collapsed;
			}
			else
			{
				EmptyTipsVisibility = Visibility.Visible;
			}
			OnPropertyChanged("HistoryItems");
		}
	}

	public Visibility EmptyTipsVisibility
	{
		get
		{
			return _EmptyTipsVisibility;
		}
		set
		{
			_EmptyTipsVisibility = value;
			OnPropertyChanged("EmptyTipsVisibility");
		}
	}

	public bool? IsAllSelected
	{
		get
		{
			return _isAllSelected;
		}
		set
		{
			_isAllSelected = value;
			SetButtonEnable(HistoryItems.Count((BackupHistoryListItemViewModel n) => n.IsSelected));
			OnPropertyChanged("IsAllSelected");
		}
	}

	public bool RestoreEnabled
	{
		get
		{
			return _RestoreEnabled;
		}
		set
		{
			_RestoreEnabled = value;
			OnPropertyChanged("RestoreEnabled");
		}
	}

	public bool ExtractEnabled
	{
		get
		{
			return _ExtractEnabled;
		}
		set
		{
			_ExtractEnabled = value;
			OnPropertyChanged("ExtractEnabled");
		}
	}

	public bool DeleteEnabled
	{
		get
		{
			return _DeleteEnabled;
		}
		set
		{
			_DeleteEnabled = value;
			OnPropertyChanged("DeleteEnabled");
		}
	}

	public RestoreMainViewModel()
	{
		AllClickCommand = new ReplayCommand(AllClickCommandHandler);
		SingleClickCommand = new ReplayCommand(SingleClickCommandHandler);
		ClickCommand = new ReplayCommand(ClickCommandHandler);
		DeleteCommand = new ReplayCommand(DeleteCommandHandler);
	}

	public override void LoadData()
	{
		base.LoadData();
		List<BackupDescription> backupList = _mgt.GetBackupList();
		HistoryItems.Clear();
		IsAllSelected = false;
		if (backupList != null && backupList.Count > 0)
		{
			backupList.OrderByDescending((BackupDescription o) => o.BackupDateTime).ToList().ForEach(delegate(BackupDescription n)
			{
				HistoryItems.Add(ConvertBackupDescription(n));
			});
		}
	}

	private void AllClickCommandHandler(object data)
	{
		if (IsAllSelected.HasValue)
		{
			foreach (BackupHistoryListItemViewModel historyItem in HistoryItems)
			{
				historyItem.IsSelected = IsAllSelected.Value;
			}
		}
		SetButtonEnable(HistoryItems.Count((BackupHistoryListItemViewModel n) => n.IsSelected));
	}

	private void SingleClickCommandHandler(object data)
	{
		if (HistoryItems.Count((BackupHistoryListItemViewModel n) => n.IsSelected) == 0)
		{
			IsAllSelected = false;
		}
		else if (HistoryItems.Any((BackupHistoryListItemViewModel n) => !n.IsSelected))
		{
			IsAllSelected = null;
		}
		else
		{
			IsAllSelected = true;
		}
		SetButtonEnable(HistoryItems.Count((BackupHistoryListItemViewModel n) => n.IsSelected));
	}

	private void ClickCommandHandler(object data)
	{
		switch (data.ToString())
		{
		case "import":
			ImportButtonClickCommandHandler(data);
			break;
		case "extract":
			ExtractButtonClickCommandHandler(data);
			break;
		case "delete":
		{
			List<BackupHistoryListItemViewModel> selectedList = HistoryItems.Where((BackupHistoryListItemViewModel n) => n.IsSelected).ToList();
			DeleteButtonClickCommandHandler(selectedList);
			break;
		}
		case "restore":
			RestoreBottonClickCommandHandler(data);
			break;
		}
		RefreshButtonClickCommandHandler(data);
	}

	private void ImportButtonClickCommandHandler(object parameter)
	{
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Title = "K0496";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			openFileDialog.Title = HostProxy.LanguageService.Translate("K0496");
		}
		openFileDialog.Filter = string.Format("{0}|*.mabk;", HostProxy.LanguageService.Translate("K0480"));
		openFileDialog.Multiselect = false;
		openFileDialog.FileName = string.Empty;
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() != true || string.IsNullOrEmpty(openFileDialog.FileName))
		{
			return;
		}
		try
		{
			List<BackupDescription> importStorageList = null;
			_mgt.ImportBackupFile(openFileDialog.FileName, out importStorageList);
			if (importStorageList != null && importStorageList.Count > 0)
			{
				importStorageList.ForEach(delegate(BackupDescription n)
				{
					HistoryItems.Insert(0, ConvertBackupDescription(n));
				});
			}
		}
		catch (Exception)
		{
			Context.MessageBox.ShowMessage("K0669");
		}
	}

	private void ExtractButtonClickCommandHandler(object parameter)
	{
		FolderBrowserDialog fbd = new FolderBrowserDialog();
		fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		if (DialogResult.OK != fbd.ShowDialog())
		{
			return;
		}
		CreateBackupStorage(HistoryItems.First((BackupHistoryListItemViewModel n) => n.IsSelected), delegate(IBackupStorage backupStorage)
		{
			HostProxy.AsyncCommonProgressLoader.Progress(Context.MessageBox, delegate(IAsyncTaskContext context, CommonProgressWindowViewModel viewModel)
			{
				((Action<NotifyTypes, object>)context.ObjectState)?.Invoke(NotifyTypes.INFO_MESSAGE, new List<object>
				{
					ResourcesHelper.StringResources.SingleInstance.BACKUPRESTORE_RELEASE_MESSAGE_NORMAL,
					true
				});
				using IWorker worker = new ReleaseBackupFileWorker(context, backupStorage, fbd.SelectedPath);
				worker.DoProcess(null);
			}, delegate(IAsyncTaskResult result, WorkStatus workStatus)
			{
				Action<NotifyTypes, object> action = (Action<NotifyTypes, object>)result.ObjectState;
				switch (workStatus)
				{
				case WorkStatus.Failed:
				{
					string empty = string.Empty;
					empty = ((result.Exception == null || (result.Exception.HResult != -2147024857 && result.Exception.HResult != -2147024784)) ? ResourcesHelper.StringResources.SingleInstance.BACKUPRESTORE_RELEASE_MESSAGE_FAILED : "K0784");
					action?.Invoke(NotifyTypes.ERROR_MESSAGE, new List<object> { empty, true });
					break;
				}
				case WorkStatus.Success:
					action?.Invoke(NotifyTypes.INFO_MESSAGE, new List<object>
					{
						ResourcesHelper.StringResources.SingleInstance.BACKUPRESTORE_RELEASE_MESSAGE_SUCCESS,
						true
					});
					break;
				}
			});
		});
	}

	private void DeleteCommandHandler(object parameter)
	{
		DeleteButtonClickCommandHandler(new List<BackupHistoryListItemViewModel> { parameter as BackupHistoryListItemViewModel });
		RefreshButtonClickCommandHandler(parameter);
	}

	private void DeleteButtonClickCommandHandler(List<BackupHistoryListItemViewModel> selectedList)
	{
		if (Context.MessageBox.ShowMessage("K0583", "K0611", "K0583", "K0208") == true)
		{
			DeleteRestoreFile(selectedList);
		}
	}

	private void DeleteRestoreFile(List<BackupHistoryListItemViewModel> selectedList)
	{
		List<BackupDescription> deletes = selectedList.Select((BackupHistoryListItemViewModel n) => n.BackupDescriptionInfo).ToList();
		_mgt.DeleteBackupFile(deletes);
		selectedList.ForEach(delegate(BackupHistoryListItemViewModel n)
		{
			HistoryItems.Remove(n);
		});
		SetButtonEnable(0);
	}

	private void RefreshButtonClickCommandHandler(object parameter)
	{
		AsyncDataLoader.BeginLoading(delegate
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				LoadData();
			});
		}, Context.Level2Frame);
	}

	private void RestoreBottonClickCommandHandler(object parameter)
	{
		BackupHistoryListItemViewModel backupHistoryListItemViewModel = HistoryItems.First((BackupHistoryListItemViewModel n) => n.IsSelected);
		_ = backupHistoryListItemViewModel.BackupDescriptionInfo;
		Action<IBackupStorage> task = delegate(IBackupStorage t)
		{
			try
			{
				Context.CurrentRestoreViewType = ViewType.RESTORE;
				Context.Switch(ViewType.RESTORE, t, reload: true, level2: true);
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Restore failed, exception:" + ex.ToString());
			}
		};
		CreateBackupStorage(backupHistoryListItemViewModel, delegate(IBackupStorage backupStorage)
		{
			task(backupStorage);
		});
	}

	private void CreateBackupStorage(BackupHistoryListItemViewModel data, Action<IBackupStorage> callback)
	{
		string storagepath = data.Storagepath;
		_ = data.BackupDescriptionInfo;
		IBackupStorage storage = new BackupStorage(new FileInfo(storagepath));
		try
		{
			string version = string.Empty;
			IBackupResourceReader reader = storage.OpenRead(out version);
			if (reader.IsSetPassword())
			{
				EnterPasswordViewModel enterPasswordViewModel = new EnterPasswordViewModel((string p) => reader.CheckPassword(p));
				EnterPasswordWindow userUi = new EnterPasswordWindow
				{
					DataContext = enterPasswordViewModel
				};
				Context.MessageBox.ShowMessage(userUi);
				if (enterPasswordViewModel.Result == true)
				{
					System.Windows.Application.Current.Dispatcher.Invoke(delegate
					{
						callback(storage);
					});
				}
			}
			else
			{
				callback(storage);
			}
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Read file[" + storagepath + "] failed,throw exception:" + ex);
			storage?.Dispose();
			Context.MessageBox.ShowMessage("K1408", MessageBoxButton.OK, MessageBoxImage.Hand);
			DeleteRestoreFile(new List<BackupHistoryListItemViewModel> { data });
		}
	}

	public void SetButtonEnable(int selectedCount)
	{
		if (selectedCount > 1)
		{
			RestoreEnabled = false;
			ExtractEnabled = false;
			DeleteEnabled = true;
		}
		else if (selectedCount == 1)
		{
			RestoreEnabled = true;
			ExtractEnabled = true;
			DeleteEnabled = true;
		}
		else
		{
			RestoreEnabled = false;
			ExtractEnabled = false;
			DeleteEnabled = false;
		}
	}

	private BackupHistoryListItemViewModel ConvertBackupDescription(BackupDescription desc)
	{
		return new BackupHistoryListItemViewModel
		{
			ModelName = desc.ModelName,
			SizeStrFormat = GlobalFun.ConvertLong2String(desc.StorageSize, "F2"),
			Size = desc.StorageSize,
			BackupDateStrFormat = desc.BackupDateTime,
			Notes = desc.Notes,
			Storagepath = desc.StoragePath,
			BackupDescriptionInfo = desc,
			IsPhone = (string.IsNullOrEmpty(desc.Category) || desc.Category.Equals("phone", StringComparison.CurrentCultureIgnoreCase))
		};
	}
}
