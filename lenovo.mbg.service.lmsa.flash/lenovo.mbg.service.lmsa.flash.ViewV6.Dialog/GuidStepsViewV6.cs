using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.themes.generic;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class GuidStepsViewV6 : UserControl, IMessageViewV6, IComponentConnector
{
	public UserControl View => this;

	public Action<bool?> CloseAction { get; set; }

	public AutoResetEvent WaitHandler { get; }

	public bool? Result { get; set; }

	public BaseGuidStepsViewModelV6 VM { get; private set; }

	public GuidStepsViewV6()
	{
		InitializeComponent();
		WaitHandler = new AutoResetEvent(initialState: false);
	}

	public void Init(BaseGuidStepsViewModelV6 viewModel)
	{
		VM = viewModel;
		base.DataContext = VM;
	}

	public void Close()
	{
		Result = null;
		WaitHandler.Set();
		CloseAction?.Invoke(null);
	}
}
