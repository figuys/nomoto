using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;

namespace lenovo.themes.generic.Controls;

public partial class CalendarTextBox : UserControl, IComponentConnector
{
	private Popup popup;

	private Calendar calendar;

	public static readonly DependencyProperty SelectedDateTimeProperty = DependencyProperty.Register("SelectedDateTime", typeof(DateTime?), typeof(CalendarTextBox), new PropertyMetadata(null, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
	{
		if (sender is CalendarTextBox calendarTextBox)
		{
			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs = e;
			if (dependencyPropertyChangedEventArgs.NewValue != null)
			{
				calendarTextBox.txtBoxCalendar.Text = ((DateTime?)dependencyPropertyChangedEventArgs.NewValue).Value.ToString("d");
			}
		}
	}));

	public static readonly DependencyProperty SelectedDatesChangedCommandProperty = DependencyProperty.Register("SelectedDatesChangedCommand", typeof(ICommand), typeof(CalendarTextBox), new PropertyMetadata(null));

	public DateTime? SelectedDateTime
	{
		get
		{
			return (DateTime?)GetValue(SelectedDateTimeProperty);
		}
		set
		{
			SetValue(SelectedDateTimeProperty, value);
		}
	}

	public ICommand SelectedDatesChangedCommand
	{
		get
		{
			return (ICommand)GetValue(SelectedDatesChangedCommandProperty);
		}
		set
		{
			SetValue(SelectedDatesChangedCommandProperty, value);
		}
	}

	public CalendarTextBox()
	{
		InitializeComponent();
		base.Language = XmlLanguage.GetLanguage(LMSAContext.CurrentLanguage);
	}

	protected override void OnLostMouseCapture(MouseEventArgs e)
	{
		base.OnLostMouseCapture(e);
	}

	public override void OnApplyTemplate()
	{
		txtBoxCalendar.ApplyTemplate();
		popup = txtBoxCalendar.Template.FindName("popupCalendar", txtBoxCalendar) as Popup;
		calendar = txtBoxCalendar.Template.FindName("calendar", txtBoxCalendar) as Calendar;
		calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;
		base.OnApplyTemplate();
	}

	private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
	{
		if (calendar.SelectedDate.HasValue)
		{
			DateTime value = calendar.SelectedDate.Value;
			popup.IsOpen = false;
			if (SelectedDatesChangedCommand != null && SelectedDatesChangedCommand.CanExecute(value))
			{
				SelectedDatesChangedCommand.Execute(value);
				e.Handled = true;
			}
		}
	}

	private void txtBoxCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		popup.IsOpen = true;
	}
}
