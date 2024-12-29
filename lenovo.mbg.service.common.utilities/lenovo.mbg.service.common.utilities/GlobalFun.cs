using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;
using lenovo.mbg.service.common.log;
using Microsoft.Win32;

namespace lenovo.mbg.service.common.utilities;

public class GlobalFun
{
	private static List<string> PackageVersionList = new List<string>
	{
		"Rescue and Smart Assistant", "{1930BFD1-2F0A-43D9-B760-FAA2A40806DE}", "{21E28485-F3A3-4D3E-86A3-7E17C1BAAF42}", "{63BF7D1C-95B4-4E37-97A7-F8835B8A13D0}", "{3E34ECBE-D771-4D2B-8BDB-F4CF0BF663D1}", "{B72D19C8-EDA7-4C56-B8E8-EA3D35749BF4}", "{DB16BC9B-89C2-46A5-840F-79410CAB73B1}", "{C5E66E98-6776-477B-B0B3-B1A8372F8CC6}", "{4EE4FC82-5245-478B-BBD2-15E977072240}", "{4F7F8A4C-5D6E-4A4D-AD0B-8FF4964CA277}",
		"{A92D6B74-8528-4284-841F-76B3D9CE478B}", "{4226EB1C-E6C5-471F-88BC-5081A8D3FBD8}"
	};

	public static string GetMd5Hash(string pathName)
	{
		using MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		try
		{
			pathName = pathName.Replace("\"", "");
			byte[] array;
			using (FileStream inputStream = File.OpenRead(pathName))
			{
				array = mD5CryptoServiceProvider.ComputeHash(inputStream);
			}
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				stringBuilder.AppendFormat("{0:X2}", b);
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error calculating MD5 value for the specified file: " + ex.Message);
			return string.Empty;
		}
	}

