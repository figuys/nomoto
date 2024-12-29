using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.themes.generic.Controls;

public class LoadingButton : Button
{
	public static readonly DependencyProperty ProgressBarBackgroundProperty = DependencyProperty.Register("ProgressBarBackground", typeof(Color), typeof(LoadingButton), new PropertyMetadata(null));

	public static readonly DependencyProperty ProgressBarForegroundProperty = DependencyProperty.Register("ProgressBarForeground", typeof(Brush), typeof(LoadingButton), new PropertyMetadata(null));

	public static readonly DependencyProperty ProgressBarPanelSizeProperty = DependencyProperty.Register("ProgressBarPanelSize", typeof(double), typeof(LoadingButton), new PropertyMetadata(20.0));

	public static readonly DependencyProperty EllipseStrokeThicknessProperty = DependencyProperty.Register("EllipseStrokeThickness", typeof(double), typeof(LoadingButton), new PropertyMetadata(0.0));

	public static readonly DependencyProperty EllipseStrokeProperty = DependencyProperty.Register("EllipseStroke", typeof(Brush), typeof(LoadingButton), new PropertyMetadata(null));

	public static readonly DependencyProperty EllipseFillProperty = DependencyProperty.Register("EllipseFill", typeof(Brush), typeof(LoadingButton), new PropertyMetadata(null));

	public static readonly DependencyProperty RectangleFillProperty = DependencyProperty.Register("RectangleFill", typeof(Brush), typeof(LoadingButton), new PropertyMetadata(null));

	public static readonly DependencyProperty RectangleWidthProperty = DependencyProperty.Register("RectangleWidth", typeof(double), typeof(LoadingButton), new PropertyMetadata(0.0));

	public static readonly DependencyProperty RectangleHeightProperty = DependencyProperty.Register("RectangleHeight", typeof(double), typeof(LoadingButton), new PropertyMetadata(0.0));

	public static readonly DependencyProperty RectangleRadiusYProperty = DependencyProperty.Register("RectangleRadiusY", typeof(double), typeof(LoadingButton), new PropertyMetadata(10.0));

	public static readonly DependencyProperty RectangleRadiusXProperty = DependencyProperty.Register("RectangleRadiusX", typeof(double), typeof(LoadingButton), new PropertyMetadata(0.0));

	public static readonly DependencyProperty RectangleEffectRadiusProperty = DependencyProperty.Register("RectangleEffectRadius", typeof(double), typeof(LoadingButton), new PropertyMetadata(0.0));

	public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(LoadingButton), new PropertyMetadata(false));

	public Color ProgressBarBackground
	{
		get
		{
			return (Color)GetValue(ProgressBarBackgroundProperty);
		}
		set
		{
			SetValue(ProgressBarBackgroundProperty, value);
		}
	}

	public Brush ProgressBarForeground
	{
		get
		{
			return (Brush)GetValue(ProgressBarForegroundProperty);
		}
		set
		{
			SetValue(ProgressBarForegroundProperty, value);
		}
	}

	public double ProgressBarPanelSize
	{
		get
		{
			return (double)GetValue(ProgressBarPanelSizeProperty);
		}
		set
		{
			SetValue(ProgressBarPanelSizeProperty, value);
		}
	}

	public double EllipseStrokeThickness
	{
		get
		{
			return (double)GetValue(EllipseStrokeThicknessProperty);
		}
		set
		{
			SetValue(EllipseStrokeThicknessProperty, value);
		}
	}

	public Brush EllipseStroke
	{
		get
		{
			return (Brush)GetValue(EllipseStrokeProperty);
		}
		set
		{
			SetValue(EllipseStrokeProperty, value);
		}
	}

	public Brush EllipseFill
	{
		get
		{
			return (Brush)GetValue(EllipseFillProperty);
		}
		set
		{
			SetValue(EllipseFillProperty, value);
		}
	}

	public Brush RectangleFill
	{
		get
		{
			return (Brush)GetValue(RectangleFillProperty);
		}
		set
		{
			SetValue(RectangleFillProperty, value);
		}
	}

	public double RectangleWidth
	{
		get
		{
			return (double)GetValue(RectangleWidthProperty);
		}
		set
		{
			SetValue(RectangleWidthProperty, value);
		}
	}

	public double RectangleHeight
	{
		get
		{
			return (double)GetValue(RectangleHeightProperty);
		}
		set
		{
			SetValue(RectangleHeightProperty, value);
		}
	}

	public double RectangleRadiusY
	{
		get
		{
			return (double)GetValue(RectangleRadiusYProperty);
		}
		set
		{
			SetValue(RectangleRadiusYProperty, value);
		}
	}

	public double RectangleRadiusX
	{
		get
		{
			return (double)GetValue(RectangleRadiusXProperty);
		}
		set
		{
			SetValue(RectangleRadiusXProperty, value);
		}
	}

	public double RectangleEffectRadius
	{
		get
		{
			return (double)GetValue(RectangleEffectRadiusProperty);
		}
		set
		{
			SetValue(RectangleEffectRadiusProperty, value);
		}
	}

	public bool IsLoading
	{
		get
		{
			return (bool)GetValue(IsLoadingProperty);
		}
		set
		{
			SetValue(IsLoadingProperty, value);
		}
	}
}
