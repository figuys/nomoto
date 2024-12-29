using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace lenovo.themes.generic.AttachedProperty;

public class Adorners
{
	public static readonly DependencyProperty TemplateProperty = DependencyProperty.RegisterAttached("Template", typeof(ControlTemplate), typeof(Adorners), new PropertyMetadata(TemplateChanged));

	public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(Adorners), new PropertyMetadata(IsVisibleChanged));

	public static readonly DependencyProperty InternalAdornerProperty = DependencyProperty.RegisterAttached("InternalAdorner", typeof(ControlAdorner), typeof(Adorners));

	public static Dispatcher CurrentDispatcher { get; set; }

	public static ControlTemplate GetTemplate(UIElement target)
	{
		return (ControlTemplate)target.GetValue(TemplateProperty);
	}

	public static void SetTemplate(UIElement target, ControlTemplate value)
	{
		target.SetValue(TemplateProperty, value);
	}

	private static void TemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		UpdateAdorner((UIElement)d, GetIsVisible((UIElement)d), (ControlTemplate)e.NewValue);
	}

	public static bool GetIsVisible(UIElement target)
	{
		return (bool)target.GetValue(IsVisibleProperty);
	}

	public static void SetIsVisible(UIElement target, bool value)
	{
		target.SetValue(IsVisibleProperty, value);
	}

	private static void IsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		UpdateAdorner((UIElement)d, (bool)e.NewValue, GetTemplate((UIElement)d));
	}

	public static ControlAdorner GetInternalAdorner(DependencyObject target)
	{
		return (ControlAdorner)target.GetValue(InternalAdornerProperty);
	}

	public static void SetInternalAdorner(DependencyObject target, ControlAdorner value)
	{
		target.SetValue(InternalAdornerProperty, value);
	}

	private static void UpdateAdorner(UIElement adorned)
	{
		UpdateAdorner(adorned, GetIsVisible(adorned), GetTemplate(adorned));
	}

	public static void SetDispatcher(Dispatcher dispatcher)
	{
		CurrentDispatcher = dispatcher;
	}

	private static void UpdateAdorner(UIElement adorned, bool isVisible, ControlTemplate controlTemplate)
	{
		AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adorned);
		if (adornerLayer == null)
		{
			if (CurrentDispatcher != null)
			{
				CurrentDispatcher.InvokeAsync(delegate
				{
					UpdateAdorner(adorned);
				}, DispatcherPriority.Background);
			}
			else
			{
				Dispatcher.CurrentDispatcher.InvokeAsync(delegate
				{
					UpdateAdorner(adorned);
				}, DispatcherPriority.Background);
			}
			return;
		}
		ControlAdorner internalAdorner = GetInternalAdorner(adorned);
		if (internalAdorner == null)
		{
			if (controlTemplate != null && isVisible)
			{
				ControlAdorner controlAdorner = new ControlAdorner(adorned);
				controlAdorner.Child = new Control
				{
					Template = controlTemplate,
					Focusable = false
				};
				adornerLayer.Add(controlAdorner);
				SetInternalAdorner(adorned, controlAdorner);
			}
		}
		else if (controlTemplate != null && isVisible)
		{
			internalAdorner.Child.Template = controlTemplate;
		}
		else
		{
			internalAdorner.Child = null;
			adornerLayer.Remove(internalAdorner);
			SetInternalAdorner(adorned, null);
		}
	}
}
