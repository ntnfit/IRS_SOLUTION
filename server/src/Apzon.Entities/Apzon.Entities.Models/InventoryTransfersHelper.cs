using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Apzon.Commons;
using Apzon.Entities.Models.Inventory;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models;

public static class InventoryTransfersHelper
{
	public static InventoryTransferTable InventoryTransfers { get; set; }

	public static InventoryTransferLineTable InventoryTransferLines { get; set; }

	public static InventoryTranferImpl ToTransferDoc(this DataRow dr)
	{
		InventoryTranferImpl inventoryTranferImpl = new InventoryTranferImpl();
		try
		{
			List<string> list = (from DataColumn c in dr.Table.Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(InventoryTranferImpl).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (list.Contains(propertyInfo.Name))
				{
					try
					{
						PropertyInfo property = inventoryTranferImpl.GetType().GetProperty(propertyInfo.Name);
						propertyInfo.SetValue(inventoryTranferImpl, (dr[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(dr[propertyInfo.Name], property.PropertyType));
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				}
			}
			IEnumerable<string> enumerable = list.Where((string t) => t.StartsWith("U_"));
			if (enumerable != null && enumerable.ToList() != null && enumerable.ToList().Count > 0)
			{
				inventoryTranferImpl.UserFields = new List<UserFieldsImpl>();
				for (int num2 = 0; num2 < enumerable.ToList().Count; num2++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Name = enumerable.ToList()[num2];
					userFieldsImpl.Value = Function.ToString(dr[enumerable.ToList()[num2]]);
					Type dataType = dr.Table.Columns[enumerable.ToList()[num2]].DataType;
					if (dataType == typeof(DateTime))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (dataType == typeof(float))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Float;
					}
					else if (dataType == typeof(double))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					inventoryTranferImpl.UserFields.Add(userFieldsImpl);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			throw;
		}
		return inventoryTranferImpl;
	}

	public static TranferRequestImpl ToRequestDoc(this DataRow dr)
	{
		TranferRequestImpl tranferRequestImpl = new TranferRequestImpl();
		try
		{
			List<string> list = (from DataColumn c in dr.Table.Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(TranferRequestImpl).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (list.Contains(propertyInfo.Name))
				{
					try
					{
						PropertyInfo property = tranferRequestImpl.GetType().GetProperty(propertyInfo.Name);
						propertyInfo.SetValue(tranferRequestImpl, (dr[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(dr[propertyInfo.Name], property.PropertyType));
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				}
			}
			IEnumerable<string> enumerable = list.Where((string t) => t.StartsWith("U_"));
			if (enumerable != null && enumerable.ToList() != null && enumerable.ToList().Count > 0)
			{
				tranferRequestImpl.UserFields = new List<UserFieldsImpl>();
				for (int num2 = 0; num2 < enumerable.ToList().Count; num2++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Name = enumerable.ToList()[num2];
					userFieldsImpl.Value = Function.ToString(dr[enumerable.ToList()[num2]]);
					Type dataType = dr.Table.Columns[enumerable.ToList()[num2]].DataType;
					if (dataType == typeof(DateTime))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (dataType == typeof(float))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Float;
					}
					else if (dataType == typeof(double))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					tranferRequestImpl.UserFields.Add(userFieldsImpl);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			throw;
		}
		return tranferRequestImpl;
	}

	public static List<InventoryTranfer_LineImpl> ToTransferLine(this InventoryTransferLineTable dt)
	{
		try
		{
			List<string> columnNames = (from DataColumn c in dt.Columns
				select c.ColumnName).ToList();
			IEnumerable<string> udfCols = columnNames.Where((string t) => t.StartsWith("U_"));
			PropertyInfo[] properties = typeof(InventoryTranfer_LineImpl).GetProperties();
			return dt.AsEnumerable().Select((DataRow row) =>
			{
				InventoryTranfer_LineImpl inventoryTranfer_LineImpl = new InventoryTranfer_LineImpl();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (columnNames.Contains(propertyInfo.Name))
					{
						PropertyInfo property = inventoryTranfer_LineImpl.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((row[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(row[propertyInfo.Name], conversionType));
							propertyInfo.SetValue(inventoryTranfer_LineImpl, value, null);
						}
					}
				}
				if (udfCols != null && udfCols.ToList() != null && udfCols.ToList().Count > 0)
				{
					inventoryTranfer_LineImpl.UserFields = new List<UserFieldsImpl>();
					for (int j = 0; j < udfCols.ToList().Count; j++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(DateTime))
						{
							userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(row[udfCols.ToList()[j]]));
							userFieldsImpl.Type = BoFieldTypes.db_Date;
						}
						else if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(double))
						{
							userFieldsImpl.Value = Function.ToString(Function.ParseDouble(row[udfCols.ToList()[j]]));
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							userFieldsImpl.Value = Function.ToString(Function.ToString(row[udfCols.ToList()[j]]));
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						}
						userFieldsImpl.Name = udfCols.ToList()[j];
						inventoryTranfer_LineImpl.UserFields.Add(userFieldsImpl);
					}
				}
				return inventoryTranfer_LineImpl;
			}).ToList();
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static List<TranferRequest_LineImpl> ToRequestLine(this InventoryTransferLineTable dt)
	{
		try
		{
			List<string> columnNames = (from DataColumn c in dt.Columns
				select c.ColumnName).ToList();
			IEnumerable<string> udfCols = columnNames.Where((string t) => t.StartsWith("U_"));
			PropertyInfo[] properties = typeof(TranferRequest_LineImpl).GetProperties();
			return dt.AsEnumerable().Select((DataRow row) =>
			{
				TranferRequest_LineImpl tranferRequest_LineImpl = new TranferRequest_LineImpl();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (columnNames.Contains(propertyInfo.Name))
					{
						PropertyInfo property = tranferRequest_LineImpl.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((row[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(row[propertyInfo.Name], conversionType));
							propertyInfo.SetValue(tranferRequest_LineImpl, value, null);
						}
					}
				}
				if (udfCols != null && udfCols.ToList() != null && udfCols.ToList().Count > 0)
				{
					tranferRequest_LineImpl.UserFields = new List<UserFieldsImpl>();
					for (int j = 0; j < udfCols.ToList().Count; j++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(DateTime))
						{
							userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(row[udfCols.ToList()[j]]));
							userFieldsImpl.Type = BoFieldTypes.db_Date;
						}
						else if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(double))
						{
							userFieldsImpl.Value = Function.ToString(Function.ParseDouble(row[udfCols.ToList()[j]]));
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							userFieldsImpl.Value = Function.ToString(Function.ToString(row[udfCols.ToList()[j]]));
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						}
						userFieldsImpl.Name = udfCols.ToList()[j];
						tranferRequest_LineImpl.UserFields.Add(userFieldsImpl);
					}
				}
				return tranferRequest_LineImpl;
			}).ToList();
		}
		catch (Exception)
		{
			throw;
		}
	}
}
