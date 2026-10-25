using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator.Plugin;

namespace Apzon.Services.Repository.Administrator.Plugin;

public class PluginManagerRepository : BaseService<DataTable>, IPluginManager
{
	public PluginManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData()
	{
		try
		{
			return ExecuteDataTable("Select *, 'D' \"Status\" from APZ_OPIM;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string Save(DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return "Data could not empty";
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dt, "#TempPlugin");
				BulkCopy(dt, "#TempPlugin");
				string text = Function.ToString(ExecuteScalar("APZ_PluginManager_Save", CommandType.StoredProcedure));
				if (string.IsNullOrEmpty(text))
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Delete(string plugName)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				ExecuteNonQuery("Delete from APZ_OPIM where Name = @plugName", CommandType.Text, new SqlParameter("@plugName", plugName));
				dbTransaction.Commit();
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Update(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (!dt.IsNotNull())
				{
					ExecuteNonQuery("Delete FROM APZ_OPIM", CommandType.Text);
					dbTransaction.Commit();
					return "";
				}
				CreateTempTable(dt, "#TempPlugin");
				BulkCopy(dt, "#TempPlugin");
				string text = Function.ToString(ExecuteScalar("APZ_PluginManager_Update", CommandType.StoredProcedure));
				if (string.IsNullOrEmpty(text))
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
