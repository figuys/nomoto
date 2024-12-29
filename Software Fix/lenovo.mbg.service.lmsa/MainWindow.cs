using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GoogleAnalytics;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.framework.lang;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.Business;
using lenovo.mbg.service.lmsa.Feedback.ViewModel;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.Login.ViewModel.UserOperation;
using lenovo.mbg.service.lmsa.ModelV6;
using lenovo.mbg.service.lmsa.OrderView;
using lenovo.mbg.service.lmsa.UserControls;
using lenovo.mbg.service.lmsa.ViewModels;
using lenovo.mbg.service.lmsa.ViewV6;
using lenovo.themes.generic.ModelV6;
using lenovo.themes.generic.ViewModelV6;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa;

public partial class MainWindow : Window, IMessageBox, IComponentConnector, IStyleConnector
{
	private const int WM_CLOSE = 16;

	private const int WM_SYSCOMMAND = 274;

	private const int SC_CLOSE = 61536;

	private const int SC_MINIMIZE = 61472;

	private Storyboard PinStoryboard;

	private DevConnectView devConnWnd = null;

	private object asynic_loc = new object();

	protected static List<ComboboxViewModel> MultiFlashTutorialQuestions = new List<ComboboxViewModel>();

	protected static bool TutorialLock = false;

	private volatile bool _IsFrozen = false;

	private List<IUserMsgControl> _DlgArr = new List<IUserMsgControl>();

	public bool IsBackupOrRestory = false;

	public MainWindow()
	{
		ApplcationClass.ApplcationStartWindow = this;
		InitializeComponent();
		InitPinAnimation();
		base.DataContext = MainWindowViewModel.SingleInstance;
		base.Loaded += MainWindow_Loaded;
		base.Unloaded += MainWindow_Unloaded;
		base.LocationChanged += delegate
		{
			if (DownLoadCenter.IsOpen)
			{
				IntPtr handle = new WindowInteropHelper(this).Handle;
				Screen screen = Screen.FromHandle(handle);
				if (base.Left + base.ActualWidth > (double)screen.WorkingArea.Width)
				{
					DownLoadCenter.IsOpen = false;
				}
				else if (base.Top < 0.0)
				{
					DownLoadCenter.IsOpen = false;
				}
				else if (base.Left < -400.0)
				{
					DownLoadCenter.IsOpen = false;
				}
				else if (base.Top + base.Height > (double)screen.WorkingArea.Height)
				{
					DownLoadCenter.IsOpen = false;
				}
			}
		};
		base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		OnRbtnChecked(rbtn, null);
		devConnWnd = new DevConnectView();
		global::Smart.DeviceManagerEx.BeforeValidateEvent = delegate(dynamic param)
		{
			Tuple<bool, string> tuple = param as Tuple<bool, string>;
			if (tuple.Item1)
			{
				base.Dispatcher.Invoke(delegate
				{
					if (base.WindowState == WindowState.Minimized)
					{
						base.WindowState = WindowState.Normal;
					}
					CloseLowLeveWnd();
					DeviceConnectViewModel.Instance.m_loadingWindow?.Close();
					devConnWnd.ShowWnd(tuple.Item2);
				});
			}
		};
		global::Smart.DeviceManagerEx.AfterValidateEvent = delegate(dynamic param)
		{
			Tuple<bool, string> tuple2 = param as Tuple<bool, string>;
			if (tuple2.Item1)
			{
				base.Dispatcher.Invoke(delegate
				{
					devConnWnd.HideWnd(tuple2.Item2);
				});
			}
		};
	}

