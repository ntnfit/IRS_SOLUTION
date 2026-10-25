using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Inventory;
using Apzon.Services.Interfaces.Inventory.Batches;

namespace Apzon.Services.Repository.Inventory.Batches;

public class BatchTransactionRepository : BaseService<BatchTransactionImpl>, IBatchTransaction
{
	public BatchTransactionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocs(string distNumber, string itemfrom, string itemto, string status)
	{
		try
		{
			string query = "APZ_Inventory_BatchManagerment_BatchTransaction_GetDocs";
			if (string.IsNullOrEmpty(distNumber))
			{
				distNumber = "";
			}
			if (string.IsNullOrEmpty(itemfrom))
			{
				itemfrom = "";
			}
			if (string.IsNullOrEmpty(itemto))
			{
				itemto = "";
			}
			if (string.IsNullOrEmpty(status))
			{
				status = "-1";
			}
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@distNumber", distNumber), new SqlParameter("@ItemFrom", itemfrom), new SqlParameter("@ItemTo", itemto), new SqlParameter("@Status", Function.ParseInt(status)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDoc(string itemCode, string sysNumber, string locCode)
	{
		try
		{
			string query = "APZ_Inventory_BatchTransaction_GetDoc";
			if (string.IsNullOrEmpty(itemCode))
			{
				itemCode = "";
			}
			if (string.IsNullOrEmpty(sysNumber))
			{
				sysNumber = "";
			}
			if (string.IsNullOrEmpty(locCode))
			{
				locCode = "";
			}
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemCode", itemCode));
			list.Add(new SqlParameter("@sysNumber", sysNumber));
			list.Add(new SqlParameter("@locCode", locCode));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
			return new DataTable();
		}
	}
}
