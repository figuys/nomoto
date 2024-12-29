using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.lmsa.ResourcesCleanUp.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ResourcesCleanUp.ViewModel;

public class ScanningViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	public void StartingScan(Action<List<ResourceAbstract>> callback)
	{
		Task.Factory.StartNew(delegate
		{
			List<ResourceAbstract> obj;
			try
			{
				obj = ResourcesLog.Single.GetResources();
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Scan resource throw exception:" + ex.ToString());
				obj = new List<ResourceAbstract>();
			}
			callback?.Invoke(obj);
		});
	}
}
