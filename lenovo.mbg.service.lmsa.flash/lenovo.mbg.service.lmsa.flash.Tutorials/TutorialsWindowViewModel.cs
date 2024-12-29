using System;
using System.Windows;
using lenovo.mbg.service.lmsa.flash.Tutorials.Model;
using lenovo.mbg.service.lmsa.flash.Tutorials.RescueTutorials;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.flash.Tutorials;

public class TutorialsWindowViewModel : ViewModelBase
{
	private TutorialDefineModel m_StartModel;

	private TutorialDefineModel currentModel;

	private bool _IsPrevBtnEnable;

	private bool _IsNextBtnEnable = true;

	private bool nextEnable;

	public object Tag { get; private set; }

	public TutorialDefineModel CurrentModel
	{
		get
		{
			return currentModel;
		}
		set
		{
			currentModel = value;
			OnPropertyChanged("CurrentModel");
		}
	}

	public ReplayCommand CloseCommand { get; private set; }

	public ReplayCommand NextCommand { get; private set; }

	public ReplayCommand PreviousCommand { get; private set; }

	public ReplayCommand RadioClickCommand { get; private set; }

	public ReplayCommand GoBackCommand { get; private set; }

	public bool IsPrevBtnEnable
	{
		get
		{
			return _IsPrevBtnEnable;
		}
		set
		{
			_IsPrevBtnEnable = value;
			OnPropertyChanged("IsPrevBtnEnable");
		}
	}

	public bool IsNextBtnEnable
	{
		get
		{
			return _IsNextBtnEnable;
		}
		set
		{
			_IsNextBtnEnable = value;
			OnPropertyChanged("IsNextBtnEnable");
		}
	}

	public bool NextEnable
	{
		get
		{
			return nextEnable;
		}
		set
		{
			nextEnable = value;
			OnPropertyChanged("NextEnable");
		}
	}

	private Action<object> customerNextCommand { get; set; }

	private Action<object> customerPrevCommand { get; set; }

	public TutorialsWindowViewModel()
		: this(null, null)
	{
	}

	public TutorialsWindowViewModel(Action<object> customerNextCommand, Action<object> customerPrevCommand)
	{
		CloseCommand = new ReplayCommand(CloseCommandHandler);
		NextCommand = new ReplayCommand(NextCommandHandler);
		PreviousCommand = new ReplayCommand(PreviousCommandHandler);
		RadioClickCommand = new ReplayCommand(RadioClickCommandHandler);
		GoBackCommand = new ReplayCommand(GoBackCommandHandler);
		this.customerNextCommand = customerNextCommand ?? new Action<object>(NextCommandAction);
		this.customerPrevCommand = customerPrevCommand ?? new Action<object>(PrevCommandAction);
	}

	public TutorialsWindowViewModel Inititalize(TutorialDefineModel currentModel, object tag)
	{
		Tag = tag;
		CurrentModel = currentModel;
		m_StartModel = currentModel;
		if (CurrentModel is StartTutorial)
		{
			IsNextBtnEnable = false;
		}
		else
		{
			IsNextBtnEnable = true;
		}
		return this;
	}

	private void NextCommandAction(object data)
	{
		if (data == null)
		{
			return;
		}
		TutorialsBaseModel tutorialsBaseModel = data as TutorialsBaseModel;
		if (tutorialsBaseModel.NextModel != null)
		{
			tutorialsBaseModel.NextModel.Steps.ForEach(delegate(TutorialsBaseModel n)
			{
				n.IsSelected = false;
			});
			tutorialsBaseModel.NextModel.Steps[0].IsSelected = true;
			CurrentModel = tutorialsBaseModel.NextModel;
			IsPrevBtnEnable = false;
			return;
		}
		int index = CurrentModel.Steps.IndexOf(tutorialsBaseModel);
		TutorialsBaseModel nextStep = CurrentModel.GetNextStep(index);
		if (nextStep != null)
		{
			int num = CurrentModel.Steps.IndexOf(nextStep);
			if (nextStep.IsManual)
			{
				IsNextBtnEnable = false;
			}
			else
			{
				IsNextBtnEnable = num < CurrentModel.Steps.Count - 1;
			}
			if (nextStep.RadioTitleVisibility == Visibility.Collapsed)
			{
				IsPrevBtnEnable = true;
				tutorialsBaseModel.IsSelected = false;
				nextStep.IsSelected = true;
			}
		}
	}

	private void PrevCommandAction(object data)
	{
		TutorialsBaseModel tutorialsBaseModel = data as TutorialsBaseModel;
		int index = CurrentModel.Steps.IndexOf(tutorialsBaseModel);
		TutorialsBaseModel prevStep = CurrentModel.GetPrevStep(index);
		if (prevStep == null)
		{
			if (CurrentModel.PreviousModel != null)
			{
				CurrentModel = CurrentModel.PreviousModel;
			}
			return;
		}
		int num = CurrentModel.Steps.IndexOf(prevStep);
		IsPrevBtnEnable = num != 0 || CurrentModel.PreviousModel != null;
		if (prevStep.IsManual)
		{
			IsNextBtnEnable = false;
		}
		else
		{
			IsNextBtnEnable = true;
		}
		if (prevStep.RadioTitleVisibility == Visibility.Collapsed)
		{
			tutorialsBaseModel.IsSelected = false;
			prevStep.IsSelected = true;
		}
	}

	private void RadioClickCommandHandler(object data)
	{
		if (data != null)
		{
			TutorialsBaseModel tutorialsBaseModel = data as TutorialsBaseModel;
			int num = CurrentModel.Steps.IndexOf(tutorialsBaseModel);
			IsPrevBtnEnable = num != 0 || CurrentModel.PreviousModel != null;
			if (tutorialsBaseModel.IsManual && num == 0)
			{
				IsNextBtnEnable = false;
			}
			else if (!tutorialsBaseModel.IsManual && num == CurrentModel.Steps.Count - 1)
			{
				IsNextBtnEnable = false;
			}
			else
			{
				IsNextBtnEnable = true;
			}
		}
	}

	private void CloseCommandHandler(object data)
	{
		(data as Window)?.Close();
	}

	private void NextCommandHandler(object data)
	{
		customerNextCommand(data);
	}

	private void PreviousCommandHandler(object data)
	{
		customerPrevCommand(data);
	}

	private void GoBackCommandHandler(object data)
	{
		CurrentModel = m_StartModel;
		IsPrevBtnEnable = true;
		IsNextBtnEnable = true;
	}
}
