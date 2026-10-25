using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models;

public static class BussinessPartnerTableHelper
{
	public static DataTable BusinessPartner { get; set; }

	public static DataTable BPAddresses { get; set; }

	public static DataTable ContactEmployees { get; set; }

	public static BPImpls ToBusinessPartner(this DataRow dr)
	{
		BPImpls bPImpls = new BPImpls();
		try
		{
			List<string> list = (from DataColumn c in dr.Table.Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(BPImpls).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!list.Contains(propertyInfo.Name))
				{
					continue;
				}
				try
				{
					PropertyInfo property = bPImpls.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						Type type = property.PropertyType;
						if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
						{
							type = Nullable.GetUnderlyingType(type);
						}
						propertyInfo.SetValue(bPImpls, (dr[propertyInfo.Name] == DBNull.Value || string.IsNullOrWhiteSpace(Function.ToString(dr[propertyInfo.Name]))) ? null : Convert.ChangeType(dr[propertyInfo.Name], type));
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
				bPImpls.ListUserFields = new List<UserFieldsImpl>();
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
					bPImpls.ListUserFields.Add(userFieldsImpl);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			throw;
		}
		return bPImpls;
	}

	public static List<ContactPersonImpl> ToContactEmployees(this DataTable dt)
	{
		List<string> list = (from t in (from DataColumn c in dt.Columns
				select c.ColumnName).ToList()
			where !t.StartsWith("U_")
			select t.ToUpper()).ToList();
		PropertyInfo[] properties = typeof(ContactPersonImpl).GetProperties();
		List<ContactPersonImpl> list2 = new List<ContactPersonImpl>();
		for (int num = 0; num < dt.Rows.Count; num++)
		{
			DataRow dataRow = dt.Rows[num];
			ContactPersonImpl contactPersonImpl = new ContactPersonImpl();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (list.Contains(propertyInfo.Name.ToUpper()))
				{
					PropertyInfo property = contactPersonImpl.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
						object value = ((dataRow[propertyInfo.Name] == DBNull.Value || string.IsNullOrEmpty(Function.ToString(dataRow[propertyInfo.Name]))) ? null : Convert.ChangeType(dataRow[propertyInfo.Name], conversionType));
						propertyInfo.SetValue(contactPersonImpl, value, null);
					}
				}
			}
			list2.Add(contactPersonImpl);
		}
		return list2;
	}

	public static List<AddressesImpl> ToBpAddresses(this DataTable dt)
	{
		List<string> source = (from DataColumn c in dt.Columns
			select c.ColumnName).ToList();
		List<string> list = (from t in source
			where !t.StartsWith("U_")
			select t.ToUpper()).ToList();
		List<string> list2 = source.Where((string t) => t.StartsWith("U_")).ToList();
		PropertyInfo[] properties = typeof(AddressesImpl).GetProperties();
		List<AddressesImpl> list3 = new List<AddressesImpl>();
		for (int num = 0; num < dt.Rows.Count; num++)
		{
			DataRow dataRow = dt.Rows[num];
			AddressesImpl addressesImpl = new AddressesImpl();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (list.Contains(propertyInfo.Name.ToUpper()))
				{
					PropertyInfo property = addressesImpl.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
						object value = ((dataRow[propertyInfo.Name] == DBNull.Value || string.IsNullOrEmpty(Function.ToString(dataRow[propertyInfo.Name]))) ? null : Convert.ChangeType(dataRow[propertyInfo.Name], conversionType));
						propertyInfo.SetValue(addressesImpl, value, null);
					}
				}
			}
			if (list2.Count > 0)
			{
				addressesImpl.UserFields = new List<UserFieldsImpl>();
				for (int num3 = 0; num3 < list2.Count; num3++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl
					{
						Name = list2[num3]
					};
					if (dt.Columns[list2[num3]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ParseDateTime(dt.Rows[num][list2[num3]]);
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (dt.Columns[list2[num3]].DataType == typeof(double) || dt.Columns[list2[num3]].DataType == typeof(decimal))
					{
						userFieldsImpl.Value = Function.ParseDouble(dt.Rows[num][list2[num3]]);
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(dt.Rows[num][list2[num3]]);
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					addressesImpl.UserFields.Add(userFieldsImpl);
				}
			}
			list3.Add(addressesImpl);
		}
		return list3;
	}
}
