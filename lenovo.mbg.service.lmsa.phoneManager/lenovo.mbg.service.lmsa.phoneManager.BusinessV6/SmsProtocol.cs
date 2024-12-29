namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6;

internal static class SmsProtocol
{
	internal static class SmsCount
	{
		public const string REQUEST_GET_COUNT = "getSmsCount";

		public const string RESPONSE_GET_COUNT = "getSmsCountResponse";
	}

	internal static class SmsContact
	{
		public const string REQUEST_GET_CONTACTS = "getSmsContactList";

		public const string RESPONSE_GET_CONTACTS = "getSmsContactListResponse";
	}

	internal static class SmsContent
	{
		public const string REQUEST_GET_SMS = "getAllSmsByAddress";

		public const string RESPONSE_GET_SMS = "getAllSmsByAddressResponse";
	}

	internal static class SmsImport
	{
		public const string REQUEST_IMPORT_SMS = "importSms";

		public const string RESPONSE_IMPORT_SMS = "importSmsResponse";
	}

	internal static class SmsDelete
	{
		public const string REQUEST_DELETE_SMS = "deleteSms";

		public const string RESPONSE_DELETE_SMS = "deleteSMSResponse";
	}
}
