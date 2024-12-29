using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class SetPasswordViewModel : ViewModelBase
{
	public string _PassWord;

	public string _ConfirmPassWord;

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

	public string ConfirmPassWord
	{
		get
		{
			return _ConfirmPassWord;
		}
		set
		{
			_ConfirmPassWord = value;
			OnPropertyChanged("ConfirmPassWord");
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

	public SetPasswordViewModel()
	{
		CloseCommand = new ReplayCommand(CloseCommandHandler);
		CancelCommand = new ReplayCommand(CancelCommandHandler);
		OkCommand = new ReplayCommand(OkCommandHandler);
		VerifyCommand = new ReplayCommand(VerifyCommandHandler);
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
		if (ErrorTips == null)
		{
			if (string.IsNullOrEmpty(PassWord) || string.IsNullOrEmpty(ConfirmPassWord))
			{
				ErrorTips = "Required";
				return;
			}
			Result = true;
			CloseCommandHandler(data);
		}
	}

	private void VerifyCommandHandler(object data)
	{
		if (string.IsNullOrEmpty(PassWord) || string.IsNullOrEmpty(ConfirmPassWord) || PassWord.Equals(ConfirmPassWord))
		{
			ErrorTips = null;
		}
		else
		{
			ErrorTips = "K0027";
		}
	}
}
