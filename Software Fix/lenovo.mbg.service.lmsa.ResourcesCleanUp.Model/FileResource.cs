using System.IO;

namespace lenovo.mbg.service.lmsa.ResourcesCleanUp.Model;

public class FileResource : ResourceAbstract
{
	public override bool Exists => File.Exists(base.Path);

	public FileResource(string path)
	{
		base.RootId = "files";
		base.CountFlag = 1;
		base.Path = path;
	}

	public override void Delete()
	{
		if (File.Exists(base.Path))
		{
			File.Delete(base.Path);
		}
	}
}
