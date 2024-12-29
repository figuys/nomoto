namespace lenovo.mbg.service.framework.services;

public interface IViewModelBase
{
	void LoadData();

	void LoadData(object data);

	void Reset();
}
