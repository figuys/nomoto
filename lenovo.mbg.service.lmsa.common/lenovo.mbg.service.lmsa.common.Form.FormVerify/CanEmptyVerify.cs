namespace lenovo.mbg.service.lmsa.common.Form.FormVerify;

public class CanEmptyVerify : IFormVerify
{
	public VerifyResult Verify(object data)
	{
		VerifyResult verifyResult = new VerifyResult();
		verifyResult.IsCorrect = true;
		if (data != null && !string.IsNullOrWhiteSpace(data.ToString()))
		{
			verifyResult.WraningCode = 2;
		}
		else
		{
			verifyResult.WraningCode = 0;
		}
		return verifyResult;
	}
}
