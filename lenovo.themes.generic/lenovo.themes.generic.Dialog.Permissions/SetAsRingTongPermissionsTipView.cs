using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace lenovo.themes.generic.Dialog.Permissions;

public partial class SetAsRingTongPermissionsTipView : UserControl, IComponentConnector
{
	public static readonly DependencyProperty AppIconImageSourceProperty = DependencyProperty.Register("AppIconImageSource", typeof(ImageSource), typeof(SetAsRingTongPermissionsTipView), new PropertyMetadata(null));

	public static readonly DependencyProperty AppNameProperty = DependencyProperty.Register("AppName", typeof(string), typeof(SetAsRingTongPermissionsTipView), new PropertyMetadata(null));

	public ImageSource AppIconImageSource
	{
		get
		{
			return (ImageSource)GetValue(AppIconImageSourceProperty);
		}
		set
		{
			SetValue(AppIconImageSourceProperty, value);
		}
	}

	public string AppName
	{
		get
		{
			return (string)GetValue(AppNameProperty);
		}
		set
		{
			SetValue(AppNameProperty, value);
		}
	}

	public SetAsRingTongPermissionsTipView()
	{
		InitializeComponent();
	}
}
