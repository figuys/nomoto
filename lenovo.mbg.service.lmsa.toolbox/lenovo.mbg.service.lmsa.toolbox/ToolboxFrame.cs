using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using GoogleAnalytics;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.framework.socket;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.View;
using lenovo.mbg.service.lmsa.toolbox.Ringtone.View;
using lenovo.mbg.service.lmsa.toolbox.ScreenCapture.View;
using lenovo.mbg.service.lmsa.toolbox.ScreenCapture.ViewModel;
using lenovo.mbg.service.lmsa.toolbox.ScreenPlayer;
using lenovo.themes.generic.Component;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.Controls.Windows;
using lenovo.themes.generic.Dialog.Permissions;

namespace lenovo.mbg.service.lmsa.toolbox;

public partial class ToolboxFrame : UserControl, IComponentConnector
{
	private ClipboardUserControl clipboardWindow;

	private ScreenCaptureFrame screenCaptureFrame;

	public static ICommand ClickCommand = new RoutedCommand();

	public IHostOperationService HostOperationService { get; private set; }

	public ToolboxFrame()
	{
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
		InitializeComponent();
		base.CommandBindings.Add(new CommandBinding(ClickCommand, delegate(object sender, ExecutedRoutedEventArgs e)
		{
			switch (e.Parameter.ToString())
			{
			case "CLIPBOARD":
				OnClipBoard();
				break;
			case "GIFMAKER":
				OnGifMaker();
				break;
			case "RINGTONEMAKER":
				OnRingtoneMaker();
				break;
			case "SCREENCAPTURE":
				OnScreenCapture();
				break;
			case "SCREENPLAYER":
				OnScreenPlayer();
				break;
			}
		}));
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		switch (e)
		{
		case DeviceSoftStateEx.Online:
			clipboardWindow?.DeviceConnected();
			break;
		case DeviceSoftStateEx.Offline:
			clipboardWindow?.DeviceDisconnected();
			break;
		}
	}

	private void OnClipBoard()
	{
		HostProxy.BehaviorService.Collect(BusinessType.CLIP_BOARD, null);
		Plugin.Tracker.Send(HitBuilder.CreateCustomEvent(Plugin.Category, "clipboardClick", "Click clipboard tool button", 0L).Build());
		string uid = Guid.NewGuid().ToString("N");
		clipboardWindow = new ClipboardUserControl(uid);
		if (HostProxy.deviceManager.MasterDevice is TcpAndroidDevice { SoftStatus: DeviceSoftStateEx.Online })
		{
			clipboardWindow.DeviceConnected();
		}
		IntPtr owner = ToolboxViewContext.SingleInstance.HostOperationService.ShowMaskLayer(uid);
		Plugin.Tracker.Send(HitBuilder.CreateScreenView("lmsa-plugin-toolbox-clipboard").Build());
		new WindowInteropHelper(clipboardWindow).Owner = owner;
		clipboardWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		clipboardWindow.Height = 560.0;
		clipboardWindow.Width = 800.0;
		clipboardWindow.ShowInTaskbar = false;
		clipboardWindow.ShowDialog();
	}

	private void OnGifMaker()
	{
		HostProxy.BehaviorService.Collect(BusinessType.GIF_MAKER, null);
		Plugin.Tracker.Send(HitBuilder.CreateCustomEvent(Plugin.Category, "gifMakerClick", "Click GIF Maker tool button", 0L).Build());
		try
		{
			string uid = Guid.NewGuid().ToString("N");
			GifMakerView gifMakerView = new GifMakerView();
			IntPtr owner = ToolboxViewContext.SingleInstance.HostOperationService.ShowMaskLayer(uid);
			Plugin.Tracker.Send(HitBuilder.CreateScreenView("lmsa-plugin-toolbox-gifmaker").Build());
			new WindowInteropHelper(gifMakerView).Owner = owner;
			gifMakerView.Title = LangTranslation.Translate("K0206");
			gifMakerView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			gifMakerView.ShowInTaskbar = false;
			gifMakerView.ShowDialog();
			ToolboxViewContext.SingleInstance.HostOperationService.CloseMaskLayer(uid);
			GC.Collect();
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(ex.ToString());
		}
	}

