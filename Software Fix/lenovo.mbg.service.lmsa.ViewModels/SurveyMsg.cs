using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class SurveyMsg
{
	public string id { get; set; }

	public string type { get; set; }

	public string content { get; set; }

	public string question { get; set; }

	public List<SurveyNewOptions> options { get; set; }
}