	private void InitPinAnimation()
	{
		PinStoryboard = new Storyboard();
		ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
		objectAnimationUsingKeyFrames.KeyFrames.Add(new DiscreteObjectKeyFrame(GlobalFun.LoadBitmap("pack://application:,,,/Software Fix;component/ResourceV6/main_security.png"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
		objectAnimationUsingKeyFrames.KeyFrames.Add(new DiscreteObjectKeyFrame(GlobalFun.LoadBitmap("pack://application:,,,/Software Fix;component/ResourceV6/main_pin.png"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3.0))));
		objectAnimationUsingKeyFrames.Duration = new Duration(TimeSpan.FromSeconds(6.0));
		objectAnimationUsingKeyFrames.RepeatBehavior = RepeatBehavior.Forever;
		PinStoryboard.Children.Add(objectAnimationUsingKeyFrames);
		Storyboard.SetTarget(objectAnimationUsingKeyFrames, validcode);
		Storyboard.SetTargetProperty(objectAnimationUsingKeyFrames, new PropertyPath(System.Windows.Controls.Image.SourceProperty));
		PinStoryboard.Begin(validcode, isControllable: true);
	}

	private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
	{
		global::Smart.DeviceManagerEx.BeforeValidateEvent = null;
		global::Smart.DeviceManagerEx.AfterValidateEvent = null;
	}

	protected override void OnSourceInitialized(EventArgs e)
	{
		base.OnSourceInitialized(e);
		if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
		{
			hwndSource.AddHook(WndProc);
		}
	}

	private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (274 == msg)
		{
			if (wParam.ToInt32() == 61536 && lParam.ToInt32() == 0 && (ApplcationClass.IsUpdatingPlug || ApplcationClass.ForceUpdate))
			{
				MainWindowViewModel.SingleInstance.Exit(0);
			}
			if (61472 != wParam.ToInt32())
			{
			}
		}
		if (537 == msg)
		{
			switch (wParam.ToInt32())
			{
			case 32768:
				try
				{
					string comPortName = HardwareHelper.Com.GetComPortName(wParam, lParam);
					string hardwareInfo = HardwareHelper.GetHardwareInfo(HardwareEnum.Win32_PnPEntity, "K0487", comPortName);
				}
				catch (Exception ex)
				{
					LogHelper.LogInstance.Error("Get Hardware info error:" + ex);
				}
				break;
			}
		}
		return IntPtr.Zero;
	}

	private void MainWindow_Loaded(object sender, RoutedEventArgs e)
	{
		Matrix transformToDevice = PresentationSource.FromVisual(System.Windows.Application.Current.MainWindow).CompositionTarget.TransformToDevice;
		base.MaxWidth = (double)Screen.PrimaryScreen.WorkingArea.Width / transformToDevice.M11;
		base.MaxHeight = (double)Screen.PrimaryScreen.WorkingArea.Height / transformToDevice.M22;
		base.WindowState = WindowState.Normal;
		devConnWnd.Owner = this;
		MainWindowViewModel.SingleInstance.Initialize();
		Task.Run(delegate
		{
			string qFeedbackRespStr = AppContext.WebApi.RequestBase(WebApiUrl.FEEDBACK_GET_ISSUE_INFO, null, 0).content?.ToString();
			FeedbackMainViewModel.QFeedbackRespStr = qFeedbackRespStr;
		});
		btnBuy.Visibility = ((!UserService.Single.CurrentLoggedInUser.B2BBuyNowDisplay) ? Visibility.Collapsed : Visibility.Visible);
		MainWindowViewModel.SingleInstance.UserOperation.MenuItems[1].ItemVisibility = ((!UserService.Single.CurrentLoggedInUser.B2BEntranceEnable) ? Visibility.Collapsed : Visibility.Visible);
		DownLoadCenter.DataContext = DownloadControlViewModel.SingleInstance;
		base.Closing += MainWindow_Closing;
	}

	private void MainWindow_Closing(object sender, CancelEventArgs e)
	{
		if (MainWindowViewModel.SingleInstance.IsExecuteWork())
		{
			e.Cancel = true;
		}
	}

	protected override void OnStateChanged(EventArgs e)
	{
		base.OnStateChanged(e);
		if (MainWindowViewModel.SingleInstance.DeletePersonalDataViewModel.IsShowDeletePersonalDataGuidePopup)
		{
			if (base.WindowState == WindowState.Minimized)
			{
				MainWindowViewModel.SingleInstance.DeletePersonalDataViewModel.IsShowDeletePersonalDataGuide = false;
			}
			else
			{
				MainWindowViewModel.SingleInstance.DeletePersonalDataViewModel.IsShowDeletePersonalDataGuide = true;
			}
		}
	}

	protected override void OnActivated(EventArgs e)
	{
	}

	private void downloadTipsPopupClick(object sender, RoutedEventArgs e)
	{
		ApplcationClass.NonTopmostPopup.IsOpen = false;
	}

	private void OnBtnHelp(object sender, RoutedEventArgs e)
	{
		string clientHelpUrl = new MenuPopupWindowBusiness().GetClientHelpUrl();
		GlobalFun.OpenUrlByBrowser(clientHelpUrl);
	}

	private void OnBtnClose(object sender, RoutedEventArgs e)
	{
		MainWindowViewModel.SingleInstance.CloseWindow();
	}

	private void OnLeftBtnDown(object sender, MouseButtonEventArgs e)
	{
		DragMove();
	}

	private void OnBtnMax(object sender, RoutedEventArgs e)
	{
		if (base.WindowState == WindowState.Normal)
		{
			base.WindowState = WindowState.Maximized;
		}
		else
		{
			base.WindowState = WindowState.Normal;
		}
	}

	private void OnBtnMin(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private void OnBtnFeedback(object sender, RoutedEventArgs e)
	{
		global::Smart.GoogleAnalyticsTracker.Tracker.Send(HitBuilder.CreateCustomEvent(App.Category, "MenuFeedbackButtonClick", "menu-feedback button click", 0L).Build());
		HostProxy.HostOperationService.ShowFeedBack();
	}

	private void OnBtnBuyNow(object sender, RoutedEventArgs e)
	{
		OrderBuyView orderBuyView = new OrderBuyView();
		orderBuyView.ShowDialog();
	}

	private void OnBtnDownLoadCenter(object sender, RoutedEventArgs e)
	{
		ShowDownloadCenter(!DownLoadCenter.IsOpen);
	}

	private void OnValidateCode(object sender, MouseButtonEventArgs e)
	{
		devConnWnd.Display();
	}

	private void LoadBrandAnimation()
	{
		Storyboard storyboard = new Storyboard();
		ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
		objectAnimationUsingKeyFrames.KeyFrames.Add(new DiscreteObjectKeyFrame(GlobalFun.LoadBitmap("pack://application:,,,/Software Fix;component/ResourceV6/LLogo.png"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
		objectAnimationUsingKeyFrames.KeyFrames.Add(new DiscreteObjectKeyFrame(GlobalFun.LoadBitmap("pack://application:,,,/Software Fix;component/ResourceV6/MLogo.png"), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3.0))));
		objectAnimationUsingKeyFrames.Duration = new Duration(TimeSpan.FromSeconds(6.0));
		objectAnimationUsingKeyFrames.RepeatBehavior = RepeatBehavior.Forever;
		storyboard.Children.Add(objectAnimationUsingKeyFrames);
		Storyboard.SetTarget(objectAnimationUsingKeyFrames, Logo);
		Storyboard.SetTargetProperty(objectAnimationUsingKeyFrames, new PropertyPath(System.Windows.Controls.Image.SourceProperty));
		Logo.IsVisibleChanged += delegate(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				storyboard.Begin();
			}
			else
			{
				storyboard.Pause();
			}
		};
	}

	public void FrozenWindow(bool isShowLoading = false)
	{
		base.Dispatcher.Invoke(delegate
		{
			_IsFrozen = true;
			MaskGrid.Visibility = Visibility.Visible;
			if (isShowLoading)
			{
				imgLoading.Visibility = Visibility.Visible;
			}
		});
	}

	public void UnFrozenWindow()
	{
		base.Dispatcher.Invoke(delegate
		{
			_IsFrozen = false;
			imgLoading.Visibility = Visibility.Collapsed;
			if (_DlgArr.Count <= 0)
			{
				MaskGrid.Visibility = Visibility.Collapsed;
			}
		});
	}

	public void SetUiWorkEnable(bool isWorkEnable)
	{
		if (isWorkEnable)
		{
			UnFrozenWindow();
		}
		else
		{
			FrozenWindow();
		}
	}

	public void ShowDownloadCenter(bool isShow = true)
	{
		base.Dispatcher.Invoke(delegate
		{
			DownLoadCenter.IsOpen = isShow;
		});
	}

	public void SelRegistedDevIfExist(string category, Action<string> callBack = null)
	{
		base.Dispatcher.Invoke(delegate
		{
			RegisterDevView wnd = new RegisterDevView("K0067");
			if (wnd.IsExistRegistedDev(category))
			{
				wnd.CallBackAction = delegate(bool? param)
				{
					if (param == true)
					{
						RegistedDevModel selRegistedDev = wnd.GetSelRegistedDev();
						if (selRegistedDev.Brand.Equals("Motorola", StringComparison.CurrentCultureIgnoreCase))
						{
							callBack?.Invoke(selRegistedDev.MotoModelName);
						}
						else
						{
							callBack?.Invoke(selRegistedDev.ModelName);
						}
					}
				};
				ShowMessage(wnd);
			}
		});
	}

	public void CallMotoCare(string imei, object wModel)
	{
		WarrantyInfoBaseModel warrantyInfoBaseModel = wModel as WarrantyInfoBaseModel;
		if (!string.IsNullOrEmpty(imei) && warrantyInfoBaseModel != null)
		{
			warrantyInfoBaseModel.imei = imei;
			MainWindowViewModel.SingleInstance.ShowBanner(warrantyInfoBaseModel);
		}
	}

	public bool? ShowMessage(string msg, out IUserMsgControl win, MessageBoxImage icon = MessageBoxImage.Asterisk, MessageBoxButton btn = MessageBoxButton.OK)
	{
		MessageBoxV6 messageBoxV = (MessageBoxV6)(win = new MessageBoxV6());
		messageBoxV.Init(msg, btn, icon, isCloseBtn: false);
		Display(messageBoxV);
		return messageBoxV.Result;
	}

	public bool? ShowMessage(string msg, MessageBoxButton btn = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Asterisk, bool isCloseBtn = false, bool isPrivacy = false, Action<bool?> closeAction = null)
	{
		return base.Dispatcher.Invoke(delegate
		{
			MessageBoxV6 messageBoxV = new MessageBoxV6();
			messageBoxV.CallBackAction = closeAction;
			messageBoxV.Init(msg, btn, icon, isCloseBtn, isPrivacy);
			Display(messageBoxV);
			return messageBoxV.Result;
		});
	}

	public bool? ShowMessage(string title, string msg, string okBtnText = "K0327", string cancelBtnText = null, bool isCloseBtn = false, ImageSource image = null, MessageBoxImage icon = MessageBoxImage.Asterisk, Action<bool?> closeAction = null, Window ownerWindow = null)
	{
		return base.Dispatcher.Invoke(delegate
		{
			IUserMsgControl userMsgControl = null;
			if (image == null)
			{
				MessageBoxV6 messageBoxV = new MessageBoxV6();
				messageBoxV.Init(title, msg, okBtnText, cancelBtnText, isCloseBtn, icon);
				userMsgControl = messageBoxV;
			}
			else
			{
				RightPicMessageBox rightPicMessageBox = new RightPicMessageBox();
				rightPicMessageBox.Init(image, title, msg, okBtnText, cancelBtnText, null, isCloseBtn);
				userMsgControl = rightPicMessageBox;
			}
			userMsgControl.CallBackAction = closeAction;
			Display(userMsgControl, ownerWindow);
			return userMsgControl.Result;
		});
	}

	public bool? ShowMessage(string title, string msg, string okBtnText, string cancelBtnText, bool isCloseBtn, ImageSource image, out Window wnd)
	{
		IUserMsgControl userMsgControl = null;
		if (image == null)
		{
			MessageBoxV6 messageBoxV = new MessageBoxV6();
			messageBoxV.Init(title, msg, okBtnText, cancelBtnText, isCloseBtn, MessageBoxImage.Asterisk);
			userMsgControl = messageBoxV;
		}
		else
		{
			RightPicMessageBox rightPicMessageBox = new RightPicMessageBox();
			rightPicMessageBox.Init(image, title, msg, okBtnText, cancelBtnText, null, isCloseBtn);
			userMsgControl = rightPicMessageBox;
		}
		wnd = userMsgControl.GetMsgUi();
		Display(userMsgControl);
		return userMsgControl.Result;
	}

	public bool? ShowMessage(string title, string msg, string okBtnText, string cancelBtnText, MessageBoxImage icon, string notifyText)
	{
		return base.Dispatcher.Invoke(delegate
		{
			MessageBoxV6 messageBoxV = new MessageBoxV6();
			messageBoxV.Init(title, msg, okBtnText, cancelBtnText, icon, notifyText);
			Display(messageBoxV);
			return messageBoxV.Result;
		});
	}

	public bool? ShowConfrimCloseMessage(out Action<bool?> callBack, string title, string msg, string okBtnText = "K0327", string cancelBtnText = null, string tips = null, ImageSource image = null, Action<bool?> closeAction = null)
	{
		RightPicMessageBox box = new RightPicMessageBox();
		box.Init(image, title, msg, okBtnText, cancelBtnText, tips, isCloseBtn: true, isPopup: true);
		box.CallBackAction = closeAction;
		callBack = delegate(bool? param)
		{
			box.Close();
			OnMsgCloseAction(box, param);
		};
		Display(box);
		return box.Result;
	}

	public bool? ShowMessageWithPrivacy(string title, string msg, string okBtnText = "K0327", string cancelBtnText = null, bool isCloseBtn = false, MessageBoxImage icon = MessageBoxImage.Asterisk, Action<bool?> closeAction = null)
	{
		MessageBoxV6 messageBoxV = new MessageBoxV6();
		messageBoxV.Init(title, msg, okBtnText, cancelBtnText, isCloseBtn, icon, isPrivacy: true);
		messageBoxV.CallBackAction = closeAction;
		Display(messageBoxV);
		return messageBoxV.Result;
	}

	public bool? ShowOutterCloseMessage(string title, string msg, MessageBoxImage icon = MessageBoxImage.Asterisk, Action<IUserMsgControl> outterAction = null)
	{
		return base.Dispatcher.Invoke(delegate
		{
			MessageBoxV6 messageBoxV = new MessageBoxV6();
			messageBoxV.Init(title, msg, null, null, isCloseBtn: false, icon);
			outterAction?.Invoke(messageBoxV);
			Display(messageBoxV);
			return messageBoxV.Result;
		});
	}

	public bool? ShowRescueBackupWnd(string title, string msg, string imagePath, string okBtnText = "K0327", string cancelBtnText = null, bool isCloseBtn = false, bool isNotifyText = false)
	{
		return base.Dispatcher.Invoke(delegate
		{
			PicMessageBox picMessageBox = new PicMessageBox();
			picMessageBox.Init(title, msg, imagePath, okBtnText, cancelBtnText, isCloseBtn, isNotifyText);
			Display(picMessageBox);
			return picMessageBox.Result;
		});
	}

	public bool? ContentMssage(FrameworkElement content, string title, string okBtn = "K0327", string cancelBtn = null, bool isCloseBtn = false, MessageBoxImage icon = MessageBoxImage.Asterisk)
	{
		ContentMessageBox contentMessageBox = new ContentMessageBox(content, title, okBtn, cancelBtn, isCloseBtn, icon);
		Display(contentMessageBox);
		return contentMessageBox.Result;
	}

	public bool? ShowMessage(IUserMsgControl ui)
	{
		return System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			Display(ui);
			return ui.Result;
		});
	}

