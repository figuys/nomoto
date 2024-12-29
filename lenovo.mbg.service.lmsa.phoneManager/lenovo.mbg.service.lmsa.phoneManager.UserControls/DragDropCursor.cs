using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class DragDropCursor : UserControl, IComponentConnector
{
	public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(DragDropCursor), new PropertyMetadata(null));

	public static readonly DependencyProperty TipsProperty = DependencyProperty.Register("Tips", typeof(string), typeof(DragDropCursor), new PropertyMetadata(string.Empty));

	public ImageSource IconSource
	{
		get
		{
			return (ImageSource)GetValue(IconSourceProperty);
		}
		set
		{
			SetValue(IconSourceProperty, value);
		}
	}

	public string Tips
	{
		get
		{
			return (string)GetValue(TipsProperty);
		}
		set
		{
			SetValue(TipsProperty, value);
		}
	}

	public DragDropCursor()
	{
		InitializeComponent();
	}
}
