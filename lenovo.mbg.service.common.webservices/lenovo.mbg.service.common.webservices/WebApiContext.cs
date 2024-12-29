using System;
using System.Collections.Generic;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiServices;

namespace lenovo.mbg.service.common.webservices;

public static class WebApiContext
{
	public static string GUID = Guid.NewGuid().ToString();

	public const string REQUEST_CODE_ERROR = "ERROR";

	public const string REQUEST_CODE_0000 = "0000";

	public const string REQUEST_CODE_3010 = "3010";

	public const string REQUEST_CODE_3020 = "3020";

	public const string REQUEST_CODE_3030 = "3030";

	public const string REQUEST_CODE_3040 = "3040";

	public const string REQUEST_CODE_4000 = "4000";

	public const string REQUEST_CODE_4010 = "4010";

	public const string REQUEST_CODE_TOKENTIMEOUT = "402";

	private static string rsa_public_key = null;

	private static string language = null;

	private static string windows_version = null;

	private static string client_version = null;

	public static string JWT_TOKEN { get; set; }

	public static string RSA_PUBLIC_KEY
	{
		get
		{
			if (string.IsNullOrEmpty(rsa_public_key))
			{
				rsa_public_key = RsaService.InitPublicKey();
			}
			return rsa_public_key;
		}
	}

	public static string LANGUAGE
	{
		get
		{
			if (language == null)
			{
				language = LMSAContext.CurrentLanguage;
			}
			return language;
		}
	}

	public static string WINDOWS_VERSION
	{
		get
		{
			if (windows_version == null)
			{
				windows_version = LMSAContext.OsVersionName;
			}
			return windows_version;
		}
	}

	public static string CLIENT_VERSION
	{
		get
		{
			if (client_version == null)
			{
				client_version = LMSAContext.MainProcessVersion;
			}
			return client_version;
		}
	}

	public static Dictionary<string, string> REQUEST_AUTHOR_HEADERS
	{
		get
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "guid", GUID } };
			if (!string.IsNullOrEmpty(JWT_TOKEN))
			{
				dictionary.Add("Authorization", "Bearer " + JWT_TOKEN);
			}
			return dictionary;
		}
	}
}
