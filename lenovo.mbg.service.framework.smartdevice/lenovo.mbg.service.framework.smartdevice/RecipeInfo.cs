using System.Collections.Generic;
using System.Dynamic;

namespace lenovo.mbg.service.framework.smartdevice;

public class RecipeInfo
{
	public string Name { get; private set; }

	public string UseCase { get; private set; }

	public List<StepInfo> Steps { get; private set; }

	public dynamic Args { get; private set; }

	public bool CheckClientVersion { get; private set; }

	public void Load(dynamic content)
	{
		Name = content.Name;
		UseCase = content.UseCase;
		Steps = new List<StepInfo>();
		CheckClientVersion = content.CheckClientVersion ?? ((object)false);
		foreach (dynamic item in content.Steps)
		{
			StepInfo stepInfo = new StepInfo();
			stepInfo.Load(item);
			Steps.Add(stepInfo);
		}
		Args = new ExpandoObject();
	}
}
