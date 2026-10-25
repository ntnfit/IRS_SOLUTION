using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class PickAndPackRespository : BaseService<DataTable>, IPickAndPack
{
	public PickAndPackRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public int GetMaxDocEntry(string tableName)
	{
		try
		{
			string query = "SELECT ISNULL( MAX(DocEntry),0) as max from " + tableName;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["max"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxDocEntry  FAILED ", ex.Message);
		}
		return 0;
	}

	public DataSet GetRecord(int docEntry, string type)
	{
		new DataTable();
		try
		{
			string query = "APZ_PickAndPack_GetRecord";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", docEntry));
			list.Add(new SqlParameter("@Type", type));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord ", ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetBatchItem(string barcode)
	{
		try
		{
			string query = "APZ_PickAndPack_GetBatchItem";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@barcode", barcode));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBatchItem ", ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetBatchDocument(string barcode)
	{
		try
		{
			string query = "APZ_PickAndPack_GetBatchDocument";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@barcode", barcode));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBatchDocument ", ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetLineItem(int docEntry)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "apz_PickAndPack_GetById";
			result = ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@Id", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetLineItem ", ex.Message);
		}
		return result;
	}
}
