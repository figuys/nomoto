using System.Collections.Generic;
using lenovo.mbg.service.lmsa.flash.Tutorials.Model;
using lenovo.mbg.service.lmsa.flash.Tutorials.Resources;

namespace lenovo.mbg.service.lmsa.flash.Tutorials.RescueTutorials;

public class SmartDeviceRescueTutorial : TutorialDefineModel
{
	public SmartDeviceRescueTutorial(TutorialDefineModel pre)
	{
		PreviousModel = pre;
		base.Steps = new List<TutorialsBaseModel>();
		base.Steps.Add(new TutorialsBaseModel
		{
			FirstTitle = TutorialsResources.StringResource.First_Title_1,
			Content = "K1292",
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials32,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials33,
			IsSelected = true
		});
		base.Steps.Add(new TutorialsBaseModel
		{
			FirstTitle = TutorialsResources.StringResource.First_Title_2,
			Content = "K1291",
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials34,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials35
		});
		base.Steps.Add(new TutorialsBaseModel
		{
			FirstTitle = TutorialsResources.StringResource.First_Title_3,
			Content = TutorialsResources.StringResource.Start_12,
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials36,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials37
		});
	}
}
