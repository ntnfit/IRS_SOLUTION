using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Apzon.Commons.Helper;

public static class ClassHelper
{
	public static object ToType(this object obj, Type type)
	{
		object obj2 = Activator.CreateInstance(type);
		PropertyInfo[] properties = obj.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			try
			{
				obj2.GetType().GetProperty(propertyInfo.Name).SetValue(obj2, propertyInfo.GetValue(obj, null), null);
			}
			catch (Exception)
			{
			}
		}
		return obj2;
	}

	public static object GetValueByName(this object obj, string fieldName, out Type type)
	{
		try
		{
			type = obj.GetType().GetProperty(fieldName).PropertyType;
			return obj.GetType().GetProperty(fieldName).GetValue(obj, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		type = null;
		return null;
	}

	public static bool IsNotNull(this List<object> lst)
	{
		try
		{
			return lst != null && lst.Count > 0;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static object GetDefaultSqlValue(Type type)
	{
		try
		{
			if (type == null)
			{
				return "";
			}
			if (type == typeof(bool))
			{
				return 0;
			}
			if (type == typeof(int))
			{
				return 0;
			}
			if (type == typeof(double))
			{
				return 0;
			}
			if (type == typeof(decimal))
			{
				return 0;
			}
			if (type == typeof(DateTime))
			{
				return "1900-01-01";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}
}
