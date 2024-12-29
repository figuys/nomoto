using System.Windows;

namespace lenovo.mbg.service.lmsa.backuprestore.Common;

public class ResourcesHelper
{
	public class StringResources
	{
		private static StringResources _singleInstance;

		public readonly string PC_SPACE_NOT_ENOUGH = "The target device doesn't have enough space. Release some and try again.";

		public readonly string CONTACT_DELETE_CONTACT_CONTENT = string.Empty;

		public readonly string CONTACT_DELETE_CALLLOG_CONTENT = string.Empty;

		public readonly string CONTACT_DELETE_SMS_CONTENT = string.Empty;

		public readonly string CONTACT_DELETE_TITLE = string.Empty;

		public readonly string MUSIC_PLAY_WARN = string.Empty;

		public readonly string CONTACT_ADD_WARN_CONTENT = string.Empty;

		public readonly string CONTACT_EMPUTY_WARN_CONTENT = string.Empty;

		public readonly string UNINSTALL_CONFIRM_TITLE = string.Empty;

		public readonly string UNINSTALL_APP_CONTENT = string.Empty;

		public readonly string UNINSTALL_SINGAL_APP_CONTENT = string.Empty;

		public readonly string MUSIC_DELETE_CONTENT = string.Empty;

		public readonly string PIC_DELETE_CONTENT = string.Empty;

		public readonly string SMS_DELETE_CONTENT = string.Empty;

		public readonly string VIDEO_DELETE_CONTENT = string.Empty;

		public readonly string APP_INSTALL_TITLE = string.Empty;

		public readonly string APP_UNINSTALL_TITLE = string.Empty;

		public readonly string APP_INSTALL_CONTENT = string.Empty;

		public readonly string APP_UNINSTALL_CONTENT = string.Empty;

		public readonly string PIC_CONTENT = string.Empty;

		public readonly string PIC_IMPORT_MESSAGE = string.Empty;

		public readonly string PIC_EXPORT_MESSAGE = string.Empty;

		public readonly string MUSIC_CONTENT = string.Empty;

		public readonly string MUSIC_IMPORT_MESSAGE = string.Empty;

		public readonly string MUSIC_EXPORT_MESSAGE = string.Empty;

		public readonly string APP_CONTENT = string.Empty;

		public readonly string APP_IMPORT_MESSAGE = string.Empty;

		public readonly string APP_IMPORT_SUCCESS = string.Empty;

		public readonly string APP_IMPORT_FAILED = string.Empty;

		public readonly string APP_EXPORT_MESSAGE = string.Empty;

		public readonly string APP_EXPORT_SUCCESS = string.Empty;

		public readonly string APP_UNINSTALL_MESSAGE = string.Empty;

		public readonly string VIDEO_CONTENT = string.Empty;

		public readonly string VIDEO_IMPORT_MESSAGE = string.Empty;

		public readonly string VIDEO_EXPORT_MESSAGE = string.Empty;

		public readonly string SMS_CONTENT = string.Empty;

		public readonly string SMS_IMPORT_MESSAGE = string.Empty;

		public readonly string SMS_EXPORT_MESSAGE = string.Empty;

		public readonly string SMS_SEND_FINISHED_MESSAGE = string.Empty;

		public readonly string SMS_DELETE_MESSAGE = string.Empty;

		public readonly string SMS_SENDING_MESSAGE = string.Empty;

		public readonly string BACKUPRESTORE_RELEASE_CONTENT = string.Empty;

		public readonly string BACKUPRESTORE_RELEASE_MESSAGE_NORMAL = string.Empty;

		public readonly string BACKUPRESTORE_RELEASE_MESSAGE_SUCCESS = string.Empty;

		public readonly string BACKUPRESTORE_RELEASE_MESSAGE_FAILED = string.Empty;

		public readonly string IMPORT_SUCCESS_MESSAGE = string.Empty;

		public readonly string IMPORT_FAILED_MESSAGE = string.Empty;

		public readonly string EXPORT_SUCCESS_MESSAGE = string.Empty;

		public readonly string EXPORT_FAILED_MESSAGE = string.Empty;

		public readonly string DELETE_SUCCESS_MESSAGE = string.Empty;

		public readonly string UNINSTALL_SUCCESS_MESSAGE = string.Empty;

		public readonly string NOT_ENOUGH_SAPCE = string.Empty;

		public readonly string FILE_NOT_EXISTS = string.Empty;

		public readonly string ProgressButtonContent_Cancel = string.Empty;

		public readonly string ProgressButtonContent_Finish = string.Empty;

		public readonly string ProgressButtonContent_Failed = string.Empty;

		public readonly string ProgressButtonContent_Ok = string.Empty;

		public readonly string ProgressTipsContent_Backup_Normal = string.Empty;

		public readonly string ProgressTipsContent_Backup_Finish_First = string.Empty;

		public readonly string ProgressTipsContent_Backup_Finish_End = string.Empty;

		public readonly string ProgressTipsContent_Backup_Failed = string.Empty;

		public readonly string ProgressTipsContent_Restore_Normal = string.Empty;

