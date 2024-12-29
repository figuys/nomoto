using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa;

public interface ISingleInstanceApp
{
	bool SignalExternalCommandLineArgs(IList<string> args);
}
