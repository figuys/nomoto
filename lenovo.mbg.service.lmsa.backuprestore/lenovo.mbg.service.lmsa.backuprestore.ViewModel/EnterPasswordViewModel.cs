using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class EnterPasswordViewModel : ViewModelBase
{
	private long locker;

	public string _PassWord;

	public string _ErrorTips;

	public ReplayCommand CloseCommand { get; }

	public ReplayCommand CancelCommand { get; }

	public ReplayCommand OkCommand { get; }

	public ReplayCommand VerifyCommand { get; }

	public bool? Result { get; private set; }

	public string PassWord
	{
		get
		{
			return _PassWord;
		}
		set
		{
			_PassWord = value;
			OnPropertyChanged("PassWord");
		}
	}

	public string ErrorTips
	{
		get
		{
			return _ErrorTips;
		}
		set
		{
			_ErrorTips = value;
			OnPropertyChanged("ErrorTips");
		}
	}

	public Func<string, bool> PasswordCheckDelegate { get; set; }

	public EnterPasswordViewModel(Func<string, bool> passwordCheckDelegate)
	{
		CloseCommand = new ReplayCommand(CloseCommandHandler);
		CancelCommand = new ReplayCommand(CancelCommandHandler);
		OkCommand = new ReplayCommand(OkCommandHandler);
		VerifyCommand = new ReplayCommand(VerifyCommandHandler);
		PasswordCheckDelegate = passwordCheckDelegate;
	}

	private void CloseCommandHandler(object data)
	{
		(data as Window).Close();
	}

	private void CancelCommandHandler(object data)
	{
		Result = false;
		CloseCommandHandler(data);
	}

	private void OkCommandHandler(object data)
	{
		if (ErrorTips == null && string.IsNullOrEmpty(PassWord))
		{
			ErrorTips = "Required";
		}
		else
		{
			if (Interlocked.Read(ref locker) != 0L)
			{
				return;
			}
			Interlocked.Exchange(ref locker, 1L);
			Task.Run(() => PasswordCheckDelegate(PassWord)).ContinueWith(delegate(Task<bool> s)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					if (s.Result)
					{
						Result = true;
						CloseCommandHandler(data);
					}
					else
					{
						ErrorTips = "K0788";
					}
					Interlocked.Exchange(ref locker, 0L);
				});
			});
		}
	}

	private void VerifyCommandHandler(object data)
	{
		if (!string.IsNullOrEmpty(PassWord) && !PasswordCheckDelegate(PassWord))
		{
			ErrorTips = "K0788";
		}
		else
		{
			ErrorTips = null;
		}
	}
}
