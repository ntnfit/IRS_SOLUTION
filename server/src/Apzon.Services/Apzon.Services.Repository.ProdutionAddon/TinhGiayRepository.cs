using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class TinhGiayRepository : BaseService<ITinhGiay>, ITinhGiay
{
	public TinhGiayRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable APZ_ProductionAddon_ListItemTinhGiay(DataTable tbl)
	{
		try
		{
			string query = "APZ_ProductionAddon_ListItemTinhGiay";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@SO", tbl.Rows[0][0]));
			list.Add(new SqlParameter("@LineSO", tbl.Rows[0][1]));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "APZ_ProductionAddon_ListItemTinhGiay  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable APZ_ProductionAddon_ListWarehouse()
	{
		try
		{
			string query = "APZ_ProductionAddon_ListWarehouse";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "APZ_ProductionAddon_ListWarehouse  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable APZ_ProductionAddon_ListCuonPhuHop(DataTable dt)
	{
		try
		{
			string query = "APZ_ProductionAddon_ListCuonPhuHop";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Whse", dt.Rows[0][0]));
			list.Add(new SqlParameter("@SO", dt.Rows[0][1]));
			list.Add(new SqlParameter("@LineSO", dt.Rows[0][2]));
			list.Add(new SqlParameter("@Batch", dt.Rows[0][3]));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "APZ_ProductionAddon_ListCuonPhuHop  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string APZ_ProductionAddon_SaveDSTinhGiay(DataTable dt)
	{
		try
		{
			dt.TableName = "apz_YCTinhGiay";
			dt.Columns.Add("Test", typeof(string));
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				dt.Rows[i]["Test"] = "abc";
			}
			return "Error";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "APZ_ProductionAddon_SaveDSTinhGiay  FAILED ", ex.Message);
			return "error!";
		}
	}

	public DataTable TinhGhepKho(DataTable dtTinhGiay)
	{
		try
		{
			string query = "APZ_SaleQuotation_TinhGhepKho";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tblTinhGiay", dtTinhGiay));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "TinhGhepKho  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet TinhChonGiay(DataTable dtGhepKho, string whs)
	{
		try
		{
			string query = "APZ_SaleQuotation_TinhChonGiay";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tblGhepKho", dtGhepKho));
			list.Add(new SqlParameter("@whs", whs));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "TinhChonGiay  FAILED ", ex.Message);
		}
		return new DataSet();
	}

	public DataSet XemTonKho(DataTable dtGhepKho, string branch, string statusQC)
	{
		try
		{
			string query = "APZ_SaleQuotation_XemTonKho";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tblGhepKho", dtGhepKho));
			list.Add(new SqlParameter("@branch", branch));
			list.Add(new SqlParameter("@statusQC", statusQC));
			if (dtGhepKho.IsNotNull() && dtGhepKho.ExistsColumn("Joins"))
			{
				list.Add(new SqlParameter("@joins", Function.ToString(dtGhepKho.Rows[0]["Joins"])));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "XemTonKho  FAILED ", ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetBatchByItemCode(string itemCode)
	{
		try
		{
			string query = "APZ_ProductionAddon_GetBatchByItemCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), $"APZ_ProductionAddon_GetBatchByItemCode {itemCode} FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetWarehouseForBatch(int branchId)
	{
		try
		{
			string query = "APZ_SaleQuotation_WarehouseForBatch";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@branch", branchId));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "APZ_SaleQuotation_WarehouseForBatch  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string FilterTinhGiay(DataTable dtGhepKho)
	{
		try
		{
			string query = "APZ_SaleQuotation_FilterTinhGiay";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@tblTinhGiay", dtGhepKho));
			return ExecuteScalar(query, CommandType.StoredProcedure, list.ToArray()).ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "FilterTinhGiay  FAILED ", ex.Message);
		}
		return "";
	}
}