	public void ShowB2BExpired(int _modeType)
	{
		ShowMessage(new OrderExpiredView(_modeType));
		btnBuy.Visibility = Visibility.Visible;
		UserService.Single.SaveB2BBuyNowButtonDisplay();
	}

	public void ShowB2BRemind(int used, int total)
	{
		ShowMessage(new OrderRemindView(used, total));
		btnBuy.Visibility = Visibility.Visible;
		UserService.Single.SaveB2BBuyNowButtonDisplay();
	}

	public void CloseLowLeveWnd()
	{
		for (int num = _DlgArr.Count - 1; num >= 0; num--)
		{
			IUserMsgControl item = _DlgArr[num];
			Window msgUi = _DlgArr[num].GetMsgUi();
			if (msgUi.Tag != null && (UserMsgWndData)msgUi.Tag == UserMsgWndData.CanCloseByOthers)
			{
				msgUi.Close();
				_DlgArr.Remove(item);
			}
		}
	}

	private void Display(IUserMsgControl ui, Window ownerWindow = null)
	{
		if (!ui.Result.HasValue)
		{
			ui.CloseAction = delegate(bool? param)
			{
				OnMsgCloseAction(ui, param);
			};
			_DlgArr.Add(ui);
			MaskGrid.Visibility = Visibility.Visible;
			Window msgUi = ui.GetMsgUi();
			msgUi.Owner = ownerWindow ?? this;
			msgUi.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			GlobalCmdHelper.Instance.Execute(new
			{
				type = GlobalCmdType.CLOSE_RESCUE_HELP
			});
			msgUi.ShowDialog();
		}
	}

