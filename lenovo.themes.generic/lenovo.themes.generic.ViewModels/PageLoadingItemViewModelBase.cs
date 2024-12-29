using lenovo.themes.generic.ViewModelV6;

namespace lenovo.themes.generic.ViewModels;

public class PageLoadingItemViewModelBase<TId> : ViewModelBase
{
	public long DataVersion { get; set; }

	public bool IdIsLoaded { get; protected internal set; }

	public bool BasicPropertyIsLoaded { get; set; }

	public bool BigPropertyIsLoaded { get; set; }

	public object Tag { get; set; }

	public TId Id { get; set; }

	public PageLoadingItemViewModelBase()
	{
	}

	public PageLoadingItemViewModelBase(TId id)
	{
		Id = id;
	}
}
