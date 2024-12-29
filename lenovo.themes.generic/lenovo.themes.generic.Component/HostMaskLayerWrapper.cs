using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using lenovo.mbg.service.framework.services;

namespace lenovo.themes.generic.Component;

public class HostMaskLayerWrapper
{
	public enum CloseMaskOperation
	{
		CloseWhenDeviceDisconnect = 1,
		NotCloseWhenDeviceDisconnect,
		ForceCloseWhenDeviceDisconnect
	}

	public class HostMaskLayer
	{
		private HostMaskLayerWrapper outer;

		public Func<bool> CloseCondition;

		private volatile bool canClose;

		public string UID { get; private set; }

		public bool CloseMasklayerAfterWinClosed { get; set; }

		public CloseMaskOperation CloseOperation { get; set; }

		public FrameworkElement ContentFrameworkElement { get; set; }

		public Window ContentHostWindow { get; private set; }

		public HostMaskLayer(HostMaskLayerWrapper outer, FrameworkElement frameworkElement, Action winCloseCallback, Func<bool> closeCondtion)
		{
			HostMaskLayer hostMaskLayer = this;
			this.outer = outer;
			UID = Guid.NewGuid().ToString("N");
			lock (hostMaskLaytersLockObj)
			{
				hostMaskLayters.Add(this);
			}
			ContentFrameworkElement = frameworkElement;
			CloseCondition = closeCondtion;
			Window window = frameworkElement as Window;
			if (window == null)
			{
				window = new Window
				{
					Content = frameworkElement,
					Background = new SolidColorBrush(Colors.Transparent),
					AllowsTransparency = true,
					SizeToContent = SizeToContent.WidthAndHeight,
					WindowStyle = WindowStyle.None
				};
			}
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.ShowInTaskbar = false;
			window.ResizeMode = ResizeMode.NoResize;
			ContentHostWindow = window;
			if (CloseCondition != null)
			{
				window.Closing += delegate(object s, CancelEventArgs e)
				{
					if (!hostMaskLayer.CanClose())
					{
						e.Cancel = true;
					}
				};
			}
			window.Closed += delegate
			{
				winCloseCallback?.BeginInvoke(null, null);
			};
			CloseMasklayerAfterWinClosed = true;
		}

		public void Show()
		{
			ProcessWithMask(delegate
			{
				Task.Factory.StartNew(delegate
				{
					Application.Current.Dispatcher.Invoke(delegate
					{
						ContentHostWindow?.ShowDialog();
					});
				});
			});
		}

		public void ShowDialog()
		{
			ProcessWithMask(delegate
			{
				if (Thread.CurrentThread.IsBackground)
				{
					Application.Current.Dispatcher.Invoke(delegate
					{
						ContentHostWindow?.ShowDialog();
					});
				}
				else
				{
					ContentHostWindow.ShowDialog();
				}
			});
		}

		public void ProcessWithMask(Action task, Action<Window> callback = null)
		{
			if (outer.service == null)
			{
				throw new Exception("Host operation server is null");
			}
			if (CloseMasklayerAfterWinClosed && ContentHostWindow != null)
			{
				ContentHostWindow.Closed += delegate
				{
					lock (hostMaskLaytersLockObj)
					{
						hostMaskLayters.Remove(this);
					}
					outer.service.CloseMaskLayer(UID);
				};
			}
			IntPtr owner = outer.service.ShowMaskLayer(UID);
			if (ContentHostWindow != null)
			{
				callback?.Invoke(ContentHostWindow);
				new WindowInteropHelper(ContentHostWindow).Owner = owner;
			}
			task?.Invoke();
		}

		public bool? ProcessWithMask(Func<bool?> task)
		{
			if (outer.service == null)
			{
				throw new Exception("Host operation server is null");
			}
			if (CloseMasklayerAfterWinClosed && ContentHostWindow != null)
			{
				ContentHostWindow.Closed += delegate
				{
					lock (hostMaskLaytersLockObj)
					{
						hostMaskLayters.Remove(this);
					}
					outer.service.CloseMaskLayer(UID);
				};
			}
			IntPtr owner = outer.service.ShowMaskLayer(UID);
			if (ContentHostWindow != null)
			{
				new WindowInteropHelper(ContentHostWindow).Owner = owner;
			}
			return task?.Invoke();
		}

		public void Close()
		{
			if (ContentHostWindow != null && ContentHostWindow.IsVisible)
			{
				ContentHostWindow.Close();
			}
			outer.service.CloseMaskLayer(UID);
			lock (hostMaskLaytersLockObj)
			{
				hostMaskLayters.Remove(this);
			}
		}

		private bool CanClose()
		{
			if (canClose)
			{
				return true;
			}
			if (CloseCondition == null)
			{
				return canClose = CloseOperation == CloseMaskOperation.ForceCloseWhenDeviceDisconnect;
			}
			return canClose = CloseCondition();
		}

		public void FireCloseConditionCheck()
		{
			if (CanClose())
			{
				Close();
			}
		}
	}

	private IHostOperationService service;

	private static List<HostMaskLayer> hostMaskLayters = new List<HostMaskLayer>();

	private static readonly object hostMaskLaytersLockObj = new object();

