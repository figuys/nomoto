using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services;

public class UserInfo
{
	public string UserId { get; set; }

	public string UserName { get; set; }

	public string EmailAddress { get; set; }

	public string Country { get; set; }

	public string FullName { get; set; }

	public string PhoneNumber { get; set; }

	public string UserSource { get; set; }

	public bool IsB2BSupportMultDev { get; set; }

	public Dictionary<string, object> Config { get; set; }
}
