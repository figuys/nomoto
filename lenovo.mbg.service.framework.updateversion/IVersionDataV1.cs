namespace lenovo.mbg.service.framework.updateversion;

public interface IVersionDataV1 : IVersionEvent
{
	object Data { get; }

	object Get();
}
