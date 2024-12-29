using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using lenovo.mbg.service.framework.lang;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public static class ControlAttachProperty
{
	public static readonly DependencyProperty FocusBackgroundProperty = DependencyProperty.RegisterAttached("FocusBackground", typeof(Brush), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty FocusForegroundProperty = DependencyProperty.RegisterAttached("FocusForeground", typeof(Brush), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.RegisterAttached("MouseOverBackground", typeof(Brush), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.RegisterAttached("MouseOverForeground", typeof(Brush), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty FocusBorderBrushProperty = DependencyProperty.RegisterAttached("FocusBorderBrush", typeof(Brush), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty MouseOverBorderBrushProperty = DependencyProperty.RegisterAttached("MouseOverBorderBrush", typeof(Brush), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

	public static readonly DependencyProperty AttachContentProperty = DependencyProperty.RegisterAttached("AttachContent", typeof(ControlTemplate), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(""));

	public static readonly DependencyProperty FIconProperty = DependencyProperty.RegisterAttached("FIcon", typeof(string), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(""));

	public static readonly DependencyProperty FIconSizeProperty = DependencyProperty.RegisterAttached("FIconSize", typeof(double), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(12.0));

	public static readonly DependencyProperty FIconMarginProperty = DependencyProperty.RegisterAttached("FIconMargin", typeof(Thickness), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty AllowsAnimationProperty = DependencyProperty.RegisterAttached("AllowsAnimation", typeof(bool), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(false, AllowsAnimationChanged));

	private static DoubleAnimation RotateAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromMilliseconds(200.0)));

	public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached("Label", typeof(string), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty LabelTemplateProperty = DependencyProperty.RegisterAttached("LabelTemplate", typeof(ControlTemplate), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached("ImageSource", typeof(ImageSource), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty ImageTemplateProperty = DependencyProperty.RegisterAttached("ImageTemplate", typeof(ControlTemplate), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty LeftClickCommandProperty = DependencyProperty.RegisterAttached("LeftClickCommand", typeof(ICommand), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

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

	public static void SetMouseOverBackground(DependencyObject element, Brush value)
	{
		element.SetValue(MouseOverBackgroundProperty, value);
	}

	public static Brush GetMouseOverBackground(DependencyObject element)
	{
		return (Brush)element.GetValue(FocusBackgroundProperty);
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

	[AttachedPropertyBrowsableForType(typeof(TextBox))]
	[AttachedPropertyBrowsableForType(typeof(CheckBox))]
	[AttachedPropertyBrowsableForType(typeof(RadioButton))]
	[AttachedPropertyBrowsableForType(typeof(DatePicker))]
	[AttachedPropertyBrowsableForType(typeof(ComboBox))]
	[AttachedPropertyBrowsableForType(typeof(RichTextBox))]
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
		return LangTranslation.Translate((string)d.GetValue(WatermarkProperty));
	}

	public static void SetWatermark(DependencyObject obj, string value)
	{
		obj.SetValue(WatermarkProperty, value);
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

	[AttachedPropertyBrowsableForType(typeof(TextBox))]
	public static string GetLabel(DependencyObject d)
	{
		return (string)d.GetValue(LabelProperty);
	}

	public static void SetLabel(DependencyObject obj, string value)
	{
		obj.SetValue(LabelProperty, value);
	}

	[AttachedPropertyBrowsableForType(typeof(TextBox))]
	public static ControlTemplate GetLabelTemplate(DependencyObject d)
	{
		return (ControlTemplate)d.GetValue(LabelTemplateProperty);
	}

	public static void SetLabelTemplate(DependencyObject obj, ControlTemplate value)
	{
		obj.SetValue(LabelTemplateProperty, value);
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

	public static ICommand GetLeftClickCommand(DependencyObject d)
	{
		return (ICommand)d.GetValue(LeftClickCommandProperty);
	}

	public static void SetLeftClickCommand(DependencyObject obj, ICommand value)
	{
		obj.SetValue(LeftClickCommandProperty, value);
	}
}
