using lenovo.mbg.service.lmsa.common.Form.FormVerify;

namespace lenovo.mbg.service.lmsa.Login.Business.FormVerify;

public class CountryVerify : IFormVerify
{
	public VerifyResult Verify(object data)
	{
		VerifyResult verifyResult = new VerifyResult();
		if (data == null || string.IsNullOrWhiteSpace(data.ToString()))
		{
			verifyResult.IsCorrect = false;
			verifyResult.WraningCode = 1;
			verifyResult.WraningContent = "K0041";
			return verifyResult;
		}
		verifyResult.WraningCode = 2;
		verifyResult.IsCorrect = true;
		return verifyResult;
	}
}
