using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace lenovo.themes.generic.ControlsV6;

public class AutoFilteredComboBox : ComboBox
{
	private int _silenceEvents;

	private ICollectionView _collView;

	private bool _keyboardSelectionGuard;

	public static readonly DependencyProperty IsCaseSensitiveProperty = DependencyProperty.Register("IsCaseSensitive", typeof(bool), typeof(AutoFilteredComboBox), new UIPropertyMetadata(false));

	public static readonly DependencyProperty DropDownOnFocusProperty = DependencyProperty.Register("DropDownOnFocus", typeof(bool), typeof(AutoFilteredComboBox), new UIPropertyMetadata(true));

	[Description("The way the combo box treats the case sensitivity of typed text")]
	[Category("AutoFiltered ComboBox")]
	[DefaultValue(true)]
	public bool IsCaseSensitive
	{
		[DebuggerStepThrough]
		get
		{
			return (bool)GetValue(IsCaseSensitiveProperty);
		}
		[DebuggerStepThrough]
		set
		{
			SetValue(IsCaseSensitiveProperty, value);
		}
	}

	[Description("The way the combo box behaves when it receives focus")]
	[Category("AutoFiltered ComboBox")]
	[DefaultValue(true)]
	public bool DropDownOnFocus
	{
		[DebuggerStepThrough]
		get
		{
			return (bool)GetValue(DropDownOnFocusProperty);
		}
		[DebuggerStepThrough]
		set
		{
			SetValue(DropDownOnFocusProperty, value);
		}
	}

	private TextBox EditableTextBox => (TextBox)GetTemplateChild("PART_EditableTextBox");

	private Popup ItemsPopup => (Popup)GetTemplateChild("PART_Popup");

	private ScrollViewer ItemsScrollViewer
	{
		get
		{
			if (!(ItemsPopup.FindName("PopupBorder") is Border border))
			{
				return null;
			}
			return border.Child as ScrollViewer;
		}
	}

	public AutoFilteredComboBox()
	{
		DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(AutoFilteredComboBox)).AddValueChanged(this, OnTextChanged);
		RegisterIsCaseSensitiveChangeNotification();
	}

	protected virtual void OnIsCaseSensitiveChanged(object sender, EventArgs e)
	{
		if (IsCaseSensitive)
		{
			base.IsTextSearchEnabled = false;
		}
		RefreshFilter();
	}

	private void RegisterIsCaseSensitiveChangeNotification()
	{
		DependencyPropertyDescriptor.FromProperty(IsCaseSensitiveProperty, typeof(AutoFilteredComboBox)).AddValueChanged(this, OnIsCaseSensitiveChanged);
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		EditableTextBox.SelectionChanged += EditableTextBox_SelectionChanged;
		ItemsPopup.Focusable = true;
	}

	private void EditableTextBox_SelectionChanged(object sender, RoutedEventArgs e)
	{
		TextBox obj = (TextBox)e.OriginalSource;
		_ = obj.SelectionStart;
		_ = obj.SelectionLength;
		ScrollItemsToTop();
	}

	private void ClearFilter()
	{
		RefreshFilter();
		base.Text = "";
		ScrollItemsToTop();
	}

	private void SilenceEvents()
	{
		_silenceEvents++;
	}

	private void UnSilenceEvents()
	{
		if (_silenceEvents > 0)
		{
			_silenceEvents--;
		}
	}

	protected override void OnGotFocus(RoutedEventArgs e)
	{
		base.OnGotFocus(e);
		if (base.ItemsSource != null && DropDownOnFocus)
		{
			base.IsDropDownOpen = true;
		}
	}

	protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
	{
		base.OnPreviewLostKeyboardFocus(e);
	}

	private void ScrollItemsToTop()
	{
		ItemsScrollViewer?.ScrollToTop();
	}

	private void RefreshFilter()
	{
		if (base.ItemsSource != null)
		{
			_collView = CollectionViewSource.GetDefaultView(base.ItemsSource);
			_collView.Refresh();
			base.IsDropDownOpen = true;
		}
	}

	private bool FilterPredicate(object value)
	{
		if (value == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(base.Text))
		{
			return true;
		}
		string input = (string.IsNullOrEmpty(base.DisplayMemberPath) ? value.ToString() : value.GetType().GetProperty(base.DisplayMemberPath).GetValue(value, null)
			.ToString());
		return Regex.IsMatch(input, base.Text, RegexOptions.IgnoreCase);
	}

	protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
	{
		if (newValue != null)
		{
			_collView = CollectionViewSource.GetDefaultView(newValue);
			ICollectionView collView = _collView;
			collView.Filter = (Predicate<object>)Delegate.Combine(collView.Filter, new Predicate<object>(FilterPredicate));
		}
		if (oldValue != null)
		{
			_collView = CollectionViewSource.GetDefaultView(oldValue);
			ICollectionView collView2 = _collView;
			collView2.Filter = (Predicate<object>)Delegate.Remove(collView2.Filter, new Predicate<object>(FilterPredicate));
		}
		base.OnItemsSourceChanged(oldValue, newValue);
	}

	private void OnTextChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(base.Text))
		{
			base.SelectedIndex = -1;
		}
		RefreshFilter();
		EditableTextBox.SelectionStart = base.Text.Length;
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
			_keyboardSelectionGuard = false;
			UnSilenceEvents();
			ClearFilter();
			base.IsDropDownOpen = true;
			return;
		case Key.Up:
		case Key.Down:
			base.IsDropDownOpen = true;
			if (!_keyboardSelectionGuard)
			{
				_keyboardSelectionGuard = true;
				SilenceEvents();
			}
			EditableTextBox.SelectionStart = base.Text.Length;
			break;
		}
		base.OnPreviewKeyDown(e);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.Key == Key.Escape)
		{
			_keyboardSelectionGuard = false;
			UnSilenceEvents();
			ClearFilter();
			base.IsDropDownOpen = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}
}
