using System;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.log;

namespace lenovo.mbg.service.lmsa.common.Form.FormVerify;

public class EmailAddressVerify : IFormVerify
{
	public bool IsCanEmpty { get; set; }

	public VerifyResult Verify(object data)
	{
		VerifyResult verifyResult = new VerifyResult();
		string text = data as string;
		if (string.IsNullOrWhiteSpace(text))
		{
			if (IsCanEmpty)
			{
				verifyResult.WraningCode = 2;
				verifyResult.IsCorrect = true;
			}
			else
			{
				verifyResult.IsCorrect = false;
				verifyResult.WraningCode = 1;
				verifyResult.WraningContent = "K0041";
			}
		}
		else if (FormateVerify(text))
		{
			verifyResult.WraningCode = 2;
			verifyResult.IsCorrect = true;
		}
		else
		{
			verifyResult.IsCorrect = false;
			verifyResult.WraningCode = 1;
			verifyResult.WraningContent = "K0042";
		}
		return verifyResult;
	}

	private bool FormateVerify(string email)
	{
		try
		{
			return new Regex("^([a-z0-9A-Z]+[-_\\.]?)+[a-z0-9A-Z]@([a-z0-9A-Z]+(-[a-z0-9A-Z]+)?\\.)+[a-zA-Z]{2,}$").IsMatch(email);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Email check error:" + ex.ToString());
			return false;
		}
	}
}
