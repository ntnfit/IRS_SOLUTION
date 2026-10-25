using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Inventory;

namespace Apzon.Services.Repository.Inventory.InventoryReport;

public class InventoryInWareHouseRepository : BaseService<DataTable>, IInventoryInWareHouse
{
	public InventoryInWareHouseRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadComboWareHouse()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select WhsCode,WhsName from OWHS";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadComboItemGroup()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT ITMSGRPCOD,ITMSGRPNAM FROM OITB";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable Search(DataTable dtS)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_InventoryReport_InWhs";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CodeFrom", Function.ToString(dtS.Rows[0]["CodeFrom"])));
			list.Add(new SqlParameter("@CodeTo", Function.ToString(dtS.Rows[0]["CodeTo"])));
			list.Add(new SqlParameter("@Warehouse", Function.ToString(dtS.Rows[0]["Warehouse"])));
			list.Add(new SqlParameter("@ItemGroup", Function.ToString(dtS.Rows[0]["ItemGroup"])));
			list.Add(new SqlParameter("@FromDate", (!Function.ParseDateTime(dtS.Rows[0]["FromDate"]).HasValue) ? new DateTime(1990, 1, 1) : Function.ParseDateTime(dtS.Rows[0]["FromDate"]).Value));
			list.Add(new SqlParameter("@ToDate", (!Function.ParseDateTime(dtS.Rows[0]["ToDate"]).HasValue) ? new DateTime(1990, 1, 1) : Function.ParseDateTime(dtS.Rows[0]["ToDate"]).Value));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}
}
