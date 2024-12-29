using System;
using System.Configuration;

namespace LmsaWindowsService.Common;

public static class Context
{
	public static class WebAPI
	{
		public static string INIT_TOKEN = BaseUrl + "/client/initToken.jhtml";

		public static string UPLOAD_URL = BaseUrl + "/dataCollection/uploadFile.jhtml";

		public static string GET_PUBLIC_KEY = BaseUrl + "/common/rsa.jhtml";
	}

	public static string GUID = Guid.NewGuid().ToString();

	public const string CLIENT_MARK = "LMSA-2017-02-28";

	public static string TOKEN { get; set; }

	public static string IDENTIFIER { get; set; }

	public static string BaseUrl
	{
		get
		{
			string text = ConfigurationManager.AppSettings["BaseHttpUrl"];
			string text2 = "https://lsa.lenovo.com";
			if (text != null)
			{
				text2 = text.ToString();
			}
			if (text2.EndsWith("/"))
			{
				text2 = text2.Substring(0, text2.Length - 1);
			}
			return text2 + "/Interface";
		}
	}
}
