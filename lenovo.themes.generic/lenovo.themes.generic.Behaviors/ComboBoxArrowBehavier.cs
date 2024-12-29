using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace lenovo.themes.generic.Behaviors;

internal class ComboBoxArrowBehavier
{
	public static readonly DependencyProperty AllowsAnimationProperty = DependencyProperty.RegisterAttached("AllowsAnimation", typeof(bool), typeof(ComboBoxArrowBehavier), new FrameworkPropertyMetadata(false, AllowsAnimationChanged));

	private static DoubleAnimation RotateAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromMilliseconds(200.0)));

	public static bool GetAllowsAnimation(DependencyObject d)
	{
		return (bool)d.GetValue(AllowsAnimationProperty);
	}

	public static void SetAllowsAnimation(DependencyObject obj, bool value)
	{
		obj.SetValue(AllowsAnimationProperty, value);
	}

	private static void AllowsAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is FrameworkElement frameworkElement)
		{
			if (frameworkElement.RenderTransformOrigin == new Point(0.0, 0.0))
			{
				frameworkElement.RenderTransformOrigin = new Point(0.5, 0.5);
				RotateTransform renderTransform = new RotateTransform(0.0);
				frameworkElement.RenderTransform = renderTransform;
			}
			if ((bool)e.NewValue)
			{
				RotateAnimation.To = 180.0;
				frameworkElement.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
			}
			else
			{
				RotateAnimation.To = 0.0;
				frameworkElement.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
			}
		}
	}
}
