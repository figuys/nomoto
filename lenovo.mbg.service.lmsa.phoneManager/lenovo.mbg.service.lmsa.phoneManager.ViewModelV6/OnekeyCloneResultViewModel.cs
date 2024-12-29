using System.Threading.Tasks;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class OnekeyCloneResultViewModel : ViewModelBase
{
	public OnekeyCloneTransferViewModel _TransferViewModel;

	public ReplayCommand OKCommand { get; }

	public ReplayCommand RetryCommand { get; }

	public OnekeyCloneTransferViewModel TransferViewModel
	{
		get
		{
			return _TransferViewModel;
		}
		set
		{
			_TransferViewModel = value;
			OnPropertyChanged("TransferViewModel");
		}
	}

	public OnekeyCloneResultViewModel()
	{
		OKCommand = new ReplayCommand(OKCommandHandler);
		RetryCommand = new ReplayCommand(RetryCommandHandler);
	}

	public override void LoadData(object data)
	{
		TransferViewModel = data as OnekeyCloneTransferViewModel;
		base.LoadData(data);
	}

	private void OKCommandHandler(object data)
	{
		Context.Switch(ViewType.ONEKEYCLONE, null, reload: false, reloadData: true);
	}

	private void RetryCommandHandler(object data)
	{
		if (TransferViewModel.SelectedMasterDevice == null || TransferViewModel.SelectedTargetDevice == null)
		{
			Context.Switch(ViewType.ONEKEYCLONE, null, reload: false, reloadData: true);
			return;
		}
		Context.Switch(ViewType.ONEKEYCLONE_TRANSFER);
		Task.Run(delegate
		{
			TransferViewModel.RetryClone();
		});
	}
}
