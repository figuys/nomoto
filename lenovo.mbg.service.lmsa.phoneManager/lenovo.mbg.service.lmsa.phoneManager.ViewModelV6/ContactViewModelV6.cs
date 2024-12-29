using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.common.ImportExport;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic;
using lenovo.themes.generic.Interactivity.Ex;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class ContactViewModelV6 : ViewModelBase
{
	internal class ContactDataCache
	{
		private const string CONTACT_LIST_KEY = "cl";

		private const string CONTACT_GROUP_LIST_KEY = "cgl";

		private Cache mCache;

		private static ContactDataCache single;

		public static ContactDataCache Single
		{
			get
			{
				if (single == null)
				{
					single = new ContactDataCache();
				}
				return single;
			}
		}

		public int ContactsCount { get; private set; }

		private ContactDataCache()
		{
			mCache = new Cache();
			mCache.Add("cl", new Cache.CacheItem<List<ContactItemViewModelV6>>
			{
				Data = new List<ContactItemViewModelV6>()
			});
			mCache.Add("cgl", new Cache.CacheItem<List<ContactGroupItemViewModelV6>>
			{
				Data = new List<ContactGroupItemViewModelV6>()
			});
		}

		public Tuple<List<ContactItemViewModelV6>, List<ContactItemViewModelV6>> FillContactId(List<string> contactIdList)
		{
			Cache.CacheItem<List<ContactItemViewModelV6>> cacheItem = mCache.Get<List<ContactItemViewModelV6>>("cl");
			int newVersion = ++cacheItem.Version;
			List<ContactItemViewModelV6> data = cacheItem.Data;
			List<ContactItemViewModelV6> list = new List<ContactItemViewModelV6>();
			bool flag = false;
			ContactItemViewModelV6 contactItemViewModelV = null;
			foreach (string contactId in contactIdList)
			{
				flag = false;
				for (int i = 0; i < data.Count; i++)
				{
					contactItemViewModelV = data[i];
					if (contactId.Equals(contactItemViewModelV.Id))
					{
						contactItemViewModelV.DataVersion = newVersion;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ContactItemViewModelV6 item = new ContactItemViewModelV6
					{
						Id = contactId,
						DataVersion = newVersion
					};
					list.Add(item);
					data.Add(item);
				}
			}
			List<ContactItemViewModelV6> needRemovedList = data.Where((ContactItemViewModelV6 m) => m.DataVersion != newVersion).ToList();
			data.RemoveAll((ContactItemViewModelV6 m) => needRemovedList.Contains(m));
			ContactsCount = data.Count;
			return new Tuple<List<ContactItemViewModelV6>, List<ContactItemViewModelV6>>(list, needRemovedList);
		}

		public void FillContactProperty(List<Contact> contractList)
		{
			List<ContactItemViewModelV6> data = mCache.Get<List<ContactItemViewModelV6>>("cl").Data;
			foreach (Contact contract in contractList)
			{
				foreach (ContactItemViewModelV6 item in data)
				{
					if (!contract.Id.Equals(item.Id))
					{
						continue;
					}
					item.Name = contract.Name;
					item.Number = contract.Number;
					item.Acronym = contract.Acronym;
					item.SortKey = contract.SortKey;
					item.Type = contract.Type;
					item.PropertyDataUpdated = true;
					if (contract.NumberList == null || contract.NumberList.Count <= 0)
					{
						break;
					}
					List<ContactItemPhoneViewModelV6> list = new List<ContactItemPhoneViewModelV6>();
					foreach (Phone number in contract.NumberList)
					{
						list.Add(new ContactItemPhoneViewModelV6
						{
							Content = number.PhoneNumber,
							Type = number.PhoneType
						});
					}
					item.NumberList = list;
					break;
				}
			}
		}

		public List<ContactItemViewModelV6> GetAllContacts()
		{
			return mCache.Get<List<ContactItemViewModelV6>>("cl").Data;
		}

		public Tuple<List<ContactGroupItemViewModelV6>, List<ContactGroupItemViewModelV6>> FillGroup(List<ContactGroup> contractGroupList)
		{
			Cache.CacheItem<List<ContactGroupItemViewModelV6>> cacheItem = mCache.Get<List<ContactGroupItemViewModelV6>>("cgl");
			int dataVersion = ++cacheItem.Version;
			List<ContactGroupItemViewModelV6> data = cacheItem.Data;
			List<ContactGroupItemViewModelV6> list = new List<ContactGroupItemViewModelV6>();
			bool flag = false;
			ContactGroupItemViewModelV6 contactGroupItemViewModelV = null;
			foreach (ContactGroup contractGroup in contractGroupList)
			{
				flag = false;
				for (int i = 0; i < data.Count; i++)
				{
					contactGroupItemViewModelV = data[i];
					if (contractGroup.Id.Equals(contactGroupItemViewModelV.Id))
					{
						contactGroupItemViewModelV.DataVersion = dataVersion;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					contactGroupItemViewModelV = new ContactGroupItemViewModelV6
					{
						Id = contractGroup.Id,
						Name = contractGroup.Name,
						DataVersion = dataVersion
					};
					data.Add(contactGroupItemViewModelV);
					list.Add(contactGroupItemViewModelV);
				}
			}
			List<ContactGroupItemViewModelV6> needRemovedList = data.Where((ContactGroupItemViewModelV6 m) => m.DataVersion != dataVersion).ToList();
			data.RemoveAll((ContactGroupItemViewModelV6 m) => needRemovedList.Contains(m));
			return new Tuple<List<ContactGroupItemViewModelV6>, List<ContactGroupItemViewModelV6>>(list, needRemovedList);
		}

		public List<ContactGroupItemViewModelV6> GetAllContactsGroup()
		{
			return mCache.Get<List<ContactGroupItemViewModelV6>>("cgl").Data;
		}

		public void Clear()
		{
			Cache.CacheItem<List<ContactItemViewModelV6>> cacheItem = mCache.Get<List<ContactItemViewModelV6>>("cl");
			cacheItem.Version = 0;
			cacheItem.Data.Clear();
			Cache.CacheItem<List<ContactGroupItemViewModelV6>> cacheItem2 = mCache.Get<List<ContactGroupItemViewModelV6>>("cgl");
			cacheItem2.Version = 0;
			cacheItem2.Data.Clear();
		}
	}

	private class Cache : IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		public class CacheItem<T>
		{
			public int Version { get; set; }

			public T Data { get; set; }
		}

		private Dictionary<string, object> storage = new Dictionary<string, object>();

		public CacheItem<T> Get<T>(string key)
		{
			if (storage.Keys.Contains(key))
			{
				return (CacheItem<T>)storage[key];
			}
			return null;
		}

		public bool Add<T>(string key, CacheItem<T> item)
		{
			if (storage.Keys.Contains(key))
			{
				return false;
			}
			storage.Add(key, item);
			return true;
		}

		public void Clear()
		{
			storage.Clear();
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<string, object>>)storage).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<string, object>>)storage).GetEnumerator();
		}
	}

	private DeviceContactManager mgt = new DeviceContactManager();

	private bool _isAllContactSelected;

	private ContactDetailViewV6 _contactDetailViewV6;

	private bool _isAllCallLogSelected;

	protected SdCardViewModel sdCarVm;

	private int contactCnt;

	private int selectContactCount;

	private int callLogCnt;

	private ContactGroupItemViewModelV6 _selectedContactGroup;

	private static ContactViewModelV6 _singleInstance;

	private bool contactCanSort;

	private ContactItemViewModelV6 selectedContact;

	private bool contactExportToolBtnEnable;

	private bool contactDeleteToolBtnEnable;

	private bool contactRefreshToolBtnEnable;

	private volatile int mContactDisplayIdexInList;

	public const int CONTACTS_PAGE_COUNT = 20;

	private ContactAddOrEditViewModelV6 contactAddOrEditViewModel;

	private ContactDetailViewModelV6 contactDetailViewModel;

	private bool callLogCanSort;

	public bool IsAllContactSelected
	{
		get
		{
			return _isAllContactSelected;
		}
		set
		{
			_isAllContactSelected = value;
			OnPropertyChanged("IsAllContactSelected");
		}
	}

	public bool IsAllCallLogSelected
	{
		get
		{
			return _isAllCallLogSelected;
		}
		set
		{
			_isAllCallLogSelected = value;
			OnPropertyChanged("IsAllCallLogSelected");
		}
	}

	public SdCardViewModel SdCarVm
	{
		get
		{
			return sdCarVm;
		}
		set
		{
			sdCarVm = value;
			OnPropertyChanged("SdCarVm");
		}
	}

	public ObservableCollection<ContactItemViewModelV6> ContactList { get; set; }

	public ObservableCollection<ContactGroupItemViewModelV6> ContactGroupList { get; set; }

	public ContactGroupItemViewModelV6 SelectedContactGroup
	{
		get
		{
			return _selectedContactGroup;
		}
		set
		{
			if (_selectedContactGroup != value)
			{
				_selectedContactGroup = value;
				ContactDataLoading(delegate
				{
					LoadContactIdListByGroup(_selectedContactGroup, updateDisplayList: true);
				});
				OnPropertyChanged("SelectedContactGroup");
			}
		}
	}

	public int ContactTotalCount
	{
		get
		{
			return contactCnt;
		}
		set
		{
			contactCnt = value;
			OnPropertyChanged("ContactTotalCount");
		}
	}

	public int CallLogTotalCount
	{
		get
		{
			return callLogCnt;
		}
		set
		{
			callLogCnt = value;
			OnPropertyChanged("CallLogTotalCount");
		}
	}

	public int SelectContactCount
	{
		get
		{
			return selectContactCount;
		}
		set
		{
			selectContactCount = value;
			OnPropertyChanged("SelectContactCount");
		}
	}

	public string CallLogTotalCountLabel => "K0509";

	public static ContactViewModelV6 SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new ContactViewModelV6();
			}
			return _singleInstance;
		}
	}

	public ReplayCommand ContactSearchCommand { get; set; }

	public ReplayCommand AddContactCommand { get; set; }

	public ReplayCommand AddGroupCommand { get; set; }

	public ReplayCommand ExportContactCommand { get; set; }

	public ReplayCommand ImportContactCommand { get; set; }

	public ReplayCommand ImportCallCommand { get; set; }

	public ReplayCommand DelContactCommand { get; set; }

	public ReplayCommand RefreshAllCommand { get; set; }

	public bool ContactCanSort
	{
		get
		{
			return contactCanSort;
		}
		set
		{
			if (contactCanSort != value)
			{
				contactCanSort = value;
				OnPropertyChanged("ContactCanSort");
			}
		}
	}

	public ContactItemViewModelV6 SelectedContact
	{
		get
		{
			return selectedContact;
		}
		set
		{
			if (selectedContact != value)
			{
				selectedContact = value;
				OnPropertyChanged("SelectedContact");
			}
		}
	}

	public bool ContactExportToolBtnEnable
	{
		get
		{
			return contactExportToolBtnEnable;
		}
		set
		{
			if (contactExportToolBtnEnable != value)
			{
				contactExportToolBtnEnable = value;
				OnPropertyChanged("ContactExportToolBtnEnable");
			}
		}
	}

	public bool ContactDeleteToolBtnEnable
	{
		get
		{
			return contactDeleteToolBtnEnable;
		}
		set
		{
			if (contactDeleteToolBtnEnable != value)
			{
				contactDeleteToolBtnEnable = value;
				OnPropertyChanged("ContactDeleteToolBtnEnable");
			}
		}
	}

	public bool ContactRefreshToolBtnEnable
	{
		get
		{
			return contactRefreshToolBtnEnable;
		}
		set
		{
			if (contactRefreshToolBtnEnable != value)
			{
				contactRefreshToolBtnEnable = value;
				OnPropertyChanged("ContactRefreshToolBtnEnable");
			}
		}
	}

	public ReplayCommand CheckAllContactClickCommand { get; set; }

	public ReplayCommand ContactScrollViewChangedCommand { get; set; }

	public ReplayCommand MouseDoubleClickCommand { get; set; }

	public ContactAddOrEditViewModelV6 ContactAddOrEditViewModel
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

	public ContactDetailViewModelV6 ContactDetailViewModel
	{
		get
		{
			return contactDetailViewModel;
		}
		private set
		{
			if (contactDetailViewModel != value)
			{
				contactDetailViewModel = value;
				OnPropertyChanged("ContactDetailViewModel");
			}
		}
	}

	public bool CallLogCanSort
	{
		get
		{
			return callLogCanSort;
		}
		set
		{
			if (callLogCanSort != value)
			{
				callLogCanSort = value;
				OnPropertyChanged("CallLogCanSort");
			}
		}
	}

	public ContactViewModelV6()
	{
		ContactList = new ObservableCollection<ContactItemViewModelV6>();
		ContactGroupList = new ObservableCollection<ContactGroupItemViewModelV6>();
		ContactDetailViewModel = new ContactDetailViewModelV6(this);
		ContactAddOrEditViewModel = new ContactAddOrEditViewModelV6(ContactDetailViewModel);
		ContactAddOrEditViewModel.OnContactSaved += ContactAddOrEditViewModel_OnContactSaved;
		ContactSearchCommand = new ReplayCommand(ContactSearchCommandHandler);
		ExportContactCommand = new ReplayCommand(ExportContactCommandHandler);
		ImportContactCommand = new ReplayCommand(ImportContactCommandHandler);
		DelContactCommand = new ReplayCommand(DelContactCommandHandler);
		AddContactCommand = new ReplayCommand(AddContactCommandHandler);
		AddGroupCommand = new ReplayCommand(AddGroupCommandHandler);
		ContactScrollViewChangedCommand = new ReplayCommand(ContactScrollViewChangedCommandHandler);
		MouseDoubleClickCommand = new ReplayCommand(ShowContactDetailClickCommandHandler);
		CheckAllContactClickCommand = new ReplayCommand(CheckAllContactClickCommandHandler);
		RefreshAllCommand = new ReplayCommand(RefreshAllCommandHandler);
		SdCarVm = new SdCardViewModel();
	}

	public override void Dispose()
	{
		base.Dispose();
		HostProxy.CurrentDispatcher.Invoke(delegate
		{
			_contactDetailViewV6?.Close();
		});
	}

	private void ContactAddOrEditViewModel_OnContactSaved(object sender, ContactSavedEventArgs e)
	{
		ContactDataLoading(delegate
		{
			RefreshContacts(resetContactPropertyValueLoadedStatus: true);
		});
	}

	public override void LoadData()
	{
		Reset();
		base.LoadData();
		SdCarVm.LoadData(Context.CurrentDevice);
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			BeginContactLoading();
		});
		try
		{
			InitContactGroup();
		}
		finally
		{
			HostProxy.CurrentDispatcher?.Invoke(delegate
			{
				EndContactLoading();
			});
		}
	}

	public override void Reset()
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			base.Reset();
			SelectedContactGroup = null;
			ContactList?.Clear();
			ContactGroupList?.Clear();
			IsAllCallLogSelected = false;
			IsAllContactSelected = false;
			ContactTotalCount = 0;
			ContactCanSort = false;
			CallLogCanSort = false;
			ContactDetailViewModel.Clear();
			ContactAddOrEditViewModel.Clear();
			ContactDataCache.Single.Clear();
		});
	}

	private void ContactSearchCommandHandler(object prameter)
	{
		ContactDataLoading(delegate
		{
			List<string> idList = new List<string>();
			string text = string.Empty;
			if (prameter != null)
			{
				text = prameter.ToString();
			}
			if (string.IsNullOrEmpty(text))
			{
				idList = SelectedContactGroup.Contacts.Select((ContactItemViewModelV6 n) => n.Id).ToList();
			}
			else
			{
				idList = mgt.queryContactIdListByKeyWords(text);
				idList = idList.Where((string m) => SelectedContactGroup.Contacts.Select((ContactItemViewModelV6 n) => n.Id).Contains(m)).ToList();
			}
			List<ContactItemViewModelV6> allContacts = ContactDataCache.Single.GetAllContacts();
			if (allContacts != null)
			{
				allContacts.ForEach(delegate(ContactItemViewModelV6 m)
				{
					m.isSelected = false;
				});
				UpdateContactDisplayList(allContacts.Where((ContactItemViewModelV6 m) => idList.Contains(m.Id)), null, resetAll: true);
				PagingUpdateContactInfo();
			}
		});
	}

	private void AddContactCommandHandler(object prameter)
	{
		HidenDetialPanel();
		ContactAddOrEditViewModel.Clear();
		RawContactAddOrEditViewModelV6 rawContactAddOrEditViewModelV = new RawContactAddOrEditViewModelV6();
		ObservableCollection<MultiCheckBoxItemViewModel> observableCollection = new ObservableCollection<MultiCheckBoxItemViewModel>();
		foreach (ContactGroupItemViewModelV6 item in ContactDataCache.Single.GetAllContactsGroup())
		{
			if (!string.IsNullOrEmpty(item.Id) && !"0".Equals(item.Id))
			{
				observableCollection.Add(new MultiCheckBoxItemViewModel
				{
					IsChecked = false,
					DisplayContent = item.Name,
					Tag = item
				});
			}
		}
		rawContactAddOrEditViewModelV.PhoneGroupList = observableCollection;
		List<ContactItemPhoneViewModelV6> list = new List<ContactItemPhoneViewModelV6>();
		list.Add(new ContactItemPhoneViewModelV6
		{
			ContactInfoType = ContactInfoType.Telephone,
			Type = DetailType.Home,
			Content = string.Empty
		});
		list.Add(new ContactItemPhoneViewModelV6
		{
			ContactInfoType = ContactInfoType.Telephone,
			Type = DetailType.Home,
			Content = string.Empty,
			IsLast = true
		});
		rawContactAddOrEditViewModelV.PhoneNumberList = new ObservableCollection<ContactItemPhoneViewModelV6>(list);
		List<ContactItemPhoneViewModelV6> list2 = new List<ContactItemPhoneViewModelV6>();
		list2.Add(new ContactItemPhoneViewModelV6
		{
			ContactInfoType = ContactInfoType.Email,
			Type = DetailType.Home,
			Content = string.Empty
		});
		list2.Add(new ContactItemPhoneViewModelV6
		{
			ContactInfoType = ContactInfoType.Email,
			Type = DetailType.Home,
			Content = string.Empty,
			IsLast = true
		});
		rawContactAddOrEditViewModelV.EmailList = new ObservableCollection<ContactItemPhoneViewModelV6>(list2);
		List<ContactItemPhoneViewModelV6> list3 = new List<ContactItemPhoneViewModelV6>();
		list3.Add(new ContactItemPhoneViewModelV6
		{
			ContactInfoType = ContactInfoType.Address,
			Type = DetailType.Home,
			Content = string.Empty
		});
		list3.Add(new ContactItemPhoneViewModelV6
		{
			ContactInfoType = ContactInfoType.Address,
			Type = DetailType.Home,
			Content = string.Empty,
			IsLast = true
		});
		rawContactAddOrEditViewModelV.AddressList = new ObservableCollection<ContactItemPhoneViewModelV6>(list3);
		ContactAddOrEditViewModel.RawContactAddOrEditViewModelList = new ObservableCollection<RawContactAddOrEditViewModelV6> { rawContactAddOrEditViewModelV };
		ContactAddOrEditViewModel.EditMode = EditMode.Add;
		ContactAddOrEditViewModel.Title = "K0510";
		ContactAddOrEditViewModel.ContactAddPanelVisibility = Visibility.Visible;
		ContactDetailViewModel.ContactDetailVisibility = Visibility.Collapsed;
		_contactDetailViewV6 = new ContactDetailViewV6();
		_contactDetailViewV6.DataContext = this;
		Context.MessageBox.ShowMessage(_contactDetailViewV6);
	}

	private void AddGroupCommandHandler(object prameter)
	{
		ContactAddGroupViewV6 userUi = new ContactAddGroupViewV6();
		Context.MessageBox.ShowMessage(userUi);
	}

	public void AddNewGroup(string _groupName)
	{
		AsyncDataLoader.Loading(delegate
		{
			if (mgt.AddContactGroup(_groupName))
			{
				RefreshContacts(resetContactPropertyValueLoadedStatus: false);
			}
		});
	}

	private void ShowContactDetailClickCommandHandler(object parameter)
	{
		ContactItemViewModelV6 contactItemViewModelV = parameter as ContactItemViewModelV6;
		ContactDetailViewModel.Clear();
		ContactDetailViewModel.LoadData(contactItemViewModelV.Id);
		ContactAddOrEditViewModel.ContactAddPanelVisibility = Visibility.Collapsed;
		ContactDetailViewModel.ContactDetailVisibility = Visibility.Visible;
		_contactDetailViewV6 = new ContactDetailViewV6();
		_contactDetailViewV6.DataContext = this;
		Context.MessageBox.ShowMessage(_contactDetailViewV6);
	}

	private void ExportContactCommandHandler(object prameter)
	{
		HidenAddPanel();
		HidenDetialPanel();
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string dir = folderBrowserDialog.SelectedPath.Trim();
		List<string> idList = (from p in ContactList
			where p.isSelected
			select p into m
			select m.Id).ToList();
		if (idList.Count <= 0)
		{
			return;
		}
		new ImportAndExportWrapper().ExportString(BusinessType.CONTACT_EXPORT, 23, "K0533", "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", BackupRestoreStaticResources.SingleInstance.CONTACT, (IAsyncTaskContext taskContext) => idList, delegate(Action<Action<string>> trigger)
		{
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(Path.Combine(dir, "contacts_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".vcf"));
			StreamWriter writer = new StreamWriter(fileInfo.Create());
			try
			{
				trigger(delegate(string str)
				{
					writer.WriteLine(str);
				});
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

	private void ImportContactCommandHandler(object prameter)
	{
		HidenAddPanel();
		HidenDetialPanel();
		Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
		dlg.Title = "K0496";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			dlg.Title = HostProxy.LanguageService.Translate("K0496");
		}
		dlg.Filter = string.Format("{0}|*.vcf", HostProxy.LanguageService.Translate("K0478"));
		dlg.Multiselect = false;
		dlg.FileName = string.Empty;
		dlg.FilterIndex = 1;
		if (dlg.ShowDialog() != true)
		{
			return;
		}
		new ImportAndExportWrapper().ImportString(createDataHandler: delegate
		{
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(dlg.FileName);
			List<string> list = null;
			try
			{
				using StreamReader streamReader = new StreamReader(fileInfo.OpenRead());
				list = new List<string>();
				StringBuilder stringBuilder = new StringBuilder();
				string empty = string.Empty;
				while ((empty = streamReader.ReadLine()) != null)
				{
					stringBuilder.AppendLine(empty);
					if ("END:VCARD".Equals(empty))
					{
						list.Add(stringBuilder.ToString());
						stringBuilder.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.LogInstance.Error("Import contact throw ex:" + ex);
				list = null;
			}
			return list;
		}, businessType: BusinessType.CONTACT_IMPORT, requestServiceCode: 23, progressTitle: "K0532", resourceType: "{5A60E6A0-35DE-4EA5-926E-96C15B54DDF6}", resourceTypeName: BackupRestoreStaticResources.SingleInstance.CONTACT);
		RefreshAllCommandHandler(null);
	}

	private void ImportCallCommandHandler(object prameter)
	{
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
		openFileDialog.Title = "K0496";
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			openFileDialog.Title = HostProxy.LanguageService.Translate("K0496");
		}
		openFileDialog.Filter = string.Format("{0}|*.calllog", HostProxy.LanguageService.Translate("K0480"));
		openFileDialog.Multiselect = false;
		openFileDialog.FileName = string.Empty;
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() != true)
		{
			return;
		}
		string json = File.ReadAllText(openFileDialog.FileName);
		List<CallLog> callLogList = JsonHelper.DeserializeJson2List<CallLog>(json);
		if (callLogList == null || callLogList.Count == 0)
		{
			return;
		}
		new List<string>().Add(openFileDialog.FileName);
		_ = HostProxy.deviceManager.MasterDevice;
		new ImportAndExportWrapper().ImportString(createDataHandler: () => callLogList.Select((CallLog m) => JsonConvert.SerializeObject(m)).ToList(), businessType: BusinessType.CONTACT_CALLLOG_IMPORT, requestServiceCode: 22, progressTitle: "K0536", resourceType: "{89D4DB68-4258-4002-8557-E65959C558B3}", resourceTypeName: BackupRestoreStaticResources.SingleInstance.CALLLOG);
	}

	private void DelContactCommandHandler(object prameter)
	{
		HidenAddPanel();
		HidenDetialPanel();
		if (Context.MessageBox.ShowMessage("K0534", MessageBoxButton.OKCancel) != true)
		{
			return;
		}
		List<string> dellist = (from p in ContactList
			where p.isSelected
			select p into m
			select m.Id).ToList();
		if (dellist.Count <= 0)
		{
			return;
		}
		BeginContactLoading();
		AsyncDataLoader.Loading(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			BusinessData businessData = new BusinessData(BusinessType.CONTACT_DELETE, Context.CurrentDevice);
			bool flag = mgt.DeleteContact(dellist);
			stopwatch.Stop();
			HostProxy.BehaviorService.Collect(BusinessType.CONTACT_DELETE, businessData.Update(stopwatch.ElapsedMilliseconds, flag ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, dellist));
			if (flag)
			{
				RefreshContacts(resetContactPropertyValueLoadedStatus: true);
			}
			EndContactLoading();
		});
	}

	private void RefreshAllCommandHandler(object parameter)
	{
		ContactDataLoading(delegate
		{
			RefreshContacts(resetContactPropertyValueLoadedStatus: true);
		});
	}

	private void CheckAllContactClickCommandHandler(object parameter)
	{
		bool flag = (bool)parameter;
		foreach (ContactItemViewModelV6 contact in ContactList)
		{
			contact.isSelected = flag;
		}
		bool flag3 = (ContactDeleteToolBtnEnable = flag);
		ContactExportToolBtnEnable = flag3;
		HidenDetialPanel();
		HidenAddPanel();
		if (flag)
		{
			SelectContactCount = ContactList.Count;
		}
		else
		{
			SelectContactCount = 0;
		}
	}

	private void ContactItemCheckClickCommandHandler(object parameter)
	{
		int num = ContactList.Where((ContactItemViewModelV6 p) => p.isSelected).Count();
		bool flag2 = (ContactDeleteToolBtnEnable = ((num != 0) ? true : false));
		ContactExportToolBtnEnable = flag2;
		IsAllContactSelected = num != 0 && num == ContactList.Count;
		SelectContactCount = num;
	}

	private void ContactScrollViewChangedCommandHandler(object parameter)
	{
		if ((parameter as InvokeCommandActionParameters).InvokeParameter is ScrollChangedEventArgs scrollChangedEventArgs)
		{
			int num = (int)scrollChangedEventArgs.VerticalOffset;
			mContactDisplayIdexInList = num;
			PagingUpdateContactInfo();
		}
	}

	private void MouseDoubleClickCommandHandler(object parameter)
	{
		if (parameter is ContactItemViewModelV6 contactItemViewModelV)
		{
			ContactDetailViewModel.Clear();
			ContactDetailViewModel.LoadData(contactItemViewModelV.Id);
			ContactAddOrEditViewModel.EditMode = EditMode.Update;
			ContactAddOrEditViewModel.Title = "K0521";
			ContactAddOrEditViewModel.ContactAddPanelVisibility = Visibility.Visible;
			ContactAddOrEditViewModel.LoadData();
		}
	}

	private void PagingUpdateContactInfo()
	{
		if (ContactList != null)
		{
			List<string> list = (from m in ContactList
				where !m.PropertyDataUpdated
				select m.Id).ToList();
			if (list.Count > 0)
			{
				new DataPartPageLoader().Load(list, PartPageLoadHandler, PartPageLoadFinished);
			}
			else
			{
				ContactCanSort = true;
			}
		}
	}

	private void PartPageLoadHandler(List<string> contactList, CancellationTokenSource cancel)
	{
		int num = 0;
		List<string> list = null;
		ContactCanSort = false;
		while (!cancel.IsCancellationRequested)
		{
			list = contactList.Skip(num * 20).Take(20).ToList();
			if (list.Count == 0)
			{
				ContactCanSort = true;
				break;
			}
			List<Contact> targetContactList = mgt.getContactListByContactId(list);
			if (cancel.IsCancellationRequested || targetContactList == null)
			{
				break;
			}
			HostProxy.CurrentDispatcher?.BeginInvoke((Action)delegate
			{
				ContactDataCache.Single.FillContactProperty(targetContactList);
				List<ContactItemViewModelV6> list2 = ContactList.OrderBy((ContactItemViewModelV6 p) => p.Name).ToList();
				ContactList.Clear();
				list2.ForEach(delegate(ContactItemViewModelV6 p)
				{
					ContactList.Add(p);
				});
			});
			num++;
		}
	}

	private void PartPageLoadFinished()
	{
	}

	private void UpdateContactGroupDisplayList(IEnumerable<ContactGroupItemViewModelV6> needAddList, IEnumerable<ContactGroupItemViewModelV6> needDeleteList, bool reset = false)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (reset)
			{
				ContactGroupList.Clear();
			}
			if (needAddList != null)
			{
				foreach (ContactGroupItemViewModelV6 needAdd in needAddList)
				{
					ContactGroupList.Add(needAdd);
				}
			}
			if (needDeleteList != null && ContactGroupList.Count > 0)
			{
				foreach (ContactGroupItemViewModelV6 needDelete in needDeleteList)
				{
					for (int num = ContactGroupList.Count - 1; num >= 0; num--)
					{
						if (needDelete.Id.Equals(ContactGroupList[num].Id))
						{
							ContactGroupList.RemoveAt(num);
							break;
						}
					}
				}
			}
			SelectedContactGroup = ((SelectedContactGroup == null && ContactGroupList.Count > 0) ? ContactGroupList[0] : SelectedContactGroup);
		});
	}

	private void RefreshContacts(bool resetContactPropertyValueLoadedStatus)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			foreach (ContactGroupItemViewModelV6 contactGroup in ContactGroupList)
			{
				contactGroup.Contacts = null;
				contactGroup.isSelected = false;
			}
		});
		if (resetContactPropertyValueLoadedStatus)
		{
			foreach (ContactItemViewModelV6 allContact in ContactDataCache.Single.GetAllContacts())
			{
				allContact.PropertyDataUpdated = false;
			}
		}
		ContactTotalCount = UpdateContactIdListCacheData();
		InitContactGroup();
		if (SelectedContactGroup != null)
		{
			LoadContactIdListByGroup(SelectedContactGroup, updateDisplayList: true);
			PagingUpdateContactInfo();
		}
	}

	private void ContactDataLoading(Action task)
	{
		BeginContactLoading();
		Task.Factory.StartNew(delegate
		{
			try
			{
				task();
			}
			finally
			{
				HostProxy.CurrentDispatcher?.Invoke(delegate
				{
					EndContactLoading();
				});
			}
		});
	}

	private void BeginContactLoading()
	{
		ContactExportToolBtnEnable = false;
		ContactDeleteToolBtnEnable = false;
		ContactRefreshToolBtnEnable = false;
	}

	private void EndContactLoading()
	{
		ContactItemCheckClickCommandHandler(null);
		ContactRefreshToolBtnEnable = true;
	}

	private void InitContactGroup()
	{
		List<ContactGroup> contactGroup = mgt.GetContactGroup();
		if (contactGroup != null)
		{
			Tuple<List<ContactGroupItemViewModelV6>, List<ContactGroupItemViewModelV6>> tuple = ContactDataCache.Single.FillGroup(contactGroup);
			UpdateContactGroupDisplayList(tuple.Item1, tuple.Item2);
		}
	}

	private void LoadContactIdListByGroup(ContactGroupItemViewModelV6 group, bool updateDisplayList)
	{
		if (group == null)
		{
			return;
		}
		if (group.Contacts == null)
		{
			if (string.IsNullOrEmpty(group.Id))
			{
				List<string> groupedContactIdList = mgt.getHaveBeenGroupedContactIdList();
				if (groupedContactIdList == null)
				{
					return;
				}
				List<ContactItemViewModelV6> allContacts = ContactDataCache.Single.GetAllContacts();
				if (allContacts == null)
				{
					return;
				}
				group.Contacts = allContacts.Where((ContactItemViewModelV6 m) => !groupedContactIdList.Contains(m.Id)).ToList();
			}
			else if (group.Id.Equals("0"))
			{
				UpdateContactIdListCacheData();
				List<ContactItemViewModelV6> allContacts2 = ContactDataCache.Single.GetAllContacts();
				if (allContacts2 == null)
				{
					return;
				}
				group.Contacts = allContacts2.ToList();
				ContactTotalCount = allContacts2.Count;
			}
			else
			{
				List<string> contactIdList = mgt.getContactsIdListByGroupId(group.Id);
				if (contactIdList == null)
				{
					return;
				}
				List<ContactItemViewModelV6> allContacts3 = ContactDataCache.Single.GetAllContacts();
				if (allContacts3 == null)
				{
					return;
				}
				group.Contacts = allContacts3.Where((ContactItemViewModelV6 m) => contactIdList.Contains(m.Id)).ToList();
			}
		}
		if (updateDisplayList)
		{
			UpdateContactDisplayList(group.Contacts, null, resetAll: true);
		}
		PagingUpdateContactInfo();
	}

	private int UpdateContactIdListCacheData()
	{
		List<string> allContactsId = mgt.GetAllContactsId();
		if (allContactsId == null)
		{
			return 0;
		}
		ContactDataCache.Single.FillContactId(allContactsId);
		return allContactsId.Count;
	}

	private void UpdateContactDisplayList(IEnumerable<ContactItemViewModelV6> needAddList, IEnumerable<ContactItemViewModelV6> needRemoveList, bool resetAll = false)
	{
		HostProxy.CurrentDispatcher?.Invoke(delegate
		{
			if (resetAll)
			{
				ContactList.Clear();
			}
			if (needAddList != null)
			{
				foreach (ContactItemViewModelV6 needAdd in needAddList)
				{
					ContactList.Add(needAdd);
					if (needAdd.CheckClickCommand == null)
					{
						needAdd.CheckClickCommand = new ReplayCommand(ContactItemCheckClickCommandHandler);
					}
					if (needAdd.ShowContactDetailClickCommand == null)
					{
						needAdd.ShowContactDetailClickCommand = new ReplayCommand(ShowContactDetailClickCommandHandler);
					}
				}
			}
			if (needRemoveList != null && ContactList.Count > 0)
			{
				foreach (ContactItemViewModelV6 needRemove in needRemoveList)
				{
					for (int num = ContactList.Count - 1; num >= 0; num--)
					{
						if (needRemove.Id.Equals(ContactList[num].Id))
						{
							ContactList.RemoveAt(num);
							break;
						}
					}
				}
			}
		});
	}

	private void HidenAddPanel()
	{
		ContactAddOrEditViewModel.ContactAddPanelVisibility = Visibility.Hidden;
	}

	private void HidenDetialPanel()
	{
		ContactDetailViewModel.ContactDetailVisibility = Visibility.Hidden;
	}

	private void ShowDetailPanel()
	{
	}
}
