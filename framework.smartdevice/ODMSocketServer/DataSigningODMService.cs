using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.ODMSocketServer;

public class DataSigningODMService : RestService
{
	public struct DataSigningODMInput
	{
		public string NewImei { get; private set; }

		public string MascId { get; private set; }

		public string ClientIp { get; private set; }

		public string ClientReqType { get; private set; }

		public string RsdLogId { get; private set; }

		public string ProdId { get; private set; }

		public string KeyType { get; private set; }

		public string KeyName { get; private set; }

		public string Data { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.newIMEI = NewImei;
				val.mascid = MascId;
				val.clientIP = ClientIp;
				val.clientReqType = ClientReqType;
				val.rsd_log_id = RsdLogId;
				val.prod_id = ProdId;
				val.type = KeyType;
				val.keyname = KeyName;
				val.data = Data;
				IDictionary<string, object> dictionary = val;
				foreach (string item in new List<string>(dictionary.Keys))
				{
					if (dictionary[item] == null)
					{
						dictionary.Remove(item);
					}
				}
				return val;
			}
		}

		public DataSigningODMInput(string newImei, string mascId, string clientIp, string clientReqType, string rsdLogId, string prodId, string keyType, string keyName, string data)
		{
			this = default(DataSigningODMInput);
			NewImei = newImei;
			MascId = mascId;
			ClientIp = clientIp;
			ClientReqType = clientReqType;
			RsdLogId = rsdLogId;
			ProdId = prodId;
			KeyType = keyType;
			KeyName = keyName;
			Data = data;
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> item in (IDictionary<string, object>)Fields)
			{
				dictionary[item.Key] = item.Value.ToString();
			}
			return Convert.ToString(GetType().Name, dictionary.ToList());
		}

		public override bool Equals(object obj)
		{
			return ToString().Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}

	public struct DataSigningODMOutput
	{
		private string TAG => GetType().FullName;

		public string ResponseCode { get; private set; }

		public string ResponseMessage { get; private set; }

		public string ReturnedData { get; private set; }

		public DataSigningODMOutput(string responseCode, string responseMessage, string returnedData)
		{
			this = default(DataSigningODMOutput);
			ResponseCode = responseCode;
			ResponseMessage = responseMessage;
			ReturnedData = returnedData;
		}

		public static DataSigningODMOutput FromDictionary(dynamic fields)
		{
			string responseCode = fields.responseCode;
			string responseMessage = fields.responseMsg;
			string returnedData = fields.msg;
			return new DataSigningODMOutput(responseCode, responseMessage, returnedData);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ResponseCode"] = ResponseCode;
			dictionary["ResponseMessage"] = ResponseMessage;
			dictionary["ReturnedData"] = ReturnedData;
			return Convert.ToString(GetType().Name, dictionary.ToList());
		}

		public override bool Equals(object obj)
		{
			return ToString().Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}

	protected override NetworkCredential Credential => new NetworkCredential(Login.UserName, Login.Password);

	public DataSigningODMService()
	{
		base.Url = "https://ebiz-esb.cloud.motorola.net/callsimunlockservice";
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public DataSigningODMOutput SignData(DataSigningODMInput input)
	{
		LogHelper.LogInstance.Debug("Contacting DataSigningODMService");
		LogHelper.LogInstance.Debug("UserName: " + Login.UserName + ", Password: " + Login.Password);
		LogHelper.LogInstance.Debug(JsonHelper.SerializeObject2FormatJson(input.Fields));
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		DataSigningODMOutput result = DataSigningODMOutput.FromDictionary(val);
		LogHelper.LogInstance.Debug("DataSigningODMService request completed");
		LogHelper.LogInstance.Debug(result.ToString());
		return result;
	}
}
