using System;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;

namespace lenovo.themes.generic.Behaviors;

public class ScrollIntoViewBehavior : Behavior<ListBox>
{
	protected override void OnAttached()
	{
		base.OnAttached();
		base.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
	}

	private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ListBox ctrl = sender as ListBox;
		if (ctrl != null && ctrl.SelectedItem != null)
		{
			ctrl.Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)delegate
			{
				ctrl.UpdateLayout();
				ctrl.ScrollIntoView(ctrl.SelectedItem);
			});
		}
	}

	protected override void OnDetaching()
	{
		base.OnDetaching();
		base.AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
	}
}
