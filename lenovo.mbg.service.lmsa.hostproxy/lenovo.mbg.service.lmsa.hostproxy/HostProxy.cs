using System.Windows;
using System.Windows.Threading;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Download;
using lenovo.mbg.service.framework.socket;
using lenovo.themes.generic.Component;
using lenovo.themes.generic.Component.Progress;

namespace lenovo.mbg.service.lmsa.hostproxy;

public class HostProxy
{
	protected static HostMaskLayerWrapper hostMaskLayerWrapper = new HostMaskLayerWrapper(HostOperationService);

	protected static AsyncCommonProgressLoader asyncCommonProgressLoader = new AsyncCommonProgressLoader(CurrentDispatcher, HostMaskLayerWrapper);

	public static AbstractDeviceConnectionManagerEx deviceManager => global::Smart.DeviceManagerEx;

	public static IFileDownload DownloadServerV6 => global::Smart.FileDownloadV6;

	public static IHostNavigation HostNavigation => global::Smart.HostNavigation;

	public static ISequence Sequence => lenovo.mbg.service.framework.socket.Sequence.SingleInstance;

	public static IHost Host => global::Smart.Host;

	public static IResourcesLoggingService ResourcesLoggingService => global::Smart.ResourcesLoggingService;

	public static IConfigService ConfigService => global::Smart.ConfigService;

	public static IHostOperationService HostOperationService => global::Smart.HostOperationService;

	public static HostMaskLayerWrapper HostMaskLayerWrapper => hostMaskLayerWrapper;

	public static ILanguage LanguageService => global::Smart.LanguageService;

	public static IGoogleAnalyticsTracker GoogleAnalyticsTracker => global::Smart.GoogleAnalyticsTracker;

	public static AsyncCommonProgressLoader AsyncCommonProgressLoader => asyncCommonProgressLoader;

	public static Dispatcher CurrentDispatcher => Application.Current?.Dispatcher;

	public static IUser User => global::Smart.User;

	public static IViewContext ViewContext => global::Smart.ViewContext;

	public static IPermission PermissionService => global::Smart.PermissionService;

	public static IGlobalCache GlobalCache => global::Smart.GlobalCache;

	public static IUserBehaviorService BehaviorService => global::Smart.BehaviorService;

	private HostProxy()
	{
	}
}
