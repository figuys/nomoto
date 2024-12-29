using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace lenovo.mbg.service.common.utilities;

public class CustomConvert
{
	private const string MEID_IMEI = "^(99)([0-9]{12,13})$";

	private const string IMEI = "^[0-9]{14,15}$";

	private const string MEID = "^[0-9A-F]{14,15}$";

	private const string MSN = "^[0-9A-Z]{10}$";

	private const string HSN = "^[0-9A-Z]{11}$";

	private static CustomConvert _instance;

	public static CustomConvert Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			return _instance = new CustomConvert();
		}
	}

	public string BytesToHex(byte[] bytes)
	{
		return BitConverter.ToString(bytes).Replace("-", string.Empty);
	}

	public byte[] HexToBytes(string hex)
	{
		int length = hex.Length;
		byte[] array = new byte[length / 2];
		for (int i = 0; i < length; i += 2)
		{
			array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
		}
		return array;
	}

	public byte[] LongToBytes(long value)
	{
		return ValueToBytes(value, 8);
	}

	public byte[] IntToBytes(int value)
	{
		return ValueToBytes((uint)value, 4);
	}

	public byte[] UShortToBytes(ushort value)
	{
		return ValueToBytes(value, 2);
	}

	public long BytesToLong(byte[] value)
	{
		return BytesToValue(value, 8);
	}

	public int BytesToInt(byte[] value)
	{
		return (int)BytesToValue(value, 4);
	}

	public ushort BytesToUShort(byte[] value)
	{
		return (ushort)BytesToValue(value, 2);
	}

	private byte[] ValueToBytes(long value, int expectedBytes)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes);
		}
		for (int i = 0; i < bytes.Length - expectedBytes; i++)
		{
			if (bytes[i] != 0)
			{
				throw new OverflowException($"Value size of {value} is larger than {expectedBytes} bytes");
			}
		}
		byte[] array = new byte[expectedBytes];
		Array.Copy(bytes, bytes.Length - expectedBytes, array, 0, expectedBytes);
		return array;
	}

	private long BytesToValue(byte[] value, int expectedBytes)
	{
		if (value.Length > expectedBytes)
		{
			for (int i = 0; i < value.Length - expectedBytes; i++)
			{
				if (value[i] != 0)
				{
					throw new OverflowException($"Value size of {value.Length} is larger than {expectedBytes} bytes");
				}
			}
		}
		else if (value.Length < expectedBytes)
		{
			expectedBytes = value.Length;
		}
		byte[] array = new byte[8];
		Array.Copy(value, value.Length - expectedBytes, array, array.Length - expectedBytes, expectedBytes);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToInt64(array, 0);
	}

	public byte[] AsciiToBytes(string asciiString)
	{
		return Encoding.UTF8.GetBytes(asciiString);
	}

	public string BytesToAscii(byte[] asciiBytes)
	{
		return Encoding.UTF8.GetString(asciiBytes);
	}

	public byte[] Base64ToBytes(string base64)
	{
		return Convert.FromBase64String(base64);
	}

	public string BytesToBase64(byte[] bytes)
	{
		return Convert.ToBase64String(bytes);
	}

	public string LongToBase26(long value)
	{
		int num = 65;
		List<char> list = new List<char>();
		do
		{
			char item = (char)((int)(value % 26) + num);
			list.Insert(0, item);
			value /= 26;
		}
		while (value >= 1);
		return new string(list.ToArray());
	}

	public long Base26ToLong(string base26)
	{
		int num = 65;
		long num2 = 0L;
		for (int i = 0; i < base26.Length; i++)
		{
			int num3 = base26[base26.Length - 1 - i];
			num3 -= num;
			int num4 = (int)Math.Pow(26.0, i);
			num2 += num3 * num4;
		}
		return num2;
	}

	public byte[] StreamToBytes(Stream stream)
	{
		using MemoryStream memoryStream = new MemoryStream();
		MemoryStream memoryStream2 = memoryStream;
		if (typeof(MemoryStream).IsAssignableFrom(stream.GetType()))
		{
			memoryStream2 = (MemoryStream)stream;
		}
		else
		{
			CustomFile.Instance.CopyStream(stream, memoryStream2);
		}
		return memoryStream2.ToArray();
	}

	public Stream BytesToStream(byte[] bytes)
	{
		return new MemoryStream(bytes);
	}

	public List<EnumType> EnumToValues<EnumType>() where EnumType : struct, IConvertible
	{
		List<EnumType> list = new List<EnumType>();
		foreach (object value in Enum.GetValues(typeof(EnumType)))
		{
			list.Add((EnumType)value);
		}
		return list;
	}

	public string TimeSpanToDisplay(TimeSpan time)
	{
		string text = " years";
		string text2 = " year";
		string text3 = " days";
		string text4 = " day";
		string text5 = " hours";
		string text6 = " hour";
		string text7 = " minutes";
		string text8 = " minute";
		string text9 = " seconds";
		string text10 = " second";
		string text11 = " milliseconds";
		string text12 = " millisecond";
		int num = time.Days / 365;
		int num2 = time.Days % 365;
		string text13 = string.Empty;
		if (num > 0)
		{
			text13 += num;
			text13 = ((num <= 1) ? (text13 + text2) : (text13 + text));
			text13 += " ";
		}
		if (num2 > 0)
		{
			text13 += num2;
			text13 = ((num2 <= 1) ? (text13 + text4) : (text13 + text3));
			text13 += " ";
		}
		if (time.Hours > 0 && num < 1)
		{
			text13 += time.Hours;
			text13 = ((time.Hours <= 1) ? (text13 + text6) : (text13 + text5));
			text13 += " ";
		}
		if (time.Minutes > 0 && time.TotalDays < 1.0)
		{
			text13 += time.Minutes;
			text13 = ((time.Minutes <= 1) ? (text13 + text8) : (text13 + text7));
			text13 += " ";
		}
		if (time.Seconds > 0 && time.TotalMinutes < 5.0)
		{
			text13 += time.Seconds;
			text13 = ((time.Seconds <= 1) ? (text13 + text10) : (text13 + text9));
			text13 += " ";
		}
		if (time.TotalSeconds < 1.0)
		{
			text13 += time.Milliseconds;
			text13 = ((time.Milliseconds <= 1) ? (text13 + text12) : (text13 + text11));
			text13 += " ";
		}
		return text13.TrimEnd();
	}

	public int TwosComplement(int data)
	{
		if (data > 32768)
		{
			data -= 65536;
		}
		return data;
	}

	public byte[] ByteSwap(byte[] bytes)
	{
		byte[] array = new byte[bytes.Length];
		Array.Copy(bytes, array, bytes.Length);
		Array.Reverse(array);
		return array;
	}

	public string ToString(string name, IEnumerable<KeyValuePair<string, object>> fields)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(name);
		foreach (KeyValuePair<string, object> field in fields)
		{
			stringBuilder.Append("    ");
			stringBuilder.Append(field.Key);
			stringBuilder.Append(": ");
			if (field.Value != null)
			{
				stringBuilder.AppendLine(field.Value.ToString());
			}
			else
			{
				stringBuilder.AppendLine("[NULL]");
			}
		}
		return stringBuilder.ToString();
	}

	public string ToCommaSeparated(IEnumerable list)
	{
		string text = string.Empty;
		foreach (object item in list)
		{
			text = text + item.ToString() + ",";
		}
		return text.TrimEnd(',');
	}

	public string ToStatusText(string message, double percentageComplete)
	{
		string result = message;
		if (percentageComplete > 0.0 && percentageComplete < 100.0)
		{
			int num = (int)Math.Round(percentageComplete);
			result = $"{message} {num}% complete";
		}
		return result;
	}

	public bool IsSerialNumberValid(string serialNumber)
	{
		if (ToSerialNumberType(serialNumber) == SerialNumberType.Unknown)
		{
			return false;
		}
		return serialNumber.ToLowerInvariant() == CalculateCheckDigit(serialNumber).ToLowerInvariant();
	}

	public string CalculateCheckDigit(string serialNumber)
	{
		serialNumber = serialNumber.Trim();
		SerialNumberType serialNumberType = ToSerialNumberType(serialNumber);
		if (serialNumberType == SerialNumberType.Imei || RegMatch(serialNumber, "^(99)([0-9]{12,13})$"))
		{
			string text = serialNumber;
			if (text.Length > 14)
			{
				text = text.Substring(0, 14);
			}
			int num = 0;
			List<char> list = new List<char>();
			for (int i = 1; i < text.Length; i += 2)
			{
				string text2 = (int.Parse(text[i].ToString()) * 2).ToString();
				list.AddRange(text2.ToCharArray());
			}
			for (int j = 0; j < text.Length; j += 2)
			{
				list.Add(text[j]);
			}
			foreach (char item3 in list)
			{
				num += int.Parse(item3.ToString());
			}
			num %= 10;
			if (num != 0)
			{
				num = 10 - num;
			}
			return text + num;
		}
		switch (serialNumberType)
		{
		case SerialNumberType.Meid:
		{
			char[] array = serialNumber.ToCharArray(0, 14);
			List<char> list2 = new List<char>();
			for (int k = 1; k <= array.Length; k++)
			{
				char item = array[k - 1];
				if (k % 2 == 0)
				{
					byte b = byte.Parse(item.ToString(), NumberStyles.HexNumber);
					char[] array2 = ((byte)(b + b)).ToString("X").ToCharArray();
					foreach (char item2 in array2)
					{
						list2.Add(item2);
					}
				}
				else
				{
					list2.Add(item);
				}
			}
			int num2 = 0;
			foreach (char item4 in list2)
			{
				num2 += byte.Parse(item4.ToString(), NumberStyles.HexNumber);
			}
			int num3 = num2 % 16;
			if (num3 != 0)
			{
				num3 = 16 - num3;
			}
			string text3 = num3.ToString("X");
			return new string(array) + text3;
		}
		case SerialNumberType.Msn:
		case SerialNumberType.Hsn:
			return serialNumber;
		default:
			throw new NotSupportedException("Check digit calculation not supported for " + serialNumberType.ToString() + " ('" + serialNumber + "')");
		}
	}

	public SerialNumberType ToSerialNumberType(string serialNumber)
	{
		if (RegMatch(serialNumber, "^(99)([0-9]{12,13})$"))
		{
			return SerialNumberType.Meid;
		}
		if (RegMatch(serialNumber, "^[0-9]{14,15}$"))
		{
			return SerialNumberType.Imei;
		}
		if (RegMatch(serialNumber, "^[0-9A-F]{14,15}$"))
		{
			return SerialNumberType.Meid;
		}
		if (RegMatch(serialNumber, "^[0-9A-Z]{10}$"))
		{
			return SerialNumberType.Msn;
		}
		if (RegMatch(serialNumber, "^[0-9A-Z]{11}$"))
		{
			return SerialNumberType.Hsn;
		}
		return SerialNumberType.Unknown;
	}

	private bool RegMatch(string input, string pattern)
	{
		return Regex.IsMatch(input, pattern);
	}

	public string ByteArrayToString(byte[] ba)
	{
		StringBuilder stringBuilder = new StringBuilder(ba.Length * 2);
		foreach (byte b in ba)
		{
			stringBuilder.AppendFormat("{0:x2}", b);
		}
		return stringBuilder.ToString();
	}

	public byte[] StringToByteArray(string hex)
	{
		return (from x in Enumerable.Range(0, hex.Length)
			where x % 2 == 0
			select Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
	}
}
