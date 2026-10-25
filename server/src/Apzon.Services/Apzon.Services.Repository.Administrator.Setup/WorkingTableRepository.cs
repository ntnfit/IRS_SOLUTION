using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class WorkingTableRepository : BaseService<DataTable>, IWorkingTable
{
	public WorkingTableRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetWorkingTableLine(string id)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "SP_APZ_WorkingTable_GetWorkingTableLine";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", Function.ParseInt(id)));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetWorkingTableDoc(string type, string user)
	{
		DataSet result = new DataSet();
		try
		{
			result = type switch
			{
				"M" => GetWorkingTableDocMyRQ(user), 
				"A" => GetWorkingTableDocAll(user), 
				"P" => GetWorkingTableDocPending(user), 
				_ => GetWorkingTableDocAll(user), 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetWorkingTableDocMyRQ(string user)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "SP_APZ_WorkingTable_GetWorkingTableDocMyRQ";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserCode", user));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetWorkingTableDocAll(string user)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "SP_APZ_WorkingTable_GetWorkingTableDocAll";
			result = ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetWorkingTableDocPending(string user)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "SP_APZ_WorkingTable_GetWorkingTableDocPending";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserCode", user));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetDataStepStatus(string id)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "SP_APZ_WorkingTable_GetDataStepStatus";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", Function.ParseInt(id)));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetUserSettings(string username)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_WorkingTable_GetUserSettings";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@user_code", username));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public HttpResult GetTopDraft(string username)
	{
		string message = "";
		DataTable obj = new DataTable();
		try
		{
			string query = "SP_APZ_WorkingTable_GetTopDraft";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserCode", username));
			obj = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			message = "ERR: Exception occured: " + ex.Message;
		}
		return new HttpResult(Enums.MessageCode.Success, message, obj);
	}

	public string MarkReadMessage(int docEntry, string userCode)
	{
		try
		{
			string query = "SP_APZ_WorkingTable_MarkReadMessage";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", docEntry));
			list.Add(new SqlParameter("@UserCode", userCode));
			ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray());
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult GetDraftApprovalStatus(int draftKey)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"WddStatus\",\"DocStatus\" FROM ODRF WHERE \"DocEntry\"=@DraftKey;", CommandType.Text, new SqlParameter("@DraftKey", draftKey));
			if (dataTable.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Success, "", $"{Function.ToString(dataTable.Rows[0][0])},{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, Messages.D404);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
