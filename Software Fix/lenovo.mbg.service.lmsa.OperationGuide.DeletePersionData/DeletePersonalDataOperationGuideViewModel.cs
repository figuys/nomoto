using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.OperationGuide.DeletePersionData;

public class DeletePersonalDataOperationGuideViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private bool m_isShowDeletePersonalDataGuide;

	private string m_tipContent;

	private string bOkContent;

	private ReplayCommand m_iKnowCommand;

	public bool IsShowDeletePersonalDataGuidePopup { get; set; }

	public bool IsShowDeletePersonalDataGuide
	{
		get
		{
			return m_isShowDeletePersonalDataGuide;
		}
		set
		{
			if (m_isShowDeletePersonalDataGuide != value)
			{
				m_isShowDeletePersonalDataGuide = value;
				OnPropertyChanged("IsShowDeletePersonalDataGuide");
			}
		}
	}

	public string TipContent
	{
		get
		{
			return m_tipContent;
		}
		set
		{
			if (!(m_tipContent == value))
			{
				m_tipContent = value;
				OnPropertyChanged("TipContent");
			}
		}
	}

	public string OkContent
	{
		get
		{
			return bOkContent;
		}
		set
		{
			if (!(bOkContent == value))
			{
				bOkContent = value;
				OnPropertyChanged("OkContent");
			}
		}
	}

	public ReplayCommand IKnowCommand
	{
		get
		{
			return m_iKnowCommand;
		}
		set
		{
			if (m_iKnowCommand != value)
			{
				m_iKnowCommand = value;
				OnPropertyChanged("IKnowCommand");
			}
		}
	}

	public DeletePersonalDataOperationGuideViewModel()
	{
		IKnowCommand = new ReplayCommand(IKnowCommandHandler);
		TipContent = "K0704";
		OkContent = "K0698";
	}

	private void IKnowCommandHandler(object e)
	{
		IsShowDeletePersonalDataGuide = false;
		IsShowDeletePersonalDataGuidePopup = false;
	}
}
