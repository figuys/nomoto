namespace lenovo.mbg.service.lmsa.common.Form.FormVerify;

public class CanNotEmptyVerify : IFormVerify
{
	public VerifyResult Verify(object data)
	{
		VerifyResult verifyResult = new VerifyResult();
		if (data != null && !string.IsNullOrWhiteSpace(data.ToString()))
		{
			verifyResult.IsCorrect = true;
			verifyResult.WraningCode = 2;
		}
		else
		{
			verifyResult.IsCorrect = false;
			verifyResult.WraningCode = 1;
			verifyResult.WraningContent = "K0041";
		}
		return verifyResult;
	}
}
