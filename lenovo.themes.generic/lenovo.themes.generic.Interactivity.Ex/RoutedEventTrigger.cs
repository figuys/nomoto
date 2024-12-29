using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace lenovo.themes.generic.Interactivity.Ex;

public class RoutedEventTrigger : EventTriggerBase<DependencyObject>
{
	private RoutedEvent routedEvent;

	public RoutedEvent RoutedEvent
	{
		get
		{
			return routedEvent;
		}
		set
		{
			routedEvent = value;
		}
	}

	protected override void OnAttached()
	{
		Behavior behavior = base.AssociatedObject as Behavior;
		FrameworkElement frameworkElement = base.AssociatedObject as FrameworkElement;
		if (behavior != null)
		{
			frameworkElement = ((IAttachedObject)behavior).AssociatedObject as FrameworkElement;
		}
		if (frameworkElement == null)
		{
			throw new ArgumentException("Routed Event trigger can only be associated to framework elements");
		}
		if (RoutedEvent != null)
		{
			frameworkElement.AddHandler(RoutedEvent, new RoutedEventHandler(OnRoutedEvent));
		}
	}

	private void OnRoutedEvent(object sender, RoutedEventArgs args)
	{
		base.OnEvent(args);
	}

	protected override string GetEventName()
	{
		return RoutedEvent.Name;
	}
}
