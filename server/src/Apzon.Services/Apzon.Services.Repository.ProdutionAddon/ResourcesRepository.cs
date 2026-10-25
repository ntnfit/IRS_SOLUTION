using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class ResourcesRepository : BaseService<IResources>, IResources
{
	public ResourcesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocs()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_ProductionAddOnResources_GetDocs", CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetMaxLineNumByResCode(string resCode)
	{
		try
		{
			string query = "APZ_ProductionAddOnResources_GetMaxLineNumByResCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ResCode", resCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetCapacityChildren(string rescode)
	{
		try
		{
			string query = "APZ_ProductionAddOnResources_GetDoc_Capacity_GetCapacity";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ResCode", rescode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDocTabFix(string rescode)
	{
		try
		{
			string query = "APZ_ProductionAddOnResources_GetDoc_FixedAsset";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ResCode", rescode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc Tab Fix  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocTabCosting(string rescode)
	{
		try
		{
			string query = "APZ_ProductionAddOnResources_GetDoc_Costing";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ResCode", rescode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListResCode()
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "APZ_ProductionAddOn_Resources_GetListResCode";
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDate(string resCode)
	{
		try
		{
			string query = "APZ_ProductionAddOnResources_GetDoc_Capacity_GetDate";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ResCode", resCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetCapacityByRes(Obj_Date obj)
	{
		try
		{
			string query = "APZ_ProductionAddOnResources_GetDoc_Capacity_GetCapacity";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ResCode", obj.ResCode));
			list.Add(new SqlParameter("@fDate", obj.validFrom));
			list.Add(new SqlParameter("@tDate", obj.validTo));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
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
				string query = "APZ_ProductionAddOnResources_SaveDocs";
				list.Add(new SqlParameter("@dt", ds.Tables[0]));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					string text = SaveDocTapCap(ds.Tables[1]);
					string text2 = SaveDocTapCosting(ds.Tables[2]);
					string text3 = SaveDocTapFixAsset(ds.Tables[3]);
					if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2) || !string.IsNullOrEmpty(text3))
					{
						dbTransaction.Rollback();
						return text + text2 + text3;
					}
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Dispose();
				return "Error!";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				return ex.Message;
			}
		}
	}

	public string SaveDocTapCap(DataTable dt)
	{
		try
		{
			if (dt != null && dt.Rows.Count > 0)
			{
				List<SqlParameter> list = new List<SqlParameter>();
				string query = "APZ_ProductionAddOnResources_SaveDoc_Capacity";
				list.Add(new SqlParameter("@dt", dt));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					return "";
				}
				return "Can't Save Tab Capacity";
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string SaveDocTapCosting(DataTable dt)
	{
		try
		{
			if (dt != null && dt.Rows.Count > 0)
			{
				List<SqlParameter> list = new List<SqlParameter>();
				string query = "APZ_ProductionAddOnResources_SaveDoc_Costing";
				list.Add(new SqlParameter("@dt", dt));
				ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray());
				_ = 0;
				return "";
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save   FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveDocTapFixAsset(DataTable dt)
	{
		try
		{
			if (dt != null && dt.Rows.Count > 0)
			{
				List<SqlParameter> list = new List<SqlParameter>();
				string query = "APZ_ProductionAddOnResources_SaveDoc_FixAsset";
				list.Add(new SqlParameter("@dt", dt));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					return "";
				}
				return "Can't Add into Table Fixasset";
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save   FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDocs(string resCode)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				string query = "DELETE FROM dbo.APZ_ORSC WHERE ResCode = '" + resCode + "'     DELETE FROM dbo.APZ_RSC6 WHERE ResCode ='" + resCode + "'    DELETE FROM dbo.APZ_RSC7 WHERE ResCode ='" + resCode + "'      DELETE FROM dbo.APZ_RSC3 WHERE ResCode ='" + resCode + "'  ";
				if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "ERROR";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save   FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}
}
