using System.Linq;
using lenovo.mbg.service.common.webservices;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.Business;

public class MenuPopupWindowBusiness
{
	private class UserGuideWebServices
	{
		public string GetUserGuide()
		{
			JObject jObject = AppContext.WebApi.RequestContent<JObject>(WebApiUrl.USER_GUIDE, null);
			JProperty jProperty = jObject.Properties().FirstOrDefault((JProperty n) => n.Name.ToLower() == "guidefile");
			if (jProperty == null || jProperty.Value == null || jProperty.Value["uri"] == null)
			{
				return string.Empty;
			}
			return jProperty.Value["uri"].ToString();
		}

		public string CheckClientHelp()
		{
			JObject jObject = AppContext.WebApi.RequestContent<JObject>(WebApiUrl.HELP_URI, null);
			JProperty jProperty = jObject.Properties().FirstOrDefault((JProperty n) => n.Name.ToLower() == "url");
			if (jProperty == null || jProperty.Value == null)
			{
				return string.Empty;
			}
			return jProperty.Value.ToString();
		}
	}

	public string GetUserGuideUrl()
	{
		return new UserGuideWebServices().GetUserGuide();
	}

	public string GetClientHelpUrl()
	{
		return new UserGuideWebServices().CheckClientHelp();
	}
}
