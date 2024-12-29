using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;

namespace lenovo.themes.generic.Behaviors;

public class EllipseBehavior : Behavior<TextBlock>
{
	private MethodInfo m_GetLineDetails = typeof(TextBlock).GetMethod("GetLineDetails", BindingFlags.Instance | BindingFlags.NonPublic);

	private object[] args = new object[5] { 0, 0, 0, 0, 0 };

	public static readonly DependencyProperty AutoShowTooltipWhenEllipsedProperty = DependencyProperty.Register("AutoShowTooltipWhenEllipsed", typeof(bool), typeof(EllipseBehavior), new UIPropertyMetadata(true));

	public bool AutoShowTooltipWhenEllipsed
	{
		get
		{
			return (bool)GetValue(AutoShowTooltipWhenEllipsedProperty);
		}
		set
		{
			SetValue(AutoShowTooltipWhenEllipsedProperty, value);
		}
	}

	protected override void OnAttached()
	{
		base.OnAttached();
		if (AutoShowTooltipWhenEllipsed && base.AssociatedObject.TextTrimming == TextTrimming.None)
		{
			base.AssociatedObject.TextTrimming = TextTrimming.CharacterEllipsis;
		}
		DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock)).AddValueChanged(base.AssociatedObject, OnTextChanged);
		base.AssociatedObject.Loaded += OnLoaded;
	}

	protected override void OnDetaching()
	{
		base.OnDetaching();
		base.AssociatedObject.Loaded -= OnLoaded;
		base.AssociatedObject.SizeChanged -= OnSizeChanged;
		DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock)).RemoveValueChanged(base.AssociatedObject, OnTextChanged);
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs e)
	{
		SetToolTip(null);
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		SetToolTip(null);
		base.AssociatedObject.SizeChanged += OnSizeChanged;
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);
		if (base.AssociatedObject != null && e.Property == AutoShowTooltipWhenEllipsedProperty)
		{
			if ((bool)e.NewValue && base.AssociatedObject.TextTrimming == TextTrimming.None)
			{
				base.AssociatedObject.TextTrimming = TextTrimming.CharacterEllipsis;
			}
			if (base.AssociatedObject.IsLoaded)
			{
				SetToolTip(null);
			}
		}
	}

	private void OnTextChanged(object sender, EventArgs e)
	{
		base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action<object>(SetToolTip), null);
	}

	private void SetToolTip(object obj)
	{
		if (!AutoShowTooltipWhenEllipsed || base.AssociatedObject.TextTrimming == TextTrimming.None)
		{
			return;
		}
		TextBlock textBlock = base.AssociatedObject;
		if (!(textBlock.ActualWidth <= 0.0))
		{
			if (IsTextTrimmed(textBlock))
			{
				textBlock.ToolTip = textBlock.Text;
			}
			else
			{
				textBlock.ToolTip = null;
			}
		}
	}

	private bool IsTextTrimmed(TextBlock textBlock)
	{
		Typeface typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
		return new FormattedText(textBlock.Text, Thread.CurrentThread.CurrentCulture, textBlock.FlowDirection, typeface, textBlock.FontSize, textBlock.Foreground, VisualTreeHelper.GetDpi(textBlock).PixelsPerDip).Width > textBlock.ActualWidth;
	}
}
