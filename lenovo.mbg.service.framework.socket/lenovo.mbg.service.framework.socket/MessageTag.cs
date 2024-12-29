namespace lenovo.mbg.service.framework.socket;

public static class MessageTag
{
	public const string MSG_END_TAG = "\\r\\n";

	public const string TAG_APP_HEARTBEAT = "app-heartbeat";

	public const string TAG_PC_HEARTBEAT_RESPONSE = "pc-heartbeat-response";

	public const string TAG_APP_CREATE_CMD_CHANNEL = "app-cmd-createcmdchannel";

	public const string TAG_PC_CMD_CHANNEL_CREATED = "pc-cmdchannelcreated";

	public const string TAG_PC_CMD_GET_DCIM_INFO = "pc-cmd-getdciminfo";

	public const string TAG_CMD_RESULT_GET_DCIM_INFO = "app-cmd-result-getdciminfo";
}
