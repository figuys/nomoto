using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace lenovo.themes.generic.Interactivity.Ex;

public class InvokeCommandAction : TriggerAction<DependencyObject>
{
	public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandAction), new PropertyMetadata(null));

	public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandAction), new PropertyMetadata(null));

	public static readonly DependencyProperty CommandParameter1Property = DependencyProperty.Register("CommandParameter1", typeof(object), typeof(InvokeCommandAction), new PropertyMetadata(null));

	public static readonly DependencyProperty CommandParameter2Property = DependencyProperty.Register(" CommandParameter2", typeof(object), typeof(InvokeCommandAction), new PropertyMetadata(null));

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

	public object CommandParameter1
	{
		get
		{
			return GetValue(CommandParameter1Property);
		}
		set
		{
			SetValue(CommandParameter1Property, value);
		}
	}

	public object CommandParameter2
	{
		get
		{
			return GetValue(CommandParameter2Property);
		}
		set
		{
			SetValue(CommandParameter2Property, value);
		}
	}

	protected override void Invoke(object parameter)
	{
		InvokeCommandActionParameters parameter2 = new InvokeCommandActionParameters(parameter, CommandParameter, CommandParameter1, CommandParameter2);
		if (Command != null && Command.CanExecute(parameter2))
		{
			Command.Execute(parameter2);
		}
	}
}
