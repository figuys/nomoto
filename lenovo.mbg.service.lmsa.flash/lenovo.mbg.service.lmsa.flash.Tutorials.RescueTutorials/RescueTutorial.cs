using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.Tutorials.Model;
using lenovo.mbg.service.lmsa.flash.Tutorials.Resources;

namespace lenovo.mbg.service.lmsa.flash.Tutorials.RescueTutorials;

public class RescueTutorial : TutorialDefineModel
{
	public RescueTutorial(DevCategory category, Visibility backBtnVisibility = Visibility.Collapsed)
	{
		base.BackBtnVisbility = backBtnVisibility;
		base.Steps = new List<TutorialsBaseModel>();
		switch (category)
		{
		case DevCategory.Phone:
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = TutorialsResources.StringResource.First_Title_1,
				Content = "K1176",
				TipImage = TutorialsResources.ImageResources.rescue_start_tutorials16,
				TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials17,
				IsSelected = true
			});
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = TutorialsResources.StringResource.First_Title_2,
				Content = "K0955",
				TipImage = TutorialsResources.ImageResources.rescue_start_tutorials18,
				TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials19
			});
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = TutorialsResources.StringResource.First_Title_3,
				Content = TutorialsResources.StringResource.Start_12,
				TipImage = TutorialsResources.ImageResources.rescue_start_tutorials20,
				TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials21
			});
			break;
		case DevCategory.Tablet:
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = TutorialsResources.StringResource.First_Title_1,
				Content = "K1176",
				TipImage = TutorialsResources.ImageResources.rescue_start_tutorials22,
				TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials23,
				IsSelected = true
			});
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = TutorialsResources.StringResource.First_Title_2,
				Content = "K0960",
				TipImage = TutorialsResources.ImageResources.rescue_start_tutorials24,
				TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials25
			});
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = TutorialsResources.StringResource.First_Title_3,
				Content = TutorialsResources.StringResource.Start_12,
				TipImage = TutorialsResources.ImageResources.rescue_start_tutorials26,
				TipImagePartDetail = TutorialsResources.ImageResources.rescue_start_tutorials27
			});
			break;
		case DevCategory.Smart:
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = TutorialsResources.StringResource.First_Title_1,
				Content = "K1176",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Tutorials/Themes/Images/guide-smart-01.png")),
				TipImagePartDetail = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Tutorials/Themes/Images/guide-smart-01b.png")),
				IsSelected = true
			});
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = TutorialsResources.StringResource.First_Title_2,
				Content = "K1291",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Tutorials/Themes/Images/guide-smart-02.png")),
				TipImagePartDetail = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Tutorials/Themes/Images/guide-smart-02b.png"))
			});
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = TutorialsResources.StringResource.First_Title_3,
				Content = TutorialsResources.StringResource.Start_12,
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Tutorials/Themes/Images/guide-smart-03.png")),
				TipImagePartDetail = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Tutorials/Themes/Images/guide-smart-03b.png"))
			});
			break;
		default:
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = "1.",
				Content = "K1455",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/Wifi1.png")),
				IsSelected = true
			});
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = "2.",
				Content = "K1456",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/Wifi2.png"))
			});
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = "3.",
				Content = "K1457",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/Wifi3.png"))
			});
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = "4.",
				Content = "K1458",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/Wifi4.png"))
			});
			base.Steps.Add(new TutorialsBaseModel
			{
				FirstTitle = "5.",
				Content = "K1461",
				TipImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/Wifi5.png"))
			});
			break;
		}
	}
}
