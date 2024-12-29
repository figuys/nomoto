using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace lenovo.themes.generic.Controls;

public class PopupEx : Popup
{
	public struct RECT
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
	}

	public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(typeof(PopupEx), new FrameworkPropertyMetadata(false, OnTopmostChanged));

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

	private const uint TOPMOST_FLAGS = 1563u;

	public bool Topmost
	{
		get
		{
			return (bool)GetValue(TopmostProperty);
		}
		set
		{
			SetValue(TopmostProperty, value);
		}
	}

	private static void OnTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		(obj as PopupEx).UpdateWindow();
	}

	protected override void OnOpened(EventArgs e)
	{
		UpdateWindow();
	}

	public PopupEx()
	{
		base.Loaded += PopupEx_Loaded;
		base.Unloaded += PopupEx_Unloaded;
	}

	private void PopupEx_Loaded(object sender, RoutedEventArgs e)
	{
		Window window = Window.GetWindow(base.PlacementTarget);
		if (window != null)
		{
			window.LocationChanged += Win_LocationChanged;
			window.SizeChanged += Win_LocationChanged;
		}
	}

	private void PopupEx_Unloaded(object sender, RoutedEventArgs e)
	{
		Window window = Window.GetWindow(base.PlacementTarget);
		if (window != null)
		{
			window.LocationChanged -= Win_LocationChanged;
		}
	}

	private void Win_LocationChanged(object sender, EventArgs e)
	{
		base.HorizontalOffset = base.HorizontalOffset++;
	}

	private void UpdateWindow()
	{
		IntPtr handle = ((HwndSource)PresentationSource.FromVisual(this)).Handle;
		if (GetWindowRect(handle, out var lpRect))
		{
			SetWindowPos(handle, Topmost ? (-1) : (-2), lpRect.Left, lpRect.Top, (int)base.Width, (int)base.Height, 1051u);
		}
	}

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

	[DllImport("user32")]
	private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, uint wFlags);
}
