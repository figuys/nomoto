using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class SmartQMatchViewV6 : UserControl, IComponentConnector
{
	private Button _BtnDel;

	private readonly SolidColorBrush _NormalBorderBrush;

	private readonly SolidColorBrush _ErrorBorderBrush;

	public SmartQMatchViewModel _VM;

	public SmartQMatchViewV6()
	{
		InitializeComponent();
		base.Tag = PageIndex.SMART_SEARCH;
		_VM = new SmartQMatchViewModel(this);
		base.DataContext = _VM;
		_NormalBorderBrush = TryFindResource("V6_BorderBrushKey") as SolidColorBrush;
		_ErrorBorderBrush = TryFindResource("V6_WarnningBrushKey") as SolidColorBrush;
		txtSearch.Loaded += delegate
		{
			ToggleButton toggleButton = txtSearch.Template.FindName("PART_Toggle", txtSearch) as ToggleButton;
			toggleButton?.SetBinding(UIElement.VisibilityProperty, "ToggleButtonVisibility");
			PART_Popup.SetBinding(Popup.IsOpenProperty, new Binding
			{
				Source = toggleButton,
				Path = new PropertyPath("IsChecked")
			});
			_BtnDel = txtSearch.Template.FindName("PART_Empty", txtSearch) as Button;
			_BtnDel.Click += delegate
			{
				_VM.SearchKeyText = string.Empty;
			};
		};
		txtSearch.GotFocus += delegate
		{
			_VM.SearchWarnText = string.Empty;
		};
	}

	private void OnPreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
		{
			e.Handled = true;
		}
		else if (e.Key == Key.Back)
		{
			e.Handled = false;
		}
		else if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.A && e.Key <= Key.Z) || e.Key == Key.Left || e.Key == Key.Right || (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V))
		{
			TextBox textBox = sender as TextBox;
			if (textBox.Text.Length >= 8 && textBox.SelectionLength == 0)
			{
				e.Handled = true;
			}
			else
			{
				e.Handled = false;
			}
		}
		else
		{
			e.Handled = true;
		}
	}

	private void Txt_TextChanged(object sender, TextChangedEventArgs e)
	{
		TextBox textBox = sender as TextBox;
		textBox.Text = textBox.Text.Trim();
		_BtnDel.Visibility = (string.IsNullOrEmpty(textBox.Text) ? Visibility.Collapsed : Visibility.Visible);
		if (string.IsNullOrEmpty(textBox.Text) || _VM.ValidateSN(textBox.Text))
		{
			_VM.SearchWarnText = string.Empty;
			_VM.IsBtnEnable = !string.IsNullOrEmpty(textBox.Text);
			txtSearch.BorderBrush = _NormalBorderBrush;
		}
		else if (textBox.Text.Length < 8)
		{
			_VM.SearchWarnText = "K1182";
			_VM.IsBtnEnable = false;
			txtSearch.BorderBrush = _ErrorBorderBrush;
		}
		else
		{
			_VM.SearchWarnText = "K1183";
			_VM.IsBtnEnable = false;
			txtSearch.BorderBrush = _ErrorBorderBrush;
		}
	}

	private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if ((e.OriginalSource as ListBox).SelectedIndex != -1)
		{
			PART_Popup.IsOpen = false;
		}
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		FindSmartSNView userUi = new FindSmartSNView();
		if (true == MainFrameV6.Instance.IMsgManager.ShowMessage(userUi))
		{
			MainFrameV6.Instance.ChangeView(PageIndex.SMART_MANUAL);
		}
	}

	private void OnBtnSearch(object sender, RoutedEventArgs e)
	{
		_VM.SmartMatchBySN();
	}
}
