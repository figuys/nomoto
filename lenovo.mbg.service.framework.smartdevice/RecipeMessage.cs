using System.Collections.Generic;
using lenovo.mbg.service.framework.services.Device;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace lenovo.mbg.service.framework.smartdevice;

public struct RecipeMessage
{
	public string RecipeName;

	public double Progress;

	public string StepName;

	public UseCase UseCase;

	[JsonConverter(typeof(StringEnumConverter))]
	public Result OverallResult;

	public object Message;

	public int Index;

	public SortedList<string, string> Info;

	public string failedDescription;

	public Result? FailedResult;

	public string FailedStepName;

	[JsonIgnore]
	public DeviceEx Device;
}
