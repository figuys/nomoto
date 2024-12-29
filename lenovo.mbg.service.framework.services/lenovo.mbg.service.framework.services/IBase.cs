using System;

namespace lenovo.mbg.service.framework.services;

public interface IBase : IDisposable
{
	InterfaceType Load<InterfaceType>(string typeName);

	InterfaceType LoadCached<InterfaceType>();

	InterfaceType LoadNew<InterfaceType>(string typeName);
}
