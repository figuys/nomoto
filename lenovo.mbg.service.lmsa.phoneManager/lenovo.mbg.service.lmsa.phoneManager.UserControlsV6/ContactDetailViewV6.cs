using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class ContactDetailViewV6 : Window, IUserMsgControl, IComponentConnector, IStyleConnector
{
	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public ContactDetailViewV6()
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
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

	private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		ContactItemPhoneViewModelV6 obj = ((TextBox)sender).DataContext as ContactItemPhoneViewModelV6;
		if (obj != null && obj.ContactInfoType == ContactInfoType.Telephone)
		{
			if (((TextBox)sender).Text.Length >= 15)
			{
				e.Handled = true;
				return;
			}
			Regex regex = new Regex("[^\\d-\\+\\(\\),;#N/\\*\\.]+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}

	private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		TextBox textBox = (TextBox)sender;
		ContactItemPhoneViewModelV6 obj = textBox.DataContext as ContactItemPhoneViewModelV6;
		if (obj != null && obj.ContactInfoType == ContactInfoType.Telephone && textBox.Text.Length > 15)
		{
			textBox.Text = textBox.Text.Substring(0, 15);
			textBox.Select(15, 0);
		}
	}

	private void OnBtnSave(object sender, RoutedEventArgs e)
	{
		ObservableCollection<RawContactAddOrEditViewModelV6> source = listboxRawContact.ItemsSource as ObservableCollection<RawContactAddOrEditViewModelV6>;
		RawContactAddOrEditViewModelV6 rawContactAddOrEditViewModelV = null;
		if ((rawContactAddOrEditViewModelV = source.Where((RawContactAddOrEditViewModelV6 m) => string.IsNullOrEmpty(m.Name)).FirstOrDefault()) != null)
		{
			listboxRawContact.ScrollIntoView(rawContactAddOrEditViewModelV);
			Context.MessageBox.ShowMessage("K0071", ResourcesHelper.StringResources.SingleInstance.CONTACT_EMPUTY_WARN_CONTENT);
			return;
		}
		((ContactViewModelV6)base.DataContext).ContactAddOrEditViewModel.SaveContactInfoClickCommand.Execute(listboxRawContact);
		Result = true;
		CloseAction?.Invoke(true);
		Close();
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Result = false;
		CloseAction?.Invoke(false);
		Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
