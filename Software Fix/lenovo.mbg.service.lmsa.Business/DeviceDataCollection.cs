using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.mbg.service.lmsa.Login.Business;
using lenovo.mbg.service.lmsa.ModelV6;
using lenovo.mbg.service.lmsa.ViewModels;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.Business;

public class DeviceDataCollection : IDisposable
{
	private string _Json_File_Name = Configurations.DefaultOptionsFileName;

	private string _Json_Property_Key = "devices";

	public static DeviceDataCollection Instance { get; private set; }

	private List<UserDeviceModel> _CacheLocalDevices { get; set; }

	public event Func<DeviceModel, bool> OnNewDeviceFound;

	public event EventHandler<UserDeviceModel> OnDevicesChanged;

	public void Initialize()
	{
		global::Smart.DeviceManagerEx.MasterDeviceChanged += MasterDeviceChangedHandler;
		UserService.Single.OnlineUserChanged += OnlineUserChangedHandler;
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void MasterDeviceChangedHandler(object sender, MasterDeviceChangedEventArgs e)
	{
		if (e.Current != null)
		{
			e.Current.SoftStatusChanged += Current_SoftStatusChanged;
			if ("985c66acdde2483ed96844a6b5ea4337" == HostProxy.HostNavigation.CurrentPluginID)
			{
				DeviceViewModel selectedItem = DeviceConnectViewModel.Instance.ConnectedDevices.FirstOrDefault((DeviceViewModel p) => p.Device.Identifer == e.Current.Identifer);
				DeviceConnectViewModel.Instance.SelectedItem = selectedItem;
			}
		}
		if (e.Previous != null)
		{
			e.Previous.SoftStatusChanged -= Current_SoftStatusChanged;
		}
	}

	private void Current_SoftStatusChanged(object sender, DeviceSoftStateEx e)
	{
		if (e == DeviceSoftStateEx.Online)
		{
			DeviceEx deviceEx = (DeviceEx)sender;
			if (MainWindowViewModel.SingleInstance.PluginArr.Count((PluginModel n) => n.Plugin != null && n.Plugin.IsExecuteWork()) == 0 && CheckUserValidity())
			{
				DeviceModel device = Convert(deviceEx.Property);
				WriteDeviceToFile(device, UserService.Single.CurrentLoggedInUser.UserId);
			}
		}
	}

	public List<DeviceModel> GetUserDevices()
	{
		return _CacheLocalDevices.FirstOrDefault((UserDeviceModel n) => n.UserID == UserService.Single.CurrentLoggedInUser.UserId)?.Devices;
	}

	public void Remove(DeviceModel device)
	{
		UserDeviceModel userDeviceModel = _CacheLocalDevices.FirstOrDefault((UserDeviceModel n) => n.UserID == UserService.Single.CurrentLoggedInUser.UserId);
		if (userDeviceModel == null || userDeviceModel.Devices == null || !userDeviceModel.Devices.Remove(device))
		{
			return;
		}
		FileHelper.WriteJsonWithAesEncrypt(_Json_File_Name, _Json_Property_Key, _CacheLocalDevices, async: true);
		if (!string.IsNullOrEmpty(device.id))
		{
			Task.Factory.StartNew(() => AppContext.WebApi.RequestContent(WebApiUrl.DELETE_USER_DEVICE, new Dictionary<string, List<int>> { 
			{
				"ids",
				new List<int> { int.Parse(device.id) }
			} }));
		}
	}

	public void Dispose()
	{
		_CacheLocalDevices.Clear();
		this.OnNewDeviceFound = null;
		this.OnDevicesChanged = null;
	}

	private void OnlineUserChangedHandler(object sender, OnlineUserChangedEventArgs e)
	{
		if (!e.IsOnline || e.UserInfo == null)
		{
			return;
		}
		if (_CacheLocalDevices == null)
		{
			GetLocalDevices();
		}
		UserDeviceModel userDeviceModel = _CacheLocalDevices.FirstOrDefault((UserDeviceModel n) => n.UserID == e.UserInfo.UserId);
		if (userDeviceModel == null)
		{
			userDeviceModel = new UserDeviceModel
			{
				UserID = e.UserInfo.UserId,
				Devices = new List<DeviceModel>()
			};
			_CacheLocalDevices.Add(userDeviceModel);
		}
		if (CheckUserValidity())
		{
			List<DeviceModel> devices = new List<DeviceModel>();
			if (userDeviceModel.Devices.Count > 0)
			{
				devices = userDeviceModel.Devices.Where((DeviceModel n) => !n.IsUpload).ToList();
			}
			UserDeviceModel userDeviceModel2 = new UserDeviceModel
			{
				UserID = e.UserInfo.UserId,
				Devices = devices
			};
			UserDeviceModel userDeviceModel3 = AppContext.WebApi.RequestContent<UserDeviceModel>(WebApiUrl.UPLOAD_USER_DEVICE, Convert(userDeviceModel2));
			if (userDeviceModel3 != null && userDeviceModel3.Devices != null && userDeviceModel3.Devices.Count > 0)
			{
				List<DeviceModel> list = new List<DeviceModel>();
				userDeviceModel3.Devices.ForEach(delegate(DeviceModel n)
				{
					n.IsUpload = true;
				});
				list.AddRange(userDeviceModel3.Devices);
				userDeviceModel.Devices.Clear();
				userDeviceModel.Devices.AddRange(list);
				FileHelper.WriteJsonWithAesEncrypt(_Json_File_Name, _Json_Property_Key, _CacheLocalDevices);
			}
			this.OnDevicesChanged?.Invoke(this, userDeviceModel);
		}
		else
		{
			userDeviceModel.Devices.Clear();
			FileHelper.WriteJsonWithAesEncrypt(_Json_File_Name, _Json_Property_Key, _CacheLocalDevices);
		}
	}

	private void WriteDeviceToFile(DeviceModel device, string userId)
	{
		UserDeviceModel userDeviceModel = null;
		if (_CacheLocalDevices != null && _CacheLocalDevices.Count > 0)
		{
			userDeviceModel = _CacheLocalDevices.FirstOrDefault((UserDeviceModel n) => n.UserID == userId);
		}
		bool flag = false;
		if (userDeviceModel == null)
		{
			flag = true;
			userDeviceModel = new UserDeviceModel
			{
				UserID = userId,
				Devices = new List<DeviceModel>()
			};
			_CacheLocalDevices.Add(userDeviceModel);
		}
		else if (!userDeviceModel.Devices.Exists((DeviceModel n) => n.Key == device.Key))
		{
			flag = true;
		}
		if (flag && this.OnNewDeviceFound != null && this.OnNewDeviceFound(device))
		{
			userDeviceModel.Devices.Add(device);
			FileHelper.WriteJsonWithAesEncrypt(_Json_File_Name, _Json_Property_Key, _CacheLocalDevices, async: true);
			this.OnDevicesChanged?.Invoke(this, userDeviceModel);
		}
	}

	private bool CheckUserValidity()
	{
		return UserService.Single.IsOnline && UserService.Single.CurrentLoggedInUser != null && !PermissionService.Single.CheckPermission(UserService.Single.CurrentLoggedInUser.UserId, "10", "1");
	}

	private DeviceModel Convert(IAndroidDevice device)
	{
		string text = null;
		if ("motorola".Equals(device.Brand, StringComparison.CurrentCultureIgnoreCase))
		{
			text = device.GetPropertyValue("ro.boot.hardware.sku");
			if (string.IsNullOrEmpty(text))
			{
				text = device.ModelName2;
			}
		}
		return new DeviceModel
		{
			ModelName = device.ModelName2,
			IMEI = device.IMEI1,
			IMEI2 = device.IMEI2,
			SN = device.SN,
			PN = device.PN,
			Brand = device.Brand,
			Category = device.Category,
			MotoModelName = text,
			Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
		};
	}

	private JObject Convert(UserDeviceModel userDeviceModel)
	{
		JObject jObject = new JObject();
		if (userDeviceModel != null)
		{
			JArray jarray = new JArray();
			jObject["registeredModels"] = jarray;
			if (userDeviceModel.Devices != null && userDeviceModel.Devices.Count > 0)
			{
				userDeviceModel.Devices.ForEach(delegate(DeviceModel n)
				{
					JObject item = new JObject
					{
						["modelName"] = n.ModelName,
						["imei"] = n.IMEI,
						["imei2"] = n.IMEI2,
						["sn"] = n.SN,
						["pn"] = n.PN,
						["registerDate"] = n.Date,
						["brand"] = n.Brand,
						["category"] = n.Category,
						["motoModelName"] = n.MotoModelName
					};
					jarray.Add(item);
				});
			}
		}
		return jObject;
	}

	public void GetLocalDevices()
	{
		string json = FileHelper.ReadWithAesDecrypt(_Json_File_Name, _Json_Property_Key);
		_CacheLocalDevices = JsonHelper.DeserializeJson2List<UserDeviceModel>(json);
		if (_CacheLocalDevices == null)
		{
			_CacheLocalDevices = new List<UserDeviceModel>();
		}
	}
}
