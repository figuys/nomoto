using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.smartdevice;
using lenovo.mbg.service.framework.smartdevice.Steps;
using lenovo.mbg.service.lmsa.flash.ViewV6;
using lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;
using lenovo.themes.generic;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class RecipeMessageImpl : IRecipeMessage
{
	protected MessageViewHelper Mh { get; }

	public RecipeMessageImpl(MessageViewHelper mh)
	{
		Mh = mh;
	}

	public Task<bool?> Show(string title, string message, string ok = "K0327", string cancel = null, bool showClose = false, MessageBoxImage icon = MessageBoxImage.Asterisk, string notifyText = null, bool isPrivacy = false)
	{
		return Mh.Show(title, message, ok, cancel, showClose, icon, notifyText, isPrivacy);
	}

	public Task<bool?> ShowCustom(IMessageViewV6 view)
	{
		return Mh.ShowCustom(view);
	}

	public Task<bool?> WaitByAdb(string title, string message, string image)
	{
		return Mh.ShowRightPic(title, message, Strring2ImageSource(image), null, null, "K0104");
	}

	public Task<bool?> WaitByFastboot(UseCase _usecase, string modelname)
	{
		GuidStepsViewV6 lv = null;
		Application.Current.Dispatcher.Invoke(delegate
		{
			lv = new GuidStepsViewV6();
			BaseGuidStepsViewModelV6 viewModel = new GuidStepsViewModelV6(lv, autoPlay: false, popupWhenClose: true).Init(modelname, _usecase != UseCase.LMSA_Recovery);
			lv.Init(viewModel);
			lv.VM.CheckingTxtVisibility = Visibility.Visible;
			lv.CloseAction = delegate(bool? r)
			{
				Close(r);
			};
		});
		return Mh.ShowCustom(lv);
	}

	public Task<bool?> Linker(string title, string message, object data)
	{
		List<Tuple<string, string>> LinkArr = new List<Tuple<string, string>>();
		if (data is JArray)
		{
			foreach (JToken item in data as JArray)
			{
				LinkArr.Add(new Tuple<string, string>(item.Value<string>("text"), item.Value<string>("url")));
			}
		}
		MessageWithLinkerView lv = null;
		Application.Current.Dispatcher.Invoke(delegate
		{
			lv = new MessageWithLinkerView();
			lv.Init(title, message, "K0397", "K0208", LinkArr);
			lv.CloseAction = delegate(bool? r)
			{
				Close(r);
			};
		});
		return Mh.ShowCustom(lv);
	}

	public Task<bool?> RightPic(string title, string message, string image, string ok = "K0327", string cancel = null, string tips = null, bool showClose = false, bool popupWhenClose = false)
	{
		return Mh.ShowRightPic(title, message, Strring2ImageSource(image), ok, cancel, tips, showClose, popupWhenClose);
	}

	public bool? AutoClose(string title, string message, string image, List<string> buttons = null, int milliseconds = -1, string link = null, bool showClose = false, bool popupWhenClose = false, bool format = true, bool? autoCloseResult = true)
	{
		string ok = null;
		string cancel = null;
		if (buttons != null && buttons.Count == 1)
		{
			ok = buttons[0];
		}
		else if (buttons != null && buttons.Count == 2)
		{
			ok = buttons[0];
			cancel = buttons[1];
		}
		RescueMessageView lv = null;
		Application.Current.Dispatcher.Invoke(delegate
		{
			lv = new RescueMessageView();
			lv.Init(title, message, Strring2ImageSource(image), ok, cancel, link, showClose, popupWhenClose, format);
			lv.CloseAction = delegate(bool? r)
			{
				Close(r);
			};
		});
		Task<bool?> task = Mh.ShowCustom(lv);
		if (milliseconds > 0 && !task.Wait(milliseconds))
		{
			Close(autoCloseResult);
		}
		return task.Result;
	}

	public bool? AutoCloseMoreStep(string title, ConnectStepInfo stepArr, int milliseconds = -1, bool popupWhenClose = false)
	{
		string[] ratioArr = stepArr.WidthRatio.Split(':');
		if (stepArr.Steps.Count == 0)
		{
			return null;
		}
		Rescue3ColumnView lv = null;
		Application.Current.Dispatcher.Invoke(delegate
		{
			ColumnViewModel columnViewModel = new ColumnViewModel
			{
				IsClosePopup = popupWhenClose,
				StepTitle = title,
				StepNoteText = stepArr.NoteText
			};
			if (stepArr.Steps.Count == 1)
			{
				columnViewModel.StepContext1 = stepArr.Steps[0].StepContent;
				columnViewModel.WidthRatio1 = new GridLength(1.0, GridUnitType.Star);
				columnViewModel.WidthRatio2 = new GridLength(0.0, GridUnitType.Star);
				columnViewModel.WidthRatio3 = new GridLength(0.0, GridUnitType.Star);
				columnViewModel.StepImage1 = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Resources/connect_new/" + stepArr.Steps[0].StepImage;
			}
			else if (stepArr.Steps.Count == 2)
			{
				columnViewModel.StepContext1 = stepArr.Steps[0].StepContent;
				columnViewModel.StepContext2 = stepArr.Steps[1].StepContent;
				columnViewModel.WidthRatio1 = new GridLength(Convert.ToDouble(ratioArr[0]), GridUnitType.Star);
				columnViewModel.WidthRatio2 = new GridLength(Convert.ToDouble(ratioArr[1]), GridUnitType.Star);
				columnViewModel.WidthRatio3 = new GridLength(0.0, GridUnitType.Star);
				columnViewModel.StepImage1 = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Resources/connect_new/" + stepArr.Steps[0].StepImage;
				columnViewModel.StepImage2 = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Resources/connect_new/" + stepArr.Steps[1].StepImage;
			}
			else
			{
				columnViewModel.StepContext1 = stepArr.Steps[0].StepContent;
				columnViewModel.StepContext2 = stepArr.Steps[1].StepContent;
				columnViewModel.StepContext3 = stepArr.Steps[2].StepContent;
				columnViewModel.WidthRatio1 = new GridLength(Convert.ToDouble(ratioArr[0]), GridUnitType.Star);
				columnViewModel.WidthRatio2 = new GridLength(Convert.ToDouble(ratioArr[1]), GridUnitType.Star);
				columnViewModel.WidthRatio3 = new GridLength(Convert.ToDouble(ratioArr[2]), GridUnitType.Star);
				columnViewModel.StepImage1 = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Resources/connect_new/" + stepArr.Steps[0].StepImage;
				columnViewModel.StepImage2 = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Resources/connect_new/" + stepArr.Steps[1].StepImage;
				columnViewModel.StepImage3 = "pack://application:,,,/lenovo.mbg.service.lmsa.flash;component/Resources/connect_new/" + stepArr.Steps[2].StepImage;
			}
			lv = new Rescue3ColumnView
			{
				DataContext = columnViewModel,
				CloseAction = delegate(bool? r)
				{
					Close(r);
				}
			};
		});
		Task<bool?> task = Mh.ShowCustom(lv);
		if (milliseconds > 0 && !task.Wait(milliseconds))
		{
			Close(true);
		}
		return task.Result;
	}

	public bool? AutoCloseConnectTutorials(string title, JArray steps, int milliseconds = -1, bool autoPlay = false, double interval = 5000.0, bool showPlayControl = true, bool showClose = true, bool popupWhenClose = true)
	{
		GuidStepsViewV6 lv = null;
		Application.Current.Dispatcher.Invoke(delegate
		{
			lv = new GuidStepsViewV6();
			BaseGuidStepsViewModelV6 viewModel = new BaseGuidStepsViewModelV6(lv, autoPlay, interval, showPlayControl, showClose, popupWhenClose).InitSteps(title, steps).Ready();
			lv.Init(viewModel);
			lv.CloseAction = delegate(bool? r)
			{
				Close(r);
			};
		});
		Task<bool?> task = Mh.ShowCustom(lv);
		if (milliseconds > 0 && !task.Wait(milliseconds))
		{
			Close(true);
		}
		return task.Result;
	}

	public Task<bool?> TabletTurnoff(string title, string message, string image, string note)
	{
		TabletTurnOffView lv = null;
		Application.Current.Dispatcher.Invoke(delegate
		{
			lv = new TabletTurnOffView();
			lv.Init(title, message, image, note);
			lv.CloseAction = delegate(bool? r)
			{
				Close(r);
			};
		});
		return Mh.ShowCustom(lv);
	}

	public Task<bool?> BackConfirm(string title, string message, string ok = "K0327", string cancel = null, bool showClose = false, bool isNotifyText = false)
	{
		BackupConfirmView lv = null;
		Application.Current.Dispatcher.Invoke(delegate
		{
			BackupConfirmView obj = new BackupConfirmView
			{
				CloseAction = delegate(bool? r)
				{
					Close(r);
				}
			};
			BackupConfirmView result = obj;
			lv = obj;
			return result;
		});
		return Mh.ShowCustom(lv);
	}

	public Task<bool?> EraseData()
	{
		EraseDataView lv = null;
		Application.Current.Dispatcher.Invoke(delegate
		{
			EraseDataView obj = new EraseDataView
			{
				CloseAction = delegate(bool? r)
				{
					Close(r);
				}
			};
			EraseDataView result = obj;
			lv = obj;
			return result;
		});
		return Mh.ShowCustom(lv);
	}

	public void ShowDownloadCenter(bool show)
	{
		MainFrameV6.Instance.IMsgManager.ShowDownloadCenter(show);
	}

	public void Close(bool? result, Action<bool?> closeCallback = null)
	{
		Mh.Close(result, closeCallback);
	}

	private ImageSource Strring2ImageSource(string encodeImage)
	{
		ImageSource result = null;
		if (!string.IsNullOrEmpty(encodeImage))
		{
			string image = GlobalFun.DecodeBase64(encodeImage);
			Application.Current.Dispatcher.Invoke(delegate
			{
				try
				{
					result = XamlReader.Parse(image) as ImageSource;
				}
				catch (Exception)
				{
					result = Application.Current.Resources[encodeImage] as ImageSource;
				}
			});
		}
		return result;
	}

	public void SetMainWindowDriverBtnStatus(string _code)
	{
		MainFrameV6.Instance.IMsgManager.SetDriverButtonStatus(_code);
	}
}
