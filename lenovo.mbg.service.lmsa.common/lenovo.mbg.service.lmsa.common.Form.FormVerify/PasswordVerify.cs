namespace lenovo.mbg.service.lmsa.common.Form.FormVerify;

public class PasswordVerify : IFormVerify
{
	private int passwordLessThan;

	private object passwordLessThanWarn;

	public PasswordVerify()
	{
		passwordLessThan = 6;
		passwordLessThanWarn = "K0026";
	}

	public PasswordVerify(int passwordLessThan, string passwordLessThanWarn)
	{
		this.passwordLessThan = passwordLessThan;
		this.passwordLessThanWarn = passwordLessThanWarn;
	}

	public VerifyResult Verify(object data)
	{
		VerifyResult verifyResult = new VerifyResult();
		string empty = string.Empty;
		if (data == null || string.IsNullOrWhiteSpace(empty = data.ToString().Trim()))
		{
			verifyResult.IsCorrect = false;
			verifyResult.WraningCode = 1;
			verifyResult.WraningContent = "K0041";
			return verifyResult;
		}
		if (empty.Length < passwordLessThan)
		{
			verifyResult.IsCorrect = false;
			verifyResult.WraningCode = 1;
			verifyResult.WraningContent = passwordLessThanWarn;
			return verifyResult;
		}
		verifyResult.WraningCode = 3;
		verifyResult.WraningContent = GetSecurityLevel(empty);
		verifyResult.IsCorrect = true;
		return verifyResult;
	}

	private int GetSecurityLevel(string password)
	{
		byte b = 0;
		int num = 0;
		for (int i = 0; i < password.Length; i++)
		{
			if (b == 7)
			{
				break;
			}
			num = password[i];
			if (num >= 48 && num <= 57)
			{
				if ((b & 1) != 1)
				{
					b |= 1;
				}
			}
			else if ((num >= 65 && num <= 90) || (num >= 97 && num <= 122))
			{
				if ((b & 1) != 2)
				{
					b |= 2;
				}
			}
			else if ((b & 1) != 4)
			{
				b |= 4;
			}
		}
		return (b & 1) + ((b >> 1) & 1) + ((b >> 2) & 1);
	}
}
