namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class CommandLine
{
	public int id { get; set; }

	public string command { get; private set; }

	public int timeout { get; private set; }

	public long size { get; private set; }

	public CommandLine(int id, string command, int timeout, long size)
	{
		this.id = id;
		this.command = command;
		this.timeout = timeout;
		this.size = size;
	}
}
