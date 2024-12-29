using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.PlotCore;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.Model;
using lenovo.mbg.service.lmsa.toolbox.UserControlsV6;

namespace lenovo.mbg.service.lmsa.toolbox.ViewModelV6;

public class PlotViewModelV6
{
	private PlotViewV6 viewWnd;

	private PlotType ElementType;

	private IPlotBase CurElement;

	public ICommand CloseCmd { get; set; }

	public ICommand UndoCmd { get; set; }

	public ICommand ChangeSizeCmd { get; set; }

	public ICommand SaveImageCmd { get; set; }

	public ICommand ToolBarItemCmd { get; set; }

	public PlotModel Model { get; set; }

	public PlotViewModelV6(PlotViewV6 wnd)
	{
		PlotViewModelV6 plotViewModelV = this;
		viewWnd = wnd;
		Model = new PlotModel();
		CloseCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(CloseCmd, delegate
		{
			wnd.Close();
		}));
		SaveImageCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(SaveImageCmd, delegate
		{
			bool flag = plotViewModelV.CurElement != null && plotViewModelV.CurElement.PlType == PlotType.Clip;
			if (flag)
			{
				Rect bound = (plotViewModelV.CurElement as PlotShape).GetBound();
				flag = bound.Width >= 10.0 && bound.Height >= 10.0;
			}
			bool isClip = flag && plotViewModelV.ProcessWithMasker(() => ToolboxViewContext.SingleInstance.MessageBox.ShowMessage("K0224", MessageBoxButton.OKCancel).Value);
			plotViewModelV.Model.NewFile = string.Format("{0}\\{1}.jpg", plotViewModelV.Model.TempDir, DateTime.Now.ToString("yyyyMMddHHmmss"));
			HostProxy.ResourcesLoggingService.RegisterFile(plotViewModelV.Model.NewFile);
			BitmapSource source = plotViewModelV.CreateMemoryImage(isClip);
			using (FileStream fileStream = File.Create(plotViewModelV.Model.NewFile))
			{
				JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
				jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(source));
				jpegBitmapEncoder.Save(fileStream);
				fileStream.Flush();
			}
			plotViewModelV.viewWnd.DialogResult = true;
			plotViewModelV.viewWnd = null;
		}));
		ToolBarItemCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(ToolBarItemCmd, delegate(object sender, ExecutedRoutedEventArgs e)
		{
			switch (e.Parameter as string)
			{
			case "ELLIPSE":
				plotViewModelV.Model.Cursor = System.Windows.Input.Cursors.Cross;
				plotViewModelV.ElementType = PlotType.Ellipse;
				break;
			case "LINE":
				plotViewModelV.Model.Cursor = System.Windows.Input.Cursors.Cross;
				plotViewModelV.ElementType = PlotType.Line;
				break;
			case "RECT":
				plotViewModelV.Model.Cursor = System.Windows.Input.Cursors.Cross;
				plotViewModelV.ElementType = PlotType.Rect;
				break;
			case "TEXT":
				plotViewModelV.Model.Cursor = System.Windows.Input.Cursors.IBeam;
				plotViewModelV.ElementType = PlotType.Text;
				break;
			case "CUT":
				plotViewModelV.Model.Cursor = System.Windows.Input.Cursors.Cross;
				plotViewModelV.ElementType = PlotType.Clip;
				break;
			case "FONT":
				plotViewModelV.SetFontSetting();
				break;
			case "COLOR":
				plotViewModelV.SetColorSetting();
				break;
			}
		}));
		UndoCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(UndoCmd, delegate
		{
			plotViewModelV.viewWnd.DelLatestElement();
			plotViewModelV.CurElement = null;
		}));
		ChangeSizeCmd = new RoutedCommand();
		wnd.CommandBindings.Add(new CommandBinding(ChangeSizeCmd, delegate(object sender, ExecutedRoutedEventArgs e)
		{
			wnd.WindowState = (((bool)e.Parameter) ? WindowState.Maximized : WindowState.Normal);
		}));
	}

	private void SetFontSetting()
	{
		FontDialog fontDialog = new FontDialog();
		System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
		if (Model.Settings.IsItalic && Model.Settings.IsBold)
		{
			style = System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic;
		}
		else if (Model.Settings.IsItalic)
		{
			style = System.Drawing.FontStyle.Italic;
		}
		else if (Model.Settings.IsBold)
		{
			style = System.Drawing.FontStyle.Bold;
		}
		fontDialog.Font = new Font(Model.Settings.FontFamily, (float)Model.Settings.FontSize, style);
		if (fontDialog.ShowDialog() == DialogResult.OK)
		{
			Model.Settings.FontFamily = fontDialog.Font.Name;
			Model.Settings.FontSize = fontDialog.Font.Size;
			Model.Settings.IsBold = fontDialog.Font.Bold;
			Model.Settings.IsItalic = fontDialog.Font.Italic;
		}
	}

	private void SetColorSetting()
	{
		ColorDialog colorDialog = new ColorDialog();
		colorDialog.Color = System.Drawing.Color.FromArgb(Model.Settings.Color.A, Model.Settings.Color.R, Model.Settings.Color.G, Model.Settings.Color.B);
		if (colorDialog.ShowDialog() == DialogResult.OK)
		{
			Model.Settings.Color = System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
		}
	}

	public void OnMouseLeftBtnUp(object sender, MouseButtonEventArgs e)
	{
		if (CurElement != null)
		{
			CurElement.OnMouseUp(e.GetPosition(viewWnd.canvas));
		}
	}

	public void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
	{
		if (CurElement != null && e.LeftButton != 0)
		{
			CurElement.OnMouseMove(e.GetPosition(viewWnd.canvas));
		}
	}

	public void OnMouseLeftBtnDown(object sender, MouseButtonEventArgs e)
	{
		System.Windows.Point position = e.GetPosition(viewWnd.canvas);
		switch (ElementType)
		{
		case PlotType.Ellipse:
			DrawEllipse(position);
			break;
		case PlotType.Rect:
			DrawRectangle(position);
			break;
		case PlotType.Line:
			DrawLine(position);
			break;
		case PlotType.Text:
			DrawText(position);
			break;
		case PlotType.Clip:
			DrawClip(position);
			break;
		case PlotType.None:
		case PlotType.Arc:
			break;
		}
	}

	private bool IsNeedNewElement(PlotType type, System.Windows.Point pt)
	{
		if (CurElement != null && CurElement.IsEditModel && CurElement.PlType == type && CurElement.IsOnAchor())
		{
			CurElement.OnMouseDown(pt);
			return false;
		}
		if (CurElement != null && CurElement.IsEditModel)
		{
			if (!CurElement.IsValidate())
			{
				viewWnd.DelElement(CurElement as UIElement);
				CurElement = null;
				return false;
			}
			if (ElementType == PlotType.Clip && CurElement.PlType == PlotType.Clip)
			{
				Rect bound = (CurElement as PlotShape).GetBound();
				if (bound.Width < 10.0 || bound.Height < 10.0)
				{
					return false;
				}
				if (ProcessWithMasker(() => ToolboxViewContext.SingleInstance.MessageBox.ShowMessage("K0224", MessageBoxButton.OKCancel).Value))
				{
					Model.DisplayImage = CreateMemoryImage(isClip: true);
				}
				viewWnd.DelElement(CurElement as UIElement);
				CurElement = null;
				return false;
			}
			CurElement.IsEditModel = false;
			CurElement.Achor = AchorType.None;
			return false;
		}
		return true;
	}

	private bool ProcessWithMasker(Func<bool> task)
	{
		string uid = Guid.NewGuid().ToString("N");
		try
		{
			HostProxy.HostOperationService.ShowMaskLayer(uid, viewWnd.WindowState);
			return task();
		}
		finally
		{
			HostProxy.HostOperationService.CloseMaskLayer(uid);
		}
	}

	private BitmapSource CreateMemoryImage(bool isClip)
	{
		Int32Rect clip = Int32Rect.Empty;
		DrawingVisual drawingVisual = new DrawingVisual();
		using (DrawingContext drawingContext = drawingVisual.RenderOpen())
		{
			drawingContext.DrawImage(Model.DisplayImage, new Rect(0.0, 0.0, Model.DisplayImage.Width, Model.DisplayImage.Height));
			foreach (IPlotBase child in viewWnd.canvas.Children)
			{
				if (child.PlType != PlotType.Clip)
				{
					child.PlotElement(drawingContext);
					continue;
				}
				Rect bound = (child as PlotShape).GetBound();
				clip = new Int32Rect((int)bound.X, (int)bound.Y, (int)bound.Width, (int)bound.Height);
			}
			drawingContext.Close();
		}
		viewWnd.DelAllElement();
		RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(Model.DisplayImage.PixelWidth, Model.DisplayImage.PixelHeight, Model.DisplayImage.DpiX, Model.DisplayImage.DpiY, PixelFormats.Pbgra32);
		renderTargetBitmap.Render(drawingVisual);
		if (!isClip)
		{
			return renderTargetBitmap;
		}
		return ClipCurrentImage(clip, renderTargetBitmap);
	}

	private BitmapSource ClipCurrentImage(Int32Rect clip, BitmapSource src)
	{
		int num = src.Format.BitsPerPixel * clip.Width / 8;
		byte[] pixels = new byte[clip.Height * num];
		src.CopyPixels(clip, pixels, num, 0);
		src = null;
		return BitmapSource.Create(clip.Width, clip.Height, 96.0, 96.0, PixelFormats.Pbgra32, null, pixels, num);
	}

	private void CreateRectElement(System.Windows.Point pt, PlotType type, Func<FrameworkElement, object> callback)
	{
		if (IsNeedNewElement(type, pt))
		{
			PlotShape plotShape = new PlotShape
			{
				Start = pt,
				OwnerSize = viewWnd.canvas.RenderSize,
				Number = viewWnd.GetElementCount() + 1,
				IsEditModel = true,
				SetModel = Model.Settings.Clone(),
				Achor = AchorType.Achor5,
				PlType = type
			};
			plotShape.Content = callback(plotShape);
			Canvas.SetLeft(plotShape, pt.X);
			Canvas.SetTop(plotShape, pt.Y);
			viewWnd.AddElement(plotShape);
			CurElement = plotShape;
		}
	}

	private void DrawLine(System.Windows.Point pt)
	{
		if (IsNeedNewElement(PlotType.Line, pt))
		{
			PlotLine plotLine = new PlotLine
			{
				Start = pt,
				OwnerSize = viewWnd.canvas.RenderSize,
				Number = viewWnd.GetElementCount() + 1,
				IsEditModel = true,
				PlType = PlotType.Line,
				SetModel = Model.Settings,
				Stroke = new SolidColorBrush(Model.Settings.Color),
				StrokeThickness = Model.Settings.LineWeight
			};
			Canvas.SetLeft(plotLine, pt.X);
			Canvas.SetTop(plotLine, pt.Y);
			viewWnd.AddElement(plotLine);
			CurElement = plotLine;
		}
	}

	private void DrawText(System.Windows.Point pt)
	{
		CreateRectElement(pt, PlotType.Text, delegate(FrameworkElement param)
		{
			System.Windows.Controls.TextBox txt = new System.Windows.Controls.TextBox
			{
				FontSize = Model.Settings.FontSize,
				Foreground = new SolidColorBrush(Model.Settings.Color),
				FontFamily = new System.Windows.Media.FontFamily(Model.Settings.FontFamily),
				Style = (viewWnd.FindResource("TextBoxStyle") as Style)
			};
			if (Model.Settings.IsBold)
			{
				txt.FontWeight = FontWeights.Bold;
			}
			if (Model.Settings.IsItalic)
			{
				txt.FontStyle = FontStyles.Italic;
			}
			txt.SetBinding(FrameworkElement.WidthProperty, new System.Windows.Data.Binding
			{
				Source = param,
				Path = new PropertyPath("ActualWidth")
			});
			txt.SetBinding(FrameworkElement.HeightProperty, new System.Windows.Data.Binding
			{
				Source = param,
				Path = new PropertyPath("ActualHeight")
			});
			FocusManager.SetFocusedElement(viewWnd, txt);
			param.Width = 120.0;
			param.Height = 32.0;
			txt.Loaded += delegate
			{
				txt.Focus();
			};
			txt.AddHandler(UIElement.MouseLeftButtonDown, (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs e)
			{
				e.Handled = true;
			}, handledEventsToo: true);
			return txt;
		});
	}

	private void DrawClip(System.Windows.Point pt)
	{
		CreateRectElement(pt, PlotType.Clip, (FrameworkElement param) => new Grid
		{
			Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(20, byte.MaxValue, byte.MaxValue, byte.MaxValue))
		});
	}

	private void DrawEllipse(System.Windows.Point pt)
	{
		CreateRectElement(pt, PlotType.Ellipse, (FrameworkElement param) => new Ellipse
		{
			Stroke = new SolidColorBrush(Model.Settings.Color),
			StrokeThickness = Model.Settings.LineWeight
		});
	}

	private void DrawRectangle(System.Windows.Point pt)
	{
		CreateRectElement(pt, PlotType.Rect, (FrameworkElement param) => new System.Windows.Shapes.Rectangle
		{
			Stroke = new SolidColorBrush(Model.Settings.Color),
			StrokeThickness = Model.Settings.LineWeight
		});
	}
}
