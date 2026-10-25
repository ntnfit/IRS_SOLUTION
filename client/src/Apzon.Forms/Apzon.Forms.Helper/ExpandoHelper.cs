using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using DevExpress.XtraGrid;

namespace Apzon.Forms.Helper;

public static class ExpandoHelper
{
	public static ExpandoObject Document { get; set; }

	public static List<ExpandoObject> GetExpandoDataSource(this GridControl grd)
	{
		return grd.DataSource as List<ExpandoObject>;
	}

	public static dynamic GetByEntry(this List<ExpandoObject> lst, int docEntry)
	{
		return lst.FirstOrDefault((ExpandoObject x) => ((dynamic)x).DocEntry == docEntry);
	}

	public static dynamic GetByDocNum(this List<ExpandoObject> lst, int docNum)
	{
		return lst.FirstOrDefault((ExpandoObject x) => ((dynamic)x).DocNum == docNum);
	}

	public static dynamic GetAt(this List<ExpandoObject> lst, int index)
	{
		return lst[index];
	}

	public static dynamic GetFirstOrDefault(this List<ExpandoObject> lst)
	{
		return lst.FirstOrDefault();
	}

	public static void GetNewDocument(ExpandoObject model)
	{
		IDictionary<string, object> dictionary = new ExpandoObject();
		try
		{
			_ = ((IDictionary<string, object>)model).Keys;
			foreach (string item in ((IDictionary<string, object>)model).Keys)
			{
				object value = model.FirstOrDefault((KeyValuePair<string, object> t) => t.Key.ToUpper().Equals(item.ToUpper())).Value;
				if (value != null)
				{
					Type type = value.GetType();
					object defaultValue = type.GetDefaultValue();
					if (type == typeof(string))
					{
						dictionary.Add(item, "");
					}
					else
					{
						dictionary.Add(item, defaultValue);
					}
				}
				else
				{
					dictionary.Add(item, string.Empty);
				}
			}
		}
		catch (Exception)
		{
		}
		Document = dictionary as ExpandoObject;
	}

	public static object GetDefaultValue(this Type t)
	{
		try
		{
			if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
			{
				return Activator.CreateInstance(t);
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
