using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.flash.Common;
using lenovo.mbg.service.lmsa.flash.ModelV6;
using lenovo.mbg.service.lmsa.flash.Tutorials;
using lenovo.mbg.service.lmsa.flash.Tutorials.Model;
using lenovo.mbg.service.lmsa.flash.Tutorials.RescueTutorials;
using lenovo.mbg.service.lmsa.flash.UserModelV2;
using lenovo.mbg.service.lmsa.flash.ViewV6.Dialog;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.Controls.Windows;
using lenovo.themes.generic.ModelV6;

namespace lenovo.mbg.service.lmsa.flash.ViewV6;

public partial class MainFrameV6 : UserControl, IComponentConnector
{
	public readonly List<string> FastbootErrorStatusArr = new List<string> { "0x00FF", "0x00ff", "0xDEAD", "0xdead" };

	protected WarrantyService warrantyService = new WarrantyService();

	protected PageIndex currentPageIndex;

	private readonly Dictionary<string, int> _FastbootDevUnMatchArr;

	private static long autoMatchLock;

	public string RomMatchId { get; set; }

	public MainFrameViewModeV6 VM { get; private set; }

	public IMessageBox IMsgManager => Plugin.IMsgManager;

	public static MainFrameV6 Instance { get; private set; }

	public bool SupportMulti => Plugin.SupportMulti;

	public PageIndex CurrentPageIndex
	{
		get
		{
			return currentPageIndex;
		}
		protected set
		{
			currentPageIndex = value;
			Plugin.ShowMutilIcon();
		}
	}

	public List<IAMatchView> RescueViews { get; set; }

	public static bool InalidViewIsShow { get; set; }

	public MainFrameV6()
	{
		InitializeComponent();
		Instance = this;
		_FastbootDevUnMatchArr = new Dictionary<string, int>();
		RescueViews = new List<IAMatchView>();
		VM = new MainFrameViewModeV6();
		base.DataContext = VM;
		CurrentPageIndex = PageIndex.RESCUE_HOME;
		base.Loaded += OnLoaded;
		base.Unloaded += OnUnloaded;
		GlobalCmdHelper.Instance.ReadDevieInfoCallback = delegate(bool success)
		{
			if (!success && CurrentPageIndex == PageIndex.PHONE_ENTRANCE)
			{
				CloseGifGuideSteps();
				base.Dispatcher.Invoke(delegate
				{
					if (!InalidViewIsShow)
					{
						InalidViewIsShow = true;
						InvalidView invalidView = new InvalidView(DevCategory.Phone, 2);
						if (Interlocked.Read(ref autoMatchLock) == 0L && IMsgManager.ShowMessage(invalidView) == true)
						{
							ChangeView(invalidView.IsManualModel ? PageIndex.PHONE_MANUAL : PageIndex.PHONE_SEARCH);
						}
						InalidViewIsShow = false;
					}
				});
			}
		};
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		if (HostProxy.deviceManager.MasterDevice != null && HostProxy.deviceManager.MasterDevice.SoftStatus == DeviceSoftStateEx.Online)
		{
			if (CurrentPageIndex == PageIndex.RESCUE_FLASH)
			{
				if ((HostProxy.deviceManager.MasterDevice.Property.Category ?? "phone") == "phone")
				{
					ChangeView(PageIndex.PHONE_ENTRANCE);
				}
				else
				{
					ChangeView(PageIndex.RESCUE_HOME);
				}
			}
			Task.Run(delegate
			{
				AutoMatch(HostProxy.deviceManager.MasterDevice, jumpToMatchView: true);
			});
		}
		HostProxy.deviceManager.Connecte += OnDevConnecte;
		HostProxy.deviceManager.DisConnecte += OnDisConnecte;
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
		HostProxy.deviceManager.Connecte -= OnDevConnecte;
		HostProxy.deviceManager.DisConnecte -= OnDisConnecte;
	}

