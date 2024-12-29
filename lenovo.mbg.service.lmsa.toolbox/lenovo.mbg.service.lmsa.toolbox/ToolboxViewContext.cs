using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ControlsV6;

namespace lenovo.mbg.service.lmsa.toolbox;

public class ToolboxViewContext
{
	private static ToolboxViewContext _singleInstance;

	private IHost _host;

	private DeviceTutorialsDialogViewV6 m_DeviceTutorialsViewV6;

	private volatile bool m_MessageBoxShow;

	public static ToolboxViewContext SingleInstance
	{
		get
		{
			if (_singleInstance == null)
			{
				_singleInstance = new ToolboxViewContext();
			}
			return _singleInstance;
		}
	}

	public IHost Host
	{
		get
		{
			return _host;
		}
		set
		{
			_host = value;
		}
	}

	public IHostOperationService HostOperationService { get; private set; }

	public ILanguage LanuageService { get; private set; }

	public IMessageBox MessageBox { get; set; }

	public void Initialize()
	{
		Host = HostProxy.Host;
		HostOperationService = HostProxy.HostOperationService;
		LanuageService = HostProxy.LanguageService;
	}

	public void ShowTutorialsView()
	{
		if (!m_MessageBoxShow && !(HostProxy.HostNavigation.CurrentPluginID != "dd537b5c6c074ae49cc8b0b2965ce54a"))
		{
			m_MessageBoxShow = true;
			m_DeviceTutorialsViewV6 = new DeviceTutorialsDialogViewV6();
			m_DeviceTutorialsViewV6.Closed += delegate
			{
				m_MessageBoxShow = false;
			};
			MessageBox.ShowMessage(m_DeviceTutorialsViewV6);
		}
	}

	public void CloseTutorialsView()
	{
		m_MessageBoxShow = false;
		m_DeviceTutorialsViewV6?.CloseAction(false);
		m_DeviceTutorialsViewV6?.Close();
		MessageBox.UnFrozenWindow();
	}
}
