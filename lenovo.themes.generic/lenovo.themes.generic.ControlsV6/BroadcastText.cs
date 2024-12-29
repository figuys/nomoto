using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace lenovo.themes.generic.ControlsV6;

[ContentProperty("Text")]
[TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas))]
[TemplatePart(Name = "PART_Txt1", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_Txt2", Type = typeof(TextBlock))]
public class BroadcastText : Control
{
	public static readonly DependencyProperty TextProperty;

	public static readonly DependencyProperty SpaceProperty;

	public static readonly DependencyProperty SpeedProperty;

	public static readonly DependencyProperty DirectionProperty;

	private Canvas _canvas;

	private TextBlock _txt1;

	private TextBlock _txt2;

	private Action _delayUpdate;

	private Storyboard _storyboard;

	public string Text
	{
		get
		{
			return (string)GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

	public double Space
	{
		get
		{
			return (double)GetValue(SpaceProperty);
		}
		set
		{
			SetValue(SpaceProperty, value);
		}
	}

	public double Speed
	{
		get
		{
			return (double)GetValue(SpeedProperty);
		}
		set
		{
			SetValue(SpeedProperty, value);
		}
	}

	public BroadcastDirection Direction
	{
		get
		{
			return (BroadcastDirection)GetValue(DirectionProperty);
		}
		set
		{
			SetValue(DirectionProperty, value);
		}
	}

	static BroadcastText()
	{
		TextProperty = TextBlock.TextProperty.AddOwner(typeof(BroadcastText));
		SpaceProperty = DependencyProperty.Register("Space", typeof(double), typeof(BroadcastText), new PropertyMetadata(double.NaN, OnSpacePropertyChanged));
		SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(BroadcastText), new PropertyMetadata(120.0, OnSpeedPropertyChanged));
		DirectionProperty = DependencyProperty.Register("Direction", typeof(BroadcastDirection), typeof(BroadcastText), new PropertyMetadata(BroadcastDirection.RightToLeft, OnDirectionPropertyChanged));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(BroadcastText), new FrameworkPropertyMetadata(typeof(BroadcastText)));
	}

	public BroadcastText()
	{
		base.IsVisibleChanged += delegate
		{
			BeginUpdate();
		};
	}

	private static void OnSpacePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((BroadcastText)d).BeginUpdate();
	}

	private static void OnSpeedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((BroadcastText)d).BeginUpdate();
	}

	private static void OnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((BroadcastText)d).BeginUpdate();
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		if (_canvas != null)
		{
			_canvas.SizeChanged -= OnSizeChanged;
		}
		if (_txt1 != null)
		{
			_txt1.SizeChanged -= OnSizeChanged;
		}
		_canvas = base.Template.FindName("PART_Canvas", this) as Canvas;
		_txt1 = base.Template.FindName("PART_Txt1", this) as TextBlock;
		_txt2 = base.Template.FindName("PART_Txt2", this) as TextBlock;
		if (_canvas != null && _txt1 != null && _txt2 != null)
		{
			_txt1.SizeChanged += OnSizeChanged;
			_canvas.SizeChanged += OnSizeChanged;
		}
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs e)
	{
		BeginUpdate();
	}

	private void BeginUpdate()
	{
		_delayUpdate = Update;
		base.Dispatcher.InvokeAsync(delegate
		{
			if (_delayUpdate != null)
			{
				_delayUpdate();
				_delayUpdate = null;
			}
		}, DispatcherPriority.Loaded);
	}

	private void Update()
	{
		if (_storyboard != null)
		{
			_storyboard.Stop();
			_storyboard.Remove();
			_storyboard = null;
		}
		if (_canvas != null && _txt1 != null && _txt2 != null && !string.IsNullOrEmpty(Text) && base.IsVisible)
		{
			switch (Direction)
			{
			case BroadcastDirection.RightToLeft:
				UpdateRightToLeft();
				break;
			case BroadcastDirection.LeftToRight:
				UpdateLeftToRight();
				break;
			case BroadcastDirection.BottomToTop:
				UpdateBottomToTop();
				break;
			case BroadcastDirection.TopToBottom:
				UpdateTopToBottom();
				break;
			}
		}
	}

	private void UpdateRightToLeft()
	{
		GetHorizontal(out var from, out var to, out var len);
		UpdateHorizontal(to, from, len);
	}

	private void UpdateLeftToRight()
	{
		GetHorizontal(out var from, out var to, out var len);
		UpdateHorizontal(from, to, len);
	}

	private void UpdateBottomToTop()
	{
		GetVertical(out var from, out var to, out var len);
		UpdateVertical(to, from, len);
	}

	private void UpdateTopToBottom()
	{
		GetVertical(out var from, out var to, out var len);
		UpdateVertical(from, to, len);
	}

	private void GetHorizontal(out double from, out double to, out double len)
	{
		double actualWidth = _canvas.ActualWidth;
		double actualWidth2 = _txt1.ActualWidth;
		from = 0.0 - actualWidth2;
		to = actualWidth;
		if (double.IsNaN(Space) || Space < 0.0)
		{
			len = ((actualWidth2 < actualWidth) ? actualWidth : (actualWidth2 + actualWidth));
		}
		else
		{
			len = ((actualWidth2 < actualWidth - Space) ? actualWidth : (actualWidth2 + Space));
		}
	}

	private void UpdateHorizontal(double from, double to, double len)
	{
		Canvas.SetLeft(_txt1, from);
		Canvas.SetLeft(_txt2, from);
		_txt1.SetCurrentValue(Canvas.LeftProperty, from);
		_txt2.SetCurrentValue(Canvas.LeftProperty, from);
		TimeSpan timeSpan = TimeSpan.FromSeconds(len / Speed);
		TimeSpan timeSpan2 = TimeSpan.FromSeconds(Math.Abs(from - to) / Speed);
		TimeSpan timeSpan3 = timeSpan + timeSpan;
		_storyboard = new Storyboard();
		DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames
		{
			Duration = timeSpan3,
			RepeatBehavior = RepeatBehavior.Forever
		};
		doubleAnimationUsingKeyFrames.KeyFrames.Add(new DiscreteDoubleKeyFrame(from, TimeSpan.FromSeconds(0.0)));
		doubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame(to, timeSpan2));
		Storyboard.SetTarget(doubleAnimationUsingKeyFrames, _txt1);
		Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath(Canvas.LeftProperty));
		_storyboard.Children.Add(doubleAnimationUsingKeyFrames);
		DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames2 = new DoubleAnimationUsingKeyFrames
		{
			BeginTime = timeSpan,
			Duration = timeSpan3,
			RepeatBehavior = RepeatBehavior.Forever
		};
		doubleAnimationUsingKeyFrames2.KeyFrames.Add(new DiscreteDoubleKeyFrame(from, TimeSpan.FromSeconds(0.0)));
		doubleAnimationUsingKeyFrames2.KeyFrames.Add(new LinearDoubleKeyFrame(to, timeSpan2));
		Storyboard.SetTarget(doubleAnimationUsingKeyFrames2, _txt2);
		Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames2, new PropertyPath(Canvas.LeftProperty));
		_storyboard.Children.Add(doubleAnimationUsingKeyFrames2);
		_txt1.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);
		_txt2.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);
		ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames = new ObjectAnimationUsingKeyFrames
		{
			Duration = timeSpan3,
			RepeatBehavior = RepeatBehavior.Forever
		};
		objectAnimationUsingKeyFrames.KeyFrames.Add(new DiscreteObjectKeyFrame
		{
			Value = Visibility.Visible,
			KeyTime = TimeSpan.FromSeconds(0.0)
		});
		objectAnimationUsingKeyFrames.KeyFrames.Add(new DiscreteObjectKeyFrame
		{
			Value = Visibility.Hidden,
			KeyTime = timeSpan2
		});
		Storyboard.SetTarget(objectAnimationUsingKeyFrames, _txt1);
		Storyboard.SetTargetProperty(objectAnimationUsingKeyFrames, new PropertyPath(UIElement.VisibilityProperty));
		_storyboard.Children.Add(objectAnimationUsingKeyFrames);
		ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames2 = new ObjectAnimationUsingKeyFrames
		{
			BeginTime = timeSpan,
			Duration = timeSpan3,
			RepeatBehavior = RepeatBehavior.Forever
		};
		objectAnimationUsingKeyFrames2.KeyFrames.Add(new DiscreteObjectKeyFrame
		{
			Value = Visibility.Visible,
			KeyTime = TimeSpan.FromSeconds(0.0)
		});
		objectAnimationUsingKeyFrames2.KeyFrames.Add(new DiscreteObjectKeyFrame
		{
			Value = Visibility.Hidden,
			KeyTime = timeSpan2
		});
		Storyboard.SetTarget(objectAnimationUsingKeyFrames2, _txt2);
		Storyboard.SetTargetProperty(objectAnimationUsingKeyFrames2, new PropertyPath(UIElement.VisibilityProperty));
		_storyboard.Children.Add(objectAnimationUsingKeyFrames2);
		_storyboard.Begin();
	}

	private void GetVertical(out double from, out double to, out double len)
	{
		double actualHeight = _canvas.ActualHeight;
		double actualHeight2 = _txt1.ActualHeight;
		from = 0.0 - actualHeight2;
		to = actualHeight;
		if (double.IsNaN(Space) || Space < 0.0)
		{
			len = ((actualHeight2 < actualHeight) ? actualHeight : (actualHeight2 + actualHeight));
		}
		else
		{
			len = ((actualHeight2 < actualHeight - Space) ? actualHeight : (actualHeight2 + Space));
		}
	}

	private void UpdateVertical(double from, double to, double len)
	{
		Canvas.SetTop(_txt1, from);
		Canvas.SetTop(_txt2, from);
		_txt1.SetCurrentValue(Canvas.TopProperty, from);
		_txt2.SetCurrentValue(Canvas.TopProperty, from);
		TimeSpan timeSpan = TimeSpan.FromSeconds(len / Speed);
		TimeSpan timeSpan2 = TimeSpan.FromSeconds(Math.Abs(from - to) / Speed);
		TimeSpan timeSpan3 = timeSpan + timeSpan;
		_storyboard = new Storyboard();
		DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames
		{
			Duration = timeSpan3,
			RepeatBehavior = RepeatBehavior.Forever
		};
		doubleAnimationUsingKeyFrames.KeyFrames.Add(new DiscreteDoubleKeyFrame(from, TimeSpan.FromSeconds(0.0)));
		doubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame(to, timeSpan2));
		Storyboard.SetTarget(doubleAnimationUsingKeyFrames, _txt1);
		Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath(Canvas.TopProperty));
		_storyboard.Children.Add(doubleAnimationUsingKeyFrames);
		DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames2 = new DoubleAnimationUsingKeyFrames
		{
			BeginTime = timeSpan,
			Duration = timeSpan3,
			RepeatBehavior = RepeatBehavior.Forever
		};
		doubleAnimationUsingKeyFrames2.KeyFrames.Add(new DiscreteDoubleKeyFrame(from, TimeSpan.FromSeconds(0.0)));
		doubleAnimationUsingKeyFrames2.KeyFrames.Add(new LinearDoubleKeyFrame(to, timeSpan2));
		Storyboard.SetTarget(doubleAnimationUsingKeyFrames2, _txt2);
		Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames2, new PropertyPath(Canvas.TopProperty));
		_storyboard.Children.Add(doubleAnimationUsingKeyFrames2);
		_txt1.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);
		_txt2.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);
		ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames = new ObjectAnimationUsingKeyFrames
		{
			Duration = timeSpan3,
			RepeatBehavior = RepeatBehavior.Forever
		};
		objectAnimationUsingKeyFrames.KeyFrames.Add(new DiscreteObjectKeyFrame
		{
			Value = Visibility.Visible,
			KeyTime = TimeSpan.FromSeconds(0.0)
		});
		objectAnimationUsingKeyFrames.KeyFrames.Add(new DiscreteObjectKeyFrame
		{
			Value = Visibility.Hidden,
			KeyTime = timeSpan2
		});
		Storyboard.SetTarget(objectAnimationUsingKeyFrames, _txt1);
		Storyboard.SetTargetProperty(objectAnimationUsingKeyFrames, new PropertyPath(UIElement.VisibilityProperty));
		_storyboard.Children.Add(objectAnimationUsingKeyFrames);
		ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames2 = new ObjectAnimationUsingKeyFrames
		{
			BeginTime = timeSpan,
			Duration = timeSpan3,
			RepeatBehavior = RepeatBehavior.Forever
		};
		objectAnimationUsingKeyFrames2.KeyFrames.Add(new DiscreteObjectKeyFrame
		{
			Value = Visibility.Visible,
			KeyTime = TimeSpan.FromSeconds(0.0)
		});
		objectAnimationUsingKeyFrames2.KeyFrames.Add(new DiscreteObjectKeyFrame
		{
			Value = Visibility.Hidden,
			KeyTime = timeSpan2
		});
		Storyboard.SetTarget(objectAnimationUsingKeyFrames2, _txt2);
		Storyboard.SetTargetProperty(objectAnimationUsingKeyFrames2, new PropertyPath(UIElement.VisibilityProperty));
		_storyboard.Children.Add(objectAnimationUsingKeyFrames2);
		_storyboard.Begin();
	}
}
