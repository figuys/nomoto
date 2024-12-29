using System.IO;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class RomFileCheck : BaseStep
{
	public override void Run()
	{
		if (base.Info.Args == null || base.Info.Args.Files == null)
		{
			base.Log.AddResult(this, Result.PASSED);
			return;
		}
		string text = base.Resources.Get(RecipeResources.Rom);
		if (string.IsNullOrEmpty(text) || !Directory.Exists(text))
		{
			base.Log.AddResult(this, Result.LOAD_RESOURCE_FAILED, "Resources directory does not exist!");
			return;
		}
		foreach (dynamic item in base.Info.Args.Files)
		{
			if (!(string.IsNullOrEmpty(item?.Value) ? true : false))
			{
				dynamic files = Directory.GetFiles(text, item.Value, SearchOption.AllDirectories);
				if (files.Length < 1)
				{
					string response = $"Rom:[{text}] search file:[{(object)item.Value}] not exist!";
					base.Log.AddResult(this, Result.CHECK_ROM_FILE_FAILED, response);
					return;
				}
			}
		}
		base.Log.AddResult(this, Result.PASSED);
	}
}
