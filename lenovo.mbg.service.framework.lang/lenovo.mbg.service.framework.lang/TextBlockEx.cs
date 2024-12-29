using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.lang;

public class TextBlockEx : TextBlock
{
	public static readonly DependencyProperty LangKeyProperty = DependencyProperty.RegisterAttached("LangKey", typeof(string), typeof(TextBlockEx), new PropertyMetadata(string.Empty, OnLangKeyChanged));

	public string LangKey
	{
		get
		{
			return (string)GetValue(LangKeyProperty);
		}
		set
		{
			SetValue(LangKeyProperty, value);
		}
	}

	private static void OnLangKeyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
	{
		TextBlock textBlock = obj as TextBlock;
		textBlock.Inlines.Clear();
		if (args.NewValue == null || args.NewValue == args.OldValue)
		{
			return;
		}
		string text = LangTranslation.Translate(args.NewValue.ToString());
		string pattern = "\\[(a|h1|h2|h3|h4|b4|b5|b6|b7|r)\\](.*?)\\[/\\1\\]";
		string[] array = Regex.Split(text, pattern, RegexOptions.IgnoreCase);
		if (array.Length == 1)
		{
			textBlock.Text = text;
			return;
		}
		FontFamily fontFamily = Application.Current.TryFindResource("SystemFontKey") as FontFamily;
		for (int i = 0; i < array.Length; i++)
		{
			if (string.IsNullOrEmpty(array[i]))
			{
				continue;
			}
			if (array[i].Equals("a", StringComparison.OrdinalIgnoreCase))
			{
				i++;
				SolidColorBrush foreground = Application.Current.TryFindResource("V6_HyperLinkBrushKey") as SolidColorBrush;
				Run run = new Run
				{
					FontWeight = FontWeight.FromOpenTypeWeight(400),
					Foreground = foreground,
					Cursor = Cursors.Hand,
					FontFamily = fontFamily
				};
				run.TextDecorations.Add(new TextDecoration
				{
					Location = TextDecorationLocation.Underline
				});
				if (array[i].Contains('|'))
				{
					string[] array2 = array[i].Split('|');
					run.Text = array2[0];
					run.Tag = array2[1];
				}
				else
				{
					run.Text = array[i];
					run.Tag = array[i];
				}
				run.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
				{
					Run run2 = sender as Run;
					if (run2.Tag != null)
					{
						string text2 = run.Tag.ToString();
						if (text2 == "rasa.page.link/ma")
						{
							text2 = (Convert.ToBoolean((run2.Parent as FrameworkElement).Tag) ? $"https://download.lenovo.com/lsa/Resource/MA/ma{Configurations.AppVersionCode}.apk" : $"https://rasa.page.link/ma{Configurations.AppVersionCode}");
						}
						GlobalFun.OpenUrlByBrowser(text2);
					}
				};
				textBlock.Inlines.Add(run);
			}
			else if (array[i].Equals("h1", StringComparison.OrdinalIgnoreCase))
			{
				i++;
				SolidColorBrush foreground2 = Application.Current.TryFindResource("V6_TitleBrushKey") as SolidColorBrush;
				textBlock.Inlines.Add(new Run
				{
					Text = array[i],
					FontSize = 16.0,
					FontWeight = FontWeight.FromOpenTypeWeight(500),
					Foreground = foreground2,
					FontFamily = fontFamily
				});
			}
			else if (array[i].Equals("h2", StringComparison.OrdinalIgnoreCase))
			{
				i++;
				SolidColorBrush foreground3 = Application.Current.TryFindResource("V6_TitleBrushKey") as SolidColorBrush;
				textBlock.Inlines.Add(new Run
				{
					Text = array[i],
					FontSize = 15.0,
					FontWeight = FontWeight.FromOpenTypeWeight(500),
					Foreground = foreground3,
					FontFamily = fontFamily
				});
			}
			else if (array[i].Equals("h3", StringComparison.OrdinalIgnoreCase))
			{
				i++;
				SolidColorBrush foreground4 = Application.Current.TryFindResource("V6_TitleBrushKey") as SolidColorBrush;
				textBlock.Inlines.Add(new Run
				{
					Text = array[i],
					FontSize = 14.0,
					FontWeight = FontWeight.FromOpenTypeWeight(500),
					Foreground = foreground4,
					FontFamily = fontFamily
				});
			}
			else if (array[i].Equals("h4", StringComparison.OrdinalIgnoreCase))
			{
				i++;
				SolidColorBrush foreground5 = Application.Current.TryFindResource("V6_TitleBrushKey") as SolidColorBrush;
				textBlock.Inlines.Add(new Run
				{
					Text = array[i],
					FontSize = 13.0,
					FontWeight = FontWeight.FromOpenTypeWeight(500),
					Foreground = foreground5,
					FontFamily = fontFamily
				});
			}
			else if (array[i].Equals("r", StringComparison.OrdinalIgnoreCase))
			{
				i++;
				SolidColorBrush foreground6 = Application.Current.TryFindResource("V6_WarnningBrushKey") as SolidColorBrush;
				textBlock.Inlines.Add(new Run
				{
					Text = array[i],
					FontWeight = FontWeight.FromOpenTypeWeight(500),
					Foreground = foreground6,
					FontFamily = fontFamily
				});
			}
			else if (array[i].Equals("b4", StringComparison.OrdinalIgnoreCase))
			{
				i++;
				textBlock.Inlines.Add(new Run
				{
					Text = array[i],
					FontWeight = FontWeight.FromOpenTypeWeight(400),
					FontFamily = fontFamily
				});
			}
			else if (array[i].Equals("b5", StringComparison.OrdinalIgnoreCase))
			{
				i++;
				textBlock.Inlines.Add(new Run
				{
					Text = array[i],
					FontWeight = FontWeight.FromOpenTypeWeight(500),
					FontFamily = fontFamily
				});
			}
			else if (array[i].Equals("b6", StringComparison.OrdinalIgnoreCase))
			{
				i++;
				textBlock.Inlines.Add(new Run
				{
					Text = array[i],
					FontWeight = FontWeight.FromOpenTypeWeight(600),
					FontFamily = fontFamily
				});
			}
			else if (array[i].Equals("b7", StringComparison.OrdinalIgnoreCase))
			{
				i++;
				textBlock.Inlines.Add(new Run
				{
					Text = array[i],
					FontWeight = FontWeight.FromOpenTypeWeight(700),
					FontFamily = fontFamily
				});
			}
			else
			{
				textBlock.Inlines.Add(new Run
				{
					Text = array[i],
					FontFamily = fontFamily
				});
			}
		}
	}
}
