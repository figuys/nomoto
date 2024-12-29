using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.phoneManager.Business;
using lenovo.mbg.service.lmsa.phoneManager.Common;
using lenovo.mbg.service.lmsa.phoneManager.Model;
using lenovo.themes.generic.ViewModels;
using lenovo.themes.generic.ViewModelV6;
using Microsoft.Win32;

namespace lenovo.mbg.service.lmsa.phoneManager.ViewModels;

public class RawContactAddOrEditViewModel : ViewModelBase
{
	private static ImageSource _defaultImage;

	private string _name = string.Empty;

	private ObservableCollection<ContactItemPhoneViewModel> _numberList;

	private ObservableCollection<ContactItemPhoneViewModel> _emailList;

	private ObservableCollection<ContactItemPhoneViewModel> _addressList;

	private ObservableCollection<MultiCheckBoxItemViewModel> groupList;

	private string _note;

	private string _email;

	private string _address;

	private DateTime? birthday;

	private string newAvatarPath = string.Empty;

	private ImageSource avatarImage;

	public RawContactDetail RawContactDetailData { get; set; }

	public string Id { get; set; }

	public string AvatarId { get; set; }

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

	public ObservableCollection<ContactItemPhoneViewModel> PhoneNumberList
	{
		get
		{
			return _numberList;
		}
		set
		{
			if (_numberList != value)
			{
				_numberList = value;
				OnPropertyChanged("PhoneNumberList");
			}
		}
	}

	public ObservableCollection<ContactItemPhoneViewModel> EmailList
	{
		get
		{
			return _emailList;
		}
		set
		{
			if (_emailList != value)
			{
				_emailList = value;
				OnPropertyChanged("EmailList");
			}
		}
	}

	public ObservableCollection<ContactItemPhoneViewModel> AddressList
	{
		get
		{
			return _addressList;
		}
		set
		{
			if (_addressList != value)
			{
				_addressList = value;
				OnPropertyChanged("AddressList");
			}
		}
	}

	public ObservableCollection<MultiCheckBoxItemViewModel> PhoneGroupList
	{
		get
		{
			return groupList;
		}
		set
		{
			if (groupList != value)
			{
				groupList = value;
				OnPropertyChanged("PhoneGroupList");
			}
		}
	}

	public string Note
	{
		get
		{
			return _note;
		}
		set
		{
			_note = value;
			OnPropertyChanged("Note");
		}
	}

	public string Email
	{
		get
		{
			return _email;
		}
		set
		{
			_email = value;
			OnPropertyChanged("Email");
		}
	}

	public string Address
	{
		get
		{
			return _address;
		}
		set
		{
			_address = value;
			OnPropertyChanged("Address");
		}
	}

	public DateTime? Birthday
	{
		get
		{
			return birthday;
		}
		set
		{
			if (!(birthday == value))
			{
				birthday = value;
				OnPropertyChanged("Birthday");
			}
		}
	}

	public static ReplayCommand ApplendTextboxPanelLineClickCommand { get; set; }

	public string AvatarPath { get; set; }

	public string NewAvatarPath
	{
		get
		{
			return newAvatarPath;
		}
		private set
		{
			newAvatarPath = value;
		}
	}

	public ContactType Type { get; set; }

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

	public ReplayCommand AddAvatarButtonClickCommand { get; set; }

	static RawContactAddOrEditViewModel()
	{
		_defaultImage = Application.Current.FindResource("v6_icon_contact_default_image") as ImageSource;
	}

	public RawContactAddOrEditViewModel()
	{
		ApplendTextboxPanelLineClickCommand = new ReplayCommand(ApplendTextboxPanelLineClickCommandHandler);
		AddAvatarButtonClickCommand = new ReplayCommand(AddAvatarButtonClickCommandHandler);
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

	private void ApplendTextboxPanelLineClickCommandHandler(object parameter)
	{
		if (parameter != null)
		{
			ContactInfoType contactInfoType = (ContactInfoType)Enum.Parse(typeof(ContactInfoType), parameter.ToString());
			switch (contactInfoType)
			{
			case ContactInfoType.Telephone:
				AddNewElement(_numberList, contactInfoType);
				break;
			case ContactInfoType.Email:
				AddNewElement(_emailList, contactInfoType);
				break;
			case ContactInfoType.Address:
				AddNewElement(_addressList, contactInfoType);
				break;
			}
		}
	}

	private void AddNewElement(ObservableCollection<ContactItemPhoneViewModel> _list, ContactInfoType _type)
	{
		ContactItemPhoneViewModel item = new ContactItemPhoneViewModel
		{
			Content = string.Empty,
			Type = DetailType.Home,
			IsLast = true,
			ContactInfoType = _type
		};
		ContactItemPhoneViewModel contactItemPhoneViewModel = _list.Last();
		contactItemPhoneViewModel.IsLast = false;
		_list.RemoveAt(_list.Count - 1);
		_list.Add(contactItemPhoneViewModel);
		_list.Add(item);
	}

	private void AddAvatarButtonClickCommandHandler(object parameter)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "pic|*.jpg;*.png;*.jpeg;*.bmp;*.gif";
		if (openFileDialog.ShowDialog() == true)
		{
			SetAvatarImage(openFileDialog.FileName.ToString());
		}
	}

