namespace lenovo.mbg.service.framework.devicemgt;

public enum ConnectErrorCode
{
	Unknown,
	OK,
	TcpConnectFailWithAppNotAllowed,
	TcpConnectFailWithTimeout,
	AppVersionNotMatched,
	ApkInstallFailWithHaveNoSpace,
	ApkInstallFail,
	PropertyLoadFail,
	LaunchAppFail
}
