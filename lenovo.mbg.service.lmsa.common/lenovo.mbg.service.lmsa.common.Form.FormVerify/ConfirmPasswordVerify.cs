using System;

namespace lenovo.mbg.service.lmsa.common.Form.FormVerify;

public class ConfirmPasswordVerify : IFormVerify
{
	public VerifyResult Verify(object data)
	{
		VerifyResult verifyResult = new VerifyResult();
		Tuple<IFormVerify, string, string> obj = data as Tuple<IFormVerify, string, string>;
		IFormVerify item = obj.Item1;
		string item2 = obj.Item2;
		string item3 = obj.Item3;
		if (string.IsNullOrWhiteSpace(item2))
		{
			verifyResult.IsCorrect = false;
			verifyResult.WraningCode = 1;
			verifyResult.WraningContent = "K0041";
			return verifyResult;
		}
		if (string.Compare(item2, item3) == 0)
		{
			if (item.Verify(item2).IsCorrect)
			{
				verifyResult.WraningCode = 2;
				verifyResult.IsCorrect = true;
				return verifyResult;
			}
			verifyResult.IsCorrect = false;
			verifyResult.WraningCode = 0;
			verifyResult.WraningContent = string.Empty;
			return verifyResult;
		}
		verifyResult.IsCorrect = false;
		verifyResult.WraningCode = 1;
		verifyResult.WraningContent = "K0027";
		return verifyResult;
	}
}
