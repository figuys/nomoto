using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.themes.generic.Controls;

public static class RsaControlAttachProperty
{
	public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.RegisterAttached("IsRequired", typeof(bool), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(false));

	public static readonly DependencyProperty FocusBackgroundProperty = DependencyProperty.RegisterAttached("FocusBackground", typeof(Brush), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty FocusForegroundProperty = DependencyProperty.RegisterAttached("FocusForeground", typeof(Brush), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.RegisterAttached("MouseOverBackground", typeof(Brush), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.RegisterAttached("MouseOverForeground", typeof(Brush), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty FocusBorderBrushProperty = DependencyProperty.RegisterAttached("FocusBorderBrush", typeof(Brush), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty MouseOverBorderBrushProperty = DependencyProperty.RegisterAttached("MouseOverBorderBrush", typeof(Brush), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

	public static readonly DependencyProperty AttachContentProperty = DependencyProperty.RegisterAttached("AttachContent", typeof(ControlTemplate), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(""));

	public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached("Label", typeof(string), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(null));

	public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached("ImageSource", typeof(ImageSource), typeof(RsaControlAttachProperty), new FrameworkPropertyMetadata(null));

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

	public static void SetMouseOverBackground(DependencyObject element, Brush value)
	{
		element.SetValue(MouseOverBackgroundProperty, value);
	}

	public static Brush MouseOverBackground(DependencyObject element)
	{
		return (Brush)element.GetValue(FocusBackgroundProperty);
	}

	public static void SetMouseOverForeground(DependencyObject element, Brush value)
	{
		element.SetValue(MouseOverForegroundProperty, value);
	}

	public static Brush MouseOverForeground(DependencyObject element)
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
		return (string)d.GetValue(WatermarkProperty);
	}

	public static void SetWatermark(DependencyObject obj, string value)
	{
		obj.SetValue(WatermarkProperty, value);
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

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static ImageSource GetImageSource(DependencyObject d)
	{
		return (ImageSource)d.GetValue(ImageSourceProperty);
	}

	public static void SetImageSource(DependencyObject obj, ImageSource value)
	{
		obj.SetValue(ImageSourceProperty, value);
	}
}
