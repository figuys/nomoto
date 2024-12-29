using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Win32;

namespace lenovo.themes.generic.ControlsV6;

public static class ControlAttachPropertyV6
{
	public static readonly DependencyProperty IsRequiredProperty;

	public static readonly DependencyProperty FocusBackgroundProperty;

	public static readonly DependencyProperty FocusForegroundProperty;

	public static readonly DependencyProperty MouseOverBackgroundProperty;

	public static readonly DependencyProperty MouseOverForegroundProperty;

	public static readonly DependencyProperty FocusBorderBrushProperty;

	public static readonly DependencyProperty MouseOverBorderBrushProperty;

	public static readonly DependencyProperty AttachContentProperty;

	public static readonly DependencyProperty WatermarkProperty;

	public static readonly DependencyProperty CommVisibilityProperty;

	public static readonly DependencyProperty ToggleButtonVisibilityProperty;

	public static readonly DependencyProperty FIconProperty;

	public static readonly DependencyProperty FIconSizeProperty;

	public static readonly DependencyProperty FIconMarginProperty;

	public static readonly DependencyProperty AllowsAnimationProperty;

	private static DoubleAnimation RotateAnimation;

	public static readonly DependencyProperty CornerRadiusProperty;

	public static readonly DependencyProperty LabelProperty;

	public static readonly DependencyProperty LabelTemplateProperty;

	public static readonly DependencyProperty DisabledImageSourceProperty;

	public static readonly DependencyProperty MouseOverImageSourceProperty;

	public static readonly DependencyProperty ImageSourceProperty;

	public static readonly DependencyProperty ImageTemplateProperty;

	public static readonly DependencyProperty TailImageTemplateProperty;

	public static readonly DependencyProperty CusCommandProperty;

	private static readonly DependencyProperty PasswordInitializedProperty;

	private static readonly DependencyProperty SettingPasswordProperty;

	public static readonly DependencyProperty PasswordProperty;

	public static readonly DependencyProperty IsClearTextButtonBehaviorEnabledProperty;

	public static readonly DependencyProperty IsOpenFileButtonBehaviorEnabledProperty;

	public static readonly DependencyProperty IsOpenFolderButtonBehaviorEnabledProperty;

	public static readonly DependencyProperty IsSaveFileButtonBehaviorEnabledProperty;

	private static readonly CommandBinding ClearTextCommandBinding;

	private static readonly CommandBinding OpenFileCommandBinding;

	private static readonly CommandBinding OpenFolderCommandBinding;

	private static readonly CommandBinding SaveFileCommandBinding;

	public static RoutedUICommand ClearTextCommand { get; private set; }

	public static RoutedUICommand OpenFileCommand { get; private set; }

	public static RoutedUICommand OpenFolderCommand { get; private set; }

	public static RoutedUICommand SaveFileCommand { get; private set; }

	public static bool GetIsRequired(DependencyObject element)
	{
		return (bool)element.GetValue(IsRequiredProperty);
	}

	public static void SetIsRequired(DependencyObject element, bool value)
	{
		element.SetValue(IsRequiredProperty, value);
	}

	public static void SetFocusBackground(DependencyObject element, Brush value)
	{
		element.SetValue(FocusBackgroundProperty, value);
	}

	public static Brush GetFocusBackground(DependencyObject element)
	{
		return (Brush)element.GetValue(FocusBackgroundProperty);
	}

	public static void SetFocusForeground(DependencyObject element, Brush value)
	{
		element.SetValue(FocusForegroundProperty, value);
	}

	public static Brush GetFocusForeground(DependencyObject element)
	{
		return (Brush)element.GetValue(FocusForegroundProperty);
	}

	public static Brush GetMouseOverBackground(UIElement target)
	{
		return (Brush)target.GetValue(MouseOverBackgroundProperty);
	}

	public static void SetMouseOverBackground(UIElement target, Brush value)
	{
		target.SetValue(MouseOverBackgroundProperty, value);
	}

	public static void SetMouseOverForeground(DependencyObject element, Brush value)
	{
		element.SetValue(MouseOverForegroundProperty, value);
	}

	public static Brush GetMouseOverForeground(DependencyObject element)
	{
		return (Brush)element.GetValue(FocusForegroundProperty);
	}

	public static void SetFocusBorderBrush(DependencyObject element, Brush value)
	{
		element.SetValue(FocusBorderBrushProperty, value);
	}

