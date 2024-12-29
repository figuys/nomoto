using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace lenovo.themes.generic.AttachedProperty;

public class PopupManager
{
	public static readonly DependencyProperty AutoMoveProperty;

	private static volatile bool mHaveRegisterEvent;

	static PopupManager()
	{
		AutoMoveProperty = DependencyProperty.RegisterAttached("AutoMove", typeof(bool), typeof(PopupManager), new PropertyMetadata(false, AutoMovePropertyChangedCallback));
		mHaveRegisterEvent = false;
	}

	public static bool GetAutoMove(DependencyObject obj)
	{
		return (bool)obj.GetValue(AutoMoveProperty);
	}

	public static void SetAutoMove(DependencyObject obj, bool value)
	{
		obj.SetValue(AutoMoveProperty, value);
	}

	private static void AutoMovePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		Popup popup = d as Popup;
		if (popup == null || mHaveRegisterEvent)
		{
			return;
		}
		EventHandler mainWindowStateChangedEventHandler = delegate
		{
			Task.Run(delegate
			{
				int num = 0;
				while (++num < 3)
				{
					Thread.Sleep(100);
					popup.Dispatcher.Invoke(delegate
					{
						ResetPopupPosition(popup);
					});
				}
			});
		};
		EventHandler mainWindowLocationChangedEventHandler = delegate
		{
			ResetPopupPosition(popup);
		};
		SizeChangedEventHandler mainWindowSizeChangedEventHandler = delegate
		{
			ResetPopupPosition(popup);
		};
		EventHandler value = delegate
		{
			Application.Current.MainWindow.LocationChanged += mainWindowLocationChangedEventHandler;
			Application.Current.MainWindow.SizeChanged += mainWindowSizeChangedEventHandler;
			Application.Current.MainWindow.StateChanged += mainWindowStateChangedEventHandler;
		};
		EventHandler value2 = delegate
		{
			Application.Current.MainWindow.LocationChanged -= mainWindowLocationChangedEventHandler;
			Application.Current.MainWindow.SizeChanged -= mainWindowSizeChangedEventHandler;
			Application.Current.MainWindow.StateChanged -= mainWindowStateChangedEventHandler;
		};
		popup.Opened += value;
		popup.Closed += value2;
	}

	public static void ResetPopupPosition(Popup popup)
	{
		popup.HorizontalOffset = popup.HorizontalOffset++;
	}
}
