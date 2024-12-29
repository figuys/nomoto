using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class ContactMgtViewV6 : UserControl, IComponentConnector, IStyleConnector
{
	public ContactMgtViewV6()
	{
		InitializeComponent();
		base.DataContext = ContactViewModel.SingleInstance;
	}

	private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		ContactItemPhoneViewModel obj = ((TextBox)sender).DataContext as ContactItemPhoneViewModel;
		if (obj != null && obj.ContactInfoType == ContactInfoType.Telephone)
		{
			Regex regex = new Regex("[^\\d-\\+\\(\\),;#N/\\*\\.]+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}

	private void OnDataGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		DataGrid dataGrid = sender as DataGrid;
		Point position = e.GetPosition(dataGrid);
		DependencyObject dependencyObject = dataGrid.InputHitTest(position) as DependencyObject;
		bool flag = false;
		while (dependencyObject != null)
		{
			if (dependencyObject is DataGridRow)
			{
				flag = true;
				break;
			}
			dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
		}
		e.Handled = !flag;
	}

	private void LangButton_Click(object sender, RoutedEventArgs e)
	{
		if (!(ContactListData.ItemsSource is ObservableCollection<ContactItemViewModelV6> { Count: >0 } observableCollection))
		{
			return;
		}
		if (btn_all.LangKey == "K0764")
		{
			for (int i = 0; i < observableCollection.Count; i++)
			{
				observableCollection[i].isSelected = true;
			}
			(base.DataContext as ContactViewModelV6).ContactExportToolBtnEnable = true;
			(base.DataContext as ContactViewModelV6).ContactDeleteToolBtnEnable = true;
			(base.DataContext as ContactViewModelV6).IsAllContactSelected = true;
			(base.DataContext as ContactViewModelV6).SelectContactCount = observableCollection.Count;
		}
		else
		{
			for (int j = 0; j < observableCollection.Count; j++)
			{
				observableCollection[j].isSelected = false;
			}
			(base.DataContext as ContactViewModelV6).ContactExportToolBtnEnable = false;
			(base.DataContext as ContactViewModelV6).ContactDeleteToolBtnEnable = false;
			(base.DataContext as ContactViewModelV6).IsAllContactSelected = false;
			(base.DataContext as ContactViewModelV6).SelectContactCount = 0;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
