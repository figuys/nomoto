using System.Collections.Generic;
using System.Linq;
using System.Windows;
using lenovo.mbg.service.lmsa.ResourcesCleanUp.Model;
using lenovo.mbg.service.lmsa.ResourcesCleanUp.View;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ResourcesCleanUp.ViewModel;

public class ClearupMainViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private object m_content;

	private ReplayCommand m_loaded;

	public object Content
	{
		get
		{
			return m_content;
		}
		set
		{
			if (m_content != value)
			{
				m_content = value;
				OnPropertyChanged("Content");
			}
		}
	}

	public ReplayCommand LoadedCommand
	{
		get
		{
			return m_loaded;
		}
		set
		{
			if (m_loaded != value)
			{
				m_loaded = value;
				OnPropertyChanged("LoadedCommand");
			}
		}
	}

	public ClearupMainViewModel()
	{
		LoadedCommand = new ReplayCommand(LoadedCommandHandler);
	}

	private void LoadedCommandHandler(object e)
	{
		ScanningViewModel scanningViewModel = new ScanningViewModel();
		Content = new ScanningView
		{
			DataContext = scanningViewModel
		};
		scanningViewModel.StartingScan(delegate(List<ResourceAbstract> rsources)
		{
			if (rsources.Sum((ResourceAbstract m) => m.CountFlag) == 0)
			{
				AppContext.Single.CurrentDispatcher.Invoke(delegate
				{
					Content = new TipsView
					{
						DataContext = new TipsViewModel
						{
							Tips = "K0695"
						}
					};
				});
			}
			else
			{
				AppContext.Single.CurrentDispatcher.Invoke(delegate
				{
					Content = new DeleteResourceView
					{
						VerticalAlignment = VerticalAlignment.Center,
						DataContext = new DeleteResourceViewModel(rsources)
					};
				});
			}
		});
	}
}
