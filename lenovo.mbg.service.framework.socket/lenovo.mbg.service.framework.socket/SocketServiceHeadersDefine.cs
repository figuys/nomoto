namespace lenovo.mbg.service.framework.socket;

public class SocketServiceHeadersDefine
{
	public sealed class File
	{
		public const string FILE_ID = "FileId";

		public const string FILE_NAME = "FileName";

		public const string FILE_FULL_NAME = "FileFullName";

		public const string FILE_DIR = "FileDir";

		public const string FILE_IS_REWRITE = "FileIsRewrite";

		public const string CREATE_DATE_TIME = "CreateDateTime";

		public const string LAST_MODIFY_DATE_TIME = "LastModifyDateTime";
	}

	public sealed class Status
	{
		public const string STATUS_KEY = "Status";

		public const string CREATE_DIR_FAILED = "-1";

		public const string CREATE_FILE_FAILED = "-2";

		public const string DELETE_FILE_FAILED = "-3";

		public const string FILE_NOT_EXISTS = "-4";

		public const string FILE_OPEN_FAILED = "-5";

		public const string NORMAL = "-6";

		public const string NO_PERMISSION = "-7";

		public const string UNDEFINE = "-8";

		public const string RECEIVE_STREAM_ERROR = "-9";

		public const string THERE_IS_NO_RESOURCE = "-10";

		public const string THE_END = "-11";
	}

	public const string METHOD_NAME = "MethodName";

	public const string STREAM_LENGTH = "StreamLength";

	public const string RESOURCE_ID = "ResourceId";

	public const string TIMEOUT = "Timeout";
}
