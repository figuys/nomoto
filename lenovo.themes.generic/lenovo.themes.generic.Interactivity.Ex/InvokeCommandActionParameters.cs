namespace lenovo.themes.generic.Interactivity.Ex;

public class InvokeCommandActionParameters
{
	public object InvokeParameter { get; private set; }

	public object CommandParameter { get; private set; }

	public object CommandParameter1 { get; private set; }

	public object CommandParameter2 { get; private set; }

	public InvokeCommandActionParameters()
	{
	}

	public InvokeCommandActionParameters(object invokeParameter, object commandParameter, object commandParameter1, object commandParameter2)
	{
		InvokeParameter = invokeParameter;
		CommandParameter = commandParameter;
		CommandParameter1 = commandParameter1;
		CommandParameter2 = commandParameter2;
	}
}
