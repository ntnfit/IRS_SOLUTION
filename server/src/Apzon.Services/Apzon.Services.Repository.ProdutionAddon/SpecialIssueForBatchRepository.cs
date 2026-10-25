using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class SpecialIssueForBatchRepository : BaseService<ISpecialIssueForBatch>, ISpecialIssueForBatch
{
	public SpecialIssueForBatchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable InsertRowByBatch(string batIssue)
	{
		try
		{
			string query = "APZ_ProductionAddon_CheckBarCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@batIssue", batIssue));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "InsertRowByBatch  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable InsertRowByBatchToReceipt(string batIssue)
	{
		try
		{
			string query = "APZ_ProductionAddon_CheckBarCodeToGoodReceipt";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@BatIssue", batIssue));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "InsertRowByBatch  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string UpdateQtyByItemCode(double qty, int docEntry, double Complete)
	{
		try
		{
			string query = "UPDATE WOR1 SET plannedQty= " + qty + "  WHERE ItemCode= (Select top 1 Code from [@OSET]) \nUPDATE dbo.OWOR SET PlannedQty ='" + Complete + "'  WHERE DocEntry= '" + docEntry + "'";
			new List<SqlParameter>();
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
			{
				return null;
			}
			return "error!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "InsertRowByBatch  FAILED ", ex.Message);
			return "error!";
		}
	}
}
