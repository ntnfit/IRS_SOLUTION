using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models;

public static class ItemsTableHelper
{
	public static DataTable Items { get; set; }

	public static DataTable ItemPrices { get; set; }

	public static DataTable ItemWarehouses { get; set; }

	public static ItemsImpl ToItems(this DataRow dr)
	{
		ItemsImpl itemsImpl = new ItemsImpl();
		try
		{
			List<string> list = (from DataColumn c in dr.Table.Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(ItemsImpl).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!list.Contains(propertyInfo.Name))
				{
					continue;
				}
				try
				{
					PropertyInfo property = itemsImpl.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						Type type = property.PropertyType;
						if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
						{
							type = Nullable.GetUnderlyingType(type);
						}
						propertyInfo.SetValue(itemsImpl, (dr[propertyInfo.Name] == DBNull.Value || string.IsNullOrWhiteSpace(Function.ToString(dr[propertyInfo.Name]))) ? null : Convert.ChangeType(dr[propertyInfo.Name], type));
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", ex);
				}
			}
			List<string> list2 = list.Where((string t) => t.StartsWith("U_")).ToList();
			if (list2.Count > 0)
			{
				itemsImpl.ListUserFields = new List<UserFieldsImpl>();
				for (int num2 = 0; num2 < list2.ToList().Count; num2++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Name = list2.ToList()[num2];
					userFieldsImpl.Value = dr[list2.ToList()[num2]];
					Type dataType = dr.Table.Columns[list2.ToList()[num2]].DataType;
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
					itemsImpl.ListUserFields.Add(userFieldsImpl);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			throw;
		}
		return itemsImpl;
	}

	public static List<Items_LineImpl> ToItemPrices(this DataTable dt)
	{
		List<string> list = (from t in (from DataColumn c in dt.Columns
				select c.ColumnName).ToList()
			where !t.StartsWith("U_")
			select t.ToUpper()).ToList();
		PropertyInfo[] properties = typeof(Items_LineImpl).GetProperties();
		List<Items_LineImpl> list2 = new List<Items_LineImpl>();
		for (int num = 0; num < dt.Rows.Count; num++)
		{
			DataRow dataRow = dt.Rows[num];
			Items_LineImpl items_LineImpl = new Items_LineImpl();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (list.Contains(propertyInfo.Name.ToUpper()))
				{
					PropertyInfo property = items_LineImpl.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
						object value = ((dataRow[propertyInfo.Name] == DBNull.Value || string.IsNullOrEmpty(Function.ToString(dataRow[propertyInfo.Name]))) ? null : Convert.ChangeType(dataRow[propertyInfo.Name], conversionType));
						propertyInfo.SetValue(items_LineImpl, value, null);
					}
				}
			}
			list2.Add(items_LineImpl);
		}
		return list2;
	}

	public static List<OITW_Impl> ToItemWarehouses(this DataTable dt)
	{
		List<string> list = (from t in (from DataColumn c in dt.Columns
				select c.ColumnName).ToList()
			where !t.StartsWith("U_")
			select t.ToUpper()).ToList();
		PropertyInfo[] properties = typeof(OITW_Impl).GetProperties();
		List<OITW_Impl> list2 = new List<OITW_Impl>();
		for (int num = 0; num < dt.Rows.Count; num++)
		{
			DataRow dataRow = dt.Rows[num];
			OITW_Impl oITW_Impl = new OITW_Impl();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (list.Contains(propertyInfo.Name.ToUpper()))
				{
					PropertyInfo property = oITW_Impl.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
						object value = ((dataRow[propertyInfo.Name] == DBNull.Value || string.IsNullOrEmpty(Function.ToString(dataRow[propertyInfo.Name]))) ? null : Convert.ChangeType(dataRow[propertyInfo.Name], conversionType));
						propertyInfo.SetValue(oITW_Impl, value, null);
					}
				}
			}
			list2.Add(oITW_Impl);
		}
		return list2;
	}
}
