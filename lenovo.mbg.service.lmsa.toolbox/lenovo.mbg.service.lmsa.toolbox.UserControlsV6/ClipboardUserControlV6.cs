using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.tooBox.Business;

namespace lenovo.mbg.service.lmsa.toolbox.UserControlsV6;

public partial class ClipboardUserControlV6 : UserControl, IDisposable, IComponentConnector
{
	private DeviceClipboardManagement mgt = new DeviceClipboardManagement();

	private bool _copyWin2PhoneStatus = true;

	public ClipboardUserControlV6()
	{
		InitializeComponent();
		base.Loaded += delegate
		{
			HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
			DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
			if (masterDevice == null || masterDevice.SoftStatus != DeviceSoftStateEx.Online)
			{
				ToolboxViewContext.SingleInstance.ShowTutorialsView();
			}
		};
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		else
		{
			SetButtonStatus(_enable: false);
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		SetButtonStatus(e == DeviceSoftStateEx.Online);
	}

	private void SetButtonStatus(bool _enable)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			btnTransfer.IsEnabled = _enable;
			btnCopy.IsEnabled = _enable;
		});
	}

	private void TransferClick(object sender, RoutedEventArgs e)
	{
		_copyWin2PhoneStatus = !_copyWin2PhoneStatus;
		if (_copyWin2PhoneStatus)
		{
			btnCopyFrom.LangKey = "K0756";
			btnCopyTo.LangKey = "K0757";
			btnCopy.LangKey = "K0185";
			clipboardTxt.Tag = "K1352";
		}
		else
		{
			btnCopyFrom.LangKey = "K0757";
			btnCopyTo.LangKey = "K0756";
			btnCopy.LangKey = "K0186";
			clipboardTxt.Tag = "K0188";
		}
	}

	private void btnCopy_Click(object sender, RoutedEventArgs e)
	{
		SetButtonStatus(_enable: false);
		if (_copyWin2PhoneStatus)
		{
			string _clipboardText = clipboardTxt.Text;
			if (!string.IsNullOrEmpty(_clipboardText))
			{
				Task.Run(delegate
				{
					Stopwatch stopwatch = new Stopwatch();
					BusinessData businessData = new BusinessData(BusinessType.CLIP_BOARD_IMPORT, HostProxy.deviceManager.MasterDevice);
					stopwatch.Start();
					bool flag = mgt.ImportClipboardInfo(_clipboardText.Trim('\n').Trim('\r'));
					stopwatch.Stop();
					HostProxy.BehaviorService.Collect(BusinessType.CLIP_BOARD_IMPORT, businessData.Update(stopwatch.ElapsedMilliseconds, flag ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, null));
					return flag;
				}).GetAwaiter().OnCompleted(delegate
				{
					ToolboxViewContext.SingleInstance.MessageBox.ShowMessage("K0185", "K0370");
					SetButtonStatus(_enable: true);
				});
			}
			else
			{
				ToolboxViewContext.SingleInstance.MessageBox.ShowMessage("K1413");
				SetButtonStatus(_enable: true);
			}
			return;
		}
		LogHelper.LogInstance.Info("get clipboard content");
		Task<string> task = Task.Run(() => mgt.GetClipboardInfo());
		task.GetAwaiter().OnCompleted(delegate
		{
			string clipboardContent = task.Result;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				try
				{
					clipboardTxt.Text = clipboardContent;
					Clipboard.SetDataObject(clipboardContent);
				}
				catch
				{
				}
			});
			SetButtonStatus(_enable: true);
		});
	}

	public void Dispose()
	{
	}
}
