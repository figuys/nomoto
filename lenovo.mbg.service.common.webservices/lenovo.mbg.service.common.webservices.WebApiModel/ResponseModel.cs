using System;

namespace lenovo.mbg.service.common.webservices.WebApiModel;

[Serializable]
public class ResponseModel<T>
{
	public bool success { get; set; }

	public string code { get; set; }

	public string desc { get; set; }

	public T content { get; set; }
}
