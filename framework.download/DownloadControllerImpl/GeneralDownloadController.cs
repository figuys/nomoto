using lenovo.mbg.service.framework.download.DownloadSave;

namespace lenovo.mbg.service.framework.download.DownloadControllerImpl;

public class GeneralDownloadController : AbstractDownloadController
{
	public GeneralDownloadController()
		: this(null)
	{
	}

	public GeneralDownloadController(ISaveDownloadInfoMode SaveMode)
		: this(SaveMode, int.MaxValue)
	{
	}

	public GeneralDownloadController(ISaveDownloadInfoMode SaveMode, int maxDownloadCount)
		: this(SaveMode, maxDownloadCount, 0)
	{
	}

	public GeneralDownloadController(ISaveDownloadInfoMode saveMode, int maxDownloadCount, int controllerLevel)
		: base(saveMode, maxDownloadCount, controllerLevel)
	{
	}
}
