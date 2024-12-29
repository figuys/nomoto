using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using GoogleAnalytics;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.UserControls.MessageBoxWindow;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;

namespace lenovo.mbg.service.lmsa.ViewModels.SystemOperation;

public class LanguageOperationItemViewModel : MouseOverMenuItemViewModel
{
	public LanguageOperationItemViewModel()
	{
		base.Icon = Application.Current.FindResource("option_defaultDrawingImage") as ImageSource;
		base.MouseOverIcon = Application.Current.FindResource("option_clickDrawingImage") as ImageSource;
		base.Header = "K0281";
	}

	public override void ClickCommandHandler(object e)
	{
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateCustomEvent(App.Category, "MenuOptionButtonClick", "menu-option button click", 0L).Build());
		LenovoPopupWindow win = new LanguageSelectWindowModel(0).CreateWindow("K0281", "K0298", null);
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
		LanguageSelectViewModel viewModel = win.WindowModel.GetViewModel<LanguageSelectViewModel>();
		if (!viewModel.IsOKResult)
		{
			return;
		}
		string languageSelected = viewModel.LanguageSelected;
		if (LMSAContext.CurrentLanguage == languageSelected || !viewModel.IsOKResult)
		{
			return;
		}
		LenovoPopupWindow win2 = new LanguageSelectWindowModel(1).CreateWindow("K0071", "K0299", "K0300", "K0301", Application.Current.FindResource("backupDrawingImage") as ImageSource);
		HostProxy.HostMaskLayerWrapper.New(win2, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win2.ShowDialog();
		});
		if (win2.WindowModel.GetViewModel<LanguageSelectViewModel>().IsOKResult)
		{
			global::Smart.LanguageService.SetCurrentLanguage(languageSelected);
			if (MainWindowViewModel.SingleInstance.CheckCanCloseWindow())
			{
				Process process = new Process();
				process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "Software Fix.exe";
				process.StartInfo.Verb = "RunAs";
				process.StartInfo.Arguments = string.Format("restart " + Process.GetCurrentProcess().Id);
				process.StartInfo.UseShellExecute = false;
				process.Start();
				MainWindowViewModel.SingleInstance.Exit(0);
			}
		}
	}
}
