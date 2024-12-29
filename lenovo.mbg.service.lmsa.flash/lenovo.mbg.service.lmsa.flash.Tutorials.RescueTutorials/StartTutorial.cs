using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.Tutorials.Model;
using lenovo.mbg.service.lmsa.flash.Tutorials.Resources;

namespace lenovo.mbg.service.lmsa.flash.Tutorials.RescueTutorials;

internal class StartTutorial : TutorialDefineModel
{
	public StartTutorial()
	{
		base.Steps = new List<TutorialsBaseModel>();
		base.Steps.Add(new TutorialsBaseModel
		{
			FirstTitle = TutorialsResources.StringResource.First_Title_1,
			Content = TutorialsResources.StringResource.Start_01,
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials1,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials2,
			IsManual = true,
			IsSelected = true
		});
		base.Steps.Add(new TutorialsBaseModel
		{
			RadioTitleVisibility = Visibility.Visible,
			Content = TutorialsResources.StringResource.Start_02,
			TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Tutorials/Themes/Images/5.png")),
			TipImagePartDetail = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Tutorials/Themes/Images/5b.png")),
			IsManual = true,
			NextModel = new RescueTutorial(DevCategory.Phone, Visibility.Visible)
		});
		base.Steps.Add(new TutorialsBaseModel
		{
			RadioTitleVisibility = Visibility.Visible,
			Content = TutorialsResources.StringResource.Start_03,
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials10,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials11,
			IsManual = true,
			NextModel = new RescueTutorial(DevCategory.Tablet, Visibility.Visible)
		});
		base.Steps.Add(new TutorialsBaseModel
		{
			RadioTitleVisibility = Visibility.Visible,
			Content = "K1290",
			TipImage = TutorialsResources.ImageResources.rescue_start_tutorials32,
			TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials33,
			IsManual = true,
			NextModel = new RescueTutorial(DevCategory.Smart, Visibility.Visible)
		});
	}
}
