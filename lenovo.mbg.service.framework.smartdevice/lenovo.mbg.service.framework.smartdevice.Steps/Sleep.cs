using System.Threading;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class Sleep : BaseStep
{
	public override void Run()
	{
		Thread.Sleep((int)base.Info.Args.Milliseconds);
		base.Log.AddResult(this, Result.PASSED);
	}
}
