using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Opportunity;

namespace Apzon.Services.Repository.Opportunity;

public class ActivitySubjectsRepository : BaseService<DataTable>, IActivitySubjects
{
	public ActivitySubjectsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public int GetMaxActivitySubject()
	{
		try
		{
			string query = "APZ_ActivitySubjects_GetMax";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxActivitySubject  FAILED ", ex.Message);
			return -1;
		}
	}

	public DataTable GetAll(string madk)
	{
		try
		{
			string query = "APZ_ActivitySubjects_GetAll";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Type", madk));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetAll  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable ShowData(string madk)
	{
		try
		{
			string query = "APZ_ActivitySubjects_ShowData";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Type", madk));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowData  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetUserSignByUserCode_tableOUSR(string madk)
	{
		try
		{
			string query = "APZ_ActivitySubjects_GetUserSignByUserCodeOUSR";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserCode", madk));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUserSignByUserCode_tableOUSR  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveDoc(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oActivitySubject, -1, Enums.TransactionType.A, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			httpResult.Message = ProcessChildData(dt, "APZ_ActivitySubjects_SaveDoc", "#dtAcSubject");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	private string ProcessChildData(DataTable dt, string store, string tempTable)
	{
		try
		{
			CreateTempTable(dt, tempTable);
			BulkCopy(dt, tempTable);
			DataTable dataTable = ExecuteDataTable(store, CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(int code)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "APZ_ActivitySubjects_DeleteDoc";
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@code", code));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return Function.ToString(dataTable.Rows[0][0]);
				}
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
}
