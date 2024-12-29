using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class RawContactDetail
{
	[JsonProperty("name")]
	public string Name;

	[JsonProperty("rawContactId")]
	public string RawContactId { get; set; }

	[JsonProperty("nameIdInDataTable")]
	public string NameIdInDataTable { get; set; }

	[JsonProperty("avatarPath")]
	public string AvatarPath { get; set; }

	[JsonProperty("avatarIdInDataTable")]
	public string AvatarIdInDataTable { get; set; }

	[JsonProperty("phoneList")]
	public List<Phone> PhoneList { get; set; }

	[JsonProperty("groupList")]
	public List<ContactGroup> GroupList { get; set; }

	[JsonProperty("birthday")]
	public string Birthday { get; set; }

	[JsonProperty("birthdayIdInDataTable")]
	public string BirthdayIdInDataTable { get; set; }

	[JsonProperty("note")]
	public string Note { get; set; }

	[JsonProperty("noteIdInDataTable")]
	public string NoteIdInDataTable { get; set; }

	[JsonProperty("email")]
	public string Email { get; set; }

	[JsonProperty("emailIdInDataTable")]
	public string EmailIdInDataTable { get; set; }

	[JsonProperty("emailList")]
	public List<Email> EmailList { get; set; }

	[JsonProperty("address")]
	public string Address { get; set; }

	[JsonProperty("addressIdInDataTable")]
	public string AddressIdInDataTable { get; set; }

	[JsonProperty("addressList")]
	public List<Address> AddressList { get; set; }
}
