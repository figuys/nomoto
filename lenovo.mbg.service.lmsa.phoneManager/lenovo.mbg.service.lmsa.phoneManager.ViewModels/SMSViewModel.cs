using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.devicemgt;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic;
using lenovo.themes.generic.Component.Progress;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class SMSViewModel : ViewModelBase
{
	private DeviceContactManager _contactMgt = new DeviceContactManager();

	private DeviceSmsManagement _smsMgt = new DeviceSmsManagement();

	private List<SMS> _tempSMSList = new List<SMS>();

	private ContactDetail _ContactDetail;

	private BitmapImage _defaultImage;

	private BitmapImage _avatarImage;

	private int _smsTotalCount;

	private bool _exportButtonEnabled;

	private bool saveButtonEnabled;

	private SMSContactMerged _selectedContact;

	private static SMSViewModel _singleInstance;

	private int zIndex = -1;

	private volatile bool contactFocusedAndFirstPageDataLoaded;

	private bool _isAllSelected;

	private const int SMS_LIMIT_PAGE_COUNT = 10;

	protected SortedList<string, List<SMS>> _CacheSms = new SortedList<string, List<SMS>>();

	private string _inputSmsContent = string.Empty;

	private string _inputSmsContentCharCount = "0";

	private string _inputSmsContactList = string.Empty;

	private string _inputSmsContactListCount = "0";

	private Visibility _smsListBorderVisibility;

	private Visibility _forwardBorderVisibility = Visibility.Collapsed;

	private string inputForwardSMSContent = string.Empty;

	private string _inputForwardSMSContentCharCount = "0";

	private bool _smsSentOutButtonEnable;

	private bool _smsForwardSendOutButtonEnable;

	private ContactAddOrEditViewModel contactAddOrEditViewModel;

	public ObservableCollection<SMS> SMSList { get; set; }

	public ObservableCollection<SMSContactMerged> SMSContactList { get; set; }

	public ObservableCollection<ContactGroup> ContactGroupList { get; set; }

	public ContactDetail ContactDetailNode
	{
		get
		{
			return _ContactDetail;
		}
		set
		{
			_ContactDetail = value;
			OnPropertyChanged("ContactDetailNode");
		}
	}

	public BitmapImage AvatarImage
	{
		get
		{
			return _avatarImage;
		}
		set
		{
			_avatarImage = value;
			OnPropertyChanged("AvatarImage");
		}
	}

	public List<string> DelSMSList { get; set; }

	public List<SMS> ExportSMSList { get; set; }

	public int SMSTotalCount
	{
		get
		{
			return _smsTotalCount;
		}
		set
		{
			_smsTotalCount = value;
			OnPropertyChanged("SMSTotalCount");
		}
	}

	public bool ExportButtonEnabled
	{
		get
		{
			return _exportButtonEnabled;
		}
		set
		{
			if (_exportButtonEnabled != value)
			{
				_exportButtonEnabled = value;
				OnPropertyChanged("ExportButtonEnabled");
			}
		}
	}

	public bool SaveButtonEnabled
	{
		get
		{
			return saveButtonEnabled;
		}
		set
		{
			if (saveButtonEnabled != value)
			{
				saveButtonEnabled = value;
				OnPropertyChanged("SaveButtonEnabled");
			}
		}
	}

	public SMSContactMerged SelectedContact
	{
		get
		{
			return _selectedContact;
		}
		set
		{
			if (_selectedContact != value)
			{
				_selectedContact = value;
				OnPropertyChanged("SelectedContact");
			}
		}
	}

	public static SMSViewModel SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new SMSViewModel();
			}
			return _singleInstance;
		}
	}

	public int ZIndex
	{
		get
		{
			return zIndex;
		}
		set
		{
			zIndex = value;
			OnPropertyChanged("ZIndex");
		}
	}

	public ReplayCommand RefreshCommand { get; set; }

	public ReplayCommand AddContactCommand { get; set; }

	public bool ContactFocusedAndFirstPageDataLoaded
	{
		get
		{
			return contactFocusedAndFirstPageDataLoaded;
		}
		set
		{
			contactFocusedAndFirstPageDataLoaded = value;
		}
	}

	public ReplayCommand SearchCommand { get; set; }

	public ReplayCommand ExportCommand { get; set; }

	public ReplayCommand ImportCommand { get; set; }

	public ReplayCommand DelContactCommand { get; set; }

	public ReplayCommand SendOutSmsCommand { get; set; }

	public ReplayCommand ForwardSmsSendOutCommand { get; set; }

	public ReplayCommand ForwardSmsCancelCommand { get; set; }

	public bool IsAllSelected
	{
		get
		{
			return _isAllSelected;
		}
		set
		{
			if (_isAllSelected != value)
			{
				_isAllSelected = value;
				OnPropertyChanged("IsAllSelected");
			}
		}
	}

	private int CurrentPageIndex
	{
		get
		{
			if (SMSList == null || SMSList.Count == 0)
			{
				return 0;
			}
			return (int)Math.Ceiling((double)SMSList.Count * 1.0 / 10.0);
		}
	}

	public string InputSmsContent
	{
		get
		{
			return _inputSmsContent;
		}
		set
		{
			if (!(_inputSmsContent == value))
			{
				_inputSmsContent = value;
				OnPropertyChanged("InputSmsContent");
			}
		}
	}

	public string InputSmsContentCharCount
	{
		get
		{
			return _inputSmsContentCharCount;
		}
		set
		{
			if (!(_inputSmsContentCharCount == value))
			{
				_inputSmsContentCharCount = value;
				SmsSentOutButtonEnable = !"0".Equals(value);
				OnPropertyChanged("InputSmsContentCharCount");
			}
		}
	}

	public string InputSmsContactList
	{
		get
		{
			return _inputSmsContactList;
		}
		set
		{
			if (!(_inputSmsContactList == value))
			{
				_inputSmsContactList = value;
				OnPropertyChanged("InputSmsContactList");
			}
		}
	}

	public string InputSmsContactListCount
	{
		get
		{
			return _inputSmsContactListCount;
		}
		set
		{
			if (!(_inputSmsContactListCount == value))
			{
				_inputSmsContactListCount = value;
				ResetSmsForwardSendOutButtnEnable();
				OnPropertyChanged("InputSmsContactListCount");
			}
		}
	}

	public Visibility SMSListBorderVisibility
	{
		get
		{
			return _smsListBorderVisibility;
		}
		set
		{
			if (_smsListBorderVisibility != value)
			{
				_smsListBorderVisibility = value;
				OnPropertyChanged("SMSListBorderVisibility");
			}
		}
	}

	public Visibility ForwardBorderVisibility
	{
		get
		{
			return _forwardBorderVisibility;
		}
		set
		{
			if (_forwardBorderVisibility != value)
			{
				_forwardBorderVisibility = value;
				OnPropertyChanged("ForwardBorderVisibility");
			}
		}
	}

	public string InputForwardSMSContent
	{
		get
		{
			return inputForwardSMSContent;
		}
		set
		{
			if (!(inputForwardSMSContent == value))
			{
				inputForwardSMSContent = value;
				OnPropertyChanged("InputForwardSMSContent");
			}
		}
	}

	public string InputForwardSMSContentCharCount
	{
		get
		{
			return _inputForwardSMSContentCharCount;
		}
		set
		{
			if (!(_inputForwardSMSContentCharCount == value))
			{
				_inputForwardSMSContentCharCount = value;
				ResetSmsForwardSendOutButtnEnable();
				OnPropertyChanged("InputForwardSMSContentCharCount");
			}
		}
	}

	public bool SmsSentOutButtonEnable
	{
		get
		{
			return _smsSentOutButtonEnable;
		}
		set
		{
			if (_smsSentOutButtonEnable != value)
			{
				_smsSentOutButtonEnable = value;
				OnPropertyChanged("SmsSentOutButtonEnable");
			}
		}
	}

	public bool SmsForwardSendOutButtonEnable
	{
		get
		{
			return _smsForwardSendOutButtonEnable;
		}
		set
		{
			if (_smsForwardSendOutButtonEnable != value)
			{
				_smsForwardSendOutButtonEnable = value;
				OnPropertyChanged("SmsForwardSendOutButtonEnable");
			}
		}
	}

	public ContactAddOrEditViewModel ContactAddOrEditViewModel
	{
		get
		{
			return contactAddOrEditViewModel;
		}
		private set
		{
			if (contactAddOrEditViewModel != value)
			{
				contactAddOrEditViewModel = value;
				OnPropertyChanged("ContactAddOrEditViewModel");
			}
		}
	}

	public event EventHandler ResetContactUIHandler;

	public event EventHandler RefreshHandler;

	public event EventHandler UpdateAlphaFilterHandler;

	private SMSViewModel()
	{
		SMSContactList = new ObservableCollection<SMSContactMerged>();
		SMSList = new ObservableCollection<SMS>();
		ContactGroupList = new ObservableCollection<ContactGroup>();
		ContactDetailNode = new ContactDetail();
		SelectedContact = null;
		_defaultImage = new BitmapImage(new Uri("/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/avatar.png", UriKind.Relative));
		AvatarImage = _defaultImage;
		RefreshCommand = new ReplayCommand(RefreshCommandHandler);
		ExportCommand = new ReplayCommand(ExportCommandHandler);
		ImportCommand = new ReplayCommand(ImportCommandHandler);
		DelContactCommand = new ReplayCommand(DelContactCommandHandler);
		AddContactCommand = new ReplayCommand(AddContactCommandHandler);
		SearchCommand = new ReplayCommand(SearchCommandHandler);
		SendOutSmsCommand = new ReplayCommand(SendOutSmsCommandHandler);
		ForwardSmsSendOutCommand = new ReplayCommand(ForwardSmsSendOutCommandHandler);
		ForwardSmsCancelCommand = new ReplayCommand(ForwardSmsCancelCommandHandler);
		ContactAddOrEditViewModel = new ContactAddOrEditViewModel(null);
	}

	private void ResetContactDetail()
	{
		this.ResetContactUIHandler?.Invoke(null, null);
		AvatarImage = _defaultImage;
		ContactDetailNode = new ContactDetail();
		ZIndex = -1;
	}

	public override void LoadData()
	{
		base.LoadData();
		Task.Run(delegate
		{
			inputForwardSMSContent = string.Empty;
			InputSmsContent = string.Empty;
			InputSmsContactList = string.Empty;
			RefreshEx();
			ShowSMSListPanel();
		});
	}

	private void DeleteSmsCommandHandler(object parameter)
	{
		TcpAndroidDevice currentDevice = HostProxy.deviceManager.MasterDevice as TcpAndroidDevice;
		if (currentDevice == null)
		{
			return;
		}
		_ = currentDevice.Property.ApiLevel;
		SMS sms = parameter as SMS;
		if (sms == null || string.IsNullOrEmpty(sms._id) || string.IsNullOrEmpty(sms._id))
		{
			return;
		}
		LenovoPopupWindow win = new OkCancelWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0585", "K0586", "K0208", "K0583", new BitmapImage(new Uri("Pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;Component/Assets/Images/PicPopup/delete.png")));
		HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			win.ShowDialog();
		});
		if (!win.WindowModel.GetViewModel<OKCancelViewModel>().IsOKResult)
		{
			return;
		}
		HostProxy.PermissionService.BeginConfirmAppIsReady(HostProxy.deviceManager.MasterDevice, "SMS", null, delegate(bool? isReady)
		{
			if (isReady.HasValue && isReady.Value)
			{
				HostProxy.AsyncCommonProgressLoader.Progress(Context.MessageBox, delegate(IAsyncTaskContext context, CommonProgressWindowViewModel viewModel)
				{
					Action<NotifyTypes, object> exectingNotifyHandler = (Action<NotifyTypes, object>)context.ObjectState;
					exectingNotifyHandler(NotifyTypes.INITILIZE, new List<object>
					{
						new List<object>
						{
							ResourcesHelper.StringResources.SingleInstance.SMS_CONTENT,
							1
						},
						new List<ProgressPramater>
						{
							new ProgressPramater
							{
								Message = ResourcesHelper.StringResources.SingleInstance.SMS_DELETE_MESSAGE
							}
						}
					});
					if (!context.IsCancelCommandRequested)
					{
						_smsMgt.DoProcessWithChangeSMSDefault(currentDevice, delegate
						{
							if (_smsMgt.Delete(new List<string> { sms._id }))
							{
								exectingNotifyHandler(NotifyTypes.PERCENT, 1);
							}
						});
						if (!context.IsCancelCommandRequested)
						{
							exectingNotifyHandler(NotifyTypes.SUCCESS, new List<object>
							{
								new List<ProgressPramater>
								{
									new ProgressPramater
									{
										Message = ResourcesHelper.StringResources.SingleInstance.DELETE_SUCCESS_MESSAGE
									}
								},
								true
							});
						}
					}
				});
			}
		});
	}

	private void ForwardPanleCommandHandler(object parameter)
	{
		SMS sMS = parameter as SMS;
		ShowForwardPanel();
		InputForwardSMSContent = sMS.body;
		InputSmsContactList = string.Empty;
	}

	public void SetAvatarImage(string filePath)
	{
		if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
		{
			AvatarImage = ImageHandleHelper.LoadBitmap(filePath);
			ContactDetailNode.AvatarPath = filePath;
		}
	}

	private void RefreshCommandHandler(object prameter)
	{
		RefreshEx();
		ShowSMSListPanel();
	}

	private void AddContactCommandHandler(object prameter)
	{
		if (SMSContactList == null)
		{
			return;
		}
		SMSContactMerged sMSContactMerged = SMSContactList.Where((SMSContactMerged m) => m.isSelected).FirstOrDefault();
		if (sMSContactMerged == null)
		{
			return;
		}
		ContactAddOrEditViewModel.Clear();
		ContactAddOrEditViewModel.ContactAddPanelVisibility = Visibility.Visible;
		RawContactAddOrEditViewModel rawContactAddOrEditViewModel = new RawContactAddOrEditViewModel();
		ObservableCollection<MultiCheckBoxItemViewModel> observableCollection = new ObservableCollection<MultiCheckBoxItemViewModel>();
		List<ContactGroup> contactGroup = new DeviceContactManager().GetContactGroup();
		if (contactGroup != null)
		{
			foreach (ContactGroup item in contactGroup)
			{
				if (!string.IsNullOrEmpty(item.Id) && !"0".Equals(item.Id))
				{
					observableCollection.Add(new MultiCheckBoxItemViewModel
					{
						IsChecked = false,
						DisplayContent = item.Name,
						Tag = new ContactGroupItemViewModel
						{
							Id = item.Id,
							Name = item.Name
						}
					});
				}
			}
			rawContactAddOrEditViewModel.PhoneGroupList = observableCollection;
		}
		rawContactAddOrEditViewModel.PhoneNumberList = new ObservableCollection<ContactItemPhoneViewModel>(new List<ContactItemPhoneViewModel>
		{
			new ContactItemPhoneViewModel
			{
				Type = DetailType.Home,
				Content = sMSContactMerged.PhoneNumber
			},
			new ContactItemPhoneViewModel
			{
				Type = DetailType.Home,
				Content = string.Empty
			},
			new ContactItemPhoneViewModel
			{
				Type = DetailType.Home,
				Content = string.Empty
			}
		});
		ContactAddOrEditViewModel.RawContactAddOrEditViewModelList = new ObservableCollection<RawContactAddOrEditViewModel> { rawContactAddOrEditViewModel };
		ContactAddOrEditViewModel.EditMode = EditMode.Add;
		ContactAddOrEditViewModel.Title = "K0575";
		ContactAddOrEditViewModel.OnContactSaved += delegate
		{
			AsyncDataLoader.BeginLoading(delegate
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					RefreshEx();
				});
			}, ViewContext.SingleInstance.MainViewModel);
		};
	}

	private void SearchCommandHandler(object prameter)
	{
		ContactFocusedAndFirstPageDataLoaded = false;
		if (prameter == null || SelectedContact == null || !_CacheSms.ContainsKey(SelectedContact.PhoneNumber))
		{
			return;
		}
		string searchText = prameter.ToString().ToLower();
		List<SMS> list = _CacheSms[SelectedContact.PhoneNumber];
		SMSList.Clear();
		if (!string.IsNullOrEmpty(searchText))
		{
			list = list.Where((SMS n) => n.body.ToLower().Contains(searchText)).ToList();
		}
		list.ForEach(delegate(SMS n)
		{
			SMSList.Add(n);
		});
	}

	private void ExportCommandHandler(object prameter)
	{
		if (SMSContactList == null || SMSContactList.Count == 0)
		{
			return;
		}
		List<SMSContactMerged> selectedContactList = SMSContactList.Where((SMSContactMerged m) => m.isSelected).ToList();
		if (selectedContactList.Count == 0)
		{
			return;
		}
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
		{
			return;
		}
		string dir = folderBrowserDialog.SelectedPath.Trim();
		new ImportAndExportWrapper().ExportString(BusinessType.SMS_EXPORT, 21, ResourcesHelper.StringResources.SingleInstance.SMS_EXPORT_MESSAGE, "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}", BackupRestoreStaticResources.SingleInstance.SMS, delegate(IAsyncTaskContext taskContext)
		{
			List<string> addressList = selectedContactList.Select((SMSContactMerged m) => m.PhoneNumber).ToList();
			return new DeviceSmsManagement().getIdListByAddress(addressList, taskContext);
		}, delegate(Action<Action<string>> trigger)
		{
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(Path.Combine(dir, "SMS" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".sms"));
			bool isFirst = true;
			StreamWriter writer = new StreamWriter(fileInfo.Create());
			try
			{
				writer.WriteLine("[");
				trigger(delegate(string str)
				{
					writer.WriteLine((isFirst ? "" : ",") + str);
					isFirst = false;
				});
				writer.WriteLine("]");
			}
			finally
			{
				if (writer != null)
				{
					((IDisposable)writer).Dispose();
				}
			}
		});
	}

	private void ImportCommandHandler(object prameter)
	{
		if (!(HostProxy.deviceManager.MasterDevice is TcpAndroidDevice tcpAndroidDevice))
		{
			return;
		}
		_ = tcpAndroidDevice.Property.ApiLevel;
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Title = "K0496";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			openFileDialog.Title = HostProxy.LanguageService.Translate("K0496");
		}
		openFileDialog.Filter = "SMS|*.sms";
		openFileDialog.Multiselect = false;
		openFileDialog.FileName = string.Empty;
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() != true)
		{
			return;
		}
		List<SMS> smsList = null;
		try
		{
			string value = File.ReadAllText(openFileDialog.FileName);
			smsList = JsonConvert.DeserializeObject<List<SMS>>(value);
			_ = smsList.Count;
		}
		catch (Exception)
		{
			smsList = null;
		}
		if (smsList == null || tcpAndroidDevice == null)
		{
			return;
		}
		_smsMgt.DoProcessWithChangeSMSDefault(tcpAndroidDevice, delegate
		{
			new ImportAndExportWrapper().ImportString(createDataHandler: delegate
			{
				List<string> list = null;
				try
				{
					return smsList.Select((SMS m) => JsonConvert.SerializeObject(m)).ToList();
				}
				catch (Exception)
				{
					return (List<string>)null;
				}
			}, businessType: BusinessType.SMS_IMPORT, requestServiceCode: 21, progressTitle: "Importing messages...", resourceType: "{C6E1253A-3ED6-41EF-B37B-454EA43CF0A4}", resourceTypeName: BackupRestoreStaticResources.SingleInstance.SMS);
		});
	}

	private void DelContactCommandHandler(object prameter)
	{
		if (!MessageBoxHelper.DeleteConfirmMessagebox(ResourcesHelper.StringResources.SingleInstance.CONTACT_DELETE_TITLE, ResourcesHelper.StringResources.SingleInstance.CONTACT_DELETE_SMS_CONTENT) || SMSContactList == null || SMSContactList.Count == 0)
		{
			return;
		}
		List<SMSContactMerged> selectedContactList = SMSContactList.Where((SMSContactMerged m) => m.isSelected).ToList();
		if (selectedContactList.Count == 0)
		{
			return;
		}
		int total = selectedContactList.Sum((SMSContactMerged m) => m.total);
		new DeviceSmsManagement();
		SMSAppletSettingTips.DoProcess(delegate(Action showFirstTips, Action showSecondTips)
		{
			showFirstTips();
			HostProxy.AsyncCommonProgressLoader.Progress(Context.MessageBox, delegate(IAsyncTaskContext context, CommonProgressWindowViewModel viewModel)
			{
				Action<NotifyTypes, object> exectingNotifyHandler = (Action<NotifyTypes, object>)context.ObjectState;
				exectingNotifyHandler(NotifyTypes.INITILIZE, new List<object>
				{
					new List<object>
					{
						ResourcesHelper.StringResources.SingleInstance.SMS_CONTENT,
						total
					},
					new List<ProgressPramater>
					{
						new ProgressPramater
						{
							Message = ResourcesHelper.StringResources.SingleInstance.SMS_DELETE_MESSAGE
						}
					}
				});
				try
				{
					_smsMgt.Delete(context, selectedContactList, delegate(bool rs)
					{
						if (rs)
						{
							exectingNotifyHandler(NotifyTypes.PERCENT, 1);
						}
					});
				}
				catch
				{
				}
				finally
				{
					showSecondTips();
				}
				exectingNotifyHandler(NotifyTypes.SUCCESS, new List<object>
				{
					new List<ProgressPramater>
					{
						new ProgressPramater
						{
							Message = ResourcesHelper.StringResources.SingleInstance.DELETE_SUCCESS_MESSAGE
						}
					},
					true
				});
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					RefreshEx();
					ShowSMSListPanel();
				});
			});
		});
	}

	private void SendOutSmsCommandHandler(object parameter)
	{
		System.Windows.Controls.ListView listView = (System.Windows.Controls.ListView)parameter;
		if (SelectedContact == null)
		{
			return;
		}
		SendSmsHandler(InputSmsContent, new List<string> { SelectedContact.PhoneNumber }, delegate
		{
			ObservableCollection<SMS> smsList = SMSList;
			if (smsList != null)
			{
				_ = SelectedContact.PhoneNumber;
				if (smsList.LastOrDefault() != null)
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						LoadPagingData();
						SMS sMS = smsList.LastOrDefault();
						if (sMS != null)
						{
							InputSmsContent = string.Empty;
							listView.ScrollIntoView(sMS);
						}
					});
				}
			}
		});
	}

	private void ForwardSmsSendOutCommandHandler(object parameter)
	{
		if (string.IsNullOrEmpty(InputSmsContactList) || string.IsNullOrEmpty(inputForwardSMSContent))
		{
			return;
		}
		System.Windows.Controls.ListView listView = (System.Windows.Controls.ListView)parameter;
		List<string> contacts = InputSmsContactList.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		SendSmsHandler(inputForwardSMSContent, contacts, delegate
		{
			ObservableCollection<SMS> smsList = SMSList;
			if (smsList != null && SelectedContact != null)
			{
				_ = SelectedContact.PhoneNumber;
				if (smsList.LastOrDefault() != null)
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						LoadPagingData();
						SMS sMS = smsList.LastOrDefault();
						if (sMS != null)
						{
							inputForwardSMSContent = string.Empty;
							InputSmsContactList = string.Empty;
							ShowSMSListPanel();
							listView.ScrollIntoView(sMS);
						}
					});
				}
			}
		});
	}

	private void ForwardSmsCancelCommandHandler(object parameter)
	{
		ShowSMSListPanel();
	}

	private void SendSmsHandler(string smsContent, List<string> contacts, Action callbackAction)
	{
		DeviceSmsManagement mamager = new DeviceSmsManagement();
		HostProxy.AsyncCommonProgressLoader.Progress(Context.MessageBox, delegate(IAsyncTaskContext context, CommonProgressWindowViewModel viewModel)
		{
			try
			{
				Action<NotifyTypes, object> exectingNotifyHandler = (Action<NotifyTypes, object>)context.ObjectState;
				exectingNotifyHandler(NotifyTypes.INITILIZE, new List<object>
				{
					new List<object>
					{
						ResourcesHelper.StringResources.SingleInstance.SMS_CONTENT,
						contacts.Count
					},
					new List<ProgressPramater>
					{
						new ProgressPramater
						{
							Message = ResourcesHelper.StringResources.SingleInstance.SMS_SENDING_MESSAGE
						}
					}
				});
				mamager.SendSms(smsContent, contacts, delegate(bool sendSuccess)
				{
					if (sendSuccess)
					{
						exectingNotifyHandler(NotifyTypes.PERCENT, 1);
					}
				});
				if (!context.IsCancelCommandRequested)
				{
					exectingNotifyHandler(NotifyTypes.SUCCESS, new List<object>
					{
						new List<ProgressPramater>
						{
							new ProgressPramater
							{
								Message = ResourcesHelper.StringResources.SingleInstance.SMS_SEND_FINISHED_MESSAGE
							}
						},
						true
					});
				}
			}
			finally
			{
				callbackAction?.Invoke();
			}
		});
	}

	public override void Reset()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			base.Reset();
			SelectedContact = null;
			SMSContactList.Clear();
			SMSList.Clear();
			ContactGroupList.Clear();
			ResetContactDetail();
			SMSTotalCount = 0;
			_CacheSms.Clear();
			IsAllSelected = false;
		});
	}

	private void RefreshEx()
	{
		Reset();
		List<SMSContactMerged> smsContactList = new List<SMSContactMerged>();
		int totalSmsCount = 0;
		if (!_smsMgt.GetSmsInfoEx(out smsContactList, out totalSmsCount) || smsContactList == null || smsContactList.Count == 0)
		{
			ExportButtonEnabled = false;
			return;
		}
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			smsContactList.ForEach(delegate(SMSContactMerged m)
			{
				SMSContactList.Add(m);
			});
		});
		if (SelectedContact == null || !smsContactList.Exists((SMSContactMerged s) => s.PhoneNumber.Equals(SelectedContact.PhoneNumber)))
		{
			SelectedContact = smsContactList.First();
		}
		LoadPagingData();
		base.LoadData();
		this.RefreshHandler?.Invoke(null, null);
		this.UpdateAlphaFilterHandler?.Invoke(null, null);
	}

	public int LoadPagingData()
	{
		int result = 0;
		if (SelectedContact == null || string.IsNullOrEmpty(SelectedContact.PhoneNumber))
		{
			return 0;
		}
		string phoneNumber = SelectedContact.PhoneNumber;
		List<SMS> smsList = new List<SMS>();
		if (_smsMgt.GetSmsByAddressEx(new AsyncTaskContext(null), SelectedContact, CurrentPageIndex, 10, "desc", out smsList) && smsList != null)
		{
			if (_CacheSms.ContainsKey(phoneNumber))
			{
				_CacheSms[phoneNumber].InsertRange(0, smsList.OrderByDescending((SMS m) => m.OrderBySequence));
			}
			else
			{
				_CacheSms.Add(phoneNumber, smsList.OrderByDescending((SMS m) => m.OrderBySequence).ToList());
			}
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				smsList.ForEach(delegate(SMS s)
				{
					SMSList.Insert(0, s);
					s.DeleteSmsCommand = new ReplayCommand(DeleteSmsCommandHandler);
					s.ForwardSmsCommand = new ReplayCommand(ForwardPanleCommandHandler);
				});
			});
			result = smsList.Count;
		}
		return result;
	}

	public void ContactSelectionChangedHandler(SMSContactMerged selectItem, Action callback)
	{
		if (selectItem == null)
		{
			callback?.Invoke();
			return;
		}
		AddAddressIntoForwardList(selectItem.PhoneNumber);
		SelectedContact = selectItem;
		SMSList.Clear();
		_tempSMSList.Clear();
		if (_CacheSms.ContainsKey(SelectedContact.PhoneNumber))
		{
			_CacheSms[SelectedContact.PhoneNumber].ForEach(delegate(SMS n)
			{
				SMSList.Add(n);
			});
		}
		else
		{
			LoadPagingData();
		}
		callback?.Invoke();
		this.RefreshHandler?.Invoke(null, null);
	}

	private void AddAddressIntoForwardList(string address)
	{
		if (!string.IsNullOrEmpty(address))
		{
			InputSmsContactList += $"{address};";
		}
	}

	private void ResetSmsForwardSendOutButtnEnable()
	{
		SmsForwardSendOutButtonEnable = !"0".Equals(InputForwardSMSContentCharCount) && !"0".Equals(InputSmsContactListCount);
	}

	private void ShowSMSListPanel()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			ForwardBorderVisibility = Visibility.Collapsed;
			if (SMSContactList != null && SMSContactList.Count > 0)
			{
				SMSListBorderVisibility = Visibility.Visible;
			}
			else
			{
				SMSListBorderVisibility = Visibility.Collapsed;
			}
		});
	}

	private void ShowForwardPanel()
	{
		ForwardBorderVisibility = Visibility.Visible;
		SMSListBorderVisibility = Visibility.Collapsed;
	}
}
