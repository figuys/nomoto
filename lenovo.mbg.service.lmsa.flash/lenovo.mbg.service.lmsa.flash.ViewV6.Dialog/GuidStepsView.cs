using System;
using System.Windows;
using System.Windows.Markup;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.smartdevice;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class GuidStepsView : Window, IUserMsgControl, IComponentConnector
{
	public BaseGuidStepsViewModelV6 VM { get; private set; }

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public GuidStepsView(string modelName, UseCase useCase = UseCase.LMSA_Read_Fastboot, bool autoPlay = false)
	{
		InitializeComponent();
		base.Owner = Application.Current.MainWindow;
		GuidStepsViewV6 guidStepsViewV = new GuidStepsViewV6();
		BaseGuidStepsViewModelV6 viewModel = new GuidStepsViewModelV6(guidStepsViewV, autoPlay).Init(modelName, useCase != UseCase.LMSA_Recovery);
		guidStepsViewV.Init(viewModel);
		guidStepsViewV.CloseAction = delegate
		{
			Close();
		};
		content.Content = guidStepsViewV;
		VM = guidStepsViewV.VM;
		GlobalCmdHelper.Instance.CloseGuidStepDlgEvent = delegate
		{
			base.Dispatcher.Invoke(delegate
			{
				Close();
			});
			GlobalCmdHelper.Instance.CloseGuidStepDlgEvent = null;
		};
	}

	protected override void OnClosed(EventArgs e)
	{
		Result = true;
		VM.Dispose();
		base.OnClosed(e);
		CloseAction?.Invoke(true);
	}

	public Window GetMsgUi()
	{
		return this;
	}
}
