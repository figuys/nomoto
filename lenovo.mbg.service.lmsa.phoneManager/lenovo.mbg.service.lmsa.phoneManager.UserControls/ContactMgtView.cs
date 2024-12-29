using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewModels;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class ContactMgtView : UserControl, IComponentConnector, IStyleConnector
{
	public ContactMgtView()
	{
		InitializeComponent();
		base.DataContext = ContactViewModel.SingleInstance;
		tabs.SelectedIndex = 0;
	}

	private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (tabs.SelectedIndex == 0)
		{
			Tab_Contact.Visibility = Visibility.Visible;
			Tab_CallLog.Visibility = Visibility.Hidden;
		}
		else
		{
			Tab_Contact.Visibility = Visibility.Hidden;
			Tab_CallLog.Visibility = Visibility.Visible;
		}
	}

	private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		if ((((TextBox)sender).DataContext as ContactItemPhoneViewModel).ContactInfoType == ContactInfoType.Telephone)
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

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