	private void OnRingtoneMaker()
	{
		HostProxy.BehaviorService.Collect(BusinessType.RINGTONE_MAKER, null);
		Plugin.Tracker.Send(HitBuilder.CreateCustomEvent(Plugin.Category, "ringtoneMakerClick", "Click Ringtone Maker tool button", 0L).Build());
		try
		{
			RingtoneMakerView ringtoneMakerView = new RingtoneMakerView();
			string uid = Guid.NewGuid().ToString("N");
			IntPtr owner = ToolboxViewContext.SingleInstance.HostOperationService.ShowMaskLayer(uid);
			Plugin.Tracker.Send(HitBuilder.CreateScreenView("lmsa-plugin-toolbox-ringtonemaker").Build());
			new WindowInteropHelper(ringtoneMakerView).Owner = owner;
			ringtoneMakerView.Title = LangTranslation.Translate("K0234");
			ringtoneMakerView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			ringtoneMakerView.ShowInTaskbar = false;
			ringtoneMakerView.ShowDialog();
			ToolboxViewContext.SingleInstance.HostOperationService.CloseMaskLayer(uid);
			GC.Collect();
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(ex.ToString());
		}
	}

	private void OnScreenCapture()
	{
		HostProxy.BehaviorService.Collect(BusinessType.SCREEN_RECORDER, null);
		Plugin.Tracker.Send(HitBuilder.CreateCustomEvent(Plugin.Category, "screenCaptureClick", "Click Screen Capture tool button", 0L).Build());
		ScreenCaptureFrameViewMode dataContext = new ScreenCaptureFrameViewMode();
		screenCaptureFrame = new ScreenCaptureFrame
		{
			DataContext = dataContext
		};
		HostProxy.HostMaskLayerWrapper.Show(screenCaptureFrame, null, HostMaskLayerWrapper.CloseMaskOperation.NotCloseWhenDeviceDisconnect);
		Plugin.Tracker.Send(HitBuilder.CreateScreenView("lmsa-plugin-toolbox-screencapture").Build());
	}

	private void OnScreenPlayer()
	{
		ScreenView screenView = new ScreenView();
		ScreenViewModel screenViewModel2 = (ScreenViewModel)(screenView.DataContext = new ScreenViewModel(screenView));
		ShowScene(screenView, screenViewModel2, 0);
		screenViewModel2.Stop();
	}

	public void ShowScene(Window win, ScreenViewModel vm, int time)
	{
		TcpAndroidDevice tcpAndroidDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (tcpAndroidDevice?.ConnectedAppType == "Moto")
		{
			MessageBoxEx.Show(HostProxy.HostMaskLayerWrapper, "K1077", MessageBoxButton.OK);
		}
		else if (tcpAndroidDevice == null || tcpAndroidDevice.ConnectedAppType != "Ma" || tcpAndroidDevice.MessageManager == null)
		{
			bool? flag = MessageBoxEx.Show(HostProxy.HostMaskLayerWrapper, "K1047", "K1048", "K1050", "K1049");
			int num = 2;
			if (!flag.HasValue)
			{
				return;
			}
			if (flag == false)
			{
				UsbConnectView conView = new UsbConnectView();
				HostProxy.HostMaskLayerWrapper.New(conView, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => conView.ShowDialog());
				num = conView.VM.CloseMode;
			}
			if (num == 0)
			{
				return;
			}
			vm.SocketProc(win, time);
			if (num == 2)
			{
				Size frameSize = Size.Empty;
				WifiConnectView wnd = new WifiConnectView();
				vm.OnClientStartEvent = delegate(Size size)
				{
					frameSize = size;
					wnd.VM.CloseCommand.Execute(true);
				};
				HostProxy.HostMaskLayerWrapper.New(wnd, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => wnd.ShowDialog());
				if (wnd.VM.IsConnectedtoMA.HasValue)
				{
					if (wnd.VM.IsConnectedtoMA == true)
					{
						ShowScreenView(win, vm, frameSize);
					}
					else
					{
						ShowScene(win, vm, 1);
					}
				}
			}
			else
			{
				ShowScene(win, vm, 1);
			}
		}
		else
		{
			vm.SocketProc(win, time);
			if (tcpAndroidDevice is WifiDeviceEx)
			{
				CmdConnectByWiFi(tcpAndroidDevice, win, vm);
			}
			else
			{
				CmdConnectByUSB(tcpAndroidDevice, win, vm);
			}
		}
	}

	private void CmdConnectByWiFi(TcpAndroidDevice device, Window win, ScreenViewModel vm)
	{
		List<string> param = new List<string>
		{
			"WIFI",
			(device as WifiDeviceEx).DeviceData.IpRSA,
			"10086"
		};
		vm.IsUsbModel = false;
		NotifyStart(device, param, win, vm);
	}

