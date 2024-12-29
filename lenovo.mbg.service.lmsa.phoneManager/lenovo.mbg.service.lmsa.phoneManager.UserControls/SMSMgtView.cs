using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class SMSMgtView : UserControl, IComponentConnector, IStyleConnector
{
	private Dictionary<string, RadioButton> _alphaFilters = new Dictionary<string, RadioButton>();

	private ScrollViewer smsListScrollViewer;

	private double verticalOffsetBaseBottom;

	public SMSMgtView()
	{
		InitializeComponent();
		base.DataContext = SMSViewModel.SingleInstance;
		listContact.ItemsSource = SMSViewModel.SingleInstance.SMSContactList;
		SMSViewModel.SingleInstance.RefreshHandler += RefreshHandler;
		SMSViewModel.SingleInstance.UpdateAlphaFilterHandler += UpdateAlphaFilterHandler;
	}

	private void SmsListdataScrollChangedHandler(object sender, ScrollChangedEventArgs e)
	{
		if (smsListScrollViewer == null)
		{
			if (!(e.OriginalSource is ScrollViewer scrollViewer))
			{
				return;
			}
			smsListScrollViewer = scrollViewer;
		}
		if (!SMSViewModel.SingleInstance.ContactFocusedAndFirstPageDataLoaded)
		{
			SMSViewModel.SingleInstance.ContactFocusedAndFirstPageDataLoaded = true;
			smsListScrollViewer?.ScrollToEnd();
			verticalOffsetBaseBottom = 0.0;
		}
		else if (verticalOffsetBaseBottom > 0.0)
		{
			smsListScrollViewer.ScrollToVerticalOffset(smsListScrollViewer.ExtentHeight - verticalOffsetBaseBottom);
			verticalOffsetBaseBottom = 0.0;
		}
		else if (smsListScrollViewer.VerticalOffset == 0.0)
		{
			double num = smsListScrollViewer.ExtentHeight - smsListScrollViewer.VerticalOffset;
			if (SMSViewModel.SingleInstance.LoadPagingData() > 0)
			{
				verticalOffsetBaseBottom = num;
			}
		}
	}

	private void UpdateAlphaFilters()
	{
		_alphaFilters.Values.Where((RadioButton rb) => rb.IsEnabled).ToList().ForEach(delegate(RadioButton rb)
		{
			rb.IsEnabled = false;
			rb.IsChecked = false;
		});
		List<SMSContactMerged> list = new List<SMSContactMerged>(SMSViewModel.SingleInstance.SMSContactList);
		List<char> list2 = new List<char>();
		foreach (SMSContactMerged item in list)
		{
			string empty = string.Empty;
			string phonePerson = item.PhonePerson;
			if (string.IsNullOrEmpty(phonePerson))
			{
				if (list2.Contains('#'))
				{
					continue;
				}
				list2.Add('#');
				empty = "#";
			}
			else if ((phonePerson[0] >= 'a' && phonePerson[0] <= 'z') || (phonePerson[0] >= 'A' && phonePerson[0] <= 'Z'))
			{
				if (list2.Contains(phonePerson.ToUpper()[0]))
				{
					continue;
				}
				list2.Add(phonePerson.ToUpper()[0]);
				empty = phonePerson.ToUpper()[0].ToString();
			}
			else
			{
				if (list2.Contains('#'))
				{
					continue;
				}
				list2.Add('#');
				empty = "#";
			}
			if (!string.IsNullOrEmpty(empty) && _alphaFilters.Keys.Contains(empty))
			{
				_alphaFilters[empty].IsEnabled = true;
			}
		}
	}

	private void UpdateAlphaFilterHandler(object sender, EventArgs e)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			UpdateAlphaFilters();
		});
	}

	private void RefreshHandler(object sender, EventArgs e)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			checkAll.IsChecked = false;
			CheckAllClick(checkAll, null);
		});
	}

	private void CheckAllClick(object sender, RoutedEventArgs e)
	{
		if (!(sender is CheckBox { IsChecked: not null } checkBox) || !(base.DataContext is SMSViewModel sMSViewModel))
		{
			return;
		}
		bool isSelected = checkBox.IsChecked.HasValue && checkBox.IsChecked.Value;
		ObservableCollection<SMSContactMerged> sMSContactList = sMSViewModel.SMSContactList;
		if (sMSContactList == null)
		{
			return;
		}
		if (sMSContactList != null && sMSContactList.Count > 0)
		{
			foreach (SMSContactMerged item in sMSContactList)
			{
				item.isSelected = isSelected;
			}
		}
		sMSViewModel.ExportButtonEnabled = sMSViewModel.SMSList != null && sMSViewModel.SMSList.Count != 0 && checkBox.IsChecked.Value;
		int num = sMSContactList.Count((SMSContactMerged n) => n.isSelected);
		sMSViewModel.SaveButtonEnabled = ((sMSViewModel.SMSContactList != null && num == 1) ? true : false);
	}

	private void CheckBoxClick(object sender, RoutedEventArgs e)
	{
		CheckBox checkBox = checkAll;
		if (!(base.DataContext is SMSViewModel { SMSContactList: var sMSContactList } sMSViewModel))
		{
			return;
		}
		int count = sMSContactList.Count;
		if (sMSContactList != null && sMSContactList.Count != 0)
		{
			int num = sMSContactList.Where((SMSContactMerged m) => !m.isSelected).Count();
			checkBox.IsChecked = num == 0;
			sMSViewModel.SaveButtonEnabled = count - num == 1;
			sMSViewModel.ExportButtonEnabled = num < count;
		}
	}

	private void AlphaFilter_Click(object sender, RoutedEventArgs e)
	{
		if (!(sender is RadioButton radioButton))
		{
			return;
		}
		string text = radioButton.Content.ToString();
		int count = SMSViewModel.SingleInstance.SMSContactList.Count;
		if (count == 0)
		{
			return;
		}
		if (text == "#")
		{
			List<SMSContactMerged> list = SMSViewModel.SingleInstance.SMSContactList.Where(delegate(SMSContactMerged s)
			{
				string phonePerson = s.PhonePerson;
				if (string.IsNullOrEmpty(phonePerson))
				{
					return true;
				}
				return ((phonePerson[0] < 'a' || phonePerson[0] > 'z') && (phonePerson[0] < 'A' || phonePerson[0] > 'Z')) ? true : false;
			}).ToList();
			if (list.Count == 0)
			{
				return;
			}
			listContact.SelectedItem = list[0];
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				string phonePerson2 = SMSViewModel.SingleInstance.SMSContactList[i].PhonePerson;
				if (!string.IsNullOrEmpty(phonePerson2) && phonePerson2.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
				{
					listContact.SelectedItem = SMSViewModel.SingleInstance.SMSContactList[i];
					break;
				}
			}
		}
		if (listContact.SelectedItem != null)
		{
			listContact.ScrollIntoView(listContact.SelectedItem);
		}
	}

	private void listContact_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		SMSViewModel.SingleInstance.ContactFocusedAndFirstPageDataLoaded = false;
		listContact.IsEnabled = false;
		txbSearch.Text = string.Empty;
		SMSContactMerged selectItem = (SMSContactMerged)listContact.SelectedItem;
		SMSViewModel.SingleInstance.ContactSelectionChangedHandler(selectItem, delegate
		{
			listContact.IsEnabled = true;
		});
	}

	private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
	{
		SMS dataContext = ((Grid)sender).DataContext as SMS;
		contentView.DataContext = dataContext;
		contentView.Visibility = Visibility.Visible;
	}

	private void txtInputSmsContent_TextChanged(object sender, TextChangedEventArgs e)
	{
		runTextInputSmsContentCharCount.Text = txtInputSmsContent.Text.Length.ToString();
	}

	private void txtInputForwardSmsContent_TextChanged(object sender, TextChangedEventArgs e)
	{
		runTextInputForwardSmsContentCharCount.Text = txtInputForwardSmsContent.Text.Length.ToString();
	}

	private void txtInputSmsContact_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (e.Changes.Count == 0)
		{
			return;
		}
		TextBox textBox = (TextBox)e.Source;
		string text = textBox.Text.Trim();
		int length = text.Length;
		if (length == 0)
		{
			textInputSmsContactListCount.Text = "0";
			return;
		}
		char obj = text[length - 1];
		string empty = string.Empty;
		if (';'.Equals(obj))
		{
			List<string> list = SMSContactAddressFormater.Format(text);
			int count = list.Count;
			empty = SMSContactAddressFormater.Format(list);
			textInputSmsContactListCount.Text = count.ToString();
			if (!text.Equals(list))
			{
				textBox.Text = empty;
			}
		}
		textBox.SelectionStart = textBox.Text.Length;
	}

	private void txtInputSmsContact_LostFocus(object sender, RoutedEventArgs e)
	{
		TextBox textBox = (TextBox)e.Source;
		string text = textBox.Text.Trim();
		if (!string.IsNullOrEmpty(text))
		{
			int length = text.Length;
			char obj = text[length - 1];
			if (!';'.Equals(obj))
			{
				textBox.Text += ";";
			}
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
