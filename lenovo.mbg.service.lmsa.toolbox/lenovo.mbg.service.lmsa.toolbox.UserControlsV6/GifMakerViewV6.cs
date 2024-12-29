using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.toolbox.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox.UserControlsV6;

public partial class GifMakerViewV6 : UserControl, IDisposable, IComponentConnector
{
	public GifMakerViewModelV6 ViewModel { get; set; }

	public GifMakerViewV6()
	{
		InitializeComponent();
		ViewModel = new GifMakerViewModelV6(this);
		base.DataContext = ViewModel;
		base.Unloaded += delegate
		{
			ViewModel.Model.GifImage = null;
			ViewModel.Model.IsAnimation = false;
			ViewModel.Model.IsChanged = true;
		};
	}

	private void OnTextChanged(object sender, TextChangedEventArgs e)
	{
		TextBox textBox = sender as TextBox;
		if (img.Source == null || !textBox.IsFocused)
		{
			return;
		}
		int num = (int)img.Source.Width;
		int num2 = (int)img.Source.Height;
		if (string.IsNullOrEmpty(textBox.Text))
		{
			ViewModel.Model.GifWidth = 1;
			ViewModel.Model.GifHeight = 1;
		}
		else if (textBox.Name == "txtHeight")
		{
			if (ViewModel.Model.GifHeight > 4000)
			{
				ViewModel.Model.GifHeight /= 10;
				txtHeight.SelectionStart = txtHeight.Text.Length;
				return;
			}
			if (ViewModel.Model.GifHeight == 0)
			{
				ViewModel.Model.GifHeight = 1;
				return;
			}
			num = num * ViewModel.Model.GifHeight / num2;
			if (num > 4000)
			{
				ViewModel.Model.GifWidth = 4000;
			}
			else if (num == 0)
			{
				ViewModel.Model.GifWidth = 1;
			}
			else
			{
				ViewModel.Model.GifWidth = num;
			}
		}
		else
		{
			if (!(textBox.Name == "txtWidth"))
			{
				return;
			}
			if (ViewModel.Model.GifWidth > 4000)
			{
				ViewModel.Model.GifWidth /= 10;
				txtWidth.SelectionStart = txtWidth.Text.Length;
				return;
			}
			if (ViewModel.Model.GifWidth == 0)
			{
				ViewModel.Model.GifWidth = 1;
				return;
			}
			num2 = num2 * ViewModel.Model.GifWidth / num;
			if (num2 > 4000)
			{
				ViewModel.Model.GifHeight = 4000;
			}
			else if (num2 == 0)
			{
				ViewModel.Model.GifHeight = 1;
			}
			else
			{
				ViewModel.Model.GifHeight = num2;
			}
		}
	}

	private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		if (new Regex("[^0-9.-]+").IsMatch(e.Text))
		{
			e.Handled = true;
		}
	}

	public void StopAnimation()
	{
	}

	public void ResumeAnimation()
	{
	}

	public void Dispose()
	{
		ViewModel.Dispose();
	}
}
