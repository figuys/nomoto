namespace lenovo.mbg.service.lmsa.support.ViewModel;

public class ModelnameSn
{
	public string RegisteredModelName { get; set; }

	public string modelName { get; set; }

	public string sn { get; set; }

	public override string ToString()
	{
		return sn;
	}
}
