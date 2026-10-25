using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Inventory;
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class InventoryCoutingBinRespository : BaseService<InventoryCoutingBinImpl>, IInventoryCoutingBin
{
	public InventoryCoutingBinRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable SearchDataByBarCode(string barcode, string whs, string bin)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_InventoryCountingBin_GetBatchItem";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@barcode", barcode), new SqlParameter("@whs", whs), new SqlParameter("@bin", bin));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDataByBarCode ", ex.Message);
		}
		return result;
	}

	public string GetRecord(int docEntry, int type)
	{
		DataTable dataTable = new DataTable();
		int num = 0;
		try
		{
			string query = "APZ_InventoryCoutingBin_GetRecord_NextLastFirtPrev";
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry), new SqlParameter("@Type", type));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				num = dataTable.Rows[0].Field<int>("DocEntry");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord ", ex.Message);
		}
		return num.ToString();
	}

	public DataSet TransferGet(string batchNo, string bin)
	{
		try
		{
			return ExecuteData("apz_countingbin_gettransfer", CommandType.StoredProcedure, new SqlParameter("@BatchNo", batchNo), new SqlParameter("@Bin", bin));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable LoadAllBin(string itemCode, string whsCode, int batchSerialAbs, int tranType, string binCode)
	{
		try
		{
			return ExecuteDataTable("apz_inventorybin_allbin", CommandType.StoredProcedure, new SqlParameter("@whsCode", whsCode), new SqlParameter("@itemCode", itemCode), new SqlParameter("@batchSerialAbs", batchSerialAbs), new SqlParameter("@tranType", tranType), new SqlParameter("@binCode", binCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetBinInstock(string bin, string period)
	{
		try
		{
			return ExecuteData("apz_countingbin_getbin_instock", CommandType.StoredProcedure, new SqlParameter("@Bin", bin), new SqlParameter("@Period", period));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetPeriods()
	{
		try
		{
			return ExecuteDataTable("apz_countingbin_GetPeriods", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable CheckBatch(string batch, string bin, string period)
	{
		try
		{
			return ExecuteDataTable("apz_countingbin_checkbatch", CommandType.StoredProcedure, new SqlParameter("@BatchNo", batch), new SqlParameter("@Bin", bin), new SqlParameter("@Period", period));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public int GetPage(int current, int type)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_countingbin_navigation", CommandType.StoredProcedure, new SqlParameter("@Current", current), new SqlParameter("@Type", type));
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public DataSet GetById(int id)
	{
		try
		{
			return ExecuteData("apz_countingbin_GetById", CommandType.StoredProcedure, new SqlParameter("@Id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetCountingReport(string period, string bin)
	{
		try
		{
			return ExecuteData("apz_countingbin_period_report", CommandType.StoredProcedure, new SqlParameter("@Period", period), new SqlParameter("@Bin", bin));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSetResult GetDataByBarcode(string barcode, string whs, string fromWhs, int requestId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode),
				new SqlParameter("@whs", whs),
				new SqlParameter("@fromWhs", fromWhs),
				new SqlParameter("@requestId", requestId)
			};
			return new DataSetResult("", ExecuteData("Apz_InventoryTransfer_GetDataByBarcode", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public DataSetResult GetDataSummaryByBarcode(string barcode, string whs, string fromWhs)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode),
				new SqlParameter("@whs", whs),
				new SqlParameter("@fromWhs", fromWhs)
			};
			return new DataSetResult("", ExecuteData("Apz_InventoryTransfer_GetDataSummaryByBarcode", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public DataTableResult GetWarehouseRequest(string docEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@docEntry", docEntry)
			};
			return new DataTableResult("", ExecuteDataTable("Apz_InventoryTransfer_GetWarehouseRequest", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTableResult(ex.Message);
		}
	}
}
