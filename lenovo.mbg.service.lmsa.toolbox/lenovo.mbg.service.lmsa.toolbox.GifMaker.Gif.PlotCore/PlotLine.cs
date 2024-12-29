using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.PlotCore;

public class PlotLine : Control, IPlotBase
{
	public static DependencyProperty StrokeProperty;

	public static DependencyProperty StrokeThicknessProperty;

	public static DependencyProperty AchorProperty;

	public static DependencyProperty EndXProperty;

	public static DependencyProperty EndYProperty;

	private Grid editPanel;

	private Line line;

	private Line area;

	private Border[] achorArr;

	private bool isEditModel;

	public double EndX
	{
		get
		{
			return (double)GetValue(EndXProperty);
		}
		set
		{
			SetValue(EndXProperty, value);
		}
	}

	public double EndY
	{
		get
		{
			return (double)GetValue(EndYProperty);
		}
		set
		{
			SetValue(EndYProperty, value);
		}
	}

	public SolidColorBrush Stroke
	{
		get
		{
			return (SolidColorBrush)GetValue(StrokeProperty);
		}
		set
		{
			SetValue(StrokeProperty, value);
		}
	}

	public double StrokeThickness
	{
		get
		{
			return (double)GetValue(StrokeThicknessProperty);
		}
		set
		{
			SetValue(StrokeThicknessProperty, value);
		}
	}

	public AchorType Achor
	{
		get
		{
			return (AchorType)GetValue(AchorProperty);
		}
		set
		{
			SetValue(AchorProperty, value);
		}
	}

	public Point Start { get; set; }

	public Size OwnerSize { get; set; }

	public int Number { get; set; }

	public PlotType PlType { get; set; }

	public PlotSetModel SetModel { get; set; }

	public bool IsEditModel
	{
		get
		{
			return isEditModel;
		}
		set
		{
			isEditModel = value;
			if (editPanel != null)
			{
				editPanel.Visibility = ((!isEditModel) ? Visibility.Collapsed : Visibility.Visible);
			}
		}
	}