	public void ChangeView(PageIndex index, Dictionary<string, string> condition = null)
	{
		base.Dispatcher.Invoke(delegate
		{
			CurrentPageIndex = index;
			ChangeMutilDeviceShowType(CurrentPageIndex == PageIndex.RESCUE_FLASH && SupportMulti);
			switch (index)
			{
			case PageIndex.PHONE_SEARCH:
			{
				FrameworkElement frameworkElement = null;
				if (!SupportMulti)
				{
					if (!VM.PageViewArr.ContainsKey(index))
					{
						frameworkElement = new PhoneQueryViewV6();
						VM.PageViewArr.Add(index, frameworkElement);
					}
					else
					{
						frameworkElement = VM.PageViewArr[index];
					}
				}
				else
				{
					frameworkElement = new PhoneQueryViewV6();
				}
				VM.CurrentView = frameworkElement;
				ChangeFooter(phoneHost: false);
				break;
			}
			case PageIndex.PHONE_MANUAL:
			{
				PhoneMMatchViewV6 phoneMMatchViewV = null;
				if (!SupportMulti)
				{
					if (!VM.PageViewArr.ContainsKey(index))
					{
						phoneMMatchViewV = new PhoneMMatchViewV6();
						VM.PageViewArr.Add(index, phoneMMatchViewV);
					}
					else
					{
						phoneMMatchViewV = VM.PageViewArr[index] as PhoneMMatchViewV6;
					}
				}
				else
				{
					phoneMMatchViewV = new PhoneMMatchViewV6();
				}
				VM.CurrentView = phoneMMatchViewV;
				ChangeFooter(phoneHost: false);
				if (condition != null && condition.ContainsKey("modelName"))
				{
					phoneMMatchViewV.VM.MatchFromDownloadCenter(condition);
				}
				break;
			}
			case PageIndex.PHONE_ENTRANCE:
			{
				if (!VM.PageViewArr.ContainsKey(index))
				{
					VM.PageViewArr.Add(index, new PhoneEntranceViewV6());
				}
				PhoneEntranceViewV6 phoneEntranceViewV = VM.PageViewArr[index] as PhoneEntranceViewV6;
				phoneEntranceViewV.UpdateGridLayout();
				VM.CurrentView = phoneEntranceViewV;
				ChangeFooter(phoneHost: true);
				break;
			}
			case PageIndex.TABLET_ENTRANCE:
			{
				if (!VM.PageViewArr.ContainsKey(index))
				{
					VM.PageViewArr.Add(index, new TabletEntranceViewV6());
				}
				TabletEntranceViewV6 currentView = VM.PageViewArr[index] as TabletEntranceViewV6;
				VM.CurrentView = currentView;
				ChangeFooter(phoneHost: true);
				break;
			}
			case PageIndex.TABLET_SEARCH:
				if (!VM.PageViewArr.ContainsKey(index))
				{
					VM.PageViewArr.Add(index, new TabletQMatchViewV6());
				}
				VM.CurrentView = VM.PageViewArr[index];
				ChangeFooter(phoneHost: false);
				break;
			case PageIndex.TABLET_MANUAL:
			{
				if (!VM.PageViewArr.ContainsKey(index))
				{
					VM.PageViewArr.Add(index, new TabletMMatchViewV6());
				}
				TabletMMatchViewV6 tabletMMatchViewV = VM.PageViewArr[index] as TabletMMatchViewV6;
				VM.CurrentView = tabletMMatchViewV;
				ChangeFooter(phoneHost: false);
				if (condition != null && condition.ContainsKey("modelName"))
				{
					tabletMMatchViewV.VM.MatchFromDownloadCenter(condition);
				}
				break;
			}
			case PageIndex.SMART_SEARCH:
				if (!VM.PageViewArr.ContainsKey(index))
				{
					VM.PageViewArr.Add(index, new SmartQMatchViewV6());
				}
				VM.CurrentView = VM.PageViewArr[index];
				ChangeFooter(phoneHost: false);
				break;
			case PageIndex.SMART_MANUAL:
			{
				if (!VM.PageViewArr.ContainsKey(index))
				{
					VM.PageViewArr.Add(index, new SmartMMatchViewV6());
				}
				SmartMMatchViewV6 smartMMatchViewV = VM.PageViewArr[index] as SmartMMatchViewV6;
				VM.CurrentView = smartMMatchViewV;
				ChangeFooter(phoneHost: false);
				if (condition != null && condition.ContainsKey("modelName"))
				{
					smartMMatchViewV.VM.MatchFromDownloadCenter(condition);
				}
				break;
			}
			case PageIndex.RESCUE_HOME:
				VM.FooterVisible = Visibility.Collapsed;
				VM.CurrentView = VM.PageViewArr[index];
				break;
			case PageIndex.RESCUE_FLASH:
			{
				IAMatchView view = VM.Devices.First((DeviceViewModel n) => n.IsSelected).View;
				VM.CurrentView = view.RescueView ?? (view as FrameworkElement);
				ChangeFooter(phoneHost: false);
				break;
			}
			case PageIndex.TABLET_AUTO:
			case PageIndex.SMART_AUTO:
				break;
			}
		});
	}

