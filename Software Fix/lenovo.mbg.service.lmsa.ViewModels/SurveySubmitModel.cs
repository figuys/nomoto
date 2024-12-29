using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class SurveySubmitModel
{
	public string type => "Quit";

	public string clientUuid { get; set; }

	public List<SubmitItem> records { get; set; }
}
