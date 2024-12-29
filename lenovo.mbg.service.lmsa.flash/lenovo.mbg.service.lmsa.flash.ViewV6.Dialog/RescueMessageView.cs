using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.framework.lang;
using lenovo.themes.generic;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class RescueMessageView : UserControl, IMessageViewV6, IComponentConnector
{
	private bool _IsPopup;

	private string _UrlLink;

	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public RescueMessageView()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	public void Init(string title, string content, ImageSource image, string okBtn, string cancelBtn, string link, bool canClose, bool isPopup, bool format)
	{
		txtTitle.LangKey = title;
		content = LangTranslation.Translate(content);
		if (image != null)
		{
			imgborder.Visibility = Visibility.Visible;
			img.Source = image;
		}
		if (!string.IsNullOrEmpty(content))
		{
			content = content.Replace("&#x0a;", "\n");
			content = LangTranslation.Translate(content);
		}
		_UrlLink = link;
		_IsPopup = isPopup;
		SolidColorBrush foreground = Application.Current.TryFindResource("V6_TitleBrushKey") as SolidColorBrush;
		SolidColorBrush foreground2 = Application.Current.TryFindResource("V6_ContentBrushKey") as SolidColorBrush;
		string[] array = content.Split(new string[3] { "\n\n", "\n", " - " }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length > 1 && format)
		{
			List<Inline> list = new List<Inline>();
			for (int i = 0; i < array.Length; i++)
			{
				if (i == 0)
				{
					list.Add(new Run
					{
						Text = array[i] + "\n\n",
						FontWeight = FontWeight.FromOpenTypeWeight(500),
						Foreground = foreground
					});
				}
				else if (i % 2 == 1)
				{
					list.Add(new Run
					{
						Text = array[i],
						FontWeight = FontWeight.FromOpenTypeWeight(500),
						Foreground = foreground2
					});
				}
				else
				{
					list.Add(new Run
					{
						Text = " - " + array[i] + "\n"
					});
				}
			}
			txtContent.Inlines.AddRange(list);
		}
		else
		{
			txtContent.Text = content;
		}
		txtLink.Visibility = (string.IsNullOrEmpty(link) ? Visibility.Collapsed : Visibility.Visible);
		btnClose.Visibility = ((!canClose) ? Visibility.Collapsed : Visibility.Visible);
		btnOk.LangKey = okBtn;
		btnOk.Visibility = (string.IsNullOrEmpty(okBtn) ? Visibility.Collapsed : Visibility.Visible);
		btnCancel.LangKey = cancelBtn;
		btnCancel.Visibility = (string.IsNullOrEmpty(cancelBtn) ? Visibility.Collapsed : Visibility.Visible);
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		Uri uri = new Uri(_UrlLink);
		if (!string.IsNullOrEmpty(uri.Host))
		{
			Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
		}
		else if (File.Exists(_UrlLink))
		{
			Process.Start("explorer.exe", _UrlLink);
		}
	}

	private void OnBtnCancel(object sender, RoutedEventArgs e)
	{
		FireClose(false);
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		FireClose(true);
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		if (_IsPopup)
		{
			pop.IsOpen = true;
		}
		else
		{
			FireClose(null);
		}
	}

	private void OnPopCancel(object sender, RoutedEventArgs e)
	{
		pop.IsOpen = false;
		FireClose(null);
	}

	private void OnPopOk(object sender, RoutedEventArgs e)
	{
		pop.IsOpen = false;
	}

	private void FireClose(bool? result)
	{
		Result = result;
		WaitHandler.Set();
		CloseAction?.Invoke(result);
	}
}
