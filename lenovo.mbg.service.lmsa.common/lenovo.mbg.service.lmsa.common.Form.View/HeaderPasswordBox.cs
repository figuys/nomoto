using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.common.Form.View;

public partial class HeaderPasswordBox : UserControl, IComponentConnector, IStyleConnector
{
	public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(HeaderPasswordBox), new PropertyMetadata(null));

	public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(HeaderPasswordBox), new PropertyMetadata(null));

	public static readonly DependencyProperty PasswordMaskProperty = DependencyProperty.Register("PasswordMask", typeof(string), typeof(HeaderPasswordBox), new PropertyMetadata("●●●●●●●●"));

	public static readonly DependencyProperty IsShowPasswordMaskProperty = DependencyProperty.Register("IsShowPasswordMask", typeof(bool), typeof(HeaderPasswordBox), new PropertyMetadata(false, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs = e;
		HeaderPasswordBox headerPasswordBox = d as HeaderPasswordBox;
		if (dependencyPropertyChangedEventArgs.NewValue != null && (bool)dependencyPropertyChangedEventArgs.NewValue)
		{
			headerPasswordBox.ContentHostVisibility = Visibility.Hidden;
			headerPasswordBox.PasswordMaskBorderVisibility = Visibility.Visible;
		}
		else
		{
			headerPasswordBox.ContentHostVisibility = Visibility.Visible;
			headerPasswordBox.PasswordMaskBorderVisibility = Visibility.Hidden;
		}
	}));

	public static readonly DependencyProperty ContentHostVisibilityProperty = DependencyProperty.Register("ContentHostVisibility", typeof(Visibility), typeof(HeaderPasswordBox), new PropertyMetadata(Visibility.Visible));

	public static readonly DependencyProperty PasswordMaskBorderVisibilityProperty = DependencyProperty.Register("PasswordMaskBorderVisibility", typeof(Visibility), typeof(HeaderPasswordBox), new PropertyMetadata(Visibility.Hidden));

	public static readonly DependencyProperty EmptyValTipProperty = DependencyProperty.Register("EmptyValTip", typeof(object), typeof(HeaderPasswordBox), new PropertyMetadata(null));

	public object Header
	{
		get
		{
			return GetValue(HeaderProperty);
		}
		set
		{
			SetValue(HeaderProperty, value);
		}
	}

	public string Password
	{
		get
		{
			return (string)GetValue(PasswordProperty);
		}
		set
		{
			SetValue(PasswordProperty, value);
		}
	}

	public string PasswordMask
	{
		get
		{
			return (string)GetValue(PasswordMaskProperty);
		}
		set
		{
			SetValue(PasswordMaskProperty, value);
		}
	}

	public bool IsShowPasswordMask
	{
		get
		{
			return (bool)GetValue(IsShowPasswordMaskProperty);
		}
		set
		{
			SetValue(IsShowPasswordMaskProperty, value);
		}
	}

	public Visibility ContentHostVisibility
	{
		get
		{
			return (Visibility)GetValue(ContentHostVisibilityProperty);
		}
		set
		{
			SetValue(ContentHostVisibilityProperty, value);
		}
	}

	public Visibility PasswordMaskBorderVisibility
	{
		get
		{
			return (Visibility)GetValue(PasswordMaskBorderVisibilityProperty);
		}
		set
		{
			SetValue(PasswordMaskBorderVisibilityProperty, value);
		}
	}

	public object TipWhenTextIsEmpty
	{
		get
		{
			return GetValue(EmptyValTipProperty);
		}
		set
		{
			SetValue(EmptyValTipProperty, value);
		}
	}

	public HeaderPasswordBox()
	{
		InitializeComponent();
	}

	private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
	{
		PasswordBox passwordBox = e.Source as PasswordBox;
		Password = passwordBox.Password;
	}

	private void PasswordMaskBorderTextBox_GotFocus(object sender, RoutedEventArgs e)
	{
		ContentHostVisibility = Visibility.Visible;
		PasswordMaskBorderVisibility = Visibility.Hidden;
		Password = string.Empty;
	}
}
