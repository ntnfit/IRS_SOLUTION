using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class ProjectRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IProject
{
	private string TempTableTab = "#dtOPRJ";

	public HttpResult Save_OPRJ(DataTable dt)
	{
		return Process(ApzObjectTypes.oProjectCodes, -1, Enums.TransactionType.U, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			object obj = ExecuteScalar("DELETE OPRJ\r\n                                     INSERT INTO OPRJ\r\n                                     SELECT * FROM #dtOPRJ", CommandType.Text);
			objId = Function.ToString(obj);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable ShowData()
	{
		try
		{
			string query = "SELECT * FROM OPRJ ;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Show Data Project  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string CheckExistCode(string code)
	{
		string result = "";
		try
		{
			string query = "APZ_Project_ExistInDocument";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@PrjCode", code));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable GetUserSignByUserCode_tableOUSR(string madk)
	{
		try
		{
			string query = "APZ_ActivitiesMeetingLocation_GetUserSignByUserCodeOUSR";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserCode", madk));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUserSignByUserCode_tableOUSR  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LadDataMrpProject(string LstProjectCode)
	{
		try
		{
			string query = "select * from OPRJ where \"PrjCode\" in (select data from split(@prjCode, ','));";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@prjCode", LstProjectCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string DeleteProject(string code)
	{
		try
		{
			string query = "Delete OPRJ where \"PrjCode\"=@PrjCode;";
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@PrjCode", code));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public bool ExistProject(string code)
	{
		try
		{
			string query = "Select 1 From OPRJ where \"PrjCode\"=@PrjCode;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@PrjCode", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public DataTable GetStruct()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM APZ_OPRG;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}
}
