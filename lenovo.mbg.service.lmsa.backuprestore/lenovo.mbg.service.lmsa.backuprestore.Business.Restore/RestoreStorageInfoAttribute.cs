using System;

namespace lenovo.mbg.service.lmsa.backuprestore.Business.Restore;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class RestoreStorageInfoAttribute : Attribute
{
	public string ResourceType { get; private set; }

	public string StorageFileVersion { get; private set; }

	public RestoreStorageInfoAttribute(string resourceType, string storageFileVersion)
	{
		ResourceType = resourceType;
		StorageFileVersion = storageFileVersion;
	}
}
