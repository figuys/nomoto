using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Controls;

[TemplateVisualState(Name = "SelectionEmptyStates", GroupName = "SelectionEmpty")]
[TemplateVisualState(Name = "SelectionEmptyStates", GroupName = "SelectionNotEmpty")]
[TemplateVisualState(GroupName = "DataLoadingStates", Name = "DataLoading")]
[TemplateVisualState(GroupName = "DataLoadingStates", Name = "DataLoadingUnloading")]
public class LComboBox : ComboBox
{
	private ICollectionView _collView;

	public static readonly DependencyProperty IsCaseSensitiveProperty = DependencyProperty.Register("IsCaseSensitive", typeof(bool), typeof(LComboBox), new UIPropertyMetadata(false));

	public static readonly DependencyProperty IsSearchableProperty = DependencyProperty.Register("IsSearchable", typeof(bool), typeof(LComboBox), new UIPropertyMetadata(true));

	public static readonly DependencyProperty IsSortableProperty = DependencyProperty.Register("IsSortable", typeof(bool), typeof(LComboBox), new UIPropertyMetadata(true, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		LComboBox lComboBox = sender as LComboBox;
		if (Convert.ToBoolean(e.NewValue))
		{
			lComboBox.Items.SortDescriptions.Clear();
			lComboBox.Items.SortDescriptions.Add(new SortDescription("IsUsed", ListSortDirection.Descending));
			lComboBox.Items.SortDescriptions.Add(new SortDescription("ItemText", ListSortDirection.Ascending));
		}
		else
		{
			lComboBox.Items.SortDescriptions.Clear();
		}
	}));

	public static readonly DependencyProperty DropDownOnFocusProperty = DependencyProperty.Register("DropDownOnFocus", typeof(bool), typeof(LComboBox), new UIPropertyMetadata(true));

	public static readonly DependencyProperty FilterCompareDelegateProperty = DependencyProperty.Register("FilterCompareDelegate", typeof(Func<object, string, int>), typeof(LComboBox), new PropertyMetadata(null, FilterCompareDelegateChangedCallback));

	public static readonly DependencyProperty EditableTextBoxForegroundProperty = DependencyProperty.Register("EditableTextBoxForeground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#949494"))));

	public static readonly DependencyProperty DropDownIsEnaledProperty = DependencyProperty.Register("DropDownIsEnaled", typeof(bool), typeof(LComboBox), new PropertyMetadata(true));

	public static readonly RoutedEvent TextChangedEvent;

	public static readonly DependencyProperty CornerRadiusProperty;

	public static readonly DependencyProperty MoreButtonVisibilityProperty;

	public static readonly DependencyProperty MoreButtonCommandProperty;

	public static readonly DependencyProperty EmptyTipsForegroundProperty;

	public static readonly DependencyProperty EmptyTipsProperty;

	public static readonly DependencyProperty WaterMarkerVisibilityProperty;

	public static readonly DependencyProperty EmptyTipsVisibilityProperty;

	public static readonly DependencyProperty ToggleButtonVisibilityProperty;

	public static readonly DependencyProperty ToggleButtonIsCheckedBorderBurshProperty;

	public static readonly DependencyProperty ToggleButtonIsCheckedBackgroundProperty;

	public static readonly DependencyProperty ToggleButtonIsCheckedForegroundProperty;

	public static readonly DependencyProperty ToggleButtonIsCheckedArrowBackgroundProperty;

	public static readonly DependencyProperty ToggleButtonBorderBurshProperty;

	public static readonly DependencyProperty ToggleButtonBackgroundProperty;

	public static readonly DependencyProperty ToggleButtonForegroundProperty;

	public static readonly DependencyProperty ToggleButtonArrowBackgroundProperty;

	public static readonly DependencyProperty DropDownBackgroundProperty;

	public static readonly DependencyProperty DropDownBorderBrushProperty;

	public static readonly DependencyProperty DropDownBorderThicknessProperty;

	public static readonly DependencyProperty DropDownItemBackgroundProperty;

	public static readonly DependencyProperty DropDownItemForegroundProperty;

	public static readonly DependencyProperty DropDownItemMouseOverBackgroundProperty;

	public static readonly DependencyProperty DropDownItemMouseOverForegroundProperty;

	public static readonly DependencyProperty DropDownItemSelectedBackgroundProperty;

	public static readonly DependencyProperty DropDownItemSelectedForegroundProperty;

	public static readonly DependencyProperty DorpDownPanelVerticalOffsetProperty;

	public static readonly DependencyProperty DropDownBorderCornerRadiusProperty;

	public static readonly DependencyProperty DropDownBorderPaddingProperty;

	public static readonly DependencyProperty DataLoadingProperty;

	public static readonly DependencyProperty ClearCommandProperty;

	public static readonly DependencyProperty ShowClearProperty;

	public static readonly DependencyProperty ClearButtonVisibilityProperty;

	[DefaultValue(true)]
	public bool IsCaseSensitive
	{
		get
		{
			return (bool)GetValue(IsCaseSensitiveProperty);
		}
		set
		{
			SetValue(IsCaseSensitiveProperty, value);
		}
	}

	[DefaultValue(true)]
	public bool IsSearchable
	{
		get
		{
			return (bool)GetValue(IsSearchableProperty);
		}
		set
		{
			SetValue(IsSearchableProperty, value);
		}
	}

	[DefaultValue(true)]
	public bool IsSortable
	{
		get
		{
			return (bool)GetValue(IsSortableProperty);
		}
		set
		{
			SetValue(IsSortableProperty, value);
		}
	}

	[DefaultValue(true)]
	public bool DropDownOnFocus
	{
		get
		{
			return (bool)GetValue(DropDownOnFocusProperty);
		}
		set
		{
			SetValue(DropDownOnFocusProperty, value);
		}
	}

	private TextBox EditableTextBox => (TextBox)GetTemplateChild("PART_EditableTextBox");

	public Func<object, string, int> FilterCompareDelegate
	{
		get
		{
			return (Func<object, string, int>)GetValue(FilterCompareDelegateProperty);
		}
		set
		{
			SetValue(FilterCompareDelegateProperty, value);
		}
	}

	public Brush EditableTextBoxForeground
	{
		get
		{
			return (Brush)GetValue(EditableTextBoxForegroundProperty);
		}
		set
		{
			SetValue(EditableTextBoxForegroundProperty, value);
		}
	}

	public bool DropDownIsEnaled
	{
		get
		{
			return (bool)GetValue(DropDownIsEnaledProperty);
		}
		set
		{
			SetValue(DropDownIsEnaledProperty, value);
		}
	}

	public CornerRadius CornerRadius
	{
		get
		{
			return (CornerRadius)GetValue(CornerRadiusProperty);
		}
		set
		{
			SetValue(CornerRadiusProperty, value);
		}
	}

	public Visibility MoreButtonVisibility
	{
		get
		{
			return (Visibility)GetValue(MoreButtonVisibilityProperty);
		}
		set
		{
			SetValue(MoreButtonVisibilityProperty, value);
		}
	}

	public ICommand MoreButtonCommand
	{
		get
		{
			return (ICommand)GetValue(MoreButtonCommandProperty);
		}
		set
		{
			SetValue(MoreButtonCommandProperty, value);
		}
	}

	private Button MoreButton => GetTemplateChild("PART_MoreButton") as Button;

	public Brush EmptyTipsForeground
	{
		get
		{
			return (Brush)GetValue(EmptyTipsForegroundProperty);
		}
		set
		{
			SetValue(EmptyTipsForegroundProperty, value);
		}
	}

	public string EmptyTips
	{
		get
		{
			return (string)GetValue(EmptyTipsProperty);
		}
		set
		{
			SetValue(EmptyTipsProperty, value);
		}
	}

	public Visibility WaterMarkerVisibility
	{
		get
		{
			return (Visibility)GetValue(WaterMarkerVisibilityProperty);
		}
		set
		{
			SetValue(WaterMarkerVisibilityProperty, value);
		}
	}

	public Visibility EmptyTipsVisibility
	{
		get
		{
			return (Visibility)GetValue(EmptyTipsVisibilityProperty);
		}
		set
		{
			SetValue(EmptyTipsVisibilityProperty, value);
		}
	}

	public Visibility ToggleButtonVisibility
	{
		get
		{
			return (Visibility)GetValue(ToggleButtonVisibilityProperty);
		}
		set
		{
			SetValue(ToggleButtonVisibilityProperty, value);
		}
	}

	public Brush ToggleButtonIsCheckedBorderBursh
	{
		get
		{
			return (Brush)GetValue(ToggleButtonIsCheckedBorderBurshProperty);
		}
		set
		{
			SetValue(ToggleButtonIsCheckedBorderBurshProperty, value);
		}
	}

	public Brush ToggleButtonIsCheckedBackground
	{
		get
		{
			return (Brush)GetValue(ToggleButtonIsCheckedBackgroundProperty);
		}
		set
		{
			SetValue(ToggleButtonIsCheckedBackgroundProperty, value);
		}
	}

	public Brush ToggleButtonIsCheckedForeground
	{
		get
		{
			return (Brush)GetValue(ToggleButtonIsCheckedForegroundProperty);
		}
		set
		{
			SetValue(ToggleButtonIsCheckedForegroundProperty, value);
		}
	}

	public Brush ToggleButtonIsCheckedArrowBackground
	{
		get
		{
			return (Brush)GetValue(ToggleButtonIsCheckedArrowBackgroundProperty);
		}
		set
		{
			SetValue(ToggleButtonIsCheckedArrowBackgroundProperty, value);
		}
	}

	public Brush ToggleButtonBorderBursh
	{
		get
		{
			return (Brush)GetValue(ToggleButtonBorderBurshProperty);
		}
		set
		{
			SetValue(ToggleButtonBorderBurshProperty, value);
		}
	}

	public Brush ToggleButtonBackground
	{
		get
		{
			return (Brush)GetValue(ToggleButtonBackgroundProperty);
		}
		set
		{
			SetValue(ToggleButtonBackgroundProperty, value);
		}
	}

	public Brush ToggleButtonForeground
	{
		get
		{
			return (Brush)GetValue(ToggleButtonForegroundProperty);
		}
		set
		{
			SetValue(ToggleButtonForegroundProperty, value);
		}
	}

	public Brush ToggleButtonArrowBackground
	{
		get
		{
			return (Brush)GetValue(ToggleButtonArrowBackgroundProperty);
		}
		set
		{
			SetValue(ToggleButtonArrowBackgroundProperty, value);
		}
	}

	public Brush DropDownBackground
	{
		get
		{
			return (Brush)GetValue(DropDownBackgroundProperty);
		}
		set
		{
			SetValue(DropDownBackgroundProperty, value);
		}
	}

	public Brush DropDownBorderBrush
	{
		get
		{
			return (Brush)GetValue(DropDownBorderBrushProperty);
		}
		set
		{
			SetValue(DropDownBorderBrushProperty, value);
		}
	}

	public Thickness DropDownBorderThickness
	{
		get
		{
			return (Thickness)GetValue(DropDownBorderThicknessProperty);
		}
		set
		{
			SetValue(DropDownBorderThicknessProperty, value);
		}
	}

	public Brush DropDownItemBackground
	{
		get
		{
			return (Brush)GetValue(DropDownItemBackgroundProperty);
		}
		set
		{
			SetValue(DropDownItemBackgroundProperty, value);
		}
	}

	public Brush DropDownItemForeground
	{
		get
		{
			return (Brush)GetValue(DropDownItemForegroundProperty);
		}
		set
		{
			SetValue(DropDownItemForegroundProperty, value);
		}
	}

	public Brush DropDownItemMouseOverBackground
	{
		get
		{
			return (Brush)GetValue(DropDownItemMouseOverBackgroundProperty);
		}
		set
		{
			SetValue(DropDownItemMouseOverBackgroundProperty, value);
		}
	}

	public Brush DropDownItemMouseOverForeground
	{
		get
		{
			return (Brush)GetValue(DropDownItemMouseOverForegroundProperty);
		}
		set
		{
			SetValue(DropDownItemMouseOverForegroundProperty, value);
		}
	}

	public Brush DropDownItemSelectedBackground
	{
		get
		{
			return (Brush)GetValue(DropDownItemSelectedBackgroundProperty);
		}
		set
		{
			SetValue(DropDownItemSelectedBackgroundProperty, value);
		}
	}

	public Brush DropDownItemSelectedForeground
	{
		get
		{
			return (Brush)GetValue(DropDownItemSelectedForegroundProperty);
		}
		set
		{
			SetValue(DropDownItemSelectedForegroundProperty, value);
		}
	}

	public double DorpDownPanelVerticalOffset
	{
		get
		{
			return (double)GetValue(DorpDownPanelVerticalOffsetProperty);
		}
		set
		{
			SetValue(DorpDownPanelVerticalOffsetProperty, value);
		}
	}

	public CornerRadius DropDownBorderCornerRadius
	{
		get
		{
			return (CornerRadius)GetValue(DropDownBorderCornerRadiusProperty);
		}
		set
		{
			SetValue(DropDownBorderCornerRadiusProperty, value);
		}
	}

	public Thickness DropDownBorderPadding
	{
		get
		{
			return (Thickness)GetValue(DropDownBorderPaddingProperty);
		}
		set
		{
			SetValue(DropDownBorderPaddingProperty, value);
		}
	}

	public bool DataLoading
	{
		get
		{
			return (bool)GetValue(DataLoadingProperty);
		}
		set
		{
			SetValue(DataLoadingProperty, value);
		}
	}

	public ICommand ClearCommand
	{
		get
		{
			return (ICommand)GetValue(ClearCommandProperty);
		}
		set
		{
			SetValue(ClearCommandProperty, value);
		}
	}

	public bool ShowClear
	{
		get
		{
			return (bool)GetValue(ShowClearProperty);
		}
		set
		{
			SetValue(ShowClearProperty, value);
		}
	}

	public Visibility ClearButtonVisibility
	{
		get
		{
			return (Visibility)GetValue(ClearButtonVisibilityProperty);
		}
		set
		{
			SetValue(ClearButtonVisibilityProperty, value);
		}
	}

	public event RoutedEventHandler TextChanged
	{
		add
		{
			AddHandler(TextChangedEvent, value);
		}
		remove
		{
			RemoveHandler(TextChangedEvent, value);
		}
	}

	public LComboBox()
	{
		DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(LComboBox));
		ClearCommand = new ReplayCommand(FireClearCommand);
		base.Items.SortDescriptions.Clear();
		base.Items.SortDescriptions.Add(new SortDescription("IsUsed", ListSortDirection.Descending));
		base.Items.SortDescriptions.Add(new SortDescription("ItemText", ListSortDirection.Ascending));
		dependencyPropertyDescriptor.AddValueChanged(this, OnTextChanged);
	}

	protected virtual void OnIsCaseSensitiveChanged(object sender, EventArgs e)
	{
		if (IsCaseSensitive)
		{
			base.IsTextSearchEnabled = false;
		}
		RefreshAndShowDropDown();
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		UpdateDataLoadingStates(useTransitions: true);
		RegisterMoreButtonClickHandler();
	}

	private void ClearFilter()
	{
		RefreshAndShowDropDown();
		base.Text = string.Empty;
	}

	protected override void OnGotFocus(RoutedEventArgs e)
	{
		base.OnGotFocus(e);
		if (base.ItemsSource == null || (_collView as ListCollectionView).Count == 0)
		{
			base.IsDropDownOpen = false;
		}
		else if (DropDownOnFocus)
		{
			base.IsDropDownOpen = true;
		}
	}

	private void RefreshAndShowDropDown()
	{
		if ((_collView as ListCollectionView).Count == 0)
		{
			base.IsDropDownOpen = false;
		}
		else if (DropDownIsEnaled)
		{
			Refresh();
			if ((_collView as ListCollectionView).Count == 0)
			{
				base.IsDropDownOpen = false;
			}
			else
			{
				base.IsDropDownOpen = true;
			}
		}
	}

	private void Refresh()
	{
		if (base.ItemsSource != null)
		{
			_collView = CollectionViewSource.GetDefaultView(base.ItemsSource);
			_collView.Refresh();
		}
	}

	private bool FilterPredicate(object value)
	{
		if (FilterCompareDelegate != null)
		{
			return FilterCompareDelegate(value, base.Text) == 0;
		}
		if (value == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(base.Text))
		{
			return true;
		}
		if (string.IsNullOrEmpty(base.DisplayMemberPath))
		{
			return true;
		}
		return value.GetType().GetProperty(base.DisplayMemberPath).GetValue(value, null)
			.ToString()
			.ToLower()
			.Contains(base.Text.ToLower());
	}

	public static void FilterCompareDelegateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		(d as LComboBox).Refresh();
	}

	protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
	{
		if (oldValue != null)
		{
			CollectionViewSource.GetDefaultView(oldValue).Filter = null;
		}
		if (newValue != null)
		{
			_collView = CollectionViewSource.GetDefaultView(newValue);
			_collView.Filter = FilterPredicate;
		}
		base.OnItemsSourceChanged(oldValue, newValue);
	}

	private void OnTextChanged(object sender, EventArgs e)
	{
		RaiseTextChangedEvent(new TextChangedRoutedEventArgs(base.Text, TextChanged));
		if (string.IsNullOrEmpty(base.Text))
		{
			WaterMarkerVisibility = Visibility.Visible;
			ClearButtonVisibility = Visibility.Collapsed;
			VisualStateManager.GoToState(this, "SelectionEmpty", useTransitions: true);
		}
		else
		{
			WaterMarkerVisibility = Visibility.Collapsed;
			if (ShowClear)
			{
				ClearButtonVisibility = Visibility.Visible;
			}
			VisualStateManager.GoToState(this, "SelectionNotEmpty", useTransitions: true);
		}
		if (base.IsEditable && _collView != null)
		{
			if (_collView.Filter != new Predicate<object>(FilterPredicate))
			{
				_collView.Filter = FilterPredicate;
			}
			if (string.IsNullOrEmpty(base.Text))
			{
				base.SelectedIndex = -1;
				Refresh();
			}
			else
			{
				RefreshAndShowDropDown();
			}
		}
	}

	private void RaiseTextChangedEvent(TextChangedRoutedEventArgs args)
	{
		RaiseEvent(args);
	}

	protected override void OnPreviewKeyDown(KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Tab:
		case Key.Return:
			base.IsDropDownOpen = false;
			break;
		case Key.Escape:
			ClearFilter();
			base.IsDropDownOpen = true;
			return;
		case Key.Up:
		case Key.Down:
			base.IsDropDownOpen = true;
			EditableTextBox.SelectionStart = int.MaxValue;
			break;
		}
		base.OnPreviewKeyDown(e);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.Key == Key.Escape)
		{
			ClearFilter();
			base.IsDropDownOpen = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);
		if (e.Property.Equals(System.Windows.Controls.Primitives.Selector.SelectedItemProperty))
		{
			UpdateSelectionEmptyStates(useTransitions: true);
		}
	}

	private void RegisterMoreButtonClickHandler()
	{
		if (MoreButton == null)
		{
			return;
		}
		MoreButton.Click += delegate
		{
			MoreButton.Dispatcher.BeginInvoke((Action)delegate
			{
				RefreshAndShowDropDown();
			}, null);
		};
	}

	private static void DataLoadingChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		LComboBox obj2 = (LComboBox)obj;
		bool useTransitions = (bool)e.NewValue;
		obj2.UpdateDataLoadingStates(useTransitions);
	}

	protected override void OnSelectionChanged(SelectionChangedEventArgs e)
	{
		base.OnSelectionChanged(e);
	}

	private void UpdateDataLoadingStates(bool useTransitions)
	{
		if (DataLoading)
		{
			VisualStateManager.GoToState(this, "DataLoading", useTransitions);
		}
		else
		{
			VisualStateManager.GoToState(this, "DataLoadingUnloading", useTransitions);
		}
	}

	private void UpdateSelectionEmptyStates(bool useTransitions)
	{
		if (base.SelectedIndex < 0)
		{
			VisualStateManager.GoToState(this, "SelectionEmpty", useTransitions);
		}
		else
		{
			VisualStateManager.GoToState(this, "SelectionNotEmpty", useTransitions);
		}
	}

	private void FireClearCommand(object data)
	{
		base.SelectedItem = null;
		base.SelectedIndex = -1;
		if (!string.IsNullOrEmpty(EditableTextBox.Text))
		{
			EditableTextBox.Clear();
		}
	}

	protected override void OnDropDownOpened(EventArgs e)
	{
		base.OnDropDownOpened(e);
		EditableTextBox.SelectionStart = base.Text.Length;
	}

	static LComboBox()
	{
		TextChanged = EventManager.RegisterRoutedEvent("TextChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LComboBox));
		CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(LComboBox), new PropertyMetadata(new CornerRadius(0.0)));
		MoreButtonVisibilityProperty = DependencyProperty.Register("MoreButtonVisibility", typeof(Visibility), typeof(LComboBox), new PropertyMetadata(Visibility.Collapsed));
		MoreButtonCommandProperty = DependencyProperty.Register("MoreButtonCommand", typeof(ICommand), typeof(LComboBox), new PropertyMetadata(null));
		EmptyTipsForegroundProperty = DependencyProperty.Register("EmptyTipsForeground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		EmptyTipsProperty = DependencyProperty.Register("EmptyTips", typeof(string), typeof(LComboBox), new PropertyMetadata(string.Empty));
		WaterMarkerVisibilityProperty = DependencyProperty.Register("WaterMarkerVisibility", typeof(Visibility), typeof(LComboBox), new PropertyMetadata(Visibility.Visible));
		EmptyTipsVisibilityProperty = DependencyProperty.Register("EmptyTipsVisibility", typeof(Visibility), typeof(LComboBox), new PropertyMetadata(Visibility.Visible));
		ToggleButtonVisibilityProperty = DependencyProperty.Register("ToggleButtonVisibility", typeof(Visibility), typeof(LComboBox), new PropertyMetadata(Visibility.Visible));
		ToggleButtonIsCheckedBorderBurshProperty = DependencyProperty.Register("ToggleButtonIsCheckedBorderBursh", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		ToggleButtonIsCheckedBackgroundProperty = DependencyProperty.Register("ToggleButtonIsCheckedBackground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		ToggleButtonIsCheckedForegroundProperty = DependencyProperty.Register("ToggleButtonIsCheckedForeground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		ToggleButtonIsCheckedArrowBackgroundProperty = DependencyProperty.Register("ToggleButtonIsCheckedArrowBackground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		ToggleButtonBorderBurshProperty = DependencyProperty.Register("ToggleButtonBorderBursh", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		ToggleButtonBackgroundProperty = DependencyProperty.Register("ToggleButtonBackground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		ToggleButtonForegroundProperty = DependencyProperty.Register("ToggleButtonForeground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		ToggleButtonArrowBackgroundProperty = DependencyProperty.Register("ToggleButtonArrowBackground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		DropDownBackgroundProperty = DependencyProperty.Register("DropDownBackground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		DropDownBorderBrushProperty = DependencyProperty.Register("DropDownBorderBrush", typeof(Brush), typeof(LComboBox), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
		DropDownBorderThicknessProperty = DependencyProperty.Register("DropDownBorderThickness", typeof(Thickness), typeof(LComboBox), new PropertyMetadata(new Thickness(0.0)));
		DropDownItemBackgroundProperty = DependencyProperty.Register("DropDownItemBackground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		DropDownItemForegroundProperty = DependencyProperty.Register("DropDownItemForeground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		DropDownItemMouseOverBackgroundProperty = DependencyProperty.Register("DropDownItemMouseOverBackground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		DropDownItemMouseOverForegroundProperty = DependencyProperty.Register("DropDownItemMouseOverForeground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		DropDownItemSelectedBackgroundProperty = DependencyProperty.Register("DropDownItemSelectedBackground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		DropDownItemSelectedForegroundProperty = DependencyProperty.Register("DropDownItemSelectedForeground", typeof(Brush), typeof(LComboBox), new PropertyMetadata(null));
		DorpDownPanelVerticalOffsetProperty = DependencyProperty.Register("DorpDownPanelVerticalOffset", typeof(double), typeof(LComboBox), new PropertyMetadata(0.0));
		DropDownBorderCornerRadiusProperty = DependencyProperty.Register("DropDownBorderCornerRadius", typeof(CornerRadius), typeof(LComboBox), new PropertyMetadata(new CornerRadius(0.0)));
		DropDownBorderPaddingProperty = DependencyProperty.Register("DropDownBorderPadding", typeof(Thickness), typeof(LComboBox), new PropertyMetadata(new Thickness(0.0)));
		DataLoadingProperty = DependencyProperty.Register("DataLoading", typeof(bool), typeof(LComboBox), new PropertyMetadata(false, DataLoadingChangedCallback));
		ClearCommandProperty = DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(LComboBox), new PropertyMetadata(null));
		ShowClearProperty = DependencyProperty.Register("ShowClear", typeof(bool), typeof(LComboBox), new PropertyMetadata(false));
		ClearButtonVisibilityProperty = DependencyProperty.Register("ClearButtonVisibility", typeof(Visibility), typeof(LComboBox), new PropertyMetadata(Visibility.Collapsed));
	}
}