	private void CmdConnectByUSB(TcpAndroidDevice device, Window win, ScreenViewModel vm)
	{
		List<object> adpterAddr = GetAdpterAddr();
		if (adpterAddr == null || adpterAddr.Count == 0)
		{
			NoNetworkView wnd = new NoNetworkView();
			HostProxy.HostMaskLayerWrapper.New(wnd, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => wnd.ShowDialog());
			if (wnd.VM.IsOk)
			{
				GifDisplayWindow view = new GifDisplayWindow();
				view.VM.Init("K1062", new Uri[3]
				{
					new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/tutorial1.gif"),
					new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/tutorial2.gif"),
					new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/tutorial3.gif")
				}, new string[3] { "K1063", "K1064", "K1065" });
				HostProxy.HostMaskLayerWrapper.New(view, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => view.ShowDialog());
			}
		}
		else
		{
			List<string> list = new List<string>();
			list.Add("ADB");
			list.AddRange(adpterAddr.Select((dynamic p) => p.ip as string));
			list.Add("10086");
			vm.IsUsbModel = true;
			NotifyStart(device, list, win, vm);
		}
	}

	private void ShowScreenView(Window win, ScreenViewModel vm, Size size)
	{
		vm.InitFFmpeg((int)size.Width, (int)size.Height);
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => win.ShowDialog());
	}

	private void NotifyStart(TcpAndroidDevice device, List<string> param, Window win, ScreenViewModel vm)
	{
		Size screenSize = Size.Empty;
		switch (NotifyApkClientIp(device, param, out screenSize))
		{
		case "1":
			ShowScreenView(win, vm, screenSize);
			return;
		case "2":
		{
			MAHideNotifyView wnd = new MAHideNotifyView();
			HostProxy.HostMaskLayerWrapper.New(wnd, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => wnd.ShowDialog());
			if (wnd.VM.IsOk)
			{
				NotifyStart(device, param, win, vm);
			}
			return;
		}
		case "3":
		{
			PicVideoAuthorizeView wnd2 = new PicVideoAuthorizeView();
			HostProxy.HostMaskLayerWrapper.New(wnd2, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => wnd2.ShowDialog());
			if (true == wnd2.DialogResult)
			{
				NotifyStart(device, param, win, vm);
			}
			return;
		}
		case "4":
		{
			string message = "To screen mirror, MA need back to home page first!";
			MessageBoxEx.Show(HostProxy.HostMaskLayerWrapper, message, MessageBoxButton.OK);
			return;
		}
		}
		NoNetworkView wnd3 = new NoNetworkView();
		HostProxy.HostMaskLayerWrapper.New(wnd3, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => wnd3.ShowDialog());
		if (wnd3.VM.IsOk)
		{
			GifDisplayWindow view = new GifDisplayWindow();
			view.VM.Init("K1062", new Uri[3]
			{
				new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/tutorial1.gif"),
				new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/tutorial2.gif"),
				new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.toolbox;component/Resources/tutorial3.gif")
			}, new string[3] { "K1063", "K1064", "K1065" });
			HostProxy.HostMaskLayerWrapper.New(view, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => view.ShowDialog());
		}
	}

	private string NotifyApkClientIp(TcpAndroidDevice device, List<string> param, out Size screenSize)
	{
		screenSize = new Size(0.0, 0.0);
		using MessageReaderAndWriter messageReaderAndWriter = device.MessageManager?.CreateMessageReaderAndWriter(8000);
		if (messageReaderAndWriter == null)
		{
			return "0";
		}
		long sequence = HostProxy.Sequence.New();
		List<PropItem> receiveData = null;
		try
		{
			if (messageReaderAndWriter.SendAndReceiveSync("startScreenCapture", "startScreenCaptureResponse", param, sequence, out receiveData) && receiveData != null)
			{
				PropItem propItem = receiveData.FirstOrDefault((PropItem p) => p.Key == "actionStatus");
				if (propItem == null)
				{
					return "0";
				}
				string value = propItem.Value;
				if (value == "1")
				{
					propItem = receiveData.FirstOrDefault((PropItem p) => p.Key == "width");
					if (propItem != null)
					{
						screenSize.Width = Convert.ToInt32(propItem.Value);
					}
					propItem = receiveData.FirstOrDefault((PropItem p) => p.Key == "height");
					if (propItem != null)
					{
						screenSize.Height = Convert.ToInt32(propItem.Value);
					}
				}
				return value;
			}
			return "0";
		}
		catch (Exception)
		{
			return "0";
		}
	}

	public List<dynamic> GetAdpterAddr()
	{
		List<object> list = new List<object>();
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface networkInterface in allNetworkInterfaces)
		{
			if (networkInterface.OperationalStatus != OperationalStatus.Up || networkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
			{
				continue;
			}
			UnicastIPAddressInformationCollection unicastAddresses = networkInterface.GetIPProperties().UnicastAddresses;
			if (unicastAddresses.Count == 0)
			{
				continue;
			}
			foreach (UnicastIPAddressInformation item in unicastAddresses)
			{
				if (item.Address.AddressFamily == AddressFamily.InterNetwork)
				{
					list.Add(new
					{
						ip = item.Address.ToString(),
						type = networkInterface.NetworkInterfaceType
					});
				}
			}
		}
		return list;
	}
}
