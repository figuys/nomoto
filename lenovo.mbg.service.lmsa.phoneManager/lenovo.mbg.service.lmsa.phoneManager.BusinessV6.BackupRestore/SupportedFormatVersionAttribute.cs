using System;

namespace lenovo.mbg.service.lmsa.phoneManager.BusinessV6.BackupRestore;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class SupportedFormatVersionAttribute : Attribute
{
	public string Version { get; set; }

	public SupportedFormatVersionAttribute(string version)
	{
		Version = version;
	}
}
