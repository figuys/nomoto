using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using lenovo.mbg.service.lmsa.backuprestore.Common;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class BackupRestoreMainViewModel : ViewModelBase, ILoading
{
	private readonly object loadingLock = new object();

	private List<object> _LoadintCounter;

	private Visibility _TitleBarlVisible;

	private Visibility _btnHelpGuidelVisible;

	private ListViewItemModel _SelectedItem;

	private object _currentView;

	private Visibility _loadingViewVisibility = Visibility.Collapsed;

	protected SdCardViewModel sdCarVm;

	public ObservableCollection<ListViewItemModel> CategoryList { get; set; }

	public Visibility TitleBarlVisible
	{
		get
		{
			return _TitleBarlVisible;
		}
		set
		{
			_TitleBarlVisible = value;
			OnPropertyChanged("TitleBarlVisible");
		}
	}

	public Visibility BtnHelpGuidelVisible
	{
		get
		{
			return _btnHelpGuidelVisible;
		}
		set
		{
			_btnHelpGuidelVisible = value;
			if (HostProxy.deviceManager.MasterDevice?.Property.Category == "tablet")
			{
				_btnHelpGuidelVisible = Visibility.Hidden;
			}
			OnPropertyChanged("BtnHelpGuidelVisible");
		}
	}

	public ListViewItemModel SelectedItem
	{
		get
		{
			return _SelectedItem;
		}
		set
		{
			if (_SelectedItem == value)
			{
				return;
			}
			_SelectedItem = value;
			if (_SelectedItem != null && _SelectedItem.Id == 1)
			{
				SdCarVm.StorageSelVisible = Visibility.Hidden;
				TabChanged(Context.CurrentRestoreViewType);
			}
			else
			{
				if (HostProxy.deviceManager.MasterDevice?.ConnectedAppType == "Ma")
				{
					SdCarVm.StorageSelIndex = 0;
					SdCarVm.StorageSelVisible = Visibility.Visible;
				}
				else
				{
					SdCarVm.StorageSelIndex = -1;
					SdCarVm.StorageSelVisible = Visibility.Hidden;
				}
				TabChanged(ViewType.BACKUPMAIN);
			}
			OnPropertyChanged("SelectedItem");
		}
	}

	public object CurrentView
	{
		get
		{
			return _currentView;
		}
		set
		{
			_currentView = value;
			OnPropertyChanged("CurrentView");
		}
	}

	public Visibility LoadingViewVisibility
	{
		get
		{
			return _loadingViewVisibility;
		}
		set
		{
			if (_loadingViewVisibility != value)
			{
				_loadingViewVisibility = value;
				OnPropertyChanged("LoadingViewVisibility");
			}
		}
	}

	public SdCardViewModel SdCarVm
	{
		get
		{
			return sdCarVm;
		}
		set
		{
			sdCarVm = value;
			OnPropertyChanged("SdCarVm");
		}
	}

	public BackupRestoreMainViewModel()
	{
		SdCarVm = new SdCardViewModel();
		SdCarVm.StorageSelIndex = -1;
		SdCarVm.StorageSelVisible = Visibility.Hidden;
		CategoryList = new ObservableCollection<ListViewItemModel>();
		CategoryList.Add(new ListViewItemModel(0, "K0594", "v6_icon_backup", "v6_icon_backup_selected"));
		CategoryList.Add(new ListViewItemModel(1, "K0595", "v6_icon_restore", "v6_icon_restore_selected"));
		_LoadintCounter = new List<object>();
		Context.Level2Frame = this;
	}

	public override void LoadData(object data)
	{
		InitSdCard();
		HostProxy.CurrentDispatcher?.Invoke(() => SelectedItem = CategoryList.First());
		base.LoadData(data);
	}

	public void InitSdCard()
	{
		SdCarVm.LoadData(Context.CurrentDevice);
	}

	public void Show(object handler)
	{
		lock (loadingLock)
		{
			if (!_LoadintCounter.Contains(handler))
			{
				_LoadintCounter.Add(handler);
				LoadingViewVisibility = Visibility.Visible;
			}
		}
	}

	public void Hiden(object handler)
	{
		lock (loadingLock)
		{
			if (_LoadintCounter.Contains(handler))
			{
				_LoadintCounter.Remove(handler);
				LoadingViewVisibility = ((_LoadintCounter.Count == 0) ? Visibility.Collapsed : Visibility.Visible);
			}
		}
	}

	public void Abort()
	{
		lock (loadingLock)
		{
			_LoadintCounter.Clear();
			LoadingViewVisibility = Visibility.Collapsed;
		}
	}

	private void TabChanged(ViewType viewType)
	{
		bool reload = true;
		if (Convert.ToInt32(viewType) == 31)
		{
			reload = false;
		}
		Context.Switch(viewType, null, reload, level2: true);
	}
}
