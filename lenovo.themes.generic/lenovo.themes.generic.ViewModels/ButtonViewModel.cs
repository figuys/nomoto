using System.Windows;
using System.Windows.Media;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class ButtonViewModel : ViewModelBase
{
	private double _fontSize;

	private Brush _foreground;

	private ButtonSatus m_ButtonSatus;

	private Brush _background;

	private Brush _mouseOverBackground;

	private Brush _mouseOverForeground;

	private Brush _disabeledBackground;

	private bool _isEnabled = true;

	private object _content;

	private Visibility _visibility;

	private ReplayCommand _clickCommand;

	public double FontSize
	{
		get
		{
			return _fontSize;
		}
		set
		{
			if (_fontSize != value)
			{
				_fontSize = value;
				OnPropertyChanged("FontSize");
			}
		}
	}

	public Brush Foreground
	{
		get
		{
			return _foreground;
		}
		set
		{
			if (_foreground != value)
			{
				_foreground = value;
				OnPropertyChanged("Foreground");
			}
		}
	}

	public ButtonSatus ButtonSatus
	{
		get
		{
			return m_ButtonSatus;
		}
		set
		{
			m_ButtonSatus = value;
			OnPropertyChanged("ButtonSatus");
		}
	}

	public Brush Background
	{
		get
		{
			return _background;
		}
		set
		{
			if (_background != value)
			{
				_background = value;
				OnPropertyChanged("Background");
			}
		}
	}

	public Brush MouseOverBackground
	{
		get
		{
			if (_mouseOverBackground == null)
			{
				return Background;
			}
			return _mouseOverBackground;
		}
		set
		{
			if (_mouseOverBackground != value)
			{
				_mouseOverBackground = value;
				OnPropertyChanged("MouseOverBackground");
			}
		}
	}

	public Brush MouseOverForeground
	{
		get
		{
			if (_mouseOverForeground == null)
			{
				return Foreground;
			}
			return _mouseOverForeground;
		}
		set
		{
			if (_mouseOverForeground != value)
			{
				_mouseOverForeground = value;
				OnPropertyChanged("MouseOverForeground");
			}
		}
	}

	public Brush DisabledBackground
	{
		get
		{
			return _disabeledBackground;
		}
		set
		{
			if (_disabeledBackground != value)
			{
				_disabeledBackground = value;
				OnPropertyChanged("DisabledBackground");
			}
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			if (_isEnabled != value)
			{
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
	}

	public object Content
	{
		get
		{
			return _content;
		}
		set
		{
			if (_content != value)
			{
				_content = value;
				OnPropertyChanged("Content");
			}
		}
	}

	public Visibility Visibility
	{
		get
		{
			return _visibility;
		}
		set
		{
			if (_visibility != value)
			{
				_visibility = value;
				OnPropertyChanged("Visibility");
			}
		}
	}

	public ReplayCommand ClickCommand
	{
		get
		{
			return _clickCommand;
		}
		set
		{
			if (_clickCommand != value)
			{
				_clickCommand = value;
				OnPropertyChanged("ReplayCommand");
			}
		}
	}

	public object Tag { get; set; }
}
