using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Apzon.Commons.Helper;

public class ConvertHelper
{
	public static DataTable ConvertToDataTable<T>(IList<T> data)
	{
		PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
		DataTable dataTable = new DataTable();
		foreach (PropertyDescriptor item in properties)
		{
			dataTable.Columns.Add(item.Name, Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
		}
		foreach (T datum in data)
		{
			DataRow dataRow = dataTable.NewRow();
			foreach (PropertyDescriptor item2 in properties)
			{
				dataRow[item2.Name] = item2.GetValue(datum) ?? DBNull.Value;
			}
			dataTable.Rows.Add(dataRow);
		}
		return dataTable;
	}

	public static List<T> ConvertTo<T>(DataTable datatable) where T : new()
	{
		List<T> result = new List<T>();
		try
		{
			List<string> columnsNames = new List<string>();
			foreach (DataColumn column in datatable.Columns)
			{
				columnsNames.Add(column.ColumnName);
			}
			result = datatable.AsEnumerable().ToList().ConvertAll((DataRow row) => getObject<T>(row, columnsNames));
			return result;
		}
		catch
		{
			return result;
		}
	}

	private static T getObject<T>(DataRow row, List<string> columnsName) where T : new()
	{
		T val = new T();
		try
		{
			string text = "";
			string text2 = "";
			PropertyInfo[] properties = typeof(T).GetProperties();
			foreach (PropertyInfo objProperty in properties)
			{
				text = columnsName.Find((string name) => name.ToLower() == objProperty.Name.ToLower());
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				text2 = row[text].ToString();
				if (!string.IsNullOrEmpty(text2))
				{
					if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
					{
						text2 = row[text].ToString().Replace("$", "").Replace(",", "");
						objProperty.SetValue(val, Convert.ChangeType(text2, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
					}
					else
					{
						text2 = row[text].ToString().Replace("%", "");
						objProperty.SetValue(val, Convert.ChangeType(text2, Type.GetType(objProperty.PropertyType.ToString())), null);
					}
				}
			}
			return val;
		}
		catch
		{
			return val;
		}
	}

	public static T CopyToObject<T>(T Data) where T : class, new()
	{
		T val = new T();
		Type typeFromHandle = typeof(T);
		_ = typeFromHandle.UnderlyingSystemType.Name;
		List<PropertyInfo> list = typeFromHandle.GetProperties().ToList();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].SetValue(val, list[i].GetValue(Data, null), null);
		}
		return val;
	}

	public static List<SqlParameter> ClassToSqlParameter<T>(T Data) where T : class
	{
		List<SqlParameter> list = new List<SqlParameter>();
		Type typeFromHandle = typeof(T);
		_ = typeFromHandle.UnderlyingSystemType.Name;
		foreach (PropertyInfo item in typeFromHandle.GetProperties().ToList())
		{
			SqlParameter sqlParameter = new SqlParameter();
			sqlParameter.ParameterName = "@" + item.Name;
			sqlParameter.Value = item.GetValue(Data, null);
			list.Add(sqlParameter);
		}
		return list;
	}

	public static List<T> ToList<T>(DataTable dataTable) where T : new()
	{
		List<T> list = new List<T>();
		var list2 = (from PropertyInfo item in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
			select new
			{
				Name = item.Name,
				Type = (Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType)
			}).ToList();
		var list3 = (from DataColumn item in dataTable.Columns
			select new
			{
				Name = item.ColumnName,
				Type = item.DataType
			}).ToList();
		foreach (DataRow item in dataTable.AsEnumerable().ToList())
		{
			T val = new T();
			foreach (var dtField in list3)
			{
				PropertyInfo property = val.GetType().GetProperty(dtField.Name);
				if (list2.Find(x => x.Name == dtField.Name) == null)
				{
					continue;
				}
				if (property.PropertyType == typeof(DateTime))
				{
					property.SetValue(val, convertToDateTime(item[dtField.Name]), null);
				}
				else if (property.PropertyType == typeof(int))
				{
					property.SetValue(val, ConvertToInt(item[dtField.Name]), null);
				}
				else if (property.PropertyType == typeof(long))
				{
					property.SetValue(val, ConvertToLong(item[dtField.Name]), null);
				}
				else if (property.PropertyType == typeof(decimal))
				{
					property.SetValue(val, ConvertToDecimal(item[dtField.Name]), null);
				}
				else if (property.PropertyType == typeof(string))
				{
					if (item[dtField.Name].GetType() == typeof(DateTime))
					{
						property.SetValue(val, ConvertToDateString(item[dtField.Name]), null);
					}
					else
					{
						property.SetValue(val, ConvertToString(item[dtField.Name]), null);
					}
				}
			}
			list.Add(val);
		}
		return list;
	}

	private static string ConvertToDateString(object date)
	{
		if (date == null)
		{
			return string.Empty;
		}
		return Function.ParseDateTimes(Convert.ToDateTime(date)).ToString();
	}

	private static string ConvertToString(object value)
	{
		return Function.ToString(value);
	}

	private static int ConvertToInt(object value)
	{
		return Function.ParseInt(value);
	}

	private static long ConvertToLong(object value)
	{
		return Function.ParseLong(value);
	}

	private static decimal ConvertToDecimal(object value)
	{
		return Function.ParseDecimal(value);
	}

	private static DateTime convertToDateTime(object date)
	{
		return Function.ParseDateTimes(date);
	}

	public static DataTable ObjConvertToDt<T>(IList<T> list)
	{
		DataTable dataTable = CreateTable<T>();
		PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
		foreach (T item in list)
		{
			DataRow dataRow = dataTable.NewRow();
			foreach (PropertyDescriptor item2 in properties)
			{
				dataRow[item2.Name] = item2.GetValue(item);
			}
			dataTable.Rows.Add(dataRow);
		}
		return dataTable;
	}

	public static IList<T> DrConvertToObj<T>(IList<DataRow> rows)
	{
		IList<T> list = null;
		if (rows != null)
		{
			list = new List<T>();
			foreach (DataRow row in rows)
			{
				T item = CreateItem<T>(row);
				list.Add(item);
			}
		}
		return list;
	}

	public static List<TSource> DataTableToList<TSource>(DataTable dataTable) where TSource : class, new()
	{
		try
		{
			List<TSource> list = new List<TSource>();
			var first = (from PropertyInfo aProp in typeof(TSource).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				select new
				{
					Name = aProp.Name,
					Type = (Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType)
				}).ToList();
			var second = (from DataColumn aHeader in dataTable.Columns
				select new
				{
					Name = aHeader.ColumnName,
					Type = aHeader.DataType
				}).ToList();
			var list2 = first.Intersect(second).ToList();
			foreach (DataRow item in dataTable.AsEnumerable().ToList())
			{
				TSource val = new TSource();
				foreach (var item2 in list2)
				{
					PropertyInfo property = val.GetType().GetProperty(item2.Name);
					object value = ((item[item2.Name] == DBNull.Value) ? null : item[item2.Name]);
					property.SetValue(val, value, null);
				}
				list.Add(val);
			}
			return list;
		}
		catch
		{
			return null;
		}
	}

	public static IList<T> DtConvertToListObj<T>(DataTable table)
	{
		if (table == null)
		{
			return null;
		}
		List<DataRow> list = new List<DataRow>();
		foreach (DataRow row in table.Rows)
		{
			list.Add(row);
		}
		return DrConvertToObj<T>(list);
	}

	public static T CreateItem<T>(DataRow row)
	{
		T val = default;
		if (row != null)
		{
			val = Activator.CreateInstance<T>();
			foreach (DataColumn column in row.Table.Columns)
			{
				PropertyInfo property = val.GetType().GetProperty(column.ColumnName);
				try
				{
					object value = row[column.ColumnName];
					property.SetValue(val, value, null);
				}
				catch
				{
					throw;
				}
			}
		}
		return val;
	}

	public static DataTable CreateTable<T>()
	{
		Type typeFromHandle = typeof(T);
		DataTable dataTable = new DataTable(typeFromHandle.Name);
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeFromHandle))
		{
			dataTable.Columns.Add(property.Name, property.PropertyType);
		}
		return dataTable;
	}
}
