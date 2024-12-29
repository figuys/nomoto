using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.ViewModels;

namespace lenovo.mbg.service.lmsa.ViewV6;

public partial class LanguageSelectViewV6 : Window, IUserMsgControl, IComponentConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public LanguageSelectViewV6()
	{
		InitializeComponent();
		base.Loaded += delegate
		{
			string _current = LMSAContext.CurrentLanguage;
			IEnumerable<RadioButton> source = LogicalTreeHelper.GetChildren(gridLanguageList).OfType<RadioButton>();
			RadioButton radioButton = source.FirstOrDefault((RadioButton x) => x.Tag.ToString() == _current);
			if (radioButton != null)
			{
				radioButton.IsChecked = true;
			}
		};
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void BtnSaveClick(object sender, RoutedEventArgs e)
	{
		IEnumerable<RadioButton> source = LogicalTreeHelper.GetChildren(gridLanguageList).OfType<RadioButton>();
		RadioButton radioButton = source.FirstOrDefault((RadioButton x) => x.IsChecked.Value);
		string text = radioButton.Tag.ToString();
		if (LMSAContext.CurrentLanguage == text)
		{
			BtnCloseClick(null, null);
		}
		else if (ApplcationClass.ApplcationStartWindow.ShowMessage("K0071", "K0299", "K0301", "K0300", isCloseBtn: true) == true)
		{
			global::Smart.LanguageService.SetCurrentLanguage(text);
			if (MainWindowViewModel.SingleInstance.CheckCanCloseWindow())
			{
				LogHelper.LogInstance.Info("Language select page Save button, run new Software Fix Client.");
				DotNetBrowserHelper.Instance.Dispose();
				Process process = new Process();
				process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "Software Fix.exe";
				process.StartInfo.Verb = "RunAs";
				process.StartInfo.Arguments = string.Format("restart " + Process.GetCurrentProcess().Id);
				process.StartInfo.UseShellExecute = false;
				process.Start();
				MainWindowViewModel.SingleInstance.Exit(0);
			}
		}
		else
		{
			LogHelper.LogInstance.Info("Language select page Save button canceled.");
			BtnCloseClick(null, null);
		}
	}

	private void BtnCloseClick(object sender, RoutedEventArgs e)
	{
		Result = false;
		Close();
		CloseAction?.Invoke(false);
	}
}
