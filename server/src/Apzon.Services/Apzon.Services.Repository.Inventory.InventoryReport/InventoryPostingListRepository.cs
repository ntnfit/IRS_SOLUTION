using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Inventory.InventoryReport;

namespace Apzon.Services.Repository.Inventory.InventoryReport;

public class InventoryPostingListRepository : BaseService<DataSet>, IInventoryPostingList
{
	public InventoryPostingListRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData(DataTable dt)
	{
		try
		{
			return ExecuteDataTable("apz_inventory_posting_list", CommandType.StoredProcedure, new SqlParameter("@ItmFrom", Function.ToString(dt.Rows[0]["ItemFrom"])), new SqlParameter("@ItmTo", Function.ToString(dt.Rows[0]["ItemTo"])), new SqlParameter("@BpFrom", Function.ToString(dt.Rows[0]["BpFrom"])), new SqlParameter("@BpTo", Function.ToString(dt.Rows[0]["BpTo"])), new SqlParameter("@DateFrom", Function.ParseDateTime(dt.Rows[0]["DateFrom"])), new SqlParameter("@DateTo", Function.ParseDateTime(dt.Rows[0]["DateTo"])), new SqlParameter("@WhsCode", Function.ToString(dt.Rows[0]["WhsCode"])));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
