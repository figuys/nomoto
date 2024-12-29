using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using lenovo.mbg.service.lmsa.ResourcesCleanUp.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ResourcesCleanUp;

public class DeleteResourceFailedTipsViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private string m_deleteFailedResources;

	private ReplayCommand m_okCommand;

	public string DeleteFailedResources
	{
		get
		{
			return m_deleteFailedResources;
		}
		set
		{
			if (!(m_deleteFailedResources == value))
			{
				m_deleteFailedResources = value;
				OnPropertyChanged("DeleteFailedResources");
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

	public DeleteResourceFailedTipsViewModel(List<ResourceAbstract> deleteFailedResources)
	{
		OkCommand = new ReplayCommand(OkCommandHandler);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ResourceAbstract deleteFailedResource in deleteFailedResources)
		{
			stringBuilder.Append(deleteFailedResource.Path).Append(Environment.NewLine).Append(Environment.NewLine);
		}
		DeleteFailedResources = stringBuilder.ToString();
	}

	private void OkCommandHandler(object e)
	{
		Window window = e as Window;
		window.Close();
	}
}
