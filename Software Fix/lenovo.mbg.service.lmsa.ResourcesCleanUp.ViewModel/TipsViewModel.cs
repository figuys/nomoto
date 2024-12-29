using System.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ResourcesCleanUp.ViewModel;

public class TipsViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private string m_tips;

	private ReplayCommand m_ok;

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
			return m_ok;
		}
		set
		{
			if (m_ok != value)
			{
				m_ok = value;
				OnPropertyChanged("OkCommand");
			}
		}
	}

	public TipsViewModel()
	{
		OkCommand = new ReplayCommand(OkCommandHandler);
	}

	private void OkCommandHandler(object e)
	{
		Window window = e as Window;
		window.Close();
	}
}
