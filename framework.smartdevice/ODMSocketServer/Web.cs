using System.Net;

namespace lenovo.mbg.service.framework.smartdevice.ODMSocketServer;

public class Web
{
	private static DataSigningODMService dataSignOdm = new DataSigningODMService();

	public static string DataSignODM(string newImei, string logId, string clientReqType, string prodId, string keyType, string keyName, string data, string usename, string password, string mascId)
	{
		dataSignOdm.Login = new Login(usename, password);
		string clientIp = "99.99.99.99";
		DataSigningODMService.DataSigningODMInput input = new DataSigningODMService.DataSigningODMInput(newImei, mascId, clientIp, clientReqType, logId, prodId, keyType, keyName, data);
		DataSigningODMService.DataSigningODMOutput dataSigningODMOutput = dataSignOdm.SignData(input);
		if (dataSigningODMOutput.ResponseCode != "0")
		{
			throw new WebException($"Invalid response code '{dataSigningODMOutput.ResponseCode}' from ODM Data Signing service: {dataSigningODMOutput.ResponseMessage}");
		}
		return dataSigningODMOutput.ReturnedData;
	}
}
