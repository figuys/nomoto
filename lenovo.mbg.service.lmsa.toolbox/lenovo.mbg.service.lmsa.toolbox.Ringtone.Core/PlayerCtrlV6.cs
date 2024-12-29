using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.Core;

public class PlayerCtrlV6 : Control
{
	public static readonly DependencyProperty ItemsSourceProperty;

	public static readonly DependencyProperty BarNormalBrushProperty;

	public static readonly DependencyProperty BarHoverBrushProperty;

	public static readonly DependencyProperty IsPlayProperty;

	public static readonly DependencyProperty StartValueProperty;

	public static readonly DependencyProperty EndValueProperty;

	public static readonly DependencyProperty CurrentProperty;

	public static readonly DependencyProperty MaxValueProperty;

	private double maxYAxis;

	private Grid barContent;

	private Border leftRegion;

	private Border rightRegion;

	private Border realRegion;

	private Border realProgress;

	private Border timeLine;

	private Button leftButton;

	private Button rightButton;

	private Button realButton;

	private Label realText;

	private Label startText;

	private Label endText;

	private Button moveBtn;

	private Point start;

	private MouseButtonEventArgs autoEventArgs;

	private double physicStart;

	private double physicEnd;

	private double physicCur;

	public IEnumerable<double> ItemsSource
	{
		get
		{
			return (IEnumerable<double>)GetValue(ItemsSourceProperty);
		}
		set
		{
			SetValue(ItemsSourceProperty, value);
		}
	}

	public Brush BarNormalBrush
	{
		get
		{
			return (Brush)GetValue(BarNormalBrushProperty);
		}
		set
		{
			SetValue(BarNormalBrushProperty, value);
		}
	}

	public Brush BarHoverBrush
	{
		get
		{
			return (Brush)GetValue(BarHoverBrushProperty);
		}
		set
		{
			SetValue(BarHoverBrushProperty, value);
		}
	}

	public bool IsPlay
	{
		get
		{
			return (bool)GetValue(IsPlayProperty);
		}
		set
		{
			SetValue(IsPlayProperty, value);
		}
	}

	public double StartValue
	{
		get
		{
			return (double)GetValue(StartValueProperty);
		}
		set
		{
			SetValue(StartValueProperty, value);
		}
	}

	public double EndValue
	{
		get
		{
			return (double)GetValue(EndValueProperty);
		}
		set
		{
			SetValue(EndValueProperty, value);
		}
	}

	public double Current
	{
		get
		{
			return (double)GetValue(CurrentProperty);
		}
		set
		{
			SetValue(CurrentProperty, value);
		}
	}

	public double MaxValue
	{
		get
		{
			return (double)GetValue(MaxValueProperty);
		}
		set
		{
			SetValue(MaxValueProperty, value);
		}
	}

	public Action<double> StartChangedEvent { get; set; }

	public Action<double> EndChangedEvent { get; set; }

	private double PhysicCur
	{
		get
		{
			return physicCur;
		}
		set
		{
			if (physicCur != value)
			{
				if (value < 0.0)
				{
					physicCur = 0.0;
				}
				else if (value > timeLine.ActualWidth)
				{
					physicCur = timeLine.ActualWidth;
				}
				else
				{
					physicCur = value;
				}
				realRegion.Width = physicCur + 3.0;
				realButton.Margin = new Thickness(physicCur - 7.0, -5.0, -5.0, -5.0);
				realText.Margin = new Thickness(physicCur - 38.0, 0.0, -38.0, 3.0);
				int num = (int)Current;
				realText.Content = $"{num / 3600:00}:{num % 3600 / 60:00}:{num % 60:00}";
			}
		}
	}

