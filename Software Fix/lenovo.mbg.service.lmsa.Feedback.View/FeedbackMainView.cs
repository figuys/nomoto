using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.Feedback.ViewModel;

namespace lenovo.mbg.service.lmsa.Feedback.View;

public partial class FeedbackMainView : Window, IUserMsgControl, IComponentConnector
{
	private Regex regex = new Regex("([&<>\"'\\\\\\/\\u2026])");

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public FeedbackMainView(bool isMainWindowLoad = true)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		base.DataContext = new FeedbackMainViewModelV6(this, isMainWindowLoad);
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		CloseAction?.Invoke(false);
	}

	private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		e.Handled = Regex.IsMatch(e.Text, "[/,<,>,|,:,?,*,\\\\,\"]");
	}

	private void OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		if (e.Command == ApplicationCommands.Paste)
		{
			string text = Clipboard.GetText();
			e.Handled = Regex.IsMatch(text, "[/,<,>,|,:,?,*,\\\\,\"]");
		}
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		GlobalFun.OpenUrlByBrowser("www.lenovo.com/privacy/");
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void TipsTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (regex.IsMatch(txtComments.Text))
		{
			bdInvid.Visibility = Visibility.Visible;
			bdComments.CornerRadius = new CornerRadius(0.0, 0.0, 4.0, 4.0);
			txtComments.BorderThickness = new Thickness(1.0);
			bdComments.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE4848"));
			(base.DataContext as FeedbackMainViewModelV6).IsValid = true;
		}
		else
		{
			bdComments.CornerRadius = new CornerRadius(4.0, 4.0, 4.0, 4.0);
			txtComments.BorderThickness = new Thickness(1.0);
			bdComments.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6D86AB"));
			bdInvid.Visibility = Visibility.Collapsed;
			(base.DataContext as FeedbackMainViewModelV6).IsValid = false;
		}
	}
}
