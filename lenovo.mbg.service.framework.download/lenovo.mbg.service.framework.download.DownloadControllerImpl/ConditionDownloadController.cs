using lenovo.mbg.service.framework.download.DownloadSave;
using lenovo.mbg.service.framework.download.ICondition;

namespace lenovo.mbg.service.framework.download.DownloadControllerImpl;

public class ConditionDownloadController : AbstractDownloadController, IDownloadCondition
{
	public ConditionDownloadController()
		: this(null)
	{
	}

	public ConditionDownloadController(ISaveDownloadInfoMode SaveMode)
		: this(SaveMode, int.MaxValue)
	{
	}

	public ConditionDownloadController(ISaveDownloadInfoMode SaveMode, int maxDownloadCount)
		: this(SaveMode, maxDownloadCount, 0)
	{
	}

	public ConditionDownloadController(ISaveDownloadInfoMode saveMode, int maxDownloadCount, int controllerLevel)
		: base(saveMode, maxDownloadCount, controllerLevel)
	{
	}

	public bool CanDownload()
	{
		return true;
	}
}
