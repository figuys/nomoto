using System.IO;

namespace lenovo.mbg.service.lmsa.ResourcesCleanUp.Model;

public class DirResource : ResourceAbstract
{
	public override bool Exists => Directory.Exists(base.Path);

	public DirResource(string path)
	{
		base.RootId = "dirs";
		base.CountFlag = 0;
		base.Path = path;
	}

	public override void Delete()
	{
		if (Directory.Exists(base.Path))
		{
			Directory.Delete(base.Path);
		}
	}
}
