using System;
using System.Windows;
using System.Windows.Controls;

namespace lenovo.themes.generic.AttachedProperty;

public abstract class LayoutColumn
{
	protected static double? GetColumnWidth(GridViewColumn column, DependencyProperty dp)
	{
		if (column == null)
		{
			throw new ArgumentNullException("column");
		}
		object obj = column.ReadLocalValue(dp);
		if (obj != null && obj.GetType() == typeof(double))
		{
			return (double)obj;
		}
		return null;
	}
}
