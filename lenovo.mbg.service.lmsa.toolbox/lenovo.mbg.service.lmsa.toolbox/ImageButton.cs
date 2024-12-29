using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.toolbox;

public class ImageButton : Button
{
	public static readonly DependencyProperty ImageBackgroundProperty = DependencyProperty.Register("ImageBackground", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseDownImageProperty = DependencyProperty.Register("MouseDownImage", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

	public ImageSource ImageBackground
	{
		get
		{
			return (ImageSource)GetValue(ImageBackgroundProperty);
		}
		set
		{
			SetValue(ImageBackgroundProperty, value);
		}
	}

	public ImageSource MouseDownImage
	{
		get
		{
			return (ImageSource)GetValue(MouseDownImageProperty);
		}
		set
		{
			SetValue(MouseDownImageProperty, value);
		}
	}
}
