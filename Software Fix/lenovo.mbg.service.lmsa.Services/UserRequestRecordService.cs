using System.Linq;
using System.Threading.Tasks;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices.WebApiModel;
using Newtonsoft.Json.Linq;

namespace lenovo.mbg.service.lmsa.Services;

public class UserRequestRecordService
{
	public void UploadAsync()
	{
		Task.Factory.StartNew(delegate
		{
			JArray jArray = FileHelper.ReadJtokenWithAesDecrypt<JArray>(Configurations.UserRequestRecordsFile, "$.content", isDateAsStr: true);
			if (jArray != null && jArray.Count != 0)
			{
				int num = jArray.Count;
				JToken jToken = null;
				do
				{
					jToken = jArray.Skip(--num).FirstOrDefault();
					if (jToken != null)
					{
						string text = jToken.Value<string>("url");
						if (string.IsNullOrEmpty(text))
						{
							jToken.Remove();
							FileHelper.WriteJsonWithAesEncrypt(Configurations.UserRequestRecordsFile, "content", jArray, async: true);
						}
						else
						{
							JArray jArray2 = jToken.Value<JArray>("datas");
							if (jArray2 != null && jArray2.Count > 0)
							{
								int num2 = jArray2.Count;
								do
								{
									JToken jToken2 = jArray2.First();
									ResponseModel<object> responseModel = AppContext.WebApi.RequestBase(text, jToken2);
									if (responseModel.code == "0000")
									{
										jToken2.Remove();
										FileHelper.WriteJsonWithAesEncrypt(Configurations.UserRequestRecordsFile, "content", jArray, async: true);
									}
								}
								while (--num2 > 0);
							}
							if (jArray2 == null || jArray2.Count == 0)
							{
								jToken.Remove();
								FileHelper.WriteJsonWithAesEncrypt(Configurations.UserRequestRecordsFile, "content", jArray, async: true);
							}
						}
					}
				}
				while (num > 0);
			}
		});
	}
}
