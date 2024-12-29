using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Controls;

public partial class MultiCheckBox : UserControl, IComponentConnector
{
	private Popup popup;

	public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<MultiCheckBoxItemViewModel>), typeof(MultiCheckBox), new PropertyMetadata(null, ItemsSourceChangedCallback));

	public static readonly DependencyProperty ListVisibilityProperty = DependencyProperty.Register("ListVisibility", typeof(Visibility), typeof(MultiCheckBox), new PropertyMetadata(Visibility.Collapsed));

	public static readonly DependencyProperty TextBoxVisibilityProperty = DependencyProperty.Register("TextBoxVisibility", typeof(Visibility), typeof(MultiCheckBox), new PropertyMetadata(Visibility.Visible));

	public ObservableCollection<MultiCheckBoxItemViewModel> ItemsSource
	{
		get
		{
			return (ObservableCollection<MultiCheckBoxItemViewModel>)GetValue(ItemsSourceProperty);
		}
		set
		{
			SetValue(ItemsSourceProperty, value);
		}
	}

	public ReplayCommand CheckBoxClickCommand { get; set; }

	public bool IsListEmpty
	{
		set
		{
			if (value)
			{
				SetValue(ListVisibilityProperty, Visibility.Collapsed);
				SetValue(TextBoxVisibilityProperty, Visibility.Visible);
			}
			else
			{
				SetValue(ListVisibilityProperty, Visibility.Visible);
				SetValue(TextBoxVisibilityProperty, Visibility.Collapsed);
			}
		}
	}

	public MultiCheckBox()
	{
		CheckBoxClickCommand = new ReplayCommand(CheckBoxClickCommandHandler);
		base.Loaded += delegate
		{
			ObservableCollection<MultiCheckBoxItemViewModel> itemsSource = ItemsSource;
			if (itemsSource != null && itemsSource.Count() > 0)
			{
				IsListEmpty = false;
			}
		};
		InitializeComponent();
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		txtInput.ApplyTemplate();
		popup = txtInput.Template.FindName("popup", txtInput) as Popup;
	}

	private void txtInput_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		popup.IsOpen = true;
	}

	private static void ItemsSourceChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (sender is MultiCheckBox multiCheckBox)
		{
			multiCheckBox.UpdateText();
		}
	}

	private void CheckBoxClickCommandHandler(object parameter)
	{
		UpdateText();
	}

	private void UpdateText()
	{
		ObservableCollection<MultiCheckBoxItemViewModel> itemsSource = ItemsSource;
		if (itemsSource != null && itemsSource.Count() > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (MultiCheckBoxItemViewModel item in itemsSource)
			{
				if (item.IsChecked)
				{
					stringBuilder.Append(flag ? string.Empty : ",").Append(item.DisplayContent);
					flag = false;
				}
			}
			txtInput.Text = stringBuilder.ToString();
		}
		else
		{
			txtInput.Text = string.Empty;
		}
	}
}
