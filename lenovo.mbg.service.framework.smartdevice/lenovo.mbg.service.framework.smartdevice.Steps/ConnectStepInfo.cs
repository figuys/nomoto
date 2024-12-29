using System.Collections.Generic;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ConnectStepInfo
{
	public string NoteText { get; internal set; }

	public string RetryText { get; internal set; }

	public string WidthRatio { get; internal set; }

	public List<ConnectSteps> Steps { get; internal set; }
}
