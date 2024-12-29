using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace lenovo.themes.generic.Controls;

public partial class BackAndForthProgressBar : UserControl, IComponentConnector
{
	private int speed = 50;

	private Thickness currentFrom;

	public static readonly DependencyProperty BlockBrushProperty = DependencyProperty.Register("BlockBrush", typeof(Brush), typeof(BackAndForthProgressBar), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#46B5E5"))));

	public static readonly DependencyProperty BlockWidthProperty = DependencyProperty.Register("BlockWidth", typeof(double), typeof(BackAndForthProgressBar), new PropertyMetadata(10.0));

	public Storyboard storyboard { get; set; }

	public Brush BlockBrush
	{
		get
		{
			return (Brush)GetValue(BlockBrushProperty);
		}
		set
		{
			SetValue(BlockBrushProperty, value);
		}
	}

	public double BlockWidth
	{
		get
		{
			return (double)GetValue(BlockWidthProperty);
		}
		set
		{
			SetValue(BlockWidthProperty, value);
		}
	}

	public BackAndForthProgressBar()
	{
		InitializeComponent();
		base.SizeChanged += BackAndForthProgressBar_SizeChanged;
	}

	private void BackAndForthProgressBar_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (e.PreviousSize.Width != e.NewSize.Width)
		{
			currentFrom.Left = rectangle.Margin.Left;
			updateStoryboard();
		}
	}

	private void updateStoryboard()
	{
		if (storyboard != null)
		{
			storyboard.Stop();
			storyboard.Remove(rectangle);
		}
		storyboard = new Storyboard();
		double num = base.ActualWidth - rectangle.Width;
		int num2 = (int)num / speed;
		ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
		thicknessAnimation.From = currentFrom;
		thicknessAnimation.To = new Thickness(num, 0.0, 0.0, 0.0);
		thicknessAnimation.Duration = new Duration(new TimeSpan(0, 0, (num2 == 0) ? 5 : num2));
		thicknessAnimation.AutoReverse = true;
		thicknessAnimation.RepeatBehavior = RepeatBehavior.Forever;
		storyboard.Children.Add(thicknessAnimation);
		Storyboard.SetTarget(thicknessAnimation, rectangle);
		Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath(FrameworkElement.MarginProperty));
		Style style = new Style(typeof(Rectangle));
		Trigger trigger = new Trigger();
		trigger.Property = UIElement.VisibilityProperty;
		trigger.Value = Visibility.Visible;
		trigger.EnterActions.Add(new BeginStoryboard
		{
			Name = "_progressbar_",
			Storyboard = storyboard
		});
		trigger.ExitActions.Add(new StopStoryboard
		{
			BeginStoryboardName = "_progressbar_"
		});
		style.Triggers.Add(trigger);
		rectangle.Style = style;
	}
}
