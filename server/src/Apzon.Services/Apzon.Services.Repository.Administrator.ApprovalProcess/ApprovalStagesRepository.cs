using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Administrator.ApprovalProcess;

namespace Apzon.Services.Repository.Administrator.ApprovalProcess;

public class ApprovalStagesRepository : BaseService<DataTable>, IApprovalStages
{
	public ApprovalStagesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData()
	{
		try
		{
			return ExecuteDataTable("select * from OWST;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataLines(int stageCode)
	{
		try
		{
			string query = $"select a.\"WstCode\", a.\"UserID\", b.\"U_NAME\" as \"UserName\", c.\"Code\" as \"DepartmntCode\", c.\"Name\" as \"Departmnt\" from WST1 a\r\n                                                inner join OUSR b on a.\"UserID\" = b.USERID\r\n                                                inner join OUDP c on b.\"Department\" = c.\"Code\"\r\n                                                where a.\"WstCode\" = @stageCode;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@stageCode", Function.ParseInt(stageCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchData(int stageCode, string stageName)
	{
		try
		{
			string value = ((stageCode < 1) ? "" : Function.ToString(stageCode));
			return ExecuteDataTable("APZ_ApprovalStages_SearchData", CommandType.StoredProcedure, new SqlParameter("@stageCode", value), new SqlParameter("@stageName", Function.ToString(stageName)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetNewCodeQuery()
	{
		return "select ISNULL(MAX(WstCode), 0) + 1 from OWST";
	}

	public int GetNewCode()
	{
		try
		{
			string newCodeQuery = GetNewCodeQuery();
			return Function.ParseInt(ExecuteDataTable(newCodeQuery, CommandType.Text).Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return -1;
		}
	}

	public DataTable GetUserInfo(string userName)
	{
		try
		{
			string query = "select a.USERID, a.USER_CODE as \"UserCode\", a.U_NAME as \"UserName\", b.\"Code\" as \"DepartmntCode\", b.\"Name\" as \"DepartmntName\" from OUSR a\r\n                                inner join OUDP b on a.\"Department\" = b.\"Code\"\r\n                                where a.U_NAME = @userName;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@userName", Function.ToString(userName)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRecord(string position, string stageCode)
	{
		try
		{
			return ExecuteDataTable("APZ_ApprovalStages_GetRecord", CommandType.StoredProcedure, new SqlParameter("@position", position), new SqlParameter("@stageCode", stageCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