	private double PhysicStart
	{
		get
		{
			return physicStart;
		}
		set
		{
			if (physicStart != value)
			{
				if (value < 0.0)
				{
					physicStart = 0.0;
				}
				else if (value > timeLine.ActualWidth)
				{
					physicStart = timeLine.ActualWidth;
				}
				else
				{
					physicStart = value;
				}
				leftButton.Margin = new Thickness(physicStart - 8.0, -5.0, -5.0, -5.0);
				leftRegion.Width = physicStart + 3.0;
				StartValue = MaxValue * PhysicStart / timeLine.ActualWidth;
				int num = (int)StartValue;
				startText.Margin = new Thickness(physicStart - 45.0, 0.0, -45.0, 0.0);
				startText.Content = $"{num / 3600:00}:{num % 3600 / 60:00}:{num % 60:00}";
			}
		}
	}

	private double PhysicEnd
	{
		get
		{
			return physicEnd;
		}
		set
		{
			if (physicEnd != value)
			{
				if (value < 0.0)
				{
					physicEnd = 0.0;
				}
				else if (value > timeLine.ActualWidth)
				{
					physicEnd = timeLine.ActualWidth;
				}
				else
				{
					physicEnd = value;
				}
				rightButton.Margin = new Thickness(-5.0, -5.0, timeLine.ActualWidth - physicEnd - 8.0, -5.0);
				rightRegion.Width = timeLine.ActualWidth - physicEnd + 3.0;
				EndValue = MaxValue * PhysicEnd / timeLine.ActualWidth;
				int num = (int)EndValue;
				endText.Margin = new Thickness(physicEnd - 45.0, 0.0, -45.0, 0.0);
				endText.Content = $"{num / 3600:00}:{num % 3600 / 60:00}:{num % 60:00}";
			}
		}
	}

