using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.support.Contract;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.support.Business;

public class IBaseWarrantyConverter
{
	private class IBaseWarrantyData
	{
		[JsonProperty("machineInfo")]
		public IBaseWarrantyMachineInfo MachineInfo { get; set; }

		[JsonProperty("serviceInfoList")]
		public List<IBaseWarrantyServiceItemInfo> ServiceInfo { get; set; }

		[JsonProperty("aodInfoList")]
		public List<IBaseWarrantyAodItemInfo> AodInfoList { get; set; }

		[JsonProperty("upmaList")]
		public List<IBaseWarrantyUpmaItemInfo> UpmaList { get; set; }
	}

	public static IBaseWarrantyInfo Convert(string xmlContent)
	{
		IBaseWarrantyInfo baseWarrantyInfo = null;
		XmlDocument xmlDocument = new XmlDocument();
		try
		{
			xmlDocument.LoadXml(xmlContent);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Load xml throw exception:" + xmlContent);
			LogHelper.LogInstance.Error("Load xml throw exception:" + ex.ToString());
			return null;
		}
		XmlReader xmlReader = XmlReader.Create(new StringReader(xmlContent));
		while (xmlReader.Read())
		{
			if (!xmlReader.IsStartElement() || !"wiOutputForm".Equals(xmlReader.Name))
			{
				continue;
			}
			while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || !xmlReader.Name.Equals("wiOutputForm")))
			{
				if (!xmlReader.IsStartElement() || !"warrantyInfo".Equals(xmlReader.Name))
				{
					continue;
				}
				baseWarrantyInfo = IBaseWarrantyInfo.New();
				while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || !xmlReader.Name.Equals("warrantyInfo")))
				{
					if (!xmlReader.IsStartElement())
					{
						continue;
					}
					switch (xmlReader.Name)
					{
					case "aodInfoList":
					{
						List<IBaseWarrantyAodItemInfo> warrantyAodItems = baseWarrantyInfo.WarrantyAodInfo.WarrantyAodItems;
						while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || !xmlReader.Name.Equals("aodInfoList")))
						{
							if (!xmlReader.IsStartElement() || !(xmlReader.Name == "aodInfo"))
							{
								continue;
							}
							IBaseWarrantyAodItemInfo baseWarrantyAodItemInfo = new IBaseWarrantyAodItemInfo();
							warrantyAodItems.Add(baseWarrantyAodItemInfo);
							while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || !xmlReader.Name.Equals("aodInfo")))
							{
								if (!xmlReader.IsStartElement())
								{
									continue;
								}
								string name = xmlReader.Name;
								if (!(name == "aodDescription"))
								{
									if (name == "aodType")
									{
										baseWarrantyAodItemInfo.AodType = xmlReader.ReadString();
									}
								}
								else
								{
									baseWarrantyAodItemInfo.AodDescription = xmlReader.ReadString();
								}
							}
						}
						break;
					}
					case "upmaList":
					{
						List<IBaseWarrantyUpmaItemInfo> warrantyUpmaItems = baseWarrantyInfo.WarrantyUpmaInfo.WarrantyUpmaItems;
						while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || !xmlReader.Name.Equals("upmaList")))
						{
							if (!xmlReader.IsStartElement() || !(xmlReader.Name == "upma"))
							{
								continue;
							}
							IBaseWarrantyUpmaItemInfo baseWarrantyUpmaItemInfo = new IBaseWarrantyUpmaItemInfo();
							warrantyUpmaItems.Add(baseWarrantyUpmaItemInfo);
							while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || !xmlReader.Name.Equals("upma")))
							{
								if (xmlReader.IsStartElement())
								{
									switch (xmlReader.Name)
									{
									case "mStartDate":
										baseWarrantyUpmaItemInfo.StartDate = xmlReader.ReadString();
										break;
									case "mEndDate":
										baseWarrantyUpmaItemInfo.EndDate = xmlReader.ReadString();
										break;
									case "mSDF":
										baseWarrantyUpmaItemInfo.Sdf = xmlReader.ReadString();
										break;
									case "mSDFDesc":
										baseWarrantyUpmaItemInfo.SdfDesc = xmlReader.ReadString();
										break;
									case "mSDFType":
										baseWarrantyUpmaItemInfo.SdfType = xmlReader.ReadString();
										break;
									case "remainWtyCount":
										baseWarrantyUpmaItemInfo.RemainWtyCount = xmlReader.ReadString();
										break;
									}
								}
							}
						}
						break;
					}
					case "serviceInfo":
					{
						List<IBaseWarrantyServiceItemInfo> warrantyServiceItems = baseWarrantyInfo.WarrantyServiceInfo.WarrantyServiceItems;
						IBaseWarrantyServiceItemInfo baseWarrantyServiceItemInfo = null;
						while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || !xmlReader.Name.Equals("serviceInfo")))
						{
							if (!xmlReader.IsStartElement())
							{
								continue;
							}
							switch (xmlReader.Name)
							{
							case "channelid":
								baseWarrantyServiceItemInfo = new IBaseWarrantyServiceItemInfo();
								baseWarrantyServiceItemInfo.Channelid = xmlReader.ReadString();
								warrantyServiceItems.Add(baseWarrantyServiceItemInfo);
								break;
							case "countryCode":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.CountryCode = xmlReader.ReadString();
								}
								break;
							case "countryDesc":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.CountryDesc = xmlReader.ReadString();
								}
								break;
							case "deliveryNbr":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.DeliveryNbr = xmlReader.ReadString();
								}
								break;
							case "geo":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.Geo = xmlReader.ReadString();
								}
								break;
							case "wtyAttr":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.WtyAttr = xmlReader.ReadString();
								}
								break;
							case "sdf":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.Sdf = xmlReader.ReadString();
								}
								break;
							case "sdfDesc":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.SdfDesc = xmlReader.ReadString();
								}
								break;
							case "service_delivery_type":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.ServiceDeliveryType = xmlReader.ReadString();
								}
								break;
							case "shipDate":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.ShipDate = xmlReader.ReadString();
								}
								break;
							case "warstart":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.Warstart = xmlReader.ReadString();
								}
								break;
							case "wed":
								if (baseWarrantyServiceItemInfo != null)
								{
									baseWarrantyServiceItemInfo.Wed = xmlReader.ReadString();
								}
								break;
							}
						}
						break;
					}
					case "machineInfo":
					{
						IBaseWarrantyMachineInfo warrantyMachineInfo = baseWarrantyInfo.WarrantyMachineInfo;
						while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || !xmlReader.Name.Equals("machineInfo")))
						{
							if (xmlReader.IsStartElement())
							{
								switch (xmlReader.Name)
								{
								case "buildDate":
									warrantyMachineInfo.BuildDate = xmlReader.ReadString();
									break;
								case "model":
									warrantyMachineInfo.Model = xmlReader.ReadString();
									break;
								case "product":
									warrantyMachineInfo.Product = xmlReader.ReadString();
									break;
								case "productName":
									warrantyMachineInfo.ProductName = xmlReader.ReadString();
									break;
								case "serial":
									warrantyMachineInfo.Serial = xmlReader.ReadString();
									break;
								case "status":
									warrantyMachineInfo.Status = xmlReader.ReadString();
									break;
								case "type":
									warrantyMachineInfo.Type = xmlReader.ReadString();
									break;
								case "IMEI":
									warrantyMachineInfo.IMEI = xmlReader.ReadString();
									break;
								case "POP_DATE":
									warrantyMachineInfo.POPDate = xmlReader.ReadString();
									break;
								case "SWAPIMEI":
									warrantyMachineInfo.SWAPIMEI = xmlReader.ReadString();
									break;
								}
							}
						}
						break;
					}
					}
				}
			}
		}
		return baseWarrantyInfo;
	}

	public static IBaseWarrantyInfo ConvertEx(string JsonContent)
	{
		IBaseWarrantyData baseWarrantyData = JsonHelper.DeserializeJson2Object<IBaseWarrantyData>(JsonContent);
		if (baseWarrantyData == null)
		{
			return null;
		}
		IBaseWarrantyInfo info = IBaseWarrantyInfo.New();
		baseWarrantyData.AodInfoList?.ForEach(delegate(IBaseWarrantyAodItemInfo p)
		{
			info.WarrantyAodInfo.WarrantyAodItems.Add(p);
		});
		baseWarrantyData.UpmaList?.ForEach(delegate(IBaseWarrantyUpmaItemInfo p)
		{
			info.WarrantyUpmaInfo.WarrantyUpmaItems.Add(p);
		});
		baseWarrantyData.ServiceInfo?.ForEach(delegate(IBaseWarrantyServiceItemInfo p)
		{
			info.WarrantyServiceInfo.WarrantyServiceItems.Add(p);
		});
		if (baseWarrantyData.MachineInfo != null)
		{
			info.WarrantyMachineInfo = baseWarrantyData.MachineInfo;
		}
		return info;
	}
}
