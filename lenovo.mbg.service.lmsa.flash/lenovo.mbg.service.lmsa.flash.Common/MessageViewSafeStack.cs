using System.Threading.Tasks;

namespace lenovo.mbg.service.lmsa.flash.Common;

public class MessageViewSafeStack
{
	public Task<bool?> Show()
	{
		return Task.Run(() => (bool?)null);
	}
}
