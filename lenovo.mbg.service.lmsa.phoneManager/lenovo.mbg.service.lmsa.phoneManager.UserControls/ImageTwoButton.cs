using System.Windows;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public class ImageTwoButton : ImageButton
{
	public static readonly DependencyProperty ForegroundDrawingImageSourceProperty = DependencyProperty.Register("ForegroundDrawingImageSource", typeof(DrawingImage), typeof(ImageTwoButton), new PropertyMetadata(null));

	public DrawingImage ForegroundDrawingImageSource
	{
		get
		{
			return (DrawingImage)GetValue(ForegroundDrawingImageSourceProperty);
		}
		set
		{
			SetValue(ForegroundDrawingImageSourceProperty, value);
		}
	}
}
