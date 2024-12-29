using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace lenovo.themes.generic.ControlsV6;

public partial class AppPermissionsTipViewV6 : UserControl, IComponentConnector
{
	public AppPermissionsTipViewV6(string appIdendity)
	{
		InitializeComponent();
		BitmapImage bitmapImage = null;
		string empty = string.Empty;
		if ("Ma".Equals(appIdendity))
		{
			empty = "Mobile Assistant";
			bitmapImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/logo_small.png"));
		}
		else
		{
			empty = "Device Help";
			bitmapImage = new BitmapImage(new Uri("pack://application:,,,/lenovo.themes.generic;component/Resource/device_help_logo.png"));
			imgIcon1.Width = 45.0;
			imgIcon1.Height = 45.0;
			borderIcon1.BorderThickness = new Thickness(0.0);
			imgIcon2.Width = 32.0;
			imgIcon2.Height = 32.0;
			borderIcon2.Background = Brushes.Transparent;
		}
		txtAppName1.Text = empty;
		txtAppName2.Text = empty;
		imgIcon1.Source = bitmapImage;
		imgIcon2.Source = bitmapImage;
	}
}
