using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lenovo.themes.generic.Controls;

public class ProgressBarCtrl : Control
{
	public static DependencyProperty PercentProperty;

	private Border border;

	private Border progress;

	private TextBlock txt;

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

	private static void OnPercetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		ProgressBarCtrl progressBarCtrl = d as ProgressBarCtrl;
		if (progressBarCtrl.progress != null)
		{
			progressBarCtrl.Update();
		}
	}

	static ProgressBarCtrl()
	{
		PercentProperty = DependencyProperty.Register("Percent", typeof(double), typeof(ProgressBarCtrl), new PropertyMetadata(0.0, OnPercetChanged));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressBarCtrl), new FrameworkPropertyMetadata(typeof(ProgressBarCtrl)));
	}

	public override void OnApplyTemplate()
	{
		border = base.Template.FindName("PART_Border", this) as Border;
		progress = base.Template.FindName("PART_Progress", this) as Border;
		txt = base.Template.FindName("PART_Text", this) as TextBlock;
		Update();
		base.OnApplyTemplate();
	}

	private void Update()
	{
		progress.Width = Percent * border.ActualWidth / 100.0;
		txt.Text = $"{Percent}%";
		if (base.ActualWidth != 0.0)
		{
			FormattedText formattedText = new FormattedText(txt.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(txt.FontFamily, txt.FontStyle, txt.FontWeight, txt.FontStretch), txt.FontSize, txt.Foreground, VisualTreeHelper.GetDpi(txt).PixelsPerDip);
			double left = (base.ActualWidth - formattedText.Width) * Percent / 100.0;
			txt.Margin = new Thickness(left, 5.0, 0.0, 5.0);
		}
	}
}
