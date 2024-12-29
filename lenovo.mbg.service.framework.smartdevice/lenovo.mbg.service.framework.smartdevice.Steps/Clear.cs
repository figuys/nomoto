using lenovo.mbg.service.framework.smartdevice.ODMSocketServer;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class Clear : BaseStep
{
	public override void Run()
	{
		ODMServerMain.CloseAllSockets();
		base.Log.AddResult(this, Result.PASSED);
	}
}
