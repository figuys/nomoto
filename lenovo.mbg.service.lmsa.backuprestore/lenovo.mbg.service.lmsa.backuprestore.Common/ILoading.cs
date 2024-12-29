namespace lenovo.mbg.service.lmsa.backuprestore.Common;

public interface ILoading
{
	void Show(object handler);

	void Hiden(object handler);

	void Abort();
}
