using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Apzon.Commons;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models;

public static class ProductionOrderTableHelper
{
	public static DataTable ProductionHeader { get; set; }

	public static DataTable ProductionLines { get; set; }

	public static ProductionOder_Impl_OWOR ToProduction(this DataRow dr)
	{
		ProductionOder_Impl_OWOR productionOder_Impl_OWOR = new ProductionOder_Impl_OWOR();
		try
		{
			List<string> list = (from DataColumn c in dr.Table.Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(ProductionOder_Impl_OWOR).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!list.Contains(propertyInfo.Name))
				{
					continue;
				}
				try
				{
					PropertyInfo property = productionOder_Impl_OWOR.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						Type type = property.PropertyType;
						if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
						{
							type = Nullable.GetUnderlyingType(type);
						}
						propertyInfo.SetValue(productionOder_Impl_OWOR, (dr[propertyInfo.Name] == DBNull.Value || string.IsNullOrWhiteSpace(Function.ToString(dr[propertyInfo.Name]))) ? null : Convert.ChangeType(dr[propertyInfo.Name], type));
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
				productionOder_Impl_OWOR.UserFields = new List<UserFieldsImpl>();
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
					productionOder_Impl_OWOR.UserFields.Add(userFieldsImpl);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			throw;
		}
		return productionOder_Impl_OWOR;
	}

	public static List<ProductionOder_LineImpl_WOR1> ToProductionLine(this DataTable dt)
	{
		List<string> source = (from DataColumn c in dt.Columns
			select c.ColumnName).ToList();
		List<string> list = source.Where((string t) => t.StartsWith("U_")).ToList();
		List<string> list2 = (from t in source
			where !t.StartsWith("U_")
			select t.ToUpper()).ToList();
		PropertyInfo[] properties = typeof(ProductionOder_LineImpl_WOR1).GetProperties();
		List<ProductionOder_LineImpl_WOR1> list3 = new List<ProductionOder_LineImpl_WOR1>();
		for (int num = 0; num < dt.Rows.Count; num++)
		{
			DataRow dataRow = dt.Rows[num];
			ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = new ProductionOder_LineImpl_WOR1();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (list2.Contains(propertyInfo.Name.ToUpper()))
				{
					PropertyInfo property = productionOder_LineImpl_WOR.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
						object value = ((dataRow[propertyInfo.Name] == DBNull.Value || string.IsNullOrEmpty(Function.ToString(dataRow[propertyInfo.Name]))) ? null : Convert.ChangeType(dataRow[propertyInfo.Name], conversionType));
						propertyInfo.SetValue(productionOder_LineImpl_WOR, value, null);
					}
				}
			}
			if (list.Count > 0)
			{
				productionOder_LineImpl_WOR.UserFields = new List<UserFieldsImpl>();
				for (int num3 = 0; num3 < list.ToList().Count; num3++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					if (dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(dataRow[list.ToList()[num3]]));
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(double) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(int) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(short) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(decimal))
					{
						userFieldsImpl.Value = Function.ParseDouble(dataRow[list.ToList()[num3]]);
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(Function.ToString(dataRow[list.ToList()[num3]]));
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = list.ToList()[num3];
					productionOder_LineImpl_WOR.UserFields.Add(userFieldsImpl);
				}
			}
			list3.Add(productionOder_LineImpl_WOR);
		}
		return list3;
	}
}
