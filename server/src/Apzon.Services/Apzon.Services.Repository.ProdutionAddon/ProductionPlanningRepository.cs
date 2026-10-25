using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class ProductionPlanningRepository : BaseService<IProductionPlanning>, IProductionPlanning
{
	public ProductionPlanningRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDataRoutingByMOCode(string MOCode)
	{
		try
		{
			string query = "APZ_ProductionPlanning_GetRoutingByMOCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@MOCode", MOCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Routing  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetReceiptByMOCode(string MOCode)
	{
		try
		{
			string query = "APZ_ProductionPlanning_GetDataReceiptByMOCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@MOCode", MOCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Data Receipt  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAttachmentByMOCode(string MOCode)
	{
		try
		{
			string query = "APZ_ProductionPlanning_GetDataReceiptByMOCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@MOCode", MOCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Data Receipt  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public int GetMaxMOCode()
	{
		try
		{
			string query = "APZ_ProductionAddon_ProductionPlanning_GetMaxMONo";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable == null)
			{
				return 0;
			}
			return Function.ParseInt(dataTable.Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Data Receipt  FAILED ", ex.Message);
			return 0;
		}
	}

	public DataTable GetUomByItemCode(string itemcode)
	{
		try
		{
			string query = "APZ_ProductionAddon_ProductionPlanning_GetUomByItemCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemcode", itemcode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Data Receipt  FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable GetMaxMONO()
	{
		try
		{
			string query = "APZ_ProductionAddon_ProductionPlanning_GetMaxMONO";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public string Save(DataSet ds)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				string text = null;
				string query = "APZ_ProductionAddon_ProductionPlanning_Save";
				list.Add(new SqlParameter("@dt", ds.Tables[0]));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					string value = SaveRouting(ds.Tables[1]);
					text = SaveReceipt(ds.Tables[2]);
					if (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(text))
					{
						dbTransaction.Rollback();
						return "ERROR";
					}
					dbTransaction.Commit();
					return "";
				}
				return "Error!";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public string SaveRouting(DataTable dt)
	{
		try
		{
			if (dt != null && dt.Rows.Count > 0)
			{
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					List<SqlParameter> list = new List<SqlParameter>();
					string query = "APZ_ProductionAddon_ProductionPlanning_SaveRouting";
					list.Add(new SqlParameter("@dt", dt));
					if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
					{
						dbTransaction.Commit();
						return "";
					}
					dbTransaction.Rollback();
					return "ERROR";
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveReceipt(DataTable dt)
	{
		try
		{
			if (dt != null && dt.Rows.Count > 0)
			{
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					List<SqlParameter> list = new List<SqlParameter>();
					string query = "APZ_ProductionAddon_ProductionPlanning_SaveReceipt";
					list.Add(new SqlParameter("@dt", dt));
					if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
					{
						dbTransaction.Commit();
						return "";
					}
					dbTransaction.Rollback();
					return "ERROR";
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetMaxLineNumByMONo(string MONo)
	{
		try
		{
			string query = "APZ_ProductionAddon_ProductionPlanning_GetMaxLineNumByMONo";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@MONo", MONo));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable GetTableSQL()
	{
		try
		{
			string query = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='APZ_PPL1'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable Search(ProductionPlanningImpl plSearch)
	{
		try
		{
			string query = "APZ_ProductionAddon_ProductionPlanning_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", plSearch.ItemCode));
			list.Add(new SqlParameter("@ItemName", plSearch.ItemName));
			list.Add(new SqlParameter("@FromDate", plSearch.FromDate));
			list.Add(new SqlParameter("@ToDate", plSearch.ToDate));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable GetReceipt(int moNo)
	{
		try
		{
			string query = "APZ_ProductionPlanning_GetDataReceiptByMOCode2";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@moNo", moNo));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable GetDocRecpt(int MONo)
	{
		try
		{
			string query = "APZ_ProductionAddOn_Routing_GetDocRecpt";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@MONo", MONo));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable GetPlan(int type)
	{
		try
		{
			string query = "APZ_ProcductionAddOn_ProductionPlanning_Planed";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@type", type));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable GetRelease()
	{
		try
		{
			string query = "APZ_ProductionPlanning_GetRelease";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable GetBatchByItemCode(string itemcode)
	{
		try
		{
			string query = "APZ_ProductionControl_GetBatchByItemCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemcode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable ChooseWareHouseByItemCode(string itemcode)
	{
		try
		{
			string query = "APZ_ProductionAddon_ChooseWareHouseByItemCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemcode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable APZ_ProductionAddOn_GetRoutingByRouCode(string Roucode)
	{
		try
		{
			string query = "APZ_ProductionAddOn_GetRoutingByRouCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Roucode", Roucode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable APZ_ProductionPlanning_GetDataReceiptByMOCode(DataTable dta)
	{
		try
		{
			string query = "APZ_ProductionPlanning_GetDataReceiptByMOCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Docentry", dta.Rows[0][0]));
			list.Add(new SqlParameter("@LineNum", dta.Rows[0][1]));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public string CheckControlKey(string keyCode)
	{
		try
		{
			string query = "APZ_ProductionPlanning_CheckControlKey";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@keyCode", keyCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return null;
			}
			return "Error";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public string GetCtrlKeyByResCode(string resCode)
	{
		try
		{
			string query = "APZ_ProductionPlanning_GetCtrlKeyByResCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ResCode", resCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "Error";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable APZ_ProductionAddon_ProductionPlanning_GetFromAPZ_PPL1ByMoNoToCreatPlan(int MONo)
	{
		try
		{
			string query = "APZ_ProductionAddon_ProductionPlanning_GetFromAPZ_PPL1ByMoNoToCreatPlan";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@MONo", MONo));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable GetClosed()
	{
		try
		{
			string query = "APZ_ProductionPlanning_GetClosed";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}

	public DataTable APZ_ProductionPlanning_GetBatchCompleteByBatIssue()
	{
		try
		{
			string query = "APZ_ProductionPlanning_GetBatchCompleteByBatIssue";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return null;
		}
	}
}
