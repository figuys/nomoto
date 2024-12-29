using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace lenovo.mbg.service.lmsa.tips;

public class TextBoxWithEllipsis : TextBox
{
	private int _lastFitLen;

	private int _lastLongLen;

	private int _curLen;

	private bool _externalChange = true;

	private bool _internalEnabled = true;

	private string _longText = "";

	private bool _externalEnabled = true;

	private bool _useLongTextForToolTip;

	private EllipsisPlacement _placement;

	public string LongText
	{
		get
		{
			return _longText;
		}
		set
		{
			_longText = value ?? "";
			PrepareForLayout();
		}
	}

	public EllipsisPlacement EllipsisPlacement
	{
		get
		{
			return _placement;
		}
		set
		{
			if (_placement != value)
			{
				_placement = value;
				if (_DoEllipsis)
				{
					PrepareForLayout();
				}
			}
		}
	}

	public bool IsEllipsisEnabled
	{
		get
		{
			return _externalEnabled;
		}
		set
		{
			_externalEnabled = value;
			PrepareForLayout();
			if (_DoEllipsis)
			{
				TextBoxWithEllipsis_LayoutUpdated(this, EventArgs.Empty);
			}
		}
	}

	public bool UseLongTextForToolTip
	{
		get
		{
			return _useLongTextForToolTip;
		}
		set
		{
			if (_useLongTextForToolTip == value)
			{
				return;
			}
			_useLongTextForToolTip = value;
			if (value)
			{
				if (base.ExtentWidth > base.ViewportWidth || base.Text != _longText)
				{
					base.ToolTip = _longText;
				}
			}
			else if (_longText.Equals(base.ToolTip))
			{
				base.ToolTip = null;
			}
		}
	}

	public double FudgePix { get; set; }

	private bool _DoEllipsis
	{
		get
		{
			if (IsEllipsisEnabled)
			{
				return _internalEnabled;
			}
			return false;
		}
	}

	public TextBoxWithEllipsis()
	{
		base.IsReadOnlyCaretVisible = true;
		IsEllipsisEnabled = true;
		UseLongTextForToolTip = true;
		FudgePix = 3.0;
		_placement = EllipsisPlacement.Right;
		_internalEnabled = true;
		base.LayoutUpdated += TextBoxWithEllipsis_LayoutUpdated;
		base.SizeChanged += TextBoxWithEllipsis_SizeChanged;
	}

	protected override void OnTextChanged(TextChangedEventArgs e)
	{
		if (_externalChange)
		{
			_longText = base.Text ?? "";
			if (UseLongTextForToolTip)
			{
				base.ToolTip = _longText;
			}
			PrepareForLayout();
			base.OnTextChanged(e);
		}
	}

	protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
	{
		_internalEnabled = false;
		SetText(_longText);
		base.OnGotKeyboardFocus(e);
	}

	protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
	{
		_internalEnabled = true;
		PrepareForLayout();
		base.OnLostKeyboardFocus(e);
	}

	private void SetText(string text)
	{
		if (base.Text != text)
		{
			_externalChange = false;
			base.Text = text;
			_externalChange = true;
		}
	}

	private void PrepareForLayout()
	{
		_lastFitLen = 0;
		_lastLongLen = _longText.Length;
		_curLen = _longText.Length;
		SetText(_longText);
	}

	private void TextBoxWithEllipsis_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (_DoEllipsis && e.NewSize.Width != e.PreviousSize.Width)
		{
			PrepareForLayout();
		}
	}

	private void TextBoxWithEllipsis_LayoutUpdated(object sender, EventArgs e)
	{
		if (_DoEllipsis)
		{
			if (base.ViewportWidth + FudgePix < base.ExtentWidth)
			{
				_lastLongLen = _curLen;
			}
			else
			{
				_lastFitLen = _curLen;
			}
			int num = (_lastFitLen + _lastLongLen) / 2;
			if (_curLen == num)
			{
				if (UseLongTextForToolTip)
				{
					if (base.Text == _longText)
					{
						base.ToolTip = null;
					}
					else
					{
						base.ToolTip = _longText;
					}
				}
			}
			else
			{
				_curLen = num;
				CalcText();
			}
		}
		else if (UseLongTextForToolTip)
		{
			if (base.ViewportWidth < base.ExtentWidth)
			{
				base.ToolTip = _longText;
			}
			else
			{
				base.ToolTip = null;
			}
		}
	}

	private void CalcText()
	{
		switch (_placement)
		{
		case EllipsisPlacement.Right:
			SetText(_longText.Substring(0, _curLen) + "…");
			break;
		case EllipsisPlacement.Center:
		{
			int num = _curLen / 2;
			int num2 = _curLen - num;
			SetText(_longText.Substring(0, num) + "…" + _longText.Substring(_longText.Length - num2));
			break;
		}
		case EllipsisPlacement.Left:
		{
			int startIndex = _longText.Length - _curLen;
			SetText("…" + _longText.Substring(startIndex));
			break;
		}
		default:
			throw new Exception("Unexpected switch value: " + _placement);
		}
	}
}
