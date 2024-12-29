using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace lenovo.mbg.service.lmsa.flash.Tutorials.Model;

public class TutorialDefineModel
{
	public TutorialDefineModel NextModel;

	public TutorialDefineModel PreviousModel;

	public Visibility BackBtnVisbility { get; set; } = Visibility.Collapsed;

	public List<TutorialsBaseModel> Steps { get; set; }

	public TutorialsBaseModel GetNextStep(int index)
	{
		TutorialsBaseModel result = null;
		if (index < Steps.Count - 1)
		{
			result = Steps[++index];
		}
		return result;
	}

	public TutorialsBaseModel GetPrevStep(int index)
	{
		TutorialsBaseModel result = null;
		if (index > 0)
		{
			result = Steps.Take(index).LastOrDefault((TutorialsBaseModel n) => n.RadioTitleVisibility == Visibility.Collapsed);
		}
		return result;
	}
}
