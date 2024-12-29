using System;
using System.Reflection;

namespace lenovo.mbg.service.framework.smartdevice;

public class StepHelper
{
	public static ReturnType LoadNew<ReturnType>(string stepName)
	{
		ReturnType result = default(ReturnType);
		Assembly assembly = typeof(StepHelper).Assembly;
		Type type = assembly.GetType($"{assembly.GetName().Name}.Steps.{stepName}");
		try
		{
			result = (ReturnType)Activator.CreateInstance(type);
			return result;
		}
		catch (Exception)
		{
		}
		return result;
	}
}