	private void SetAvatarImage(string filePath)
	{
		if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
		{
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
			string text = filePath;
			if (fileInfo.Length > 1024000)
			{
				text = Path.Combine(Configurations.TempDir, Guid.NewGuid().ToString("N") + Path.GetExtension(filePath));
				ImageHandle.ZoomAuto(filePath, text, 400.0, 400.0);
			}
			AvatarImage = ImageHandleHelper.LoadBitmap(text);
			AvatarPath = text;
			NewAvatarPath = text;
		}
	}

	public override void LoadData(object data)
	{
		Tuple<RawContactDetail, ContactDetailViewModel> obj = data as Tuple<RawContactDetail, ContactDetailViewModel>;
		RawContactDetail item = obj.Item1;
		ContactDetailViewModel item2 = obj.Item2;
		RawContactDetailData = item;
		Id = item.RawContactId;
		Name = item.Name;
		AvatarId = item.AvatarIdInDataTable;
		if (item2.AvatarImage != null && !string.IsNullOrEmpty(AvatarId) && AvatarId.Equals(item.AvatarIdInDataTable) && !item2.AvatarImage.ToString().Equals(ContactDetailViewModel.AvatarDefaultImagePath))
		{
			AvatarImage = item2.AvatarImage;
		}
		else if (!string.IsNullOrEmpty(AvatarId))
		{
			BeginLoadAvatar(AvatarId);
		}
		ObservableCollection<MultiCheckBoxItemViewModel> observableCollection = new ObservableCollection<MultiCheckBoxItemViewModel>();
		foreach (ContactGroupItemViewModel item3 in ContactViewModel.ContactDataCache.Single.GetAllContactsGroup())
		{
			if (!string.IsNullOrEmpty(item3.Id) && !"0".Equals(item3.Id))
			{
				observableCollection.Add(new MultiCheckBoxItemViewModel
				{
					IsChecked = (item.GroupList != null && item.GroupList.Exists((ContactGroup m) => m.Id.Equals(item3.Id))),
					DisplayContent = item3.Name,
					Tag = item3
				});
			}
		}
		PhoneGroupList = observableCollection;
		if (!string.IsNullOrEmpty(item.Birthday) && DateTime.TryParse(item.Birthday, out var result))
		{
			Birthday = result;
		}
		Note = item.Note;
		int num = 0;
		List<ContactItemPhoneViewModel> list = new List<ContactItemPhoneViewModel>();
		if (item.PhoneList != null)
		{
			foreach (Phone phone in item.PhoneList)
			{
				list.Add(new ContactItemPhoneViewModel
				{
					ContactInfoType = ContactInfoType.Telephone,
					Type = phone.PhoneType,
					Content = phone.PhoneNumber,
					Id = phone.Id
				});
			}
		}
		num = list.Count;
		for (int i = 0; i < 2 - num; i++)
		{
			list.Add(new ContactItemPhoneViewModel
			{
				ContactInfoType = ContactInfoType.Telephone,
				Type = DetailType.Home,
				Content = string.Empty
			});
		}
		list.Last().IsLast = true;
		PhoneNumberList = new ObservableCollection<ContactItemPhoneViewModel>(list);
		List<ContactItemPhoneViewModel> list2 = new List<ContactItemPhoneViewModel>();
		if (!string.IsNullOrEmpty(item.Email))
		{
			list2.Add(new ContactItemPhoneViewModel
			{
				ContactInfoType = ContactInfoType.Email,
				Type = DetailType.Other,
				Content = item.Email
			});
			list2.Add(new ContactItemPhoneViewModel
			{
				ContactInfoType = ContactInfoType.Email,
				Type = DetailType.Home,
				Content = string.Empty,
				IsLast = true
			});
		}
		if (item.EmailList != null)
		{
			foreach (Email email in item.EmailList)
			{
				list2.Add(new ContactItemPhoneViewModel
				{
					ContactInfoType = ContactInfoType.Email,
					Type = email.EmailType,
					Content = email.EmailAddr,
					Id = email.Id
				});
			}
		}
		num = list2.Count;
		for (int j = 0; j < 2 - num; j++)
		{
			list2.Add(new ContactItemPhoneViewModel
			{
				ContactInfoType = ContactInfoType.Email,
				Type = DetailType.Home,
				Content = string.Empty
			});
		}
		list2.Last().IsLast = true;
		EmailList = new ObservableCollection<ContactItemPhoneViewModel>(list2);
		List<ContactItemPhoneViewModel> list3 = new List<ContactItemPhoneViewModel>();
		if (!string.IsNullOrEmpty(item.Address))
		{
			list3.Add(new ContactItemPhoneViewModel
			{
				ContactInfoType = ContactInfoType.Address,
				Type = DetailType.Other,
				Content = item.Address
			});
			list3.Add(new ContactItemPhoneViewModel
			{
				ContactInfoType = ContactInfoType.Address,
				Type = DetailType.Home,
				Content = string.Empty,
				IsLast = true
			});
		}
		if (item.AddressList != null)
		{
			foreach (Address address in item.AddressList)
			{
				list3.Add(new ContactItemPhoneViewModel
				{
					ContactInfoType = ContactInfoType.Address,
					Type = address.AddressType,
					Content = address.AddressStr,
					Id = address.Id
				});
			}
		}
		num = list3.Count;
		for (int k = 0; k < 2 - num; k++)
		{
			list3.Add(new ContactItemPhoneViewModel
			{
				ContactInfoType = ContactInfoType.Address,
				Type = DetailType.Home,
				Content = string.Empty
			});
		}
		list3.Last().IsLast = true;
		AddressList = new ObservableCollection<ContactItemPhoneViewModel>(list3);
	}
}
