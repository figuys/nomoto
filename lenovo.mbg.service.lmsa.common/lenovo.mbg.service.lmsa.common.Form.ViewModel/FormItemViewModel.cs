using lenovo.mbg.service.lmsa.common.Form.FormVerify;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.common.Form.ViewModel;

public class FormItemViewModel : ViewModelBase
{
	private string mInputValue;

	private FormItemVerifyWraningViewModel mWraning;

	private ReplayCommand mVerifyCommand;

	public IFormVerify FormVerify { get; private set; }

	public string InputValue
	{
		get
		{
			return mInputValue;
		}
		set
		{
			if (!(mInputValue == value))
			{
				mInputValue = value;
				OnPropertyChanged("InputValue");
			}
		}
	}

	public FormItemVerifyWraningViewModel Wraning
	{
		get
		{
			return mWraning;
		}
		set
		{
			if (mWraning != value)
			{
				mWraning = value;
				OnPropertyChanged("Wraning");
			}
		}
	}

	public ReplayCommand VerifyCommand
	{
		get
		{
			return mVerifyCommand;
		}
		set
		{
			if (mVerifyCommand != value)
			{
				mVerifyCommand = value;
				OnPropertyChanged("VerifyCommand");
			}
		}
	}

	public FormItemViewModel(IFormVerify verify)
	{
		FormVerify = verify;
		VerifyCommand = new ReplayCommand(VerifyCommandHandler);
	}

	public virtual bool Verify()
	{
		if (FormVerify == null)
		{
			Wraning = null;
			return true;
		}
		VerifyResult verifyResult = FormVerify.Verify(InputValue);
		Wraning = new FormItemVerifyWraningViewModel
		{
			WraningCode = verifyResult.WraningCode,
			WraningContent = verifyResult.WraningContent
		};
		return verifyResult.IsCorrect;
	}

	protected virtual void VerifyCommandHandler(object e)
	{
		Verify();
	}
}
