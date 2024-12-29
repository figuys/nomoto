namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class InfoPrompt : BaseStep
{
	public override void Run()
	{
		if (base.Info.Args.PromptText != null)
		{
			string message = base.Info.Args.PromptText.ToString();
			string text = string.Empty;
			if (base.Info.Args.Image != null)
			{
				text = base.Info.Args.Image.ToString();
			}
			if (string.IsNullOrEmpty(text))
			{
				base.Recipe.UcDevice.MessageBox.Show(base.Info.Name, message).Wait();
			}
			else
			{
				base.Recipe.UcDevice.MessageBox.RightPic(base.Info.Name, message, text).Wait();
			}
		}
		if (base.Info.Args.DiscntTitle != null && base.Info.Args.DiscntContent != null)
		{
			string title = base.Info.Args.DiscntTitle.ToString();
			string message2 = base.Info.Args.DiscntContent.ToString();
			string note = base.Info.Args.DiscntNote.ToString();
			string image = base.Info.Args.DiscntImage?.ToString();
			base.Recipe.UcDevice.MessageBox.TabletTurnoff(title, message2, image, note).Wait();
		}
		base.Log.AddResult(this, Result.PASSED);
	}
}