	public void OnMsgCloseAction(IUserMsgControl ui, bool? param)
	{
		_DlgArr.Remove(ui);
		if (_DlgArr.Count == 0 && !_IsFrozen)
		{
			MaskGrid.Visibility = Visibility.Collapsed;
		}
		ui.Result = param;
		ui.CallBackAction?.Invoke(param);
	}

	public void SetDeviceConnectIconStatus(int _status)
	{
		if (_status == 99)
		{
			DeviceConnectViewModel.Instance.ShowConnectedDeviceIcon();
		}
		else
		{
			DeviceConnectViewModel.Instance.Status = _status;
		}
	}

	public void ShowMutilIcon(bool showIcon, bool showList)
	{
		base.Dispatcher.Invoke(delegate
		{
			mutilicons.Visibility = ((!showIcon) ? Visibility.Collapsed : Visibility.Visible);
			devicelist.Visibility = ((!showList) ? Visibility.Collapsed : Visibility.Visible);
		});
	}

	public void ChangeIsEnabled(bool isEnabled)
	{
		base.Dispatcher.Invoke(delegate
		{
			help.ChangeIsEnabled(isEnabled);
			notice.ChangeIsEnabled(isEnabled);
			setting.ChangeIsEnabled(isEnabled);
			user.ChangeIsEnabled(isEnabled);
			btnDownload.IsEnabled = isEnabled;
			btnDownload.Opacity = (isEnabled ? 1.0 : 0.3);
			minbtn.IsEnabled = isEnabled;
			minbtn.Opacity = (isEnabled ? 1.0 : 0.3);
			closebtn.IsEnabled = isEnabled;
			closebtn.Opacity = (isEnabled ? 1.0 : 0.3);
			validcode.IsEnabled = isEnabled;
			validcode.Opacity = (isEnabled ? 1.0 : 0.3);
			feedback.IsEnabled = isEnabled;
			feedback.Opacity = (isEnabled ? 1.0 : 0.3);
			btnBuy.IsEnabled = isEnabled;
			btnBuy.Opacity = (isEnabled ? 1.0 : 0.3);
			foreach (PluginModel item in MainWindowViewModel.SingleInstance.PluginArr)
			{
				if (!isEnabled)
				{
					if (item.Info.PluginID != HostProxy.HostNavigation.CurrentPluginID)
					{
						item.IsEnabled = isEnabled;
					}
				}
				else
				{
					item.IsEnabled = isEnabled;
				}
			}
		});
	}

