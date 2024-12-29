using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace lenovo.mbg.service.lmsa.phoneManager.Common;

public static class CustomeTabControlBehavior
{
	public static readonly DependencyProperty IsOverrideSelectionChangedProperty = DependencyProperty.RegisterAttached("IsOverrideSelectionChanged", typeof(bool), typeof(CustomeTabControlBehavior), new FrameworkPropertyMetadata(false, FireIsOverrideSelectionChanged));

	public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.RegisterAttached("SelectionChangedCommand", typeof(ICommand), typeof(CustomeTabControlBehavior), new PropertyMetadata(null));

	[AttachedPropertyBrowsableForType(typeof(TabControl))]
	public static bool GetIsOverrideSelectionChanged(DependencyObject d)
	{
		return (bool)d.GetValue(IsOverrideSelectionChangedProperty);
	}

	public static void SetIsOverrideSelectionChanged(DependencyObject obj, bool value)
	{
		obj.SetValue(IsOverrideSelectionChangedProperty, value);
	}

	private static void FireIsOverrideSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		TabControl tabControl = d as TabControl;
		if ((bool)e.NewValue)
		{
			tabControl.SelectionChanged += FireSelectionChangedEventHandler;
		}
		else
		{
			tabControl.SelectionChanged -= FireSelectionChangedEventHandler;
		}
	}

	[AttachedPropertyBrowsableForType(typeof(TabControl))]
	public static ICommand GetSelectionChangedCommand(DependencyObject d)
	{
		return (ICommand)d.GetValue(SelectionChangedCommandProperty);
	}

	public static void SetSelectionChangedCommand(DependencyObject obj, ICommand value)
	{
		obj.SetValue(SelectionChangedCommandProperty, value);
	}

	private static void FireSelectionChangedEventHandler(object sender, SelectionChangedEventArgs e)
	{
		e.Handled = true;
		ICommand selectionChangedCommand = GetSelectionChangedCommand((DependencyObject)sender);
		TabControl tabControl = sender as TabControl;
		selectionChangedCommand.Execute(tabControl.SelectedItem);
	}
}
