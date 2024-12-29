using System;

namespace lenovo.mbg.service.common.utilities;

public class ProcessEventArgs : EventArgs
{
	private string content = string.Empty;

	public string Content => content;

	public ProcessEventArgs()
	{
	}

	public ProcessEventArgs(string content)
	{
		this.content = content;
	}
}
