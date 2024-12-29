using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Download;

namespace lenovo.mbg.service.framework.smartbase;

internal class Smart
{
    protected static IBase bases = Base.instance;

    public static IBase SmartBase => Base.instance;

    public static IFileDownload FileDownloadV6 => SmartBase.Load<IFileDownload>("lenovo.mbg.service.framework.resources.FileDownloadManagerV6");

    public static IDeviceOperator DeviceOperator => bases.LoadNew<IDeviceOperator>("lenovo.mbg.service.framework.devicemgt.AdbOperator");

    public static IDeviceOperator FastBootOperator => bases.LoadNew<IDeviceOperator>("lenovo.mbg.service.framework.devicemgt.FastbootOperator");

    public static IRsd Rsd => bases.Load<IRsd>("lenovo.mbg.service.framework.resources.Rsd");

    public static AbstractDeviceConnectionManagerEx DeviceManagerEx => bases.Load<AbstractDeviceConnectionManagerEx>("lenovo.mbg.service.framework.devicemgt.DeviceConnectionManagerEx");

    public static IHost Host => bases.Load<IHost>("lenovo.mbg.service.framework.hostcontroller.PluginViewOfHost");

    public static IHostNavigation HostNavigation => bases.Load<IHostNavigation>("Software Fix.hostnavgationservice");

    public static IResourcesLoggingService ResourcesLoggingService => bases.Load<IResourcesLoggingService>("Software Fix.ResourcesLoggingService");

    public static IConfigService ConfigService => bases.Load<IConfigService>("Software Fix.ConfigService");

    public static IHostOperationService HostOperationService => bases.Load<IHostOperationService>("Software Fix.HostOperationService");

    public static ILanguage LanguageService => bases.Load<ILanguage>("Software Fix.LanguageHelper");

    public static IPermission PermissionService => bases.Load<IPermission>("software fix.permissionservice");

    public static IGoogleAnalyticsTracker GoogleAnalyticsTracker => bases.Load<IGoogleAnalyticsTracker>("Software Fix.GoogleAnalyticsTracker");

    public static IViewContext ViewContext => bases.Load<IViewContext>("Software Fix.ViewContext");

    public static IUser User => bases.Load<IUser>("Software Fix.User");

    public static IGlobalCache GlobalCache => bases.Load<IGlobalCache>("software fix.globalcacheservice");

    public static IUserBehaviorService BehaviorService => bases.Load<IUserBehaviorService>("Software Fix.UserBehaviorService");
}