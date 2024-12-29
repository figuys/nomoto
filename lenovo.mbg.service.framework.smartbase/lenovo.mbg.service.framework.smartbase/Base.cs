using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using lenovo.mbg.service.framework.services;

namespace lenovo.mbg.service.framework.smartbase;

public class Base : IBase, IDisposable
{
	protected delegate ObjectType LoadObject<ObjectType>();

	public static Base instance = new Base();

	private HashSet<Type> smarts = new HashSet<Type>();

	private SortedList<string, SortedList<string, Type>> interfaces = new SortedList<string, SortedList<string, Type>>();

	private object cacheLock = new object();

	private Dictionary<string, object> cached = new Dictionary<string, object>();

	private Dictionary<string, Type> creators = new Dictionary<string, Type>();

	private string TAG => GetType().FullName;

	public Base()
	{
		smarts = LoadSmarts();
		interfaces = LoadDlls();
	}

	private HashSet<Type> LoadSmarts()
	{
		HashSet<Type> hashSet = new HashSet<Type>();
		Type[] types = typeof(IPlugin).Assembly.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsInterface || type.IsAbstract)
			{
				hashSet.Add(type);
			}
		}
		return hashSet;
	}

	private SortedList<string, SortedList<string, Type>> LoadDlls()
	{
		SortedList<string, SortedList<string, Type>> sortedList = new SortedList<string, SortedList<string, Type>>();
		List<string> list = Directory.EnumerateFiles(".", "lenovo.mbg*.dll").ToList();
		List<string> collection = Directory.EnumerateFiles(".", "Software Fix.exe").ToList();
		list.AddRange(collection);
		foreach (string item in list)
		{
			Assembly assembly = null;
			try
			{
				assembly = Assembly.LoadFrom(item);
			}
			catch (Exception)
			{
				continue;
			}
			string arg = assembly.GetName().Name.ToLowerInvariant();
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				string name = type.Name;
				if (type.IsAbstract)
				{
					continue;
				}
				Type[] array = type.GetInterfaces();
				List<Type> list2 = new List<Type>();
				if (array != null)
				{
					list2 = array.ToList();
				}
				_ = type.BaseType;
				list2.Add(type.BaseType);
				foreach (Type item2 in list2)
				{
					if (smarts.Contains(item2))
					{
						if (!sortedList.ContainsKey(item2.FullName.ToLowerInvariant()))
						{
							sortedList[item2.FullName.ToLowerInvariant()] = new SortedList<string, Type>();
						}
						string key = $"{arg}.{name}".ToLowerInvariant();
						sortedList[item2.FullName.ToLowerInvariant()][key] = type;
					}
				}
			}
		}
		return sortedList;
	}

	protected InterfaceType FindInterface<InterfaceType>(string typeName, bool create, bool skipCache, bool saveCache)
	{
		lock (cacheLock)
		{
			string text = typeof(InterfaceType).FullName.ToLowerInvariant();
			InterfaceType val = default(InterfaceType);
			bool flag = true;
			if (!skipCache && !create && cached.ContainsKey(text))
			{
				val = (InterfaceType)cached[text];
				flag = false;
			}
			if (flag)
			{
				Type type = null;
				if (!skipCache && creators.ContainsKey(text))
				{
					type = creators[text];
				}
				else
				{
					if (typeName == string.Empty)
					{
						throw new NotSupportedException($"Could not load interface: {text}");
					}
					SortedList<string, Type> sortedList = interfaces[text];
					if (typeName.StartsWith("*."))
					{
						string value = typeName.Substring(1).ToLowerInvariant();
						foreach (string key in sortedList.Keys)
						{
							if (key.EndsWith(value))
							{
								typeName = key;
							}
						}
					}
					type = sortedList[typeName.ToLowerInvariant()];
				}
				val = (InterfaceType)Activator.CreateInstance(type);
				if (create && saveCache)
				{
					creators[text] = type;
				}
			}
			if (!create && saveCache)
			{
				cached[text] = val;
			}
			return val;
		}
	}

	public InterfaceType Load<InterfaceType>(string typeName)
	{
		return FindInterface<InterfaceType>(typeName, create: false, skipCache: false, saveCache: true);
	}

	public InterfaceType LoadCached<InterfaceType>()
	{
		return FindInterface<InterfaceType>(string.Empty, create: false, skipCache: false, saveCache: false);
	}

	public InterfaceType LoadNew<InterfaceType>(string typeName)
	{
		return FindInterface<InterfaceType>(typeName, create: true, skipCache: false, saveCache: false);
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
