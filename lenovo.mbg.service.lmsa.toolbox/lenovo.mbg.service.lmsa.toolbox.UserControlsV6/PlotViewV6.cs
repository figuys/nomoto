using System;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.PlotCore;
using lenovo.mbg.service.lmsa.toolbox.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.UserControlsV6;

public partial class PlotViewV6 : Window, IComponentConnector
{
	public PlotViewModelV6 ViewModel { get; set; }

	public PlotViewV6()
	{
		InitializeComponent();
		base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		ViewModel = new PlotViewModelV6(this);
		canvas.MouseLeftButtonDown += ViewModel.OnMouseLeftBtnDown;
		canvas.MouseMove += ViewModel.OnMouseMove;
		canvas.MouseLeftButtonUp += ViewModel.OnMouseLeftBtnUp;
		base.DataContext = ViewModel;
	}

	public void AddElement(UIElement element)
	{
		canvas.Children.Add(element);
	}

	public void DelElement(UIElement element)
	{
		canvas.Children.Remove(element);
	}

	public void DelAllElement()
	{
		canvas.Children.Clear();
	}

	public int GetElementCount()
	{
		return canvas.Children.Count;
	}

	public void DelLatestElement()
	{
		int num = -1;
		IPlotBase plotBase = null;
		foreach (IPlotBase child in canvas.Children)
		{
			if (child.Number > num)
			{
				num = child.Number;
				plotBase = child;
			}
		}
		if (plotBase != null)
		{
			DelElement(plotBase as UIElement);
		}
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		Close();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.CommandBindings.Clear();
		canvas.MouseMove -= ViewModel.OnMouseMove;
		canvas.MouseLeftButtonUp -= ViewModel.OnMouseLeftBtnUp;
		canvas.MouseLeftButtonDown -= ViewModel.OnMouseLeftBtnDown;
		base.OnClosed(e);
	}

	public Window GetMsgUi()
	{
		throw new NotImplementedException();
	}
}
