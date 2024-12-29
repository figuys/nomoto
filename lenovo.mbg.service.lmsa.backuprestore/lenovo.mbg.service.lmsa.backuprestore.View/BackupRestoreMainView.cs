using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.backuprestore.ViewModel;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.backuprestore.View;

public partial class BackupRestoreMainView : UserControl, IComponentConnector
{
	private CategoryViewModel vmSMS;

	private CategoryViewModel vmCONTACT;

	private CategoryViewModel vmCALLLOG;

	public BackupRestoreMainView()
	{
		InitializeComponent();
		base.Loaded += delegate
		{
			HostProxy.deviceManager.MasterDeviceChanged -= DeviceManager_OnMasterDeviceChanged;
			HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_OnMasterDeviceChanged;
			sdview.Cbx.SelectionChanged -= OnStorageChanged;
			sdview.Cbx.SelectionChanged += OnStorageChanged;
			if (base.DataContext is BackupRestoreMainViewModel backupRestoreMainViewModel)
			{
				_ = (backupRestoreMainViewModel.CurrentView as BackUpMainView)?.DataContext is BackupMainViewModel;
			}
		};
	}

	private void DeviceManager_OnMasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			bool? flag = null;
			if (mainContent.Content is BackUpMainView)
			{
				BackupMainViewModel backupMainViewModel = (mainContent.Content as BackUpMainView).DataContext as BackupMainViewModel;
				if (backupMainViewModel?.VM != null)
				{
					flag = backupMainViewModel.VM.IsRunning;
				}
			}
			else if (mainContent.Content is RestoreView)
			{
				RestoreViewModel restoreViewModel = (mainContent.Content as RestoreView).DataContext as RestoreViewModel;
				if (restoreViewModel?.VM != null)
				{
					flag = restoreViewModel.VM.IsRunning;
				}
			}
			if (e.Current?.Property.Category == "tablet" || flag.HasValue)
			{
				btnHelpGuide.Visibility = Visibility.Hidden;
			}
			else
			{
				btnHelpGuide.Visibility = Visibility.Visible;
			}
		});
	}

	private void OnStorageChanged(object sender, SelectionChangedEventArgs e)
	{
		if ((sender as ComboBox).SelectedIndex == -1 || !(base.DataContext is BackupRestoreMainViewModel backupRestoreMainViewModel) || !((backupRestoreMainViewModel.CurrentView as BackUpMainView)?.DataContext is BackupMainViewModel backupMainViewModel))
		{
			return;
		}
		if (backupMainViewModel.UpdateSelSotrageAction == null)
		{
			backupMainViewModel.UpdateSelSotrageAction = SetSelStorage;
		}
		bool flag = backupRestoreMainViewModel.SdCarVm.StorageSelIndex == 0;
		LogHelper.LogInstance.Info("Storage drop down box selected: " + (flag ? "Internal" : "SD Card"));
		if (flag)
		{
			if (vmCONTACT != null && !backupMainViewModel.CategoriesViewModel.Categories.Contains(vmCONTACT))
			{
				backupMainViewModel.CategoriesViewModel.Categories.Insert(0, vmCONTACT);
			}
			if (vmCALLLOG != null && !backupMainViewModel.CategoriesViewModel.Categories.Contains(vmCALLLOG))
			{
				backupMainViewModel.CategoriesViewModel.Categories.Insert(0, vmCALLLOG);
			}
			if (vmSMS != null && !backupMainViewModel.CategoriesViewModel.Categories.Contains(vmSMS))
			{
				backupMainViewModel.CategoriesViewModel.Categories.Insert(0, vmSMS);
			}
		}
		else
		{
			vmSMS = backupMainViewModel.CategoriesViewModel.Categories.Where((CategoryViewModel x) => x.ResourceType == "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}").FirstOrDefault();
			vmCONTACT = backupMainViewModel.CategoriesViewModel.Categories.Where((CategoryViewModel x) => x.ResourceType == "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}").FirstOrDefault();
			vmCALLLOG = backupMainViewModel.CategoriesViewModel.Categories.Where((CategoryViewModel x) => x.ResourceType == "{89D4DB68-4258-4002-8557-E65959C558B3}").FirstOrDefault();
			backupMainViewModel.CategoriesViewModel.Categories.Remove(vmSMS);
			backupMainViewModel.CategoriesViewModel.Categories.Remove(vmCALLLOG);
			backupMainViewModel.CategoriesViewModel.Categories.Remove(vmCONTACT);
		}
		if (backupMainViewModel.IsInternalStorage != flag)
		{
			backupMainViewModel.IsInternalStorage = flag;
			backupMainViewModel.LoadResourceInfo();
		}
	}

	private void SetSelStorage(bool isInternal)
	{
		if (HostProxy.deviceManager.MasterDevice?.ConnectedAppType == "Moto")
		{
			(base.DataContext as BackupRestoreMainViewModel).SdCarVm.StorageSelIndex = -1;
		}
		else
		{
			(base.DataContext as BackupRestoreMainViewModel).SdCarVm.StorageSelIndex = 0;
		}
	}

	private void BtnHelpGuideClick(object sender, RoutedEventArgs e)
	{
		Context.MessageBox.ShowMessage(new BackUpHelpGuideView());
	}
}