	public static Brush GetFocusBorderBrush(DependencyObject element)
	{
		return (Brush)element.GetValue(FocusBorderBrushProperty);
	}

	public static void SetMouseOverBorderBrush(DependencyObject obj, Brush value)
	{
		obj.SetValue(MouseOverBorderBrushProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.CheckBox))]
	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.RadioButton))]
	[AttachedPropertyBrowsableForType(typeof(DatePicker))]
	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.ComboBox))]
	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.RichTextBox))]
	public static Brush GetMouseOverBorderBrush(DependencyObject obj)
	{
		return (Brush)obj.GetValue(MouseOverBorderBrushProperty);
	}

	public static ControlTemplate GetAttachContent(DependencyObject d)
	{
		return (ControlTemplate)d.GetValue(AttachContentProperty);
	}

	public static void SetAttachContent(DependencyObject obj, ControlTemplate value)
	{
		obj.SetValue(AttachContentProperty, value);
	}

	public static string GetWatermark(DependencyObject d)
	{
		return (string)d.GetValue(WatermarkProperty);
	}

	public static void SetWatermark(DependencyObject obj, string value)
	{
		obj.SetValue(WatermarkProperty, value);
	}

	public static Visibility GetCommVisibility(DependencyObject d)
	{
		return (Visibility)d.GetValue(CommVisibilityProperty);
	}

	public static void SetCommVisibility(DependencyObject obj, Visibility value)
	{
		obj.SetValue(CommVisibilityProperty, value);
	}

	public static Visibility GetToggleButtonVisibility(DependencyObject d)
	{
		return (Visibility)d.GetValue(ToggleButtonVisibilityProperty);
	}

	public static void SetToggleButtonVisibility(DependencyObject obj, Visibility value)
	{
		obj.SetValue(ToggleButtonVisibilityProperty, value);
	}

	public static string GetFIcon(DependencyObject d)
	{
		return (string)d.GetValue(FIconProperty);
	}

	public static void SetFIcon(DependencyObject obj, string value)
	{
		obj.SetValue(FIconProperty, value);
	}

	public static double GetFIconSize(DependencyObject d)
	{
		return (double)d.GetValue(FIconSizeProperty);
	}

	public static void SetFIconSize(DependencyObject obj, double value)
	{
		obj.SetValue(FIconSizeProperty, value);
	}

	public static Thickness GetFIconMargin(DependencyObject d)
	{
		return (Thickness)d.GetValue(FIconMarginProperty);
	}

	public static void SetFIconMargin(DependencyObject obj, Thickness value)
	{
		obj.SetValue(FIconMarginProperty, value);
	}

	public static bool GetAllowsAnimation(DependencyObject d)
	{
		return (bool)d.GetValue(AllowsAnimationProperty);
	}

	public static void SetAllowsAnimation(DependencyObject obj, bool value)
	{
		obj.SetValue(AllowsAnimationProperty, value);
	}

	private static void AllowsAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is FrameworkElement frameworkElement)
		{
			if (frameworkElement.RenderTransformOrigin == new Point(0.0, 0.0))
			{
				frameworkElement.RenderTransformOrigin = new Point(0.5, 0.5);
				RotateTransform renderTransform = new RotateTransform(0.0);
				frameworkElement.RenderTransform = renderTransform;
			}
			if ((bool)e.NewValue)
			{
				RotateAnimation.To = 180.0;
				frameworkElement.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
			}
			else
			{
				RotateAnimation.To = 0.0;
				frameworkElement.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
			}
		}
	}

	public static CornerRadius GetCornerRadius(DependencyObject d)
	{
		return (CornerRadius)d.GetValue(CornerRadiusProperty);
	}

	public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
	{
		obj.SetValue(CornerRadiusProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
	public static string GetLabel(DependencyObject d)
	{
		return (string)d.GetValue(LabelProperty);
	}

	public static void SetLabel(DependencyObject obj, string value)
	{
		obj.SetValue(LabelProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
	public static ControlTemplate GetLabelTemplate(DependencyObject d)
	{
		return (ControlTemplate)d.GetValue(LabelTemplateProperty);
	}

	public static void SetLabelTemplate(DependencyObject obj, ControlTemplate value)
	{
		obj.SetValue(LabelTemplateProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static ImageSource GetDisabledImageSource(DependencyObject d)
	{
		return (ImageSource)d.GetValue(DisabledImageSourceProperty);
	}

	public static void SetDisabledImageSource(DependencyObject obj, ImageSource value)
	{
		obj.SetValue(DisabledImageSourceProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static ImageSource GetMouseOverImageSource(DependencyObject d)
	{
		return (ImageSource)d.GetValue(MouseOverImageSourceProperty);
	}

	public static void SetMouseOverImageSource(DependencyObject obj, ImageSource value)
	{
		obj.SetValue(MouseOverImageSourceProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static ImageSource GetImageSource(DependencyObject d)
	{
		return (ImageSource)d.GetValue(ImageSourceProperty);
	}

	public static void SetImageSource(DependencyObject obj, ImageSource value)
	{
		obj.SetValue(ImageSourceProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static ControlTemplate GetImageTemplate(DependencyObject d)
	{
		return (ControlTemplate)d.GetValue(ImageTemplateProperty);
	}

	public static void SetImageTemplate(DependencyObject obj, ControlTemplate value)
	{
		obj.SetValue(ImageTemplateProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static ControlTemplate GetTailImageTemplate(DependencyObject d)
	{
		return (ControlTemplate)d.GetValue(TailImageTemplateProperty);
	}

	public static void SetTailImageTemplate(DependencyObject obj, ControlTemplate value)
	{
		obj.SetValue(TailImageTemplateProperty, value);
	}

	public static ICommand GetCusCommand(DependencyObject d)
	{
		return (ICommand)d.GetValue(CusCommandProperty);
	}

	public static void SetCusCommand(DependencyObject obj, ICommand value)
	{
		obj.SetValue(WatermarkProperty, value);
	}

	public static string GetPassword(DependencyObject obj)
	{
		return (string)obj.GetValue(PasswordProperty);
	}

	public static void SetPassword(DependencyObject obj, string value)
	{
		obj.SetValue(PasswordProperty, value);
	}

	private static void HandleBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
	{
		if (dp is PasswordBox passwordBox && !(bool)passwordBox.GetValue(SettingPasswordProperty))
		{
			if (!(bool)passwordBox.GetValue(PasswordInitializedProperty))
			{
				passwordBox.SetValue(PasswordInitializedProperty, true);
				passwordBox.PasswordChanged += HandlePasswordChanged;
			}
			passwordBox.Password = e.NewValue as string;
		}
	}

	private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
	{
		PasswordBox passwordBox = (PasswordBox)sender;
		passwordBox.SetValue(SettingPasswordProperty, true);
		SetPassword(passwordBox, passwordBox.Password);
		passwordBox.SetValue(SettingPasswordProperty, false);
		SetPasswordBoxSelection(passwordBox, passwordBox.Password.Length + 1, passwordBox.Password.Length + 1);
	}

	private static void SetPasswordBoxSelection(PasswordBox passwordBox, int start, int length)
	{
		passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[2] { start, length });
	}

	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
	public static bool GetIsClearTextButtonBehaviorEnabled(DependencyObject d)
	{
		return (bool)d.GetValue(IsClearTextButtonBehaviorEnabledProperty);
	}

	public static void SetIsClearTextButtonBehaviorEnabled(DependencyObject obj, bool value)
	{
		obj.SetValue(IsClearTextButtonBehaviorEnabledProperty, value);
	}

	private static void IsClearTextButtonBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		System.Windows.Controls.Button button = d as System.Windows.Controls.Button;
		if (e.OldValue != e.NewValue)
		{
			button?.CommandBindings.Add(ClearTextCommandBinding);
		}
	}

	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
	public static bool GetIsOpenFileButtonBehaviorEnabled(DependencyObject d)
	{
		return (bool)d.GetValue(IsOpenFileButtonBehaviorEnabledProperty);
	}

	public static void SetIsOpenFileButtonBehaviorEnabled(DependencyObject obj, bool value)
	{
		obj.SetValue(IsOpenFileButtonBehaviorEnabledProperty, value);
	}

	private static void IsOpenFileButtonBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		System.Windows.Controls.Button button = d as System.Windows.Controls.Button;
		if (e.OldValue != e.NewValue)
		{
			button?.CommandBindings.Add(OpenFileCommandBinding);
		}
	}

	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
	public static bool GetIsOpenFolderButtonBehaviorEnabled(DependencyObject d)
	{
		return (bool)d.GetValue(IsOpenFolderButtonBehaviorEnabledProperty);
	}

	public static void SetIsOpenFolderButtonBehaviorEnabled(DependencyObject obj, bool value)
	{
		obj.SetValue(IsOpenFolderButtonBehaviorEnabledProperty, value);
	}

	private static void IsOpenFolderButtonBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		System.Windows.Controls.Button button = d as System.Windows.Controls.Button;
		if (e.OldValue != e.NewValue)
		{
			button?.CommandBindings.Add(OpenFolderCommandBinding);
		}
	}

	[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
	public static bool GetIsSaveFileButtonBehaviorEnabled(DependencyObject d)
	{
		return (bool)d.GetValue(IsSaveFileButtonBehaviorEnabledProperty);
	}

	public static void SetIsSaveFileButtonBehaviorEnabled(DependencyObject obj, bool value)
	{
		obj.SetValue(IsSaveFileButtonBehaviorEnabledProperty, value);
	}

	private static void IsSaveFileButtonBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		System.Windows.Controls.Button button = d as System.Windows.Controls.Button;
		if (e.OldValue != e.NewValue)
		{
			button?.CommandBindings.Add(SaveFileCommandBinding);
		}
	}

	private static void ClearButtonClick(object sender, ExecutedRoutedEventArgs e)
	{
		if (e.Parameter is FrameworkElement frameworkElement)
		{
			if (frameworkElement is System.Windows.Controls.TextBox)
			{
				((System.Windows.Controls.TextBox)frameworkElement).Clear();
			}
			if (frameworkElement is PasswordBox)
			{
				((PasswordBox)frameworkElement).Clear();
			}
			if (frameworkElement is System.Windows.Controls.ComboBox)
			{
				System.Windows.Controls.ComboBox obj = frameworkElement as System.Windows.Controls.ComboBox;
				obj.SelectedItem = null;
				obj.Text = string.Empty;
			}
			if (frameworkElement is DatePicker)
			{
				DatePicker obj2 = frameworkElement as DatePicker;
				obj2.SelectedDate = null;
				obj2.Text = string.Empty;
			}
			frameworkElement.Focus();
		}
	}

	private static void OpenFileButtonClick(object sender, ExecutedRoutedEventArgs e)
	{
		FrameworkElement frameworkElement = e.Parameter as FrameworkElement;
		System.Windows.Controls.TextBox textBox = frameworkElement as System.Windows.Controls.TextBox;
		string text = ((textBox.Tag == null) ? "所有文件(*.*)|*.*" : textBox.Tag.ToString());
		if (text.Contains(".bin"))
		{
			text += "|所有文件(*.*)|*.*";
		}
		if (textBox != null)
		{
			Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
			openFileDialog.Title = "请选择文件";
			openFileDialog.Filter = text;
			openFileDialog.FileName = textBox.Text.Trim();
			if (openFileDialog.ShowDialog() == true)
			{
				textBox.Text = openFileDialog.FileName;
			}
			frameworkElement.Focus();
		}
	}

	private static void OpenFolderButtonClick(object sender, ExecutedRoutedEventArgs e)
	{
		FrameworkElement frameworkElement = e.Parameter as FrameworkElement;
		if (frameworkElement is System.Windows.Controls.TextBox textBox)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.Description = "请选择文件路径";
			folderBrowserDialog.SelectedPath = textBox.Text.Trim();
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				textBox.Text = folderBrowserDialog.SelectedPath;
			}
			frameworkElement.Focus();
		}
	}

	private static void SaveFileButtonClick(object sender, ExecutedRoutedEventArgs e)
	{
		FrameworkElement frameworkElement = e.Parameter as FrameworkElement;
		if (frameworkElement is System.Windows.Controls.TextBox textBox)
		{
			System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			saveFileDialog.Title = "文件保存路径";
			saveFileDialog.Filter = "所有文件(*.*)|*.*";
			saveFileDialog.FileName = textBox.Text.Trim();
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				textBox.Text = saveFileDialog.FileName;
			}
			frameworkElement.Focus();
		}
	}

	static ControlAttachPropertyV6()
	{
		IsRequiredProperty = DependencyProperty.RegisterAttached("IsRequired", typeof(bool), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(false));
		FocusBackgroundProperty = DependencyProperty.RegisterAttached("FocusBackground", typeof(Brush), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		FocusForegroundProperty = DependencyProperty.RegisterAttached("FocusForeground", typeof(Brush), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		MouseOverBackgroundProperty = DependencyProperty.RegisterAttached("MouseOverBackground", typeof(Brush), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		MouseOverForegroundProperty = DependencyProperty.RegisterAttached("MouseOverForeground", typeof(Brush), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		FocusBorderBrushProperty = DependencyProperty.RegisterAttached("FocusBorderBrush", typeof(Brush), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		MouseOverBorderBrushProperty = DependencyProperty.RegisterAttached("MouseOverBorderBrush", typeof(Brush), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
		AttachContentProperty = DependencyProperty.RegisterAttached("AttachContent", typeof(ControlTemplate), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(""));
		CommVisibilityProperty = DependencyProperty.RegisterAttached("CommVisibility", typeof(Visibility), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(Visibility.Collapsed));
		ToggleButtonVisibilityProperty = DependencyProperty.RegisterAttached("ToggleButtonVisibility", typeof(Visibility), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(Visibility.Visible));
		FIconProperty = DependencyProperty.RegisterAttached("FIcon", typeof(string), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(""));
		FIconSizeProperty = DependencyProperty.RegisterAttached("FIconSize", typeof(double), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(12.0));
		FIconMarginProperty = DependencyProperty.RegisterAttached("FIconMargin", typeof(Thickness), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		AllowsAnimationProperty = DependencyProperty.RegisterAttached("AllowsAnimation", typeof(bool), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(false, AllowsAnimationChanged));
		RotateAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromMilliseconds(200.0)));
		CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		LabelProperty = DependencyProperty.RegisterAttached("Label", typeof(string), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		LabelTemplateProperty = DependencyProperty.RegisterAttached("LabelTemplate", typeof(ControlTemplate), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		DisabledImageSourceProperty = DependencyProperty.RegisterAttached("DisabledImageSource", typeof(ImageSource), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		MouseOverImageSourceProperty = DependencyProperty.RegisterAttached("MouseOverImageSource", typeof(ImageSource), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		ImageSourceProperty = DependencyProperty.RegisterAttached("ImageSource", typeof(ImageSource), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		ImageTemplateProperty = DependencyProperty.RegisterAttached("ImageTemplate", typeof(ControlTemplate), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		TailImageTemplateProperty = DependencyProperty.RegisterAttached("TailImageTemplate", typeof(ControlTemplate), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		CusCommandProperty = DependencyProperty.RegisterAttached("CusCommand", typeof(ICommand), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(null));
		PasswordInitializedProperty = DependencyProperty.RegisterAttached("PasswordInitialized", typeof(bool), typeof(ControlAttachPropertyV6), new PropertyMetadata(false));
		SettingPasswordProperty = DependencyProperty.RegisterAttached("SettingPassword", typeof(bool), typeof(ControlAttachPropertyV6), new PropertyMetadata(false));
		PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(Guid.NewGuid().ToString(), HandleBoundPasswordChanged)
		{
			BindsTwoWayByDefault = true,
			DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus
		});
		IsClearTextButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsClearTextButtonBehaviorEnabled", typeof(bool), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(false, IsClearTextButtonBehaviorEnabledChanged));
		IsOpenFileButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsOpenFileButtonBehaviorEnabled", typeof(bool), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(false, IsOpenFileButtonBehaviorEnabledChanged));
		IsOpenFolderButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsOpenFolderButtonBehaviorEnabled", typeof(bool), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(false, IsOpenFolderButtonBehaviorEnabledChanged));
		IsSaveFileButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsSaveFileButtonBehaviorEnabled", typeof(bool), typeof(ControlAttachPropertyV6), new FrameworkPropertyMetadata(false, IsSaveFileButtonBehaviorEnabledChanged));
		ClearTextCommand = new RoutedUICommand();
		ClearTextCommandBinding = new CommandBinding(ClearTextCommand);
		ClearTextCommandBinding.Executed += ClearButtonClick;
		OpenFileCommand = new RoutedUICommand();
		OpenFileCommandBinding = new CommandBinding(OpenFileCommand);
		OpenFileCommandBinding.Executed += OpenFileButtonClick;
		OpenFolderCommand = new RoutedUICommand();
		OpenFolderCommandBinding = new CommandBinding(OpenFolderCommand);
		OpenFolderCommandBinding.Executed += OpenFolderButtonClick;
		SaveFileCommand = new RoutedUICommand();
		SaveFileCommandBinding = new CommandBinding(SaveFileCommand);
		SaveFileCommandBinding.Executed += SaveFileButtonClick;
	}
}
