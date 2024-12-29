using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using lenovo.mbg.service.framework.services.Device;

namespace lenovo.mbg.service.framework.smartdevice;

public class UseCaseDevice
{
	public DeviceEx Device { get; set; }

	public string Id { get; set; }

	public bool ManualDevice { get; set; }

	public bool Locked { get; set; }

	public bool RealFlash { get; set; }

	public bool RecipeLocked { get; set; }

	public RecipeResources Resources { get; set; }

	protected Func<RecipeMessageType, object, object> NotifyFunc { get; set; }

	public IRecipeMessage MessageBox { get; set; }

	public ResultLogger Log { get; set; }

	public string StartTime { get; set; }

	public AutoResetEvent EventHandle { get; set; }

	public UseCaseDevice(DeviceEx device, string id)
	{
		EventHandle = new AutoResetEvent(initialState: false);
		Device = ((device != null && device.ConnectType == ConnectType.Wifi) ? null : device);
		ManualDevice = Device == null;
		Id = id;
	}

	public void Register(RecipeResources resources, IRecipeMessage messagebox, Func<RecipeMessageType, object, object> notifyFunc)
	{
		Resources = resources;
		MessageBox = messagebox;
		NotifyFunc = notifyFunc;
	}

	public virtual Task<object> NotifyAsync(RecipeMessageType tag, object data)
	{
		return Task.Run(() => NotifyFunc(tag, data));
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder("Virtual device id: " + Id + ", Physical device id: " + Device?.Identifer);
		if (ManualDevice)
		{
			stringBuilder.AppendLine(",NOTE: This device was manually registered, no physical device was detected");
		}
		else
		{
			stringBuilder.AppendLine(",NOTE: Physical device detected");
		}
		return stringBuilder.ToString();
	}
}