	public void JumptoRescueView(DevCategory category, AutoMatchResource data, object wModel = null, FrameworkElement parentView = null, bool jumpToMatchView = false)
	{
		base.Dispatcher.Invoke(delegate
		{
			LogHelper.LogInstance.Info($"Category:[{category}] Device Info:[{JsonHelper.SerializeObject2Json(data.deviceInfo)}].");
			LogHelper.LogInstance.Info($"Match info:[{data.matchInfo}].");
			if (category == DevCategory.Phone)
			{
				IAMatchView iAMatchView = VM.InitPhoneMatchView(data, wModel, parentView, jumpToMatchView);
				if (iAMatchView != null)
				{
					if (VM.Devices.Where((DeviceViewModel n) => !string.IsNullOrEmpty(n.Id)).Count((DeviceViewModel n) => !n.View.VM.UcDevice.RealFlash) > 0)
					{
						DeviceViewModel deviceViewModel = VM.Devices.FirstOrDefault((DeviceViewModel n) => string.IsNullOrEmpty(n.Id) && n.IsEnabled);
						if (deviceViewModel != null)
						{
							deviceViewModel.IsEnabled = false;
						}
					}
					CurrentPageIndex = PageIndex.RESCUE_FLASH;
					VM.CurrentView = iAMatchView.RescueView ?? (iAMatchView as FrameworkElement);
					ChangeMutilDeviceShowType(SupportMulti);
				}
			}
			else
			{
				FrameworkElement frameworkElement = null;
				bool flag = false;
				if (category == DevCategory.Tablet)
				{
					if (!VM.PageViewArr.ContainsKey(PageIndex.TABLET_AUTO))
					{
						flag = true;
						VM.PageViewArr.Add(PageIndex.TABLET_AUTO, new TabletAMatchViewV6());
					}
					frameworkElement = VM.PageViewArr[PageIndex.TABLET_AUTO];
				}
				else
				{
					if (!VM.PageViewArr.ContainsKey(PageIndex.SMART_AUTO))
					{
						flag = true;
						VM.PageViewArr.Add(PageIndex.SMART_AUTO, new SmartAMatchViewV6());
					}
					frameworkElement = VM.PageViewArr[PageIndex.SMART_AUTO];
				}
				(frameworkElement as IAMatchView).Init(data, wModel, parentView);
				CurrentPageIndex = PageIndex.RESCUE_FLASH;
				VM.CurrentView = frameworkElement;
				if (flag)
				{
					RescueViews.Add(frameworkElement as IAMatchView);
				}
				ChangeMutilDeviceShowType(mutil: false);
			}
			ChangeFooter(phoneHost: false);
		});
	}

	public void BacktoView()
	{
		switch (CurrentPageIndex)
		{
		case PageIndex.PHONE_SEARCH:
		case PageIndex.PHONE_MANUAL:
			ChangeView(PageIndex.PHONE_ENTRANCE);
			break;
		case PageIndex.TABLET_SEARCH:
			ChangeView(PageIndex.TABLET_ENTRANCE);
			break;
		case PageIndex.RESCUE_HOME:
		case PageIndex.PHONE_ENTRANCE:
		case PageIndex.SMART_SEARCH:
			ChangeView(PageIndex.RESCUE_HOME);
			break;
		case PageIndex.RESCUE_FLASH:
			if (VM.CurrentView is IAMatchView { ParentView: null } iAMatchView)
			{
				if (iAMatchView.VM.Category == DevCategory.Phone)
				{
					ChangeView(PageIndex.PHONE_ENTRANCE);
				}
				else
				{
					ChangeView(PageIndex.RESCUE_HOME);
				}
			}
			else
			{
				FromRescueViewToPreviousView();
			}
			break;
		case PageIndex.TABLET_MANUAL:
			ChangeView(PageIndex.TABLET_SEARCH);
			break;
		case PageIndex.SMART_MANUAL:
			ChangeView(PageIndex.SMART_SEARCH);
			break;
		case PageIndex.TABLET_ENTRANCE:
		case PageIndex.TABLET_AUTO:
		case PageIndex.SMART_AUTO:
			break;
		}
	}

	public void FromRescueViewToPreviousView()
	{
		IAMatchView view = VM.CurrentView as IAMatchView;
		FormRescueSuccessViewToPrevView(view);
	}

	public void FormRescueSuccessViewToPrevView(IAMatchView view)
	{
		ChangeMutilDeviceShowType(mutil: false);
		FrameworkElement parentView = view.ParentView;
		RescueViews.Remove(view);
		if (parentView == null)
		{
			ChangeFooter(phoneHost: true);
			ChangeView((view.VM.Category == DevCategory.Phone) ? PageIndex.PHONE_ENTRANCE : PageIndex.RESCUE_HOME);
			return;
		}
		if (parentView.Tag is PageIndex pageIndex)
		{
			CurrentPageIndex = pageIndex;
		}
		else
		{
			CurrentPageIndex = PageIndex.RESCUE_HOME;
		}
		ChangeFooter(phoneHost: false);
		VM.CurrentView = parentView;
	}

