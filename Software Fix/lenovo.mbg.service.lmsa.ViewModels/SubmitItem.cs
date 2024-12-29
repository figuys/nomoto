namespace lenovo.mbg.service.lmsa.ViewModels;

public class SubmitItem
{
	public string qId { get; set; }

	public string result { get; set; }

	public SubmitItem(string _qid, string _result)
	{
		qId = _qid;
		result = _result;
	}
}