	static PlayerCtrlV6()
	{
		ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<double>), typeof(PlayerCtrlV6), new PropertyMetadata(new List<double>(), OnItemsSourceChanged));
		BarNormalBrushProperty = DependencyProperty.Register("BarNormalBrush", typeof(Brush), typeof(PlayerCtrlV6), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF96A6B8"))));
		BarHoverBrushProperty = DependencyProperty.Register("BarHoverBrush", typeof(Brush), typeof(PlayerCtrlV6), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF96A6B8"))));
		IsPlayProperty = DependencyProperty.Register("IsPlay", typeof(bool), typeof(PlayerCtrlV6), new PropertyMetadata(false, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			PlayerCtrlV6 playerCtrlV = sender as PlayerCtrlV6;
			if (playerCtrlV.leftRegion != null)
			{
				if ((bool)e.NewValue)
				{
					playerCtrlV.RemoveBtnEvent(playerCtrlV.realButton);
				}
				else
				{
					playerCtrlV.SetBtnEvent(playerCtrlV.realButton);
				}
			}
		}));
		StartValueProperty = DependencyProperty.Register("StartValue", typeof(double), typeof(PlayerCtrlV6), new PropertyMetadata(0.0, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			PlayerCtrlV6 playerCtrlV2 = sender as PlayerCtrlV6;
			if (playerCtrlV2.leftRegion != null)
			{
				if (playerCtrlV2.StartChangedEvent != null && e.NewValue != e.OldValue)
				{
					playerCtrlV2.StartChangedEvent((double)e.NewValue);
				}
				playerCtrlV2.PhysicStart = (double)e.NewValue * playerCtrlV2.timeLine.ActualWidth / playerCtrlV2.MaxValue;
			}
		}));
		EndValueProperty = DependencyProperty.Register("EndValue", typeof(double), typeof(PlayerCtrlV6), new PropertyMetadata(120.0, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			PlayerCtrlV6 playerCtrlV3 = sender as PlayerCtrlV6;
			if (playerCtrlV3.leftRegion != null)
			{
				if (playerCtrlV3.EndChangedEvent != null && e.NewValue != e.OldValue)
				{
					playerCtrlV3.EndChangedEvent((double)e.NewValue);
				}
				playerCtrlV3.PhysicEnd = (double)e.NewValue * playerCtrlV3.timeLine.ActualWidth / playerCtrlV3.MaxValue;
			}
		}));
		CurrentProperty = DependencyProperty.Register("Current", typeof(double), typeof(PlayerCtrlV6), new PropertyMetadata(0.0, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			PlayerCtrlV6 playerCtrlV4 = sender as PlayerCtrlV6;
			if (playerCtrlV4.leftRegion != null)
			{
				playerCtrlV4.PhysicCur = (double)e.NewValue * playerCtrlV4.timeLine.ActualWidth / playerCtrlV4.MaxValue;
			}
		}));
		MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(PlayerCtrlV6), new PropertyMetadata(120.0, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			PlayerCtrlV6 playerCtrlV5 = sender as PlayerCtrlV6;
			if (playerCtrlV5.leftRegion != null)
			{
				playerCtrlV5.Current = 0.0;
				playerCtrlV5.StartValue = 0.0;
				playerCtrlV5.EndValue = (double)e.NewValue;
				int num = (int)playerCtrlV5.EndValue;
				playerCtrlV5.endText.Content = $"{num / 3600:00}:{num % 3600 / 60:00}:{num % 60:00}";
			}
		}));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PlayerCtrlV6), new FrameworkPropertyMetadata(typeof(PlayerCtrlV6)));
	}

	private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		PlayerCtrlV6 playerCtrlV = d as PlayerCtrlV6;
		if (playerCtrlV.leftRegion != null)
		{
			playerCtrlV.DrawBars();
		}
	}

	public PlayerCtrlV6()
	{
		base.SizeChanged += delegate
		{
			PhysicStart = 0.0;
			PhysicEnd = timeLine.ActualWidth;
			DrawBars();
		};
		base.MouseMove += PlayerCtrl_MouseMove;
		base.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource is Grid && (e.OriginalSource as Grid).Tag is Button)
			{
				start = e.GetPosition(this);
				moveBtn = (e.OriginalSource as Grid).Tag as Button;
				if (moveBtn.Name == leftButton.Name)
				{
					Panel.SetZIndex(startText, 8080);
					Panel.SetZIndex(endText, 0);
				}
				else if (moveBtn.Name == rightButton.Name)
				{
					Panel.SetZIndex(startText, 0);
					Panel.SetZIndex(endText, 8080);
				}
			}
			else
			{
				moveBtn = null;
			}
		};
		base.MouseLeftButtonUp += delegate(object sender, MouseButtonEventArgs e)
		{
			moveBtn = null;
			e.Handled = true;
		};
		autoEventArgs = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
		autoEventArgs.RoutedEvent = UIElement.MouseLeftButtonUp;
	}

	private void PlayerCtrl_MouseMove(object sender, MouseEventArgs e)
	{
		_ = timeLine.ActualWidth;
		e.Handled = true;
		if (e.LeftButton != MouseButtonState.Pressed || moveBtn == null)
		{
			return;
		}
		Point position = e.GetPosition(this);
		double num = position.X - start.X;
		if (num == 0.0)
		{
			start = position;
			return;
		}
		if (moveBtn.Name == leftButton.Name)
		{
			if (physicStart + num > physicEnd)
			{
				moveBtn = rightButton;
				double num2 = physicStart + num;
				PhysicStart = physicEnd;
				PhysicEnd = num2;
				Panel.SetZIndex(startText, 0);
				Panel.SetZIndex(endText, 8080);
			}
			else
			{
				PhysicStart += num;
			}
		}
		else if (moveBtn.Name == rightButton.Name)
		{
			if (physicEnd + num < physicStart)
			{
				moveBtn = leftButton;
				double num3 = physicEnd + num;
				PhysicEnd = physicStart;
				PhysicStart = num3;
				Panel.SetZIndex(startText, 8080);
				Panel.SetZIndex(endText, 0);
			}
			else
			{
				PhysicEnd += num;
			}
		}
		else if (moveBtn.Name == realButton.Name)
		{
			double num4 = physicCur;
			num4 = ((num4 + num < PhysicStart) ? PhysicStart : ((!(num4 + num > PhysicEnd)) ? (num4 + num) : PhysicEnd));
			Current = num4 * MaxValue / timeLine.ActualWidth;
		}
		start = position;
	}

	public override void OnApplyTemplate()
	{
		barContent = base.Template.FindName("PART_barcontent", this) as Grid;
		leftRegion = base.Template.FindName("PART_LeftRegion", this) as Border;
		rightRegion = base.Template.FindName("PART_RightRegion", this) as Border;
		realRegion = base.Template.FindName("PART_RealData", this) as Border;
		realProgress = base.Template.FindName("PART_ProReal", this) as Border;
		timeLine = base.Template.FindName("PART_TimeLine", this) as Border;
		leftButton = base.Template.FindName("PART_BtnLeft", this) as Button;
		rightButton = base.Template.FindName("PART_BtnRight", this) as Button;
		realButton = base.Template.FindName("PART_BtnReal", this) as Button;
		realText = base.Template.FindName("PART_RealText", this) as Label;
		startText = base.Template.FindName("PART_StartText", this) as Label;
		endText = base.Template.FindName("PART_EndText", this) as Label;
		SetBtnEvent(leftButton);
		SetBtnEvent(rightButton);
		SetBtnEvent(realButton);
		base.OnApplyTemplate();
	}

	private void DrawBars()
	{
		maxYAxis = GetMaxOfYAxis();
		barContent.ColumnDefinitions.Clear();
		barContent.RowDefinitions.Clear();
		barContent.Children.Clear();
		double actualHeight = barContent.ActualHeight;
		double actualWidth = barContent.ActualWidth;
		foreach (double item in ItemsSource)
		{
			Rectangle rectangle = new Rectangle
			{
				Fill = BarNormalBrush,
				Height = actualHeight * (item / maxYAxis),
				Width = actualWidth / (double)ItemsSource.Count() * 0.6,
				VerticalAlignment = VerticalAlignment.Center
			};
			barContent.Children.Add(rectangle);
			DoubleAnimation animation = new DoubleAnimation(0.0, actualHeight * (item / maxYAxis), new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
			rectangle.BeginAnimation(FrameworkElement.HeightProperty, animation);
			rectangle.Tag = item;
			rectangle.MouseEnter += delegate
			{
			};
			rectangle.MouseLeave += delegate
			{
			};
			barContent.ColumnDefinitions.Add(new ColumnDefinition());
			Grid.SetColumn(rectangle, barContent.ColumnDefinitions.Count - 1);
		}
	}

	private double GetMaxOfYAxis()
	{
		if (ItemsSource == null || ItemsSource.Count() == 0)
		{
			return 200.0;
		}
		double num = ItemsSource.Max((double p) => p);
		int num2 = 1;
		if (num > 1.0)
		{
			int num3 = 2;
			while ((double)num2 < num)
			{
				num2 = 5 * num3;
				num3 += 2;
			}
			num2 /= 10;
		}
		return num2 * 11 * 4 / 3;
	}

	private void SetBtnEvent(Button btn)
	{
		btn.AddHandler(UIElement.MouseMove, new MouseEventHandler(OnMouseMoveEvent), handledEventsToo: true);
		btn.AddHandler(UIElement.MouseLeftButtonDown, new MouseButtonEventHandler(OnLeftButtonEvent), handledEventsToo: true);
		btn.AddHandler(UIElement.MouseLeftButtonUp, new MouseButtonEventHandler(OnLeftButtonEvent), handledEventsToo: true);
	}

	private void RemoveBtnEvent(Button btn)
	{
		btn.RemoveHandler(UIElement.MouseMove, new MouseEventHandler(OnMouseMoveEvent));
		btn.RemoveHandler(UIElement.MouseLeftButtonDown, new MouseButtonEventHandler(OnLeftButtonEvent));
		btn.RemoveHandler(UIElement.MouseLeftButtonUp, new MouseButtonEventHandler(OnLeftButtonEvent));
	}

	private void OnMouseMoveEvent(object sender, MouseEventArgs e)
	{
		e.Handled = false;
	}

	private void OnLeftButtonEvent(object sender, MouseButtonEventArgs e)
	{
		e.Handled = false;
	}
}
