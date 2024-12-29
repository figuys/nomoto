using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

public class ContactAddOrEditViewModelV6 : ViewModelBase
{
	public const string ADD_MODEL_TITLE = "K0510";

	public const string EDIT_MODEL_TITLE = "K0521";

	private EditMode editMode;

	private string title;

	private Visibility contactAddPanelVisibility = Visibility.Hidden;

	private ObservableCollection<RawContactAddOrEditViewModelV6> rawContactAddOrEditViewModelList;

	public EditMode EditMode
	{
		get
		{
			return editMode;
		}
		set
		{
			editMode = value;
		}
	}

	public string Title
	{
		get
		{
			return title;
		}
		set
		{
			if (!(title == value))
			{
				title = value;
				OnPropertyChanged("Title");
			}
		}
	}

	public ContactDetailViewModelV6 ContactDetailViewModel { get; private set; }

	public ReplayCommand CloseAddOrEditPandelClickCommand { get; set; }

	public Visibility ContactAddPanelVisibility
	{
		get
		{
			return contactAddPanelVisibility;
		}
		set
		{
			if (contactAddPanelVisibility != value)
			{
				contactAddPanelVisibility = value;
				OnPropertyChanged("ContactAddPanelVisibility");
			}
		}
	}

	public ObservableCollection<RawContactAddOrEditViewModelV6> RawContactAddOrEditViewModelList
	{
		get
		{
			return rawContactAddOrEditViewModelList;
		}
		set
		{
			if (rawContactAddOrEditViewModelList != value)
			{
				rawContactAddOrEditViewModelList = value;
				OnPropertyChanged("RawContactAddOrEditViewModelList");
			}
		}
	}

	public ReplayCommand SaveContactInfoClickCommand { get; set; }

	public event EventHandler<ContactSavedEventArgs> OnContactSaved;

	public ContactAddOrEditViewModelV6(ContactDetailViewModelV6 detailViewModel)
	{
		ContactDetailViewModel = detailViewModel;
		SaveContactInfoClickCommand = new ReplayCommand(SaveContactInfoClickCommandHandler);
		CloseAddOrEditPandelClickCommand = new ReplayCommand(CloseAddOrEditPandelClickCommandHandler);
	}

	private void CloseAddOrEditPandelClickCommandHandler(object prameter)
	{
		ContactAddPanelVisibility = Visibility.Hidden;
	}

