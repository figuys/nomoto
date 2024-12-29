using System.Collections.Generic;

namespace lenovo.mbg.service.framework.services.Device;

public interface IAndroidDevice
{
	int ApiLevel { get; }

	string IMEI1 { get; }

	string IMEI2 { get; }

	string ModelId { get; }

	string SN { get; }

	string PN { get; }

	string ModelName { get; }

	string ModelName2 { get; }

	string HWCode { get; }

	string Brand { get; }

	string Processor { get; }

	string Uptime { get; }

	string RomVersion { get; }

	string CountryCode { get; }

	string Carrier { get; }

	string FingerPrint { get; }

	string FsgVersion { get; }

	string AndroidVersion { get; }

	double BatteryQuantityPercentage { get; }

	string InternalStoragePath { get; }

	string ExternalStoragePath { get; }

	string TotalExternalStorageWithUnit { get; }

	string UsedExternalStorageWithUnit { get; }

	string FreeExternalStorageWithUnit { get; }

	string TotalInternalStorageWithUnit { get; }

	string UsedInternalStorageWithUnit { get; }

	string FreeInternalStorageWithUnit { get; }

	long TotalExternalStorage { get; }

	long UsedExternalStorage { get; }

	long FreeExternalStorage { get; }

	long TotalInternalStorage { get; }

	long UsedInternalStorage { get; }

	long FreeInternalStorage { get; }

	string Operator { get; }

	string OtaModel { get; }

	string RoHardWare { get; }

	string CustomerVersion { get; }

	string Category { get; }

	int SimCount { get; }

	Dictionary<string, string> Others { get; }

	string GetPropertyValue(string name);
}
