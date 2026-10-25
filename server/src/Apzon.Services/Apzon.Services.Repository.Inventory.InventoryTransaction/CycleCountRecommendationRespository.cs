using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class CycleCountRecommendationRespository : BaseService<DataTable>, ICycleCountRecommendation
{
	private const string Doctemp = "##tempdoc";

	private const string Linetemp = "##templine";

	public CycleCountRecommendationRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetListCycleCode()
	{
		try
		{
			string listCycleCodeQuery = GetListCycleCodeQuery();
			return ExecuteDataTable(listCycleCodeQuery, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetListCycleCodeQuery()
	{
		return "select Code,Name from OCYC order by Code ASC";
	}

	public DataTable GetListItemCode()
	{
		try
		{
			return ExecuteDataTable("APZ_Cycle_ItemCode", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetWarehouse()
	{
		try
		{
			return ExecuteDataTable("APZ_Cycle_GetWareHouse", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListDataLine(string whs, int fromCycleCode, int toCycleCode, string fromItemCode, string toItemCode)
	{
		try
		{
			return ExecuteDataTable("Apz_Cycle_GetListDataline", CommandType.StoredProcedure, new SqlParameter("@whs", whs), new SqlParameter("@fromCycle", fromCycleCode), new SqlParameter("@toCycle", toCycleCode), new SqlParameter("@fromItem", fromItemCode), new SqlParameter("@toItem", toItemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