	public HostMaskLayerWrapper(IHostOperationService service)
	{
		this.service = service;
	}

	public HostMaskLayer New(Window childWin, bool closeMasklayerAfterWinClosed, bool showInTaskbar = false, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterOwner, CloseMaskOperation closeOperation = CloseMaskOperation.CloseWhenDeviceDisconnect)
	{
		if (childWin == null)
		{
			throw new Exception("Child wind is null");
		}
		childWin.ShowInTaskbar = showInTaskbar;
		childWin.WindowStartupLocation = windowStartupLocation;
		return new HostMaskLayer(this, childWin, null, null)
		{
			CloseMasklayerAfterWinClosed = closeMasklayerAfterWinClosed,
			CloseOperation = closeOperation
		};
	}

	public void ShowDialog(FrameworkElement frameworkElement, CloseMaskOperation closeMaskOperation = CloseMaskOperation.CloseWhenDeviceDisconnect)
	{
		HostMaskLayer hostMaskLayer = new HostMaskLayer(this, frameworkElement, null, null);
		hostMaskLayer.CloseMasklayerAfterWinClosed = true;
		hostMaskLayer.CloseOperation = closeMaskOperation;
		hostMaskLayer.ShowDialog();
	}

	public void Show(FrameworkElement frameworkElement, CloseMaskOperation closeMaskOperation, bool single = true)
	{
		Show(frameworkElement, null, closeMaskOperation, single);
	}

	public void Show(FrameworkElement frameworkElement, bool single = true)
	{
		Show(frameworkElement, (Func<bool>)null, single);
	}

	public void Show(FrameworkElement frameworkElement, Action winClosedCallback, bool single = true)
	{
		Show(frameworkElement, winClosedCallback, null, single);
	}

	public void Show(FrameworkElement frameworkElement, Func<bool> closeCondtion, bool single = true)
	{
		Show(frameworkElement, null, closeCondtion);
	}

	public void Show(FrameworkElement frameworkElement, Action winCloseCallback, Func<bool> closeCondtion, bool single = true)
	{
		if (single && frameworkElement != null)
		{
			lock (hostMaskLaytersLockObj)
			{
				if (hostMaskLayters.Exists((HostMaskLayer m) => m.ContentFrameworkElement.GetType().Equals(frameworkElement.GetType())))
				{
					return;
				}
			}
		}
		HostMaskLayer hostMaskLayer = new HostMaskLayer(this, frameworkElement, winCloseCallback, closeCondtion);
		hostMaskLayer.CloseMasklayerAfterWinClosed = true;
		hostMaskLayer.CloseOperation = CloseMaskOperation.CloseWhenDeviceDisconnect;
		hostMaskLayer.Show();
	}

	public void Show(FrameworkElement frameworkElement, Func<bool> closeCondtion, CloseMaskOperation closeMaskOperation, bool single = true)
	{
		if (single && frameworkElement != null)
		{
			lock (hostMaskLaytersLockObj)
			{
				if (hostMaskLayters.Exists((HostMaskLayer m) => m.ContentFrameworkElement.GetType().Equals(frameworkElement.GetType())))
				{
					return;
				}
			}
		}
		HostMaskLayer hostMaskLayer = new HostMaskLayer(this, frameworkElement, null, closeCondtion);
		hostMaskLayer.CloseMasklayerAfterWinClosed = true;
		hostMaskLayer.CloseOperation = closeMaskOperation;
		hostMaskLayer.Show();
	}

	public void Close(FrameworkElement frameworkElement)
	{
		lock (hostMaskLaytersLockObj)
		{
			foreach (HostMaskLayer item in hostMaskLayters.Where((HostMaskLayer m) => m.ContentFrameworkElement == frameworkElement).ToList())
			{
				hostMaskLayters.Remove(item);
				item.ContentHostWindow?.Close();
			}
		}
	}

	public void FireCloseConditionCheck()
	{
		List<HostMaskLayer> list = null;
		lock (hostMaskLaytersLockObj)
		{
			list = hostMaskLayters.ToList();
		}
		foreach (HostMaskLayer item in list)
		{
			item.FireCloseConditionCheck();
		}
	}

	public void CloseAll(CloseMaskOperation closeOperation)
	{
		List<HostMaskLayer> list = null;
		lock (hostMaskLaytersLockObj)
		{
			list = hostMaskLayters.ToList();
		}
		list.Reverse();
		if ((closeOperation & CloseMaskOperation.CloseWhenDeviceDisconnect) == CloseMaskOperation.CloseWhenDeviceDisconnect)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list[num].CloseOperation != CloseMaskOperation.NotCloseWhenDeviceDisconnect)
				{
					list[num].ContentHostWindow?.Close();
				}
			}
		}
		else
		{
			if ((closeOperation & CloseMaskOperation.NotCloseWhenDeviceDisconnect) != CloseMaskOperation.NotCloseWhenDeviceDisconnect)
			{
				return;
			}
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				if (list[num2].CloseOperation != CloseMaskOperation.NotCloseWhenDeviceDisconnect)
				{
					list[num2].ContentHostWindow?.Close();
				}
			}
		}
	}
}
