using System;
using System.Security.Cryptography;
using System.Text;

namespace lenovo.mbg.service.common.log;

public class LogAesEncrypt : ILogEncrypt
{
	public virtual string Encrypt(string content)
	{
		if (string.IsNullOrEmpty(content))
		{
			return string.Empty;
		}
		return Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(content), null, DataProtectionScope.LocalMachine));
	}
}
