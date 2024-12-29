using System;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiServices;
using lenovo.mbg.service.framework.updateversion.model;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.framework.updateversion.impl;

public class VersionDataV1Impl : IVersionDataV1, IVersionEvent
{
	protected static ApiService service = new ApiService();

	public object Data { get; private set; }

	public event EventHandler<VersionV1EventArgs> OnVersionEvent;

	public virtual object Get()
	{
		Data = null;
		object obj = service.RequestContent(WebApiUrl.CLIENT_VERSION, null);
		if (obj == null)
		{
			this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_GETDATA_FAILED), null, null);
		}
		else
		{
			JObject jObject = obj as JObject;
			Data = new VersionModel(jObject.Value<string>("clientVersion"), jObject.Value<string>("filePath"), jObject.Value<bool>("increment"), jObject.Value<bool>("forceUpdate"), jObject.Value<string>("releaseNotes"), GlobalFun.ConvertDateTime(jObject.Value<long>("releaseDate")));
			this.OnVersionEvent?.BeginInvoke(this, new VersionV1EventArgs(VersionV1Status.VERSION_GETDATA_SUCCESS, Data), null, null);
		}
		return Data;
	}
}
