using System;
using System.Threading;
using System.Windows.Controls;

namespace lenovo.themes.generic;

public interface IMessageViewV6
{
	UserControl View { get; }

	Action<bool?> CloseAction { get; set; }

	AutoResetEvent WaitHandler { get; }

	bool? Result { get; set; }
}
