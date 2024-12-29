using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.common.webservices;

public static class WebApiUrl
{
	public static string GET_PUBLIC_KEY = Configurations.ServiceInterfaceUrl + "/common/rsa.jhtml";

	public static string INIT_TOKEN = Configurations.ServiceInterfaceUrl + "/client/initToken.jhtml";

	public static string GET_DEVICE_INFO = Configurations.ServiceInterfaceUrl + "/device/getDeviceInfo.jhtml";

	public static string GET_DEVICE_ICON = Configurations.ServiceInterfaceUrl + "/device/getDeviceIcon.jhtml";

	public static string POST_UPGRADE_FLASH_INFO = Configurations.ServiceInterfaceUrl + "/dataCollection/UpgradeFlashInfo.jhtml";

	public static string DISPOSE_TOKEN = Configurations.ServiceInterfaceUrl + "/client/deleteToken.jhtml";

	public static string USER_GUIDE = Configurations.ServiceInterfaceUrl + "/client/getUserGuide.jhtml";

	public static string HELP_URI = Configurations.ServiceInterfaceUrl + "/client/clientHelp.jhtml";

	public static string CLIENT_VERSION = Configurations.ServiceInterfaceUrl + "/client/getNextUpdateClient.jhtml";

	public static string UPDATE_VERSION = Configurations.ServiceInterfaceUrl + "/client/getPluginCategoryList.jhtml";

	public static string PLUGIN_VERSION = Configurations.ServiceInterfaceUrl + "/client/getClientPlugins.jhtml";

	public static string SHOW_FEEDBACK = Configurations.BaseHttpUrl + "/Tips/feedback.html";

	public static string Webwervice_Get_RomResources = Configurations.ServiceInterfaceUrl + "/priv/getRomList.jhtml";

	public static string SURVEY_URL = Configurations.ServiceInterfaceUrl + "/survey/getIsNeedTrigger.jhtml";

	public static string UPLOAD_USER_DEVICE = Configurations.ServiceInterfaceUrl + "/registeredModel/addModels.jhtml";

	public static string DELETE_USER_DEVICE = Configurations.ServiceInterfaceUrl + "/registeredModel/models.jhtml";

	public static string LENOVOID_LOGIN_CALLBACK = Configurations.ServiceInterfaceUrl + "/user/lenovoIdLogin.jhtml";

	public static string UPDATE_DOWNLOAD_URL = Configurations.ServiceInterfaceUrl + "/client/renewFileLink.jhtml";

	public static string NOTICE_URL = Configurations.ServiceInterfaceUrl + "/notice/getNoticeInfo.jhtml";

	public static string NOTICE_BROADCAST_URL = Configurations.ServiceInterfaceUrl + "/notice/getBroadcast.jhtml";

	public static string GET_UPGRADEFLASH_MATCH_TYPES = Configurations.ServiceInterfaceUrl + "/rescueDevice/getParamType.jhtml";

	public static string RESUCE_AUTOMATCH_GETPARAMS_MAPPING = Configurations.ServiceInterfaceUrl + "/rescueDevice/getRomMatchParams.jhtml";

	public static string RESUCE_AUTOMATCH_GETROM = Configurations.ServiceInterfaceUrl + "/rescueDevice/getNewResource.jhtml";

	public static string RESUCE_CHECK_SUPPORT_FASTBOOT_MODE = Configurations.ServiceInterfaceUrl + "/rescueDevice/getMarketSupport.jhtml";

	public static string GET_MOLI_REQUEST_URL = Configurations.ServiceInterfaceUrl + "/moli/getMoliUrl.jhtml";

	public static string MODEL_READ_CONFIG = Configurations.ServiceInterfaceUrl + "/rescueDevice/modelReadConfigration.jhtml";

	public static string NETWORK_CONNECT_CHECK = "https://lsa.lenovo.com/lmsa-web/index.jsp";

	public static string LOAD_SMART_DEVICE = Configurations.ServiceInterfaceUrl + "/rescueDevice/smartMarketNames.jhtml";

	public static string LOAD_WARRANTY_BANNER = Configurations.ServiceInterfaceUrl + "/client/motoCare.jhtml";

	public static string LOAD_COUPON = Configurations.ServiceInterfaceUrl + "/client/discountCoupon.jhtml";

	public static string CALL_API_URL = Configurations.ServiceInterfaceUrl + "/dictionary/getApiInfo.jhtml";

	public static string CALL_B2B_ORDERS_URL = Configurations.ServiceInterfaceUrl + "/vip/getB2BInfo.jhtml";

	public static string CALL_B2B_ACTIVE_ORDERS_URL = Configurations.ServiceInterfaceUrl + "/vip/getActiveB2BInfos.jhtml";

	public static string CALL_B2B_QUERY_ORDER_URL = Configurations.ServiceInterfaceUrl + "/vip/getEnableB2BOrder.jhtml";