	public static string GetStringMd5(string str, bool removeSplitter = false)
	{
		string text = string.Empty;
		using (MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
		{
			text = BitConverter.ToString(mD5CryptoServiceProvider.ComputeHash(new UTF8Encoding().GetBytes(str)));
		}
		if (removeSplitter && !string.IsNullOrEmpty(text))
		{
			return text.Replace("-", "").ToLower();
		}
		return text;
	}

	public static bool MD5Check(string path, string md5)
	{
		return md5.ToUpper().Equals(GetMd5Hash(path).ToUpper());
	}

	public static bool TryDeleteFile(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				File.SetAttributes(path, FileAttributes.Normal);
				File.Delete(path);
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool CopyDirectory(string srcDir, string tarDir)
	{
		try
		{
			if (!Directory.Exists(srcDir))
			{
				return true;
			}
			if (!Directory.Exists(tarDir))
			{
				Directory.CreateDirectory(tarDir);
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(srcDir);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				try
				{
					fileInfo.CopyTo(Path.Combine(tarDir, fileInfo.Name), overwrite: true);
				}
				catch
				{
					return false;
				}
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				if (!CopyDirectory(Path.Combine(srcDir, directoryInfo2.Name), Path.Combine(tarDir, directoryInfo2.Name)))
				{
					return false;
				}
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool DeleteFileInDirectory(string dirName)
	{
		try
		{
			if (!Directory.Exists(dirName))
			{
				return true;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(dirName);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				try
				{
					fileInfo.Delete();
				}
				catch
				{
					return false;
				}
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				if (!DeleteFileInDirectory(Path.Combine(dirName, directoryInfo2.Name)))
				{
					return false;
				}
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool CopyFile(string srcFile, string tarDir, out string resultInfo)
	{
		resultInfo = string.Empty;
		if (string.IsNullOrEmpty(srcFile) || string.IsNullOrEmpty(tarDir))
		{
			resultInfo = "File or directory string is empty.";
			return false;
		}
		FileStream fileStream = null;
		FileStream fileStream2 = null;
		try
		{
			int num = 512000;
			int num2 = 0;
			byte[] buffer = new byte[num];
			string fileName = Path.GetFileName(srcFile);
			string path = Path.Combine(tarDir, fileName);
			fileStream = File.Open(srcFile, FileMode.Open);
			fileStream2 = File.Open(path, FileMode.Append);
			fileStream.Position = fileStream2.Position;
			long num3 = 0L;
			long num4 = 0L;
			while (fileStream.Position < fileStream.Length)
			{
				num2 = fileStream.Read(buffer, 0, num);
				fileStream2.Write(buffer, 0, num2);
				fileStream2.Flush();
				num4 = fileStream2.Position * 100 / fileStream.Length;
				if (num3 != num4)
				{
					num3 = num4;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			resultInfo = ex.ToString();
			return false;
		}
		finally
		{
			fileStream?.Close();
			fileStream2?.Close();
		}
	}

	public static long GetFileLen(string fileName)
	{
		if (!File.Exists(fileName))
		{
			return 0L;
		}
		return new FileInfo(fileName).Length;
	}

	public static string GetRandomCode(int count)
	{
		string text = string.Empty;
		Random random = new Random((int)DateTime.Now.Ticks);
		for (int i = 0; i < count; i++)
		{
			int num = random.Next();
			num %= 36;
			num = ((num >= 10) ? (num + 55) : (num + 48));
			text += (char)num;
		}
		return text;
	}

	public static string GetRandomCodeEx(int count)
	{
		string[] array = "1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,i,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');
		string text = "";
		int num = -1;
		Random random = new Random();
		for (int i = 0; i < count; i++)
		{
			if (num != -1)
			{
				random = new Random(num * i * (int)DateTime.Now.Ticks);
			}
			int num2 = random.Next(array.Length - 1);
			while (num == num2)
			{
				num2 = random.Next(array.Length - 1);
			}
			num = num2;
			text += array[num2];
		}
		return text;
	}

	public static bool FileRename(string oldPathName, string newPathName)
	{
		try
		{
			new FileInfo(oldPathName).MoveTo(newPathName);
			if (File.Exists(oldPathName))
			{
				File.Delete(oldPathName);
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static string ConvertLong2String(long bytes, string format = "F1")
	{
		float num = bytes;
		if (bytes > 1000)
		{
			if (bytes >= 1024000)
			{
				if (bytes >= 1024000000)
				{
					return (num / 1.0737418E+09f).ToString(format) + " GB";
				}
				return (num / 1048576f).ToString(format) + " MB";
			}
			return (num / 1024f).ToString(format) + " KB";
		}
		return bytes + " B";
	}

	public static long ConvertString2Long(string size)
	{
		if (string.IsNullOrEmpty(size))
		{
			return 0L;
		}
		Match match = Regex.Match(size.Trim().Replace(',', '.'), "^(?<value>[\\d.]+)\\s*(?<unit>[A-Za-z]).*$");
		string value = match.Groups["value"].Value;
		string text = match.Groups["unit"].Value.ToLower();
		double result = 0.0;
		double.TryParse(value, out result);
		return text switch
		{
			"k" => (long)(result * 1024.0), 
			"m" => (long)(result * 1024.0 * 1024.0), 
			"g" => (long)(result * 1024.0 * 1024.0 * 1024.0), 
			"t" => (long)(result * 1024.0 * 1024.0 * 1024.0 * 1024.0), 
			_ => (long)result, 
		};
	}

	public static string GetStringFromUrl(string url)
	{
		WebRequest webRequest = null;
		WebResponse webResponse = null;
		Stream stream = null;
		StreamReader streamReader = null;
		string result = string.Empty;
		try
		{
			webRequest = WebRequest.Create(url);
			webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
			webResponse = webRequest.GetResponse();
			stream = webResponse.GetResponseStream();
			streamReader = new StreamReader(stream);
			result = streamReader.ReadToEnd();
		}
		catch (Exception)
		{
		}
		finally
		{
			streamReader?.Close();
			stream?.Close();
			webResponse?.Close();
			webRequest?.Abort();
		}
		return result;
	}

	public static bool GetStreamFormServer(string url, Action<Stream> action, string method = "GET", int timeout = 30000)
	{
		HttpWebRequest httpWebRequest = null;
		WebResponse webResponse = null;
		Stream stream = null;
		try
		{
			httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
			httpWebRequest.Method = method;
			httpWebRequest.UserAgent = GlobalVar.UserAgent;
			httpWebRequest.Timeout = timeout;
			httpWebRequest.KeepAlive = false;
			webResponse = httpWebRequest.GetResponse();
			stream = webResponse.GetResponseStream();
			action(stream);
			return true;
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error(ex.ToString());
			return false;
		}
		finally
		{
			stream?.Close();
			webResponse?.Close();
			httpWebRequest?.Abort();
		}
	}

	public static void OpenFileExplorer(string filePath)
	{
		try
		{
			if (Directory.Exists(filePath))
			{
				Process.Start("Explorer", "/e,\"" + filePath + "\"");
			}
			else if (File.Exists(filePath))
			{
				Process.Start("Explorer", "/e,/select,\"" + filePath + "\"");
			}
			else
			{
				Process.Start("Explorer", "/e,\"" + Path.GetDirectoryName(filePath) + "\"");
			}
		}
		catch (Exception)
		{
		}
	}

	public static void OpenUrlByBrowser(string url)
	{
		try
		{
			Process.Start(HttpUtility.UrlPathEncode(url));
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("open url by browser failed", exception);
		}
	}

	public static void KillProcess(string processName)
	{
		Process[] processesByName = Process.GetProcessesByName(processName);
		if (processesByName == null)
		{
			return;
		}
		Process[] array = processesByName;
		foreach (Process process in array)
		{
			try
			{
				process.Kill();
			}
			catch
			{
			}
		}
	}

	public static bool GetFileSize(string url, out long filesize, bool throwException = true)
	{
		int num = 3;
		filesize = 0L;
		do
		{
			HttpWebRequest httpWebRequest = null;
			try
			{
				GC.Collect();
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				ServicePointManager.DefaultConnectionLimit = 50;
				url = (url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) ? url : ("http://" + url));
				httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
				httpWebRequest.Method = "GET";
				httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36";
				httpWebRequest.Headers.Add("Request-Tag: lmsa");
				httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
				httpWebRequest.KeepAlive = false;
				httpWebRequest.ReadWriteTimeout = 60000;
				using HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				if (httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					filesize = httpWebResponse.ContentLength;
					return true;
				}
			}
			catch (Exception exception)
			{
				if (throwException)
				{
					LogHelper.LogInstance.Error("Get file size occur an exception form: " + url, exception);
				}
			}
			finally
			{
				try
				{
					httpWebRequest?.Abort();
				}
				catch (Exception)
				{
				}
			}
			Thread.Sleep(new Random().Next(100));
		}
		while (--num > 0);
		return false;
	}

	public static string Uuid()
	{
		return Guid.NewGuid().ToString("N").ToLowerInvariant();
	}

	public static string GetFullPathNameFromDirectory(string dir, string filePattern)
	{
		string result = string.Empty;
		if (Directory.Exists(dir))
		{
			string[] files = Directory.GetFiles(dir, filePattern, SearchOption.AllDirectories);
			if (files.Length != 0)
			{
				result = files[0];
			}
		}
		return result;
	}

	public static bool Exists(string path)
	{
		if (!File.Exists(path))
		{
			return Directory.Exists(path);
		}
		return true;
	}

	public static long GetDirectorySize(string directory)
	{
		if (!Directory.Exists(directory))
		{
			return 0L;
		}
		return Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Sum((string t) => new FileInfo(t).Length);
	}

	public static long GetFileSize(string file)
	{
		if (!File.Exists(file))
		{
			return 0L;
		}
		return new FileInfo(file).Length;
	}

	public static bool DeleteDirectory(string dirPath)
	{
		try
		{
			if (Directory.Exists(dirPath))
			{
				Directory.Delete(dirPath, recursive: true);
				return true;
			}
			LogHelper.LogInstance.Warn("delete directory [" + dirPath + "] failed. Directory path does not exist.");
			return false;
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"delete directory [{dirPath}] failed. Exception:{arg}");
			return false;
		}
	}

	public static bool DeleteDirectoryEx(string dirPath)
	{
		if (!Directory.Exists(dirPath))
		{
			return true;
		}
		SetDir2Normal(dirPath);
		return DeleteDirectory(dirPath);
	}

	public static void SetDir2Normal(string dirPath)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
		FileInfo[] files = directoryInfo.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				fileInfo.Attributes &= ~FileAttributes.ReadOnly;
			}
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			SetDir2Normal(directories[i].FullName);
		}
		directoryInfo.Attributes = (FileAttributes)0;
	}

	public static string RenameIfExist(string fileFullName)
	{
		string result = fileFullName;
		FileInfo fileInfo = new FileInfo(fileFullName);
		if (!fileInfo.Exists)
		{
			return result;
		}
		string directoryName = fileInfo.DirectoryName;
		string extension = fileInfo.Extension;
		string name = fileInfo.Name;
		name = name.Substring(0, name.LastIndexOf(extension));
		int num = 0;
		while (true)
		{
			result = Path.Combine(directoryName, $"{name}({num}){extension}");
			if (!File.Exists(result))
			{
				break;
			}
			num++;
		}
		return result;
	}

	public static string DecodeBase64(string rawString)
	{
		try
		{
			byte[] bytes = Convert.FromBase64String(rawString);
			return Encoding.UTF8.GetString(bytes);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static BitmapImage LoadBitmap(string imageUri)
	{
		BitmapImage bitmapImage = new BitmapImage();
		bitmapImage.BeginInit();
		bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
		bitmapImage.UriSource = new Uri(imageUri);
		bitmapImage.EndInit();
		bitmapImage.Freeze();
		return bitmapImage;
	}

	public static BitmapImage ConvertBitmap(Bitmap bitmap)
	{
		BitmapImage bitmapImage = new BitmapImage();
		using MemoryStream memoryStream = new MemoryStream();
		bitmap.Save(memoryStream, bitmap.RawFormat);
		memoryStream.Position = 0L;
		bitmapImage.BeginInit();
		bitmapImage.StreamSource = memoryStream;
		bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
		bitmapImage.EndInit();
		bitmapImage.Freeze();
		return bitmapImage;
	}

	public static string EncodeBase64(byte[] buffer)
	{
		return Convert.ToBase64String(buffer);
	}

	public static Image CreateThumbnail(int width, int height, string imagePath)
	{
		if (!File.Exists(imagePath))
		{
			return null;
		}
		using Image image = Image.FromFile(imagePath);
		return CreateThumbnail(width, height, image);
	}

	public static Image CreateThumbnail(int width, int height, Image image)
	{
		if (image == null)
		{
			return null;
		}
		Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
		Graphics graphics = Graphics.FromImage(bitmap);
		graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
		graphics.SmoothingMode = SmoothingMode.HighQuality;
		graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
		graphics.DrawImage(image, new Rectangle(0, 0, width, height));
		return bitmap;
	}

	public static bool TryGetRegistryKey(RegistryKey registrykey, string path, string key, out object value)
	{
		value = null;
		try
		{
			using RegistryKey registryKey = registrykey.OpenSubKey(path);
			if (registryKey == null)
			{
				return false;
			}
			value = registryKey.GetValue(key);
			return value != null;
		}
		catch
		{
			return false;
		}
	}

	public static bool TryGetRegistryKey(RegistryHive hive, string subKeyPath, string key, out object value)
	{
		value = null;
		try
		{
			RegistryKey registryKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32).OpenSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
			if (registryKey == null)
			{
				registryKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
				if (registryKey == null)
				{
					return false;
				}
			}
			value = registryKey.GetValue(key);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool TryGetRegistryKey64(RegistryHive hive, string subKeyPath, string key, out object value)
	{
		value = null;
		try
		{
			RegistryKey registryKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
			if (registryKey == null)
			{
				return false;
			}
			value = registryKey.GetValue(key);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static void WriteRegistryKey(string subKeyPath, string key, object value)
	{
		RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
		RegistryKey registryKey2 = registryKey.OpenSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
		if (registryKey2 == null)
		{
			registryKey2 = registryKey.CreateSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
			if (registryKey2 == null)
			{
				registryKey2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
			}
		}
		registryKey2.SetValue(key, value);
	}

	public static bool CheckServerIsRunning(string serviceName)
	{
		ServiceController serviceController = ServiceController.GetServices().FirstOrDefault((ServiceController n) => n.ServiceName.Equals(serviceName, StringComparison.CurrentCultureIgnoreCase));
		if (serviceController == null)
		{
			return false;
		}
		return serviceController.Status == ServiceControllerStatus.Running;
	}

	public static string GetClientUUID()
	{
		TryGetRegistryKey64(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Cryptography", "MachineGuid", out var value);
		if (value == null || string.IsNullOrEmpty(value.ToString()))
		{
			TryGetRegistryKey(RegistryHive.LocalMachine, "SYSTEM\\Software\\Microsoft\\LmsaDrivers\\clientuuid", "clientuuid", out var value2);
			if (value2 == null || string.IsNullOrEmpty(value2.ToString()))
			{
				value2 = Guid.NewGuid();
				WriteRegistryKey("SYSTEM\\Software\\Microsoft\\LmsaDrivers\\clientuuid", "clientuuid", value2.ToString());
			}
			value = value2.ToString();
		}
		return value?.ToString();
	}

	public static long GetDriverTotalFreeSapce(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return 0L;
		}
		return new DriveInfo(Path.GetPathRoot(path)).TotalFreeSpace;
	}

	public static async Task<bool> Upload(string url, string filePath, Dictionary<string, string> aparams, int trycount, int reuploadwaitmilliseconds)
	{
		_ = 1;
		try
		{
			HttpContent val = (HttpContent)new StreamContent((Stream)new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read));
			val.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
			HttpClient client = new HttpClient();
			try
			{
				MultipartFormDataContent mulContent = new MultipartFormDataContent("----");
				try
				{
					if (aparams != null && aparams.Count > 0)
					{
						foreach (KeyValuePair<string, string> aparam in aparams)
						{
							mulContent.Add((HttpContent)new StringContent(aparam.Value ?? ""), aparam.Key);
						}
					}
					string[] array = Regex.Split(filePath, "\\\\|/");
					mulContent.Add(val, "file", array[array.Length - 1]);
					do
					{
						try
						{
							HttpResponseMessage response = await client.PostAsync(url, (HttpContent)(object)mulContent);
							await response.Content.ReadAsStringAsync();
							if (response.IsSuccessStatusCode)
							{
								return response.IsSuccessStatusCode;
							}
						}
						catch
						{
							Thread.Sleep(reuploadwaitmilliseconds);
						}
					}
					while (trycount-- > 0);
					return false;
				}
				finally
				{
					((IDisposable)mulContent)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)client)?.Dispose();
			}
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static RegionInfo GetRegionInfo()
	{
		if (!TryGetRegistryKey(RegistryHive.CurrentUser, "Control Panel\\International\\Geo", "Nation", out var geoID))
		{
			TryGetRegistryKey(RegistryHive.Users, ".DEFAULT\\Control Panel\\International\\Geo", "Nation", out geoID);
		}
		RegionInfo regionInfo = (from x in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
			select new RegionInfo(x.ToString())).FirstOrDefault((RegionInfo r) => r.GeoId == int.Parse(geoID.ToString()));
		if (regionInfo == null)
		{
			regionInfo = new RegionInfo("US");
		}
		return regionInfo;
	}

	public static string GetMacAddr()
	{
		NetworkInterface[] array = (from p in NetworkInterface.GetAllNetworkInterfaces()
			where p.NetworkInterfaceType == NetworkInterfaceType.Ethernet || p.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
			orderby p.NetworkInterfaceType
			select p).ToArray();
		string text = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
		NetworkInterface[] array2 = array;
		foreach (NetworkInterface networkInterface in array2)
		{
			string name = text + networkInterface.Id + "\\Connection";
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, writable: false);
			if (registryKey == null)
			{
				continue;
			}
			string text2 = registryKey.GetValue("PnpInstanceID", "").ToString();
			if (text2.Length > 3 && text2.Substring(0, 3) == "PCI")
			{
				string text3 = networkInterface.GetPhysicalAddress().ToString().ToUpper();
				for (int j = 1; j < 6; j++)
				{
					text3 = text3.Insert(3 * j - 1, "-");
				}
				return text3;
			}
		}
		LogHelper.LogInstance.Warn("No MAC address found for this machine.");
		return "00-00-00-00-00-00";
	}

	public static List<string> GetComInfo()
	{
		List<string> list = new List<string>();
		List<string> comInfo = GetComInfo("Select * From Win32_PnPEntity where name like '%(COM%'");
		List<string> comInfo2 = GetComInfo("Select * From Win32_PnPEntity where name like '%ADB%' ");
		if (comInfo != null && comInfo.Count > 0)
		{
			list.AddRange(comInfo);
		}
		if (comInfo2 != null && comInfo2.Count > 0)
		{
			list.AddRange(comInfo2);
		}
		return list;
	}

	public static List<string> GetComInfo(string wql)
	{
		List<string> list = new List<string>();
		using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(wql);
		foreach (ManagementBaseObject item in managementObjectSearcher.Get())
		{
			string text = item.GetPropertyValue("Name").ToString();
			try
			{
				text = string.Format("{0}({1})", text, item.GetPropertyValue("DeviceID"));
			}
			catch
			{
			}
			list.Add(text);
		}
		return list;
	}

	public static string GetLmsaVersion()
	{
		foreach (string packageVersion in PackageVersionList)
		{
			string subKeyPath = $"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{packageVersion}";
			TryGetRegistryKey(RegistryHive.LocalMachine, subKeyPath, "DisplayVersion", out var value);
			if (value != null)
			{
				return value.ToString();
			}
		}
		return "0.0.0.0";
	}

	public static long ToUtcTimeStamp(DateTime datetime)
	{
		return new DateTimeOffset(datetime).ToUnixTimeMilliseconds();
	}

	public static DateTime ToUtcDateTime(long timestamp)
	{
		return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
	}

	public static DateTime? ConvertDateTime(object value)
	{
		try
		{
			if (value == null)
			{
				return null;
			}
			long num = long.Parse((string)value);
			return new DateTime(1970, 1, 1).AddMilliseconds(num).ToLocalTime();
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static DateTime ConvertDateTime(long? _data)
	{
		try
		{
			if (_data.HasValue)
			{
				return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(_data.Value);
			}
		}
		catch (Exception)
		{
		}
		return new DateTime(1970, 1, 1);
	}

	public static void ClearFlashToolDirectory()
	{
		try
		{
			string path = Configurations.FileSaveLocalPath["TOOL"];
			if (!Directory.Exists(path))
			{
				return;
			}
			string[] directories = Directory.GetDirectories(path);
			foreach (string text in directories)
			{
				string[] files = Directory.GetFiles(text, "*.exe", SearchOption.AllDirectories);
				for (int j = 0; j < files.Length; j++)
				{
					KillProcess(Path.GetFileNameWithoutExtension(files[j]));
				}
				for (int k = 0; k < 3; k++)
				{
					Thread.Sleep(1000);
					if (DeleteDirectory(text))
					{
						break;
					}
				}
			}
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"clear flash tool directory exception:[{arg}]");
		}
	}

	public static string GetLasterDirectory(string directory)
	{
		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
		{
			return null;
		}
		List<DirectoryInfo> list = new List<DirectoryInfo>();
		string[] directories = Directory.GetDirectories(directory);
		foreach (string path in directories)
		{
			list.Add(new DirectoryInfo(path));
		}
		if (list.Count == 0)
		{
			return null;
		}
		DateTime lasterTime = list.Max((DirectoryInfo n) => n.LastWriteTime);
		return list.FirstOrDefault((DirectoryInfo n) => n.LastWriteTime.Equals(lasterTime))?.FullName;
	}

	public static List<string> GetAllFiles(string directory, string searchPattern = "*")
	{
		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
		{
			return null;
		}
		return Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories).ToList();
	}

	public static void KillRescueToolIfExist()
	{
		foreach (Process item in Process.GetProcesses().ToList())
		{
			if (item.ProcessName == "QFIL" || item.ProcessName.ToLower() == "flash_tool" || item.ProcessName == "QcomDLoader" || item.ProcessName == "CmdDloader" || item.ProcessName == "UpgradeDownload")
			{
				try
				{
					item.Kill();
				}
				catch (Exception arg)
				{
					LogHelper.LogInstance.Error($"kill rescue tool:[{item.ProcessName}] exception:[{arg}]");
				}
			}
		}
	}

	public static List<string> DecryptRomFile(string _romPath, string _fileType)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		string[] array = _fileType.Split('|');
		foreach (string searchPattern in array)
		{
			list2.AddRange(Directory.GetFiles(_romPath, searchPattern, SearchOption.AllDirectories));
		}
		string text = string.Empty;
		foreach (string item in list2)
		{
			string extension = Path.GetExtension(item);
			if (!(extension == ".t"))
			{
				if (extension == ".x")
				{
					text = item + "ml";
				}
			}
			else
			{
				text = item + "xt";
			}
			list.Add(text);
			DecryptRomFileHelper.DecryptFile(item, text);
		}
		return list;
	}

	public static void DeleteDecryptedFile(List<string> _DecryptFileWaitDelete)
	{
		foreach (string item in _DecryptFileWaitDelete)
		{
			File.Delete(item);
		}
	}
}
