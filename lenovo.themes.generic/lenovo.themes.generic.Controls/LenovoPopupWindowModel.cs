using System;
using System.Windows.Controls;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.Controls;

public class LenovoPopupWindowModel
{
	public ViewModelBase ViewModel { get; protected set; }

	public ControlTemplate ContentControlTemplate { get; protected set; }

	public LenovoPopupWindowModel()
	{
		ViewModel = GetDefaultViewModel();
		ContentControlTemplate = GetDefaultControlTemplate();
	}

	public T GetViewModel<T>() where T : ViewModelBase
	{
		return (T)ViewModel;
	}

	public void SetViewModel(ViewModelBase viewModel)
	{
		ViewModel = viewModel;
	}

	protected virtual ViewModelBase GetDefaultViewModel()
	{
		return new ViewModelBase();
	}

	public void SetContentControlTemplate(ControlTemplate template)
	{
		ContentControlTemplate = template;
	}

	protected virtual ControlTemplate GetDefaultControlTemplate()
	{
		return new ControlTemplate
		{
			TargetType = typeof(UserControl)
		};
	}

	public virtual LenovoPopupWindow CreateWindow()
	{
		if (ViewModel == null)
		{
			throw new Exception("View model is null");
		}
		if (ContentControlTemplate == null)
		{
			throw new Exception("Control template is null");
		}
		Type type = ContentControlTemplate.TargetType;
		if (type == null)
		{
			type = typeof(UserControl);
		}
		Control control = Activator.CreateInstance(type) as Control;
		control.Template = ContentControlTemplate;
		control.DataContext = ViewModel;
		return new LenovoPopupWindow
		{
			Content = control,
			WindowModel = this
		};
	}
}
