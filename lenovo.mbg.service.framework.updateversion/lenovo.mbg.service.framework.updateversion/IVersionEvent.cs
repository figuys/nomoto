using System;
using lenovo.mbg.service.framework.updateversion.model;

namespace lenovo.mbg.service.framework.updateversion;

public interface IVersionEvent
{
	event EventHandler<VersionV1EventArgs> OnVersionEvent;
}