	static PlotLine()
	{
		StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(PlotLine), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
		StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(PlotLine), new PropertyMetadata(1.0));
		AchorProperty = DependencyProperty.Register("Achor", typeof(AchorType), typeof(PlotLine), new PropertyMetadata(AchorType.Achor8));
		EndXProperty = DependencyProperty.Register("EndX", typeof(double), typeof(PlotLine), new PropertyMetadata(0.0, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			PlotLine plotLine = sender as PlotLine;
			if (plotLine.line != null)
			{
				plotLine.line.X2 = (double)e.NewValue;
				plotLine.area.X2 = (double)e.NewValue;
			}
		}));
		EndYProperty = DependencyProperty.Register("EndY", typeof(double), typeof(PlotLine), new PropertyMetadata(0.0, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			PlotLine plotLine2 = sender as PlotLine;
			if (plotLine2.line != null)
			{
				plotLine2.line.Y2 = (double)e.NewValue;
				plotLine2.area.Y2 = plotLine2.line.Y2 - 0.0;
			}
		}));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PlotLine), new FrameworkPropertyMetadata(typeof(PlotLine)));
	}

	public PlotLine()
	{
		isEditModel = true;
		base.Cursor = Cursors.SizeNS;
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		achorArr = new Border[2];
		line = base.Template.FindName("line", this) as Line;
		area = base.Template.FindName("area", this) as Line;
		editPanel = base.Template.FindName("editPanel", this) as Grid;
		for (int i = 0; i < 2; i++)
		{
			achorArr[i] = base.Template.FindName($"achor{i + 1}", this) as Border;
			achorArr[i].MouseEnter += OnAchorMoseEnter;
			achorArr[i].MouseLeave += OnAchorMouseLeave;
		}
		area.MouseEnter += PlotLine_MouseEnter;
		area.MouseLeave += PlotLine_MouseLeave;
	}

	private void PlotLine_MouseLeave(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed)
		{
			base.Cursor = Cursors.Cross;
			Achor = AchorType.None;
		}
	}

	private void PlotLine_MouseEnter(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed)
		{
			base.Cursor = Cursors.SizeAll;
			Achor = AchorType.All;
		}
	}

	private void OnAchorMoseEnter(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed && sender is Border)
		{
			Border border = sender as Border;
			if (border.Name == "achor1")
			{
				base.Cursor = Cursors.SizeNS;
				Achor = AchorType.Achor1;
			}
			else if (border.Name == "achor2")
			{
				base.Cursor = Cursors.SizeNS;
				Achor = AchorType.Achor8;
			}
			else
			{
				Achor = AchorType.None;
				base.Cursor = Cursors.Cross;
			}
		}
	}

	private void OnAchorMouseLeave(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed)
		{
			base.Cursor = Cursors.Cross;
			Achor = AchorType.None;
		}
	}

	private void SetEndAchor()
	{
		Thickness margin = default(Thickness);
		margin.Left = ((EndX > 0.0) ? EndX : (2.0 * EndX));
		margin.Top = ((EndY > 0.0) ? EndY : (2.0 * EndY));
		margin.Left -= 24.0;
		margin.Top -= 30.0;
		margin.Right = -20.0;
		margin.Bottom = -20.0;
		achorArr[1].Margin = margin;
	}

	private Point GetStartPt()
	{
		Point result = default(Point);
		result.X = Canvas.GetLeft(this);
		result.Y = Canvas.GetTop(this);
		return result;
	}

	private Point GetEndPt()
	{
		Point startPt = GetStartPt();
		startPt.X += (float)line.X2;
		startPt.Y += (float)line.Y2;
		return startPt;
	}

	public bool IsOnAchor()
	{
		return Achor != AchorType.None;
	}

	public bool IsValidate()
	{
		Point startPt = GetStartPt();
		Point endPt = GetEndPt();
		if (!(Math.Abs(endPt.X - startPt.X) > 2.0))
		{
			return Math.Abs(endPt.Y - startPt.Y) > 2.0;
		}
		return true;
	}

	public void OnMouseDown(Point pt)
	{
		Start = pt;
	}

	public void OnMouseMove(Point pt)
	{
		if (pt == Start)
		{
			return;
		}
		double num = pt.X - Start.X;
		double num2 = pt.Y - Start.Y;
		if (Achor == AchorType.Achor8)
		{
			double num3 = Canvas.GetLeft(this) + num + EndX;
			double num4 = Canvas.GetTop(this) + num2 + EndY;
			if (num3 < 0.0 || num3 > OwnerSize.Width || num4 < 0.0 || num4 > OwnerSize.Height)
			{
				return;
			}
			EndX += num;
			EndY += num2;
			SetEndAchor();
		}
		else if (Achor == AchorType.Achor1)
		{
			if (pt.X < 0.0 || pt.X > OwnerSize.Width || pt.Y < 0.0 || pt.Y > OwnerSize.Height)
			{
				return;
			}
			Canvas.SetLeft(this, pt.X);
			Canvas.SetTop(this, pt.Y);
			EndX -= num;
			EndY -= num2;
			SetEndAchor();
		}
		else if (Achor == AchorType.All)
		{
			double num5 = num + Canvas.GetLeft(this);
			double num6 = num2 + Canvas.GetTop(this);
			if (num5 < 0.0 || num5 + EndX > OwnerSize.Width || num6 < 0.0 || num6 + EndY > OwnerSize.Height)
			{
				return;
			}
			Canvas.SetLeft(this, num5);
			Canvas.SetTop(this, num6);
		}
		Start = pt;
	}

	public void OnMouseUp(Point pt)
	{
		Rect rect = default(Rect);
		rect.Width = 6.0;
		rect.Height = 6.0;
		Rect rect2 = rect;
		if (Achor == AchorType.Achor1)
		{
			Point startPt = GetStartPt();
			rect2.X = startPt.X - 3.0;
			rect2.Y = startPt.Y - 3.0;
		}
		else
		{
			if (Achor != AchorType.Achor8)
			{
				return;
			}
			Point endPt = GetEndPt();
			rect2.X = endPt.X - 3.0;
			rect2.Y = endPt.Y - 3.0;
		}
		if (!rect2.Contains(pt))
		{
			base.Cursor = Cursors.Cross;
			Achor = AchorType.None;
		}
	}

	public void PlotElement(DrawingContext context)
	{
		Point startPt = GetStartPt();
		Point endPt = GetEndPt();
		context.DrawLine(new Pen(Stroke, StrokeThickness), startPt, endPt);
	}
}
