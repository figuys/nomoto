using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.Login.Business;

namespace lenovo.mbg.service.lmsa.Login.View;

public partial class VerificationCodeView : UserControl, IComponentConnector
{
	public static readonly DependencyProperty VerificationCodeListProperty = DependencyProperty.Register("VerificationCodeList", typeof(List<string>), typeof(VerificationCodeView), new PropertyMetadata(null, PropertyChangedCallbackHandler));

	public List<string> VerificationCodeList
	{
		get
		{
			return (List<string>)GetValue(VerificationCodeListProperty);
		}
		set
		{
			SetValue(VerificationCodeListProperty, value);
		}
	}

	public VerificationCodeView()
	{
		InitializeComponent();
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
	}

	private static void PropertyChangedCallbackHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (e.NewValue != null && e.NewValue is List<string> codes)
		{
			VerificationCodeView verificationCodeView = d as VerificationCodeView;
			verificationCodeView.RefreshCode(codes);
		}
	}

	private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
	{
		List<string> codes = (VerificationCodeList = VerificationCodeFactory.CeateNewCode(4, VerificationCodeFactory.PredefinedExceptChars));
		RefreshCode(codes);
	}

	private void RefreshCode(List<string> codes)
	{
		FontSizeConverter fontSizeConverter = new FontSizeConverter();
		TextBlock textBlock = new TextBlock();
		textBlock.FontSize = 16.0;
		textBlock.Foreground = base.Foreground;
		textBlock.TextDecorations = TextDecorations.Strikethrough;
		textBlock.FontStyle = FontStyles.Italic;
		Canvas.SetTop(textBlock, 2.0);
		Canvas.SetLeft(textBlock, 9.0);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string code in codes)
		{
			stringBuilder.Append(code).Append(' ');
		}
		textBlock.Text = stringBuilder.ToString();
		paint.Children.Clear();
		paint.Children.Add(textBlock);
	}
}
