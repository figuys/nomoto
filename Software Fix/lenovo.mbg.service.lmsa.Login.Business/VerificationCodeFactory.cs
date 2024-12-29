using System;
using System.Collections.Generic;
using System.Linq;

namespace lenovo.mbg.service.lmsa.Login.Business;

public sealed class VerificationCodeFactory
{
	public static char[] PredefinedExceptChars = new char[3] { 'O', 'o', '\0' };

	public static List<string> CeateNewCode(int targetLength, char[] exceptChars)
	{
		List<string> list = new List<string>(targetLength);
		Random random = new Random(DateTime.Now.Millisecond);
		int num = 0;
		int num2 = 0;
		char value = ' ';
		while (num < targetLength)
		{
			switch (random.Next(0, 3))
			{
			case 0:
				value = (char)random.Next(48, 58);
				break;
			case 1:
				value = (char)random.Next(65, 91);
				break;
			case 2:
				value = (char)random.Next(97, 123);
				break;
			}
			if (exceptChars != null && !exceptChars.Contains(value))
			{
				num++;
				list.Add(value.ToString());
			}
		}
		return list;
	}
}
