using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.Feedback.View;

public partial class FeedbackListViewV6 : Window, IUserMsgControl, IComponentConnector, IStyleConnector
{
	private Regex regex = new Regex("([&<>\"'\\\\\\/\\u2026])");

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public FeedbackListViewV6()
	{
		InitializeComponent();
		base.Closed += delegate
		{
			Result = false;
			CloseAction?.Invoke(false);
		};
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer scrollViewer = (ScrollViewer)sender;
		scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - (double)e.Delta);
		e.Handled = true;
	}

	private void CloseBtnClick(object sender, RoutedEventArgs e)
	{
		Result = false;
		Close();
		CloseAction?.Invoke(false);
	}

	private void TxtCommentsTextChanged(object sender, TextChangedEventArgs e)
	{
		btnReply.IsEnabled = !string.IsNullOrWhiteSpace(txtComments.Text);
		if (regex.IsMatch(txtComments.Text))
		{
			bdInvid.Visibility = Visibility.Visible;
			txtComments.BorderThickness = new Thickness(1.0);
			txtComments.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE4848"));
			btnReply.IsEnabled = false;
		}
		else
		{
			txtComments.BorderThickness = new Thickness(1.0);
			txtComments.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6D86AB"));
			bdInvid.Visibility = Visibility.Collapsed;
		}
	}

	private void ImgContentPreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		string path = ((Image)sender).Tag.ToString();
		string text = Path.Combine(Path.GetTempPath(), "feedback_img_" + Path.GetFileName(path));
		if (File.Exists(text))
		{
			Process.Start(text);
		}
	}
}
