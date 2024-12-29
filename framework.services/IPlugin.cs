using System.Windows;

namespace lenovo.mbg.service.framework.services;

public interface IPlugin
{
	void Init();

	FrameworkElement CreateControl(IMessageBox iMsg);

	bool CanClose();

	bool IsExecuteWork();

	void OnSelected(string val);

	void OnSelecting(string val);

	void OnInit(object data);
}
