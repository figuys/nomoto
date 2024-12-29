using System.Collections.Generic;
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

public partial class TabletQMatchViewV6 : UserControl, IComponentConnector
{
	private Button _BtnDel;

	private SolidColorBrush _NormalBorderBrush;

	private SolidColorBrush _ErrorBorderBrush;

	private int CurrentIndex = 1;

	private readonly Dictionary<int, string> ConnectTutorialsMap = new Dictionary<int, string>
	{
		{ 1, "K1879" },
		{ 2, "K1880" },
		{ 3, "K1881" }
	};

	public TabletQMatchViewModelV6 VM { get; private set; }

	public TabletQMatchViewV6()
	{
		InitializeComponent();
		base.Tag = PageIndex.TABLET_SEARCH;
		VM = new TabletQMatchViewModelV6(this);
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
				VM.SearchKeyText = string.Empty;
			};
		};
		base.Loaded += TabletQMatchViewV6_Loaded;
		txtSearch.GotFocus += delegate
		{
			VM.SearchWarnText = string.Empty;
		};
		base.DataContext = VM;
	}

	private void TabletQMatchViewV6_Loaded(object sender, RoutedEventArgs e)
	{
		CurrentIndex = 1;
		nextbtn.IsEnabled = true;
		prevbtn.IsEnabled = false;
		steptext.LangKey = ConnectTutorialsMap[CurrentIndex];
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
		if (_BtnDel != null)
		{
			TextBox textBox = sender as TextBox;
			textBox.Text = textBox.Text.Trim();
			_BtnDel.Visibility = (string.IsNullOrEmpty(textBox.Text) ? Visibility.Collapsed : Visibility.Visible);
			if (string.IsNullOrEmpty(textBox.Text) || VM.ValidateSN(textBox.Text))
			{
				VM.SearchWarnText = string.Empty;
				VM.IsBtnEnable = !string.IsNullOrEmpty(textBox.Text);
				txtSearch.BorderBrush = _NormalBorderBrush;
			}
			else if (textBox.Text.Length < 8)
			{
				VM.SearchWarnText = "K1182";
				VM.IsBtnEnable = false;
				txtSearch.BorderBrush = _ErrorBorderBrush;
			}
			else
			{
				VM.SearchWarnText = "K1183";
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
		FindTabletSNView userUi = new FindTabletSNView();
		if (true == MainFrameV6.Instance.IMsgManager.ShowMessage(userUi))
		{
			MainFrameV6.Instance.ChangeView(PageIndex.TABLET_MANUAL);
		}
	}

	private void OnBtnSearch(object sender, RoutedEventArgs e)
	{
		VM.TabletMatchBySN();
	}

	private void prevbtn_Click(object sender, RoutedEventArgs e)
	{
		CurrentIndex--;
		steptext.LangKey = ConnectTutorialsMap[CurrentIndex];
		nextbtn.IsEnabled = CurrentIndex < ConnectTutorialsMap.Count;
		prevbtn.IsEnabled = CurrentIndex > 1;
	}

	private void nextbtn_Click(object sender, RoutedEventArgs e)
	{
		CurrentIndex++;
		steptext.LangKey = ConnectTutorialsMap[CurrentIndex];
		nextbtn.IsEnabled = CurrentIndex < ConnectTutorialsMap.Count;
		prevbtn.IsEnabled = CurrentIndex > 1;
	}
}
