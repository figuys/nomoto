using System;
using System.Windows;

namespace lenovo.mbg.service.framework.services;

public abstract class PluginBase : IPlugin
{
	public static ILanguage LangHelper;

	public abstract FrameworkElement CreateControl(IMessageBox iMessage);

	public virtual object GetService(Type serviceType)
	{
		if (serviceType.IsAssignableFrom(GetType()))
		{
			return this;
		}
		return null;
	}

	public virtual void Dispose()
	{
	}

	public virtual bool CanClose()
	{
		return true;
	}

	public virtual bool IsExecuteWork()
	{
		return false;
	}

	public virtual void OnSelected(string val)
	{
	}

	public void OnSelecting(string val)
	{
	}

	public abstract void Init();

	public virtual void OnInit(object data)
	{
	}
}
