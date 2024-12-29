using System.Collections.Generic;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class Manual : BaseStep
{
	public override void Run()
	{
		string text = base.Info.Args.PromptText.ToString();
		string text2 = string.Empty;
		if (base.Info.Args.Image != null)
		{
			text2 = base.Info.Args.Image.ToString();
		}
		List<string> list = new List<string>();
		if (base.Info.Args.ButtonContent != null)
		{
			foreach (object item2 in base.Info.Args.ButtonContent)
			{
				string item = (string)(dynamic)item2;
				list.Add(item);
			}
		}
		bool? flag = base.Recipe.UcDevice.MessageBox.AutoClose(base.Name, text, text2, list, link: base.Info.Args.link?.ToString(), showClose: true, popupWhenClose: true);
		Result result = Result.PASSED;
		string response = null;
		if (!flag.HasValue)
		{
			result = Result.MANUAL_QUIT;
			response = "manual quit";
		}
		base.Log.AddResult(this, result, response);
	}
}
