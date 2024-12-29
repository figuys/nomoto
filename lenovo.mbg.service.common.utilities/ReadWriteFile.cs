using System.Collections.Generic;

namespace lenovo.mbg.service.common.utilities;

public class ReadWriteFile
{
	private string _filePath;

	public ReadWriteFile(string filePath)
	{
		_filePath = filePath;
	}

	public dynamic Read()
	{
		if (!GlobalFun.Exists(_filePath))
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["fileversion"] = 1.0;
			Write(dictionary);
		}
		return JsonHelper.DeserializeJson2ObjcetFromFile<object>(_filePath);
	}

	public void Write(dynamic content)
	{
		JsonHelper.SerializeObject2File(_filePath, content);
	}
}
