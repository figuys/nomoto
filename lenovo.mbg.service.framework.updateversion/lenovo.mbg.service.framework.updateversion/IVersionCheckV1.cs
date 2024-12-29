namespace lenovo.mbg.service.framework.updateversion;

public interface IVersionCheckV1 : IVersionEvent
{
	IVersionDataV1 Data { get; }

	void Check();

	void CheckAsync();
}
