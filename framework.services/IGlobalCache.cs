namespace lenovo.mbg.service.framework.services;

public interface IGlobalCache
{
	object Get(string key);

	object AddOrUpdate(string key, object value);
}
