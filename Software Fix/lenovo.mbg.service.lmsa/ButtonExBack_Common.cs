using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa;

public partial class ButtonExBack_Common : UserControl, IComponentConnector
{
	private SolidColorBrush BgEnable = new SolidColorBrush(Colors.White);

	private SolidColorBrush BgUnEnable = new SolidColorBrush(Color.FromRgb(127, 127, 127));

	public static readonly DependencyProperty ButtonEnableProperty = DependencyProperty.Register("ButtonEnable", typeof(bool), typeof(ButtonExBack_Common), new PropertyMetadata(true));

	public static readonly DependencyProperty ButtonHeightProperty = DependencyProperty.Register("ButtonHeight", typeof(double), typeof(ButtonExBack_Common), new PropertyMetadata(60.0));

	public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register("ButtonWidth", typeof(double), typeof(ButtonExBack_Common), new PropertyMetadata(200.0));

	public static readonly DependencyProperty CronRadioProperty = DependencyProperty.Register("CronRadio", typeof(double), typeof(ButtonExBack_Common), new PropertyMetadata(30.0));

	public static readonly DependencyProperty StrButtonTitleProperty = DependencyProperty.Register("StrButtonTitle", typeof(string), typeof(ButtonExBack_Common), new PropertyMetadata(string.Empty));

	public bool ButtonEnable
	{
		get
		{
			return (bool)GetValue(ButtonEnableProperty);
		}
		set
		{
			SetValue(ButtonEnableProperty, value);
			if (value)
			{
				MainBorder.Background = BgEnable;
			}
			else
			{
				MainBorder.Background = BgUnEnable;
			}
		}
	}

	public double ButtonHeight
	{
		get
		{
			return (double)GetValue(ButtonHeightProperty);
		}
		set
		{
			SetValue(ButtonHeightProperty, value);
			CronRadio = ButtonWidth / 2.0;
		}
	}

	public double ButtonWidth
	{
		get
		{
			return (double)GetValue(ButtonWidthProperty);
		}
		set
		{
			SetValue(ButtonWidthProperty, value);
		}
	}

	public double CronRadio
	{
		get
		{
			return (double)GetValue(CronRadioProperty);
		}
		set
		{
			SetValue(CronRadioProperty, value);
		}
	}

	public string StrButtonTitle
	{
		get
		{
			return (string)GetValue(StrButtonTitleProperty);
		}
		set
		{
			SetValue(StrButtonTitleProperty, value);
		}
	}

	public event EventHandler ButtonClick;

	public ButtonExBack_Common()
	{
		InitializeComponent();
		InitializeResource();
	}

	private void InitializeResource()
	{
		MainBorder.MouseDown += MainBorder_MouseDown;
		MainBorder.MouseUp += MainBorder_MouseUp;
	}

	private void MainBorder_MouseUp(object sender, MouseButtonEventArgs e)
	{
		if (ButtonEnable)
		{
			MainBorder.BorderThickness = new Thickness(3.0);
			ShandowBorder.Visibility = Visibility.Visible;
		}
	}

	private void MainBorder_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (ButtonEnable)
		{
			MainBorder.BorderThickness = new Thickness(0.0);
			ShandowBorder.Visibility = Visibility.Collapsed;
			if (this.ButtonClick != null)
			{
				this.ButtonClick(this, e);
			}
			ShandowBorder.Visibility = Visibility.Visible;
		}
	}
}
