using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.ResourcesCleanUp.Model;
using lenovo.themes.generic.Controls.Windows;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ResourcesCleanUp.ViewModel;

public class DeleteResourceViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private List<ResourceAbstract> m_resources;

	private double m_progressBarMaxValue;

	private double m_progressBarValue = 0.0;

	private string m_percentage = "0%";

	private ReplayCommand m_operationCommand;

	private string m_operationButtonText = "K0583";

	private string m_operationType;

	private string m_title;

	private volatile bool isStop = false;

	public double ProgressBarMaxValue
	{
		get
		{
			return m_progressBarMaxValue;
		}
		set
		{
			if (m_progressBarMaxValue != value)
			{
				m_progressBarMaxValue = value;
				OnPropertyChanged("ProgressBarMaxValue");
			}
		}
	}

	public double ProgressBarValue
	{
		get
		{
			return m_progressBarValue;
		}
		set
		{
			if (m_progressBarValue != value)
			{
				m_progressBarValue = value;
				Percentage = ConvertPercentage(ProgressBarMaxValue, value);
				OnPropertyChanged("ProgressBarValue");
			}
		}
	}

	public string Percentage
	{
		get
		{
			return m_percentage;
		}
		set
		{
			if (!(m_percentage == value))
			{
				m_percentage = value;
				OnPropertyChanged("Percentage");
			}
		}
	}

	public ReplayCommand OperationCommand
	{
		get
		{
			return m_operationCommand;
		}
		set
		{
			if (m_operationCommand != value)
			{
				m_operationCommand = value;
				OnPropertyChanged("OperationCommand");
			}
		}
	}

	public string OperationButtonText
	{
		get
		{
			return m_operationButtonText;
		}
		set
		{
			if (!(m_operationButtonText == value))
			{
				m_operationButtonText = value;
				OnPropertyChanged("OperationButtonText");
			}
		}
	}

	private string OperationType
	{
		get
		{
			return m_operationType;
		}
		set
		{
			if (!(m_operationType == value))
			{
				OperationButtonText = value;
				m_operationType = value;
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

	public DeleteResourceViewModel(List<ResourceAbstract> resources)
	{
		OperationCommand = new ReplayCommand(OperationCommandHandler);
		m_resources = resources;
		ProgressBarValue = 0.0;
		ProgressBarMaxValue = m_resources.Sum((ResourceAbstract m) => m.CountFlag);
		OperationType = "K0583";
		Title = "K0696";
	}

	private string ConvertPercentage(double maxiValue, double value)
	{
		if (maxiValue == 0.0)
		{
			return "0%";
		}
		return $"{Math.Round(value / maxiValue * 100.0, 0).ToString()}%";
	}

	private void OperationCommandHandler(object e)
	{
		switch (OperationType)
		{
		case "K0583":
		{
			OperationType = "K0694";
			isStop = false;
			Title = "K0693";
			Window window2 = e as Window;
			window2.Closed += delegate
			{
				isStop = true;
			};
			Delete();
			break;
		}
		case "K0694":
			OperationType = "K0386";
			Title = "K0682";
			isStop = true;
			break;
		case "K0386":
		{
			Window window = e as Window;
			window.Close();
			break;
		}
		}
	}

	private void Delete()
	{
		Task.Factory.StartNew(delegate
		{
			List<ResourceAbstract> list = new List<ResourceAbstract>();
			try
			{
				string text = m_resources.Count.ToString();
				int num = 0;
				LogHelper.LogInstance.Debug($"Starting delete resources[{text}]");
				foreach (ResourceAbstract resource in m_resources)
				{
					if (isStop)
					{
						break;
					}
					try
					{
						resource.Delete();
						LogHelper.LogInstance.Debug($"Delete success[{num++.ToString()}/{text}]:{resource.Path}");
					}
					catch (Exception ex)
					{
						list.Add(resource);
						LogHelper.LogInstance.Error($"Delete failed[{num++.ToString()}/{text}]:{resource.Path}[exception:{ex.ToString()}]");
					}
					ProgressBarValue += resource.CountFlag;
				}
				LogHelper.LogInstance.Debug($"Finished delete resources[failed count:{list.Count.ToString()},total:{text}]");
			}
			finally
			{
				AppContext.Single.CurrentDispatcher.Invoke(delegate
				{
					OperationType = "K0386";
					Title = "K0682";
				});
			}
			List<ResourceAbstract> failedFiles = list.Where((ResourceAbstract m) => string.Compare("files", m.RootId) == 0).ToList();
			foreach (ResourceAbstract item in failedFiles.ToList())
			{
				if (!item.Exists)
				{
					failedFiles.RemoveAll((ResourceAbstract m) => m.Path.Equals(item));
				}
			}
			if (failedFiles.Count > 0)
			{
				AppContext.Single.CurrentDispatcher.Invoke(delegate
				{
					LenovoWindow win = new LenovoWindow();
					win.SizeToContent = SizeToContent.WidthAndHeight;
					DeleteResourceFailedTipsViewModel dataContext = new DeleteResourceFailedTipsViewModel(failedFiles);
					win.Content = new DeleteResourceFailedTipsView
					{
						DataContext = dataContext
					};
					HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
					{
						win.ShowDialog();
					});
				});
			}
		});
	}
}
