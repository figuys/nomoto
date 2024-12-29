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
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class ContactAddOrEditViewModel : ViewModelBase
{
	public const string ADD_MODEL_TITLE = "K0510";

	public const string EDIT_MODEL_TITLE = "K0521";

	private EditMode editMode;

	private string title;

	private Visibility contactAddPanelVisibility = Visibility.Hidden;

	private ObservableCollection<RawContactAddOrEditViewModel> rawContactAddOrEditViewModelList;

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

	public ContactDetailViewModel ContactDetailViewModel { get; private set; }

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

	public ObservableCollection<RawContactAddOrEditViewModel> RawContactAddOrEditViewModelList
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

	public ContactAddOrEditViewModel(ContactDetailViewModel detailViewModel)
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
		ObservableCollection<RawContactAddOrEditViewModel> observableCollection = listBox.ItemsSource as ObservableCollection<RawContactAddOrEditViewModel>;
		RawContactAddOrEditViewModel rawContactAddOrEditViewModel = null;
		if ((rawContactAddOrEditViewModel = observableCollection.Where((RawContactAddOrEditViewModel m) => string.IsNullOrEmpty(m.Name)).FirstOrDefault()) != null)
		{
			listBox.ScrollIntoView(rawContactAddOrEditViewModel);
			LenovoPopupWindow win = new OkWindowModel().CreateWindow(HostProxy.Host.HostMainWindowHandle, "K0071", ResourcesHelper.StringResources.SingleInstance.CONTACT_EMPUTY_WARN_CONTENT, "K0327", null);
			HostProxy.HostMaskLayerWrapper.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
			{
				win.ShowDialog();
			});
			return;
		}
		ContactDetailEx contactDetailEx = null;
		contactDetailEx = ((EditMode != 0) ? ContactDetailViewModel.ContactDetailData : new ContactDetailEx());
		List<RawContactDetail> list = new List<RawContactDetail>(observableCollection.Count);
		Dictionary<string, System.IO.FileInfo> dictionary = new Dictionary<string, System.IO.FileInfo>();
		RawContactAddOrEditViewModel rawContactAddOrEditViewModel2 = null;
		for (int i = 0; i < observableCollection.Count; i++)
		{
			rawContactAddOrEditViewModel2 = observableCollection[i];
			RawContactDetail rawContactDetail = new RawContactDetail();
			rawContactDetail.Name = rawContactAddOrEditViewModel2.Name;
			rawContactDetail.NameIdInDataTable = rawContactAddOrEditViewModel2.RawContactDetailData?.NameIdInDataTable ?? string.Empty;
			rawContactDetail.RawContactId = rawContactAddOrEditViewModel2.Id;
			if (!string.IsNullOrEmpty(rawContactAddOrEditViewModel2.NewAvatarPath))
			{
				dictionary[i.ToString()] = new System.IO.FileInfo(rawContactAddOrEditViewModel2.NewAvatarPath);
			}
			if (rawContactAddOrEditViewModel2.PhoneGroupList != null)
			{
				List<ContactGroupItemViewModel> list2 = (from m in rawContactAddOrEditViewModel2.PhoneGroupList
					where m.IsChecked
					select m.Tag).Cast<ContactGroupItemViewModel>().ToList();
				rawContactDetail.GroupList = new List<ContactGroup>();
				foreach (ContactGroupItemViewModel item in list2)
				{
					rawContactDetail.GroupList.Add(new ContactGroup
					{
						Id = item.Id,
						Name = item.Name
					});
				}
			}
			if (rawContactAddOrEditViewModel2.PhoneNumberList != null)
			{
				rawContactDetail.PhoneList = new List<Phone>();
				foreach (ContactItemPhoneViewModel phoneNumber in rawContactAddOrEditViewModel2.PhoneNumberList)
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
			if (rawContactAddOrEditViewModel2.EmailList != null)
			{
				rawContactDetail.EmailList = new List<Email>();
				foreach (ContactItemPhoneViewModel email in rawContactAddOrEditViewModel2.EmailList)
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
						rawContactDetail.EmailIdInDataTable = rawContactAddOrEditViewModel2.RawContactDetailData?.EmailIdInDataTable ?? string.Empty;
					}
				}
			}
			if (rawContactAddOrEditViewModel2.AddressList != null)
			{
				rawContactDetail.AddressList = new List<Address>();
				foreach (ContactItemPhoneViewModel address in rawContactAddOrEditViewModel2.AddressList)
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
						rawContactDetail.AddressIdInDataTable = rawContactAddOrEditViewModel2.RawContactDetailData?.AddressIdInDataTable ?? string.Empty;
					}
				}
			}
			rawContactDetail.Birthday = ((!rawContactAddOrEditViewModel2.Birthday.HasValue) ? string.Empty : rawContactAddOrEditViewModel2.Birthday.Value.ToString("d"));
			rawContactDetail.BirthdayIdInDataTable = rawContactAddOrEditViewModel2.RawContactDetailData?.BirthdayIdInDataTable ?? string.Empty;
			rawContactDetail.Note = rawContactAddOrEditViewModel2.Note;
			rawContactDetail.NoteIdInDataTable = rawContactAddOrEditViewModel2.RawContactDetailData?.NoteIdInDataTable ?? string.Empty;
			list.Add(rawContactDetail);
		}
		contactDetailEx.RawContactList = list;
		bool flag = false;
		BusinessData businessData = new BusinessData(BusinessType.CONTACT_ADD, Context.CurrentDevice);
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
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
		ContactDetailViewModel contactDetailViewModel = ContactDetailViewModel;
		ContactDetailEx contactDetailData = contactDetailViewModel.ContactDetailData;
		if (contactDetailData == null || contactDetailData.RawContactList == null)
		{
			return;
		}
		RawContactAddOrEditViewModelList = new ObservableCollection<RawContactAddOrEditViewModel>();
		foreach (RawContactDetail rawContact in contactDetailData.RawContactList)
		{
			RawContactAddOrEditViewModel rawContactAddOrEditViewModel = new RawContactAddOrEditViewModel();
			Tuple<RawContactDetail, ContactDetailViewModel> data = new Tuple<RawContactDetail, ContactDetailViewModel>(rawContact, contactDetailViewModel);
			rawContactAddOrEditViewModel.LoadData(data);
			RawContactAddOrEditViewModelList.Add(rawContactAddOrEditViewModel);
		}
	}
}
