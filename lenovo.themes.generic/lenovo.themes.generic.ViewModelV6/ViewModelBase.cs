using System;
using System.Windows.Controls;
using lenovo.mbg.service.framework.services;

namespace lenovo.themes.generic.ViewModelV6;

public class ViewModelBase : NotifyBase, IViewModelBase, IDisposable
{
	private UserControl messageView;

	public virtual bool DataIsLoaded { get; private set; }

	public UserControl MessageView
	{
		get
		{
			return messageView;
		}
		set
		{
			messageView = value;
			OnPropertyChanged("MessageView");
		}
	}

	public bool IsActived { get; private set; }

	public virtual void Dispose()
	{
		Reset();
	}

	public virtual void LoadData()
	{
		DataIsLoaded = true;
	}

	public virtual void LoadData(object data)
	{
		LoadData();
	}

	public virtual void Reset()
	{
		DataIsLoaded = false;
	}

	public virtual void Active(bool actived)
	{
		IsActived = actived;
	}

	public void UnActivate()
	{
	}
}
