namespace lenovo.mbg.service.common.utilities;

public sealed class MessageConstant
{
	public sealed class Permission
	{
		public sealed class Modules
		{
			public const string BASIC_INFO = "BasicInfo";

			public const string SET_RING_TONE = "SetRingTone";

			public const string APP = "Apps";

			public const string PICTURE = "Pictures";

			public const string SONG = "Songs";

			public const string VIDEO = "Videos";

			public const string CONTACT = "Contacts";

			public const string CALL_LOG = "CallLogs";

			public const string SMS = "SMS";

			public const string FILE = "File";

			public const string BACKUP = "Backup";

			public const string RESTORE = "Restore";

			public const string SCREEN_RECORD = "ScreenRecord";

			public const string ACCESS_ALL_FILES = "ACCESS_ALL_FILES";
		}

		public sealed class Actions
		{
			public const string CHECK_PERMISSIONS = "checkPermissions";
		}
	}

	public sealed class SMS
	{
		public sealed class Actions
		{
			public const string IS_MESSENGER_APPEXISTED = "isMessengerAppExisted";
		}
	}

	private const string ACTION_RESPONSE_SUFFIX = "Response";

	public static string getResponseAction(string action)
	{
		return action + "Response";
	}
}
