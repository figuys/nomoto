using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace lenovo.mbg.service.lmsa.Converters;

public class IndexConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		ListViewItem container = (ListViewItem)value;
		ListView listView = ItemsControl.ItemsControlFromItemContainer(container) as ListView;
		int num = listView.ItemContainerGenerator.IndexFromContainer(container);
		int num2 = ++num;
		return num2.ToString();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
