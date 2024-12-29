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

public partial class PhoneQueryViewV6 : UserControl, IComponentConnector
{
	private Button _BtnDel;

	private SolidColorBrush _NormalBorderBrush;

	private SolidColorBrush _ErrorBorderBrush;

	public PhoneQueryViewModelV6 VM { get; private set; }

	public PhoneQueryViewV6()
	{
		InitializeComponent();
		base.Tag = PageIndex.PHONE_SEARCH;
		_NormalBorderBrush = TryFindResource("V6_BorderBrushKey") as SolidColorBrush;
		_ErrorBorderBrush = TryFindResource("V6_WarnningBrushKey") as SolidColorBrush;
		txtSearch.Loaded += delegate
		{
			ToggleButton toggleButton = txtSearch.Template.FindName("PART_Toggle", txtSearch) as ToggleButton;
			toggleButton?.SetBinding(UIElement.VisibilityProperty, "ToggleButtonVisibility");
			PART_Popup.SetBinding(Popup.IsOpenProperty, new Binding
			{
				Source = toggleButton,
				Path = new PropertyPath("IsChecked"),
				Mode = BindingMode.TwoWay
			});
			_BtnDel = txtSearch.Template.FindName("PART_Empty", txtSearch) as Button;
			if (_BtnDel != null)
			{
				_BtnDel.Click += delegate
				{
					VM.SearchKeyText = string.Empty;
				};
			}
		};
		txtSearch.GotFocus += delegate
		{
			VM.SearchWarnText = string.Empty;
		};
		VM = new PhoneQueryViewModelV6(this);
		base.DataContext = VM;
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
		else if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Left || e.Key == Key.Right || (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V))
		{
			TextBox textBox = sender as TextBox;
			if (textBox.Text.Length >= 18 && textBox.SelectionLength == 0)
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
		if (_BtnDel != null)
		{
			TextBox textBox = sender as TextBox;
			textBox.Text = textBox.Text.Trim();
			_BtnDel.Visibility = (string.IsNullOrEmpty(textBox.Text) ? Visibility.Collapsed : Visibility.Visible);
			if (string.IsNullOrEmpty(textBox.Text) || VM.ValidateImei(textBox.Text))
			{
				VM.SearchWarnText = string.Empty;
				VM.IsBtnEnable = !string.IsNullOrEmpty(textBox.Text);
				txtSearch.BorderBrush = _NormalBorderBrush;
			}
			else if (textBox.Text.Length < 15)
			{
				VM.SearchWarnText = "K1157";
				VM.IsBtnEnable = false;
				txtSearch.BorderBrush = _ErrorBorderBrush;
			}
			else
			{
				VM.SearchWarnText = "K0989";
				VM.IsBtnEnable = false;
				txtSearch.BorderBrush = _ErrorBorderBrush;
			}
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
		Plugin.OperateTracker("HowToFindImeiBtnClick", "User clicked how to find imei help button.");
		FindIMEIView userUi = new FindIMEIView();
		if (true == MainFrameV6.Instance.IMsgManager.ShowMessage(userUi))
		{
			MainFrameV6.Instance.ShowGifGuideSteps(_showTextDetect: true, null);
		}
	}

	private void OnBtnSearch(object sender, RoutedEventArgs e)
	{
		VM.PhoneMatchByIemi();
	}

	private void OnLBtnEntry(object sender, MouseButtonEventArgs e)
	{
		Plugin.OperateTracker("OnLBtnEntry", "User clicked Additional ways to connect can be found here\u200b!");
		MainFrameV6.Instance.IMsgManager.ShowMessage(new PhoneEnterySelView());
	}
}
