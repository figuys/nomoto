namespace lenovo.mbg.service.lmsa.Feedback.Model;

public class FeedbackSubmitModel
{
	public string UserName { get; set; }

	public string Email { get; set; }

	public string FeedbackContent { get; set; }

	public string SN { get; set; }

	public string Imei1 { get; set; }

	public string Imei2 { get; set; }

	public string ModelName { get; set; }

	public string MarketName { get; set; }

	public string Module { get; set; }

	public bool IsReplay { get; set; }

	public long? FeedbackId { get; set; }

	public string LogFilePath { get; set; }
}
