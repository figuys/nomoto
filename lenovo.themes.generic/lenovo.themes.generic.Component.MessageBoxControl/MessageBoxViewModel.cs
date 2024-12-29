using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Component.MessageBoxControl;

public class MessageBoxViewModel : ViewModelBase
{
	private double m_width;

	private double m_contentHeight;

	private string m_title;

	private string m_tips;

	private ReplayCommand m_okCommand;

	public double ContentWidth
	{
		get
		{
			return m_width;
		}
		set
		{
			if (m_width != value)
			{
				m_width = value;
				OnPropertyChanged("ContentWidth");
			}
		}
	}

	public double ContentHeight
	{
		get
		{
			return m_contentHeight;
		}
		set
		{
			if (m_contentHeight != value)
			{
				m_contentHeight = value;
				OnPropertyChanged("ContentHeight");
			}
		}
	}

	public string Title
	{
		get
		{
			return m_title;
		}
		set
		{
			if (!(m_title == value))
			{
				m_title = value;
				OnPropertyChanged("Title");
			}
		}
	}

	public string Tips
	{
		get
		{
			return m_tips;
		}
		set
		{
			if (!(m_tips == value))
			{
				m_tips = value;
				OnPropertyChanged("Tips");
			}
		}
	}

	public ReplayCommand OkCommand
	{
		get
		{
			return m_okCommand;
		}
		set
		{
			if (m_okCommand != value)
			{
				m_okCommand = value;
				OnPropertyChanged("OkCommand");
			}
		}
	}

	public MessageBoxViewModel(string title, string tips)
	{
		OkCommand = new ReplayCommand(OkCommandHandler);
		m_title = title;
		m_tips = tips;
	}

	private void OkCommandHandler(object e)
	{
		(e as Window).Close();
	}
}
