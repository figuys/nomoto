using System;
using System.Collections.Generic;
using System.Linq;
using lenovo.mbg.service.framework.services;
using lenovo.mbg.service.framework.services.Device;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;

namespace lenovo.mbg.service.lmsa.Services;

public class UserBehaviorService : IUserBehaviorService
{
	private object lockobj = new object();

	protected string user;

	private Dictionary<BusinessType, BehaviorModel> Cache { get; set; }

	protected DeviceEx Device
	{
		get
		{
			DeviceEx masterDevice = HostProxy.deviceManager.MasterDevice;
			if (masterDevice == null)
			{
				IList<DeviceEx> conntectedDevices = HostProxy.deviceManager.ConntectedDevices;
				if (conntectedDevices != null && conntectedDevices.Count > 0)
				{
					return conntectedDevices.FirstOrDefault((DeviceEx n) => n.SoftStatus == DeviceSoftStateEx.Online);
				}
			}
			return masterDevice;
		}
	}

	public void Collect(BusinessType business, BusinessData data)
	{
		lock (lockobj)
		{
			if (data == null)
			{
				data = new BusinessData(business, Device);
			}
			if (Cache == null)
			{
				Cache = new Dictionary<BusinessType, BehaviorModel>();
			}
			if (Cache.ContainsKey(business))
			{
				Cache[business].count++;
				if (data != null)
				{
					Cache[business].data.Add(data);
				}
				return;
			}
			Cache.Add(business, new BehaviorModel
			{
				user = user,
				business = business,
				businessName = business.ToString(),
				count = 1,
				dateTime = DateTime.Now.ToString("yyyyMMdd"),
				data = new List<BusinessData> { data }
			});
		}
	}

	public List<BehaviorModel> GetAll()
	{
		lock (lockobj)
		{
			if (Cache == null || Cache.Count == 0)
			{
				return null;
			}
			List<BehaviorModel> list = new List<BehaviorModel>();
			foreach (BehaviorModel value in Cache.Values)
			{
				list.Add(value);
			}
			return list;
		}
	}

	public void InitUser(string user)
	{
		this.user = user;
	}
}
