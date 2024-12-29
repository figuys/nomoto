using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

public interface IContact
{
	[JsonIgnore]
	string PhonePerson { get; }

	[JsonIgnore]
	string PhoneNumber { get; }

	[JsonIgnore]
	int MsgTotal { get; }

	[JsonIgnore]
	long Latest { get; }
}
