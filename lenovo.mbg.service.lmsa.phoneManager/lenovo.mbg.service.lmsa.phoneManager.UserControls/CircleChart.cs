using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using lenovo.mbg.service.common.utilities;
using Microsoft.Expression.Shapes;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public class CircleChart : Control
{
	private Arc arcUse;

	private TextBlock txtFree;

	private TextBlock txtUse;

	private double curAngle;

	public static DependencyProperty ArcThicknessProperty;

	public static DependencyProperty PercentProperty;

	public static DependencyProperty TotalProperty;

	private Storyboard story;

	public double Percent
	{
		get
		{
			return (double)GetValue(PercentProperty);
		}
		set
		{
			SetValue(PercentProperty, value);
		}
	}

	public double Total
	{
		get
		{
			return (double)GetValue(TotalProperty);
		}
		set
		{
			SetValue(TotalProperty, value);
		}
	}

	public double ArcThickness
	{
		get
		{
			return (double)GetValue(ArcThicknessProperty);
		}
		set
		{
			SetValue(ArcThicknessProperty, value);
		}
	}

	static CircleChart()
	{
		ArcThicknessProperty = DependencyProperty.Register("ArcThickness", typeof(double), typeof(CircleChart), new PropertyMetadata(30.0));
		PercentProperty = DependencyProperty.Register("Percent", typeof(double), typeof(CircleChart), new PropertyMetadata(0.0, OnPercetChanged));
		TotalProperty = DependencyProperty.Register("Total", typeof(double), typeof(CircleChart), new PropertyMetadata(100.0, OnTotalChanged));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleChart), new FrameworkPropertyMetadata(typeof(CircleChart)));
	}

	private static void OnPercetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		CircleChart circleChart = d as CircleChart;
		if (circleChart.arcUse != null)
		{
			circleChart.Update();
		}
	}

	private static void OnTotalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		CircleChart circleChart = d as CircleChart;
		if (circleChart.arcUse != null)
		{
			circleChart.Update();
		}
	}

	public CircleChart()
	{
		curAngle = 0.0;
		story = new Storyboard();
		story.Completed += delegate
		{
			story.Children.Clear();
		};
		base.SizeChanged += delegate
		{
			Update();
		};
	}

	public override void OnApplyTemplate()
	{
		arcUse = base.Template.FindName("PART_Arc_Use", this) as Arc;
		txtUse = base.Template.FindName("PART_Txt_Use", this) as TextBlock;
		txtFree = base.Template.FindName("PART_Txt_Free", this) as TextBlock;
		Update();
		base.OnApplyTemplate();
	}

	public void Update()
	{
		FrameworkElement frameworkElement = base.Parent as FrameworkElement;
		int num = 25;
		if (frameworkElement.ActualHeight != 0.0)
		{
			double num2 = frameworkElement.ActualHeight - 100.0;
			double num3 = frameworkElement.ActualWidth - 60.0;
			if (num2 > num3)
			{
				double num4 = (num2 - num3) / 2.0;
				base.Margin = new Thickness(num, (double)num + num4, num, (double)num + num4);
			}
			else if (num2 < num3)
			{
				double num5 = (num3 - num2) / 2.0;
				base.Margin = new Thickness((double)num + num5, num, (double)num + num5, num);
			}
		}
		double num6 = Percent * 360.0 / Total;
		if (curAngle != num6)
		{
			curAngle = num6;
			if (double.IsNaN(curAngle))
			{
				curAngle = 0.0;
			}
			Animation(curAngle);
		}
		_ = arcUse.ActualWidth / 2.0 + 10.0;
		Math.Sin(Angle2Pi(num6 / 2.0));
		Math.Cos(Angle2Pi(num6 / 2.0));
		txtUse.Text = GlobalFun.ConvertLong2String((long)Percent);
		txtFree.Text = GlobalFun.ConvertLong2String((long)(Total - Percent));
	}

	private void Animation(double target)
	{
		if (!double.IsInfinity(target))
		{
			DoubleAnimation doubleAnimation = new DoubleAnimation();
			doubleAnimation.From = 0.0;
			doubleAnimation.To = target;
			doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
			Storyboard.SetTargetName(doubleAnimation, arcUse.Name);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(Arc.EndAngleProperty));
			story.Children.Add(doubleAnimation);
			story.Begin(arcUse);
		}
	}

	private double Angle2Pi(double angle)
	{
		return Math.PI * 2.0 * angle / 360.0;
	}
}
