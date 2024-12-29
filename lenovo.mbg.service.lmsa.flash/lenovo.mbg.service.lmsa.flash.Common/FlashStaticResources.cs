using System.Runtime.InteropServices;

namespace lenovo.mbg.service.lmsa.flash.Common;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct FlashStaticResources
{
	public static string RESUCE_FASTBOOT_DEVICE_CONFIRM = "K0112";

	public static string RESUCE_AUTO_MATCH_FASTBOOT_DEVICE_CONFIRM = "K1070";

	public static string FASTBOOT_MODELNAME_UNMATCH = "K0170";

	public static string RESUCE_FASTBOOT_CONDITION_UNMATCHED = "K0098";

	public static string RESUCE_FASTBOOT_CONDITION_UNMATCHED_EX = "K0830";

	public static string RESUCE_CONDITION_UNMATCHED_EX = "K0943";

	public static string DEVICE_NOT_SUPPORT = "K0113";

	public static string RESCUE_DOWNLOAD_FAILED = "K0324";

	public static string AUTO_DEVICECOUNT_CHECK = "K0962";

	public static string MANUAL_DEVICECOUNT_CHECK = "K0942";

	public static string MANUAL_EMULATOR_DEVICECOUNT_CHECK = "K0820";

	public static string FLASH_INFORMATION_COLLECTION_CONFIRM_TITLE = "K0939";

	public static string FLASH_INFORMATION_COLLECTION_CONFIRM_CONTENT = "K0940";

	public static string FLASH_LESSSFREESPACE = "K0110";

	public static string RESCUE_BACKUP_FILE_TITLE = "K0108";

	public static string RESCUE_BACKUP_FILE_CONTENT = "K0107";

	public static string COUNTRYCODE_SETTING_SUCCESS_TITLE = "K0106";

	public static string COUNTRYCODE_SETTING_FAILED_TITLE = "K0105";

	public static string RESCUE_SUCCESS_TITLE = "K0111";

	public static string FASTBOOT_AUTOMATCH_FAILED_FIRST = "K0935";

	public static string FASTBOOT_AUTOMATCH_FAILED_GREATER_FIRST = "K0937";

	public static string FASTBOOT_MANUALMATCH_FAILED_FIRST = "K0935";

	public static string FASTBOOT_MANUALMATCH_FAILED_GREATER_FIRST = "K0937";

	public static string IMEI_MATCH_TITLE_FORMAT = "K0936";

	public static string IMEI_MATCH_FORMAT = "K0952";

	public static string IMEI_MATCH_FAILED_TITLE = "K0933";

	public static string IMEI_INVALIDATE_TITLE = "IMEI invalid";

	public static string IMEI_MATCH_FAILED_CONTENT = "K0934";

	public static string IMEI_INVALIDATE_CONTENT = "K0979";

	public static string USBDEVICE_AUTOMATCH_FAILED_TITLE = "K0933";

	public static string USBDEVICE_AUTOMATCH_FAILED_CONTENT = "K1046";

	public static string AUTO_MATCH_CONTENT = "K1045";

	public static string MODELNAME_NULL_TITLE = "K1093";

	public static string MODELNAME_NULL_CONTENT = "K1092";

	public static string DRIVER_INSTALL_CONFIRM = "K1094";
}
