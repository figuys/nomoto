namespace LmsaWindowsService.Contracts;

public interface IPipeMessageWorker
{
	void Do(object data);
}
