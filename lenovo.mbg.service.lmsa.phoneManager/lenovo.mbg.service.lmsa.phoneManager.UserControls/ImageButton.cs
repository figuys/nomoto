using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public class ImageButton : Button
{
	public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ImageButton), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty ImageBackgroundProperty = DependencyProperty.Register("ImageBackground", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null));

	public static readonly DependencyProperty MouseDownImageProperty = DependencyProperty.Register("MouseDownImage", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));

	public string Text
	{
		get
		{
			return (string)GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

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

	public Brush MouseOverBackground
	{
		get
		{
			return (Brush)GetValue(MouseOverBackgroundProperty);
		}
		set
		{
			SetValue(MouseOverBackgroundProperty, value);
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
