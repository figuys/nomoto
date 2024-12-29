using System.Windows;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa;

public class ReadDotTipImageButton : ImageButton
{
	public static readonly DependencyProperty RedDotImageSourceProperty = DependencyProperty.Register("RedDotImageSource", typeof(ImageSource), typeof(ReadDotTipImageButton), new PropertyMetadata(null));

	public static readonly DependencyProperty IsShowRedDotProperty = DependencyProperty.Register("IsShowRedDot", typeof(bool), typeof(ReadDotTipImageButton), new PropertyMetadata(false));

	public ImageSource RedDotImageSource
	{
		get
		{
			return (ImageSource)GetValue(RedDotImageSourceProperty);
		}
		set
		{
			SetValue(RedDotImageSourceProperty, value);
		}
	}

	public bool IsShowRedDot
	{
		get
		{
			return (bool)GetValue(IsShowRedDotProperty);
		}
		set
		{
			SetValue(IsShowRedDotProperty, value);
		}
	}
}
