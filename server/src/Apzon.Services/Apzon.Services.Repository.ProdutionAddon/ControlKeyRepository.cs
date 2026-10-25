using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class ControlKeyRepository : BaseService<IControlKey>, IControlKey
{
	public ControlKeyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocs()
	{
		try
		{
			string query = "APZ_ProductionAddOn_ControlKey_GetDocs";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
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

	public int GetMaxIndicatorID(string keycode)
	{
		try
		{
			string query = "APZ_ProductionAddOn_ControlKey_GetMaxIndicator";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@keycode", keycode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxIndicatorID FAILED ", ex.Message);
			return -1;
		}
	}

	public DataTable GetDoc()
	{
		try
		{
			string query = "APZ_ProductionAddOn_ControlKey_GetDoc";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDocSaved(string keycode)
	{
		try
		{
			string query = "APZ_ProductionAddOn_ControlKey_SaveDocSaved";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@KeyCode", keycode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveDocs(DataSet ds)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				string query = "APZ_ProductionAddOn_ControlKey_SaveDocs";
				list.Add(new SqlParameter("@dt", ds.Tables[0]));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					if (!string.IsNullOrEmpty(SaveDoc(ds.Tables[1])))
					{
						dbTransaction.Rollback();
						return "ERROR";
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
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveDocs  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public string SaveDoc(DataTable dt)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				if (dt != null && dt.Rows.Count > 0)
				{
					List<SqlParameter> list = new List<SqlParameter>();
					string query = "APZ_ProductionAddOn_ControlKey_SaveDoc";
					list.Add(new SqlParameter("@dt", dt));
					if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
					{
						dbTransaction.Commit();
						return "";
					}
					dbTransaction.Rollback();
					return "ERROR";
				}
				dbTransaction.Dispose();
				return "Table is empty";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveDoc  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public string DeleteDoc(DataTable dts)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				DataRow dataRow = dts.Rows[0];
				string query = "APZ_ProductionAddOn_ControlKey_DeleteDoc";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@KeyCode", Function.ToString(dataRow["KeyCode"])));
				list.Add(new SqlParameter("@IndicatorID", Function.ToString(dataRow["IndicatorID"])));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
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
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDoc  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public DataTable GetListKeyCode()
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "APZ_ProductionAddOn_ControlKey_GetListKeyCode";
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return new DataTable();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string DeleteDocs(string keycode)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				string query = "DELETE FROM APZ_OCTK WHERE KeyCode = '" + keycode + "'";
				if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
				{
					if (DeleteAllDocByKeyCode(keycode) == "")
					{
						dbTransaction.Commit();
						return "";
					}
					dbTransaction.Rollback();
					return "No rows deleted";
				}
				dbTransaction.Rollback();
				return "No rows deleted";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListKeyCode  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public DataTable CheckExistKeyCode(string keycode)
	{
		try
		{
			string query = "SELECT COUNT(Ctrlkey) AS ExistRouting FROM dbo.APZ_ORSC WHERE Ctrlkey='" + keycode + "' \n SELECT COUNT(ControlKey) AS ExistProductionPlanning FROM dbo.APZ_PPL1 where ControlKey='" + keycode + "'";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetListKeyCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string DeleteAllDocByKeyCode(string keycode)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				string query = "delete from APZ_CTK1 where KeyCode = '" + keycode + "'";
				if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "No rows deleted";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteAllDocByKeyCode  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public string DeleteDocsAlone(string ids)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				string query = "APZ_ProductionAddOn_ControlKey_DeleteDocs";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@ids", ids));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "No rows deleted";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}
}
