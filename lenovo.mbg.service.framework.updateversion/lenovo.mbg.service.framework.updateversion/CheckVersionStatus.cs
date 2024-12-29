namespace lenovo.mbg.service.framework.updateversion;

public enum CheckVersionStatus
{
	CHECK_VERSION_START,
	CHECK_VERSION_FAILED,
	CHECK_VERSION_HAVE_NEW_VERSION,
	CHECK_VERSION_NOT_NEW_VERSION,
	CHECK_VERSION_DATA_REPEAT,
	CHECK_VERSION_DATA_INVALID,
	CHECK_VERSION_FINISH,
	CHECK_VERSION_NOT_NETWORK,
	CHECK_VERSION_WCF_CONNECTION_ERROR,
	CHECK_VERSION_UNDEFINE
}
