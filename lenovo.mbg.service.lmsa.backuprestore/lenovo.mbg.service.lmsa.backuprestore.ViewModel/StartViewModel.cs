using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.backuprestore.ViewContext;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Controls.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class StartViewModel : ViewModelBase
{
	private WaitTips m_loadingWindow;

	private bool m_ChangingDH2MA;

	protected static IUserMsgControl m_WifiDialog;

	public MainFrameViewModel MFrame => Context.MainFrame;

	public ReplayCommand LinkCommand { get; }

	public StartViewModel()
	{
		LinkCommand = new ReplayCommand(LinkCommandHandler);
	}

	private void LinkCommandHandler(object args)
	{
		Context.Switch(ViewType.USBCONNECTHELPER, null, reload: true);
	}

	public void MotoHelperCheck()
	{
		DeviceEx _currentDevice = Context.CurrentDevice;
		if (_currentDevice == null || _currentDevice.SoftStatus != DeviceSoftStateEx.Online)
		{
			return;
		}
		if (m_WifiDialog != null)
		{
			HostProxy.CurrentDispatcher.Invoke(delegate
			{
				m_WifiDialog?.GetMsgUi()?.Close();
				m_WifiDialog?.CloseAction?.Invoke(true);
				m_WifiDialog = null;
			});
		}
		if (!(_currentDevice.ConnectedAppType == "Moto"))
		{
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			bool? flag = null;
			if (((_currentDevice.ConnectType != ConnectType.Wifi) ? Context.MessageBox.ShowMessage("K0071", "K1848", "K0327", "K0208", isCloseBtn: false, null, MessageBoxImage.Exclamation) : Context.MessageBox.ShowMessage("K1849", out m_WifiDialog, MessageBoxImage.Exclamation, MessageBoxButton.OKCancel)) == true)
			{
				Context.MessageBox.SetDeviceConnectIconStatus(-1);
				m_ChangingDH2MA = true;
				Configurations.AppMinVersionCodeOfMoto = 900805000;
				_currentDevice.SoftStatus = DeviceSoftStateEx.Offline;
				m_loadingWindow = new WaitTips("K1854");
				if (_currentDevice.ConnectType != ConnectType.Wifi)
				{
					Task.Run(delegate
					{
						Application.Current.Dispatcher.Invoke(delegate
						{
							HostProxy.HostMaskLayerWrapper.New(m_loadingWindow, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => m_loadingWindow.ShowDialog());
						});
					});
				}
				_currentDevice.PhysicalStatusChanged -= DH2MA_PhysicalStatusChanged;
				_currentDevice.PhysicalStatusChanged += DH2MA_PhysicalStatusChanged;
				if (_currentDevice is TcpAndroidDevice tcpAndroidDevice)
				{
					tcpAndroidDevice.TcpConnectStepChanged -= DH2MA_SoftStatusChanged;
					tcpAndroidDevice.TcpConnectStepChanged += DH2MA_SoftStatusChanged;
				}
			}
		});
	}

	private void DH2MA_PhysicalStatusChanged(object sender, DevicePhysicalStateEx e)
	{
		if (e == DevicePhysicalStateEx.Offline)
		{
			Context.MessageBox.SetDeviceConnectIconStatus(99);
			Application.Current.Dispatcher.Invoke(delegate
			{
				m_loadingWindow?.Close();
			});
			m_ChangingDH2MA = false;
		}
	}

	private void DH2MA_SoftStatusChanged(object sender, TcpConnectStepChangedEventArgs e)
	{
		if (!m_ChangingDH2MA)
		{
			return;
		}
		string step = e.Step;
		if (!(step == "TcpConnect"))
		{
			if (!(step == "InstallApp"))
			{
				return;
			}
			switch (e.Result)
			{
			case ConnectStepStatus.Fail:
				Application.Current.Dispatcher.Invoke(delegate
				{
					m_loadingWindow?.Close();
				});
				m_ChangingDH2MA = false;
				break;
			case ConnectStepStatus.Starting:
				Application.Current.Dispatcher.Invoke(delegate
				{
					m_loadingWindow?.Close();
				});
				break;
			case ConnectStepStatus.Success:
				break;
			}
		}
		else
		{
			Context.MessageBox.SetDeviceConnectIconStatus(0);
			Application.Current.Dispatcher.Invoke(delegate
			{
				m_loadingWindow?.Close();
			});
			m_ChangingDH2MA = false;
		}
	}
}
