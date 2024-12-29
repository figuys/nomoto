using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace lenovo.mbg.service.lmsa.phoneManager.Common;

public class InteractionsSelectorDoubleClickCommandAction : TargetedTriggerAction<FrameworkElement>, ICommandSource
{
	public static readonly DependencyProperty TheCommandToRunProperty = DependencyProperty.RegisterAttached("TheCommandToRun", typeof(ICommand), typeof(InteractionsSelectorDoubleClickCommandAction), new FrameworkPropertyMetadata((object)null));

	public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InteractionsSelectorDoubleClickCommandAction), new PropertyMetadata(null, OnCommandChanged));

	private EventHandler CanExecuteChangedHandler;

	public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InteractionsSelectorDoubleClickCommandAction), new PropertyMetadata());

	public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(InteractionsSelectorDoubleClickCommandAction), new PropertyMetadata());

	public static readonly DependencyProperty SyncOwnerIsEnabledProperty = DependencyProperty.Register("SyncOwnerIsEnabled", typeof(bool), typeof(InteractionsSelectorDoubleClickCommandAction), new PropertyMetadata());

	[Category("Command Properties")]
	public ICommand Command
	{
		get
		{
			return (ICommand)GetValue(CommandProperty);
		}
		set
		{
			SetValue(CommandProperty, value);
		}
	}

	[Category("Command Properties")]
	public object CommandParameter
	{
		get
		{
			return GetValue(CommandParameterProperty);
		}
		set
		{
			SetValue(CommandParameterProperty, value);
		}
	}

	[Category("Command Properties")]
	public IInputElement CommandTarget
	{
		get
		{
			return (IInputElement)GetValue(CommandTargetProperty);
		}
		set
		{
			SetValue(CommandTargetProperty, value);
		}
	}

	[Category("Command Properties")]
	public bool SyncOwnerIsEnabled
	{
		get
		{
			return (bool)GetValue(SyncOwnerIsEnabledProperty);
		}
		set
		{
			SetValue(SyncOwnerIsEnabledProperty, value);
		}
	}

	public static ICommand GetTheCommandToRun(DependencyObject d)
	{
		return (ICommand)d.GetValue(TheCommandToRunProperty);
	}

	public static void SetTheCommandToRun(DependencyObject d, ICommand value)
	{
		d.SetValue(TheCommandToRunProperty, value);
	}

	private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((InteractionsSelectorDoubleClickCommandAction)d).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
	}

	private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
	{
		if (oldCommand != null)
		{
			UnhookCommand(oldCommand);
		}
		if (newCommand != null)
		{
			HookCommand(newCommand);
		}
	}

	private void UnhookCommand(ICommand command)
	{
		command.CanExecuteChanged -= CanExecuteChangedHandler;
		UpdateCanExecute();
	}

	private void HookCommand(ICommand command)
	{
		CanExecuteChangedHandler = OnCanExecuteChanged;
		command.CanExecuteChanged += CanExecuteChangedHandler;
		UpdateCanExecute();
	}

	private void OnCanExecuteChanged(object sender, EventArgs e)
	{
		UpdateCanExecute();
	}

	private void UpdateCanExecute()
	{
		if (Command != null)
		{
			if (Command is RoutedCommand routedCommand)
			{
				base.IsEnabled = routedCommand.CanExecute(CommandParameter, CommandTarget);
			}
			else
			{
				base.IsEnabled = Command.CanExecute(CommandParameter);
			}
			if (base.Target != null && SyncOwnerIsEnabled)
			{
				base.Target.IsEnabled = base.IsEnabled;
			}
		}
	}

	protected override void OnAttached()
	{
		base.OnAttached();
		if (base.AssociatedObject is Selector selector)
		{
			selector.MouseDoubleClick += OnMouseDoubleClick;
		}
	}

	protected override void OnDetaching()
	{
		base.OnDetaching();
		if (base.AssociatedObject is Selector selector)
		{
			selector.MouseDoubleClick -= OnMouseDoubleClick;
		}
	}

	protected override void Invoke(object parameter)
	{
	}

	private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		ItemsControl itemsControl = sender as ItemsControl;
		DependencyObject dependencyObject = e.OriginalSource as DependencyObject;
		if (itemsControl == null || dependencyObject == null)
		{
			return;
		}
		DependencyObject dependencyObject2 = ItemsControl.ContainerFromElement(sender as ItemsControl, e.OriginalSource as DependencyObject);
		if (dependencyObject2 == null || dependencyObject2 == DependencyProperty.UnsetValue)
		{
			return;
		}
		object obj = itemsControl.ItemContainerGenerator.ItemFromContainer(dependencyObject2);
		if (obj != null)
		{
			ICommand command = (ICommand)(sender as DependencyObject).GetValue(CommandProperty);
			if (command != null && command.CanExecute(null))
			{
				command.Execute(obj);
			}
		}
	}
}
