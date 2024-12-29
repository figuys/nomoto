using System;

namespace lenovo.mbg.service.lmsa.flash.UserModelV2;

[Serializable]
public class ConnectStepsModel
{
	public int Index { get; set; }

	public string Title { get; set; }

	public string Layout { get; set; }

	public string Image { get; set; }

	public string Content { get; set; }
}
