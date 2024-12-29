using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.tooBox.Business;
using lenovo.themes.generic.Controls;

namespace lenovo.mbg.service.lmsa.toolbox;

public partial class ClipboardUserControl : Window, IComponentConnector
{
	private const int WM_CLIPBOARDUPDATE = 797;

	private Action ClipboardChangedHandle;

	private DeviceClipboardManagement mgt = new DeviceClipboardManagement();

	private string _UID;

	private IntPtr handle;

	[DllImport("user32.dll")]
	public static extern bool AddClipboardFormatListener(IntPtr hwnd);

	[DllImport("user32.dll")]
	public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

	protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (msg == 797 && ClipboardChangedHandle != null)
		{
			ClipboardChangedHandle();
		}
		return IntPtr.Zero;
	}

	public ClipboardUserControl(string uid)
	{
		InitializeComponent();
		_UID = uid;
		InitializeAction();
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		base.OnClosing(e);
		if (handle != IntPtr.Zero)
		{
			RemoveClipboardFormatListener(handle);
		}
	}

	private void InitializeAction()
	{
		ClipboardChangedHandle = delegate
		{
			try
			{
				IDataObject dataObject = Clipboard.GetDataObject();
				if (dataObject.GetDataPresent(DataFormats.UnicodeText))
				{
					object dataValue = dataObject.GetData(DataFormats.UnicodeText);
					if (dataValue != null)
					{
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							clipboardTxt.Document.Blocks.Clear();
							if (!string.IsNullOrEmpty(dataValue.ToString()))
							{
								Paragraph paragraph = new Paragraph();
								Run item = new Run(dataValue.ToString());
								paragraph.Inlines.Add(item);
								clipboardTxt.Document.Blocks.Add(paragraph);
							}
						});
					}
					else
					{
						HostProxy.CurrentDispatcher?.Invoke(delegate
						{
							clipboardTxt.Document.Blocks.Clear();
						});
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Clipboard changed handle throw exception:" + ex.ToString());
			}
		};
	}

	protected override void OnSourceInitialized(EventArgs e)
	{
		base.OnSourceInitialized(e);
		win_SourceInitialized(this, e);
		handle = new WindowInteropHelper(this).Handle;
		AddClipboardFormatListener(handle);
	}

	private void win_SourceInitialized(object sender, EventArgs e)
	{
		if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
		{
			hwndSource.AddHook(WndProc);
		}
	}

	private void CToPhoneClick(object sender, RoutedEventArgs e)
	{
		TextRange clipboardContent = new TextRange(clipboardTxt.Document.ContentStart, clipboardTxt.Document.ContentEnd);
		if (string.IsNullOrEmpty(clipboardContent.Text))
		{
			return;
		}
		Task.Run(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool flag = mgt.ImportClipboardInfo(clipboardContent.Text.Trim('\n').Trim('\r'));
			stopwatch.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.CLIP_BOARD_IMPORT, new BusinessData(BusinessType.CLIP_BOARD_IMPORT, HostProxy.deviceManager.MasterDevice).Update(stopwatch.ElapsedMilliseconds, flag ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, null));
			return flag;
		}).GetAwaiter().OnCompleted(delegate
		{
			LenovoPopupWindow info = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0185", "K0370", "K0327", null);
			info.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			HostProxy.HostMaskLayerWrapper.New(info, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => info.ShowDialog());
		});
	}

	private void CFormPhoneClick(object sender, RoutedEventArgs e)
	{
		fromPhone.IsEnabled = false;
		LogHelper.LogInstance.Info("get clipboard content");
		Task<string> task = Task.Run(() => mgt.GetClipboardInfo());
		task.GetAwaiter().OnCompleted(delegate
		{
			string clipboardContent = task.Result;
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				try
				{
					Clipboard.SetDataObject(clipboardContent);
				}
				catch (Exception)
				{
				}
				fromPhone.IsEnabled = true;
			});
		});
	}

	public void DeviceConnected()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			deviceConnectedTips.Visibility = Visibility.Collapsed;
			clipboardContent.Visibility = Visibility.Visible;
			toPhone.IsEnabled = true;
			fromPhone.IsEnabled = true;
			toPhone.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));
			fromPhone.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43B5E2"));
			toPhone.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			fromPhone.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
		});
	}

	public void DeviceDisconnected()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			clipboardTxt.Document.Blocks.Clear();
			deviceConnectedTips.Visibility = Visibility.Visible;
			clipboardContent.Visibility = Visibility.Collapsed;
			toPhone.IsEnabled = false;
			fromPhone.IsEnabled = false;
			toPhone.Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136));
			fromPhone.Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136));
			toPhone.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
			fromPhone.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
		});
	}

	private void closeBtn_Click(object sender, RoutedEventArgs e)
	{
		base.DialogResult = true;
		clipboardTxt.Document.Blocks.Clear();
		RemoveClipboardFormatListener(handle);
		ToolboxViewContext.SingleInstance.HostOperationService.CloseMaskLayer(_UID);
	}
}
