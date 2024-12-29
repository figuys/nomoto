using System.Collections.Generic;
using lenovo.mbg.service.lmsa.UpdateVersion.Model;

namespace lenovo.mbg.service.lmsa.Business;

public abstract class AbstractDataBase
{
	public abstract List<PluginVersionModel> GetObject();

	public abstract void UpdateObject(List<PluginVersionModel> objs);
}
