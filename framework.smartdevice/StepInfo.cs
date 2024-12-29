namespace lenovo.mbg.service.framework.smartdevice;

public class StepInfo
{
	private string TAG => GetType().FullName;

	public string Name { get; private set; }

	public string Step { get; private set; }

	public dynamic Args { get; private set; }

	public dynamic SubSteps { get; private set; }

	public void Load(dynamic stepContent)
	{
		Name = stepContent.Name;
		Step = stepContent.Step;
		Args = (object)stepContent.Args;
		SubSteps = (object)stepContent.SubSteps;
	}
}
