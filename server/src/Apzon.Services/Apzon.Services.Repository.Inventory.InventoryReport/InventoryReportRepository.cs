using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Inventory.InventoryReport;

namespace Apzon.Services.Repository.Inventory.InventoryReport;

public class InventoryReportRepository : BaseService<DataSet>, IInventoryReport
{
	public InventoryReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet StockMovementSummary(DataTable dt)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			string[] array = Function.ToString(dt.Rows[0]["Item"]).Split(',');
			foreach (string value in array)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = value;
				dataTable.Rows.Add(dataRow);
			}
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@branch", Function.ToString(dt.Rows[0]["Branch"])));
			list.Add(new SqlParameter("@fdate", Function.ParseDateTime(dt.Rows[0]["DateFrom"])));
			list.Add(new SqlParameter("@tdate", Function.ParseDateTime(dt.Rows[0]["DateTo"])));
			list.Add(new SqlParameter("@itms", dataTable));
			list.Add(new SqlParameter("@whs", Function.ToString(dt.Rows[0]["WhsCode"])));
			list.Add(new SqlParameter("@filter", Function.ToString(dt.Rows[0]["Filter"])));
			return ExecuteData("APZ_RPT_StockMovementSummary", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}
}
