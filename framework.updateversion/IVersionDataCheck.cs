using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.framework.updateversion;

public interface IVersionDataCheck
{
	event EventHandler<VersionDataCheckArgs> OnVersionDataCheck;

	bool CheckData<T>(List<T> data, Func<T, bool> selector = null) where T : class;
}
