using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class TabletTurnOffView : UserControl, IMessageViewV6, IComponentConnector
{
	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public TabletTurnOffView()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	public void Init(string title, string message, string image, string note)
	{
		tbkTitle.Text = HostProxy.LanguageService.Translate(title);
		message = HostProxy.LanguageService.Translate(message);
		MatchCollection matchCollection = Regex.Matches(message, "_#[\\s\\S]*?_#");
		MatchCollection matchCollection2 = Regex.Matches(message, "__[\\s\\S]*?__");
		string[] array = message.Split(new string[2] { "_#", "__" }, StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		foreach (Match item in matchCollection)
		{
			list.Add(item.Value.Replace("_#", string.Empty));
		}
		List<string> list2 = new List<string>();
		foreach (Match item2 in matchCollection2)
		{
			list2.Add(item2.Value.Replace("__", string.Empty));
		}
		SolidColorBrush foreground = TryFindResource("V6_HighLightBkgBrushKey") as SolidColorBrush;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (list.Contains(text))
			{
				txtInfo.Inlines.Add(new Run
				{
					Text = text,
					FontWeight = FontWeight.FromOpenTypeWeight(500)
				});
			}
			else if (list2.Contains(text))
			{
				txtInfo.Inlines.Add(new Run
				{
					Text = text,
					FontWeight = FontWeight.FromOpenTypeWeight(500),
					Foreground = foreground
				});
			}
			else
			{
				txtInfo.Inlines.Add(new Run
				{
					Text = text
				});
			}
		}
		if (!string.IsNullOrEmpty(note))
		{
			note = HostProxy.LanguageService.Translate(note);
			matchCollection = Regex.Matches(note, "_#[\\s\\S]*?_#");
			array = note.Split(new string[1] { "_#" }, StringSplitOptions.RemoveEmptyEntries);
			list.Clear();
			foreach (Match item3 in matchCollection)
			{
				list.Add(item3.Value.Replace("_#", string.Empty));
			}
			array2 = array;
			foreach (string text2 in array2)
			{
				if (list.Contains(text2))
				{
					txtInfo2.Inlines.Add(new Run
					{
						Text = text2,
						FontWeight = FontWeight.FromOpenTypeWeight(600)
					});
				}
				else
				{
					txtInfo2.Inlines.Add(new Run
					{
						Text = text2
					});
				}
			}
		}
		if (!string.IsNullOrEmpty(image))
		{
			img.Source = Application.Current.Resources[image] as ImageSource;
		}
	}

	private void OnBtnOk(object sender, RoutedEventArgs e)
	{
		Result = true;
		WaitHandler.Set();
		CloseAction?.Invoke(true);
	}
}