	public static string CALL_B2B_GET_ORDERID_URL = Configurations.ServiceInterfaceUrl + "/vip/getOrderNum.jhtml";

	public static string CALL_B2B_ORDER_BUY_URL = Configurations.ServiceInterfaceUrl + "/vip/buy.jhtml";

	public static string CALL_B2B_GET_PRICE_URL = Configurations.ServiceInterfaceUrl + "/vip/card.jhtml";

	public static string ROMFILE_CHECK_RULES = Configurations.ServiceInterfaceUrl + "/model/rules.jhtml";

	public static string GET_SUPPORT_FASTBOOT_BY_MODELNAME = Configurations.ServiceInterfaceUrl + "/model/isReadSupport.jhtml";

	public static string GET_MUTIL_TUTORIALS_QUESTIONS = Configurations.ServiceInterfaceUrl + "/guide/getGuideQuestion.jhtml";

	public static string USER_LOGOUT = Configurations.ServiceInterfaceUrl + "/user/logout.jhtml";

	public static string USER_FORGOT_PASSWORD = Configurations.ServiceInterfaceUrl + "/user/forgotPassword.jhtml";

	public static string USER_CHANGE_PASSWORD = Configurations.ServiceInterfaceUrl + "/user/changePassword.jhtml";

	public static string USER_LOGIN = Configurations.ServiceInterfaceUrl + "/user/login.jhtml";

	public static string USER_GUEST_LOGIN = Configurations.ServiceInterfaceUrl + "/user/guestLogin.jhtml";

	public static string USER_RECORD_LOGIN = Configurations.ServiceInterfaceUrl + "/user/recordLogin.jhtml";

	public static string PRIV_GET_PRIV_INFO = Configurations.ServiceInterfaceUrl + "/priv/getPrivInfo.jhtml";

	public static string SURVEY_REFRESH = Configurations.ServiceInterfaceUrl + "/survey/refreshTrigger.jhtml";

	public static string SURVEY_GET_QUESTIONS = Configurations.ServiceInterfaceUrl + "/survey/getAllQuestions.jhtml";

	public static string SURVEY_RECORD = Configurations.ServiceInterfaceUrl + "/survey/record.jhtml";

	public static string FEEDBACK_GET_LIST = Configurations.ServiceInterfaceUrl + "/feedback/getFeedbackList.jhtml";

	public static string FEEDBACK_GET_INFO = Configurations.ServiceInterfaceUrl + "/feedback/getFeedbackInfo.jhtml";

	public static string FEEDBACK_FILE_SINGNATURE = Configurations.ServiceInterfaceUrl + "/feedback/fileSignatureUrl.jhtml";

	public static string FEEDBACK_GET_HELPFUL = Configurations.ServiceInterfaceUrl + "/feedback/replyHelpful.jhtml";

	public static string FEEDBACK_GET_UPLOAD = Configurations.ServiceInterfaceUrl + "/feedback/postFeedbackInfo.jhtml";

	public static string FEEDBACK_GET_UPLOAD_GUEST = Configurations.ServiceInterfaceUrl + "/feedback/guestPostFeedbackInfo.jhtml";

	public static string FEEDBACK_GET_ISSUE_INFO = Configurations.ServiceInterfaceUrl + "/feedback/getFeedbackIssueInfo.jhtml";

	public static string UPLOAD_DOWNLOAD_SPEEDINFO = Configurations.ServiceInterfaceUrl + "/dataCollection/romDownloadInfo.jhtml";

	public static string RESUCE_FAILED_UPLOAD = Configurations.ServiceInterfaceUrl + "/dataCollection/uploadFile.jhtml";

	public static string FEEDBACK_NO_TRANSLATE = Configurations.ServiceInterfaceUrl + "/dataCollection/untranslatedSentences.jhtml";

	public static string USER_BEHAVIOR_COLLECTION = Configurations.ServiceInterfaceUrl + "/dataCollection/addUserBehavior.jhtml";

	public static string UPLOAD_RESCUE_TOOL_LOG = Configurations.ServiceInterfaceUrl + "/dataCollection/nativeToolLog.jhtml";

	public static string COLLECTION_RESCUE_SUCCESS_LOG_UPLOAD = Configurations.ServiceInterfaceUrl + "/dataCollection/rescueSuccessLog.jhtml";

	public static string COLLECTION_ASSISTANTAPP = Configurations.ServiceInterfaceUrl + "/dataCollection/assistantApp.jhtml";

	public static string MOLI_INFO = Configurations.ServiceInterfaceUrl + "/moli/moliAndLena.jhtml";

	public static string CHECK_MA_VERSION = Configurations.ServiceInterfaceUrl + "/apk/download.jhtml";

	public static string FORMAT_LENOVOID_ACCOUNT = "https://passport.lenovo.com/interserver/authen/1.2/getaccountid?lpsust={0}&realm=lmsaclient";
}
