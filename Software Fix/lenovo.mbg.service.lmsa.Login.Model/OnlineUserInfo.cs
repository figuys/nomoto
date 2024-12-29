using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.Login.Model;

public class OnlineUserInfo
{
	public string UserId { get; set; }

	public string UserName { get; set; }

	public string EmailAddress { get; set; }

	public string Country { get; set; }

	public string FullName { get; set; }

	public string PhoneNumber { get; set; }

	public string UserSource { get; set; }

	public int quitSurvey { get; set; }

	public int failureNum { get; set; }

	public bool IsB2BSupportMultDev { get; set; }

	public bool IsRtNotify { get; set; }

	public bool B2BBuyNowDisplay { get; set; }

	public bool B2BEntranceEnable { get; set; }

	public string PriceUnit { get; set; }

	public float Price1 { get; set; }

	public float Price2 { get; set; }

	public float Price3 { get; set; }

	public string SkuName1 { get; set; }

	public string SkuName2 { get; set; }

	public string SkuName3 { get; set; }

	public Dictionary<string, object> config { get; set; }
}
