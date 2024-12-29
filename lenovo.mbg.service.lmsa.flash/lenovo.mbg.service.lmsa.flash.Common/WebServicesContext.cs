using System.Runtime.InteropServices;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.lmsa.flash.Common;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct WebServicesContext
{
	private static readonly string BaseUri = Configurations.ServiceInterfaceUrl;

	public static string GET_MARKETNAMES = BaseUri + "/rescueDevice/getRescueModelNames.jhtml";

	public static string GET_ALLMODELNAMES = BaseUri + "/rescueDevice/getModelNames.jhtml";

	public static string GET_FASTBOOTDATA_RECIPE = BaseUri + "/rescueDevice/getRescueModelRecipe.jhtml";

	public static string GET_MODELS_BY_MARKETNAME = BaseUri + "/rescueDevice/modelListByMarketName.jhtml";

	public static string RESUCE_AUTOMATCH_GETPARAMS_MAPPING = BaseUri + "/rescueDevice/getRomMatchParams.jhtml";

	public static string RESUCE_AUTOMATCH_GETROM = BaseUri + "/rescueDevice/getNewResource.jhtml";

	public static string RESUCE_MANUAL_GETROM = BaseUri + "/rescueDevice/getResource.jhtml";

	public static string RESUCE_MANUAL_GETSTEPTIPS = BaseUri + "/rescueDevice/getXamlList.jhtml";

	public static string UPLOAD_WHEN_FLASH_FINISHED = BaseUri + "/dataCollection/RescueflashInfo.jhtml";

	public static string UPLOAD_HW_DETECTION = BaseUri + "/dataCollection/HwDetectionInfo.jhtml";

	public static string GET_RESOURCES_BY_IMEI = BaseUri + "/rescueDevice/getNewResourceByImei.jhtml";

	public static string GET_RESOURCES_BY_SN = BaseUri + "/rescueDevice/getNewResourceBySN.jhtml";

	public static string COLLECTION_DOWNGRADE_INFO = BaseUri + "/dataCollection/rescueDegradeInfo.jhtml";

	public static string COLLECTION_SHARE_RESCUE_RESULT = BaseUri + "/dataCollection/rescueSocialShare.jhtml";

	public static string SURVEY_RESCUE_REASON = BaseUri + "/survey/saveRescueSuccessRecord.jhtml";

	public static string GET_B2B_ALL_ORDER_INFO = BaseUri + "/vip/getB2BInfo.jhtml";

	public static string GET_B2B_AVAILABLE_ORDER_INFO = BaseUri + "/vip/getB2BInfo.jhtml";
}
