using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.backuprestore.Business;
using lenovo.mbg.service.lmsa.backuprestore.Business.Backup;
using lenovo.mbg.service.lmsa.backuprestore.View;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.backuprestore.ViewModel;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.backuprestore;

[PluginExport(typeof(IPlugin), "13f79fe4cfc98747c78794a943886bcd")]
public class Plugin : PluginBase
{
	public override FrameworkElement CreateControl(IMessageBox iMessage)
	{
		MainFrame result = new MainFrame
		{
			DataContext = Context.MainFrame
		};
		Context.MessageBox = iMessage;
		Context.MainFrame.LoadData();
		return result;
	}

	public override void Init()
	{
		ResourceDictionary resourceDictionary = new ResourceDictionary();
		resourceDictionary.Source = new Uri("/lenovo.mbg.service.lmsa.backuprestore;component/UserResources/Style.xaml", UriKind.Relative);
		Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
		InitBackupLastDateTime();
	}

	private void InitBackupLastDateTime()
	{
		if (!(Configurations.BackupLastDateTime == DateTime.MinValue))
		{
			return;
		}
		List<BackupDescription> backupList = new DeviceBackupMgt().GetBackupList();
		if (backupList != null && backupList.Count > 0)
		{
			Configurations.BackupLastDateTime = Convert.ToDateTime(backupList.OrderByDescending((BackupDescription m) => m.BackupDateTime).First().BackupDateTime);
		}
		else
		{
			Configurations.BackupLastDateTime = DateTime.MinValue.AddYears(1);
		}
	}

	public override void OnInit(object data)
	{
		base.OnInit(data);
		HostProxy.ViewContext.FindViewModel<BackupRestoreMainViewModel>(typeof(BackupRestoreMainView))?.InitSdCard();
		if (Context.CurrentViewType == ViewType.BACKUPMAIN)
		{
			HostProxy.ViewContext.FindViewModel<BackupMainViewModel>(typeof(BackUpMainView)).LoadData(null);
		}
		else if (Context.CurrentViewType == ViewType.START)
		{
			StartViewModel _startView = HostProxy.ViewContext.FindViewModel<StartViewModel>(typeof(StartView));
			Task.Run(delegate
			{
				_startView.MotoHelperCheck();
			});
		}
	}
}
