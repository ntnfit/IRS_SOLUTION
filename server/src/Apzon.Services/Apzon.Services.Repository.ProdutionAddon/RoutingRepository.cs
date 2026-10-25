using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class RoutingRepository : BaseService<IRouting>, IRouting
{
	public RoutingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocs()
	{
		try
		{
			string query = "APZ_ProductionAddOn_Routing_GetDocs";
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

	public DataTable GetListRouting()
	{
		try
		{
			string query = "APZ_ProductionAddOn_Routing_GetListRouting";
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

	public DataTable GetDoc(string roucode)
	{
		try
		{
			string query = "APZ_ProductionAddOn_Routing_GetDoc";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@roucode", roucode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
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

	public int GetMaxLineNum(string roucode)
	{
		try
		{
			string query = "APZ_ProductionAddOn_Routing_GetMaxLineNum";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@roucode", roucode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxAbsID FAILED ", ex.Message);
			return 0;
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
				string text = null;
				string query = "APZ_ProductionAddOn_Routing_SaveDocs";
				list.Add(new SqlParameter("@dt", ds.Tables[0]));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					text = SaveDoc(ds.Tables[1]);
					if (!string.IsNullOrEmpty(text))
					{
						dbTransaction.Rollback();
						return text;
					}
					dbTransaction.Commit();
					return "";
				}
				return "";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public string SaveDoc(DataTable dt)
	{
		try
		{
			if (dt != null && dt.Rows.Count > 0)
			{
				using (DatabaseService.OpenConnection())
				{
					using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
					List<SqlParameter> list = new List<SqlParameter>();
					string query = "APZ_ProductionAddOn_Routing_SaveDoc";
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
			return "Table is empty!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Stage  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable SearchField(DataTable dts)
	{
		try
		{
			DataRow dataRow = dts.Rows[0];
			string query = "APZ_ProductionAddOn_Routing_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@RouCode", Function.ToString(dataRow["RouCode"])));
			list.Add(new SqlParameter("@RouName", Function.ToString(dataRow["RouName"])));
			list.Add(new SqlParameter("@Status", Function.ToString(dataRow["Status"])));
			list.Add(new SqlParameter("@Type", Function.ToString(dataRow["Type"])));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Return Search - Doc ", ex.Message);
			return new DataTable();
		}
	}

	public string DeleteDocs(string ids)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				string query = "APZ_ProductionAddOn_Routing_DeleteDocs";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@ids", ids));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					if (DeleteAllDocByKeyCode(ids) == "")
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
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDocs  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public string DeleteAllDocByKeyCode(string ids)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				string query = "APZ_ProductionAddOn_Routing_DeleteDocByRouCode";
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
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDocs  FAILED ", ex.Message);
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
				string query = "APZ_ProductionAddOn_Routing_DeleteDoc";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@RouCode", Function.ToString(dataRow["RouCode"])));
				list.Add(new SqlParameter("@LineNum", Function.ToString(dataRow["LineNum"])));
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
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Stage  FAILED ", ex.Message);
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
				string query = "APZ_ProductionAddOn_Routing_DeleteDocs";
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
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDocs  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}
}
