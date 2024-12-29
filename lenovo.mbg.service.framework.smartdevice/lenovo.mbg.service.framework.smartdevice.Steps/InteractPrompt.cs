using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class InteractPrompt : BaseStep
{
	public override void Run()
	{
		Result result = Result.PASSED;
		string response = null;
		string title = base.Info.Args.title.ToString();
		string message = base.Info.Args.content.ToString();
		JArray data = base.Info.Args.links;
		if (base.Recipe.UcDevice.MessageBox.Linker(title, message, data).Result != true)
		{
			result = Result.QUIT;
			response = "interact prompt quit";
		}
		base.Log.AddResult(this, result, response);
	}
}
