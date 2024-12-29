using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Microsoft.Expression.Shapes;

namespace lenovo.themes.generic.ControlsV6;

public partial class CircleProgress : UserControl, IComponentConnector
{
	private Storyboard story;

	private Arc arcUse;

	private double curAngle;

	public static readonly DependencyProperty ArcThicknessProperty = DependencyProperty.Register("ArcThickness", typeof(double), typeof(CircleProgress), new PropertyMetadata(10.0));

	public static readonly DependencyProperty TotalProperty = DependencyProperty.Register("Total", typeof(double), typeof(CircleProgress), new PropertyMetadata(100.0, FireChanged));

	public static readonly DependencyProperty PercentProperty = DependencyProperty.Register("Percent", typeof(double), typeof(CircleProgress), new PropertyMetadata(0.0, FireChanged));

	public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(CircleProgress), new PropertyMetadata(null));

	public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register("SubTitle", typeof(string), typeof(CircleProgress), new PropertyMetadata(null));

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

	public string Title
	{
		get
		{
			return (string)GetValue(TitleProperty);
		}
		set
		{
			SetValue(TitleProperty, value);
		}
	}

	public string SubTitle
	{
		get
		{
			return (string)GetValue(SubTitleProperty);
		}
		set
		{
			SetValue(SubTitleProperty, value);
		}
	}

	public CircleProgress()
	{
		InitializeComponent();
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
		Update();
		base.OnApplyTemplate();
	}

	private static void FireChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		CircleProgress circleProgress = d as CircleProgress;
		if (circleProgress.arcUse != null)
		{
			circleProgress.Update();
		}
	}

	public void Update()
	{
		double num = Percent * 360.0 / Total;
		if (curAngle != num)
		{
			curAngle = num;
			if (double.IsNaN(curAngle))
			{
				curAngle = 0.0;
			}
			Animation(curAngle);
		}
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
}