	private void OnLButtonDown(object sender, MouseButtonEventArgs e)
	{
		DownloadControlViewModel.SingleInstance.ModifyButtonDownloadPath();
	}

	private void OnRbtnChecked(object sender, RoutedEventArgs e)
	{
		if (DownLoadListBox != null)
		{
			System.Windows.Controls.RadioButton radioButton = sender as System.Windows.Controls.RadioButton;
			DownLoadListBox.DataContext = DownloadControlViewModel.SingleInstance;
			if (Convert.ToBoolean(radioButton.Tag))
			{
				DownLoadListBox.ItemContainerStyle = TryFindResource("V6_DownLoadingListBoxItemStyle") as Style;
				DownLoadListBox.ItemsSource = DownloadControlViewModel.SingleInstance.DownloadingTasks;
			}
			else
			{
				DownLoadListBox.ItemContainerStyle = TryFindResource("V6_DownLoadedListBoxItemStyle") as Style;
				DownLoadListBox.ItemsSource = DownloadControlViewModel.SingleInstance.DownloadedTasks;
				DownloadControlViewModel.SingleInstance.DeleteInvalidDownloaded();
			}
		}
	}

	public void ShowDownloadCenterHere(bool isShow)
	{
		if (isShow)
		{
			ShowDownloadCenterHere(isShow: false);
		}
		DownLoadCenterHere.Visibility = ((!isShow) ? Visibility.Collapsed : Visibility.Visible);
	}

