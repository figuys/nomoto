using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.PlotCore;

public class PlotShape : ContentControl, IPlotBase
{
	public static DependencyProperty AchorProperty;

	private Grid border;

	private Border[] rectArr;

	private Rectangle area;

	private bool isEditModel;

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
			if (border != null)
			{
				border.Visibility = ((!isEditModel) ? Visibility.Collapsed : Visibility.Visible);
			}
		}
	}

	static PlotShape()
	{
		AchorProperty = DependencyProperty.Register("Achor", typeof(AchorType), typeof(PlotShape), new PropertyMetadata(AchorType.Achor8));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PlotShape), new FrameworkPropertyMetadata(typeof(PlotShape)));
	}

	public PlotShape()
	{
		base.Width = 0.0;
		base.Height = 0.0;
		isEditModel = true;
		base.Cursor = Cursors.SizeNWSE;
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		rectArr = new Border[8];
		rectArr[0] = base.Template.FindName("achor1", this) as Border;
		rectArr[1] = base.Template.FindName("achor2", this) as Border;
		rectArr[2] = base.Template.FindName("achor3", this) as Border;
		rectArr[3] = base.Template.FindName("achor4", this) as Border;
		rectArr[4] = base.Template.FindName("achor5", this) as Border;
		rectArr[5] = base.Template.FindName("achor6", this) as Border;
		rectArr[6] = base.Template.FindName("achor7", this) as Border;
		rectArr[7] = base.Template.FindName("achor8", this) as Border;
		border = base.Template.FindName("border", this) as Grid;
		area = base.Template.FindName("area", this) as Rectangle;
		Border[] array = rectArr;
		foreach (Border item in array)
		{
			item.MouseEnter += delegate(object sender, MouseEventArgs e)
			{
				if (e.LeftButton != MouseButtonState.Pressed)
				{
					switch (item.Name)
					{
					case "achor1":
						base.Cursor = Cursors.SizeNWSE;
						Achor = AchorType.Achor1;
						break;
					case "achor2":
						base.Cursor = Cursors.SizeNS;
						Achor = AchorType.Achor2;
						break;
					case "achor3":
						base.Cursor = Cursors.SizeNESW;
						Achor = AchorType.Achor3;
						break;
					case "achor4":
						base.Cursor = Cursors.SizeWE;
						Achor = AchorType.Achor4;
						break;
					case "achor5":
						base.Cursor = Cursors.SizeNWSE;
						Achor = AchorType.Achor5;
						break;
					case "achor6":
						base.Cursor = Cursors.SizeNS;
						Achor = AchorType.Achor6;
						break;
					case "achor7":
						base.Cursor = Cursors.SizeNESW;
						Achor = AchorType.Achor7;
						break;
					case "achor8":
						base.Cursor = Cursors.SizeWE;
						Achor = AchorType.Achor8;
						break;
					}
				}
			};
			item.MouseLeave += delegate(object sender, MouseEventArgs e)
			{
				if (e.LeftButton != MouseButtonState.Pressed && ((item.Name == "achor1" && Achor == AchorType.Achor1) || (item.Name == "achor2" && Achor == AchorType.Achor2) || (item.Name == "achor3" && Achor == AchorType.Achor3) || (item.Name == "achor4" && Achor == AchorType.Achor4) || (item.Name == "achor5" && Achor == AchorType.Achor5) || (item.Name == "achor6" && Achor == AchorType.Achor6) || (item.Name == "achor7" && Achor == AchorType.Achor7) || (item.Name == "achor8" && Achor == AchorType.Achor8)))
				{
					Achor = AchorType.None;
					if (PlType == PlotType.Text)
					{
						base.Cursor = Cursors.IBeam;
						(base.Content as TextBox).Focus();
					}
					else
					{
						base.Cursor = Cursors.Cross;
					}
				}
			};
		}
		area.MouseEnter += delegate(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				base.Cursor = Cursors.SizeAll;
				Achor = AchorType.All;
			}
		};
		area.MouseLeave += delegate(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				base.Cursor = Cursors.Cross;
				Achor = AchorType.None;
				if (base.Content is TextBox)
				{
					(base.Content as TextBox).Focus();
				}
			}
		};
		area.Fill = ((PlType == PlotType.Text) ? null : new SolidColorBrush(Colors.Transparent));
	}

	public Rect GetBound()
	{
		float num = (float)Canvas.GetLeft(this);
		float num2 = (float)Canvas.GetTop(this);
		float num3 = (float)base.Width;
		float num4 = (float)base.Height;
		return new Rect(num, num2, num3, num4);
	}

	public string GetText()
	{
		if (PlType != PlotType.Text)
		{
			return string.Empty;
		}
		return (base.Content as TextBox).Text;
	}

	public bool IsOnAchor()
	{
		return Achor != AchorType.None;
	}

	public bool IsValidate()
	{
		if (PlType == PlotType.Text && string.IsNullOrWhiteSpace(GetText()))
		{
			return false;
		}
		Rect bound = GetBound();
		if (bound.Width >= 2.0)
		{
			return bound.Height >= 2.0;
		}
		return false;
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
		switch (Achor)
		{
		case AchorType.Achor1:
			MoveLeft(pt);
			MoveTop(pt);
			break;
		case AchorType.Achor2:
			MoveTop(pt);
			break;
		case AchorType.Achor3:
			MoveRight(pt);
			MoveTop(pt);
			break;
		case AchorType.Achor4:
			MoveRight(pt);
			break;
		case AchorType.Achor5:
			MoveRight(pt);
			MoveBottom(pt);
			break;
		case AchorType.Achor6:
			MoveBottom(pt);
			break;
		case AchorType.Achor7:
			MoveLeft(pt);
			MoveBottom(pt);
			break;
		case AchorType.Achor8:
			MoveLeft(pt);
			break;
		case AchorType.All:
		{
			double num = pt.X - Start.X + Canvas.GetLeft(this);
			double num2 = pt.Y - Start.Y + Canvas.GetTop(this);
			if (!(num < 0.0) && !(num + base.Width > OwnerSize.Width) && !(num2 < 0.0) && !(num2 + base.Height > OwnerSize.Height))
			{
				Canvas.SetLeft(this, num);
				Canvas.SetTop(this, num2);
			}
			break;
		}
		}
		Start = pt;
	}

	public void OnMouseUp(Point pt)
	{
		Rect rect = default(Rect);
		rect.Width = 20.0;
		rect.Height = 20.0;
		Rect rect2 = rect;
		Rect rect3 = GeShapetBound();
		switch (Achor)
		{
		case AchorType.Achor1:
			rect2.X = rect3.X - 10.0;
			rect2.Y = rect3.Y - 10.0;
			break;
		case AchorType.Achor2:
			rect2.X = rect3.X + rect3.Width / 2.0 - 10.0;
			rect2.Y = rect3.Y - 10.0;
			break;
		case AchorType.Achor3:
			rect2.X = rect3.Right - 10.0;
			rect2.Y = rect3.Top - 10.0;
			break;
		case AchorType.Achor4:
			rect2.X = rect3.Right - 10.0;
			rect2.Y = rect3.Y + rect3.Height / 2.0 - 10.0;
			break;
		case AchorType.Achor5:
			rect2.X = rect3.Right - 10.0;
			rect2.Y = rect3.Bottom - 10.0;
			break;
		case AchorType.Achor6:
			rect2.X = rect3.X + rect3.Width / 2.0 - 10.0;
			rect2.Y = rect3.Bottom - 10.0;
			break;
		case AchorType.Achor7:
			rect2.X = rect3.X - 10.0;
			rect2.Y = rect3.Bottom - 10.0;
			break;
		case AchorType.Achor8:
			rect2.X = rect3.X - 10.0;
			rect2.Y = rect3.X + rect3.Height / 2.0 - 10.0;
			break;
		case AchorType.All:
			rect2 = rect3;
			break;
		}
		if (!rect2.Contains(pt))
		{
			base.Cursor = Cursors.Cross;
			Achor = AchorType.None;
		}
	}

	public void PlotElement(DrawingContext context)
	{
		switch (PlType)
		{
		case PlotType.Ellipse:
			PlotEllipse(context);
			break;
		case PlotType.Rect:
			PlotRect(context);
			break;
		case PlotType.Text:
			PlotText(context);
			break;
		case PlotType.Arc:
			break;
		}
	}

	public Pen GetPen()
	{
		return new Pen(new SolidColorBrush(SetModel.Color), (float)SetModel.LineWeight);
	}

	private Brush GetBrush()
	{
		return new SolidColorBrush(SetModel.Color);
	}

	private Rect GeShapetBound()
	{
		float num = (float)Canvas.GetLeft(this);
		float num2 = (float)Canvas.GetTop(this);
		float num3 = (float)base.Width;
		float num4 = (float)base.Height;
		return new Rect(num, num2, num3, num4);
	}

	private void PlotText(DrawingContext context)
	{
		string text = GetText();
		Typeface typeface = new Typeface(style: SetModel.IsItalic ? FontStyles.Italic : FontStyles.Normal, weight: SetModel.IsBold ? FontWeights.Bold : FontWeights.Normal, fontFamily: new FontFamily(SetModel.FontFamily), stretch: FontStretches.Normal);
		Rect rect = GeShapetBound();
		Point origin = new Point(rect.X, rect.Y);
		FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, SetModel.FontSize, GetBrush(), VisualTreeHelper.GetDpi(this).PixelsPerDip)
		{
			MaxTextWidth = rect.Width,
			MaxTextHeight = rect.Height
		};
		context.DrawText(formattedText, origin);
	}

	private void PlotRect(DrawingContext context)
	{
		context.DrawRectangle(new SolidColorBrush(Colors.Transparent), GetPen(), GeShapetBound());
	}

	private void PlotEllipse(DrawingContext context)
	{
		Rect rect = GeShapetBound();
		double num = rect.Width / 2.0;
		double num2 = rect.Height / 2.0;
		Point point = default(Point);
		point.X = rect.X + num;
		point.Y = rect.Y + num2;
		Point center = point;
		context.DrawEllipse(new SolidColorBrush(Colors.Transparent), GetPen(), center, num, num2);
	}

	private void MoveTop(Point pt)
	{
		if (!(pt.Y < 0.0))
		{
			double num = Start.Y - pt.Y;
			double num2 = Start.Y - Canvas.GetTop(this);
			double num3 = num + base.Height - num2;
			if (num3 >= 0.0)
			{
				Canvas.SetTop(this, pt.Y);
				base.Height = num3;
			}
		}
	}

	private void MoveLeft(Point pt)
	{
		if (!(pt.X <= 0.0))
		{
			double num = Start.X - pt.X;
			double num2 = Start.X - Canvas.GetLeft(this);
			double num3 = num + base.Width - num2;
			if (num3 >= 0.0)
			{
				Canvas.SetLeft(this, pt.X);
				base.Width = num3;
			}
		}
	}

	private void MoveRight(Point pt)
	{
		double num = pt.X - Start.X;
		double num2 = Start.X - (Canvas.GetLeft(this) + base.Width);
		double num3 = num + base.Width + num2;
		if (num3 >= 0.0 && Canvas.GetLeft(this) + num3 <= OwnerSize.Width)
		{
			base.Width = num3;
		}
	}

	private void MoveBottom(Point pt)
	{
		double num = pt.Y - Start.Y;
		double num2 = Start.Y - (Canvas.GetTop(this) + base.Height);
		double num3 = num + base.Height + num2;
		if (num3 >= 0.0 && Canvas.GetTop(this) + num3 <= OwnerSize.Height)
		{
			base.Height = num3;
		}
	}
}