		public readonly string ProgressTipsContent_Restore_Finish_First = string.Empty;

		public readonly string ProgressTipsContent_Restore_Finish_End = string.Empty;

		public readonly string ProgressTipsContent_Restore_Failed = string.Empty;

		public readonly string CONFIRMDELETE_TITLE = string.Empty;

		public readonly string CONFIRMDELETE_CONTENT = string.Empty;

		public static StringResources SingleInstance
		{
			get
			{
				if (_singleInstance == null)
				{
					_singleInstance = new StringResources();
				}
				return _singleInstance;
			}
		}

		private StringResources()
		{
			_ = Application.Current;
			CONTACT_DELETE_CONTACT_CONTENT = "K0534";
			CONTACT_DELETE_CALLLOG_CONTENT = "K0540";
			CONTACT_DELETE_SMS_CONTENT = "K0587";
			CONTACT_DELETE_TITLE = "K0585";
			MUSIC_PLAY_WARN = "K0673";
			CONTACT_ADD_WARN_CONTENT = "Input a complete birth date";
			CONTACT_EMPUTY_WARN_CONTENT = "K0523";
			UNINSTALL_CONFIRM_TITLE = "K0490";
			UNINSTALL_APP_CONTENT = "K0492";
			UNINSTALL_SINGAL_APP_CONTENT = "K0491";
			MUSIC_DELETE_CONTENT = "K0556";
			PIC_DELETE_CONTENT = "K0563";
			SMS_DELETE_CONTENT = "Delete selected SMS?";
			VIDEO_DELETE_CONTENT = "K0569";
			APP_INSTALL_TITLE = "K0498";
			APP_UNINSTALL_TITLE = "K0493";
			APP_INSTALL_CONTENT = "K0501";
			APP_UNINSTALL_CONTENT = "K0502";
			PIC_CONTENT = "Pictures";
			PIC_IMPORT_MESSAGE = "K0560";
			PIC_EXPORT_MESSAGE = "K0561";
			MUSIC_CONTENT = "Songs";
			MUSIC_IMPORT_MESSAGE = "K0554";
			MUSIC_EXPORT_MESSAGE = "K0555";
			APP_CONTENT = "Apps";
			APP_IMPORT_MESSAGE = "K0497";
			APP_IMPORT_SUCCESS = "K0499";
			APP_IMPORT_FAILED = "K0709";
			APP_EXPORT_MESSAGE = "K0500";
			APP_EXPORT_SUCCESS = "K0542";
			APP_UNINSTALL_MESSAGE = "K0494";
			VIDEO_CONTENT = "Videos";
			VIDEO_IMPORT_MESSAGE = "K0566";
			VIDEO_EXPORT_MESSAGE = "K0567";
			SMS_CONTENT = "SMS";
			SMS_IMPORT_MESSAGE = "Importing messages...";
			SMS_EXPORT_MESSAGE = "K0588";
			SMS_DELETE_MESSAGE = "K0584";
			SMS_SEND_FINISHED_MESSAGE = "K0589";
			SMS_SENDING_MESSAGE = "K0590";
			BACKUPRESTORE_RELEASE_CONTENT = "K0607";
			BACKUPRESTORE_RELEASE_MESSAGE_NORMAL = "K0606";
			BACKUPRESTORE_RELEASE_MESSAGE_SUCCESS = "K0608";
			BACKUPRESTORE_RELEASE_MESSAGE_FAILED = "K0609";
			IMPORT_SUCCESS_MESSAGE = "K0543";
			IMPORT_FAILED_MESSAGE = "K0679";
			EXPORT_SUCCESS_MESSAGE = "K0542";
			EXPORT_FAILED_MESSAGE = "K0653";
			DELETE_SUCCESS_MESSAGE = "K0682";
			UNINSTALL_SUCCESS_MESSAGE = "K0495";
			NOT_ENOUGH_SAPCE = "K0623";
			FILE_NOT_EXISTS = "The picture doesn't exist in your phone any more";
			ProgressButtonContent_Cancel = "K0208";
			ProgressButtonContent_Finish = "K0386";
			ProgressButtonContent_Failed = "K0337";
			ProgressButtonContent_Ok = "K0327";
			ProgressTipsContent_Backup_Normal = "K0600";
			ProgressTipsContent_Backup_Finish_First = "K0601";
			ProgressTipsContent_Backup_Finish_End = "K0544";
			ProgressTipsContent_Backup_Failed = "K0617";
			ProgressTipsContent_Restore_Normal = "K0613";
			ProgressTipsContent_Restore_Finish_First = "K0614";
			ProgressTipsContent_Restore_Finish_End = "K0602";
			ProgressTipsContent_Restore_Failed = "K0618";
			CONFIRMDELETE_TITLE = "K0208";
			CONFIRMDELETE_CONTENT = "K0541";
		}
	}

	public class SymbolResources
	{
		public static string LeftBrackets => "(";

		public static string RightBrackets => ")";

		public static string ForwardSlash => "/";
	}

	public class ColorString
	{
		public const string COLOR_FF00000 = "#FF0000";

		public const string COLOR_43B5E2 = "#43B5E2";
	}
}
