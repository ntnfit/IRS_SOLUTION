using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class GroupControlKeyRepository : BaseService<IGroupControlKey>, IGroupControlKey
{
	public GroupControlKeyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocs()
	{
		try
		{
			string query = "APZ_GroupControlKey_GetDocs";
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

	public DataTable GetListGroupControlKey()
	{
		try
		{
			string query = "APZ_GroupControlKey_GetListGroupControlKey";
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

	public string SaveDocs(DataTable dt)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				if (dt != null && dt.Rows.Count > 0)
				{
					List<SqlParameter> list = new List<SqlParameter>();
					string query = "APZ_GroupControlKey_Save";
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
				return "Error!";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDocs  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public string DeleteDocs(string ids)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				string query = "APZ_GroupControlKey_DeleteDocs";
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
