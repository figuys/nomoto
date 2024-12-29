using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.Form.FormVerify;
using lenovo.mbg.service.lmsa.common.Form.ViewModel;
using lenovo.mbg.service.lmsa.Feedback.Business;
using lenovo.mbg.service.lmsa.Feedback.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.Login.Business.FormVerify;
using lenovo.mbg.service.lmsa.ModelV6;
using lenovo.mbg.service.lmsa.Properties;
using lenovo.mbg.service.lmsa.UserControls.MessageBoxWindow;
using lenovo.mbg.service.lmsa.ViewModels;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Feedback.ViewModel;

public class FeedbackMainViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	private List<FormItemViewModel> mFormItems = null;

	public QTreeNode _SelFunction;

	private bool deviceIsConnected;

	private FormItemViewModel mEmailAddress;

	private FormItemViewModel mUserName;

	private FormItemViewModel mComments;

	private string marketName;

	private string modelName;

	private bool submitButtonIsEnabled = true;

	private ReplayCommand mSubmitCommand;

	private bool isOnline;

	private string loginUserName;

	private string deviceSN;

	private Visibility deviceSNPanelVisibility;

	private bool deviceInfoIsChecked;

	private string deviceImeiLabel;

	private string deviceImei1;

	private string deviceImei2;

	private Visibility DeviceIemiPanelVisibility;

	private Visibility feedbackItemPanelVisibility = Visibility.Collapsed;

	private Visibility _IsModuleWarnVisible;

	private bool logFileIsChecked = false;

	private string tips;

	private ReplayCommand closeWinCommand;

	public static string QFeedbackRespStr { get; set; }

	public FeedBackNodeModel Feedback { get; private set; }

	public ObservableCollection<QTreeNode> FunctionArr { get; set; }

	public QTreeNode SelFunction
	{
		get
		{
			return _SelFunction;
		}
		set
		{
			_SelFunction = value;
			OnPropertyChanged("SelFunction");
		}
	}

	public bool DeviceIsConnected
	{
		get
		{
			return deviceIsConnected;
		}
		set
		{
			if (deviceIsConnected != value)
			{
				deviceIsConnected = value;
				OnPropertyChanged("DeviceIsConnected");
			}
		}
	}

	public FormItemViewModel EmailAddress
	{
		get
		{
			return mEmailAddress;
		}
		set
		{
			if (mEmailAddress != value)
			{
				mEmailAddress = value;
				OnPropertyChanged("EmailAddress");
			}
		}
	}

	public FormItemViewModel UserName
	{
		get
		{
			return mUserName;
		}
		set
		{
			if (mUserName != value)
			{
				mUserName = value;
				OnPropertyChanged("UserName");
			}
		}
	}

	public FormItemViewModel Comments
	{
		get
		{
			return mComments;
		}
		set
		{
			if (mComments != value)
			{
				mComments = value;
				OnPropertyChanged("Comments");
			}
		}
	}

	public string MarketName
	{
		get
		{
			return marketName;
		}
		set
		{
			if (!(marketName == value))
			{
				marketName = value;
				OnPropertyChanged("MarketName");
			}
		}
	}

	public string ModelName
	{
		get
		{
			return modelName;
		}
		set
		{
			if (!(modelName == value))
			{
				modelName = value;
				OnPropertyChanged("ModelName");
			}
		}
	}

	public bool SubmitButtonIsEnabled
	{
		get
		{
			return submitButtonIsEnabled;
		}
		set
		{
			if (submitButtonIsEnabled != value)
			{
				submitButtonIsEnabled = value;
				OnPropertyChanged("SubmitButtonIsEnabled");
			}
		}
	}

	public ReplayCommand SubmitCommand
	{
		get
		{
			return mSubmitCommand;
		}
		set
		{
			if (mSubmitCommand != value)
			{
				mSubmitCommand = value;
				OnPropertyChanged("SubmitCommand");
			}
		}
	}

	public ReplayCommand FunSelCommand { get; set; }

	public bool IsOnline
	{
		get
		{
			return isOnline;
		}
		set
		{
			if (isOnline != value)
			{
				isOnline = value;
				EmailAddressVerify emailAddressVerify = (EmailAddressVerify)EmailAddress.FormVerify;
				emailAddressVerify.IsCanEmpty = isOnline;
				OnPropertyChanged("IsOnline");
			}
		}
	}

	public string LoginUserName
	{
		get
		{
			return loginUserName;
		}
		set
		{
			if (!(loginUserName == value))
			{
				loginUserName = value;
				OnPropertyChanged("LoginUserName");
			}
		}
	}

	public string DeviceSN
	{
		get
		{
			return deviceSN;
		}
		set
		{
			if (!(deviceSN == value))
			{
				deviceSN = value;
				OnPropertyChanged("DeviceSN");
			}
		}
	}

	public Visibility DeviceSNPanelVisibility
	{
		get
		{
			return deviceSNPanelVisibility;
		}
		set
		{
			if (deviceSNPanelVisibility != value)
			{
				deviceSNPanelVisibility = value;
				OnPropertyChanged("DeviceSNPanelVisibility");
			}
		}
	}

	public bool DeviceInfoIsChecked
	{
		get
		{
			return deviceInfoIsChecked;
		}
		set
		{
			if (deviceInfoIsChecked != value)
			{
				deviceInfoIsChecked = value;
				OnPropertyChanged("DeviceInfoIsChecked");
			}
		}
	}

	public string DeviceImeiLabel
	{
		get
		{
			return deviceImeiLabel;
		}
		set
		{
			if (!(deviceImeiLabel == value))
			{
				deviceImeiLabel = value;
				OnPropertyChanged("DeviceImeiLabel");
			}
		}
	}

	public string DeviceImei1
	{
		get
		{
			return deviceImei1;
		}
		set
		{
			if (!(deviceImei1 == value))
			{
				deviceImei1 = value;
				OnPropertyChanged("DeviceImei1");
			}
		}
	}

	public string DeviceImei2
	{
		get
		{
			return deviceImei2;
		}
		set
		{
			if (!(deviceImei2 == value))
			{
				deviceImei2 = value;
				OnPropertyChanged("DeviceImei2");
			}
		}
	}

	public Visibility MyProperty
	{
		get
		{
			return DeviceIemiPanelVisibility;
		}
		set
		{
			if (DeviceIemiPanelVisibility != value)
			{
				DeviceIemiPanelVisibility = value;
				OnPropertyChanged("MyProperty");
			}
		}
	}

	public Visibility FeedbackItemPanelVisibility
	{
		get
		{
			return feedbackItemPanelVisibility;
		}
		set
		{
			if (feedbackItemPanelVisibility != value)
			{
				feedbackItemPanelVisibility = value;
				OnPropertyChanged("FeedbackItemPanelVisibility");
			}
		}
	}

	public Visibility IsModuleWarnVisible
	{
		get
		{
			return _IsModuleWarnVisible;
		}
		set
		{
			if (_IsModuleWarnVisible != value)
			{
				_IsModuleWarnVisible = value;
				OnPropertyChanged("IsModuleWarnVisible");
			}
		}
	}

	public bool LogFileIsChekced
	{
		get
		{
			return logFileIsChecked;
		}
		set
		{
			if (logFileIsChecked != value)
			{
				logFileIsChecked = value;
				OnPropertyChanged("LogFileIsChekced");
			}
		}
	}

	public string Tips
	{
		get
		{
			return tips;
		}
		set
		{
			if (!(tips == value))
			{
				tips = value;
				OnPropertyChanged("Tips");
			}
		}
	}

	public ReplayCommand CloseWinCommand
	{
		get
		{
			return closeWinCommand;
		}
		set
		{
			if (closeWinCommand != value)
			{
				closeWinCommand = value;
				OnPropertyChanged("CloseWinCommand");
			}
		}
	}

	public FeedbackMainViewModel(FeedBackNodeModel feedBack)
	{
		Feedback = feedBack;
		FunctionArr = new ObservableCollection<QTreeNode>();
		if (string.IsNullOrEmpty(QFeedbackRespStr))
		{
			string @string = Resources.ResourceManager.GetString("QFeedbackStr");
			List<QTreeNode> arr = JsonConvert.DeserializeObject<List<QTreeNode>>(@string);
			LoadTreeNode(arr);
		}
		else
		{
			LoadTreeNode(JsonHelper.DeserializeJson2Object<ResponseModel<List<QTreeNode>>>(QFeedbackRespStr)?.content);
		}
		IsModuleWarnVisible = Visibility.Collapsed;
		FunSelCommand = new ReplayCommand(delegate
		{
			IsModuleWarnVisible = Visibility.Collapsed;
		});
		SubmitCommand = new ReplayCommand(SubmitCommandHandler);
		CloseWinCommand = new ReplayCommand(CloseWinCommandHandler);
		mFormItems = new List<FormItemViewModel>();
		EmailAddress = new FormItemViewModel(new EmailAddressVerify());
		UserName = new FormItemViewModel(new UserNameVerify());
		Comments = new FormItemViewModel(new CanNotEmptyVerify());
		UserService.Single.OnlineUserChanged += Single_OnlineUserChanged;
		HostProxy.deviceManager.MasterDeviceChanged += DeviceManager_MasterDeviceChanged;
	}

	private void LoadTreeNode(List<QTreeNode> arr)
	{
		if (arr == null || arr.Count == 0)
		{
			return;
		}
		SetArrItemmCommand(arr, new ReplayCommand(OnLinkClicked));
		if (UserService.Single.IsOnline)
		{
			QTreeNode qTreeNode = arr.FirstOrDefault((QTreeNode p) => p.Context == "Log in");
			if (qTreeNode != null)
			{
				qTreeNode.Visible = Visibility.Collapsed;
			}
			qTreeNode = arr.FirstOrDefault((QTreeNode p) => p.Context == "Rescue");
			if (qTreeNode != null)
			{
				qTreeNode.Visible = Visibility.Visible;
			}
		}
		else
		{
			QTreeNode qTreeNode2 = arr.FirstOrDefault((QTreeNode p) => p.Context == "Log in");
			if (qTreeNode2 != null)
			{
				qTreeNode2.Visible = Visibility.Visible;
			}
			qTreeNode2 = arr.FirstOrDefault((QTreeNode p) => p.Context == "Rescue");
			if (qTreeNode2 != null)
			{
				qTreeNode2.Visible = Visibility.Collapsed;
			}
		}
		QTreeNode qTreeNode3 = arr.FirstOrDefault((QTreeNode p) => p.Context == "Other");
		if (qTreeNode3 != null)
		{
			qTreeNode3.Question = string.Empty;
		}
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			FunctionArr.Clear();
			arr.ForEach(delegate(QTreeNode p)
			{
				FunctionArr.Add(p);
			});
		});
	}

	private void DeviceManager_MasterDeviceChanged(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		if (e == DeviceSoftStateEx.Online)
		{
			DeviceIsConnected = true;
			TcpAndroidDevice tcpAndroidDevice = sender as TcpAndroidDevice;
			DeviceSN = tcpAndroidDevice.Property.SN;
			DeviceSNPanelVisibility = (string.IsNullOrEmpty(DeviceSN) ? Visibility.Collapsed : Visibility.Visible);
			DeviceImei1 = tcpAndroidDevice.Property.IMEI1;
			DeviceImei2 = tcpAndroidDevice.Property.IMEI2;
			ModelName = tcpAndroidDevice.Property.ModelName;
			DeviceImeiLabel = (string.IsNullOrEmpty(tcpAndroidDevice.Property.IMEI2) ? "IMEI:" : "IMEI 1:");
			DeviceIemiPanelVisibility = ((string.IsNullOrEmpty(DeviceImei1) && string.IsNullOrEmpty(DeviceImei2)) ? Visibility.Collapsed : Visibility.Visible);
		}
		else
		{
			DeviceIsConnected = false;
			DeviceSN = string.Empty;
			DeviceImei1 = string.Empty;
			DeviceImei2 = string.Empty;
			ModelName = string.Empty;
			DeviceIemiPanelVisibility = Visibility.Collapsed;
			DeviceSNPanelVisibility = Visibility.Collapsed;
		}
	}

	private void Single_OnlineUserChanged(object sender, OnlineUserChangedEventArgs e)
	{
		IsOnline = e.IsOnline;
		if (e.IsOnline)
		{
			LoginUserName = ((e.UserInfo.UserSource == "lmsa") ? e.UserInfo.UserName : e.UserInfo.FullName);
		}
		else
		{
			LoginUserName = string.Empty;
		}
	}

	private void OnLinkClicked(object param)
	{
		string text = param as string;
		if (text == "#")
		{
			((dynamic)MainWindowViewModel.SingleInstance.PluginArr.FirstOrDefault((PluginModel p) => p.Info.PluginID == "8ab04aa975e34f1ca4f9dc3a81374e2c")?.UiElement).ShowStartTutoiral();
		}
		else
		{
			Process.Start(text);
		}
	}

	private bool IsTutorialEnable()
	{
		PluginModel pluginModel = MainWindowViewModel.SingleInstance.PluginArr.FirstOrDefault((PluginModel p) => p.Info.PluginID == "8ab04aa975e34f1ca4f9dc3a81374e2c");
		return pluginModel.IsLoaded;
	}

	private void SetArrItemmCommand(List<QTreeNode> arr, ICommand cmd)
	{
		arr.ForEach(delegate(QTreeNode p)
		{
			if (p.Url == "#" && !IsTutorialEnable())
			{
				p.Url = string.Empty;
				p.Question = string.Empty;
			}
			if (!string.IsNullOrEmpty(p.Url))
			{
				p.LinkCommand = cmd;
			}
			List<QTreeNode> children = p.Children;
			if (children != null && children.Count > 0)
			{
				SetArrItemmCommand(p.Children, cmd);
			}
		});
	}

	public bool Verify()
	{
		bool flag = true;
		foreach (FormItemViewModel mFormItem in mFormItems)
		{
			flag &= mFormItem.Verify();
		}
		return flag;
	}

	private async void SubmitCommandHandler(object args)
	{
		Window win = args as Window;
		foreach (FormItemViewModel item in mFormItems)
		{
			item.Wraning = new FormItemVerifyWraningViewModel();
		}
		mFormItems.Clear();
		EmailAddressVerify email = (EmailAddressVerify)EmailAddress.FormVerify;
		if (IsOnline)
		{
			email.IsCanEmpty = true;
		}
		else
		{
			mFormItems.Add(UserName);
			email.IsCanEmpty = false;
		}
		mFormItems.Add(EmailAddress);
		mFormItems.Add(Comments);
		if (!Verify())
		{
			return;
		}
		long? latestFeedbackId = null;
		FeedBackNodeModel current = Feedback;
		while (current != null)
		{
			if ("Q".Equals(current.Type))
			{
				latestFeedbackId = current.Id;
			}
			if (current.Children != null)
			{
				current = current.Children.FirstOrDefault((FeedBackNodeModel m) => m.Children != null && m.Children.Count > 0);
			}
		}
		FeedbackSubmitModel model = new FeedbackSubmitModel
		{
			Email = EmailAddress.InputValue,
			FeedbackContent = Comments.InputValue,
			FeedbackId = latestFeedbackId,
			Module = $"{SelFunction?.GetLastSelectedId()}",
			IsReplay = (Feedback != null)
		};
		if (DeviceIsConnected)
		{
			if (DeviceInfoIsChecked)
			{
				model.Imei1 = DeviceImei1;
				model.Imei2 = DeviceImei2;
				model.ModelName = ModelName;
				model.SN = DeviceSN;
			}
		}
		else
		{
			model.ModelName = ModelName;
		}
		bool isUploadlog;
		if (!logFileIsChecked)
		{
			CommonWindowModel winModel = new CommonWindowModel(2);
			(winModel.ViewModel as OKCancelViewModel).Title = string.Empty;
			LenovoPopupWindow wnd = winModel.CreateWindow("K0829");
			HostProxy.HostMaskLayerWrapper.New(wnd, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => wnd.ShowDialog());
			isUploadlog = winModel.GetViewModel<OKCancelViewModel>().IsOKResult;
		}
		else
		{
			isUploadlog = LogFileIsChekced;
		}
		Tips = string.Empty;
		SubmitButtonIsEnabled = false;
		FeedBackBLL bll = new FeedBackBLL();
		List<string> files = (isUploadlog ? bll.GetLogFileCopy() : null);
		Stopwatch sw = new Stopwatch();
		sw.Start();
		bool isSuccess = await bll.SubmitFeedbackAsync(model, files);
		if (files != null && files.Count > 0)
		{
			files.ForEach(delegate(string n)
			{
				if (!string.IsNullOrEmpty(n))
				{
					GlobalFun.TryDeleteFile(n);
				}
			});
		}
		sw.Stop();
		HostProxy.BehaviorService.Collect(BusinessType.FEEDBACK, new BusinessData(BusinessType.FEEDBACK, null).Update(sw.ElapsedMilliseconds, isSuccess ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, model));
		SubmitButtonIsEnabled = true;
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			win?.Close();
			ShowTips(isSuccess ? "K0733" : "K0734");
		});
	}

	private void CloseWinCommandHandler(object args)
	{
		if (args is Window window)
		{
			window.Close();
		}
	}

	private void ShowTips(string msg)
	{
		string title = "K0711";
		string okButtonText = "K0327";
		LenovoPopupWindow win = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, title, msg, okButtonText, null);
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
	}
}