	private void SaveContactInfoClickCommandHandler(object parameter)
	{
		ListBox listBox = parameter as ListBox;
		ObservableCollection<RawContactAddOrEditViewModelV6> observableCollection = listBox.ItemsSource as ObservableCollection<RawContactAddOrEditViewModelV6>;
		RawContactAddOrEditViewModelV6 rawContactAddOrEditViewModelV = null;
		if ((rawContactAddOrEditViewModelV = observableCollection.Where((RawContactAddOrEditViewModelV6 m) => string.IsNullOrEmpty(m.Name)).FirstOrDefault()) != null)
		{
			listBox.ScrollIntoView(rawContactAddOrEditViewModelV);
			Context.MessageBox.ShowMessage("K0071", ResourcesHelper.StringResources.SingleInstance.CONTACT_EMPUTY_WARN_CONTENT);
			return;
		}
		ContactDetailEx contactDetailEx = null;
		contactDetailEx = ((EditMode != 0) ? ContactDetailViewModel.ContactDetailData : new ContactDetailEx());
		List<RawContactDetail> list = new List<RawContactDetail>(observableCollection.Count);
		Dictionary<string, System.IO.FileInfo> dictionary = new Dictionary<string, System.IO.FileInfo>();
		RawContactAddOrEditViewModelV6 rawContactAddOrEditViewModelV2 = null;
		for (int i = 0; i < observableCollection.Count; i++)
		{
			rawContactAddOrEditViewModelV2 = observableCollection[i];
			RawContactDetail rawContactDetail = new RawContactDetail();
			rawContactDetail.Name = rawContactAddOrEditViewModelV2.Name;
			rawContactDetail.NameIdInDataTable = rawContactAddOrEditViewModelV2.RawContactDetailData?.NameIdInDataTable ?? string.Empty;
			rawContactDetail.RawContactId = rawContactAddOrEditViewModelV2.Id;
			if (!string.IsNullOrEmpty(rawContactAddOrEditViewModelV2.NewAvatarPath))
			{
				dictionary[i.ToString()] = new System.IO.FileInfo(rawContactAddOrEditViewModelV2.NewAvatarPath);
			}
			if (rawContactAddOrEditViewModelV2.PhoneGroupList != null)
			{
				List<ContactGroupItemViewModelV6> list2 = (from m in rawContactAddOrEditViewModelV2.PhoneGroupList
					where m.IsChecked
					select m.Tag).Cast<ContactGroupItemViewModelV6>().ToList();
				rawContactDetail.GroupList = new List<ContactGroup>();
				foreach (ContactGroupItemViewModelV6 item in list2)
				{
					rawContactDetail.GroupList.Add(new ContactGroup
					{
						Id = item.Id,
						Name = item.Name
					});
				}
			}
			if (rawContactAddOrEditViewModelV2.PhoneNumberList != null)
			{
				rawContactDetail.PhoneList = new List<Phone>();
				foreach (ContactItemPhoneViewModelV6 phoneNumber in rawContactAddOrEditViewModelV2.PhoneNumberList)
				{
					if (!string.IsNullOrEmpty(phoneNumber.Content))
					{
						rawContactDetail.PhoneList.Add(new Phone
						{
							Id = phoneNumber.Id,
							PhoneType = phoneNumber.Type,
							PhoneNumber = phoneNumber.Content
						});
					}
				}
			}
			if (rawContactAddOrEditViewModelV2.EmailList != null)
			{
				rawContactDetail.EmailList = new List<Email>();
				foreach (ContactItemPhoneViewModelV6 email in rawContactAddOrEditViewModelV2.EmailList)
				{
					if (!string.IsNullOrEmpty(email.Content))
					{
						rawContactDetail.EmailList.Add(new Email
						{
							Id = email.Id,
							EmailType = email.Type,
							EmailAddr = email.Content
						});
						rawContactDetail.Email = email.Content;
						rawContactDetail.EmailIdInDataTable = rawContactAddOrEditViewModelV2.RawContactDetailData?.EmailIdInDataTable ?? string.Empty;
					}
				}
			}
			if (rawContactAddOrEditViewModelV2.AddressList != null)
			{
				rawContactDetail.AddressList = new List<Address>();
				foreach (ContactItemPhoneViewModelV6 address in rawContactAddOrEditViewModelV2.AddressList)
				{
					if (!string.IsNullOrEmpty(address.Content))
					{
						rawContactDetail.AddressList.Add(new Address
						{
							Id = address.Id,
							AddressType = address.Type,
							AddressStr = address.Content
						});
						rawContactDetail.Address = address.Content;
						rawContactDetail.AddressIdInDataTable = rawContactAddOrEditViewModelV2.RawContactDetailData?.AddressIdInDataTable ?? string.Empty;
					}
				}
			}
			rawContactDetail.Birthday = ((!rawContactAddOrEditViewModelV2.Birthday.HasValue) ? string.Empty : rawContactAddOrEditViewModelV2.Birthday.Value.ToString("d"));
			rawContactDetail.BirthdayIdInDataTable = rawContactAddOrEditViewModelV2.RawContactDetailData?.BirthdayIdInDataTable ?? string.Empty;
			rawContactDetail.Note = rawContactAddOrEditViewModelV2.Note;
			rawContactDetail.NoteIdInDataTable = rawContactAddOrEditViewModelV2.RawContactDetailData?.NoteIdInDataTable ?? string.Empty;
			list.Add(rawContactDetail);
		}
		contactDetailEx.RawContactList = list;
		bool flag = false;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		BusinessData businessData = new BusinessData(BusinessType.CONTACT_ADD, Context.CurrentDevice);
		if (flag = new DeviceContactManager().AddOrEditContactDetail(EditMode, contactDetailEx, dictionary))
		{
			stopwatch.Stop();
			if (EditMode == EditMode.Update)
			{
				string id = ContactDetailViewModel.Id;
				ContactDetailViewModel.Clear();
				ContactDetailViewModel.LoadData(id);
				ContactDetailViewModel.ContactDetailVisibility = Visibility.Visible;
			}
		}
		ContactAddPanelVisibility = Visibility.Hidden;
		HostProxy.BehaviorService.Collect(BusinessType.CONTACT_ADD, businessData.Update(stopwatch.ElapsedMilliseconds, flag ? BusinessStatus.SUCCESS : BusinessStatus.FALIED, contactDetailEx));
		FireOnContactSaved(this, new ContactSavedEventArgs(EditMode, flag));
	}

	private void FireOnContactSaved(object sender, ContactSavedEventArgs e)
	{
		this.OnContactSaved?.Invoke(sender, e);
	}

	public void Clear()
	{
		RawContactAddOrEditViewModelList = null;
		ContactAddPanelVisibility = Visibility.Hidden;
	}

	public override void LoadData()
	{
		ContactDetailViewModelV6 contactDetailViewModel = ContactDetailViewModel;
		ContactDetailEx contactDetailData = contactDetailViewModel.ContactDetailData;
		if (contactDetailData == null || contactDetailData.RawContactList == null)
		{
			return;
		}
		RawContactAddOrEditViewModelList = new ObservableCollection<RawContactAddOrEditViewModelV6>();
		foreach (RawContactDetail rawContact in contactDetailData.RawContactList)
		{
			RawContactAddOrEditViewModelV6 rawContactAddOrEditViewModelV = new RawContactAddOrEditViewModelV6();
			Tuple<RawContactDetail, ContactDetailViewModelV6> data = new Tuple<RawContactDetail, ContactDetailViewModelV6>(rawContact, contactDetailViewModel);
			rawContactAddOrEditViewModelV.LoadData(data);
			RawContactAddOrEditViewModelList.Add(rawContactAddOrEditViewModelV);
		}
	}
}
