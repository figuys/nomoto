namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public class InputWindowReturnArgs
{
	public bool Result { get; private set; }

	public object ReturnContent { get; private set; }

	public InputWindowReturnArgs(bool result, object returnContent)
	{
		Result = result;
		ReturnContent = returnContent;
	}
}
