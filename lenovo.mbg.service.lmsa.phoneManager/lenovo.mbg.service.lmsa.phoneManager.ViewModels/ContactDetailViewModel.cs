using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class ContactDetailViewModel : ViewModelBase
{
	public static string AvatarDefaultImagePath;

	private static ImageSource _defaultImage;

	private string _name = string.Empty;

	private ImageSource avatarImage;

	private ObservableCollection<ContactDetailListItemViewModel> contactDataItemList;

	private Visibility contactDetailVisibility = Visibility.Hidden;

	private ContactViewModel ContactViewModel { get; set; }

	public ContactDetailEx ContactDetailData { get; set; }

	public string Id { get; set; }

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
			OnPropertyChanged("Name");
		}
	}

	public ImageSource AvatarImage
	{
		get
		{
			if (avatarImage == null)
			{
				return _defaultImage;
			}
			return avatarImage;
		}
		set
		{
			if (avatarImage != value)
			{
				avatarImage = value;
				OnPropertyChanged("AvatarImage");
			}
		}
	}

	public string AvatarId { get; set; }

	public ReplayCommand EditContactCommand { get; set; }

	public ObservableCollection<ContactDetailListItemViewModel> ContactDataItemList
	{
		get
		{
			return contactDataItemList;
		}
		set
		{
			if (contactDataItemList != value)
			{
				contactDataItemList = value;
				OnPropertyChanged("ContactDataItemList");
			}
		}
	}

	public Visibility ContactDetailVisibility
	{
		get
		{
			return contactDetailVisibility;
		}
		set
		{
			if (contactDetailVisibility != value)
			{
				contactDetailVisibility = value;
				OnPropertyChanged("ContactDetailVisibility");
			}
		}
	}

	public ReplayCommand CloseDetailPanelClickCommand { get; set; }

	public ContactDetailViewModel(ContactViewModel contactViewModel)
	{
		ContactViewModel = contactViewModel;
		EditContactCommand = new ReplayCommand(EditContactCommandHandler);
		CloseDetailPanelClickCommand = new ReplayCommand(CloseDetailPanelClickCommandHandler);
	}

	static ContactDetailViewModel()
	{
		AvatarDefaultImagePath = "pack://application:,,,/lenovo.mbg.service.lmsa.phoneManager;component/Assets/Images/avatar.png";
		_defaultImage = Application.Current.FindResource("v6_icon_contact_default_image") as ImageSource;
	}

	public override void LoadData(object data)
	{
		ContactDetailEx contactDetailEx = new DeviceContactManager().GetContactDetailEx(data.ToString());
		if (contactDetailEx == null)
		{
			return;
		}
		ContactDetailData = contactDetailEx;
		Id = contactDetailEx.ContactId;
		Name = contactDetailEx.DisplayName;
		AvatarId = contactDetailEx.AvatarId;
		ContactDataItemList = new ObservableCollection<ContactDetailListItemViewModel>();
		if (contactDetailEx.RawContactList == null)
		{
			return;
		}
		foreach (RawContactDetail rawContact in contactDetailEx.RawContactList)
		{
			if (!string.IsNullOrEmpty(rawContact.Birthday) && DateTime.TryParse(rawContact.Birthday, out var result))
			{
				ContactDataItemList.Add(new ContactDetailListItemViewModel
				{
					DisplayTitle = "K0530",
					DisplayValue = result.ToString("d")
				});
			}
			if (!string.IsNullOrEmpty(rawContact.Note))
			{
				ContactDataItemList.Add(new ContactDetailListItemViewModel
				{
					DisplayTitle = "K0604",
					DisplayValue = rawContact.Note
				});
			}
			if (rawContact.GroupList != null)
			{
				foreach (ContactGroup group in rawContact.GroupList)
				{
					ContactDataItemList.Add(new ContactDetailListItemViewModel
					{
						DisplayTitle = "K0515",
						DisplayValue = group.Name
					});
				}
			}
			List<Phone> phoneList = rawContact.PhoneList;
			if (phoneList != null && phoneList.Count > 0)
			{
				ContactDataItemList.Add(new ContactDetailListItemViewModel
				{
					DisplayTitle = "K1342"
				});
				foreach (Phone phone in rawContact.PhoneList)
				{
					ContactDataItemList.Add(new ContactDetailListItemViewModel
					{
						DisplayTitle = phone.PhoneType.ToString(),
						DisplayValue = phone.PhoneNumber,
						IsTop = false
					});
				}
			}
			if (string.IsNullOrEmpty(rawContact.Email))
			{
				List<Email> emailList = rawContact.EmailList;
				if (emailList == null || emailList.Count <= 0)
				{
					goto IL_02f4;
				}
			}
			ContactDataItemList.Add(new ContactDetailListItemViewModel
			{
				DisplayTitle = "K1250"
			});
			if (!string.IsNullOrEmpty(rawContact.Email))
			{
				ContactDataItemList.Add(new ContactDetailListItemViewModel
				{
					DisplayTitle = DetailType.Other.ToString(),
					DisplayValue = rawContact.Email,
					IsTop = false
				});
			}
			if (rawContact.EmailList != null)
			{
				foreach (Email email in rawContact.EmailList)
				{
					ContactDataItemList.Add(new ContactDetailListItemViewModel
					{
						DisplayTitle = email.EmailType.ToString(),
						DisplayValue = email.EmailAddr,
						IsTop = false
					});
				}
			}
			goto IL_02f4;
			IL_02f4:
			if (string.IsNullOrEmpty(rawContact.Address))
			{
				List<Address> addressList = rawContact.AddressList;
				if (addressList == null || addressList.Count <= 0)
				{
					continue;
				}
			}
			ContactDataItemList.Add(new ContactDetailListItemViewModel
			{
				DisplayTitle = "K1251"
			});
			if (!string.IsNullOrEmpty(rawContact.Address))
			{
				ContactDataItemList.Add(new ContactDetailListItemViewModel
				{
					DisplayTitle = DetailType.Other.ToString(),
					DisplayValue = rawContact.Address,
					IsTop = false
				});
			}
			if (rawContact.AddressList == null)
			{
				continue;
			}
			foreach (Address address in rawContact.AddressList)
			{
				ContactDataItemList.Add(new ContactDetailListItemViewModel
				{
					DisplayTitle = address.AddressType.ToString(),
					DisplayValue = address.AddressStr,
					IsTop = false
				});
			}
		}
		if (!string.IsNullOrEmpty(AvatarId))
		{
			BeginLoadAvatar(AvatarId);
		}
	}

	public void BeginLoadAvatar(string avatarId)
	{
		Task.Factory.StartNew(delegate
		{
			string avatarId2 = AvatarId;
			List<ContactAvatar> list = new DeviceContactManager().ExportContactAvatar(new List<string> { AvatarId });
			if (list != null && list.Count > 0 && avatarId2.Equals(AvatarId))
			{
				string filePath = list[0].ThumbnailFIlePath;
				if (File.Exists(filePath))
				{
					HostProxy.CurrentDispatcher?.Invoke(delegate
					{
						try
						{
							BitmapImage bitmapImage = new BitmapImage();
							bitmapImage.BeginInit();
							bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
							bitmapImage.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
							bitmapImage.EndInit();
							AvatarImage = bitmapImage;
						}
						catch
						{
						}
					});
					try
					{
						File.Delete(filePath);
					}
					catch (Exception)
					{
					}
				}
			}
		});
	}

	private void EditContactCommandHandler(object parameter)
	{
		ContactViewModel.ContactAddOrEditViewModel.Clear();
		ContactAddOrEditViewModel contactAddOrEditViewModel = ContactViewModel.ContactAddOrEditViewModel;
		contactAddOrEditViewModel.EditMode = EditMode.Update;
		contactAddOrEditViewModel.Title = "K0521";
		contactAddOrEditViewModel.ContactAddPanelVisibility = Visibility.Visible;
		contactAddOrEditViewModel.LoadData();
		ContactDetailVisibility = Visibility.Hidden;
	}

	private void CloseDetailPanelClickCommandHandler(object parameter)
	{
		ContactDetailVisibility = Visibility.Hidden;
	}

	public void Clear()
	{
		ContactDataItemList?.Clear();
		AvatarId = string.Empty;
		Id = string.Empty;
		Name = string.Empty;
		AvatarImage = _defaultImage;
		ContactDetailVisibility = Visibility.Hidden;
	}
}
