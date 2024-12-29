using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public partial class SearchTextBox : UserControl, IComponentConnector
{
	private Image img;

	private bool isPicContent;

	public SearchTextBox()
	{
		InitializeComponent();
		InitializeResource();
	}

	private void ExecuteQuery()
	{
		if (txbSearch.Text.Length < 1)
		{
			img.Source = new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Asset/query.png"));
			isPicContent = false;
		}
		else
		{
			img.Source = new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Asset/deleteIcon.png"));
			isPicContent = true;
		}
	}

	private void InitializeResource()
	{
		base.Loaded += SearchTextBox_Loaded;
		txbSearch.KeyDown += SearchTextBox_KeyDown;
		txbSearch.TextChanged += txbSearch_TextChanged;
	}

	private void SearchTextBox_Loaded(object sender, RoutedEventArgs e)
	{
		img = txbSearch.Template.FindName("imgShow", txbSearch) as Image;
		if (img != null)
		{
			img.Cursor = Cursors.Hand;
			img.PreviewMouseDown += img_MouseDown;
			img.MouseEnter += img_MouseEnter;
			img.MouseLeave += img_MouseLeave;
		}
	}

	private void img_MouseDown(object sender, MouseButtonEventArgs e)
	{
		e.Handled = true;
		if (isPicContent)
		{
			txbSearch.Text = string.Empty;
		}
		ExecuteQuery();
	}

	private void img_MouseLeave(object sender, MouseEventArgs e)
	{
		if (!isPicContent)
		{
			img.Source = new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Asset/query.png"));
		}
		else
		{
			img.Source = new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Asset/deleteIcon.png"));
		}
	}

	private void img_MouseEnter(object sender, MouseEventArgs e)
	{
		if (!isPicContent)
		{
			img.Source = new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Asset/query2.png"));
		}
		else
		{
			img.Source = new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Asset/deleteIcon2.png"));
		}
	}

	private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			ExecuteQuery();
		}
	}

	private void txbSearch_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (txbSearch.Text.Length < 1)
		{
			img.Source = new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Asset/query.png"));
			isPicContent = false;
		}
	}
}
