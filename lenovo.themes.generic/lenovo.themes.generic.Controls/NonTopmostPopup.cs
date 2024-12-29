using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace lenovo.themes.generic.Controls;

public class NonTopmostPopup : Popup
{
	public struct RECT
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
	}

	public static readonly DependencyProperty IsTopmostProperty = DependencyProperty.Register("IsTopmost", typeof(bool), typeof(NonTopmostPopup), new FrameworkPropertyMetadata(false, OnIsTopmostChanged));

	private bool? _appliedTopMost;

	private bool _alreadyLoaded;

	private Window _parentWindow;

	private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

	private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

	private static readonly IntPtr HWND_TOP = new IntPtr(0);

	private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

	private const uint SWP_NOSIZE = 1u;

	private const uint SWP_NOMOVE = 2u;

	private const uint SWP_NOZORDER = 4u;

	private const uint SWP_NOREDRAW = 8u;

	private const uint SWP_NOACTIVATE = 16u;

	private const uint SWP_FRAMECHANGED = 32u;

	private const uint SWP_SHOWWINDOW = 64u;

	private const uint SWP_HIDEWINDOW = 128u;

	private const uint SWP_NOCOPYBITS = 256u;

	private const uint SWP_NOOWNERZORDER = 512u;

	private const uint SWP_NOSENDCHANGING = 1024u;

	private const uint TOPMOST_FLAGS = 1051u;

	public bool IsTopmost
	{
		get
		{
			return (bool)GetValue(IsTopmostProperty);
		}
		set
		{
			SetValue(IsTopmostProperty, value);
		}
	}

	public NonTopmostPopup()
	{
		base.Loaded += OnPopupLoaded;
		base.Unloaded += OnPopupUnloaded;
	}

	private void OnPopupLoaded(object sender, RoutedEventArgs e)
	{
		if (!_alreadyLoaded)
		{
			_alreadyLoaded = true;
			if (base.Child != null)
			{
				base.Child.AddHandler(UIElement.PreviewMouseLeftButtonDown, new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonDown), handledEventsToo: true);
			}
			_parentWindow = Window.GetWindow(this);
			if (_parentWindow != null)
			{
				_parentWindow.Activated += OnParentWindowActivated;
				_parentWindow.Deactivated += OnParentWindowDeactivated;
			}
		}
	}

	private void OnPopupUnloaded(object sender, RoutedEventArgs e)
	{
		if (_parentWindow != null)
		{
			_parentWindow.Activated -= OnParentWindowActivated;
			_parentWindow.Deactivated -= OnParentWindowDeactivated;
		}
	}

	private void OnParentWindowActivated(object sender, EventArgs e)
	{
		SetTopmostState(isTop: true);
	}

	private void OnParentWindowDeactivated(object sender, EventArgs e)
	{
		if (!IsTopmost)
		{
			SetTopmostState(IsTopmost);
		}
	}

	private void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		SetTopmostState(isTop: true);
		if (!_parentWindow.IsActive && !IsTopmost)
		{
			_parentWindow.Activate();
		}
	}

	private static void OnIsTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		NonTopmostPopup obj2 = (NonTopmostPopup)obj;
		obj2.SetTopmostState(obj2.IsTopmost);
	}

	protected override void OnOpened(EventArgs e)
	{
		SetTopmostState(IsTopmost);
		base.OnOpened(e);
	}

	private void SetTopmostState(bool isTop)
	{
		if ((!_appliedTopMost.HasValue || _appliedTopMost != isTop) && base.Child != null && PresentationSource.FromVisual(base.Child) is HwndSource { Handle: var handle } && GetWindowRect(handle, out var lpRect))
		{
			if (isTop)
			{
				SetWindowPos(handle, HWND_TOPMOST, lpRect.Left, lpRect.Top, (int)base.Width, (int)base.Height, 1051u);
			}
			else
			{
				SetWindowPos(handle, HWND_BOTTOM, lpRect.Left, lpRect.Top, (int)base.Width, (int)base.Height, 1051u);
				SetWindowPos(handle, HWND_TOP, lpRect.Left, lpRect.Top, (int)base.Width, (int)base.Height, 1051u);
				SetWindowPos(handle, HWND_NOTOPMOST, lpRect.Left, lpRect.Top, (int)base.Width, (int)base.Height, 1051u);
			}
			_appliedTopMost = isTop;
		}
	}

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

	[DllImport("user32.dll")]
	private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
}
