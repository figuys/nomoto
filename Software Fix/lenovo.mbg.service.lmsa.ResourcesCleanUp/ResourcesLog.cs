using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.lmsa.ResourcesCleanUp.Model;

namespace lenovo.mbg.service.lmsa.ResourcesCleanUp;

public class ResourcesLog
{
	private static ResourcesLog mSingle = null;

	private static readonly object mSingleInstanceLock = new object();

	private string mResourcesLogFile = string.Empty;

	private readonly XmlDocument mDoc = null;

	private static readonly object fileReadWriteLock = new object();

	private static readonly List<string> m_defaultResource = new List<string>
	{
		"Contact", "Music", "PicAlbum", "PicOriginal", "Screencap", "Temp", "Video", "Pic", "Backup", "AppIcon",
		"MusicCache"
	};

	public static ResourcesLog Single
	{
		get
		{
			if (mSingle == null)
			{
				lock (mSingleInstanceLock)
				{
					if (mSingle == null)
					{
						mSingle = new ResourcesLog();
					}
				}
			}
			return mSingle;
		}
	}

	private ResourcesLog()
	{
		lock (fileReadWriteLock)
		{
			mDoc = new XmlDocument();
			mResourcesLogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "LMSA", "lmsaresources.log");
			if (!File.Exists(mResourcesLogFile))
			{
				CreateNewLogFile();
				return;
			}
			try
			{
				mDoc.Load(mResourcesLogFile);
			}
			catch (XmlException ex)
			{
				LogHelper.LogInstance.Error("Load exist resource log file throw xml exception:" + ex.ToString());
				try
				{
					File.Delete(mResourcesLogFile);
					CreateNewLogFile();
				}
				catch (Exception ex2)
				{
					LogHelper.LogInstance.Error("Delete the error resource file and create new throw exception:" + ex2.ToString());
				}
			}
			catch (Exception ex3)
			{
				LogHelper.LogInstance.Error("Load exist resource log file throw exception:" + ex3.ToString());
			}
		}
	}

	private void CreateNewLogFile()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		string name = "lenovo.mbg.service.lmsa.ResourcesCleanUp.Storage.LmsaDeviceResources.xml";
		using (Stream stream = executingAssembly.GetManifestResourceStream(name))
		{
			using StreamReader streamReader = new StreamReader(stream);
			string text = streamReader.ReadToEnd();
			LogHelper.LogInstance.Debug("Read log xml tempate:" + text);
			mDoc.LoadXml(text);
		}
		TrySaveDocument();
	}

	public void AddFileRecord(string path)
	{
		LogHelper.LogInstance.Debug($"Add new file resource[{path}]");
		lock (fileReadWriteLock)
		{
			XmlElement elementById = mDoc.GetElementById("files");
			if (elementById != null)
			{
				XmlNode xmlNode = mDoc.CreateNode(XmlNodeType.Element, "file", string.Empty);
				xmlNode.InnerText = path;
				elementById.AppendChild(xmlNode);
				TrySaveDocument();
			}
		}
	}

	public void AddDirRecord(string path)
	{
		LogHelper.LogInstance.Debug($"Add new dir resurce[{path}]");
		lock (fileReadWriteLock)
		{
			XmlElement elementById = mDoc.GetElementById("dirs");
			if (elementById != null)
			{
				XmlNode xmlNode = mDoc.CreateNode(XmlNodeType.Element, "dir", string.Empty);
				xmlNode.InnerText = path;
				elementById.AppendChild(xmlNode);
				TrySaveDocument();
			}
		}
	}

	public List<ResourceAbstract> GetResources()
	{
		List<ResourceAbstract> resources = new List<ResourceAbstract>();
		lock (fileReadWriteLock)
		{
			foreach (string item in m_defaultResource)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Configurations.ProgramDataPath, item));
				if (directoryInfo.Exists && !resources.Exists((ResourceAbstract m) => m.Path.Equals(item)))
				{
					GetDirFiles(directoryInfo, ref resources);
				}
			}
			if (mDoc == null)
			{
				return resources;
			}
			XmlElement elementById = mDoc.GetElementById("dirs");
			if (elementById != null)
			{
				foreach (XmlNode childNode in elementById.ChildNodes)
				{
					DirectoryInfo dir = new DirectoryInfo(childNode.InnerText);
					if (dir.Exists && !resources.Exists((ResourceAbstract m) => m.Path.Equals(dir.FullName)))
					{
						GetDirFiles(dir, ref resources);
					}
					else
					{
						childNode.ParentNode.RemoveChild(childNode);
					}
				}
			}
			XmlElement elementById2 = mDoc.GetElementById("files");
			if (elementById2 != null)
			{
				foreach (XmlNode node in elementById2.ChildNodes.Cast<XmlNode>().ToList())
				{
					if (!resources.Exists((ResourceAbstract m) => string.Compare(m.Path, node.InnerText) == 0) && File.Exists(node.InnerText))
					{
						resources.Add(new FileResource(node.InnerText));
					}
					else
					{
						node.ParentNode.RemoveChild(node);
					}
				}
			}
			TrySaveDocument();
		}
		return resources;
	}

	private void GetDirFiles(DirectoryInfo dir, ref List<ResourceAbstract> resources)
	{
		if (dir == null)
		{
			return;
		}
		DirectoryInfo[] directories = dir.GetDirectories();
		if (directories != null)
		{
			DirectoryInfo[] array = directories;
			foreach (DirectoryInfo dir2 in array)
			{
				GetDirFiles(dir2, ref resources);
			}
		}
		FileInfo[] files = dir.GetFiles();
		if (files != null)
		{
			FileInfo[] array2 = files;
			foreach (FileInfo item in array2)
			{
				if (!resources.Exists((ResourceAbstract m) => m.Path.Equals(item.FullName)))
				{
					resources.Add(new FileResource(item.FullName));
				}
			}
		}
		if (!resources.Exists((ResourceAbstract m) => m.Path.Equals(dir.FullName)))
		{
			resources.Add(new DirResource(dir.FullName));
		}
	}

	public void RemoveFileRecordExcept(List<ResourceAbstract> resources)
	{
		lock (fileReadWriteLock)
		{
			if (mDoc == null)
			{
				return;
			}
			XmlElement elementById = mDoc.GetElementById("files");
			if (elementById != null)
			{
				foreach (XmlNode node in elementById.ChildNodes.Cast<XmlNode>().ToList())
				{
					if (!resources.Exists((ResourceAbstract m) => string.Compare(node.InnerText, m.Path) == 0))
					{
						node.ParentNode.RemoveChild(node);
					}
				}
			}
			TrySaveDocument();
		}
	}

	public void RemoveResourceRecord(List<ResourceAbstract> resources)
	{
		lock (fileReadWriteLock)
		{
			if (mDoc == null)
			{
				return;
			}
			foreach (string itype in resources.Select((ResourceAbstract m) => m.RootId).Distinct())
			{
				XmlElement elementById = mDoc.GetElementById(itype);
				if (elementById == null)
				{
					continue;
				}
				IEnumerable<ResourceAbstract> enumerable = resources.Where((ResourceAbstract m) => string.Compare(itype, m.RootId) == 0);
				foreach (XmlNode item in elementById.ChildNodes.Cast<XmlNode>().ToList())
				{
					foreach (ResourceAbstract item2 in enumerable)
					{
						if (string.Compare(item.InnerText, item2.Path) == 0)
						{
							item.ParentNode.RemoveChild(item);
						}
					}
				}
			}
			TrySaveDocument();
		}
	}

	private void TrySaveDocument()
	{
		try
		{
			mDoc.Save(mResourcesLogFile);
		}
		catch (Exception ex)
		{
			LogHelper.LogInstance.Error("Save resource log throw exception:" + ex.ToString());
		}
	}
}
