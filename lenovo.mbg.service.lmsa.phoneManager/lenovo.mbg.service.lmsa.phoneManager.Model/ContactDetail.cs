using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.Model;

public class ContactDetail : Contact
{
	[JsonIgnore]
	private DateTime _birthday = DateTime.MinValue;

	private string _note;

	[JsonIgnore]
	private string _avatarName = string.Empty;

	[JsonProperty("contactGroupId")]
	public string GroupId { get; set; }

	[JsonProperty("email")]
	public List<Email> EmailList { get; set; }

	[JsonProperty("birthday")]
	public string ROWBirthday { get; set; }

	[JsonIgnore]
	public DateTime Birthday
	{
		get
		{
			if (_birthday != DateTime.MinValue)
			{
				return _birthday;
			}
			if (!string.IsNullOrEmpty(ROWBirthday))
			{
				DateTime.TryParse(ROWBirthday, out _birthday);
			}
			return _birthday;
		}
	}

	[JsonIgnore]
	public string AvatarPath { get; set; }

	[JsonProperty("note")]
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

	[JsonProperty("avatarName")]
	public string AvatarName
	{
		get
		{
			if (!string.IsNullOrEmpty(AvatarPath))
			{
				_avatarName = AvatarPath.Substring(AvatarPath.LastIndexOf("\\") + 1);
			}
			return _avatarName;
		}
	}

	[JsonProperty("avatar")]
	public string RowAvatarPath { get; set; }
}
