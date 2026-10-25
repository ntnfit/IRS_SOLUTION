using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Apzon.Commons;

namespace Apzon.Entities.Models.Common;

public class Row<T> : DataRow, IRow<T> where T : class
{
	public Row(DataRowBuilder builder)
		: base(builder)
	{
	}

	public T ToObject()
	{
		T val = (T)Activator.CreateInstance(typeof(T));
		try
		{
			List<string> list = (from DataColumn c in Table.Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(T).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (list.Contains(propertyInfo.Name))
				{
					PropertyInfo property = val.GetType().GetProperty(propertyInfo.Name);
					Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
					object value = ((base[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(base[propertyInfo.Name], conversionType));
					propertyInfo.SetValue(val, value, null);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return val;
	}
}
