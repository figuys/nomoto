using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace lenovo.mbg.service.lmsa;

public partial class MessageBox_Common : Window, IComponentConnector
{
	private Storyboard sbCarch = new Storyboard();

	private MessageBoxResult messageBoxChooseResult = MessageBoxResult.None;

	public static readonly DependencyProperty StrTitleTipProperty = DependencyProperty.Register("StrTitleTip", typeof(string), typeof(MessageBox_Common), new PropertyMetadata(string.Empty));

	public static readonly DependencyProperty StrButtonTitle_OKProperty = DependencyProperty.Register("StrButtonTitle_OK", typeof(string), typeof(MessageBox_Common), new PropertyMetadata("K0327"));

	public static readonly DependencyProperty StrButtonTitle_CancelProperty = DependencyProperty.Register("StrButtonTitle_Cancel", typeof(string), typeof(MessageBox_Common), new PropertyMetadata("K0208"));

	public static readonly DependencyProperty StrCashTipProperty = DependencyProperty.Register("StrCashTip", typeof(string), typeof(MessageBox_Common), new PropertyMetadata(string.Empty));

	public TypeItems.MessageBoxType MsgTypes
	{
		set
		{
			switch (value)
			{
			case TypeItems.MessageBoxType.OK:
				gridCash.Visibility = Visibility.Collapsed;
				TxbCancel.Visibility = Visibility.Hidden;
				TxbOk.Visibility = Visibility.Visible;
				TxbOk.HorizontalAlignment = HorizontalAlignment.Center;
				TxbOk.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
				break;
			case TypeItems.MessageBoxType.OKCancel:
			case TypeItems.MessageBoxType.YesNo:
				gridCash.Visibility = Visibility.Collapsed;
				TxbCancel.Visibility = Visibility.Visible;
				TxbCancel.HorizontalAlignment = HorizontalAlignment.Right;
				TxbOk.Visibility = Visibility.Visible;
				TxbOk.HorizontalAlignment = HorizontalAlignment.Left;
				TxbOk.Margin = new Thickness(50.0, 0.0, 0.0, 0.0);
				break;
			case TypeItems.MessageBoxType.YesNoCancel:
				break;
			case TypeItems.MessageBoxType.Cash_Load:
			case TypeItems.MessageBoxType.Cash_Wait:
				gridCash.Visibility = Visibility.Visible;
				TxbOk.Visibility = Visibility.Collapsed;
				TxbCancel.Visibility = Visibility.Collapsed;
				StrCashTip = "K0348";
				ExecuteAnimation();
				break;
			}
		}
	}

	public MessageBoxResult MessageBoxChooseResult
	{
		get
		{
			return messageBoxChooseResult;
		}
		set
		{
			messageBoxChooseResult = value;
		}
	}

	public string StrTitleTip
	{
		get
		{
			return (string)GetValue(StrTitleTipProperty);
		}
		set
		{
			SetValue(StrTitleTipProperty, value);
		}
	}

	public string StrButtonTitle_OK
	{
		get
		{
			return (string)GetValue(StrButtonTitle_OKProperty);
		}
		set
		{
			SetValue(StrButtonTitle_OKProperty, value);
		}
	}

	public string StrButtonTitle_Cancel
	{
		get
		{
			return (string)GetValue(StrButtonTitle_CancelProperty);
		}
		set
		{
			SetValue(StrButtonTitle_CancelProperty, value);
		}
	}

	public string StrCashTip
	{
		get
		{
			return (string)GetValue(StrCashTipProperty);
		}
		set
		{
			SetValue(StrCashTipProperty, value);
		}
	}

	public MessageBox_Common(Window ShowOwner, TypeItems.MessageBoxType MsgType, string strTitleTip, string strButtonTitle_OK, string strButtonTitle_Cancel)
	{
		InitializeComponent();
		MsgTypes = MsgType;
		StrTitleTip = strTitleTip;
		StrButtonTitle_OK = strButtonTitle_OK;
		StrButtonTitle_Cancel = strButtonTitle_Cancel;
		base.Closed += MessageBox_Common_Closed;
		base.Loaded += MessageBox_Common_Loaded;
	}

	private void MessageBox_Common_Loaded(object sender, RoutedEventArgs e)
	{
		base.Left += 56.0;
	}

	private void MessageBox_Common_Closed(object sender, EventArgs e)
	{
		sbCarch.Stop();
	}

	private void TxbCancel_MouseDown(object sender, MouseButtonEventArgs e)
	{
	}

	private void ExecuteAnimation()
	{
		imgCash.RenderTransform = rotate;
		DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, 360.0, new Duration(TimeSpan.FromSeconds(1.0)));
		Storyboard.SetTarget(doubleAnimation, imgCash);
		DependencyProperty[] array = new DependencyProperty[2]
		{
			UIElement.RenderTransformProperty,
			RotateTransform.AngleProperty
		};
		object[] pathParameters = array;
		Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(0).(1)", pathParameters));
		sbCarch.Children.Add(doubleAnimation);
		sbCarch.RepeatBehavior = RepeatBehavior.Forever;
		sbCarch.Begin();
	}

	private void TxbOk_Click(object sender, RoutedEventArgs e)
	{
		messageBoxChooseResult = MessageBoxResult.OK;
		Close();
	}

	private void TxbCancel_Click(object sender, RoutedEventArgs e)
	{
		messageBoxChooseResult = MessageBoxResult.Cancel;
		Close();
	}
}
