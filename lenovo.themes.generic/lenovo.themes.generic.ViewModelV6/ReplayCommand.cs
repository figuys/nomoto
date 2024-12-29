using System;
using System.Windows.Input;

namespace lenovo.themes.generic.ViewModelV6;

public class ReplayCommand : ICommand
{
	private Action<object> mExecute;

	private Predicate<object> mPredicate;

	public event EventHandler CanExecuteChanged
	{
		add
		{
			CommandManager.RequerySuggested += value;
		}
		remove
		{
			CommandManager.RequerySuggested -= value;
		}
	}

	public ReplayCommand(Action<object> execute)
	{
		mExecute = execute;
	}

	public ReplayCommand(Action<object> execut, Predicate<object> predicate)
	{
		mExecute = execut;
		mPredicate = predicate;
	}

	public bool CanExecute(object parameter)
	{
		if (mPredicate != null)
		{
			return mPredicate(parameter);
		}
		return true;
	}

	public void Execute(object parameter)
	{
		mExecute(parameter);
	}
}
