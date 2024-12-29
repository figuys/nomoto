using System.Collections.Generic;
using lenovo.mbg.service.lmsa.flash.Tutorials.Model;
using lenovo.mbg.service.lmsa.flash.Tutorials.Resources;

namespace lenovo.mbg.service.lmsa.flash.Tutorials.RescueTutorials;

public class TabletRescueTutorial : TutorialDefineModel
{
	public TabletRescueTutorial(TutorialDefineModel pre)
	{
		PreviousModel = pre;
		base.Steps = new List<TutorialsBaseModel>();
		base.Steps.Add(new TutorialsBaseModel
		{
			FirstTitle = TutorialsResources.StringResource.First_Title_1,
			Content = TutorialsResources.StringResource.Start_10,
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials10,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials11,
			IsSelected = true
		});
		base.Steps.Add(new TutorialsBaseModel
		{
			FirstTitle = TutorialsResources.StringResource.First_Title_2,
			Content = TutorialsResources.StringResource.Start_11,
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials12,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials13
		});
		base.Steps.Add(new TutorialsBaseModel
		{
			FirstTitle = TutorialsResources.StringResource.First_Title_3,
			Content = TutorialsResources.StringResource.Start_12,
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials14,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials15
		});
	}
}
