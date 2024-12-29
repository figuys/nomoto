using System.Windows;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.tips;

[PluginExport(typeof(IPlugin), "992e746537954a7d9ae613d5ec9bc7a6")]
public class Plugin : PluginBase
{
	private TipsFrame tipsOnlineFrame;

	private const string screenName = "lmsa-plugin-Tips";

	public override void Init()
	{
	}

	public override FrameworkElement CreateControl(IMessageBox iMessage)
	{
		tipsOnlineFrame = new TipsFrame();
		return tipsOnlineFrame;
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override bool CanClose()
	{
		return false;
	}

	public override bool IsExecuteWork()
	{
		return false;
	}
}
