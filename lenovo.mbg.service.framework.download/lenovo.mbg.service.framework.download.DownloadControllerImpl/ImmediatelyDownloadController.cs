using lenovo.mbg.service.framework.download.DownloadSave;

namespace lenovo.mbg.service.framework.download.DownloadControllerImpl;

public class ImmediatelyDownloadController : AbstractDownloadController
{
	public ImmediatelyDownloadController()
		: this(null)
	{
	}

	public ImmediatelyDownloadController(ISaveDownloadInfoMode SaveMode)
		: this(SaveMode, int.MaxValue)
	{
	}

	public ImmediatelyDownloadController(ISaveDownloadInfoMode SaveMode, int maxDownloadCount)
		: this(SaveMode, maxDownloadCount, 0)
	{
	}

	public ImmediatelyDownloadController(ISaveDownloadInfoMode saveMode, int maxDownloadCount, int controllerLevel)
		: base(saveMode, maxDownloadCount, controllerLevel)
	{
	}
}
