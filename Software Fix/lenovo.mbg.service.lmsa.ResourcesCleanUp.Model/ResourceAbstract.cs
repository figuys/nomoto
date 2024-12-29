namespace lenovo.mbg.service.lmsa.ResourcesCleanUp.Model;

public abstract class ResourceAbstract
{
	public string RootId { get; protected set; }

	public string Path { get; set; }

	public int CountFlag { get; protected set; }

	public abstract bool Exists { get; }

	public abstract void Delete();
}
