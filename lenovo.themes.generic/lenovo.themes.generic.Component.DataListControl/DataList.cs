using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace lenovo.themes.generic.Component.DataListControl;

public partial class DataList : UserControl, IComponentConnector
{
	public static readonly DependencyProperty DetailListContentProperty = DependencyProperty.Register("DetailListContent", typeof(object), typeof(DataList), new PropertyMetadata(null));

	public static readonly DependencyProperty IconListContentProperty = DependencyProperty.Register("IconListContent", typeof(object), typeof(DataList), new PropertyMetadata(null));

	public object DetailListContent
	{
		get
		{
			return GetValue(DetailListContentProperty);
		}
		set
		{
			SetValue(DetailListContentProperty, value);
		}
	}

	public object IconListContent
	{
		get
		{
			return GetValue(IconListContentProperty);
		}
		set
		{
			SetValue(IconListContentProperty, value);
		}
	}

	public DataList()
	{
		InitializeComponent();
	}
}