	private void OnCloseDownLoadCenter(object sender, RoutedEventArgs e)
	{
		DownLoadCenter.IsOpen = false;
		ShowDownloadCenterHere(isShow: true);
	}

	private void OnBtnDownLoadHere(object sender, RoutedEventArgs e)
	{
		ShowDownloadCenterHere(isShow: false);
	}

	public void SetDriverButtonStatus(string _code)
	{
		if (btnIconDriver.Visibility == Visibility.Visible)
		{
			base.Dispatcher.Invoke(delegate
			{
				btnIconDriver.Tag = _code;
			});
		}
	}

	public void ShowQuitSurvey()
	{
		if (UserService.Single.CurrentLoggedInUser == null)
		{
			return;
		}
		switch (UserService.Single.CurrentLoggedInUser.quitSurvey)
		{
		case 0:
			if (Configurations.RescueResultMap[true] <= 0 && Configurations.RescueResultMap[false] + UserService.Single.CurrentLoggedInUser.failureNum < 5)
			{
				break;
			}
			goto case 1;
		case 1:
		{
			string userId = ((UserService.Single.CurrentLoggedInUser == null) ? string.Empty : UserService.Single.CurrentLoggedInUser.UserId);
			SurveyWindowV6 win = new SurveyWindowV6();
			win.DataContext = new SurveyWindowV2ViewModel(userId);
			HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => win.ShowDialog());
			break;
		}
		}
	}

	public void ShowB2BPurchaseOverview()
	{
		Plugin.UnselectAll();
		ShowMutilIcon(showIcon: false, showList: true);
		mainWindowContent.Content = new B2BPurchaseOverviewV6();
		mainWindowContent.Visibility = Visibility.Visible;
	}

	private void Plugin_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		mainWindowContent.Visibility = Visibility.Collapsed;
	}

	public void ShowMutilTutorials(bool show)
	{
		Thread.Sleep(new Random().Next(100));
		if (TutorialLock)
		{
			return;
		}
		try
		{
			TutorialLock = true;
			if (show)
			{
				if (mutiltutorials.Visibility == Visibility.Visible)
				{
					return;
				}
				if (MultiFlashTutorialQuestions.Count == 0)
				{
					JObject jObject = AppContext.WebApi.RequestContent<JObject>(WebApiUrl.GET_MUTIL_TUTORIALS_QUESTIONS, new Dictionary<string, string> { 
					{
						"language",
						LMSAContext.CurrentLanguage
					} });
					if (jObject == null)
					{
						return;
					}
					JArray jArray = jObject.Value<JArray>("questions");
					if (jArray != null && jArray.HasValues)
					{
						IEnumerator<JToken> enumerator = jArray.OrderBy((JToken n) => n.Value<int>("order")).GetEnumerator();
						while (enumerator.MoveNext())
						{
							MultiFlashTutorialQuestions.Add(new ComboboxViewModel
							{
								Key = enumerator.Current.Value<int>("order"),
								Content = enumerator.Current.Value<string>("question"),
								ExtendData = enumerator.Current.Value<string>("questionUrl")
							});
						}
					}
				}
				base.Width = 1470.0;
				mutiltutorials.Visibility = Visibility.Visible;
				tutorialsList.ItemsSource = MultiFlashTutorialQuestions;
			}
			else
			{
				base.Width = 1012.0;
				mutiltutorials.Visibility = Visibility.Collapsed;
			}
			Screen screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
			Graphics graphics = Graphics.FromHwnd(new WindowInteropHelper(this).Handle);
			double num = graphics.DpiX / 96f;
			double num2 = graphics.DpiY / 96f;
			base.Left = (double)screen.Bounds.Left / num + ((double)screen.Bounds.Width / num - base.ActualWidth) / 2.0;
			base.Top = (double)screen.Bounds.Top / num2 + ((double)screen.Bounds.Height / num2 - base.ActualHeight) / 2.0;
		}
		finally
		{
			TutorialLock = false;
		}
	}

	private void MutilTutorialsCloseClick(object sender, RoutedEventArgs e)
	{
		ShowMutilTutorials(show: false);
	}

	private void MutilTutorialsItemClick(object sender, MouseButtonEventArgs e)
	{
		ComboboxViewModel comboboxViewModel = null;
		if (e.Source is LangTextBlock langTextBlock)
		{
			comboboxViewModel = langTextBlock.DataContext as ComboboxViewModel;
		}
		else if (e.Source is LangRun langRun)
		{
			comboboxViewModel = langRun.DataContext as ComboboxViewModel;
		}
		if (comboboxViewModel != null && comboboxViewModel.ExtendData != null)
		{
			GlobalFun.OpenUrlByBrowser(comboboxViewModel.ExtendData.ToString());
		}
	}

	public void ChangePinStoryboard(bool start)
	{
		base.Dispatcher.Invoke(delegate
		{
			if (start)
			{
				InitPinAnimation();
			}
			else
			{
				PinStoryboard.Remove(validcode);
				validcode.Source = GlobalFun.LoadBitmap("pack://application:,,,/Software Fix;component/ResourceV6/main_security.png");
			}
		});
	}

	public void LogOut()
	{
		System.Windows.Application.Current.Dispatcher.Invoke(delegate
		{
			LogoutMenuItemViewModel.LogOut(force: true);
		});
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
