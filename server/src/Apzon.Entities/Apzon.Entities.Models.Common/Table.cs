using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Apzon.Commons;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Common;

public class Table<T> : DataTable, ITable<T> where T : class
{
	public Row<T> this[int idx] => (Row<T>)Rows[idx];

	public void Add(Row<T> row)
	{
		Rows.Add(row);
	}

	public void Remove(Row<T> row)
	{
		Rows.Remove(row);
	}

	public void Insert(int idx, Row<T> row)
	{
		Rows.InsertAt(row, idx);
	}

	public Row<T> GetNewRow()
	{
		return (Row<T>)NewRow();
	}

	public Row<T> FirstOrDefault()
	{
		try
		{
			return (Row<T>)Rows[0];
		}
		catch (Exception)
		{
			return null;
		}
	}

	public ITable<T> GetClone()
	{
		return (Table<T>)Clone();
	}

	public virtual List<Row<T>> GetEnumerable()
	{
		List<Row<T>> list = new List<Row<T>>();
		for (int i = 0; i < Rows.Count; i++)
		{
			list.Add(this[i]);
		}
		return list;
	}

	public List<T> ToListObject()
	{
		string proTmp = "";
		try
		{
			List<string> columnNames = (from DataColumn c in Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(T).GetProperties();
			IEnumerable<string> udfCols = columnNames.Where((string t) => t.StartsWith("U_"));
			return this.AsEnumerable().Select((DataRow row) =>
			{
				T val = (T)Activator.CreateInstance(typeof(T));
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					proTmp = propertyInfo.Name;
					if (columnNames.Contains(propertyInfo.Name))
					{
						PropertyInfo property = val.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((row[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(row[propertyInfo.Name], conversionType));
							propertyInfo.SetValue(val, value, null);
						}
					}
				}
				if (udfCols != null && udfCols.ToList() != null && udfCols.ToList().Count > 0)
				{
					PropertyInfo property2 = val.GetType().GetProperty("UserFields");
					PropertyInfo propertyInfo2 = properties.FirstOrDefault((PropertyInfo t) => t.Name == "UserFields");
					if (property2 != null && propertyInfo2 != null)
					{
						List<UserFieldsImpl> list = new List<UserFieldsImpl>();
						for (int num = 0; num < udfCols.ToList().Count; num++)
						{
							list.Add(new UserFieldsImpl
							{
								Name = udfCols.ToList()[num],
								Value = Function.ToString(row[udfCols.ToList()[num]])
							});
						}
						propertyInfo2.SetValue(val, list, null);
					}
				}
				return val;
			}).ToList();
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}
}
