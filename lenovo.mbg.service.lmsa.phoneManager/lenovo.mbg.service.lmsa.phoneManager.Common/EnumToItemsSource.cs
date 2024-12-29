using System;
using System.Linq;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.phoneManager.Common;

public class EnumToItemsSource : MarkupExtension
{
	private readonly Type _type;

	public EnumToItemsSource(Type type)
	{
		_type = type;
	}

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		if (HostProxy.LanguageService.IsNeedTranslate())
		{
			return from object n in Enum.GetValues(_type)
				select new
				{
					ID = (int)n,
					Name = HostProxy.LanguageService.Translate(n.ToString())
				};
		}
		return from object e in Enum.GetValues(_type)
			select new
			{
				ID = (int)e,
				Name = e.ToString()
			};
	}
}
