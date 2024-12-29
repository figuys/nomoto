using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace lenovo.themes.generic.Controls.Windows;

public partial class LenovoWindowNew : Window, IComponentConnector, IStyleConnector
{
	public static readonly DependencyProperty TitlePanelHeightProperty = DependencyProperty.Register("TitlePanelHeight", typeof(double), typeof(LenovoWindowNew), new PropertyMetadata(double.NaN));

	public static readonly DependencyProperty TitlePanelBackgroundProperty = DependencyProperty.Register("TitlePanelBackground", typeof(Brush), typeof(LenovoWindowNew), new PropertyMetadata(null));

	public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register("TitleText", typeof(string), typeof(LenovoWindowNew), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty CloseButtonVisibilityProperty = DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(LenovoWindowNew), new PropertyMetadata(Visibility.Visible));

	public static readonly DependencyProperty ClosedCommandProperty = DependencyProperty.Register("ClosedCommand", typeof(ICommand), typeof(LenovoWindowNew), new PropertyMetadata(null));

	private int result;

	public double TitlePanelHeight
	{
		get
		{
			return (double)GetValue(TitlePanelHeightProperty);
		}
		set
		{
			SetValue(TitlePanelHeightProperty, value);
		}
	}

	public Brush TitlePanelBackground
	{
		get
		{
			return (Brush)GetValue(TitlePanelBackgroundProperty);
		}
		set
		{
			SetValue(TitlePanelBackgroundProperty, value);
		}
	}

	public string TitleText
	{
		get
		{
			return (string)GetValue(TitleTextProperty);
		}
		set
		{
			SetValue(TitleTextProperty, value);
		}
	}

	public Visibility CloseButtonVisibility
	{
		get
		{
			return (Visibility)GetValue(CloseButtonVisibilityProperty);
		}
		set
		{
			SetValue(CloseButtonVisibilityProperty, value);
		}
	}

	public ICommand ClosedCommand
	{
		get
		{
			return (ICommand)GetValue(ClosedCommandProperty);
		}
		set
		{
			SetValue(ClosedCommandProperty, value);
		}
	}

	public bool ClosePopup { get; set; }

	public int Result
	{
		get
		{
			return result;
		}
		set
		{
			result = value;
		}
	}

	public LenovoWindowNew()
	{
		InitializeComponent();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		if (ClosedCommand != null && ClosedCommand.CanExecute(e))
		{
			ClosedCommand.Execute(e);
		}
	}

	private void IconButton_Click(object sender, RoutedEventArgs e)
	{
		if (ClosePopup)
		{
			PopupEx popupEx = LayoutRoot.Template.FindName("pop", LayoutRoot) as PopupEx;
			if (!popupEx.IsOpen)
			{
				popupEx.IsOpen = true;
			}
		}
		else
		{
			Result = -1;
			Close();
		}
	}

	private void CancelClick(object sender, RoutedEventArgs e)
	{
		(LayoutRoot.Template.FindName("pop", LayoutRoot) as PopupEx).IsOpen = false;
		Result = -1;
		Close();
	}

	private void OkClick(object sender, RoutedEventArgs e)
	{
		PopupEx popupEx = LayoutRoot.Template.FindName("pop", LayoutRoot) as PopupEx;
		if (popupEx.IsOpen)
		{
			popupEx.IsOpen = false;
		}
	}
}
