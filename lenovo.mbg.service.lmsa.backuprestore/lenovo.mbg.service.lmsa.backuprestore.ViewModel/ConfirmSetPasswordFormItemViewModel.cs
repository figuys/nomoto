using System;
using lenovo.mbg.service.lmsa.common.Form.FormVerify;
using lenovo.mbg.service.lmsa.common.Form.ViewModel;

namespace lenovo.mbg.service.lmsa.backuprestore.ViewModel;

public class ConfirmSetPasswordFormItemViewModel : FormItemViewModel
{
	private SetPasswordFormItemViewModel PasswordFormItemViewModel;

	public ConfirmSetPasswordFormItemViewModel(SetPasswordFormItemViewModel passwordItemViewModel, IFormVerify verify)
		: base(verify)
	{
		PasswordFormItemViewModel = passwordItemViewModel;
	}

	public override bool Verify()
	{
		Tuple<IFormVerify, string, string> data = new Tuple<IFormVerify, string, string>(PasswordFormItemViewModel.FormVerify, PasswordFormItemViewModel.InputValue, base.InputValue);
		VerifyResult verifyResult = base.FormVerify.Verify(data);
		base.Wraning = new FormItemVerifyWraningViewModel
		{
			WraningCode = verifyResult.WraningCode,
			WraningContent = verifyResult.WraningContent
		};
		return verifyResult.IsCorrect;
	}
}
