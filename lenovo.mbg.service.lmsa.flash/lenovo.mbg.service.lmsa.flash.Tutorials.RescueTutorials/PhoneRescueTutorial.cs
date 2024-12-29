using System.Collections.Generic;
using lenovo.mbg.service.lmsa.flash.Tutorials.Model;
using lenovo.mbg.service.lmsa.flash.Tutorials.Resources;

namespace lenovo.mbg.service.lmsa.flash.Tutorials.RescueTutorials;

public class PhoneRescueTutorial : TutorialDefineModel
{
	public PhoneRescueTutorial(TutorialDefineModel pre)
	{
		PreviousModel = pre;
		base.Steps = new List<TutorialsBaseModel>();
		base.Steps.Add(new TutorialsBaseModel
		{
			FirstTitle = TutorialsResources.StringResource.First_Title_1,
			Content = TutorialsResources.StringResource.Start_04,
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials3,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials4,
			IsSelected = true
		});
		base.Steps.Add(new TutorialsBaseModel
		{
			FirstTitle = TutorialsResources.StringResource.First_Title_2,
			Content = TutorialsResources.StringResource.Start_05,
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials6,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials7
		});
		base.Steps.Add(new TutorialsBaseModel
		{
			FirstTitle = TutorialsResources.StringResource.First_Title_3,
			Content = TutorialsResources.StringResource.Start_06,
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials8,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials9
		});
	}
}
