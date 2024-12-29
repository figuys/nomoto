using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class SurveyResponseModel
{
	public SurveyLables lables { get; set; }

	public List<SurveyMsg> msg { get; set; }
}
