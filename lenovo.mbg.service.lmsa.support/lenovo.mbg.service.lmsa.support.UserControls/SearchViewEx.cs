using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.lmsa.support.ViewModel;
using lenovo.themes.generic.Controls;

namespace lenovo.mbg.service.lmsa.support.UserControls;

public partial class SearchViewEx : UserControl, IComponentConnector
{
	private SearchViewExModel VM;

	public SearchViewEx()
	{
		InitializeComponent();
		VM = new SearchViewExModel();
		base.DataContext = VM;
		base.Loaded += delegate
		{
			(deviceCombobox.Template.FindName("PART_EditableTextBox", deviceCombobox) as TextBox).TextChanged += delegate(object obj, TextChangedEventArgs e)
			{
				TextBox textBox = obj as TextBox;
				textBox.Text = textBox.Text.Trim();
				if (string.IsNullOrEmpty(textBox.Text))
				{
					txt_error.LangKey = string.Empty;
					btn_search.IsEnabled = false;
					VM.WarnVisibility = Visibility.Visible;
				}
				else if (Regex.IsMatch(textBox.Text, "^\\d+$") && !ValidateImei(textBox.Text))
				{
					txt_error.LangKey = "K0989";
					btn_search.IsEnabled = false;
					VM.WarnVisibility = Visibility.Visible;
					LogHelper.LogInstance.Info("Sn:" + textBox.Text);
				}
				else
				{
					txt_error.LangKey = "K0266";
					btn_search.IsEnabled = true;
					VM.WarnVisibility = Visibility.Collapsed;
				}
			};
		};
	}

	public bool ValidateImei(string strImei)
	{
		if (string.IsNullOrEmpty(strImei))
		{
			return false;
		}
		if (Regex.IsMatch(strImei, "^[1-9][0-9]{13,14}$"))
		{
			if (Regex.IsMatch(strImei, "(\\d)\\1{5,15}"))
			{
				return false;
			}
			if (IsOrderNumber(strImei))
			{
				return false;
			}
			string text = strImei;
			if (text.Length > 14)
			{
				text = text.Substring(0, 14);
			}
			int num = 0;
			List<char> list = new List<char>();
			for (int i = 1; i < text.Length; i += 2)
			{
				string text2 = (int.Parse(text[i].ToString()) * 2).ToString();
				list.AddRange(text2.ToCharArray());
			}
			for (int j = 0; j < text.Length; j += 2)
			{
				list.Add(text[j]);
			}
			foreach (char item in list)
			{
				num += int.Parse(item.ToString());
			}
			num %= 10;
			if (num != 0)
			{
				num = 10 - num;
			}
			text += num;
			return text.ToLowerInvariant() == strImei.ToLowerInvariant();
		}
		return false;
	}

	private bool IsOrderNumber(string imei)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 1; i < imei.Length; i++)
		{
			num3 = imei[i] - imei[i - 1];
			num = ((num2 != num3) ? num++ : 0);
			num2 = num3;
			if (num >= 4)
			{
				return true;
			}
		}
		return false;
	}

	private void deviceCombobox_TextChanged(object sender, RoutedEventArgs e)
	{
		if ((sender as LComboBox).Text.Length > 0)
		{
			(sender as LComboBox).EditableTextBoxForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF001429"));
		}
		else
		{
			(sender as LComboBox).EditableTextBoxForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9FAEBF"));
		}
	}
}
