using System.Windows;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.lmsa.forum;

[PluginExport(typeof(IPlugin), "310f47ad70d54880b33225d864e6fe68")]
public class Plugin : PluginBase
{
	private const string screenName = "lmsa-plugin-Forum";

	private ForumFrame _ForumFrame { get; set; }

	public override void Init()
	{
	}

	public override FrameworkElement CreateControl(IMessageBox iMessage)
	{
		_ForumFrame = new ForumFrame();
		return _ForumFrame;
	}

	public void GoToUrl(string key)
	{
		_ForumFrame?.GoToTargetUrl(key);
	}
}
