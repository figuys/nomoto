using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace lenovo.mbg.service.common.utilities;

public class XmlSerializeHelper
{
	public static object Deserialize<T>(string xml)
	{
		try
		{
			object obj = null;
			XmlLocker.Instance.ThreadSafeOperation(delegate
			{
				try
				{
					using StringReader textReader = new StringReader(xml);
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					obj = xmlSerializer.Deserialize(textReader);
				}
				catch
				{
				}
			});
			return obj;
		}
		catch
		{
			return null;
		}
	}

	public static object Deserialize<T>(Stream stream)
	{
		object result = null;
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		try
		{
			result = xmlSerializer.Deserialize(stream);
		}
		catch
		{
		}
		return result;
	}

	public static object DeserializeFromFile<T>(string fileName)
	{
		return DeserializeFromFile<T>(fileName, FileShare.None);
	}

	public static object DeserializeFromFile<T>(string fileName, FileShare fs)
	{
		if (!File.Exists(fileName))
		{
			return null;
		}
		object obj = null;
		XmlLocker.Instance.ThreadSafeOperation(delegate
		{
			try
			{
				using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, fs);
				obj = Deserialize<T>(stream);
			}
			catch
			{
			}
		});
		return obj;
	}

	public static string Serializer<T>(object obj)
	{
		if (obj != null)
		{
			string str = string.Empty;
			XmlLocker.Instance.ThreadSafeOperation(delegate
			{
				MemoryStream memoryStream = new MemoryStream();
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				try
				{
					xmlSerializer.Serialize(memoryStream, obj);
					memoryStream.Position = 0L;
					StreamReader streamReader = new StreamReader(memoryStream);
					str = streamReader.ReadToEnd();
					streamReader.Dispose();
					memoryStream.Dispose();
				}
				catch (InvalidOperationException)
				{
					throw;
				}
			});
			return str;
		}
		return string.Empty;
	}

	public static void Serializer<T>(string fileName, object obj, FileShare fs)
	{
		if (obj == null)
		{
			return;
		}
		XmlLocker.Instance.ThreadSafeOperation(delegate
		{
			if (File.Exists(fileName))
			{
				try
				{
					File.Delete(fileName);
				}
				catch (DirectoryNotFoundException ex)
				{
					throw ex;
				}
				catch (UnauthorizedAccessException ex2)
				{
					throw ex2;
				}
				catch (NotSupportedException ex3)
				{
					throw ex3;
				}
				catch (IOException ex4)
				{
					throw ex4;
				}
				catch (Exception ex5)
				{
					throw ex5;
				}
			}
			try
			{
				string directoryName = Path.GetDirectoryName(fileName);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				using FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, fs);
				new XmlSerializer(typeof(T)).Serialize(stream, obj);
				using StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8, 2048);
				streamWriter.Flush();
			}
			catch (Exception ex6)
			{
				throw ex6;
			}
		});
	}

	public static void Serializer<T>(string fileName, object obj)
	{
		Serializer<T>(fileName, obj, FileShare.None);
	}
}
