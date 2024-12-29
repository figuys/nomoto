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

	public static AbstractDeviceConnectionManagerEx deviceManager => global::lenovo.mbg.service.lmsa.hostproxy.Smart.DeviceManagerEx;

	public static IFileDownload DownloadServerV6 => global::lenovo.mbg.service.lmsa.hostproxy.Smart.FileDownloadV6;

	public static IHostNavigation HostNavigation => global::lenovo.mbg.service.lmsa.hostproxy.Smart.HostNavigation;

	public static ISequence Sequence => lenovo.mbg.service.framework.socket.Sequence.SingleInstance;

	public static IHost Host => global::lenovo.mbg.service.lmsa.hostproxy.Smart.Host;

	public static IResourcesLoggingService ResourcesLoggingService => global::lenovo.mbg.service.lmsa.hostproxy.Smart.ResourcesLoggingService;

	public static IConfigService ConfigService => global::lenovo.mbg.service.lmsa.hostproxy.Smart.ConfigService;

	public static IHostOperationService HostOperationService => global::lenovo.mbg.service.lmsa.hostproxy.Smart.HostOperationService;

	public static HostMaskLayerWrapper HostMaskLayerWrapper => hostMaskLayerWrapper;

	public static ILanguage LanguageService => global::lenovo.mbg.service.lmsa.hostproxy.Smart.LanguageService;

	public static IGoogleAnalyticsTracker GoogleAnalyticsTracker => global::lenovo.mbg.service.lmsa.hostproxy.Smart.GoogleAnalyticsTracker;

	public static AsyncCommonProgressLoader AsyncCommonProgressLoader => asyncCommonProgressLoader;

	public static Dispatcher CurrentDispatcher => Application.Current?.Dispatcher;

	public static IUser User => global::lenovo.mbg.service.lmsa.hostproxy.Smart.User;

	public static IViewContext ViewContext => global::lenovo.mbg.service.lmsa.hostproxy.Smart.ViewContext;

	public static IPermission PermissionService => global::lenovo.mbg.service.lmsa.hostproxy.Smart.PermissionService;

	public static IGlobalCache GlobalCache => global::lenovo.mbg.service.lmsa.hostproxy.Smart.GlobalCache;

	public static IUserBehaviorService BehaviorService => global::lenovo.mbg.service.lmsa.hostproxy.Smart.BehaviorService;

	private HostProxy()
	{
	}
}
