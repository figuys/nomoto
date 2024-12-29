using System;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.updateversion;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.UpdateVersion;
using lenovo.mbg.service.lmsa.UpdateVersion.Core;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;
using lenovo.mbg.service.lmsa.ViewV6;

namespace lenovo.mbg.service.lmsa.Business;

public class CheckClientVersion
{
	private MessageBox_Common msgCheckToolVersionCarsh = null;

	private HostUpdateWindowV6 hostUpdateWindow;

	private static CheckClientVersion singleInstance;

	private Action m_NoNewVersionAction = null;

	public VersionModel NewVersionModel { get; private set; }

	public static CheckClientVersion Instance
	{
		get
		{
			if (singleInstance == null)
			{
				singleInstance = new CheckClientVersion();
			}
			return singleInstance;
		}
	}

	private CheckClientVersion()
	{
	}

	public void CheckToolVersion(Action _noNewVersionAction)
	{
		m_NoNewVersionAction = _noNewVersionAction;
		UpdateManager.Instance.InitializeToolVersionUpdate();
		UpdateManager.Instance.ToolUpdateWorker.OnCheckVersionStatusChanged += ToolUpdateWorker_OnCheckVersionStatusChanged;
		UpdateManager.Instance.ToolUpdateWorker.CheckVersion(isAutoMode: true);
	}

	private void SetUIbyManualTrigger(CheckVersionStatus status)
	{
		if (!ApplcationClass.manualTrigger)
		{
			return;
		}
		switch (status)
		{
		case CheckVersionStatus.CHECK_VERSION_HAVE_NEW_VERSION:
			if (!NewVersionModel.ForceType)
			{
				ApplcationClass.manualTrigger = false;
				HostUpdateWindowV6 msg1 = new HostUpdateWindowV6(HostUpdateWindowV6.ViewStatus.DetectedVersion, NewVersionModel.ForceType, NewVersionModel);
				HostProxy.HostMaskLayerWrapper.New(msg1, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
				{
					msg1.ShowDialog();
				});
			}
			break;
		case CheckVersionStatus.CHECK_VERSION_NOT_NEW_VERSION:
			ApplcationClass.manualTrigger = false;
			ApplcationClass.ApplcationStartWindow.ShowMessage("K0285");
			break;
		}
	}

	private void ToolUpdateWorker_OnCheckVersionStatusChanged(object sender, CheckVersionEventArgs e)
	{
		if (!MainWindowControl.Instance.CanExit)
		{
			HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
			{
				if (msgCheckToolVersionCarsh != null && msgCheckToolVersionCarsh.IsActive)
				{
					msgCheckToolVersionCarsh.Close();
				}
			});
			LogHelper.LogInstance.Info("lenovo.mbg.service.lmsa.MainWindow.ToolUpdateWorker_OnCheckVersionStatusChanged  . because of isupdating plug, retrn. checkversionstatus:" + e.Status);
			return;
		}
		LogHelper.LogInstance.Info("lenovo.mbg.service.lmsa.MainWindow.ToolUpdateWorker_OnCheckVersionStatusChanged event. checkversionstatus:" + e.Status);
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			switch (e.Status)
			{
			case CheckVersionStatus.CHECK_VERSION_FAILED:
			case CheckVersionStatus.CHECK_VERSION_DATA_REPEAT:
			case CheckVersionStatus.CHECK_VERSION_DATA_INVALID:
				if (msgCheckToolVersionCarsh != null)
				{
					msgCheckToolVersionCarsh.Close();
				}
				break;
			case CheckVersionStatus.CHECK_VERSION_HAVE_NEW_VERSION:
				NewVersionModel = e.Data as VersionModel;
				if (NewVersionModel != null)
				{
					if (msgCheckToolVersionCarsh != null)
					{
						msgCheckToolVersionCarsh.Close();
					}
					if (NewVersionModel.ForceType)
					{
						ApplcationClass.ForceUpdate = true;
						hostUpdateWindow = new HostUpdateWindowV6(HostUpdateWindowV6.ViewStatus.DetectedVersion, NewVersionModel.ForceType, NewVersionModel);
						HostProxy.HostMaskLayerWrapper.New(hostUpdateWindow, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
						{
							hostUpdateWindow.ShowDialog();
						});
						ApplcationClass.ForceUpdate = false;
					}
					else
					{
						if (e.IsAutoMode)
						{
							UpgradeUserOptionManager upgradeUserOptionManager = new UpgradeUserOptionManager();
							ApplcationClass.manualTrigger = upgradeUserOptionManager.IsRemindToday(NewVersionModel.Version);
						}
						m_NoNewVersionAction?.Invoke();
						m_NoNewVersionAction = null;
					}
				}
				break;
			case CheckVersionStatus.CHECK_VERSION_NOT_NEW_VERSION:
				if (msgCheckToolVersionCarsh != null)
				{
					msgCheckToolVersionCarsh.Close();
				}
				m_NoNewVersionAction?.Invoke();
				m_NoNewVersionAction = null;
				break;
			}
			SetUIbyManualTrigger(e.Status);
		});
	}
}
