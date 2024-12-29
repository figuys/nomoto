using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.download.DownloadUnity;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.updateversion;
using lenovo.mbg.service.lmsa.UpdateVersion;
using lenovo.mbg.service.lmsa.UpdateVersion.Core;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;
using lenovo.mbg.service.lmsa.ViewModels;

namespace lenovo.mbg.service.lmsa;

public partial class HostUpdateWindow : Window, IComponentConnector
{
	public enum ViewStatus
	{
		DetectedVersion,
		DowloadVersion,
		InstallVersion
	}

	private VersionModel mVersionModel;

	private ViewStatus mCurrentViewStatus = ViewStatus.DetectedVersion;

	private MessageBoxResult messageBoxChooseResult = MessageBoxResult.None;

	public static readonly DependencyProperty StrPercentProperty = DependencyProperty.Register("StrPercent", typeof(string), typeof(HostUpdateWindow), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty PercentNumProperty = DependencyProperty.Register("PercentNum", typeof(double), typeof(HostUpdateWindow), new PropertyMetadata(0.0));

	public static readonly DependencyProperty StrDownloadSpeedProperty = DependencyProperty.Register("StrDownloadSpeed", typeof(string), typeof(HostUpdateWindow), new PropertyMetadata(string.Empty));

	private Action updateProgressAction;

	private volatile bool mRefreshUpgradeProgress = false;

	protected VersionModel currentDownloadModel = null;

	public MessageBoxResult MessageBoxChooseResult
	{
		get
		{
			return messageBoxChooseResult;
		}
		set
		{
			messageBoxChooseResult = value;
		}
	}

	public string StrPercent
	{
		get
		{
			return (string)GetValue(StrPercentProperty);
		}
		set
		{
			SetValue(StrPercentProperty, value);
		}
	}

	public double PercentNum
	{
		get
		{
			return (double)GetValue(PercentNumProperty);
		}
		set
		{
			SetValue(PercentNumProperty, value);
			StrPercent = string.Format(LangTranslation.Translate("K0294"), Math.Round(value, 0) + "%");
		}
	}

	public string StrDownloadSpeed
	{
		get
		{
			return (string)GetValue(StrDownloadSpeedProperty);
		}
		set
		{
			SetValue(StrDownloadSpeedProperty, value);
		}
	}

	public HostUpdateWindow(ViewStatus viewStatus, bool isForceUpgrade, VersionModel viewmodel)
	{
		InitializeComponent();
		if (isForceUpgrade)
		{
			checkboxNotRemindMeForThisVersionContent.Visibility = Visibility.Collapsed;
			checkboxNotRemindMeForThisVersion.Visibility = Visibility.Collapsed;
			btnCancelUpgrade.Visibility = Visibility.Collapsed;
			btnCalcelInstall.Visibility = Visibility.Collapsed;
		}
		else
		{
			checkboxNotRemindMeForThisVersionContent.Visibility = Visibility.Visible;
			checkboxNotRemindMeForThisVersion.Visibility = Visibility.Visible;
			btnCancelUpgrade.Visibility = Visibility.Visible;
			btnCalcelInstall.Visibility = Visibility.Visible;
		}
		ApplcationClass.ForceUpdate = viewmodel.ForceType;
		UpdateManager.Instance.ToolUpdateWorker.OnDownloadStatusChanged += ToolUpdateWorker_OnDownloadStatusChanged;
		mVersionModel = viewmodel;
		base.DataContext = viewmodel;
		base.Loaded += MessageBox_Common_Loaded;
		SwitchViewStatus(viewStatus);
		base.Closed += delegate
		{
			StopRefreshUpgradeProgress();
		};
	}

	private void SwitchViewStatus(ViewStatus viewStatus)
	{
		mCurrentViewStatus = viewStatus;
		switch (viewStatus)
		{
		case ViewStatus.DetectedVersion:
			TxbTips.LangKey = "K0286";
			panelDetected.Visibility = Visibility.Visible;
			panelDownload.Visibility = Visibility.Collapsed;
			panelInstall.Visibility = Visibility.Collapsed;
			break;
		case ViewStatus.DowloadVersion:
			panelDetected.Visibility = Visibility.Collapsed;
			panelDownload.Visibility = Visibility.Visible;
			panelInstall.Visibility = Visibility.Collapsed;
			TxbTips.LangKey = "K0293";
			break;
		case ViewStatus.InstallVersion:
			panelDetected.Visibility = Visibility.Collapsed;
			panelDownload.Visibility = Visibility.Collapsed;
			panelInstall.Visibility = Visibility.Visible;
			TxbTips.LangKey = "K0356";
			break;
		}
	}

	private void MessageBox_Common_Loaded(object sender, RoutedEventArgs e)
	{
		UpgradeUserOptionManager upgradeUserOptionManager = new UpgradeUserOptionManager();
		VersionModel versionModel = base.DataContext as VersionModel;
		checkboxNotRemindMeForThisVersion.IsChecked = UpgradeRemindType.NotRemindForTheCurrentVersion.Equals(upgradeUserOptionManager.GetRemindType(versionModel.Version));
		StrPercent = string.Format(LangTranslation.Translate("K0294"), "0%");
	}

	private void btnUpgradeNow_Click(object sender, RoutedEventArgs e)
	{
		updateWarn.Visibility = Visibility.Collapsed;
		SwitchViewStatus(ViewStatus.DowloadVersion);
		BeginDownlad();
	}

	private void btnCancelUpgrade_Click(object sender, RoutedEventArgs e)
	{
		UpgradeRemindType type = ((!checkboxNotRemindMeForThisVersion.IsChecked.HasValue || !checkboxNotRemindMeForThisVersion.IsChecked.Value) ? UpgradeRemindType.RemindTomorrow : UpgradeRemindType.NotRemindForTheCurrentVersion);
		SaveRemindType(type);
		ApplcationClass.manualTrigger = true;
		ApplcationClass.FrameHasNewVersion = true;
		ApplcationClass.IsUpdatingPlug = false;
		VersionModel versionModel = base.DataContext as VersionModel;
		Close();
		ForceStopProcessIfNeed();
	}

	private void btnInstallNow_Click(object sender, RoutedEventArgs e)
	{
		btnInstallNow.LangKey = "K0365";
		btnInstallNow.IsEnabled = false;
		Task.Run(delegate
		{
			UpdateManager.Instance.ToolUpdateWorker.InstallVersion(mVersionModel);
		});
	}

	private void btnCalcelInstall_Click(object sender, RoutedEventArgs e)
	{
		SaveRemindType(UpgradeRemindType.RemindTomorrow);
		Close();
		ForceStopProcessIfNeed();
	}

	private void btnCloseWin_Click(object sender, RoutedEventArgs e)
	{
		if (mCurrentViewStatus == ViewStatus.DetectedVersion)
		{
			btnCancelUpgrade_Click(null, null);
		}
		else
		{
			btnCalcelInstall_Click(null, null);
		}
	}

	private void scrViewerVersionList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer scrollViewer = (ScrollViewer)sender;
		scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - (double)e.Delta);
		e.Handled = true;
	}

	private void SaveRemindType(UpgradeRemindType type)
	{
		VersionModel versionModel = base.DataContext as VersionModel;
		UpgradeUserOptionManager upgradeUserOptionManager = new UpgradeUserOptionManager();
		upgradeUserOptionManager.SaveUpgradeRemindType(new UpgradeRemindTypeInfo
		{
			CurrentVersion = LMSAContext.MainProcessVersion,
			NewVersion = versionModel.Version,
			RemindType = type,
			SetDate = DateTime.Now
		});
	}

	private void ForceStopProcessIfNeed()
	{
		if (base.DataContext is VersionModel { ForceType: not false } && Application.Current.MainWindow is MainWindow && MainWindowViewModel.SingleInstance.CheckCanCloseWindow())
		{
			MainWindowViewModel.SingleInstance.Exit(0);
		}
	}

	private void BeginDownlad()
	{
		ApplcationClass.IsUpdatingPlug = true;
		Task.Factory.StartNew(delegate
		{
			UpdateManager.Instance.ToolUpdateWorker.DownloadVersion(mVersionModel);
		});
	}

	private void BeginRefreshDonwloadProgress()
	{
		if (mRefreshUpgradeProgress)
		{
			return;
		}
		mRefreshUpgradeProgress = true;
		double percent = 0.0;
		if (updateProgressAction == null)
		{
			updateProgressAction = delegate
			{
				PercentNum = percent * 100.0;
				StrDownloadSpeed = currentDownloadModel?.downloadSpeed;
			};
		}
		Task.Factory.StartNew(delegate
		{
			while (mRefreshUpgradeProgress)
			{
				if (currentDownloadModel != null && currentDownloadModel.downloadFileSize > 0)
				{
					percent = double.Parse(currentDownloadModel.downloadedSize.ToString()) / double.Parse(currentDownloadModel.downloadFileSize.ToString());
				}
				base.Dispatcher.Invoke(updateProgressAction);
			}
		});
	}

	private void StopRefreshUpgradeProgress()
	{
		UpdateManager.Instance.ToolUpdateWorker.OnDownloadStatusChanged -= ToolUpdateWorker_OnDownloadStatusChanged;
		updateProgressAction = null;
		mRefreshUpgradeProgress = false;
		currentDownloadModel = null;
		UpdateManager.Instance.ToolUpdateWorker.CancelDownloadVersion(mVersionModel);
	}

	private void ToolUpdateWorker_OnDownloadStatusChanged(object sender, DownloadStatusChangedArgs e)
	{
		base.Dispatcher.Invoke(delegate
		{
			if (e.Data != null)
			{
				currentDownloadModel = e.Data as VersionModel;
				switch (currentDownloadModel.downloadStatus)
				{
				case DownloadStatus.DOWNLOADING:
					BeginRefreshDonwloadProgress();
					break;
				case DownloadStatus.FAILED:
				case DownloadStatus.MD5CHECKFAILED:
				case DownloadStatus.GETFILESIZEFAILED:
				case DownloadStatus.UNENOUGHDISKSPACE:
				case DownloadStatus.CREATEDIRECTORYFAILED:
				case DownloadStatus.DOWNLOADFILENOTFOUND:
				case DownloadStatus.UNDEFINEERROR:
				case DownloadStatus.NETWORKCONNECTIONERROR:
				case DownloadStatus.FILERENAMEFAILED:
					StopRefreshUpgradeProgress();
					SwitchViewStatus(ViewStatus.DetectedVersion);
					base.Dispatcher.Invoke(delegate
					{
						updateWarn.Visibility = Visibility.Visible;
						updateWarn.LangKey = "K0357";
					});
					break;
				case DownloadStatus.PAUSE:
					StopRefreshUpgradeProgress();
					break;
				case DownloadStatus.SUCCESS:
					StopRefreshUpgradeProgress();
					base.Dispatcher.Invoke(delegate
					{
						SwitchViewStatus(ViewStatus.InstallVersion);
						ApplcationClass.IsUpdatingPlug = false;
					});
					break;
				case DownloadStatus.WAITTING:
					StopRefreshUpgradeProgress();
					break;
				default:
					ApplcationClass.IsUpdatingPlug = false;
					break;
				}
			}
		});
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