	public void ChangeStatus(string id, FlashStatusV6 status)
	{
		if (CurrentPageIndex != 0)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				ChangeFooter(CurrentPageIndex == PageIndex.PHONE_ENTRANCE);
			});
		}
		DeviceViewModel found = VM.Devices.FirstOrDefault((DeviceViewModel n) => n.Id == id);
		if (found != null)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				found.IsRescuing = status == FlashStatusV6.Rescuing;
				string statusKey = null;
				switch (status)
				{
				case FlashStatusV6.Ready:
					statusKey = "K1647";
					found.Status = status.ToString();
					break;
				case FlashStatusV6.PASS:
					statusKey = "K1648";
					found.Status = status.ToString();
					break;
				case FlashStatusV6.FAIL:
					statusKey = "K1649";
					found.Status = status.ToString();
					break;
				case FlashStatusV6.QUIT:
					statusKey = "K1650";
					found.Status = status.ToString();
					break;
				default:
					found.Status = null;
					break;
				}
				found.StatusKey = statusKey;
			});
		}
		ChangeHostIsEnabled(status == FlashStatusV6.Rescuing);
	}

	public void ChangeHostIsEnabled(bool rescuing)
	{
		Plugin.IsExecuteRescueWork = rescuing || VM.Devices.Count((DeviceViewModel n) => n.IsRescuing) > 0;
		IMsgManager.ChangeIsEnabled(!Plugin.IsExecuteRescueWork);
	}

	public void ChangeStatusWhenStartRescue(bool rescuing, bool isPhone = false)
	{
		ChangeHostIsEnabled(rescuing);
		Application.Current.Dispatcher.Invoke(delegate
		{
			if (rescuing || (Plugin.SupportMulti && isPhone))
			{
				VM.BackVisible = Visibility.Collapsed;
			}
			else
			{
				VM.BackVisible = Visibility.Visible;
			}
		});
	}

	public void RealFalshing(string id)
	{
		if (VM.Devices.FirstOrDefault((DeviceViewModel n) => n.Id == id) == null)
		{
			return;
		}
		DeviceViewModel found = VM.Devices.FirstOrDefault((DeviceViewModel n) => string.IsNullOrEmpty(n.Id));
		if (found != null)
		{
			Application.Current.Dispatcher.Invoke(() => found.IsEnabled = true);
		}
	}

	public bool ValidOtherCategoryRescueing(DevCategory category)
	{
		IAMatchView found = RescueViews.FirstOrDefault((IAMatchView n) => !n.VM.Category.Equals(category) && n.VM.UcDevice.Locked);
		if (found != null)
		{
			if (IMsgManager.ShowMessage("K0711", "K1644", "K1645", null, isCloseBtn: true) == true)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					CurrentPageIndex = PageIndex.RESCUE_FLASH;
					VM.CurrentView = found.RescueView ?? (found as FrameworkElement);
					ChangeFooter(phoneHost: false);
					ChangeMutilDeviceShowType(found.VM.Category == DevCategory.Phone && SupportMulti);
				});
			}
			return true;
		}
		return false;
	}

	public void ChangeMutilDeviceShowType(bool mutil)
	{
		if (mutil)
		{
			if (expand.Visibility == Visibility.Collapsed && devicelist.Visibility == Visibility.Collapsed)
			{
				expand.Visibility = Visibility.Collapsed;
				devicelist.Visibility = Visibility.Visible;
			}
			container.HorizontalAlignment = ((!(VM.CurrentView is IAMatchView)) ? HorizontalAlignment.Center : HorizontalAlignment.Left);
			containerBorder.Background = TryFindResource("V6_SubThemeBrushKey") as SolidColorBrush;
		}
		else
		{
			expand.Visibility = Visibility.Collapsed;
			devicelist.Visibility = Visibility.Collapsed;
			container.HorizontalAlignment = HorizontalAlignment.Center;
			containerBorder.Background = TryFindResource("V6_SubThemeBrushKey") as SolidColorBrush;
		}
	}

	public void ChangeContainerHorizontalAlignment(HorizontalAlignment horizontalAlignment)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			if (SupportMulti)
			{
				container.HorizontalAlignment = horizontalAlignment;
			}
		});
	}

	public void RemoveDevice(string id, bool forcerRemove = false)
	{
		DeviceViewModel found = VM.Devices.FirstOrDefault((DeviceViewModel n) => n.Id == id);
		if (found != null && (!found.View.VM.UcDevice.Locked || forcerRemove))
		{
			base.Dispatcher.Invoke(delegate
			{
				VM.RemoveDeviceCommandHandler(found);
			});
		}
	}

	public WarrantyInfoBaseModel LoadWarranty(string data)
	{
		WarrantyInfoBaseModel result = warrantyService.GetWarrantyInfo<WarrantyInfoBaseModel>(data).Result;
		LogHelper.LogInstance.Info("warranty query data:[" + data + "] result:[" + JsonHelper.SerializeObject2Json(result) + "]");
		return result;
	}

	public void ShowStartTutoiral()
	{
		TutorialDefineModel currentModel = new StartTutorial();
		TutorialsWindow userUi = new TutorialsWindow
		{
			DataContext = new TutorialsWindowViewModel().Inititalize(currentModel, null)
		};
		IMsgManager.ShowMessage(userUi);
	}

	public void ShowTutoiral(DevCategory category)
	{
		TutorialDefineModel currentModel = new RescueTutorial(category);
		TutorialsWindow userUi = new TutorialsWindow
		{
			DataContext = new TutorialsWindowViewModel().Inititalize(currentModel, category)
		};
		IMsgManager.ShowMessage(userUi);
	}

	public void ShowGifGuideSteps(bool _showTextDetect, string modelname)
	{
		GuidStepsView userUi = new GuidStepsView(modelname);
		IMsgManager.ShowMessage(userUi);
	}

	public void CloseGifGuideSteps()
	{
		GlobalCmdHelper.Instance.Execute(new
		{
			type = GlobalCmdType.CLOSE_GUID_SETP_DIALOG
		});
	}

	public void ShowOkWindow(string title, string content, string okButton, ImageSource image = null)
	{
		title = HostProxy.LanguageService.Translate(title);
		content = HostProxy.LanguageService.Translate(content);
		IMsgManager.ShowMessage(title, content, okButton, null, isCloseBtn: false, image);
	}

	private void OnBtnBack(object sender, RoutedEventArgs e)
	{
		BacktoView();
	}

	private void OnBtnHome(object sender, RoutedEventArgs e)
	{
		ChangeView(PageIndex.RESCUE_HOME);
	}

	private void OnBtnReturnRescue(object sender, RoutedEventArgs e)
	{
		ChangeView(PageIndex.RESCUE_FLASH);
	}

	private void HideClick(object sender, RoutedEventArgs e)
	{
		devicelist.Visibility = Visibility.Collapsed;
		expand.Visibility = Visibility.Visible;
		container.HorizontalAlignment = HorizontalAlignment.Center;
		containerBorder.Background = TryFindResource("V6_SubThemeBrushKey") as SolidColorBrush;
	}

	private void ShowClick(object sender, RoutedEventArgs e)
	{
		devicelist.Visibility = Visibility.Visible;
		expand.Visibility = Visibility.Collapsed;
		container.HorizontalAlignment = ((!(VM.CurrentView is IAMatchView)) ? HorizontalAlignment.Center : HorizontalAlignment.Left);
		containerBorder.Background = TryFindResource("V6_SubThemeBrushKey") as SolidColorBrush;
	}

	private void ChangeFooter(bool phoneHost)
	{
		VM.FooterVisible = Visibility.Visible;
		if (phoneHost)
		{
			VM.BackBtnStyle = TryFindResource("V6_DarkBackBtnStyle") as Style;
			VM.BorderBrush = TryFindResource("V6_BorderBrushKey1") as SolidColorBrush;
			VM.PageBackground = TryFindResource("V6_ThemeBrushKey") as SolidColorBrush;
			VM.ReturnRescueVisible = ((!SupportMulti || VM.Count <= 0) ? Visibility.Collapsed : Visibility.Visible);
			VM.HomeVisible = Visibility.Visible;
			VM.BackVisible = Visibility.Collapsed;
			return;
		}
		VM.BackBtnStyle = TryFindResource("V6_BackBtnStyle") as Style;
		VM.BorderBrush = TryFindResource("V6_SplitterBrushKey") as SolidColorBrush;
		VM.PageBackground = TryFindResource("V6_SubThemeBrushKey") as SolidColorBrush;
		VM.ReturnRescueVisible = Visibility.Collapsed;
		VM.HomeVisible = Visibility.Collapsed;
		if (SupportMulti && CurrentPageIndex == PageIndex.RESCUE_FLASH && VM.CurrentView is IAMatchView iAMatchView && iAMatchView.VM.Category == DevCategory.Phone)
		{
			VM.BackVisible = Visibility.Collapsed;
		}
		else if (CurrentPageIndex == PageIndex.RESCUE_FLASH && !(VM.CurrentView is IAMatchView))
		{
			VM.BackVisible = Visibility.Collapsed;
		}
		else
		{
			VM.BackVisible = Visibility.Visible;
		}
	}

	private void OnDevConnecte(object sender, DeviceEx e)
	{
		e.SoftStatusChanged += OnSoftStatusChanged;
	}

	private void OnDisConnecte(object sender, DeviceEx e)
	{
		e.SoftStatusChanged -= OnSoftStatusChanged;
	}

	private void OnSoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		DeviceEx deviceEx = sender as DeviceEx;
		switch (e)
		{
		case DeviceSoftStateEx.Online:
			AutoMatch(deviceEx);
			break;
		case DeviceSoftStateEx.Offline:
		case DeviceSoftStateEx.ManualDisconnect:
			RemoveDevice(deviceEx.Identifer);
			if (CurrentPageIndex == PageIndex.RESCUE_FLASH && VM.CurrentView is IAMatchView iAMatchView && iAMatchView.VM.Category != 0 && !iAMatchView.VM.UcDevice.Locked)
			{
				ChangeView(PageIndex.RESCUE_HOME);
			}
			break;
		}
	}

	private bool CheckPrevMatch(DeviceEx device, bool jumpToMatchView, string categoryStr)
	{
		if (!Plugin.IsRescuePlugin || device == null || CurrentPageIndex == PageIndex.PHONE_MANUAL || device.PhysicalStatus != DevicePhysicalStateEx.Online || device.SoftStatus != DeviceSoftStateEx.Online)
		{
			return false;
		}
		if (!jumpToMatchView && CurrentPageIndex == PageIndex.RESCUE_FLASH)
		{
			return false;
		}
		IAMatchView exists = RescueViews.FirstOrDefault((IAMatchView n) => n.VM.UcDevice.Id == device.Identifer);
		if (exists != null)
		{
			if (!exists.VM.UcDevice.RecipeLocked)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					CloseAllManualPopWindow(device, categoryStr);
					DeviceViewModel deviceViewModel = VM.Devices.FirstOrDefault((DeviceViewModel n) => n.Id == exists.VM.UcDevice.Id);
					if (deviceViewModel != null)
					{
						deviceViewModel.IsSelected = true;
					}
					ChangeMutilDeviceShowType(exists.VM.Category == DevCategory.Phone && SupportMulti);
					CurrentPageIndex = PageIndex.RESCUE_FLASH;
					VM.CurrentView = exists.RescueView ?? (exists as FrameworkElement);
					ChangeFooter(phoneHost: false);
				});
			}
			return false;
		}
		if (device.WorkType == DeviceWorkType.Rescue)
		{
			return false;
		}
		bool flag = VM.Devices.Where((DeviceViewModel n) => !string.IsNullOrEmpty(n.Id)).Count((DeviceViewModel n) => !n.View.VM.UcDevice.RealFlash) > 0;
		if (!jumpToMatchView && Plugin.SupportMulti && categoryStr == "phone" && flag)
		{
			return false;
		}
		int result = IsDevAllowRescue(device).Result;
		if (result != -1 && result != 1)
		{
			IMsgManager.ShowMessage("K1555", "K1478", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
			return false;
		}
		return true;
	}

	public void AutoMatch(DeviceEx device, bool jumpToMatchView = false)
	{
		if (Interlocked.Read(ref autoMatchLock) != 0L)
		{
			return;
		}
		Interlocked.Exchange(ref autoMatchLock, 1L);
		try
		{
			string text = device.Property.Category ?? "phone";
			if (device != null && device.WorkType != DeviceWorkType.Rescue)
			{
				CloseAllManualPopWindow(device, text);
			}
			if (!CheckPrevMatch(device, jumpToMatchView, text))
			{
				return;
			}
			RescueDeviceInfoModel deviceInfo = new RescueDeviceInfoModel
			{
				modelName = device.Property.ModelName,
				imei = device.Property.IMEI1,
				sn = device.Property.SN,
				hwCode = device.Property.HWCode,
				country = device.Property.CountryCode,
				channelId = device.Property.GetPropertyValue("channelid"),
				cid = device.Property.GetPropertyValue("cid"),
				trackId = ((device.ConnectType != ConnectType.Wifi) ? device.Identifer : null)
			};
			LogHelper.LogInstance.Info("\n========================================\nAuto match model name: " + deviceInfo.modelName + "\n========================================");
			WaitTips loadingWindow = null;
			Task.Run(delegate
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					loadingWindow = new WaitTips("K1588");
					HostProxy.HostMaskLayerWrapper.New(loadingWindow, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => loadingWindow.ShowDialog());
				});
			});
			DevCategory category = ((text == "tablet") ? DevCategory.Tablet : ((text == "smart") ? DevCategory.Smart : DevCategory.Phone));
			string matchText = ((category == DevCategory.Phone) ? device.Property.IMEI1 : device.Property.SN);
			Dictionary<string, object> param = null;
			Task<WarrantyInfoBaseModel> task = Task.Run(delegate
			{
				LogHelper.LogInstance.Info("====>>Warranty interface called!");
				WarrantyInfoBaseModel result = LoadWarranty(matchText);
				if (!Plugin.SupportMulti && category == DevCategory.Phone && !string.IsNullOrEmpty(deviceInfo.imei))
				{
					LogHelper.LogInstance.Info("====>>ready for call moto care!");
					Task.Run(delegate
					{
						IMsgManager.CallMotoCare(deviceInfo.imei, result);
					});
				}
				return result;
			});
			var task2 = Task.Run(delegate
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				List<ResourceResponseModel> arr = null;
				param = FlashBusiness.GetAutoMatchParams(device);
				if (param != null)
				{
					FlashBusiness.ConvertDeviceInfo(param["params"] as Dictionary<string, string>, deviceInfo);
					ResponseModel<List<ResourceResponseModel>> responseModel = FlashContext.SingleInstance.service.Request<List<ResourceResponseModel>>(WebServicesContext.RESUCE_AUTOMATCH_GETROM, param);
					stopwatch.Stop();
					arr = responseModel.content;
					BusinessStatus status = BusinessStatus.FALIED;
					if (arr != null && arr.Count > 0)
					{
						status = BusinessStatus.SUCCESS;
						deviceInfo.romMatchId = arr[0].romMatchId;
						deviceInfo.saleModel = arr[0].SalesModel;
						param["romMatchId"] = deviceInfo.romMatchId;
					}
					else if (string.IsNullOrEmpty(device.Property.IMEI1) && !string.IsNullOrEmpty(deviceInfo.trackId))
					{
						LogHelper.LogInstance.Info("Auto match device failed and device IMEI is empty, call interface get IMEI by track_id");
						string value = FlashContext.SingleInstance.service.RequestBase(WebApiUrl.CALL_API_URL, new
						{
							key = "IQS_WARRANTY_URL",
							param = deviceInfo.trackId
						}).content?.ToString();
						if (!string.IsNullOrEmpty(value))
						{
							param["imei"] = value;
							arr = FlashContext.SingleInstance.service.RequestContent<List<ResourceResponseModel>>(WebServicesContext.RESUCE_AUTOMATCH_GETROM, param);
							if (arr != null && arr.Count > 0)
							{
								status = BusinessStatus.SUCCESS;
								deviceInfo.romMatchId = arr[0].romMatchId;
								deviceInfo.saleModel = arr[0].SalesModel;
								param["romMatchId"] = deviceInfo.romMatchId;
							}
						}
					}
					LogHelper.LogInstance.Info("====>>Resouces match interface called!");
					return new
					{
						code = responseModel.code,
						arr = arr,
						time = stopwatch.ElapsedMilliseconds,
						status = status
					};
				}
				stopwatch.Stop();
				param = new Dictionary<string, object>();
				return new
				{
					code = "8080",
					arr = arr,
					time = stopwatch.ElapsedMilliseconds,
					status = BusinessStatus.FALIED
				};
			});
			Task.WaitAll(task, task2);
			Application.Current.Dispatcher.Invoke(delegate
			{
				loadingWindow?.Close();
			});
			var result2 = task2.Result;
			Dictionary<string, object> dictionary = param;
			List<ResourceResponseModel> arr2 = result2.arr;
			dictionary["romMatchId"] = ((arr2 != null && arr2.Count > 0) ? result2.arr[0].romMatchId : null);
			BusinessType businessType = ((category == DevCategory.Tablet) ? BusinessType.RESCUE_AUTO_TABLET_MATCH : ((category == DevCategory.Phone) ? BusinessType.RESCUE_AUTO_PHONE_MATCH : BusinessType.RESCUE_AUTO_SMART_MATCH));
			BusinessData businessData = new BusinessData(businessType, device);
			HostProxy.BehaviorService.Collect(businessType, businessData.Update(result2.time, result2.status, deviceInfo.modelName, param));
			string code = result2.code;
			WarrantyInfoBaseModel result3 = task.Result;
			LogHelper.LogInstance.Info("warranty info:[" + JsonHelper.SerializeObject2Json(result3) + "]");
			switch (code)
			{
			case "0000":
			case "3040":
			{
				ResourceResponseModel resourceResponseModel = result2.arr[0];
				if ((code == "3040" && true != Match3040View.ProcMatch3040(resourceResponseModel, matchText, category, result3)) || (code == "0000" && result2.arr.Count == 1 && (CurrentPageIndex == PageIndex.TABLET_MANUAL || CurrentPageIndex == PageIndex.SMART_MANUAL) && true != IMsgManager.ShowMessage("K0711", "K0968", "K0327", "K0208")))
				{
					break;
				}
				if (result2.arr.Count != 1)
				{
					resourceResponseModel = MultiRomsSelView.SelectOneFormRomArr(result2.arr);
					if (resourceResponseModel == null)
					{
						break;
					}
				}
				if (resourceResponseModel.fastboot && !string.IsNullOrEmpty(deviceInfo.fingerPrint))
				{
					Dictionary<string, string> aparams = new Dictionary<string, string>
					{
						{
							"modelName",
							param["modelName"].ToString()
						},
						{ "romFingerPrint", resourceResponseModel.fingerprint },
						{ "romMatchId", resourceResponseModel.romMatchId }
					};
					if (!new CheckFingerPrintVersion().Check(deviceInfo.fingerPrint, resourceResponseModel.fingerprint, aparams))
					{
						IMsgManager.ShowMessage("K0071", "K1119", "K0327", null, isCloseBtn: false, null, MessageBoxImage.Exclamation);
						break;
					}
				}
				MatchType matchType = ((device.ConnectType != ConnectType.Fastboot) ? ((device.ConnectType == ConnectType.Wifi) ? MatchType.WIFI : MatchType.ADB) : MatchType.FASTBOOT);
				AutoMatchResource data = new AutoMatchResource(device, deviceInfo, resourceResponseModel, new MatchInfo(matchType, param, deviceInfo));
				JumptoRescueView(category, data, result3, null, jumpToMatchView);
				break;
			}
			case "8080":
				IMsgManager.ShowMessage("K0711", "K0113");
				break;
			case "3000":
			case "3030":
				Match3030View.ProcMatch3030(device, deviceInfo, null, result2.arr[0], matchText, category, businessData, result3);
				break;
			case "3010":
				if (category == DevCategory.Phone)
				{
					FastbootDeviceAutoMatchFailed(device.Identifer, device.Property.ModelName, device.Property.IMEI1, result3);
				}
				else
				{
					UsbDeviceAutoMatchFailed(category, (device.ConnectType == ConnectType.Fastboot) ? deviceInfo.modelName : null);
				}
				break;
			default:
				UsbDeviceAutoMatchFailed(category, (device.ConnectType == ConnectType.Fastboot) ? deviceInfo.modelName : null);
				break;
			}
		}
		finally
		{
			Interlocked.Exchange(ref autoMatchLock, 0L);
		}
	}

	private void CloseAllManualPopWindow(DeviceEx device, string categoryStr)
	{
		if (device.ConnectType == ConnectType.Wifi)
		{
			GlobalCmdHelper.Instance.Execute(new
			{
				type = GlobalCmdType.CLOSE_WIFI_TUTORIAL
			});
		}
		if (categoryStr == "phone")
		{
			CloseGifGuideSteps();
			return;
		}
		GlobalCmdHelper.Instance.Execute(new
		{
			type = GlobalCmdType.TABLET_OPEN_USBDEBUG
		});
		GlobalCmdHelper.Instance.Execute(new
		{
			type = GlobalCmdType.AUTO_CLOSE_USB_CONN_TUTORIAL
		});
	}

	public async Task<int> IsDevAllowRescue(DeviceEx dev, bool isMatch = true)
	{
		return await Task.Run(delegate
		{
			int result = -1;
			IAndroidDevice androidDevice = dev?.Property;
			if (androidDevice != null)
			{
				string modelName = androidDevice.ModelName;
				string propertyValue = androidDevice.GetPropertyValue("fdr-allowed");
				string propertyValue2 = androidDevice.GetPropertyValue("securestate");
				string propertyValue3 = androidDevice.GetPropertyValue("cid");
				LogHelper.LogInstance.Info((isMatch ? "auto match" : "rescue flash click") + " device check, modelName: " + modelName + ", fdrallowed: " + propertyValue + ", securestate: " + propertyValue2 + ", cid: " + propertyValue3);
				bool flag = propertyValue?.ToLower() == "no";
				bool flag2 = propertyValue2?.ToLower() == "flashing_locked";
				if (FastbootErrorStatusArr.Contains(propertyValue3))
				{
					result = 6;
				}
				else if (flag)
				{
					result = 1;
				}
				else if (flag2)
				{
					result = 2;
				}
				else if (string.IsNullOrEmpty(modelName) || Regex.IsMatch(modelName, "^[0]+$"))
				{
					result = 5;
				}
			}
			return result;
		});
	}

	private void UsbDeviceAutoMatchFailed(DevCategory category, string modelName)
	{
		if (CurrentPageIndex == PageIndex.PHONE_MANUAL || CurrentPageIndex == PageIndex.SMART_MANUAL || CurrentPageIndex == PageIndex.TABLET_MANUAL)
		{
			return;
		}
		if (category == DevCategory.Phone)
		{
			ChangeView(PageIndex.PHONE_ENTRANCE);
		}
		var anon = Application.Current.Dispatcher.Invoke(delegate
		{
			int model = 0;
			if (category == DevCategory.Phone)
			{
				model = (IsChinaUs(isOnlyChina: true) ? 3 : 2);
			}
			else if (category == DevCategory.Tablet)
			{
				model = 2;
			}
			InvalidView invalidView = new InvalidView(category, model, modelName);
			IMsgManager.ShowMessage(invalidView);
			return new
			{
				isOk = invalidView.Result,
				isManual = invalidView.IsManualModel
			};
		});
		if (anon.isOk == true)
		{
			if (category == DevCategory.Tablet)
			{
				ChangeView(anon.isManual ? PageIndex.TABLET_MANUAL : PageIndex.TABLET_SEARCH);
			}
			else if (category == DevCategory.Smart)
			{
				ChangeView(PageIndex.SMART_MANUAL);
			}
			else
			{
				ChangeView(anon.isManual ? PageIndex.PHONE_MANUAL : PageIndex.PHONE_SEARCH);
			}
		}
	}

	private void FastbootDeviceAutoMatchFailed(string deviceId, string modelName, string imei, object wModel)
	{
		if (CurrentPageIndex == PageIndex.PHONE_MANUAL || CurrentPageIndex == PageIndex.SMART_MANUAL || CurrentPageIndex == PageIndex.TABLET_MANUAL)
		{
			return;
		}
		if (!_FastbootDevUnMatchArr.ContainsKey(deviceId))
		{
			_FastbootDevUnMatchArr.Add(deviceId, 1);
		}
		else if (string.IsNullOrEmpty(modelName))
		{
			_FastbootDevUnMatchArr[deviceId] = 1;
		}
		else
		{
			_FastbootDevUnMatchArr[deviceId]++;
		}
		if (_FastbootDevUnMatchArr[deviceId] > 1)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				Match3010View userUi = new Match3010View(DevCategory.Phone, modelName, imei, wModel);
				IMsgManager.ShowMessage(userUi);
			});
		}
		else
		{
			string fASTBOOT_AUTOMATCH_FAILED_FIRST = FlashStaticResources.FASTBOOT_AUTOMATCH_FAILED_FIRST;
			IMsgManager.ShowMessage("K0711", fASTBOOT_AUTOMATCH_FAILED_FIRST);
		}
	}

	public bool IsChinaUs(bool isOnlyChina = false)
	{
		string currentLanguage = HostProxy.LanguageService.GetCurrentLanguage();
		string twoLetterISORegionName = GlobalFun.GetRegionInfo().TwoLetterISORegionName;
		if (isOnlyChina)
		{
			if (currentLanguage == "zh-CN")
			{
				return twoLetterISORegionName == "CN";
			}
			return false;
		}
		if (!(currentLanguage == "en-US") || !(twoLetterISORegionName == "US"))
		{
			if (currentLanguage == "zh-CN")
			{
				return twoLetterISORegionName == "CN";
			}
			return false;
		}
		return true;
	}

	public BitmapImage GetQrCodeImage(string number)
	{
		return null;
	}
}
