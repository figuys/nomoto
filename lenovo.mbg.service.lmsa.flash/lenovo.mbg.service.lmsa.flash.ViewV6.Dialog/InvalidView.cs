using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;

public partial class InvalidView : Window, IUserMsgControl, IComponentConnector
{
	private int _Model;

	private DevCategory _Category;

	private InvalidViewModel _VM;

	public bool IsManualModel { get; set; }

	public bool? Result { get; set; }

	public Action<bool?> CloseAction { get; set; }

	public Action<bool?> CallBackAction { get; set; }

	public InvalidView(DevCategory category, int model, string modelname = null)
	{
		InitializeComponent();
		_Model = model;
		_Category = category;
		_VM = new InvalidViewModel();
		base.Owner = Application.Current.MainWindow;
		if (category == DevCategory.Smart)
		{
			btn2.Visibility = Visibility.Collapsed;
			RightPanel.Visibility = Visibility.Collapsed;
			LeftMark.LangKey = "K0998";
			_VM.LeftBtnText = "K1010";
			_VM.LeftLabelText = "K1332";
			_VM.LeftBtnImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/tInvalid2.png";
		}
		else if (category == DevCategory.Tablet && model == 1)
		{
			RightMark.Text = "";
			LeftMark.LangKey = "K0998";
			_VM.LeftBtnText = "K1010";
			_VM.RightBtnText = "K1009";
			_VM.LeftLabelText = "K1000";
			_VM.RightLabelText = "K0999";
			_VM.LeftBtnImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/tInvalid2.png";
			_VM.RightBtnImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/tInvalid1.png";
		}
		else if (category == DevCategory.Tablet && model == 2)
		{
			RightMark.LangKey = "K0998";
			LeftMark.Text = "";
			_VM.LeftBtnText = "K1010";
			_VM.RightBtnText = "K1134";
			_VM.LeftLabelText = "K1000";
			_VM.RightLabelText = "K1332";
			_VM.LeftBtnImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/tInvalid2.png";
			_VM.RightBtnImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/inputImei.png";
		}
		else if (category == DevCategory.Phone && model == 1)
		{
			RightMark.Text = "";
			LeftMark.LangKey = "K0998";
			_VM.LeftBtnText = "K1009";
			_VM.RightBtnText = "K1010";
			_VM.LeftLabelText = "K0999";
			_VM.RightLabelText = "K1000";
			_VM.LeftBtnImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/usbConn.png";
			_VM.RightBtnImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/phsearch.png";
		}
		else if (category == DevCategory.Phone && model == 2)
		{
			RightMark.LangKey = "K0998";
			LeftMark.Text = "";
			_VM.LeftBtnText = "K1010";
			_VM.RightBtnText = "K1134";
			_VM.LeftLabelText = "K1000";
			_VM.RightLabelText = "K1332";
			_VM.LeftBtnImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/phsearch.png";
			_VM.RightBtnImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/inputImei.png";
		}
		else if (category == DevCategory.Phone && model == 3)
		{
			btn2.Visibility = Visibility.Collapsed;
			RightPanel.Visibility = Visibility.Collapsed;
			LeftMark.Text = "";
			_VM.LeftBtnText = "K1010";
			_VM.LeftLabelText = "K1000";
			_VM.LeftBtnImage = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/ResourceV6/phsearch.png";
		}
		if (!string.IsNullOrEmpty(modelname))
		{
			ResponseModel<object> responseModel = FlashContext.SingleInstance.service.RequestBase(WebApiUrl.GET_SUPPORT_FASTBOOT_BY_MODELNAME, new Dictionary<string, string> { { "modelName", modelname } });
			if (responseModel != null && responseModel.content != null && !bool.Parse(responseModel.content.ToString()))
			{
				lblInfo.LangKey = string.Format(HostProxy.LanguageService.Translate("K1765"), modelname);
				RightMark.Text = "";
				LeftMark.LangKey = "K0998";
			}
		}
		base.DataContext = _VM;
	}

	private void OnWndClose(object sender, RoutedEventArgs e)
	{
		Result = false;
		Close();
		CloseAction?.Invoke(false);
	}

	public Window GetMsgUi()
	{
		return this;
	}

	private void OnRbtnChecked(object sender, RoutedEventArgs e)
	{
		RadioButton radioButton = e.OriginalSource as RadioButton;
		if (_Category == DevCategory.Phone)
		{
			if (_Model == 1)
			{
				IsManualModel = radioButton.Name == "btn2";
			}
			else
			{
				IsManualModel = radioButton.Name == "btn1";
			}
		}
		else if (_Category == DevCategory.Tablet)
		{
			IsManualModel = radioButton.Name == "btn1";
		}
		else
		{
			IsManualModel = true;
		}
		Result = true;
		Close();
		CloseAction?.Invoke(true);
	}
}
