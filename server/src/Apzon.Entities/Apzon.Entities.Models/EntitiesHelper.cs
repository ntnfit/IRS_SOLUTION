using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Apzon.Commons;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models;

public static class EntitiesHelper<T> where T : class
{
	public static T ToEntity(DataRow dr, bool hasUdf = true, string udfProName = "UserFields")
	{
		T val = (T)Activator.CreateInstance(typeof(T));
		try
		{
			List<string> list = (from DataColumn c in dr.Table.Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(T).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.Name == udfProName)
				{
					if (!hasUdf)
					{
						continue;
					}
					List<string> list2 = list.Where((string t) => t.StartsWith("U_")).ToList();
					if (list2.Count <= 0)
					{
						continue;
					}
					List<UserFieldsImpl> list3 = new List<UserFieldsImpl>();
					for (int num2 = 0; num2 < list2.Count; num2++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						userFieldsImpl.Name = list2[num2];
						userFieldsImpl.Value = dr[list2[num2]];
						Type dataType = dr.Table.Columns[list2[num2]].DataType;
						if (dataType == typeof(DateTime))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Date;
						}
						else if (dataType == typeof(float))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Float;
						}
						else if (dataType == typeof(double) || dataType == typeof(int) || dataType == typeof(short) || dataType == typeof(decimal))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						}
						list3.Add(userFieldsImpl);
					}
					if (list3.Count > 0)
					{
						propertyInfo.SetValue(val, list3);
					}
				}
				else
				{
					if (!list.Contains(propertyInfo.Name))
					{
						continue;
					}
					try
					{
						PropertyInfo property = val.GetType().GetProperty(propertyInfo.Name);
						if (!(property != null))
						{
							continue;
						}
						Type type = property.PropertyType;
						if (type.IsEnum)
						{
							propertyInfo.SetValue(val, (dr[propertyInfo.Name] is DBNull) ? null : Enum.Parse(type, Convert.ToString(dr[propertyInfo.Name])));
							continue;
						}
						if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
						{
							type = Nullable.GetUnderlyingType(type);
						}
						propertyInfo.SetValue(val, (dr[propertyInfo.Name] == DBNull.Value || string.IsNullOrWhiteSpace(Function.ToString(dr[propertyInfo.Name]))) ? null : Convert.ChangeType(dr[propertyInfo.Name], type));
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", ex);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
		}
		return val;
	}

	public static List<T> ToEntities(DataTable dt, bool hasUdf = true, string udfProName = "UserFields")
	{
		List<T> list = new List<T>();
		try
		{
			List<string> source = (from DataColumn c in dt.Columns
				select c.ColumnName).ToList();
			List<string> list2 = source.Where((string t) => t.StartsWith("U_")).ToList();
			List<string> list3 = (from t in source
				where !t.StartsWith("U_")
				select t.ToUpper()).ToList();
			PropertyInfo[] properties = typeof(T).GetProperties();
			for (int num = 0; num < dt.Rows.Count; num++)
			{
				DataRow dataRow = dt.Rows[num];
				T val = (T)Activator.CreateInstance(typeof(T));
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (propertyInfo.Name == udfProName)
					{
						if (!hasUdf || list2.Count <= 0)
						{
							continue;
						}
						List<UserFieldsImpl> list4 = new List<UserFieldsImpl>();
						for (int num3 = 0; num3 < list2.Count; num3++)
						{
							UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
							if (dataRow.Table.Columns[list2[num3]].DataType == typeof(DateTime))
							{
								userFieldsImpl.Value = Function.ParseDateTime(dataRow[list2[num3]]);
								userFieldsImpl.Type = BoFieldTypes.db_Date;
							}
							else if (dataRow.Table.Columns[list2[num3]].DataType == typeof(double) || dataRow.Table.Columns[list2[num3]].DataType == typeof(int) || dataRow.Table.Columns[list2[num3]].DataType == typeof(short) || dataRow.Table.Columns[list2[num3]].DataType == typeof(decimal))
							{
								userFieldsImpl.Value = Function.ParseDouble(dataRow[list2[num3]]);
								userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							}
							else
							{
								userFieldsImpl.Value = Function.ToString(dataRow[list2[num3]]);
								userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							}
							userFieldsImpl.Name = list2[num3];
							list4.Add(userFieldsImpl);
						}
						if (list4.Count > 0)
						{
							propertyInfo.SetValue(val, list4);
						}
					}
					else if (list3.Contains(propertyInfo.Name.ToUpper()))
					{
						PropertyInfo property = val.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((dataRow[propertyInfo.Name] == DBNull.Value || string.IsNullOrEmpty(Function.ToString(dataRow[propertyInfo.Name]))) ? null : Convert.ChangeType(dataRow[propertyInfo.Name], conversionType));
							propertyInfo.SetValue(val, value, null);
						}
					}
				}
				list.Add(val);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return list;
	}
}
